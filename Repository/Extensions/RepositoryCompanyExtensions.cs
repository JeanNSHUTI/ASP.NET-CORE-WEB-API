using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Repository.Extensions.Utility;

namespace Repository.Extensions
{
    public static class RepositoryCompanyExtensions
    {
        public static IQueryable<Company> FilterCompanies(this IQueryable<Company> companies, string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return companies;
            }

            return companies.Where(e => e.Country.ToLower().Equals(country.Trim().ToLower()));
        }
        public static IQueryable<Company> Search(this IQueryable<Company> companies, string searchTerm)
        {
            //Don't apply search if user does not provide it
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return companies;
            }

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return companies.Where(e => e.Name.ToLower().Contains(lowerCaseTerm) || e.Address.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Company> Sort(this IQueryable<Company> companies, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return companies.OrderBy(e => e.Name);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Company>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return companies.OrderBy(e => e.Name);

            return companies.OrderBy(orderQuery);
        }
    }
}
