using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class setValuesModel
    {
        public long valId { get; set; }
        public string value { get; set; }
        public int isDefault { get; set; }
        public int isSystem { get; set; }
        public string notes { get; set; }
        public Nullable<long> settingId { get; set; }


  
        //setting
        public string name { get; set; }
    }
}