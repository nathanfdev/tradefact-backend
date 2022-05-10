using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp.KeepAlive
{
    public static class EndPointKeepWarm
    {
        static string _endPointsToHit = Environment.GetEnvironmentVariable("EndPointUrls");

        static HttpClient _httpClient = new HttpClient();

        static async Task<HttpResponseMessage> hitUrl(string url, ILogger log)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                log.LogInformation($"hitUrl(): Successfully hit URL: '{url}'");
            } else
            {
                log.LogError($"hitUrl(): Failed to hit URL: '{url}'. Response: {$"{(int)response.StatusCode} : {response.ReasonPhrase}"}");
            }

            return response;
        }

        [FunctionName("EndPointKeepWarm")]
        public static async Task Run([TimerTrigger("%CronSchedule%")]TimerInfo myTimer, ILogger log)
        {
            // run every 15 minutes..
            log.LogInformation($"Run(): EndPointKeepWarm function executed at: {DateTime.Now}. Past due? {myTimer.IsPastDue}");

            if(!string.IsNullOrEmpty(_endPointsToHit))
            {
                string[] endPoints = _endPointsToHit.Split(';');
                foreach(string endPoint in endPoints)
                {
                    string tidiedUrl = endPoint.Trim();
                    if(!tidiedUrl.EndsWith("/"))
                    {
                        tidiedUrl += "/";
                    }
                    log.LogInformation($"Run(): About to hit URL: '{tidiedUrl}'");

                    HttpResponseMessage response = await hitUrl(tidiedUrl, log);
                }
            } else
            {
                log.LogError($"{$"Run(): No URLs specified in environment variable 'EndPointUrls'. Expected a single URL or multiple URLs "}separated with a semi-colon (;). Please add this config to use the tool.");
            }

            log.LogInformation($"Run(): Completed..");
        }
    }
}