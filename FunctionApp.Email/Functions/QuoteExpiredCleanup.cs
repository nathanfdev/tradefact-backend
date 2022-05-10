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
    public static class QuoteExpiredCleanup
    {
#if false
        [FunctionName("QuoteExpiredCleanup")]
        public static void Run([TimerTrigger("0 0 3 * * *")] TimerInfo myTimer, ILogger log)
        //public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var str = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            using SqlConnection conn = new SqlConnection(str);
            conn.Open();
            var text = @"update QuotationRequests SET State = 3
                            where Id In (
                                Select QR.Id from QuotationRequests QR
                                join FreightMovements FM on FM.Id = QR.FreightMovementId
                                where (QR.State = 1 OR QR.[State] = 0) and FM.GoodsReady < DATEADD(day, -1, GETDATE())
                            )";

            using SqlCommand cmd = new SqlCommand(text, conn);
            var rows = cmd.ExecuteNonQuery();
            log.LogInformation($"{rows} rows were updated");

        }
#endif
    }
}
