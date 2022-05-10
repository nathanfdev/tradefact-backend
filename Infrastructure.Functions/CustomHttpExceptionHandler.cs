using AzureFunctionsV2.HttpExtensions.Exceptions;
using AzureFunctionsV2.HttpExtensions.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.Functions
{
    public class CustomHttpExceptionHandler : IHttpExceptionHandler
    {
        public CustomHttpExceptionHandler() { }

        protected virtual string GetExceptionMessageRecursive(Exception outermostException)
        {
            var messages = new List<string>();
            var exception = outermostException;
            while(exception != null)
            {
                messages.Add(exception.Message);
                exception = exception.InnerException;
            }

            return string.Join("; ", messages);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public virtual async Task<IActionResult> HandleException(FunctionExecutingContext functionExecutingContext,
#pragma warning restore CS0618 // Type or member is obsolete
                                                                 HttpRequest request,
                                                                 Exception exception)
        {
            functionExecutingContext?.Logger.LogError(exception, GetExceptionMessageRecursive(exception));

            var errorObject = new Dictionary<string, string>();

            var httpExtensionsException = exception as HttpExtensionsException ?? exception.InnerException as HttpExtensionsException;

            if((httpExtensionsException is ParameterFormatConversionException) || (httpExtensionsException is ParameterRequiredException))
            {
                var response = new BadRequestObjectResult(errorObject);
                errorObject.Add("message", OutputRecursiveExceptionMessages ? GetExceptionMessageRecursive(httpExtensionsException) : httpExtensionsException.Message);
                errorObject.Add("parameter", httpExtensionsException.ParameterName);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            var cosmosException = exception as CosmosException ?? exception.InnerException as CosmosException;

            if(cosmosException != null)
            {
                errorObject.Add("source", "database");
                var response = new ObjectResult(errorObject) { StatusCode = 500 };

                response.StatusCode = cosmosException.StatusCode switch
                {
                    HttpStatusCode.NotFound => 404,
                    HttpStatusCode.PreconditionFailed => 412,
                    HttpStatusCode.Conflict => 409,
                    HttpStatusCode.ExpectationFailed => 417,
                    _ => response.StatusCode
                };

                errorObject.Add("message", OutputRecursiveExceptionMessages ? GetExceptionMessageRecursive(cosmosException.InnerException) : cosmosException.Message);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            var httpAuthenticationException = exception as HttpAuthenticationException ?? exception.InnerException as HttpAuthenticationException;

            if(httpAuthenticationException != null)
            {
                var response = new ObjectResult(errorObject) { StatusCode = 401 };
                errorObject.Add("message", OutputRecursiveExceptionMessages ? GetExceptionMessageRecursive(httpAuthenticationException) : httpAuthenticationException.Message);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            var httpAuthorizationException = exception as HttpAuthorizationException ?? exception.InnerException as HttpAuthorizationException;

            if(httpAuthorizationException != null)
            {
                var response = new ObjectResult(errorObject) { StatusCode = 403 };
                errorObject.Add("message", OutputRecursiveExceptionMessages ? GetExceptionMessageRecursive(httpAuthorizationException) : httpAuthorizationException.Message);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            var defaultResponse = new ObjectResult(errorObject) { StatusCode = 500 };
            errorObject.Add("message", OutputRecursiveExceptionMessages ? GetExceptionMessageRecursive(exception) : exception.Message);
            return await Task.FromResult(defaultResponse).ConfigureAwait(false);
        }

        public static bool OutputRecursiveExceptionMessages { get; set; }
    }
}

