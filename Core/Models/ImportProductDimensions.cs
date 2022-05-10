using System;
using System.Linq;

namespace Core.Models
{
    public class ImportProductDimensions
    {
        public decimal Height { get; set; }

        public decimal Length { get; set; }

        public string Scale { get; set; }

        public decimal Width { get; set; }
        public decimal Weight { get; set; }

        public string weightMeasurement { get; set; }
    }
}
