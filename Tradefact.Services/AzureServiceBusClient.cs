using Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tradefact.Services
{
    public interface IServiceBusPersistedConnection : IDisposable
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ServiceBusConnection Connection { get; }

        ITopicClient GetTopicClient();
        IQueueClient GetQueueClient();
    }

    public class ServiceBusPersistedConnection : IServiceBusPersistedConnection
    {
        private readonly ILogger<ServiceBusPersistedConnection> _logger;
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private ITopicClient _topicClient;
        private IQueueClient _queueClient;

        bool _disposed;

        public ServiceBusPersistedConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
            ILogger<ServiceBusPersistedConnection> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ??
                throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        public ITopicClient GetTopicClient()
        {
            if (_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
            }

            return _topicClient;
        }

        public ServiceBusConnection Connection => new ServiceBusConnection(_serviceBusConnectionStringBuilder);

        public IQueueClient GetQueueClient()
        {
            if (_queueClient == null || _queueClient.IsClosedOrClosing)
            {
                _queueClient = new QueueClient(_serviceBusConnectionStringBuilder, ReceiveMode.PeekLock, RetryPolicy.Default);
            }
            return _queueClient;
        }


        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
        }

    }


    public class AzureServiceBusClient : IServiceBusClient
    {
        private readonly IServiceBusPersistedConnection _serviceBusPersisterConnection;
        private readonly ILogger<AzureServiceBusClient> _logger;
        private readonly RetryPolicy _retryPolicy;

        private ITopicClient _topicClient => _serviceBusPersisterConnection.GetTopicClient();
        private IQueueClient _queueClient => _serviceBusPersisterConnection.GetQueueClient();

        public AzureServiceBusClient(IServiceBusPersistedConnection serviceBusPersisterConnection, ILogger<AzureServiceBusClient> logger)
        {
            _serviceBusPersisterConnection = serviceBusPersisterConnection;
            _logger = logger;
        }

        public async Task Send<T>(ServiceBusMessage<T> message, string topic)
        {

            var client = new TopicClient(_serviceBusPersisterConnection.Connection, topic, _retryPolicy);
            await client.SendAsync(MapIServiceBusMessageToConcreteAzureMessage(message));
        }

        public async Task Publish<T>(ServiceBusMessage<T> message, string destination)
        {
            _serviceBusPersisterConnection.ServiceBusConnectionStringBuilder.EntityPath = destination;
            var client = new QueueClient(_serviceBusPersisterConnection.ServiceBusConnectionStringBuilder, ReceiveMode.PeekLock, _retryPolicy);
            await client.SendAsync(MapIServiceBusMessageToConcreteAzureMessage(message));
        }

        private Message MapIServiceBusMessageToConcreteAzureMessage<T>(ServiceBusMessage<T> message)
        {
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}