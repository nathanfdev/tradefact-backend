using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Integration.Here
{
    public class HereService : ILocationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ILocationService> _logger;
        private readonly HereOptions _options;

        public HereService(IHttpClientFactory httpClientFactory, ILogger<ILocationService> logger, HereOptions options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<GeographicPosition> GetAddressLocation(Address address)
        {
            try
            {
                var response = await _httpClientFactory.CreateClient("here").GetAsync(buildRequestUri(address));

                if (response.IsSuccessStatusCode)
                {
                    var content = JObject.Parse(await response.Content.ReadAsStringAsync());

                    if (content["items"].Count() > 0)
                    {
                        return new GeographicPosition
                        {
                            Latitude = (double)content["items"][0]["position"]["lat"],
                            Longitude = (double)content["items"][0]["position"]["lng"]
                        };
                    }
                }
                else
                {
                    _logger.LogError($"Unable to get address location (HTTP {response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to get address location {ex}");
            }

            return null;
        }

        private Uri buildRequestUri(Address address)
        {
            var query = HttpUtility.UrlEncode(
                string.Join(",", 
                    new List<string>
                    {
                        address.AddressLine1,
                        address.AddressLine2,
                        address.AddressLine3,
                        address.City,
                        address.PostalCode,
                        address.Country?.Name
                    }
                    .Where(q => !string.IsNullOrEmpty(q))
                )
            );

            return new UriBuilder
            {
                Scheme = "https",
                Host = "geocode.search.hereapi.com",
                Path = "v1/geocode",
                Query = $"q={query}&apiKey={_options.APIKey}"
            }.Uri;
        }

        public Dictionary<string, string> GetOptions()
        {
            throw new NotImplementedException();
        }
    }
}
