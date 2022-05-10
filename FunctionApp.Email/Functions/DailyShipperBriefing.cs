using System;
using System.Configuration;
using System.Data.SqlClient;
using Core.Models;
using FunctionApp.Email.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp.Email.Functions
{
    public static class DailyShipperBriefing
    {
        [FunctionName("DailyShipperBriefing")]
        public static void Run([TimerTrigger("0 0 8 * * 1-5")]TimerInfo myTimer, ILogger log)
        //public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var str = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            DailyBriefingShipper variables = new DailyBriefingShipper();

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $@"declare @currentdate datetime = GETDATE();
                            ;WITH cte_shippers AS (
                                select AU.OrganisationId, AU.Email, AU.GivenName
                                from [dbo].[AspNetUsers] AU 
                                join [dbo].[Organisations] O on O.Id = AU.OrganisationId
                                WHERE AU.IsAdmin = 1
                            )
                            , cte_quotes AS (
                                select S.OrganisationId, S.Email, S.GivenName, P.Id [PartnershipId], SUM(CASE WHEN Q.[State] = 1 AND FM.GoodsReady > @currentdate THEN 1 ELSE 0 END) [OutstandingQuotes]
                                from cte_shippers S
                                    INNER JOIN [dbo].[Partnerships] P ON P.ClientId = S.OrganisationId
                                    LEFT JOIN [dbo].[QuotationRequests] Q ON Q.PartnershipId = P.Id
                                    LEFT JOIN [dbo].[FreightMovements] FM ON FM.Id = Q.FreightMovementId
                                GROUP BY S.OrganisationId, P.Id, S.Email, S.GivenName
                            )

                            select 
                                sum([Due for collection]), 
                                sum([Arrivals Expected]), 
                                sum([Arrivals Expected Ship]), 
                                sum([Arrivals Expected Air]), 
                                sum([Departures Expected]), 
                                sum([Departures Expected Ship]), 
                                sum([Departures Expected Air]), 
                                sum([Due for Delivery]), 
                                Sum(Case when [Collection Overdue] + [Departure Overdue] + [Arrival Overdue] + [Customs Issue] + [Delivery Overdue] > 0 then 1 else 0 end),
                                Email, GivenName, OutstandingQuotes
                                from (
                                    select 
                                        P.Email, 
                                        P.GivenName,
                                        P.OutstandingQuotes, 
                                        Case when @currentdate < EstimatedCollectionDate and EstimatedCollectionDate < DATEADD(day, 1, @currentdate) then 1 else 0 end [Due for collection] ,
                                        Case when @currentdate < ETA and ETA < DATEADD(day, 1, @currentdate) then 1 else 0 end [Arrivals Expected] ,
                                        Case when ShipmentType = 1 and @currentdate < ETA and ETA < DATEADD(day, 1, @currentdate) then 1 else 0 end [Arrivals Expected Ship] ,
                                        Case when ShipmentType = 2 and @currentdate < ETA and ETA < DATEADD(day, 1, @currentdate) then 1 else 0 end [Arrivals Expected Air] ,
                                        Case when @currentdate < ETD and ETD < DATEADD(day, 1, @currentdate) then 1 else 0 end [Departures Expected] ,
                                        Case when ShipmentType = 1 and @currentdate < ETD and ETD < DATEADD(day, 1, @currentdate) then 1 else 0 end [Departures Expected Ship] ,
                                        Case when ShipmentType = 2 and @currentdate < ETD and ETD < DATEADD(day, 1, @currentdate) then 1 else 0 end [Departures Expected Air] ,
                                        Case when CustomsClearence = 1 and Delivered = 0 then 1 else 0 end [Due for Delivery] ,
                                        Case when EstimatedCollectionDate < @currentdate and Collected = 0 then 1 else 0 end [Collection Overdue] ,
                                        Case when ETD < @currentdate and DepartedPOL = 0 then 1 else 0 end [Departure Overdue],
                                        Case when ETA < @currentdate and ArrivedPOD = 0 then 1 else 0 end [Arrival Overdue],
                                        CAse when IssueAtCustoms = 1 and CustomsClearence = 0 then 1 else 0 end [Customs Issue],
                                        Case when EstimatedDeliveryDate < @currentdate and Delivered = 0 then 1 else 0 end [Delivery Overdue]
                                    from cte_quotes P
                                    LEFT JOIN Shipments S on S.PartnershipId = P.PartnershipId
                                ) dt
                                group by Email, GivenName, OutstandingQuotes";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            variables.CollectionsCount = reader.GetInt32(0).ToString();
                            variables.ArrivalsCount = reader.GetInt32(1).ToString();
                            variables.ArrivalsSeaCount = reader.GetInt32(2).ToString();
                            variables.ArrivalsAirCount = reader.GetInt32(3).ToString();
                            variables.DeparturesCount = reader.GetInt32(4).ToString();
                            variables.DeparturesSeaCount = reader.GetInt32(5).ToString();
                            variables.DeparturesAirCount = reader.GetInt32(6).ToString();
                            variables.DeliveriesCount = reader.GetInt32(7).ToString();
                            variables.ExceptionsCount = reader.GetInt32(8).ToString();
                            variables.Email = reader.GetString(9);
                            if (reader.IsDBNull(10))
                            {
                                variables.FirstName = "";
                            }
                            else
                            {
                                variables.FirstName = reader.GetString(10);
                            }
                            variables.QuotesReadyCount = reader.GetInt32(11).ToString();

                            //This is for nicer formatting using Handlebars templates
                            if (variables.CollectionsCount == "0") variables.CollectionsCount = "";
                            if (variables.ArrivalsCount == "0") variables.ArrivalsCount = "";
                            if (variables.DeparturesCount == "0") variables.DeparturesCount = "";
                            if (variables.DeliveriesCount == "0") variables.DeliveriesCount = "";
                            if (variables.ExceptionsCount == "0") variables.ExceptionsCount = "";
                            if (variables.QuotesReadyCount == "0") variables.QuotesReadyCount = "";


                            //testing
                            //variables.Email = "mikebucher@mail.com";
                            EmailSender.SendSimpleMessage(variables, JsonConvert.SerializeObject(variables), "dailyshipperbriefing", "Your Daily Tradefact Update");
                        }

                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }

            }
        }
    }
}
