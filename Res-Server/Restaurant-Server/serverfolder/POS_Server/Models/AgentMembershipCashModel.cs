using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class AgentMembershipCashModel
    {


        public long agentMembershipCashId { get; set; }
        public Nullable<long> subscriptionFeesId { get; set; }
        public Nullable<long> cashTransId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public string notes { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public byte isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string subscriptionType { get; set; }


        public decimal Amount { get; set; }
      

        public bool canDelete { get; set; }
        public Nullable<int> monthsCount { get; set; }
        public string agentName { get; set; }
        public string agentcode { get; set; }
        public string agentcompany { get; set; }
        public string agenttype { get; set; }
        public string membershipName { get; set; }

        
        public string membershipcode { get; set; }
        public string transType { get; set; }
        public string transNum { get; set; }
        public Nullable<System.DateTime> payDate { get; set; }
        public byte membershipisActive { get; set; }
        public decimal discountValue { get; set; }
        public decimal total { get; set; }
        public string processType { get; set; }
        public Nullable<long> cardId { get; set; }
        public string cardName { get; set; }
        public string docNum { get; set; }

    }
    public class AgenttoPayCashModel
    {


   
        public Nullable<long> agentMembershipCashId { get; set; }
        public Nullable<long> subscriptionFeesId { get; set; }
        public Nullable<long> cashTransId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        //public Nullable<long> updateUserId { get; set; }
        //public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        //public Nullable<long> createUserId { get; set; }
        public decimal Amount { get; set; }
        public Nullable<long> pointId { get; set; }
        public string agentName { get; set; }
        public string agentcode { get; set; }
        public string agentcompany { get; set; }
        public string agentaddress { get; set; }
        public string agentemail { get; set; }
        public string agentphone { get; set; }
        public string agentmobile { get; set; }
     
        public string agenttype { get; set; }
        public string agentaccType { get; set; }
        public decimal agentbalance { get; set; }
        public byte agentbalanceType { get; set; }
       
        public string agentfax { get; set; }
        public decimal agentmaxDeserve { get; set; }
        public bool agentisLimited { get; set; }
        public string agentpayType { get; set; }
        public bool agentcanReserve { get; set; }
        public string agentdisallowReason { get; set; }
        public Nullable<long> agentresidentSecId { get; set; }
        public string agentGPSAddress { get; set; }

        public string membershipName { get; set; }

        public byte membershipisActive { get; set; }
        public string subscriptionType { get; set; }
        public string cashsubscriptionType { get; set; }
        public string membershipcode { get; set; }
        public bool isFreeDelivery { get; set; }
        public decimal deliveryDiscountPercent { get; set; }
        public Nullable<decimal> subscriptionFee { get; set; }
        public Nullable<int> monthsCount { get; set; }
        public string transType { get; set; }
        public string transNum { get; set; }
        public Nullable<System.DateTime> payDate { get; set; }
        public string membershipStatus { get; set; }
        public decimal discountValue { get; set; }
        public decimal total { get; set; }
        public string processType { get; set; }


        public int couponsCount { get; set; }
        public int invoicesClassesCount { get; set; }
        public int offersCount { get; set; }
        //public Nullable<int> cachpayrowCount { get; set; }
        public AgentMembershipCashModel agentMembershipcashobj { get; set; }
        public  List<AgentMembershipCashModel> agentMembershipcashobjList { get; set; }
     

    }
    /*
     * public long agentId { get; set; }
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
     * */
}