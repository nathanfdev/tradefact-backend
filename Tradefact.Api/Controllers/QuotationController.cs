using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Tradefact.Api.Infrastructure.Extensions;
using Tradefact.Api.Responses;
using Tradefact.Data;
using Tradfact.Api.Requests;



namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : BaseApiController
    {
        public QuotationController(TradefactDbContext context) : base(context) { }



    }
}