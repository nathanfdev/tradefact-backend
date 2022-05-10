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
    public static class InviteEmail
    {
        [FunctionName("InviteEmail")]
        public static void Run([ServiceBusTrigger("invitequeue", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);
            string template = message.Body.AlreadyRegistered ? 
                "alreadyregistered" :
                message.Body.InviteType switch
                {
                    (int)InviteType.NewEmployee => "invitation",
                    (int)InviteType.NewFreightForwarder => "freightforwarderinvite",
                    (int)InviteType.NewShipper => "shipperinvite",
                    (int)InviteType.NewFreightForwarderInvitedByShipper => "shipperinvite",
                    (int)InviteType.NewShipperNoPartnership => "freightforwarderinvite",
                    (int)InviteType.NewBuyer => "shipperinvite",
                    (int)InviteType.NewSupplier => "shipperinvite",
                    _ => "",
                };
            EmailSender.SendSimpleMessage(message.Body, JsonConvert.SerializeObject(message.Body), template, "Tradefact Invitation");
        }

		private static ServiceBusInviteEmail<InviteMail> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<InviteMail>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<InviteMail>));
        }
        
    }
}
