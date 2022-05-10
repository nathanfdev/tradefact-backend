using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.JobEngine.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Pagination
    {
        public int currentPage { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public int onPage { get; set; }
        public int nextOffset { get; set; }
        public int totalCount { get; set; }
        public int pageCount { get; set; }
    }

    public class Mapped
    {
        public string sku { get; set; }

        // PO Import
        public string brand { get; set; }
        public string unitprice { get; set; }
        public string desc1 { get; set; }
        public string desc2 { get; set; }
        public string desc3 { get; set; }
        public string desc4 { get; set; }
        public string desc5 { get; set; }
        public string qty { get; set; }
        public string totalcost { get; set; }

        // Product Import
        public string stockqty { get; set; }
        public string hscode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string batchId { get; set; }
        public object raw { get; set; }
        public Mapped mapped { get; set; }
        public bool valid { get; set; }
        public bool deleted { get; set; }
        public int sequence { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
    }

    public class FlatFileBatch
    {
        public Pagination pagination { get; set; }
        public List<Datum> data { get; set; }
    }
}
