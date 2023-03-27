using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class CategoryModel
    {
        public long categoryId { get; set; }
        public string categoryCode { get; set; }
        public string name { get; set; }
        public string details { get; set; }
        public string image { get; set; }
        public byte isActive { get; set; }
        public decimal taxes { get; set; }
        public Nullable<long> parentId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public string notes { get; set; }


        public Boolean canDelete { get; set; }
        public Nullable<int> sequence { get; set; }
        public Nullable<long> id { get; set; }
        public string type { get; set; }
      
    }
}