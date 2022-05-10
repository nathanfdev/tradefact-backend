using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Email
{

	public class ServiceBusInviteEmail<T>
	{
		public string EnqueueTime { get; set; }
		public string CorrelationId { get; set; }
		public T Body { get; set; }
	}
}
