using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Tradefact.Utilities.DataImport.Model
{
    [XmlRoot(ElementName = "Reports")]
    public class Reports
    {
        [XmlElement(ElementName = "season")]
        public string Season { get; set; }
        [XmlElement(ElementName = "ac")]
        public bool Ac { get; set; }
        [XmlElement(ElementName = "is")]
        public bool Is { get; set; }
        [XmlElement(ElementName = "withdrawn")]
        public bool Withdrawn { get; set; }
        [XmlElement(ElementName = "size_category")]
        public string SizeCategory { get; set; }
        [XmlElement(ElementName = "product_id")]
        public string ProductId { get; set; }
        [XmlElement(ElementName = "product_category_1")]
        public string ProductCategory1 { get; set; }
        [XmlElement(ElementName = "product_category_2")]
        public string ProductCategory2 { get; set; }
        [XmlElement(ElementName = "product_category_3")]
        public string ProductCategory3 { get; set; }
        [XmlElement(ElementName = "product_category_4")]
        public string ProductCategory4 { get; set; }
        [XmlElement(ElementName = "product_category_5")]
        public string ProductCategory5 { get; set; }
        [XmlElement(ElementName = "style")]
        public string Style { get; set; }
        [XmlElement(ElementName = "fabric")]
        public string Fabric { get; set; }
        [XmlElement(ElementName = "colour")]
        public string Colour { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "size")]
        public string Size { get; set; }
        [XmlElement(ElementName = "barcode")]
        public string Barcode { get; set; }
        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "manufacturer_category")]
        public string ManufacturerCategory { get; set; }
        [XmlElement(ElementName = "customs_type")]
        public string CustomsType { get; set; }
        [XmlElement(ElementName = "textile_content")]
        public string TextileContent { get; set; }
        [XmlElement(ElementName = "mid")]
        public string Mid { get; set; }
        [XmlElement(ElementName = "country_of_origin")]
        public string CountryOfOrigin { get; set; }
        [XmlElement(ElementName = "hts_code")]
        public string HtsCode { get; set; }
        [XmlElement(ElementName = "intrastat_code")]
        public object IntrastatCode { get; set; }
        [XmlElement(ElementName = "manufacturing_type")]
        public string ManufacturingType { get; set; }
        [XmlElement(ElementName = "production_notes")]
        public object ProductionNotes { get; set; }
        [XmlElement(ElementName = "labels")]
        public string Labels { get; set; }
        [XmlElement(ElementName = "ccy_symbol")]
        public string CcySymbol { get; set; }
        [XmlElement(ElementName = "ccy")]
        public string Ccy { get; set; }
        [XmlElement(ElementName = "unit_first_x002F_make_cost")]
        public int UnitFirstX002FMakeCost { get; set; }
        [XmlElement(ElementName = "unit_first_x002F_make_cost1")]
        public int UnitFirstX002FMakeCost1 { get; set; }
        [XmlElement(ElementName = "unit_rm_cost")]
        public int UnitRmCost { get; set; }
        [XmlElement(ElementName = "freight_cost")]
        public int FreightCost { get; set; }
        [XmlElement(ElementName = "import_duty")]
        public int ImportDuty { get; set; }
        [XmlElement(ElementName = "import_duty_cost")]
        public int ImportDutyCost { get; set; }
        [XmlElement(ElementName = "misc_cost")]
        public int MiscCost { get; set; }
        [XmlElement(ElementName = "unit_cost")]
        public int UnitCost { get; set; }
        [XmlElement(ElementName = "suggested_wsp")]
        public int SuggestedWsp { get; set; }
        [XmlElement(ElementName = "wsp")]
        public int Wsp { get; set; }
        [XmlElement(ElementName = "gross_profit_margin")]
        public int GrossProfitMargin { get; set; }
        [XmlElement(ElementName = "unit_gross_profit")]
        public int UnitGrossProfit { get; set; }
        [XmlElement(ElementName = "sales_order_quantity")]
        public int SalesOrderQuantity { get; set; }
        [XmlElement(ElementName = "total_first_x002F_make_cost")]
        public int TotalFirstX002FMakeCost { get; set; }
        [XmlElement(ElementName = "total_rm_cost")]
        public int TotalRmCost { get; set; }
        [XmlElement(ElementName = "total_freight")]
        public int TotalFreight { get; set; }
        [XmlElement(ElementName = "total_import_duty")]
        public int TotalImportDuty { get; set; }
        [XmlElement(ElementName = "total_misc")]
        public int TotalMisc { get; set; }
        [XmlElement(ElementName = "total_cost")]
        public int TotalCost { get; set; }
        [XmlElement(ElementName = "avg_price")]
        public int AvgPrice { get; set; }
        [XmlElement(ElementName = "wtg_avg_price")]
        public int WtgAvgPrice { get; set; }
        [XmlElement(ElementName = "wsp1")]
        public int Wsp1 { get; set; }
        [XmlElement(ElementName = "wsp2")]
        public int Wsp2 { get; set; }
        [XmlElement(ElementName = "wsp3")]
        public int Wsp3 { get; set; }
        [XmlElement(ElementName = "suggested_rrp")]
        public int SuggestedRrp { get; set; }
        [XmlElement(ElementName = "rrp")]
        public int Rrp { get; set; }
        [XmlElement(ElementName = "rrp1")]
        public int Rrp1 { get; set; }
        [XmlElement(ElementName = "rrp2")]
        public int Rrp2 { get; set; }
        [XmlElement(ElementName = "date_created")]
        public DateTime DateCreated { get; set; }
        [XmlElement(ElementName = "date_last_modified")]
        public DateTime DateLastModified { get; set; }
        [XmlElement(ElementName = "sku")]
        public string Sku { get; set; }
        [XmlElement(ElementName = "sales_order_notes")]
        public object SalesOrderNotes { get; set; }
    }

    [XmlRoot(ElementName = "Data")]
    public class Data
    {
        [XmlElement(ElementName = "Reports")]
        public List<Reports> Reports { get; set; }
    }

    [XmlRoot(ElementName = "Response")]
    public class Response
    {
        [XmlElement(ElementName = "dataAPIName")]
        public string DataAPIName { get; set; }
        [XmlElement(ElementName = "filter")]
        public string Filter { get; set; }
        [XmlElement(ElementName = "code")]
        public int Code { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Data")]
        public Data Data { get; set; }
    }

}
