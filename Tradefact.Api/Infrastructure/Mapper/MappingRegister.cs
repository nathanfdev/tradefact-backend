using Core.Dtos;
using Core.Models;
using Core.Models.Extended;
using Core.Models.External;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tradefact.Api.Model;
using Tradefact.Api.Model.User;
using Tradefact.Api.Responses;
using Tradefact.Application.Models;
using Tradefact.Data;
using Tradfact.Api.Requests;

namespace Tradefact.Api.Infrastructure.Mapper
{
	public class MappingRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<Country, CountryInfo>()
				.Map(dest => dest.Code, src => src.Code2)
				.Map(dest => dest.Name, src => src.Name);

			config.NewConfig<Country, CountryResource>()
				.Map(dest => dest.Code, src => src.Code2)
				.Map(dest => dest.Name, src => src.Name);

			config.NewConfig<ConnectionContact, ContactResource>()
				.Map(dest => dest.Email, src => src.Email.Any() ? src.Email[0] : null)
				.Map(dest => dest.Phone, src => src.Phone.Any() ? src.Phone[0] : null);

			config.NewConfig<OrganisationContact, ContactResource>()
				.Map(dest => dest.Email, src => src.Email.Any() ? src.Email[0] : null)
				.Map(dest => dest.Phone, src => src.Phone.Any() ? src.Phone[0] : null);

			config.NewConfig<CreateProductRequest, Product>()
				.Map(dest => dest.MagneticFieldContained, src => src.MagneticFieldContained.GetValueOrDefault())
				.Map(dest => dest.Rotatable, src => src.Rotatable.GetValueOrDefault())
				.Map(dest => dest.Stackable, src => src.Stackable.GetValueOrDefault())
				.Map(dest => dest.DataComplete, src => src.DataComplete != null ?
					(bool)src.DataComplete :
					src.Name != null &&
					src.SKU != null &&
					src.Packing != null &&
					src.UnitsPerPackage > 0 &&
					src.Dimensions.MeasurementsSet());

			config.NewConfig<UpdateProductRequest, Product>()
				.Map(dest => dest.MagneticFieldContained, src => src.MagneticFieldContained.GetValueOrDefault())
				.Map(dest => dest.Rotatable, src => src.Rotatable.GetValueOrDefault())
				.Map(dest => dest.Stackable, src => src.Stackable.GetValueOrDefault())
				.Map(dest => dest.DataComplete, src => src.DataComplete != null ?
					(bool)src.DataComplete :
					src.Name != null &&
					src.SKU != null &&
					src.Packing != null &&
					src.UnitsPerPackage > 0 &&
					src.Dimensions.MeasurementsSet())
				.Ignore(src=> src.Suppliers);

			config.NewConfig<Organisation, DirectoryResource>()
				.Map(dest => dest.AddressCount, src => src.Addresses.Any() ? src.Addresses.Count(q => q.IsActive) : 0);

			config.NewConfig<BuyerInfo, DirectoryResource>()
				.Map(dest => dest.AddressCount, src => src.Locations)
				.Map(dest => dest.ContactCount, src => src.Contacts);

			config.NewConfig<SupplierInfo, DirectoryResource>()
				.Map(dest => dest.AddressCount, src => src.Locations)
				.Map(dest => dest.ContactCount, src => src.Contacts);

			config.NewConfig<ConnectionInfo, DirectoryResource>()
				.Map(dest => dest.AddressCount, src => src.Locations)
				.Map(dest => dest.ContactCount, src => src.Contacts);

			config.NewConfig<ConnectionInfo, SortedDirectoryResource>()
				.Map(dest => dest.AddressCount, src => src.Locations)
				.Map(dest => dest.ContactCount, src => src.Contacts);


			config.NewConfig<Organisation, OrganisationResource>()
				.Map(dest => dest.InvoiceAddress, src => src.Addresses.Any() ? src.Addresses.FirstOrDefault(q => q.IsActive && q.IsInvoiceAddress) : null);

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


			config.NewConfig<FreightMovementRequest, FreightMovement>()
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

			config.NewConfig<Partnership, PartnershipResource>()
				.Map(dest => dest.PartnershipId, src => src.Id)
				.Map(dest => dest.Provider, src => src.Provider)
				.Map(dest => dest.Client, src => src.Client);

			config.NewConfig<QuotationCreateRequest, Quotation>()
				.Ignore(dest => dest.OriginCharges)
				.Ignore(dest => dest.FreightCharges)
				.Ignore(dest => dest.DestinationCharges)
				.Ignore(dest => dest.AdditionalCharges)
				.Map(dest => dest.TermsAndConditions, src => src.DefaultTerms)
				.Map(dest => dest.DetailedTermsAndConditions, src => src.DetailedTerms)
				.Map(dest => dest.Notes, src => src.Notes)
				.Map(dest => dest.Routes, src => JsonConvert.SerializeObject(src.Routes));

			config.NewConfig<Quotation, QuotationResource>()
				.Map(dest => dest.DefaultTerms, src => src.TermsAndConditions)
				.Map(dest => dest.DetailedTerms, src => src.DetailedTermsAndConditions)
				.Map(dest => dest.Notes, src => src.Notes)
				.Map(dest => dest.Routes,src => JsonConvert.DeserializeObject<List<RouteSchedule>>(src.Routes ?? ""));

            config.NewConfig<ProductVariant, ProductVariantResource>()
                .Map(dest => dest.Product, src => src.Product)
                .Map(dest => dest.Supplier, src => src.Supplier);

			config.NewConfig<ProductOrderInformation, ProductResource>()
				.Map(dest => dest.Dimensions, src => new ImportProductDimensions {
					Height = src.Dimensions_Height,
					Weight = src.Dimensions_Weight,
					Length = src.Dimensions_Length,
					Scale = src.Dimensions_Scale,
					weightMeasurement = src.Dimensions_weightMeasurement,
					Width = src.Dimensions_Width
				});
			//        config.NewConfig<ProductOrderInformation, ProductResource>()
			//.Map(dest => dest.Dimensions, src => new ImportProductDimensions
			//{
			//	Height = src.Dimensions.Height,
			//	Length = src.Dimensions.Length,
			//	Width = src.Dimensions.Width,
			//	Scale = src.Dimensions.Scale,
			//	Weight = src.Dimensions.Weight,
			//	weightMeasurement = src.Dimensions.weightMeasurement
			//});

			//config.NewConfig<QuotationRequest, QuotationRequestResource>()
			//	.Map(dest => dest.Id, src => src.Id)
			//	.Map(dest => dest.FreightMovement, src => src.FreightMovement)
			//	.Map(dest => dest.Provider, src => src.Partnership.Provider)
			//	.Map(dest => dest.Client, src => src.Partnership.Client);

			config.NewConfig<Currency, CurrencyResource>()
				.Map(dest => dest.Code, src => src.CurrencyCode)
				.Map(dest => dest.Currency, src => src.CurrencyName)
				.Map(dest => dest.Countries, src => src.Countries.Select(s=> new CurrencyCountryResource { Name = s.Country.Name, Code2 = s.Country.Code2, Code3 = s.Country.Code3 } ))
				.Map(dest => dest.SearchKeywords, src => $"{src.CurrencyCode}, {src.CurrencyName}, { String.Join(", ", src.Countries.Select(s => s.Country.Name))}");

			//config.NewConfig<CurrencyCountry, CurrencyCountryResource>()
			//	.Map(dest => dest.Code2, src => src.Country.Code2)
			//	.Map(dest => dest.Code3, src => src.Country.Code3)
			//	.Map(dest => dest.Name, src => src.Country.Name);

			config.NewConfig<Document, DocumentInfo>()
				.Map(dest => dest.BlobUrl, src => String.IsNullOrEmpty(src.BlobUrl) ? null : new Uri(src.BlobUrl))
				.Map(dest => dest.Name, src => WebUtility.HtmlDecode(src.Name));

			config.NewConfig<ApplicationUser, UserResource>()
				.Map(dest => dest.Roles, src => (src.IsAdmin) ? new List<String> { "Admin" } : new List<String> { "User" });

			config.NewConfig<CreateAddressRequest, Address>()
				.Map(dest => dest.Position,	src => (src.Latitude == 0 && src.Longitude == 0) ? null :
					new GeographicPosition
					{
						Latitude = (double)src.Latitude,
						Longitude = (double)src.Longitude
					});

			config.NewConfig<UpdateAddressRequest, Address>()
				.Map(dest => dest.Position, src => (src.Latitude == 0 && src.Longitude == 0) ? null :
					new GeographicPosition
					{
						Latitude = (double)src.Latitude,
						Longitude = (double)src.Longitude
					});

			config.NewConfig<Tradefact.Data.ApplicationUser, ContactResource>()
				.Map(dest => dest.Email, src =>
					new EmailAddressResource
					{
						Id = new Guid(src.Id),
						Email = src.Email,
						IsDefault = true
					})
				.Map(dest => dest.Phone, src => 
					new PhoneResource
					{
						Id = new Guid(src.Id),
						Number = src.PhoneNumber,
						IsDefault = true
					})
				.Map(dest => dest.Department, src => "Not supplied");

			config.NewConfig<ExternalPurchaseOrder, PurchaseOrder>()
				.Ignore(dest => dest.Status)
				.Map(dest => dest.CompanyId, src => src.OrganisationId)
				.Map(dest => dest.PurchaseOrderItems, src => src.LineItems)
				.Map(dest => dest.PurchaseOrderDate, src => src.OrderDate)
				.Map(dest => dest.CurrencyId, src => src.CurrencyCode);

			config.NewConfig<ExternalPurchaseOrderLineItem, PurchaseOrderItem>()
				.Map(dest => dest.OrderQuantity, src => src.Quantity)
				.Map(dest => dest.OrderPriceUnit, src => src.LineAmount)
				.Map(dest => dest.PurchaseOrderItemText, src => src.Description);
		}
	}
}
