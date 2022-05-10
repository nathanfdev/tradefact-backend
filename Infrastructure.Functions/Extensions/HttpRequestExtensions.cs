using System.Threading.Tasks;
using FluentValidation;
using Infrastructure.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Functions.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> GetJsonBody<T>(this HttpRequest request)
        {
            var requestBody = await request.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(requestBody);
        }

        public static async Task<ValidatableRequest<T>> GetJsonBody<T, TV>(this HttpRequest request, ILogger logger)
            where TV : AbstractValidator<T>, new()
        {
            var requestObject = await request.GetJsonBody<T>().ConfigureAwait(false);
            var validator = new TV();
            var validationResult = validator.Validate(requestObject);

            if(!validationResult.IsValid) {
                var validatableRequest = new ValidatableRequest<T>
                { Value = requestObject, IsValid = false, Errors = validationResult.Errors };

                logger.LogError("Invalid request body data", request, validationResult.Errors);

                return validatableRequest;
            }

            return new ValidatableRequest<T> { Value = requestObject, IsValid = true };
        }
    }
}