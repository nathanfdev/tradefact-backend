using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common;
using Core.Models.ReferenceData;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Tradefact.Api.Model;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceController : BaseApiController
    {
        public ReferenceController(TradefactDbContext context) : base(context) { }

        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<HazardCode>), Description = "OK Result")]
        [Route("HazardCodes")]
        public async Task<IActionResult> List()
        {
            List<HazardCode> codes = new List<HazardCode>();
            codes.Add(new HazardCode { Class = "1.1", Name = "Division 1.1: Mass Explosive Hazard", Division = "Explosives" });
            codes.Add(new HazardCode { Class = "1.2", Name = "Division 1.2: Projection Hazard", Division = "Explosives" });
            codes.Add(new HazardCode { Class = "1.3", Name = "Division 1.3: Fire and/or Minor Blast/Minor Projection Hazard", Division = "Explosives" });
            codes.Add(new HazardCode { Class = "1.4", Name = "Division 1.4: Minor Explosion Hazard", Division = "Explosives" });
            codes.Add(new HazardCode { Class = "1.5", Name = "Division 1.5: Very Insensitive With Mass Explosion Hazard", Division = "Explosives" });
            codes.Add(new HazardCode { Class = "1.6", Name = "Division 1.6: Extremely Insensitive; No Mass Explosion Hazard", Division = "Explosives" });

            codes.Add(new HazardCode { Class = "2.1", Name = "Division 2.1: Flammable Gases", Division = "Gases" });
            codes.Add(new HazardCode { Class = "2.2", Name = "Division 2.2: Nonflammable Gases", Division = "Gases" });
            codes.Add(new HazardCode { Class = "2.3", Name = "Division 2.3: Toxic Gases", Division = "Gases" });

            //codes.Add(new HazardCode { Class = "3.1", Name = "Division 3.1: Flashpoint below -18°C(0°F)", Division = "Flammable Liquids" });
            //codes.Add(new HazardCode { Class = "3.2", Name = "Division 3.2: Flashpoint below -18°C and above, but less than 23°C(73°F)", Division = "Flammable Liquids" });
            //codes.Add(new HazardCode { Class = "3.3", Name = "Division 3.3: Flashpoint 23°C and up to 61°C(141°F)", Division = "Flammable Liquids" });

            codes.Add(new HazardCode { Class = "4.1", Name = "Division 4.1: Flammable Solids", Division = "Flammable Solids" });
            codes.Add(new HazardCode { Class = "4.2", Name = "Division 4.2: Spontaneously Combustible", Division = "Flammable Solids" });
            codes.Add(new HazardCode { Class = "4.3", Name = "Division 4.3: Dangerous When Wet", Division = "Flammable Solids" });

            codes.Add(new HazardCode { Class = "5.1", Name = "Division 5.1: Oxidizing Substances", Division = "Oxidizing Substances, Organic Peroxides" });
            codes.Add(new HazardCode { Class = "5.2", Name = "Division 5.2: Organic Peroxides", Division = "Oxidizing Substances, Organic Peroxides" });

            codes.Add(new HazardCode { Class = "6.1", Name = "Division 6.1: Toxic Substances", Division = "Toxic Substances and Infectious Substances" });
            codes.Add(new HazardCode { Class = "6.2", Name = "Division 6.2: Infectious Substances", Division = "Toxic Substances and Infectious Substances" });

            codes.Add(new HazardCode { Class = "7.0", Name = "Radioactive Material", Division = "Radioactive Material" });

            codes.Add(new HazardCode { Class = "8.0", Name = "Corrosives (Liquids And Solids)", Division = "Corrosives (Liquids And Solids)" });

            codes.Add(new HazardCode { Class = "9.0", Name = "ID8000 materials, UN3077, UN3082, UN3334, or UN3335 materials", Division = "Miscellaneous Hazardous Materials" });

            IPagedList<HazardCode> result = await codes.ToPagedListAsync(1, codes.Count);
            return this.HandleSuccessResponse(new ListResource<HazardCode>(result));
        }

        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<CurrencyResource>), Description = "OK Result")]
        [Route("Currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = _context.Currency.Include(i=>i.Countries).ThenInclude(i=>i.Country).Where(q=>q.Active).OrderBy(o=>o.CurrencyName).ToList();

            IPagedList<CurrencyResource> result = await currencies.Adapt<List<CurrencyResource>>().ToPagedListAsync(1, currencies.Count);
            return new OkObjectResult(new ListResource<CurrencyResource>(result));

        }

        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Route("ValidateContainerId")]
        public async Task<IActionResult> ValidateContainerId(string containerid)
        {
            return new OkObjectResult(new { Containerid = containerid, IsValid = this.IsValidContainerId(containerid) } );
        }

        const string regex = @"^([A-Z]{3})([U,J,Z]{1})([0-9]{7})";
        const string alphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private bool IsValidContainerId(string containerId)
        {
            Dictionary<char, int> _equivalentNumericalValues = new Dictionary<char, int>();
            int numerical = 10;
            foreach (char alpha in alphas.ToCharArray())
            {
                // 11 and multiples thereof are omitted
                if (numerical % 11 == 0) numerical++;
                _equivalentNumericalValues.Add(alpha, numerical);
                numerical++;
            }

            Match match = Regex.Match(containerId, regex, RegexOptions.IgnoreCase);
            if (!match.Success) return false;

            string checkDigit = containerId.Substring(containerId.Length - 1);
            double check_total = 0;
            int index = 0;

            foreach (char positional_char in containerId.Substring(index, 4).ToCharArray())
            {
                int equivalentNumerical = _equivalentNumericalValues[positional_char];
                check_total = check_total + equivalentNumerical * (Math.Pow(2, index));
                index++;
            }

            foreach (int positional_int in containerId.Substring(index, 6).ToCharArray().Select(x => x - '0'))
            {
                check_total = check_total + positional_int * (Math.Pow(2, index));
                index++;
            }

            int calcCheckDigit = (int)(check_total - (Math.Floor(check_total / 11) * 11));

            return (calcCheckDigit.ToString() == checkDigit);
        }

    }
}