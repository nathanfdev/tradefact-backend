using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Infrastructure.Filters
{
    public class ValidateSharedURLSignatureAttribute: Attribute, IAsyncActionFilter
    {
        //private readonly RequestValidator _requestValidator;
        //private static IConfigurationRoot Configuration =>
        //    new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", true, true).Build();
        //private static bool IsTestEnvironment =>
        //    bool.Parse(Configuration["IsTestEnvironment"]);

        public ValidateSharedURLSignatureAttribute()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // do something before the action executes
            var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set
        }

        //private bool IsValidRequest(HttpRequest request)
        //{
        //    var requestUrl = RequestRawUrl(request);
        //    var parameters = ToDictionary(request.Form);
        //    var signature = request.Headers["X-Twilio-Signature"];
        //    return _requestValidator.Validate(requestUrl, parameters, signature);
        //}

        //private static string RequestRawUrl(HttpRequest request)
        //{
        //    return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        //}
    }
}
