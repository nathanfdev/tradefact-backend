using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    [TypescriptAutoGeneration]
    public class LCLModel
    {
        public int Qty { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal CBM { get { return (Width * Height * Length); } }
    }
}
