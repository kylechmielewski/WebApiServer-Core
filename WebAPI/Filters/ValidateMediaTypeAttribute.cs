using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Filters
{
    public class ValidateMediaTypeAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var acceptHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Content-Type");

            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult($"Content-Type header is missing");
                return;
            }

            var mediaType = context.HttpContext.Request.Headers["Content-Type"].FirstOrDefault();

            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media type is not present.");
                return;
            }

            context.HttpContext.Items.Add("ContentTypeHeaderMediaType", outMediaType);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
