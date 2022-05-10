using Core.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Helpers
{
    public static class FreightMovementHelpers
    {
        private static Product HandleVariant(Product p, ProductVariant v, string product_description, string product_sku, bool productDescriptionOverride)
        {
            Product prd = p.Adapt<Product>();
            if (v != null)
            {
                // Handle Product Variant
                prd.Reference = v.Reference;
                prd.HazardDocumentId = v.HazardDocumentId;
                prd.HazardNotes = v.HazardNotes;
                prd.HazardDescription = v.HazardDescription;
                prd.HazardClass = v.HazardClass;
                prd.HazardousContents = v.HazardousContents;
                prd.SKU = v.SKU;
                prd.UnitsPerPackage = v.UnitsPerPackage;
                prd.Packing = v.Packing;
                prd.Nickname = v.Nickname;
                prd.Name = v.Name;
                prd.HsCode = v.HsCode;
                prd.GoodsType = v.GoodsType;
                prd.Dimensions = v.Dimensions;
                prd.Description = v.Description;
                prd.Id = v.ProductId;
                prd.MagneticFieldContained = v.MagneticFieldContained;
                prd.Dimensions = v.Dimensions;
            }

            if (!string.IsNullOrEmpty(product_description) && productDescriptionOverride) {
                prd.Description = product_description;
                prd.Name = product_description;
            }
            if (!string.IsNullOrEmpty(product_sku))
            {
                prd.SKU = product_sku;
            }


            return prd;
        }

        public static (List<FCLItem> fcl, List<LCLItem> lcl) ExtractFCLandLCL(FreightMovement fm)
        {
            List<FCLItem> fclItems = new List<FCLItem>();
            List<LCLItem> lclItems = new List<LCLItem>();

            foreach (var item in fm.Items)
            {
                if (item.ContainerTypeCode != null)
                {
                    FCLItem fcl = new FCLItem
                    {
                        HazardCode = item.HazardCode,
                        ContainerType = item.ContainerTypeCode,
                        CargoItems = new List<CargoItem>()
                    };
                    foreach (var ci in item.CargoItems)
                    {
                        fcl.CargoItems.Add(new CargoItem
                        {
                            ProductId = ci.ProductId,
                            HsCode = ci.HsCode,
                            SKU = ci.SKU,
                            Qty = ci.Qty,
                            CartonQty = ci.CartonQty,
                            Width = ci.Width,
                            Length = ci.Length,
                            Height = ci.Height,
                            UOL = ci.UOL,
                            UOW = ci.UOW,
                            Weight = ci.Weight,
                            Product = HandleVariant(ci.Product, ci.ProductVariant, ci.ItemDescription, ci.SKU, ci.ProductDescriptionOverride)
                        });
                    }
                    fclItems.Add(fcl);
                }
                else
                {
                    LCLItem lcl = new LCLItem
                    {
                        HazardCode = item.HazardCode,
                        CartonQty = item.CartonQty
                    };
                    foreach (var ci in item.CargoItems)
                    {
                        lcl.ProductId = ci.ProductId;
                        lcl.HsCode = ci.HsCode;
                        lcl.SKU = ci.SKU;
                        lcl.Qty = ci.Qty;
                        lcl.Width = ci.Width;
                        lcl.Length = ci.Length;
                        lcl.Height = ci.Height;
                        lcl.UOL = ci.UOL;
                        lcl.UOW = ci.UOW;
                        lcl.Weight = ci.Weight;
                        lcl.Product = HandleVariant(ci.Product, ci.ProductVariant, ci.ItemDescription, ci.SKU, ci.ProductDescriptionOverride);
                    }
                    lclItems.Add(lcl);
                }
            }


            return (fclItems, lclItems);
        }

    }
}
