using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using FunctionApp.EquipmentTracking.Win.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;


namespace FunctionApp.EquipmentTracking.Win
{
    public static class FunctionUCT
    {
        private static HttpClient _uctClient = GetAuthorisedUCTClient();

        private static string _agentId => Environment.GetEnvironmentVariable("agent_UCT", EnvironmentVariableTarget.Process);
        private static string _userNameUCT => Environment.GetEnvironmentVariable("username_UCT", EnvironmentVariableTarget.Process);
        private static string _passwordUCT => Environment.GetEnvironmentVariable("password_UCT", EnvironmentVariableTarget.Process);

        private static List<TrackTraceEvent> _dscaTrackTraceEvents => GetCurrentDSCATrackTraceEventList().GetAwaiter().GetResult();
        private static List<UCTTraceEvent> _uctTraceEvents = GetCurrentUCTTrackTraceEventList().GetAwaiter().GetResult();


        [FunctionName(nameof(UCTBatchListTest))]
        public static async Task<IActionResult> UCTBatchListTest(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger logger)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UCTResponse response = JsonConvert.DeserializeObject<UCTResponse>(requestBody);
                await ProcessUCTResponse(response, logger);
            }
            catch (Exception e)
            {
                logger.LogError($"An error occured during UCTResponse Message Receipt. {e.Message}", e);
                return new ObjectResult(new { Error = "Tradefact: Internal Server Error" })
                {
                    StatusCode = 500
                };
            }
            return new OkResult();
        }

        [FunctionName(nameof(RunOrchestrator))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            UCTResponse container_updates = await context.CallActivityAsync<UCTResponse>(nameof(GetBatchListUpdate), DateTime.Now.AddDays(-30));

            bool complete = await context.CallActivityAsync<bool>(nameof(ProcessUCTResponse), container_updates);

            return outputs;
        }

        [FunctionName(nameof(HttpStart))]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        //[FunctionName(nameof(Scheduler))]
        //public static async Task Scheduler([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log, [DurableClient] IDurableOrchestrationClient starter)
        //{
        //    log.LogInformation($"C# Timer trigger function started at: {DateTime.Now}");

        //    string instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), null).ConfigureAwait(false);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        //}


        private static async Task<List<TrackTraceEvent>> GetCurrentDSCATrackTraceEventList()
        {
            List<TrackTraceEvent> events = new List<TrackTraceEvent>();

            CloudTableClient client = new CloudTableClient(new Uri("https://storageaccountpocuct.table.core.windows.net/"),
new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("storageaccountpocuct", "9qkbojoGU4iRIHRnl2OPtyBmyNxK0oM0MtWQF+PwuS97XhOmnXYh63XXRqzN3kt1CB8nh6WFnbKSC88++bzApg=="));

            CloudTable table = client.GetTableReference("TrackTraceCodeLookup");

            var query = new TableQuery<TrackTraceEventEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var page = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = page.ContinuationToken;
                events.AddRange(page.Results.Select(s => new TrackTraceEvent
                {
                    TFCode = s.TFCode,
                    EntityCode = s.EntityCode,
                    Entity = s.Entity,
                    EventTypeCode = s.EventTypeCode,
                    EventType = s.EventType,
                    EmptyIndicatorCode = s.EmptyIndicatorCode,
                    TransportTypeCode = s.TransportTypeCode,
                    FacilityTypeCode = s.FacilityTypeCode,
                    Location = s.Location,
                    Event = s.Event,
                    EventDescription = s.EventDescription
                }));
            }
            while (continuationToken != null);

            return events;
        }

        private static async Task<List<UCTTraceEvent>> GetCurrentUCTTrackTraceEventList()
        {
            List<UCTTraceEvent> events = new List<UCTTraceEvent>();

            CloudTableClient client = new CloudTableClient(new Uri("https://storageaccountpocuct.table.core.windows.net/"),
new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("storageaccountpocuct", "9qkbojoGU4iRIHRnl2OPtyBmyNxK0oM0MtWQF+PwuS97XhOmnXYh63XXRqzN3kt1CB8nh6WFnbKSC88++bzApg=="));

            CloudTable table = client.GetTableReference("UCTTraceEventLookup");

            var query = new TableQuery<UCTTraceEventEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var page = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = page.ContinuationToken;
                events.AddRange(page.Results.Select(s => new UCTTraceEvent
                {
                    TFCode = s.TFCode,
                    EntityCode = s.EntityCode,
                    Carrier = s.Carrier,
                    UCTActivityCode = s.UCTActivityCode,
                    TransportTypeCode = s.TransportTypeCode
                }));
            }
            while (continuationToken != null);

            return events;
        }

        private static HttpClient GetAuthorisedUCTClient()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true };

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://integration.winwebconnect.com/api/v1/");

            client.Timeout = new TimeSpan(0, 0, 120);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string credentials = JsonConvert.SerializeObject(new { Username = _userNameUCT, Password = _passwordUCT });
            StringContent content = new StringContent(credentials, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage loginResponse = client.PostAsync("login", content).Result;
            string loginResult = loginResponse.Content.ReadAsStringAsync().Result;

            return client;
        }

        [FunctionName(nameof(GetBatchListUpdate))]
        public static async Task<UCTResponse> GetBatchListUpdate([ActivityTrigger] DateTime lastExecutionTime, ILogger log)
        {
            HttpResponseMessage getResponse = await _uctClient.GetAsync($"uctapi/batchlist/agents/{_agentId}?since={lastExecutionTime:yyyy-MM-ddTHH:mm:ss}");

            string response_content = getResponse.Content.ReadAsStringAsync().Result;

            UCTResponse response = JsonConvert.DeserializeObject<UCTResponse>(response_content);

            log.LogInformation($"GetBatchListUpdate: {response_content}");

            return response;
        }

        [FunctionName(nameof(DeleteBatchList))]
        public static async Task DeleteBatchList(List<TrackRequest> removeUCTBatchList)
        {
            foreach (var s in removeUCTBatchList)
            {
                // DELETE /api/v1/uctapi/batchlist/agents/{ agentId }/{ operatorvalue }/c_id/searchvalue}
                HttpResponseMessage getResponse = await _uctClient.DeleteAsync($"uctapi/batchlist/agents/{_agentId}/{s.Carrier}/{s.SearchKey}/{s.SearchValue}");
                string response_content = getResponse.Content.ReadAsStringAsync().Result;
            }
        }

        private class EquipmentEvent
        {
            public string SearchValue { get; set; }
            public string ContainerId { get; set; }
            public DateTime TimeOfEvent { get; set; }
            public string LocationOfEvent { get; set; }
            public string Voyage { get; set; }
            public string Activity { get; set; }
            public string Information { get; set; }
        }

        [FunctionName(nameof(ProcessUCTResponse))]
        public static async Task ProcessUCTResponse([ActivityTrigger] UCTResponse response, ILogger log)
        {

            List<EquipmentEvent> events = new List<EquipmentEvent>();
            foreach (var query in response.Queries)
            {
                Console.WriteLine($"Query:  Carrier:{query.Search.Carrier}  SearchKey: {query.Search.SearchKey} SearchValue:{query.Search.SearchValue}");
                Console.WriteLine("-- ----------------------------------------------------------- --");

                events.AddRange(GetQueryEvents(query));
            }
            UpdateTrackedShipments(events);
        }

        private static void UpdateTrackedShipments(List<EquipmentEvent> events)
        {
            var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            string upsert_query = @$"DECLARE @TrackingEvents TABLE(shipmentId UNIQUEIDENTIFIER, bol varchar(20), equipmentItemId varchar(30), eventType varchar(32))

INSERT INTO @TrackingEvents(shipmentId, bol, equipmentItemId, eventType)
SELECT S.Id, @bol, @equipmentItemId , @eventType
FROM [dbo].[Shipments] S WHERE S.BillofLadingNumber = @bol

UPDATE SE
    SET SE.TimeOfEvent = @timeOfEvent, SE.Voyage = @voyage, SE.Information = @information
    FROM @TrackingEvents TE 
INNER JOIN [dbo].[ShipmentEvents] SE ON SE.ShipmentId = TE.shipmentId AND SE.EquipmentItemId = TE.equipmentItemId AND SE.Activity = TE.eventType

INSERT INTO [dbo].ShipmentEvents(ShipmentId, TrackingNumber, EquipmentItemId, EventId, TimeOfEvent, Activity, Information, LocationOfEvent)
SELECT TE.shipmentId, @bol, @equipmentItemId, NEWID(), @timeOfEvent, @eventType, @information, @location
FROM @TrackingEvents TE 
    LEFT JOIN [dbo].[ShipmentEvents] SE ON SE.ShipmentId = TE.shipmentId AND SE.EquipmentItemId = TE.equipmentItemId AND SE.Activity = TE.eventType
WHERE SE.ShipmentId IS NULL
";

            using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
            {
                connection.Open();

                foreach (var ev in events)
                {
                    var result = connection.Execute(upsert_query, new
                    {
                        @bol = ev.SearchValue,
                        @equipmentItemId = ev.ContainerId,
                        @eventType = ev.Activity,
                        @timeOfEvent = ev.TimeOfEvent,
                        @voyage = ev.Voyage,
                        @information = ev.Information,
                        @location = ev.LocationOfEvent
                    });
                }
            }
        }

        private static IEnumerable<EquipmentEvent> GetQueryEvents(Equipment query)
        {
            // Response available for query
            if (query.Results?.Status == "Fetched" && query.Results?.Data?.Containers != null)
            {
                // Ignore Errors - Only interested in processing events returned.  Queries are already paid for and will naturally drop from provider
                foreach (var container in query.Results.Data.Containers)
                {
                    Console.WriteLine($"Container:  {container.ContainerNumber},  BOL: {query.Results.Data.BLNumber ?? "NA"}");
                    Console.WriteLine("-- ----- --");

                    foreach (var ev in container.Events)
                    {
                        yield return new EquipmentEvent
                        {
                            SearchValue = query.Search.SearchValue,
                            ContainerId = container.ContainerNumber,
                            Activity = ev.Activity.Name,
                            TimeOfEvent = ev.DateTime.GetValueOrDefault(),
                            LocationOfEvent = ev.Location.Name,
                            Information = $"{ev.Activity.Name} [{ev.State} ]",
                            Voyage = $"{ev.Transport.Liner} {ev.Transport.Vessel}"

                        };
                        Console.WriteLine($"Event:  {ev.Activity.Name},  Time: {ev.DateTime}");
                    }
                    Console.WriteLine("-- ----- --");
                    Console.WriteLine(" ");
                }
                yield break;
            }
        }
    }
}
