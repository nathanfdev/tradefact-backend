using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tradefact.Api.Common.Controllers;
using Tradefact.Api.Common.Versioning.Interfaces;

namespace Tradefact.Api.Common.Versioning
{
    public class VersionController : BaseController
    {
        private readonly IVersionService _versionService;

        public VersionController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        /// <summary>
        /// Current Build Version Number
        /// </summary>
        /// <remarks>
        /// Use this operation to return the current API Build Version number
        /// </remarks>
        /// <response code="200">Returns the current build version number</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("api/version")]
        [Produces(typeof(Version))]
        public ActionResult<Version> Get()
        {
            var response = _versionService.GetVersion();
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
