using Core.Common;using Flare.Api.Model;
using Flare.Data;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Flare.Api.Controllers
{
    public class DeviceController : BaseApiController
    {
        public DeviceController(FlareDbContext context) : base(context)
        {
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(DeviceResource), Description = "OK result")]
        public virtual async Task<IActionResult> GetDeviceById(string id)
        {
            Device device = await _context.Devices
                .Include(d => d.LastDeviceReport)
                .FirstOrDefaultAsync(q => q.Id == new Guid(id) || q.DeviceId == id);

            if (device == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(device.Adapt<DeviceResource>());
        }

        [HttpGet("{id}/reports")]
        [SwaggerResponse("200", typeof(List<DeviceReportResource>), Description = "OK result")]
        public virtual async Task<IActionResult> GetDeviceReportsById(string id, DateTime? from = null, DateTime? to = null)
        {
            Device device = await _context.Devices
                .FirstOrDefaultAsync(q => q.Id == new Guid(id) || q.DeviceId == id);

            if (device == null)
            {
                return new NotFoundResult();
            }

            IQueryable<DeviceReport> reportQuery = _context.DeviceReports
                .Where(d => d.DeviceId == device.DeviceId)
                .OrderByDescending(d => d.GpsTime);

            if (from != null)
            {
                reportQuery = reportQuery.Where(q => q.GpsTime >= from);
            }

            if (to != null)
            {
                reportQuery = reportQuery.Where(q => q.GpsTime >= to);
            }

            List<DeviceReport> result = await reportQuery.ToListAsync();

            return new OkObjectResult(result.Adapt<DeviceReportResource>());
        }
    }
}
