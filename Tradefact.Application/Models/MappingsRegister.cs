using Core.Enums;
using Core.Models;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tradefact.Application.FreightMovements;
using Tradefact.Application.FreightMovements.Queries.Model;
using Tradefact.Application.Models.Collab;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Application.Schedules.Model;

namespace Tradefact.Application.Models
{
	public class MappingRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<FreightMovementDTO, FreightMovementResource>()
				.Map(dest => dest.ShipmentMethod, src => src.ShipmentType)
				.Map(dest => dest.PortOfLoading, src => src.PortOfLoadingId)
				.Map(dest => dest.PortOfDischarge, src => src.PortOfDischargeId)
				.Map(dest => dest.PortOfLoadingInfo, src => 
					new Location { 
						Name = src.PortOfLoading_Name, 
						IATA = src.PortOfLoadingId, 
						LocCode = src.PortOfLoadingId, 
						Position = new GeographicPosition { 
							Latitude = (double)src.PortOfLoading_PositionLatitude.GetValueOrDefault(), 
							Longitude = (double)src.PortOfLoading_PositionLongitude.GetValueOrDefault()
						},
						CountryCode = src.PortOfLoading_CountryCode,
					}
				)
				.Map(dest => dest.PortOfDischargeInfo, src =>
					new Location
					{
						Name = src.PortOfDischarge_Name,
						IATA = src.PortOfDischargeId,
						LocCode = src.PortOfDischargeId,
						Position = new GeographicPosition
						{
							Latitude = (double)src.PortOfDischarge_PositionLatitude.GetValueOrDefault(),
							Longitude = (double)src.PortOfDischarge_PositionLongitude.GetValueOrDefault()
						},
						CountryCode = src.PortOfDischarge_CountryCode,
					}
				)
				.Map(dest => dest.PlaceOfDispatchInfo, src =>
					new AddressResource
					{
						Id = src.PlaceOfDispatch_Id.GetValueOrDefault().ToString(),
						Name = src.PlaceOfDispatch_Name,
						AddressLine1 = src.PlaceOfDispatch_AddressLine1,
						AddressLine2 = src.PlaceOfDispatch_AddressLine2,
						AddressLine3 = src.PlaceOfDispatch_AddressLine3,
						AddressLine4 = src.PlaceOfDispatch_AddressLine4,
						PostalCode = src.PlaceOfDispatch_PostalCode,
						City = src.PlaceOfDispatch_City,
						County = src.PlaceOfDispatch_County,
						Province = src.PlaceOfDispatch_Province,
						Country = new CountryResource { Code = src.PlaceOfDispatch_CountryCode, Name = src.PlaceOfDispatch_Country },
					}
				)
				.Map(dest => dest.Tags, src => src.TagsRaw != null ? src.TagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())
				.Map(dest => dest.HsCodes, src => src.HsCodesRaw != null ? src.HsCodesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());

			config.NewConfig<FreightMovementItemDTO, FreightMovementItemResource>()
				.Map(dest => dest.PlaceOfLoadingInfo, src =>
					new AddressResource
					{
						Id = src.PlaceOfLoading_Id.GetValueOrDefault().ToString(),
						Name = src.PlaceOfLoading_Name,
						AddressLine1 = src.PlaceOfLoading_AddressLine1,
						AddressLine2 = src.PlaceOfLoading_AddressLine2,
						AddressLine3 = src.PlaceOfLoading_AddressLine3,
						AddressLine4 = src.PlaceOfLoading_AddressLine4,
						PostalCode = src.PlaceOfLoading_PostalCode,
						City = src.PlaceOfLoading_City,
						County = src.PlaceOfLoading_County,
						Province = src.PlaceOfLoading_Province,
						Position = new GeographicPosition { Latitude = src.PlaceOfLoading_Latitude, Longitude = src.PlaceOfLoading_Longitude },
						Country = new CountryResource { Code = src.PlaceOfLoading_CountryCode, Name = src.PlaceOfLoading_Country },
					}
				)
				.Map(dest => dest.IsPOSchedule, src => src.IsPoSchedule)
				.Map(dest => dest.PurchaseOrderTags, src => src.PurchaseOrderTagsRaw != null ? src.PurchaseOrderTagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());

			config.NewConfig<CargoItemDTO, CargoItemResource>()
				.Map(dest => dest.Product, src => new ProductResource
				{
					// Reverted by Patrick 04/03/21 - Name to Product_description
					Id = src.Product_Id.GetValueOrDefault().ToString(),
					ProductId = src.Product_Id.GetValueOrDefault().ToString(),
					Nickname = src.Product_Nickname,
					Name = src.Product_Name,
					Description = src.Product_Description,
					SKU = src.Product_Sku,
					Packing = src.Product_Packing,
					ThumbnailBlobUrl=src.Product_ThumbnailBlobUrl,
					Dimensions = new ImportProductDimensions
					{
						Width = src.Product_Dimensions_Width,
						Height = src.Product_Dimensions_Height,
						Length = src.Product_Dimensions_Length,
						Scale = src.Product_Dimensions_Scale,
						Weight = src.Product_Dimensions_Weight,
						weightMeasurement = src.Product_Dimensions_WeightMeasurement,
					},
					UnitsPerPackage = src.Product_UnitsPerPackage,
					HsCode = src.Product_HsCode,
					HazardClass = src.Product_HazardClass,
					HazardousContents = (Core.Enums.HazardContentsEnum)src.Product_HazardousContents,
					MagneticFieldContained = src.Product_MagneticFieldContained,
					LithiumBatteryPacking = src.Product_LithiumBatteryPacking
				})
				.Map(dest => dest.ProductId, src => src.Product_Id.GetValueOrDefault().ToString())
				.Map(dest => dest.HsCode, src => src.HsCode ?? src.Product_HsCode)
				.Map(dest => dest.SKU, src => src.Sku ?? src.Product_Sku)
				.Map(dest => dest.UOL, src => src.UOL ?? src.Product_Dimensions_Scale)
				.Map(dest => dest.UOW, src => src.UOW ?? src.Product_Dimensions_WeightMeasurement)
				.Map(dest => dest.Length, src => src.Length > 0 ? src.Length : src.Product_Dimensions_Length)
				.Map(dest => dest.Weight, src => src.Weight > 0 ? src.Weight : src.Product_Dimensions_Weight)
				.Map(dest => dest.Width, src => src.Width > 0 ? src.Width : src.Product_Dimensions_Width)
				.Map(dest => dest.Height, src => src.Height > 0 ? src.Height : src.Product_Dimensions_Height);


			config.NewConfig<FreightMovement, FreightMovementResource>()
				.Map(dest => dest.ShipmentMethod, src => src.ShipmentType)
				.Map(dest => dest.PortOfLoading, src => src.PortOfLoadingId)
				.Map(dest => dest.PortOfDischarge, src => src.PortOfDischargeId)
				.Map(dest => dest.PlaceOfLoading, src => src.PlaceOfLoadingId)
				.Map(dest => dest.PlaceOfDispatch, src => src.PlaceOfDispatchId)
				.Map(dest => dest.PortOfLoadingInfo, src => src.PortOfLoading)
				.Map(dest => dest.PortOfDischargeInfo, src => src.PortOfDischarge)
				.Map(dest => dest.PlaceOfLoadingInfo, src => src.PlaceOfLoading)
				.Map(dest => dest.PlaceOfDispatchInfo, src => src.PlaceOfDispatch)
				.Map(dest => dest.Tags, src => src.Tags != null ? src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())
				.Map(dest => dest.HsCodes, src => src.HSCodes != null ? src.HSCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());

			config.NewConfig<CreateFreightMovementCommand, FreightMovement>()
				.Map(dest => dest.ShipmentType, src => src.ShipmentMethod)
				.Map(dest => dest.InsuranceRequired, src => src.InsuranceRequired)
				.Map(dest => dest.InsuranceCurrency, src => src.InsuranceCurrency)
				.Map(dest => dest.InsuranceValue, src => src.InsuranceValue)
				.Map(dest => dest.PlaceOfLoadingId, src => src.PlaceOfLoading)
				.Map(dest => dest.PortOfLoadingId, src => src.PortOfLoading)
				.Map(dest => dest.PortOfDischargeId, src => src.PortOfDischarge)
				.Map(dest => dest.PlaceOfDispatchId, src => src.PlaceOfDispatch)
				.Map(dest => dest.Tags, src => src.TagList)
				.Map(dest => dest.HSCodes, src => src.HsCodeList)
				.Ignore(dest => dest.PortOfLoading)
				.Ignore(dest => dest.PortOfDischarge)
				.Ignore(dest => dest.PlaceOfLoading)
				.Ignore(dest => dest.PlaceOfDispatch);

			config.NewConfig<Quotation, QuotationResource>()
				.Map(dest => dest.DefaultTerms, src => src.TermsAndConditions)
				.Map(dest => dest.DetailedTerms, src => src.DetailedTermsAndConditions)
				.Map(dest => dest.Notes, src => src.Notes)
				.Map(dest => dest.Routes, src => JsonConvert.DeserializeObject<List<RouteSchedule>>(src.Routes ?? ""));

			config.NewConfig<QuotationRequest, QuotationRequestResource>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.FreightMovement, src => src.FreightMovement)
				.Map(dest => dest.Provider, src => src.Partnership.Provider)
				.Map(dest => dest.Client, src => src.Partnership.Client);


			config.NewConfig<ExtendedMessage, Message>()
				.Map(dest => dest.PostedBy, src => new User { DisplayName = src.FullName, Avatar = src.ProfileImage  });

			config.NewConfig<PurchaseOrderDTO, PurchaseOrderResource>()
				.Map(dest => dest.Id, src => src.PurchaseOrderId)
				.Map(dest => dest.Tags, src => src.Tags != null ? src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())
				.Map(dest => dest.NoOfContainers, src => src.Containers != null ? src.Containers.Count() : 0)
				.Map(dest => dest.Containers, src => src.Containers != null ? src.Containers.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())

				.Map(dest => dest.HSCodes, src => src.HSCodes != null ? src.HSCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())
				.Map(dest => dest.ProductHSCodes, src => src.Product_HSCodes != null ? src.Product_HSCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>())


				.Map(dest => dest.IncoTerms, src => (src.IncoTerms.HasValue && src.IncoTerms.Value > 0) ? (int?)src.IncoTerms.Value : null)
				.Map(dest => dest.ShipmentType, src => (src.ShipmentType.HasValue && src.ShipmentType.Value > 0) ? (int?)src.ShipmentType.Value : null)
				.Map(dest => dest.TransactionType, src => (src.TransactionType.HasValue && src.TransactionType.Value >= 0) ? (int?)src.TransactionType.Value : null)
				.Map(dest => dest.LoadType, src => (src.LoadType.HasValue && src.LoadType > 0) ? (int?)src.LoadType.Value : null)

				.Map(dest => dest.CreationDate, src => src.CreationDateInternal)
				.Map(dest => dest.LastModifiedDate, src => src.LastModifiedOnInternal)

				.Map(dest => dest.IncoTermsDesc, src => (src.IncoTerms.HasValue && src.IncoTerms.Value > 0) ? src.IncoTerms.Value.ToString() : null)
				.Map(dest => dest.ShipmentTypeDesc, src => (src.ShipmentType.HasValue && src.ShipmentType.Value > 0) ? src.ShipmentType.Value.ToString() : null)
				.Map(dest => dest.TransactionTypeDesc, src => (src.TransactionType.HasValue && src.TransactionType.Value >= 0) ? src.TransactionType.Value.ToString() : null)
				.Map(dest => dest.LoadTypeDesc, src => (src.LoadType.HasValue && src.LoadType > 0) ? src.LoadType.Value.ToString() : null)

				.Map(dest => dest.PortOfLoading, src => (src.PortOfLoadingId != null) ? new Port { Code = src.PortOfLoadingId, Name = src.PortOfLoading } : null)

				.Map(dest => dest.PortOfDischarge, src => (src.PortOfDischargeId != null) ? new Port { Code = src.PortOfDischargeId, Name = src.PortOfDischarge } : null)
				
				.Map(dest => dest.PlaceOfLoading, src => (src.PlaceOfLoadingId != null && src.PlaceOfLoadingId != Guid.Empty) ? new AddressResource { 
					Id = src.PlaceOfLoadingId.ToString(),
					Name = src.PlaceofLoading_Name,
					AddressLine1 = src.PlaceofLoading_AddressLine1,
					AddressLine2 = src.PlaceofLoading_AddressLine2,
					AddressLine3 = src.PlaceofLoading_AddressLine3,
					AddressLine4 = src.PlaceofLoading_AddressLine4,
					City = src.PlaceofLoading_City,
					PostalCode = src.PlaceofLoading_PostalCode,
					Country = (src.PlaceofLoading_Country_Code != null) ? new CountryResource { Code = src.PlaceofLoading_Country_Code, Name = src.PlaceofLoading_Country_Name }: null
				} : null)

				.Map(dest => dest.PlaceOfDispatch, src => (src.PlaceOfDispatchId != null && src.PlaceOfDispatchId != Guid.Empty) ? new AddressResource
				{
					Id = src.PlaceOfDispatchId.ToString(),
					Name = src.PlaceofDispatch_Name,
					AddressLine1 = src.PlaceofDispatch_AddressLine1,
					AddressLine2 = src.PlaceofDispatch_AddressLine2,
					AddressLine3 = src.PlaceofDispatch_AddressLine3,
					AddressLine4 = src.PlaceofDispatch_AddressLine4,
					City = src.PlaceofDispatch_City,
					PostalCode = src.PlaceofDispatch_PostalCode,
					Country = (src.PlaceofDispatch_Country_Code != null) ? new CountryResource { Code = src.PlaceofDispatch_Country_Code, Name = src.PlaceofDispatch_Country_Name } : null
				} : null)

				.Map(dest => dest.LogisticsQuotation, src => new PurchaseOrderShippingQuoteStatus
				{
					Requested = src.ShippingQuote_Requested,
					Pending = src.ShippingQuote_Pending,
					Accepted = src.ShippingQuote_Accepted,
					Ready = src.ShippingQuote_Ready,
					Rejected = src.ShippingQuote_Rejected,
					Expired = src.ShippingQuote_Expired
				})

				.Map(dest => dest.NoSchedules, src => (src.Schedules != null && src.Schedules.Count > 0) ? src.Schedules.Count : 0)
				.Map(dest => dest.Schedules, src => (src.Schedules != null && src.Schedules.Count > 0) ? src.Schedules.Adapt<List<PurchaseOrderScheduleInformationResource>>() : null)


				.Map(dest => dest.NoShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count : 0)

				.Map(dest => dest.SeaShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.SEA) : 0)
				.Map(dest => dest.AirShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.AIR) : 0)
				.Map(dest => dest.RoadShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.ROAD) : 0)
				.Map(dest => dest.RailShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.RAIL) : 0)

				.Map(dest => dest.OrderShipmentStatus, src => src.OrderShipmentStatus)

				.Map(dest => dest.ShipmentsBooked, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.Booked && !q.InTransit && !q.Delivered) : 0)
				.Map(dest => dest.ShipmentsInTransit, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.InTransit && !q.Delivered) : 0)
				.Map(dest => dest.ShipmentsBooked, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.Delivered) : 0)
				.Map(dest => dest.Shipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Adapt<List<PurchaseOrderShipmentInformationResource>>() : null)


				.Map(dest => dest.Supplier, src => (src.SupplierId != null && src.SupplierId != Guid.Empty) ? new DirectoryResource { Id = src.SupplierId, Name = src.Supplier, Currency = src.Currency, NetworkType = src.SupplierParentId == Guid.Empty ? NetworkConnectionTypeEnum.CONNECTED : NetworkConnectionTypeEnum.MANAGED } : null)
				.Map(dest => dest.BaseCurrency, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.BaseCurrency_NetAmount, TaxAmount = src.BaseCurrency_TaxAmount, TotalAmount = src.BaseCurrency_TotalAmount } )
				.Map(dest => dest.ItemsTotal, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.Items_NetAmount, TaxAmount = src.Items_TaxAmount, TotalAmount = src.Items_TotalAmount })
				.Map(dest => dest.ChargesTotal, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.Charges_NetAmount, TaxAmount = src.Charges_TaxAmount, TotalAmount = src.Charges_TotalAmount })
				.Map(dest => dest.Total, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.Total_NetAmount, TaxAmount = src.Total_TaxAmount, TotalAmount = src.Total_TotalAmount });
				

			config.NewConfig<PurchaseOrderItemDTO, PurchaseOrderItemResource>()
				.Map(dest => dest.Product, src => new ProductResource { 
					Id = src.ProductId.ToString(),
					// Reverted by Patrick 04/03/21 - Name to Product_description
					Name = src.Product_Name	,
					Description = src.Product_Description,
					Tags = src.Product_Tags,
					SKU = src.Product_Sku,
					Packing = src.Product_Packing,
					ThumbnailBlobUrl=src.Product_ThumbnailBlobUrl,
					Dimensions = new ImportProductDimensions { 
						Width = src.Product_DimensionsWidth, 
						Height = src.Product_DimensionsHeight, 
						Length = src.Product_DimensionsLength, 
						Weight = src.Product_DimensionsWeight,
						Scale = src.Product_DimensionsScale,
						weightMeasurement = src.Product_DimensionsWeightMeasurement
					},
					ProductId = src.MasterProductId.ToString(),
					HsCode = src.Product_HsCode,
					HazardClass = src.Product_HazardClass,
					HazardousContents = (Core.Enums.HazardContentsEnum)src.Product_HazardousContents,
					MagneticFieldContained = src.Product_MagneticFieldContained,
					LithiumBatteryPacking = src.Product_LithiumBatteryPacking
				})
				.Map(dest => dest.PurchaseOrderItemId, src => src.Id)


				.Map(dest => dest.NoSchedules, src => (src.Schedules != null && src.Schedules.Count > 0) ? src.Schedules.Count : 0)
				.Map(dest => dest.Schedules, src => (src.Schedules != null && src.Schedules.Count > 0) ? src.Schedules.Adapt<List<PurchaseOrderScheduleInformationResource>>() : null)


				.Map(dest => dest.NoShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count : 0)
				.Map(dest => dest.ShipmentsBooked, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.Booked && !q.InTransit && !q.Delivered) : 0)
				.Map(dest => dest.ShipmentsInTransit, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.InTransit && !q.Delivered) : 0)
				.Map(dest => dest.ShipmentsBooked, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.Delivered) : 0)
				.Map(dest => dest.ShipmentsQuotationRequested, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.QuotationRequested) : 0)

				.Map(dest => dest.SeaShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.SEA) : 0)
				.Map(dest => dest.AirShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.AIR) : 0)
				.Map(dest => dest.RoadShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.ROAD) : 0)
				.Map(dest => dest.RailShipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Count(q => q.ShipmentType == Core.Enums.ShipmentTypeEnum.RAIL) : 0)

				.Map(dest => dest.OrderShipmentStatus, src => src.OrderShipmentStatus)


				.Map(dest => dest.Shipments, src => (src.Shipments != null && src.Shipments.Count > 0) ? src.Shipments.Adapt<List<PurchaseOrderShipmentInformationResource>>() : null)

				.Map(dest => dest.NetPriceAmount, src => src.OrderQuantity * src.OrderPriceUnit);

			config.NewConfig<PurchaseOrderShipmentInformationDTO, PurchaseOrderShipmentInformationResource>()
				.Map(dest => dest.Tags, src => src.Tags != null ? src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());

			config.NewConfig<PurchaseOrderScheduleLineResourceDTO, PurchaseOrderScheduleLineResource>()
				.Map(dest => dest.PlaceOfLoading, src => new Address
				{
					Name = src.PlaceOfLoading,
					AddressLine1 = src.PlaceOfLoading_Address1,
					AddressLine2 = src.PlaceOfLoading_Address2,
					AddressLine3 = src.PlaceOfLoading_Address3,
					AddressLine4 = src.PlaceOfLoading_Address4,
					City = src.PlaceOfLoading_City,
					CountryCode = src.CountryofLoadingCode,
					Country = new Country { Code2 = src.CountryofLoadingCode, Name = src.Country },
				})
				.Map(dest => dest.Value, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.LineValue, TotalAmount = src.LineValue });


			config.NewConfig<ShipmentScheduleInfoDTO, ShipmentScheduleInfo>()
				.Map(dest => dest.Address, src => new Address
				{
					Id = src.AddressId,
					Name = src.PlaceOfLoading,
					AddressLine1 = src.AddressLine1,
					AddressLine2 = src.AddressLine2,
					AddressLine3 = src.AddressLine3,
					AddressLine4 = src.AddressLine4,
					City = src.City,
					CountryCode = src.CountryCode,
					Country = new Country { Code2 = src.CountryCode, Name = src.Country },
				});

			config.NewConfig<PurchaseOrderScheduleLineItemResourceDTO, PurchaseOrderScheduleLineItemResource>()
				.Map(dest => dest.Product, src => new ProductResource
				{
					Id = src.ProductId.ToString(),
					// Reverted by Patrick 04/03/21 - Name to Product_description
					Name = src.Product_Name,
					Description = src.Product_Description,
					SKU = src.Product_Sku,
					Packing = src.Product_Packing,
					Dimensions = new ImportProductDimensions
					{
						Width = src.Product_DimensionsWidth,
						Height = src.Product_DimensionsHeight,
						Length = src.Product_DimensionsLength,
						Weight = src.Product_DimensionsWeight,
						Scale = src.Product_DimensionsScale,
						weightMeasurement = src.Product_DimensionsWeightMeasurement
					},
					ProductId = src.ProductId.ToString(),
					HsCode = src.Product_HsCode,
					HazardClass = src.Product_HazardClass,
					HazardousContents = (Core.Enums.HazardContentsEnum)src.Product_HazardousContents,
					MagneticFieldContained = src.Product_MagneticFieldContained,
					LithiumBatteryPacking = src.Product_LithiumBatteryPacking
				})
				.Map(dest => dest.Value, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.LineValue, TotalAmount = src.LineValue });

			config.NewConfig<PurchaseOrderScheduleInformationDTO, PurchaseOrderScheduleInformationResource>()
				.Map(dest => dest.PlaceOfLoading, src => new Address
				{
					Name = src.PlaceOfLoading,
					AddressLine1 = src.PlaceOfLoading_Address1,
					AddressLine2 = src.PlaceOfLoading_Address2,
					AddressLine3 = src.PlaceOfLoading_Address3,
					AddressLine4 = src.PlaceOfLoading_Address4,
					City = src.PlaceOfLoading_City,
					CountryCode = src.CountryofLoadingCode,
					Country = new Country { Code2 = src.CountryofLoadingCode, Name = src.Country },
				})
				.Map(dest => dest.Value, src => new Total { CurrencyId = src.CurrencyId, NetAmount = src.LineValue, TotalAmount = src.LineValue });
		}

	}
}
