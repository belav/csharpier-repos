using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class GroupObjectModel
    {
        public long id { get; set; }
        public Nullable<long> groupId { get; set; }
        public Nullable<long> objectId { get; set; }
        public string notes { get; set; }
        public byte addOb { get; set; }
        public byte updateOb { get; set; }
        public byte deleteOb { get; set; }
        public byte showOb { get; set; }
        public byte reportOb { get; set; }
        public byte levelOb { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public int isActive { get; set; }


      
        public string objectName { get; set; }
        public string desc { get; set; }
      
        public Boolean canDelete { get; set; }
     //   public Nullable<long> parentObjectId { get; set; }
        public string objectType { get; set; }
        public string parentObjectName { get; set; }

        


    }
}
