using Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tradefact.Data;
using Tradefact.Utilities.DataImport.Model;

namespace Tradefact.Utilities.DataImport
{
    partial class ReferenceDataService : IReferenceDataService
    {

        public void ImportThomasPinkProductData()
        {
            Console.WriteLine("Importing Zedonk Dataset!");

            Guid companyId = Guid.Parse("5f0a659b-d628-4bc3-a23d-18734ec94575");

            using (var fileStream = File.Open(@"C:\Users\patri\Downloads\Zedonk_Data_Load_Full.xml", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Response));
                var response = (Response)serializer.Deserialize(fileStream);
                int pcount = 0;

                foreach (var item in response.Data.Reports)
                {
                    try
                    {
                        bool exists = _context.Products.Any(q => q.SKU == item.Sku && q.CompanyId == companyId);
                        if (!exists)
                        {
                            Product new_p = new Product();
                            new_p.Id = Guid.NewGuid();
                            new_p.CompanyId = companyId;
                            new_p.SKU = item.Sku;

                            new_p.Description = $"{item.Description} {String.Join(" ", GetDescriptionLabels(item))}";
                            new_p.Name = new_p.Description;

                            new_p.Dimensions = new ImportProductDimensions { Width = 0, Height = 0, Length = 0, Scale = "CM", Weight = 0, weightMeasurement = "CM" };
                            new_p.UnitsPerPackage = 1;

                            new_p.Tags = String.Join(",", GetLabels(item).Distinct());
                            new_p.IsActive = true;
                            new_p.HsCode = item.HtsCode.Split(",")[0];

                            if (new_p.Identifier == null) new_p.Identifier = new ProductIdentifier();
                            new_p.Identifier.EAN = item.Barcode ?? "";
                            new_p.Barcode = item.Barcode ?? "";
                            new_p.Reference = "TP-ZEDONK";

                            new_p.IncomingStockQuantity = 0;
                            new_p.MinStockQuantity = new_p.StockQuantity = new_p.NotifyStockQuantityBelow = new_p.OrderQuantityMaximum = 0;

                            _context.Products.Add(new_p);
                            _context.SaveChanges();
                        }

                        Product p = _context.Products.FirstOrDefault(q => q.SKU == item.Sku && q.CompanyId == companyId);
                        p.Description = $"{item.Description} {String.Join(" ", GetDescriptionLabels(item))}";
                        p.Name = p.Description;

                        p.Dimensions = new ImportProductDimensions { Width = 0, Height = 0, Length = 0, Scale = "CM", Weight = 0, weightMeasurement = "CM" };
                        p.UnitsPerPackage = 1;

                        p.Tags = String.Join(",", GetLabels(item).Distinct());
                        p.IsActive = true;
                        p.HsCode = item.HtsCode.Split(",")[0];

                        if (p.Identifier == null) p.Identifier = new ProductIdentifier();
                        p.Identifier.EAN = item.Barcode ?? "";
                        p.Barcode = item.Barcode ?? "";
                        p.Reference = "TP-ZEDONK";

                        p.IncomingStockQuantity = 0;
                        p.MinStockQuantity = p.StockQuantity = p.NotifyStockQuantityBelow = p.OrderQuantityMaximum =  0;

                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    // Console.WriteLine($"{item.Sku}:  {item.Description} {String.Join(" ", GetLabels(item))}");
                    pcount++;
                }
                Console.WriteLine($"Product Count: {pcount}");
            }
            Console.WriteLine("Zedonk Dataset Imported!");
        }

        private IEnumerable<string> GetDescriptionLabels(Reports item)
        {
            if (!String.IsNullOrEmpty(item.ProductCategory5) && !item.Description.Contains(item.ProductCategory5.Trim())) yield return item.ProductCategory5.Trim();
            if (!String.IsNullOrEmpty(item.Size)) yield return item.Size.Trim();

            yield break;
        }
        private IEnumerable<string> GetLabels(Reports item)
        {
            if (!String.IsNullOrEmpty(item.ProductCategory1) && !item.Description.Contains(item.ProductCategory1.Trim())) yield return item.ProductCategory1.Trim();
            if (!String.IsNullOrEmpty(item.ProductCategory2) && !item.Description.Contains(item.ProductCategory2.Trim())) yield return item.ProductCategory2.Trim();
            if (!String.IsNullOrEmpty(item.ProductCategory3) && !item.Description.Contains(item.ProductCategory3.Trim())) yield return item.ProductCategory3.Trim();
            if (!String.IsNullOrEmpty(item.ProductCategory4) && !item.Description.Contains(item.ProductCategory4.Trim())) yield return item.ProductCategory4.Trim();
            if (!String.IsNullOrEmpty(item.ProductCategory5) && !item.Description.Contains(item.ProductCategory5.Trim())) yield return item.ProductCategory5.Trim();
            if (!String.IsNullOrEmpty(item.Labels) && !item.Description.Contains(item.Labels.Trim())) yield return item.Labels.Trim();

            if (!String.IsNullOrEmpty(item.Size)) yield return item.Size.Trim();
            yield return "Zedonk";

            yield break;
        }

        private bool IsValid(Reports item)
        {
            return !item.Sku.StartsWith("-");
        }

    }
}
