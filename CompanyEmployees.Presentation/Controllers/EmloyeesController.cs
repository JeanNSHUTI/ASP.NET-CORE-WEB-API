using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
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
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service) => _service = service;

        /// <summary>
        /// Get a list of employees for a specific company
        /// </summary>
        /// <param name="companyId">id of specific company</param>
        /// <param name="employeeParameters"> min age, max age, search term</param>
        /// <returns>list of employees</returns>
        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);

            var result = await _service.EmployeeService.GetEmployeesAsync(companyId, linkParams, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));

            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);
        }

        /// <summary>
        /// Get a specific employee from a specific company
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="id">employee id</param>
        /// <returns>employee</returns>
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);
            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee for a company
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="employee">valid employee</param>
        /// <returns></returns>
        /// <response code="201"> Returns the newly created employee item</response>
        /// <response code="400"> If the item is null</response>
        /// <response code="422"> If the model is null</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false);

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        /// <summary>
        /// Delete an employee for a specific company
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="id">employee id to delete</param>
        /// <returns>no content</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id) 
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);
            return NoContent();
        }

        /// <summary>
        /// Full update of specific employee for a specific company
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="id">employee id to update</param>
        /// <param name="employee">valid emplyee model</param>
        /// <returns>No content</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, compTrackChanges: false, empTrackChanges: true);

            return NoContent();
        }

        /// <summary>
        /// Partially update employee for a specific company
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="id">employee id to partially update</param>
        /// <param name="patchDocForEmployee">valid patch document with update commands</param>
        /// <returns>no content</returns>
        /// <response code="400">Invalid patch document, is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocForEmployee)
        {
            if(patchDocForEmployee is null)
            {
                return BadRequest("patch document object sent from client is null");
            }

            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, compTrackChanges: false, empTrackChanges: true);

            patchDocForEmployee.ApplyTo(result.employeeToPatch, ModelState);

            TryValidateModel(result.employeeToPatch);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);

            return NoContent();
        }
    }
}
