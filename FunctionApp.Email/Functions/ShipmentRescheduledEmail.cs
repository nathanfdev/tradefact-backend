using System;
using Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Core.Enums;
using FunctionApp.Email.Common;
using Core.Models.Email;

namespace FunctionApp.Email
{
    public static class ShipmentRescheduledEmail
    {
        [FunctionName("ShipmentRescheduledEmail")]
        public static void Run([ServiceBusTrigger("reschedulequeue", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);

            string template = "rescheduleshipper";

            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "A shipment was rescheduled");
        }

		private static ServiceBusInviteEmail<ShipmentRescheduledMail> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<ShipmentRescheduledMail>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<ShipmentRescheduledMail>));
        }
        
    }
}
