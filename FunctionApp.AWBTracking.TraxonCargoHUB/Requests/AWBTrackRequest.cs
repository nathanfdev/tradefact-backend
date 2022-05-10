using System;
using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FunctionApp.AWBTracking.TraxonCargoHUB.Model
{
    public class FlightStatusMessageValidator : AbstractValidator<FlightStatusMessage>
    {
        public FlightStatusMessageValidator()
        {
            RuleFor(x => x.AirWaybillNumber).NotEmpty();
        }
    }

    public abstract class AirWayBillMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("messageHeader")]
        public MessageHeader MessageHeader { get; set; }

        [JsonProperty("airWaybillNumber")]
        public string AirWaybillNumber { get; set; }
    }

    public partial class FlightStatusMessage: AirWayBillMessage
    {
        [JsonProperty("originAndDestination")]
        public OriginAndDestination OriginAndDestination { get; set; }

        [JsonProperty("quantity")]
        public Quantity Quantity { get; set; }

        [JsonProperty("totalNumberOfPieces")]
        public int TotalNumberOfPieces { get; set; }

        [JsonProperty("events")]
        public List<FlightEvent> Events { get; set; }

        [JsonProperty("otherCustomsSecurityAndRegulatoryInformation")]
        public OtherCustomsSecurityAndRegulatoryInformation OtherCustomsSecurityAndRegulatoryInformation { get; set; }

        [JsonProperty("otherServiceInformation")]
        public string OtherServiceInformation { get; set; }
    }

    public partial class Quantity
    {
        [JsonProperty("shipmentDescriptionCode")]
        public string ShipmentDescriptionCode { get; set; }

        [JsonProperty("numberOfPieces")]
        public long NumberOfPieces { get; set; }

        [JsonProperty("weight")]
        public Weight Weight { get; set; }
    }

    public partial class Weight
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal Amount { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public partial class OriginAndDestination
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public partial class FlightEvent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("numberOfPieces")]
        public int NumberOfPieces { get; set; }

        [JsonProperty("weight")]
        public Volume Weight { get; set; }

        [JsonProperty("uld")]
        public List<Uld> Uld { get; set; }

        [JsonProperty("timeOfEvent")]
        public DateTimeOffset TimeOfEvent { get; set; }

        [JsonProperty("timeOfEventTimePartQuality")]
        public string TimeOfEventTimePartQuality { get; set; }

        [JsonProperty("otherServiceInformation")]
        public string OtherServiceInformation { get; set; }

        [JsonProperty("flight")]
        public string Flight { get; set; }

        [JsonProperty("dateOfScheduledDeparture")]
        public DateTimeOffset DateOfScheduledDeparture { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("timeOfScheduledDeparture")]
        public DateTimeOffset TimeOfScheduledDeparture { get; set; }

        [JsonProperty("estimatedTimeOfDeparture")]
        public DateTimeOffset EstimatedTimeOfDeparture { get; set; }

        [JsonProperty("timeOfScheduledArrival")]
        public DateTimeOffset TimeOfScheduledArrival { get; set; }

        [JsonProperty("estimatedTimeOfArrival")]
        public DateTimeOffset EstimatedTimeOfArrival { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }

        [JsonProperty("densityGroup")]
        public string DensityGroup { get; set; }

        [JsonProperty("airportOfDelivery")]
        public string AirportOfDelivery { get; set; }

        [JsonProperty("deliveryToName")]
        public string DeliveryToName { get; set; }

        [JsonProperty("airportOfNotification")]
        public string AirportOfNotification { get; set; }

        [JsonProperty("notificationToName")]
        public string NotificationToName { get; set; }

        [JsonProperty("airportOfReceipt")]
        public string AirportOfReceipt { get; set; }

        [JsonProperty("receivedFromName")]
        public string ReceivedFromName { get; set; }

        [JsonIgnore]
        public string QuantityWeightDescription
            => $"{this.NumberOfPieces} pieces at {this.Weight.Amount.ToString("F", CultureInfo.InvariantCulture)} {this.Weight.Unit.ToLower()}s";

        [JsonIgnore]
        public string EventDescription
            => this.Type.ToLower() switch
            {
                "booked" => $"booked on flight: {this.Flight}",
                "departed" => $"departed on flight: {this.Flight}",
                "arrived" => $"arrival of flight: {this.Flight}",
                "delivered" => $"delivered to {this.DeliveryToName} from flight: {this.Flight}",
                "consignee notified" => $"consignee {this.DeliveryToName} notified flight: {this.Flight}",
                "received from flight" => $"received from flight: {this.Flight}",
                "received from shipper" => $"received from shipper, flight: {this.Flight}",
                _ => null
            };

        [JsonIgnore]
        public string EventLocation
            => this.Type.ToLower() switch
            {
                "booked" => this.Origin,
                "departed" => this.Origin,
                "arrived" => this.Destination,
                "delivered" => this.AirportOfDelivery,
                "consignee notified" => this.AirportOfNotification,
                "received from flight" => this.Destination,
                "received from shipper" => this.AirportOfReceipt,
                _ => null
            };

        [JsonIgnore]
        public string EventType
            => this.Type.ToLower() switch
            {
                "booked" => "BKD",
                "departed" => "DEP",
                "arrived" => "ARR",
                "delivered" => "DLV",
                "consignee notified" => "NFD",
                "received from flight" => "RCF",
                "received from shipper" => "RCS",
                _ => null
            };

    }

    public class TradefactAWBEvent
    {
        public TradefactAWBEvent(DateTimeOffset TimeOfEvent)
        {

        }

        [JsonProperty("numberOfPieces")]
        public int NumberOfPieces { get; set; }

        [JsonProperty("weight")]
        public Volume Weight { get; set; }

        public string Location { get; set; }
        public string Flight { get; set; }

    }

    public partial class OtherCustomsSecurityAndRegulatoryInformation
    {
        [JsonProperty("oci")]
        public List<Oci> Oci { get; set; }

        [JsonProperty("uld")]
        public List<Uld> Uld { get; set; }
    }

    public partial class Oci
    {
        [JsonProperty("isoCountryCode")]
        public string IsoCountryCode { get; set; }

        [JsonProperty("informationIdentifier")]
        public string InformationIdentifier { get; set; }

        [JsonProperty("controlInformation")]
        public string ControlInformation { get; set; }

        [JsonProperty("additionalControlInformation")]
        public string AdditionalControlInformation { get; set; }

        [JsonProperty("supplementaryControlInformation")]
        public string SupplementaryControlInformation { get; set; }
    }

    public partial class Accounting
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("accountingInformation")]
        public string AccountingInformation { get; set; }
    }

    public partial class Agent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("iataCargoAgentNumericCode")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long IataCargoAgentNumericCode { get; set; }

        [JsonProperty("iataCargoAgentCASSAddress")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long IataCargoAgentCassAddress { get; set; }

        [JsonProperty("participantIdentifier")]
        public string ParticipantIdentifier { get; set; }
    }

    public partial class AlsoNotify
    {
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("contactDetails")]
        public List<ContactDetail> ContactDetails { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("name1")]
        public string Name1 { get; set; }

        [JsonProperty("name2")]
        public string Name2 { get; set; }

        [JsonProperty("streetAddress1")]
        public string StreetAddress1 { get; set; }

        [JsonProperty("streetAddress2")]
        public string StreetAddress2 { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("stateProvince")]
        public string StateProvince { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postCode")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long PostCode { get; set; }
    }

    public partial class ContactDetail
    {
        [JsonProperty("contactIdentifier")]
        public string ContactIdentifier { get; set; }

        [JsonProperty("contactNumber")]
        public string ContactNumber { get; set; }
    }

    public partial class CarriersExecution
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("placeOrAirportCityCode")]
        public string PlaceOrAirportCityCode { get; set; }

        [JsonProperty("authorisationSignature")]
        public string AuthorisationSignature { get; set; }
    }

    public partial class ChargeDeclarations
    {
        [JsonProperty("isoCurrencyCode")]
        public string IsoCurrencyCode { get; set; }

        [JsonProperty("chargeCode")]
        public string ChargeCode { get; set; }

        [JsonProperty("payment_WeightValuation")]
        public string PaymentWeightValuation { get; set; }

        [JsonProperty("payment_OtherCharges")]
        public string PaymentOtherCharges { get; set; }

        [JsonProperty("declaredValueForCarriage")]
        public string DeclaredValueForCarriage { get; set; }

        [JsonProperty("declaredValueForCustoms")]
        public string DeclaredValueForCustoms { get; set; }

        [JsonProperty("declaredValueForInsurance")]
        public string DeclaredValueForInsurance { get; set; }
    }

    public partial class ChargeItem
    {
        [JsonProperty("numberOfPieces")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long NumberOfPieces { get; set; }

        [JsonProperty("rateCombinationPointCityCode")]
        public string RateCombinationPointCityCode { get; set; }

        [JsonProperty("commodityItemNumber")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> CommodityItemNumber { get; set; }

        [JsonProperty("grossWeight")]
        public Volume GrossWeight { get; set; }

        [JsonProperty("goodsDescription")]
        public string GoodsDescription { get; set; }

        [JsonProperty("consolidation")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool Consolidation { get; set; }

        [JsonProperty("harmonisedCommodityCode")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> HarmonisedCommodityCode { get; set; }

        [JsonProperty("isoCountryCodeOfOriginOfGoods")]
        public string IsoCountryCodeOfOriginOfGoods { get; set; }

        [JsonProperty("packaging")]
        public List<Packaging> Packaging { get; set; }

        [JsonProperty("charges")]
        public List<Charge> Charges { get; set; }

        [JsonProperty("serviceCode")]
        public string ServiceCode { get; set; }
    }

    public partial class Charge
    {
        [JsonProperty("chargeableWeight")]
        public Volume ChargeableWeight { get; set; }

        [JsonProperty("rateClassCode")]
        public string RateClassCode { get; set; }

        [JsonProperty("rateClassCodeBasis")]
        public string RateClassCodeBasis { get; set; }

        [JsonProperty("classRatePercentage")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long ClassRatePercentage { get; set; }

        [JsonProperty("uldRateClassType")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long UldRateClassType { get; set; }

        [JsonProperty("rateOrCharge")]
        public string RateOrCharge { get; set; }

        [JsonProperty("totalChargeAmount")]
        public string TotalChargeAmount { get; set; }
    }

    public partial class Volume
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal Amount { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public partial class Packaging
    {
        [JsonProperty("numberOfPieces")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long NumberOfPieces { get; set; }

        [JsonProperty("weight")]
        public Volume Weight { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }

        [JsonProperty("dimensions")]
        public Dimensions Dimensions { get; set; }

        [JsonProperty("uld")]
        public Uld Uld { get; set; }

        [JsonProperty("shippersLoadAndCount")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long ShippersLoadAndCount { get; set; }
    }

    public partial class Dimensions
    {
        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("length")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long Length { get; set; }

        [JsonProperty("width")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long Width { get; set; }

        [JsonProperty("height")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long Height { get; set; }
    }

    public partial class Uld
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("serialNumber")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long SerialNumber { get; set; }

        [JsonProperty("ownerCode")]
        public string OwnerCode { get; set; }

        [JsonProperty("loadingIndicator")]
        public string LoadingIndicator { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("weightOfULDContents")]
        public Volume WeightOfUldContents { get; set; }
    }

    public partial class ChargesCollectInDestCurrency
    {
        [JsonProperty("isoCurrencyCode")]
        public string IsoCurrencyCode { get; set; }

        [JsonProperty("currencyConversionRateOfExchange")]
        public string CurrencyConversionRateOfExchange { get; set; }

        [JsonProperty("chargesInDestinationCurrency")]
        public string ChargesInDestinationCurrency { get; set; }

        [JsonProperty("chargesAtDestination")]
        public string ChargesAtDestination { get; set; }

        [JsonProperty("totalCollectCharges")]
        public string TotalCollectCharges { get; set; }
    }

    public partial class ChargeSummary
    {
        [JsonProperty("totalWeightCharge")]
        public string TotalWeightCharge { get; set; }

        [JsonProperty("valuationCharge")]
        public string ValuationCharge { get; set; }

        [JsonProperty("taxes")]
        public string Taxes { get; set; }

        [JsonProperty("totalOtherChargesDueAgent")]
        public string TotalOtherChargesDueAgent { get; set; }

        [JsonProperty("totalOtherChargesDueCarrier")]
        public string TotalOtherChargesDueCarrier { get; set; }

        [JsonProperty("chargeSummaryTotal")]
        public string ChargeSummaryTotal { get; set; }
    }

    public partial class CommissionInfo
    {
        [JsonProperty("amountCASSSettlementFactor")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long AmountCassSettlementFactor { get; set; }

        [JsonProperty("percentageCASSSettlementFactor")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long PercentageCassSettlementFactor { get; set; }
    }

    public partial class Flight
    {
        [JsonProperty("flight")]
        public string FlightFlight { get; set; }

        [JsonProperty("scheduledDate")]
        public DateTimeOffset ScheduledDate { get; set; }

        [JsonProperty("scheduledTime")]
        public DateTimeOffset ScheduledTime { get; set; }
    }

    public partial class MessageHeader
    {
        [JsonProperty("addressing")]
        public Addressing Addressing { get; set; }

        [JsonProperty("creationDate")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("edifactData")]
        public EdifactData EdifactData { get; set; }
    }

    public partial class Addressing
    {
        [JsonProperty("routeVia")]
        public RouteAnswerVia RouteVia { get; set; }

        [JsonProperty("routeAnswerVia")]
        public RouteAnswerVia RouteAnswerVia { get; set; }

        [JsonProperty("senderAddresses")]
        public List<RouteAnswerVia> SenderAddresses { get; set; }

        [JsonProperty("finalRecipientAddresses")]
        public List<RouteAnswerVia> FinalRecipientAddresses { get; set; }

        [JsonProperty("replyAnswerTo")]
        public List<RouteAnswerVia> ReplyAnswerTo { get; set; }
    }

    public partial class RouteAnswerVia
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public partial class EdifactData
    {
        [JsonProperty("commonAccessReference")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long CommonAccessReference { get; set; }

        [JsonProperty("messageReference")]
        public string MessageReference { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("interchangeControlReference")]
        public string InterchangeControlReference { get; set; }
    }

    public partial class NominatedHandlingParty
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }
    }

    public partial class OtherCharge
    {
        [JsonProperty("paymentCondition")]
        public string PaymentCondition { get; set; }

        [JsonProperty("otherChargeCode")]
        public string OtherChargeCode { get; set; }

        [JsonProperty("entitlementCode")]
        public string EntitlementCode { get; set; }

        [JsonProperty("chargeAmount")]
        public string ChargeAmount { get; set; }
    }

    public partial class OtherParticipant
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("officeMessageAddress")]
        public OfficeMessageAddress OfficeMessageAddress { get; set; }

        [JsonProperty("fileReference")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long FileReference { get; set; }

        [JsonProperty("participantIdentification")]
        public ParticipantIdentifi ParticipantIdentification { get; set; }
    }

    public partial class OfficeMessageAddress
    {
        [JsonProperty("airportCityCode")]
        public string AirportCityCode { get; set; }

        [JsonProperty("officeFunctionDesignator")]
        public string OfficeFunctionDesignator { get; set; }

        [JsonProperty("companyDesignator")]
        public string CompanyDesignator { get; set; }
    }

    public partial class ParticipantIdentifi
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("code")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long Code { get; set; }

        [JsonProperty("airportCityCode")]
        public string AirportCityCode { get; set; }
    }

    public partial class Route
    {
        [JsonProperty("carrierCode")]
        public string CarrierCode { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public partial class SalesIncentive
    {
        [JsonProperty("chargeAmount")]
        public string ChargeAmount { get; set; }

        [JsonProperty("cassIndicator")]
        public string CassIndicator { get; set; }
    }

    public partial class SenderReference
    {
        [JsonProperty("officeMessageAddress")]
        public OfficeMessageAddress OfficeMessageAddress { get; set; }

        [JsonProperty("fileReference")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long FileReference { get; set; }

        [JsonProperty("participantIdentifier")]
        public ParticipantIdentifi ParticipantIdentifier { get; set; }
    }

    public partial class ShipmentReferenceInformation
    {
        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringToDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(decimal) || t == typeof(decimal?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            decimal d;
            if (Decimal.TryParse(value, out d))
            {
                return d;
            }
            else {
                long l;
                if (Int64.TryParse(value, out l))
                {
                    return Convert.ToDecimal(l);
                }

            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (decimal)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringToDecimalConverter Singleton = new ParseStringToDecimalConverter();
    }

    internal class PurpleParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly PurpleParseStringConverter Singleton = new PurpleParseStringConverter();
    }

    internal class DecodeArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(List<long>);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            var value = new List<long>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var converter = PurpleParseStringConverter.Singleton;
                var arrayItem = (long)converter.ReadJson(reader, typeof(long), null, serializer);
                value.Add(arrayItem);
                reader.Read();
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (List<long>)untypedValue;
            writer.WriteStartArray();
            foreach (var arrayItem in value)
            {
                var converter = PurpleParseStringConverter.Singleton;
                converter.WriteJson(writer, arrayItem, serializer);
            }
            writer.WriteEndArray();
            return;
        }

        public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
    }

    internal class FluffyParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (bool)untypedValue;
            var boolString = value ? "true" : "false";
            serializer.Serialize(writer, boolString);
            return;
        }

        public static readonly FluffyParseStringConverter Singleton = new FluffyParseStringConverter();
    }


}
