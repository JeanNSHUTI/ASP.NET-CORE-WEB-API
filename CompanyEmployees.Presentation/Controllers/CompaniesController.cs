using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
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
    [ApiExplorerSettings(GroupName = "v1")]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CompaniesController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet(Name = "GetCompanies")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters) 
        {
            var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters,trackChanges: false);
            
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.companies);
        }

        /// <summary>
        /// Gets a specific company from the database
        /// </summary>
        /// <returns>A company dto</returns>
        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        /// <summary>
        /// Gets a collection of companies based on a list of given IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>The list of companies with corresponding ids</returns>
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids) 
        {
            var companies = await _service.CompanyService.GetCompaniesByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }

        /// <summary>
        /// Creates a newly created company 
        /// </summary>
        /// <param name="company"></param> 
        /// <returns>A newly created company</returns> 
        /// <response code="201">Returns the newly created item</response> 
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateCompany")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        /// <summary>
        /// Creates a group of newly created company 
        /// </summary>
        /// <param name="companyCollection"></param> 
        /// <returns>A newly created company</returns> 
        /// <response code="201">Returns the newly created company items</response> 
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost("collection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        /// <summary>
        /// Deletes a company
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No content</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }

        /// <summary>
        /// Updates a company in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="company">update company values</param>
        /// <returns></returns>
        /// <response code="204">Successfuly executed</response>
        /// <response code="400">If the model is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);

            return NoContent();
        }

        /// <summary>
        /// Update company fields with patch document commands
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocForCompany"></param>
        /// <returns>No content</returns>
        /// <response code="204">Successfuly executed</response>
        /// <response code="400">If the patch document was not successfully parsed and is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDocForCompany)
        {
            if (patchDocForCompany is null)
            {
                return BadRequest("patchDoc for company object sent by client is null.");
            }

            var result = await _service.CompanyService.GetCompanyForPatchAsync(id, trackChanges: true);

            patchDocForCompany.ApplyTo(result.companyToPatch, ModelState);

            TryValidateModel(result.companyToPatch);

            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _service.CompanyService.SaveChangesForPatchAsync(result.companyToPatch, result.CompanyEntity);

            return NoContent();
        }

        /// <summary>
        /// Get possible http requests for company entity
        /// </summary>
        /// <returns>list of possible http requests</returns>
        [HttpOptions(Name = "GetCompaniesOptions")]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");

            return Ok();
        }

    }
}
