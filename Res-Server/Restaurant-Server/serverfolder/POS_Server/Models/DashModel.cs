using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class DashModel
    {



    }
    public class TotalPurSale
    {
        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public decimal totalPur { get; set; }
        public decimal totalSale { get; set; }
        public int countPur { get; set; }
        public int countSale { get; set; }
        public int day { get; set; }

    }
    public class UserOnlineCount
    {
        //  public Nullable<long> branchId { get; set; }
        public long branchId { get; set; }
        public string branchName { get; set; }
        public int userOnlineCount { get; set; }

        public int allPos { get; set; }
        public int offlineUsers { get; set; }
        //   public List<userOnlineInfo> userOnlinelist = new List<userOnlineInfo>();
    }

    public class userOnlineInfo
    {
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public byte branchisActive { get; set; }
        public Nullable<long> posId { get; set; }
        public string posName { get; set; }
        public byte posisActive { get; set; }
        public Nullable<long> userId { get; set; }
        public string usernameAccount { get; set; }
        public string userName { get; set; }
        public string lastname { get; set; }
        public string job { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public short userisActive { get; set; }
        public byte isOnline { get; set; }
        public string image { get; set; }

    }
    public class BranchOnlineCount
    {

        public int branchOnline { get; set; }
        public int branchAll { get; set; }
        public int branchOffline { get; set; }


    }

    public class IUStorage
    {


        public string itemName { get; set; }
        public string unitName { get; set; }
        public long itemUnitId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branchId { get; set; }
        public long quantity { get; set; }



    }

    public class BranchInvoicedata
    {

        public long invoiceId { get; set; }
        public string invType { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }

    }
    public class BranchInvoiceCount
    {


        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public int count { get; set; }
        public int dateindex { get; set; }
        public string duration { get; set; }


    }
    public class BestOfCount
    {
        public List<BranchInvoiceCount> CountinMonthsList { get; set; }
        public List<BranchInvoiceCount> CountinDaysList { get; set; }
        public List<BranchInvoiceCount> CountinHoursList { get; set; }


        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }



    }

    public class CountByInvType
    {
        public string invType { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public int dhallCount { get; set; }
        public int selfCount { get; set; }
        public int tawayCount { get; set; }

    }
    public class BranchBalance
    {
        public string branchName { get; set; }
        public decimal balance { get; set; }
        public long branchId { get; set; }
        public string branchType { get; set; }
        public string branchCode { get; set; }
        public byte banchIsActive { get; set; }

    }
}