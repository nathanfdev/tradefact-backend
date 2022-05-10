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
    public static class WeeklyForwarderBriefing
    {
        [FunctionName("WeeklyForwarderBriefing")]
        public static void Run([TimerTrigger("0 45 7 * * Mon")]TimerInfo myTimer, ILogger log)
        //public static void Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var str = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            WeeklyBriefingForwarder variables = new WeeklyBriefingForwarder();

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $@"declare @currentdate datetime = GETDATE();

                            ;WITH cte_forwarders AS (
                                select AU.OrganisationId, AU.Email, AU.GivenName
                                from [dbo].[AspNetUsers] AU 
                                join [dbo].[Organisations] O on O.Id = AU.OrganisationId
                                WHERE AU.IsAdmin = 1
                                and O.OrganisationTypeID = 4
                            )
                            , cte_shippers_active AS (
                                select F.OrganisationId, F.Email, F.GivenName, SUM(CASE WHEN AU.Status = 'Active' THEN 1 ELSE 0 END) [ActiveShipperAccounts]
                                from cte_forwarders F
                                INNER JOIN [dbo].[Partnerships] P on P.ProviderId = F.OrganisationId
                                INNER JOIN [dbo].[AspNetUsers] AU on AU.OrganisationId = P.ClientId
                                GROUP BY F.OrganisationId, F.Email, F.GivenName
                            )
                            , cte_quote_quota AS (
                                select S.OrganisationId, S.Email, S.GivenName, S.ActiveShipperAccounts, Count(*) [TotalQuotes], Sum(Case when QR.State = 2 THEN 1 ELSE 0 END) [AcceptedQuotes], SUM(CASE WHEN QR.[State] = 0 THEN 1 ELSE 0 END) [OutstandingQuotes]
                                from cte_shippers_active S
                                INNER JOIN [dbo].[Partnerships] P on P.ProviderId = S.OrganisationId
                                LEFT JOIN [dbo].[QuotationRequests] QR ON QR.PartnershipId = P.Id
                                GROUP BY S.OrganisationId, S.Email, S.GivenName, S.ActiveShipperAccounts
                            )
                            , cte_quotes AS (
                                select Q.OrganisationId, Q.Email, Q.GivenName, Q.ActiveShipperAccounts, P.Id [PartnershipId], Q.TotalQuotes, Q.AcceptedQuotes, Q.OutstandingQuotes
                                from cte_quote_quota Q
                                    INNER JOIN [dbo].[Partnerships] P ON P.ProviderId = Q.OrganisationId
                                    LEFT JOIN [dbo].[QuotationRequests] QR ON QR.PartnershipId = P.Id
                                    LEFT JOIN [dbo].[FreightMovements] FM ON FM.Id = QR.FreightMovementId
                                GROUP BY Q.OrganisationId, P.Id, Q.Email, Q.GivenName, Q.ActiveShipperAccounts, Q.TotalQuotes, Q.AcceptedQuotes, Q.OutstandingQuotes
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
                            Email, GivenName, OutstandingQuotes, ActiveShipperAccounts, TotalQuotes, AcceptedQuotes
                            from (
                                select 
                                    P.Email, 
                                    P.GivenName,
                                    P.OutstandingQuotes, 
                                    P.ActiveShipperAccounts,
                                    P.TotalQuotes,
                                    P.AcceptedQuotes,
                                    Case when @currentdate < EstimatedCollectionDate and EstimatedCollectionDate < DATEADD(week, 1, @currentdate) then 1 else 0 end [Due for collection] ,
                                    Case when @currentdate < ETA and ETA < DATEADD(week, 1, @currentdate) then 1 else 0 end [Arrivals Expected] ,
                                    Case when ShipmentType = 1 and @currentdate < ETA and ETA < DATEADD(week, 1, @currentdate) then 1 else 0 end [Arrivals Expected Ship] ,
                                    Case when ShipmentType = 2 and @currentdate < ETA and ETA < DATEADD(week, 1, @currentdate) then 1 else 0 end [Arrivals Expected Air] ,
                                    Case when @currentdate < ETD and ETD < DATEADD(week, 1, @currentdate) then 1 else 0 end [Departures Expected] ,
                                    Case when ShipmentType = 1 and @currentdate < ETD and ETD < DATEADD(week, 1, @currentdate) then 1 else 0 end [Departures Expected Ship] ,
                                    Case when ShipmentType = 2 and @currentdate < ETD and ETD < DATEADD(week, 1, @currentdate) then 1 else 0 end [Departures Expected Air] ,
                                    Case when CustomsClearence = 1 and Delivered = 0 then 1 else 0 end [Due for Delivery] ,
                                    Case when EstimatedCollectionDate < @currentdate and Collected = 0 then 1 else 0 end [Collection Overdue] ,
                                    Case when ETD < @currentdate and DepartedPOL = 0 then 1 else 0 end [Departure Overdue],
                                    Case when ETA < @currentdate and ArrivedPOD = 0 then 1 else 0 end [Arrival Overdue],
                                    CAse when IssueAtCustoms = 1 and CustomsClearence = 0 then 1 else 0 end [Customs Issue],
                                    Case when EstimatedDeliveryDate < @currentdate then 1 else 0 end [Delivery Overdue]
                                from cte_quotes P
                                LEFT JOIN Shipments S on S.PartnershipId = P.PartnershipId
                            ) dt
                            group by Email, GivenName, OutstandingQuotes, ActiveShipperAccounts, TotalQuotes, AcceptedQuotes";

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
                            variables.QuotesPendingCount = reader.GetInt32(11).ToString();
                            variables.ShippersActiveCount = reader.GetInt32(12).ToString();

                            int acceptedQuotes = reader.GetInt32(14);
                            int totalQuotes = reader.GetInt32(13);

                            double convrate = ((double)acceptedQuotes / (double)totalQuotes);
                            variables.ConversionRatePercent = convrate.ToString("0.00%");

                            //This is for nicer formatting using Handlebars templates
                            if (variables.CollectionsCount == "0") variables.CollectionsCount = "";
                            if (variables.ArrivalsCount == "0") variables.ArrivalsCount = "";
                            if (variables.DeparturesCount == "0") variables.DeparturesCount = "";
                            if (variables.DeliveriesCount == "0") variables.DeliveriesCount = "";
                            if (variables.ExceptionsCount == "0") variables.ExceptionsCount = "";
                            if (variables.QuotesPendingCount == "0") variables.QuotesPendingCount = "";


                            //testing
                            //if(variables.Email == "patrick.mccloy@importwise.co") variables.Email = "n.lohani@tradefact.com";
                            EmailSender.SendSimpleMessage(variables, JsonConvert.SerializeObject(variables), "weeklybriefing", "Your Weekly Tradefact Update");
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
