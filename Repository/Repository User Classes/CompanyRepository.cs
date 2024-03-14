using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository_User_Classes
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) 
        {
        }

        public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters,bool trackChanges)
        {
            var companies = await FindAll(trackChanges)
                .FilterCompanies(companyParameters.Country)
                .Search(companyParameters.SearchTerm)
                .Sort(companyParameters.OrderBy)
                .ToListAsync();

            return PagedList<Company>
                .ToPagedList(companies, companyParameters.PageNumber,
                companyParameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges) 
        {
            return await FindByCondition(
                c => c.Id.Equals(companyId),
                trackChanges).SingleOrDefaultAsync();
        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public async Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            return await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
