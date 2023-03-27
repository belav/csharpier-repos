using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class UnitModel
    {
        public long unitId { get; set; }
        public string name { get; set; }
        public short isSmallest { get; set; }
        public Nullable<long> smallestId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> parentid { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
 
        public string smallestUnit { get; set; }

        public Boolean canDelete { get; set; }
    }
}