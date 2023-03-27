using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class LocationModel
    {
        public long locationId { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
        public Nullable<long> sectionId { get; set; }
        public string notes { get; set; }
        public Nullable<long> branchId { get; set; }
        public byte isFreeZone { get; set; }
        public byte isKitchen { get; set; }


        public Boolean canDelete { get; set; }
     
        public string sectionName { get; set; }
        public string branchName { get; set; }
       
    }
}