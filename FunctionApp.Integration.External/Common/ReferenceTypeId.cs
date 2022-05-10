using System.ComponentModel;

namespace FunctionApp.Integration.External.Common
{
    public enum ReferenceTypeId
    {
        [Description("category")]
        Category,

        [Description("channel")]
        Channel,

        [Description("product")]
        Product,

        [Description("product-price")]
        ProductPrice,

        [Description("product-type")]
        ProductType,

        [Description("state")]
        State,

        [Description("tax-category")]
        TaxCategory,

        [Description("type")]
        Type,

        [Description("inventory-entry")]
        InventoryEntry
    }
}
