using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ActivateModelModel
    {

    }
    public class PosSerialSend
    {

        public string serial { get; set; }
        public string posDeviceCode { get; set; }

        public Nullable<bool> isBooked { get; set; }
        public int isActive { get; set; }
        public string posName { get; set; }
        public string branchName { get; set; }
        public Nullable<long> posSettingId { get; set; }
        public Nullable<long> posId { get; set; }
        public bool unLimited { get; set; }



    }

    public class daysremain
    {
        public Nullable<int> days { get; set; }
        public string expirestate { get; set; }
        public Nullable<int> hours { get; set; }
        public Nullable<int> minute { get; set; }
    }
        public class packagesSend
    {
        public Nullable<int> packageUserId { get; set; }
        public string packageName { get; set; }

        public int branchCount { get; set; }
        public int posCount { get; set; }
        public int userCount { get; set; }
        public int vendorCount { get; set; }
        public int customerCount { get; set; }
        public int itemCount { get; set; }
        public int salesInvCount { get; set; }

        public string programName { get; set; }

        public string verName { get; set; }

        public int isActive { get; set; }

        public string packageCode { get; set; }

        public int storeCount { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public bool islimitDate { get; set; }
        public Nullable<bool> isOnlineServer { get; set; }
        public string customerServerCode { get; set; }
        public string packageSaleCode { get; set; }
        public int monthCount { get; set; }
        public bool canRenew { get; set; }
        public bool isBooked { get; set; }


        public Nullable<System.DateTime> bookDate { get; set; }

        public Nullable<System.DateTime> expireDate { get; set; }


        public string type { get; set; }
        public bool isPayed { get; set; }

        public Nullable<System.DateTime> activatedate { get; set; }
        public bool isServerActivated { get; set; }
        public int totalsalesInvCount { get; set; }
        public int result { get; set; }

        public string packageNumber { get; set; }

      
        public Nullable<int> pId { get; set; }
        public Nullable<int> pcdId { get; set; }
        public string activeState { get; set; }
        public string activeres { get; set; }

        public string customerName { get; set; }// 6- customer Name
        public string customerLastName { get; set; }// 6- customer LastName
        public string agentName { get; set; }// 5- Agent name 
        public string agentAccountName { get; set; }//5- Agent AccountName
        public string agentLastName { get; set; }//5- Agent LastName

        public Nullable<System.DateTime> pocrDate { get; set; }
        public Nullable<int> poId { get; set; }
        public string notes { get; set; }
        public string upnum { get; set; }
        public string packuserType { get; set; }
        public string activeApp { get; set; }
        public string confirmStat { get; set; }
        public string isDemo { get; set; }

    }
    public class SendDetail
    {
        public List<PosSerialSend> PosSerialSendList;

        public packagesSend packageSend;
    }
}