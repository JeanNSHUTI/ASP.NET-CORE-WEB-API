﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters ,bool trackChanges) 
        {
            var companiesWithMetaData = await _repository.Company.GetAllCompaniesAsync(companyParameters,trackChanges);

            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companiesWithMetaData);
            return (companies: companiesDto, metaData: companiesWithMetaData.MetaData);
        }

        public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges) 
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);

            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return companyToReturn;


        }

        public async Task<IEnumerable<CompanyDto>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if(ids is null)
            {
                throw new IdParametersBadRequestException();
            }

            var companyEntities = await _repository.Company.GetCompaniesByIdsAsync(ids, trackChanges);

            if(ids.Count() != companyEntities.Count())
            {
                throw new CollectionByIdsBadRequestException();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            return companiesToReturn;
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if(companyCollection is null)
            {
                throw new CompanyCollectionBadRequest();
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach( var companyEntity in companyEntities)
            {
                _repository.Company.CreateCompany(companyEntity);
            }

            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return (companies: companyCollectionToReturn, ids: ids);
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _mapper.Map(companyForUpdate, company);
            await _repository.SaveAsync();
        }

        public async Task<(CompanyForUpdateDto companyToPatch, Company CompanyEntity)> GetCompanyForPatchAsync(Guid id, bool trackChanges)
        {
            var companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (companyEntity is null)
            {
                throw new CompanyNotFoundException(id);
            }

            var companyToPatch = _mapper.Map<CompanyForUpdateDto>(companyEntity);

            return (companyToPatch, companyEntity);
        }

        public async Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company CompanyEntity)
        {
            _mapper.Map(companyToPatch, CompanyEntity);
            await _repository.SaveAsync();
        }

        private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(id);
            }
            return company;
        }
    }
}