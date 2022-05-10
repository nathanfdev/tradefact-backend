using Core.Models.External;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.External.API.Model;

namespace Tradefact.External.API.Helpers
{
    public class MappingRegister: IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<PurchaseOrder, ExternalPurchaseOrder>()
				.AddDestinationTransform((string x) => x.Trim())
				.Ignore(dest => dest.LineItems)
				.IgnoreNullValues(true)
				.Map(dest => dest.ExternalID, src => src.PurchaseOrderID ?? Guid.NewGuid().ToString())
				.Map(dest => dest.Tags, src => src.Tags.Count() > 0 ? String.Join(",", src.Tags) : null);

			config.NewConfig<LineItem, ExternalPurchaseOrderLineItem>()
				.AddDestinationTransform((string x) => x.Trim())
				.Ignore(dest => dest.ExternalPurchaseOrderId)
				.Map(dest => dest.Tags, src => src.Tags.Count() > 0 ? String.Join(",", src.Tags) : null);
		}
	}
}
