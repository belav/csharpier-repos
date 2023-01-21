using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class MedalAgentModel
    {
        public long id { get; set; }
        public Nullable<long> medalId { get; set; }
        public Nullable<long> agentId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }

      
        public Nullable<long> offerId { get; set; }
        public Nullable<long> couponId { get; set; }
      
        public string medalName { get; set; }
        public string agentName { get; set; }
        public string offerName { get; set; }
        public string couponName { get; set; }
        public string createUserName { get; set; }
    }
}