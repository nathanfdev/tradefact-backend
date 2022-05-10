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
    public static class OrderUpdateRequestEmail
    {
        [FunctionName("OrderUpdateRequestEmail")]
        public static void Run([ServiceBusTrigger("orderupdaterequest", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);
            string template;
            if (message.Body.Link == "") template = "poupdateunregisteredsupplier";
            else template = "poupdateconnectedsupplier";
            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "A user requests a status update on an order from you");
        }

		private static ServiceBusInviteEmail<OrderUpdate> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<OrderUpdate>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<OrderUpdate>));
        }
        
    }
}
