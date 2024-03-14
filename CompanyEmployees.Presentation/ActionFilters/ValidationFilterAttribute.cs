using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public ValidationFilterAttribute() { }

        public void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            var action = filterContext.RouteData.Values["action"];
            var controller = filterContext.RouteData.Values["controller"];

            var param = filterContext.ActionArguments.SingleOrDefault(x => x.Value.ToString().Contains("Dto")).Value;

            if (param is null)
            {
                filterContext.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
                return;
            }

            if (!filterContext.ModelState.IsValid)
            {
                filterContext.Result = new UnprocessableEntityObjectResult(filterContext.ModelState);
            }
            {
                
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }
    }
}
