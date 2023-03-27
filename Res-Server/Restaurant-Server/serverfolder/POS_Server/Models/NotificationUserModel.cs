using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class NotificationUserModel
    {
        public long notUserId { get; set; }
        public Nullable<long> notId { get; set; }
        public Nullable<long> userId { get; set; }
        public bool isRead { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> posId { get; set; }

        public string objectName { get; set; }
        public string prefix { get; set; }

        public Nullable<long> recieveId { get; set; }
        public Nullable<long> branchId { get; set; }

        public string title { get; set; }
        public string ncontent { get; set; }
        public string side { get; set; }
        public string msgType { get; set; }
        public string path { get; set; }
    }
}