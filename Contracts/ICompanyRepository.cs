using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters,bool trackChanges);
        Task<Company> GetCompanyAsync(Guid id, bool trackChanges);
        void CreateCompany(Company company);
        Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> id, bool trackChanges);
        void DeleteCompany(Company company);
    }
}
