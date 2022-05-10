using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Utilities.PopulateAddressLatLng
{
    class PopulateAddressLatLngService : IPopulateAddressLatLngService
    {
        private readonly ILogger<PopulateAddressLatLngService> _logger;
        private readonly TradefactDbContext _context;
        private readonly ILocationService _locationService;

        public PopulateAddressLatLngService(ILoggerFactory loggerFactory, TradefactDbContext context, ILocationService locationService)
        {
            _logger = loggerFactory.CreateLogger<PopulateAddressLatLngService>();
            _context = context;
            _locationService = locationService;
        }

        public async Task GeocodeAddresses()
        {
            var addrList = _context.Addresses.Where(q => q.IsActive && q.Position == null).ToList();

            foreach (var addr in addrList)
            {
                var position = await _locationService.GetAddressLocation(addr);

                if (position != null)
                {
                    addr.Position = position;
                    _logger.LogInformation($"{addr.ToString()} => {position.Latitude}, {position.Longitude}");
                }
            }

            _context.SaveChanges();
        }
    }

    public interface IPopulateAddressLatLngService
    {
        Task GeocodeAddresses();
    }
}
