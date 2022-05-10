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
    public static class PartnershipRequestEmail
    {
        [FunctionName("PartnershipRequestEmail")]
        public static void Run([ServiceBusTrigger("partnershiprequest", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);
            string template = "partnershiprequest";
            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "A user wants to connect with you on Tradefact");
        }

		private static ServiceBusInviteEmail<PartnershipRequest> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<PartnershipRequest>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<PartnershipRequest>));
        }
        
    }
}
