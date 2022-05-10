using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.ServiceBus
{
    public class ServiceBusMessage<T>
    {
        private T _bodyValue;

        public ServiceBusMessage()
        {

        }

        public ServiceBusMessage(T value)
        {
            _bodyValue = value;
        }

        [DataMember]
        public DateTimeOffset EnqueueTime { get; } = DateTimeOffset.UtcNow;

        [DataMember]
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
        public T Body { get { return _bodyValue; } set { _bodyValue = value; } }
    }

    public interface IServiceBusClient
    {
        Task Publish<T>(ServiceBusMessage<T> message, string desitination);

        Task Send<T>(ServiceBusMessage<T> message, string topic);
    }
}
