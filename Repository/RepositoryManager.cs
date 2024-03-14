using Contracts;
using Repository.Repository_User_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ICompanyRepository> _companyRepository;
        private readonly Lazy<IEmployeeRepository> _employeesRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _companyRepository = new Lazy<ICompanyRepository>( () => new CompanyRepository(repositoryContext));
            _employeesRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(repositoryContext));
        }

        public ICompanyRepository Company => _companyRepository.Value;
        public IEmployeeRepository Employee => _employeesRepository.Value;
        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}
