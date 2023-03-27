using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class itemsPropModel
    {
        public long itemPropId { get; set; }
        public Nullable<long> propertyItemId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }

        public Nullable<long> updateUserId { get; set; }
 
        public Nullable<int> isActive { get; set; }
        public string propValue { get; set; }
        public string propName { get; set; }
   
    }
}