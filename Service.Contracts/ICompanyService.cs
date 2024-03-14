using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters,bool trackChanges);
        Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);
        Task<IEnumerable<CompanyDto>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection);
        Task DeleteCompanyAsync(Guid id, bool trackChanges);
        Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);
        Task<(CompanyForUpdateDto companyToPatch, Company CompanyEntity)> GetCompanyForPatchAsync(Guid id, bool trackChanges);
        Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company CompanyEntity);
    }
}
