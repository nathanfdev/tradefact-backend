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
    public static class QuoteCreatedEmail
    {
        [FunctionName("QuoteCreatedEmail")]
        public static void Run([ServiceBusTrigger("quotequeue", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);

            string template = "quotecreated";

            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "A new quote was requested");
        }

		private static ServiceBusInviteEmail<QuoteCreatedMail> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<QuoteCreatedMail>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<QuoteCreatedMail>));
        }
        
    }
}
