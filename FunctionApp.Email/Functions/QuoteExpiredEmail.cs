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
    public static class QuoteExpiredEmail
    {
        [FunctionName("QuoteExpiredEmail")]
        public static void Run([ServiceBusTrigger("quoteexpiredqueue", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);

            string template = "quoteexpired";

            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "One of your quote requests has expired");
        }

		private static ServiceBusInviteEmail<QuoteExpiredMail> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<QuoteExpiredMail>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<QuoteExpiredMail>));
        }
        
    }
}
