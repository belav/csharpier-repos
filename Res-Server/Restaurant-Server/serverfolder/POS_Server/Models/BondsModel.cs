using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class BondsModel
    {

        public long bondId { get; set; }
        public string number { get; set; }
        public decimal amount { get; set; }
        public Nullable<System.DateTime> deserveDate { get; set; }
        public string type { get; set; }
        public byte isRecieved { get; set; }
        public string notes { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public byte isActive { get; set; }
        public Nullable<long> cashTransId { get; set; }

      
        public Boolean canDelete { get; set; }
       
        // cash trans

        public long ctcashTransId { get; set; }
        public string cttransType { get; set; }
        public Nullable<long> ctposId { get; set; }
        public Nullable<long> ctuserId { get; set; }
        public Nullable<long> ctagentId { get; set; }
        public Nullable<long> ctinvId { get; set; }
        public string cttransNum { get; set; }
        public Nullable<System.DateTime> ctcreateDate { get; set; }
        public Nullable<System.DateTime> ctupdateDate { get; set; }
        public decimal ctcash { get; set; }
        public Nullable<long> ctupdateUserId { get; set; }
        public Nullable<long> ctcreateUserId { get; set; }
        public string ctnotes { get; set; }
        public Nullable<long> ctposIdCreator { get; set; }
        public byte ctisConfirm { get; set; }
        public Nullable<long> ctcashTransIdSource { get; set; }
        public string ctside { get; set; }
        public string ctopSideNum { get; set; }
        public string ctdocName { get; set; }
        public string ctdocNum { get; set; }
        public string ctdocImage { get; set; }
        public Nullable<long> ctbankId { get; set; }
        public string ctbankName { get; set; }
        public string ctagentName { get; set; }
        public string ctusersName { get; set; }
        public string ctusersLName { get; set; }
        public string ctposName { get; set; }
        public string ctposCreatorName { get; set; }
        public byte ctisConfirm2 { get; set; }
        public long ctcashTrans2Id { get; set; }
        public Nullable<long> ctpos2Id { get; set; }

        public string ctpos2Name { get; set; }
        public string ctprocessType { get; set; }
        public Nullable<long> ctcardId { get; set; }
        public Nullable<long> ctbondId { get; set; }
        public string ctcreateUserName { get; set; }
        public string ctcreateUserJob { get; set; }
        public string ctcreateUserLName { get; set; }
        public string ctcardName { get; set; }
        public Nullable<System.DateTime> ctbondDeserveDate { get; set; }
        public byte ctbondIsRecieved { get; set; }
        public Nullable<long> ctshippingCompanyId { get; set; }
        public string ctshippingCompanyName { get; set; }
    }
}