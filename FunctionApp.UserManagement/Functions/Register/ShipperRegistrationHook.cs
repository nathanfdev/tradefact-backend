using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tradefact.UserManagement.Model;

namespace Tradefact.UserManagement
{
    public static class ShipperRegistrationHook
    {
        private static string qRegistration = Environment.GetEnvironmentVariable("Queue_ShipperRegistration", EnvironmentVariableTarget.Process);
        private static string secret = Environment.GetEnvironmentVariable("TypeformSecret", EnvironmentVariableTarget.Process);

        [FunctionName(nameof(ShipperRegistrationWebHook))]
        public static async Task<IActionResult> ShipperRegistrationWebHook(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger logger)
        {
            if (req.Headers.TryGetValue("Typeform-Signature", out var signature))
            {
                string payload = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (IsValidTypeformSignature(payload, signature, secret))
                {
                    try
                    {
                        Response response = JsonConvert.DeserializeObject<Response>(payload);
                        Registration reg = new Registration
                        {
                            CompanyName = response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Ref == "TF_COMPANYNAME").Text,
                            PersonName = response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Ref == "TF_YOURNAME").Text,
                            EmailAddress = response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Ref == "TF_EMAILADDRESS").Email,
                            Subscription = response.FormResponse.HiddenFields.FirstOrDefault(q => q.Key == "plan").Value.ToLower(),
                        };

                        await EnqueueShipperRegistration(reg);

                        return new OkObjectResult("OK");
                    }
                    catch (Exception)
                    {
                        return new BadRequestResult();
                    }
                }
            }
            return new BadRequestResult();
        }

        private static async Task EnqueueShipperRegistration(Registration reg)
        {
            await ServiceBusHelpers.SendMessageAsync(JsonConvert.SerializeObject(reg), qRegistration);
        }

        public static string GenerateSignature(string payload, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(payload);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);

            return Convert.ToBase64String(bytes);
        }

        private static bool IsValidTypeformSignature(string payload, string signature, string secret)
        {
            string generatedSig = $"sha256={GenerateSignature(payload, secret)}";
            return (generatedSig == signature);
        }
    }
}
