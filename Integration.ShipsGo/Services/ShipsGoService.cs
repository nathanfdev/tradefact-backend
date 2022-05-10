using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Integration.ShipsGo
{
    public class ShipsGoService: ISeaTrackingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ShipsGoService> _logger;
        private readonly ShipsGoOptions _options;

        public ShipsGoService(IHttpClientFactory httpClientFactory, ILogger<ShipsGoService> logger, ShipsGoOptions options)
        {

            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> InitiateContainerTrack(List<string> containers, string carrier)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("shipsgo");
                client.BaseAddress = new Uri("https://shipsgo.com/");

                client.Timeout = new TimeSpan(0, 0, 120);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var container in containers)
                {
                    var postData = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("authCode", _options.AuthCode),
                        new KeyValuePair<string, string>("containerNumber", container),
                        new KeyValuePair<string, string>("shippingLine", carrier)
                    };

                    HttpContent httpContent = new FormUrlEncodedContent(postData);
                    Task<HttpResponseMessage> response = client.PostAsync("api/ContainerService/PostContainerInfo/", httpContent);
                    var result = response.Result;

                    var message = await response.Result.Content.ReadAsStringAsync();
                    switch (response.Result.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            {
                                _logger.LogError(message);
                                break;
                            }
                        case HttpStatusCode.OK:
                            {
                                _logger.LogInformation(message);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to get address location {ex}");
            }
            return null;
        }
    }
}
