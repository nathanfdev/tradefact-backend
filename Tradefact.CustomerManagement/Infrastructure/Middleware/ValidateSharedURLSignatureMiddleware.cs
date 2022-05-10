using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Portal.Infrastructure.Helpers;

namespace Tradefact.Portal.Infrastructure.Middleware
{
    public class ValidateSharedURLSignatureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly SigningUrlHelper _signingUrlHelper;

        public ValidateSharedURLSignatureMiddleware(RequestDelegate next, SigningUrlHelper _signingUrlHelper, ILoggerFactory loggerFactory)
        {
            _next = next;
            this._signingUrlHelper = _signingUrlHelper;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.Request.Path.Value.Contains("POViewer") && !context.Request.Query.ContainsKey("handler"))
            {
                try
                {
                    var strippedUrl = this._signingUrlHelper.GetUrlWithoutSignature(context.Request);
                    if (!this._signingUrlHelper.SignatureIsValid(strippedUrl, context.Request))
                    {
                        await context.Response.WriteAsync("Invalid URL Contact Support!");
                        return;
                    }
                }
                catch (Exception)
                {
                    await context.Response.WriteAsync("Invalid URL Contact Support!");
                    return;
                }


            }
            await _next(context);
        }
    }
}
