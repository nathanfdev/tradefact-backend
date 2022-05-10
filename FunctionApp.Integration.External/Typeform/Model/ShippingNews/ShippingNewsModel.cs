using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApp.Integration.External.Typeform.Model
{
    public class ShippingNewsModel
    {
        public int WeekNo { get; set; }
        public List<ShippingNewsSection> Sections { get; set; } = new List<ShippingNewsSection>();
    }

    public class ShippingNewsSection
    {
        public string Reference { get; set; }
        public string Title { get; set; }

        public List<string> Items { get; set; }

        public ShippingNewsSection(string reference, string title, string data)
        {
            this.Reference = reference;
            this.Title = title;

            string[] stringSeparators = new string[] { "\r\n", "\n\n" };
            this.Items = data.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
        }


    }
}
