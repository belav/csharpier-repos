using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class PropertiesItemModel
    {
        public long propertyItemId { get; set; }
        public string name { get; set; }
        public Nullable<long> propertyId { get; set; }
        public short isDefault { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }

        public string propertyItemName { get; set; }

        public string propertyName { get; set; }

        public Boolean canDelete { get; set; }
    }
}