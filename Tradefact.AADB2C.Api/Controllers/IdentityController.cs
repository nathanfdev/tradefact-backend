using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tradefact.AADB2C.Api.Infrastructure;
using Tradefact.AADB2C.Api.Model;

namespace Tradefact.AADB2C.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {

        [HttpPost]
        [Route(nameof(SignUpCreateOrg))]
        public async Task<IActionResult> SignUpCreateOrg()
        {
            // If no data came in, then return
            if (this.Request.Body == null) throw new Exception();

            // Read the input claims from the request body
            string input = await this.Request.GetRawBodyStringAsync();

            // Check the input content value
            if (string.IsNullOrEmpty(input))
            {
                return new ConflictObjectResult(new B2CResponseContent("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into an InputClaimsRequest object
            InputClaimsRequest inputClaims = JsonConvert.DeserializeObject(input, typeof(InputClaimsRequest)) as InputClaimsRequest;

            if (inputClaims == null)
            {
                return new ConflictObjectResult(new B2CResponseContent("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            // Run an input validation
            if (inputClaims.firstName.ToLower() == "test")
            {
                return new ConflictObjectResult(new B2CResponseContent("Test name is not valid, Please provide a valid name", HttpStatusCode.Conflict));
            }

            // Create an output claims object and set the loyalty number with a random value
            OutputClaimsResponse outputClaims = new OutputClaimsResponse();
            outputClaims.OrganisationId = new Random().Next(100, 1000).ToString();

            // Return the output claim(s)
            return Ok(outputClaims);
        }

        [HttpPost]
        [Route(nameof(SignUpJoinOrg))]
        public async Task<IActionResult> SignUpJoinOrg()
        {
            // If no data came in, then return
            if (this.Request.Body == null) throw new Exception();

            // Read the input claims from the request body
            string input = await this.Request.GetRawBodyStringAsync();

            // Check the input content value
            if (string.IsNullOrEmpty(input))
            {
                return new ConflictObjectResult(new B2CResponseContent("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into an InputClaimsRequest object
            InputClaimsRequest inputClaims = JsonConvert.DeserializeObject(input, typeof(InputClaimsRequest)) as InputClaimsRequest;

            if (inputClaims == null)
            {
                return new ConflictObjectResult(new B2CResponseContent("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            // Run an input validation
            if (inputClaims.firstName.ToLower() == "test")
            {
                return new ConflictObjectResult(new B2CResponseContent("Test name is not valid, Please provide a valid name", HttpStatusCode.Conflict));
            }

            // Create an output claims object and set the loyalty number with a random value
            OutputClaimsResponse outputClaims = new OutputClaimsResponse();
            outputClaims.OrganisationId = new Random().Next(100, 1000).ToString();

            // Return the output claim(s)
            return Ok(outputClaims);
        }

    }
}