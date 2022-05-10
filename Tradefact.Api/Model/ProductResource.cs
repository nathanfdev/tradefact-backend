using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class OldProductResource
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public string GoodsType { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public int UnitsPerPackage { get; set; }

        public string ProductId { get; set; }

        public string SKU { get; set; }

        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }
        public string HazardDocumentId { get; set; }
        public bool Rotatable { get; set; }
        public bool Stackable { get; set; }


        public HazardContentsEnum HazardousContents { get; set; }
        public string Reference { get; set; }
        public bool MagneticFieldContained { get; set; }
        public string LithiumBatteryPacking { get; set; }

        public ImportProductDimensions Dimensions { get; set; }
    }
}
