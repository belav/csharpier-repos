using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class PackageModel
    {
        public long packageId { get; set; }
        public Nullable<long> parentIUId { get; set; }
        public Nullable<long> childIUId { get; set; }
        public int quantity { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }

        public bool canDelete { get; set; }

        // item parent
        public Nullable<long> pitemId { get; set; }
        public string pcode { get; set; }
        public string pitemName { get; set; }

        public string type { get; set; }
        public string image { get; set; }

        //units
        public Nullable<long> punitId { get; set; }
        public string punitName { get; set; }

        //item chiled

        public Nullable<long> citemId { get; set; }
        public string ccode { get; set; }
        public string citemName { get; set; }

        public string ctype { get; set; }
        public string cimage { get; set; }

        //units
        public Nullable<long> cunitId { get; set; }
        public string cunitName { get; set; }
        public Nullable<decimal> avgPurchasePrice { get; set; }
        public Nullable<decimal> iuCost { get; set; }
    }
}
