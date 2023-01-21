using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class SysEmailsModel
    {
        public long emailId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public bool isSSL { get; set; }
        public string smtpClient { get; set; }
        public string side { get; set; }
        public string notes { get; set; }
        public Nullable<long> branchId { get; set; }
        public byte isActive { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public bool isMajor { get; set; }

        public string branchName { get; set; }
        public bool canDelete { get; set; }

    }
}