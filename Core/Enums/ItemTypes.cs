using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemTypesEnum
    {
        Partner,
        Company,
        Import,
        Product,
        ImportProducts,
        Quotation,
        Supplier,
        Address
    }
}