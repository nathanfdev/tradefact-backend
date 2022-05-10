using Core.Enums;
using Core.Models;
using Newtonsoft.Json;

namespace Tradefact.Application.Models
{
    public interface IAddressResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public bool IsDefault { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public CountryResource Country { get; set; }
        public GeographicPosition Position { get; set; }

        public NetworkConnectionTypeEnum NetworkConnectionType { get; }
    }

    public class AddressResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public bool IsDefault { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public CountryResource Country { get; set; }
        public GeographicPosition Position { get; set; }
        public virtual NetworkConnectionTypeEnum NetworkConnectionType { get; }
    }

    public class OwnedAddressResource : AddressResource, IAddressResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.SELF;
    }
    public class ManagedAddressResource : AddressResource, IAddressResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.MANAGED;
    }
    public class ConnectedAddressResource : AddressResource, IAddressResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.CONNECTED;
    }

    public class SortedAddressResource : AddressResource, IAddressResource
    {
        [JsonIgnore]
        public string Search => $"{this.Name.ToLower()}{this.AddressLine1.ToLower()}{this.City.ToLower()}";
        public int Position(string search) => this.Search.IndexOf(search.ToLower());
    }
}
