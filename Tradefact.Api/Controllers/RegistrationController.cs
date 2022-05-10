using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Document;
using Tradefact.Api.Model.Registration;
using Tradefact.Data;
using Tradefact.Services;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : FilesController
    {
        private readonly TradefactDbContext _context;

        public RegistrationController(ILogger<RegistrationController> logger, TradefactDbContext context, IAzureBlobService blobService, IMimeMappingService mimeMappingService) : base(logger, context, blobService, mimeMappingService) { }

        [HttpPost]
        [Route(nameof(NewRegistration))]
        public async Task<IActionResult> NewRegistration([FromBody] RegistrationRequest request)
        {
            UploadResult uploadResult = await this.ProcessUploadRequest($"Registration");
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }

            OrganisationRegistration orgRegistration = new OrganisationRegistration()
            {
                Address = request.Address,
                BusinessId = request.BusinessId,
                City = request.City,
                CompanyBio = request.CompanyBio,
                CompanyLegalName = request.CompanyLegalName,
                Country = request.Country,
                WebAddress = request.WebAddress,
                RegistrationStatus = Core.Enums.OrganisationRegistrationStatusEnum.Pending
            };

            orgRegistration.RegistrationUsers = new List<OrganisationRegistrationUser>();

            foreach (var user in request.Users)
            {
                OrganisationRegistrationUser orgUser = new OrganisationRegistrationUser()
                {
                    Email = user.Email,
                    IsAdmin = user.IsAdmin,
                    Name = user.Name,
                    Role = user.Role
                };
                orgRegistration.RegistrationUsers.Add(orgUser);
            }

            if(uploadResult.Items.Count == 1)
            {
                orgRegistration.BusinessRegDocumentUrl = uploadResult.Items[0].BlobUrl;
                orgRegistration.BusinessRegDocumentName = uploadResult.Items[0].OriginalFileName;
            }
            else if(uploadResult.Items.Count > 1)
            {
                return BadRequest();
            }
            

            await _context.OrganisationRegistrations.AddAsync(orgRegistration);

            _ = await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
