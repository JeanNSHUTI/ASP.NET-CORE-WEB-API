using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _service;

        public CompaniesV2Controller(IServiceManager serviceManager)
        {
            _service = serviceManager;
        }

        /// <summary>
        /// Get a list of all companies
        /// </summary>
        /// <param name="companyParameters"></param>
        /// <returns>List of companies in database</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.companies);
        }
    }
}
