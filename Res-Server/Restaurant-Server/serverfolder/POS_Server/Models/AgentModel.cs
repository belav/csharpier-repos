using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class AgentModel
    {

        public long agentId { get; set; }
        public Nullable<long> pointId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string company { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string accType { get; set; }
        public decimal balance { get; set; }
        public byte balanceType { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
        public string fax { get; set; }
        public decimal maxDeserve { get; set; }
        public bool isLimited { get; set; }
        public string payType { get; set; }
        public bool canReserve { get; set; }
        public string disallowReason { get; set; }
        public Nullable<long> residentSecId { get; set; }
        public string GPSAddress { get; set; }
        public Boolean canDelete { get; set; }

        public long agentMembershipsId { get; set; }
        public Nullable<long> subscriptionFeesId { get; set; }
        public Nullable<long> cashTransId { get; set; }
        public Nullable<long> membershipId { get; set; }
  
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }



    }
}