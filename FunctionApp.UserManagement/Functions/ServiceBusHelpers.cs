using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tradefact.UserManagement.Model;

namespace Tradefact.UserManagement
{
    public static class ServiceBusHelpers
    {
        private static string serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);

        public static async Task SendMessageAsync(string payload, string queueName)
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
