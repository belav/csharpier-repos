using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class messagesPosModel
    {
        public long msgPosId { get; set; }
        public Nullable<long> msgId { get; set; }
        public Nullable<long> posId { get; set; }
        public bool isReaded { get; set; }
        public string notes { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }

        public bool canDelete { get; set; }
        public string posName { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<long> userReadId { get; set; }
        public string userRead { get; set; }
        public string toUserFullName { get; set; }
        public string userReadName { get; set; }
        public string userReadLastName { get; set; }

        //user
        public Nullable<long> toUserId { get; set; }

        //message

        public string title { get; set; }
        public string msgContent { get; set; }
        public bool isActive { get; set; }

        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public string msgCreatorName { get; set; }
        public string msgCreatorLast { get; set; }
        public Nullable<long> mainMsgId { get; set; }

        // public string msgNotes { get; set; }
    }
}
