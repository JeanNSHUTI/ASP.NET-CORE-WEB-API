﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ActionFilters
{
    public class ValidateMediaTypeAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            var acceptHeaderPresent = filterContext.HttpContext.Request.Headers.ContainsKey("Accept");

            if (!acceptHeaderPresent )
            {
                filterContext.Result = new BadRequestObjectResult($"Accept header is missing");
                return;
            }

            var mediaType = filterContext.HttpContext.Request.Headers["Accept"].FirstOrDefault();

            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                filterContext.Result = new BadRequestObjectResult($"Media type not present. Please add Accept header with the required media type.");
                return;
            }

            filterContext.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
