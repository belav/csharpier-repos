using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ProgramDetailsModel
    {
        public long id { get; set; }
        public string programName { get; set; }
        public int branchCount { get; set; }
        public int posCount { get; set; }
        public int userCount { get; set; }
        public int vendorCount { get; set; }
        public int customerCount { get; set; }
        public int itemCount { get; set; }
        public int saleinvCount { get; set; }
        public Nullable<int> programIncId { get; set; }
        public Nullable<int> versionIncId { get; set; }
        public string versionName { get; set; }
        public int storeCount { get; set; }
        public string packageSaleCode { get; set; }
        public string customerServerCode { get; set; }
        public Nullable<System.DateTime> expireDate { get; set; }
        public Nullable<bool> isOnlineServer { get; set; }
        public string packageNumber { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<bool> isLimitDate { get; set; }
        public Nullable<bool> isLimitCount { get; set; }
        public bool isActive { get; set; }
        public string packageName { get; set; }

        // current info
 
        public int branchCountNow { get; set; }
        public int posCountNow { get; set; }
        public int userCountNow { get; set; }
        public int vendorCountNow { get; set; }
        public int customerCountNow { get; set; }
        public int itemCountNow { get; set; }
        public int saleinvCountNow { get; set; }
       
        public int storeCountNow { get; set; }
     
        public Nullable<System.DateTime> serverDateNow { get; set; }

        public string customerName { get; set; }// 6- customer Name
        public string customerLastName { get; set; }// 6- customer LastName
        public string agentName { get; set; }// 5- Agent name 
        public string agentAccountName { get; set; }//5- Agent AccountName
        public string agentLastName { get; set; }//5- Agent LastName

        public string isDemo { get; set; }
    }
   
}