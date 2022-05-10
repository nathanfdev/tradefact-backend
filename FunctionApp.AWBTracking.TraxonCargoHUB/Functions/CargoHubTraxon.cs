using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Infrastructure.Functions.Extensions;
using FunctionApp.AWBTracking.TraxonCargoHUB.Model;
using System.Linq;
using Core.ServiceBus;
using Azure.Messaging.ServiceBus;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace FunctionApp.AWBTracking.CargoHUB
{
    public static class Traxon
    {
        private static string serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);

        [FunctionName(nameof(TraxonEventDelivery))]
        public static async Task<IActionResult> TraxonEventDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger logger)
        {
            try
            {
                var request = await req.GetJsonBody<FlightStatusMessage, FlightStatusMessageValidator>(logger).ConfigureAwait(false);
                if (!request.IsValid)
                {
                    return request.ToBadRequest();
                }
                await SendMessageAsync("tracking-awb-event-delivery", JsonConvert.SerializeObject(request.Value));
            }
            catch (Exception e)
            {
                logger.LogError($"An error occured during Traxon Cargohub Message Receipt. {e.Message}", e);
                return new ObjectResult(new { Error = "Tradefact: Internal Server Error" })
                {
                    StatusCode = 500
                };
            }
            return new OkResult();
        }

        [FunctionName(nameof(LogSubmitTraxonAWBTrack))]
        public static async Task LogSubmitTraxonAWBTrack(
            [ServiceBusTrigger("tracking-awb-event-delivery", Connection = "ServiceBusConnection")]
            string message,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");
            //log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            //log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            await SendMessageAsync("tracking-awb-traxon-event-process", message);
        }

        [FunctionName(nameof(ProcessTraxonAWBTrack))]
        public static void ProcessTraxonAWBTrack(
            [ServiceBusTrigger("tracking-awb-traxon-event-process", "%EventSubscriptionName%", Connection = "ServiceBusConnection")]
            string message,
            ILogger log)
        {
            Console.WriteLine($"Processing Message");
            Console.WriteLine($"----------------------------------------------------------------------------------");
            try
            {
                FlightStatusMessage flightStatusMessage = JsonConvert.DeserializeObject<FlightStatusMessage>(message);
                foreach (var ev in flightStatusMessage.Events)
                {
                    Console.WriteLine($"{ev.QuantityWeightDescription} {ev.EventDescription},  {ev.TimeOfEvent.UtcDateTime} {ev.EventLocation}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine($"----------------------------------------------------------------------------------");
        }


        [FunctionName(nameof(TestProcessTraxonAWBTrack))]
        public static async Task<IActionResult> TestProcessTraxonAWBTrack(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger logger)
        {
            List<string> response_body = new List<string>();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                List<string> locations = new List<string>();
                FlightStatusMessage flightStatusMessage = JsonConvert.DeserializeObject<FlightStatusMessage>(requestBody);
                foreach (var ev in flightStatusMessage.Events)
                {
                    if (!locations.Contains(ev.EventLocation))
                    {
                        locations.Add(ev.EventLocation);
                    }
                }
                List<IATAInformation> found_airports;
                var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

                string query = @$"SELECT L.IATA [IATACode], L.Name [AirportName], L.Position_Latitude, L.Position_Longitude, C.Code2 [CountryCode], C.Name [CountryName]
                                   FROM [dbo].[Locations] L
                                        INNER JOIN [dbo].Countries C ON C.Code2 = L.CountryCode
                                  WHERE L.IATA IN ({String.Join(',', locations.Select(s=> $"'{s}'"))})";

                string upsert_query = @$"IF EXISTS (
                                                SELECT EventId 
                                                FROM ShipmentEvents SE 
                                                WHERE SE.ShipmentId=@shipmentId AND SE.TrackingNumber=@awbNumber AND SE.EquipmentItemId = @awbNumber AND SE.Activity=@eventType)

                                            UPDATE SE 
                                            SET SE.TimeOfEvent = @timeOfEvent, Activity=@eventType, Voyage=@voyage, Information=@information, SE.LocationOfEvent=@location
                                            FROM ShipmentEvents SE
                                            WHERE SE.ShipmentId=@shipmentId AND SE.TrackingNumber=@awbNumber AND SE.EquipmentItemId = @awbNumber AND SE.Activity=@eventType

                                        ELSE

                                            INSERT INTO ShipmentEvents(ShipmentId, TrackingNumber, EquipmentItemId, EventId, TimeOfEvent, Activity, Voyage, Information, [LocationOfEvent])
                                            VALUES (@shipmentId, @awbNumber, @awbNumber, NEWID(), @timeOfEvent, @eventType, @voyage, @information, @location)

                                        UPDATE [dbo].Shipments SET EquipmentTrackAvailable = 1 WHERE Id = @shipmentId; ";

                string location = "N/A";
                using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
                {
                    connection.Open();

                    found_airports = connection.Query<IATAInformation>(query).ToList();
                    connection.Close();

                    foreach (var ev in flightStatusMessage.Events.OrderBy(o => o.TimeOfEvent))
                    {
                        if (found_airports.Any(q => q.IATACode == ev.EventLocation))
                        {
                            location = found_airports.FirstOrDefault(q => q.IATACode == ev.EventLocation).ToString();
                        }
                        else
                        {
                            location = ev.EventLocation;
                        }

                        var result = connection.Execute(upsert_query, new
                        {
                            @shipmentId= "44a069df-1f89-422d-b167-02f7ffba1458",
                            @awbNumber = flightStatusMessage.AirWaybillNumber,
                            @eventType = ev.EventType,
                            @timeOfEvent = ev.TimeOfEvent,
                            @voyage = ev.Flight ?? "N/A",
                            @information = ev.EventDescription,
                            @location = location
                        });
                    }
                }





            }
            catch (Exception e)
            {
                logger.LogError($"An error occured during Cargohub Response. {e.Message}", e);
                return new ObjectResult(new { Error = "Tradefact: Internal Server Error" })
                {
                    StatusCode = 500
                };
            }
            return new OkObjectResult(response_body);
        }

        static async Task SendMessageAsync(string queueName, string payload)
        {
            // create a Service Bus client 
            await using (ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString))
            {
                // create a sender for the queue 
                ServiceBusSender sender = client.CreateSender(queueName);

                // create a message that we can send
                ServiceBusMessage message = new ServiceBusMessage(payload);

                // send the message
                await sender.SendMessageAsync(message);
            }
        }

    }
}
