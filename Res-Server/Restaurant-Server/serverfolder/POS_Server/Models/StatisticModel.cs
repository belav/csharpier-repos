using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class ItemUnitCostModel
    {

        ///////////////
        public long itemUnitId { get; set; }

        public Nullable<long> itemId { get; set; }
        public string itemName { get; set; }
        public Nullable<long> unitId { get; set; }
        public string unitName { get; set; }
        public Nullable<decimal> avgPurchasePrice { get; set; }
        //  public Nullable<decimal> smallunitcost { get; set; }
        public Nullable<decimal> cost { get; set; }
        public Nullable<decimal> finalcost { get; set; }
        public Nullable<decimal> diffPercent { get; set; }
        public string itemType { get; set; }
    }

    public class OrderPreparingSTSModel
    {
        public long orderPreparingId { get; set; }
        public string orderNum { get; set; }
        public Nullable<System.DateTime> orderTime { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public string notes { get; set; }
        public Nullable<decimal> preparingTime { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }


        // item
        public string itemName { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public int quantity { get; set; }
        //order
        public string status { get; set; }
        public int num { get; set; }
        public decimal remainingTime { get; set; }
        public string tables { get; set; }
        public string waiter { get; set; }
        //invoice

        public string invType { get; set; }
        public Nullable<long> shippingCompanyId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branchId { get; set; }

        public List<itemOrderPreparingModel> items { get; set; }
        //
        public Nullable<long> categoryId { get; set; }
        public string categoryName { get; set; }
        public Nullable<decimal> realDuration { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }

        public Nullable<long> tagId { get; set; }
        public string tagName { get; set; }
        public Nullable<System.DateTime> listedDate { get; set; }

        public string shipUserName { get; set; }
        public string shipUserLastName { get; set; }
        public string shippingCompanyName { get; set; }
        public Nullable<long> shipUserId { get; set; }
        //   agentId
        public Nullable<long> agentId { get; set; }
        public string agentName { get; set; }
        public string agentCompany { get; set; }
        public string agentType { get; set; }
        public string agentCode { get; set; }
        public List<orderPreparingStatusModel> orderStatusList { get; set; }
        public decimal orderDuration { get; set; }




    }
    public class orderPreparingStatusModel
    {
        public long orderStatusId { get; set; }
        public Nullable<long> orderPreparingId { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }

        public string notes { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
 
        public Nullable<byte> isActive { get; set; }
        public string updateUserName { get; set; }

    }

    public class ItemUnitInvoiceProfitModel
    {

        ///////////////
        //public Nullable<decimal> itemAdminPay { get; set; }
        //public Nullable<decimal>  AdminPay { get; set; }
        //public Nullable<decimal> itemunitProfitOld { get; set; }
        
        //public Nullable<decimal> itemPricePercent { get; set; }
        //public Nullable<decimal> invoiceTotal { get; set; }
        //
        public Nullable<decimal> item { get; set; }
        public Nullable<decimal> avgPurchasePrice { get; set; }
        public string ITitemName { get; set; }
        public string ITunitName { get; set; }
        public long ITitemsTransId { get; set; }
        public Nullable<long> ITitemUnitId { get; set; }

        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }
        public Nullable<long> ITquantity { get; set; }

        public Nullable<System.DateTime> ITupdateDate { get; set; }
        //  public Nullable<int> IT.createUserId { get; set; } 
        public Nullable<long> ITupdateUserId { get; set; }

        public Nullable<decimal> ITprice { get; set; }
        public string ITbarcode { get; set; }

        public string ITUpdateuserNam { get; set; }
        public string ITUpdateuserLNam { get; set; }
        public string ITUpdateuserAccNam { get; set; }
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<long> posId { get; set; }
        public string invType { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
       
        
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<decimal> discountValue { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> tax { get; set; }
        // public string name { get; set; }
        //  isApproved { get; set; }


        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }


        public string posName { get; set; }
        public string posCode { get; set; }
        public string agentName { get; set; }
        public string agentCode { get; set; }
        public string agentType { get; set; }

        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public string agentCompany { get; set; }
        public Nullable<decimal> subTotal { get; set; }
        public decimal purchasePrice { get; set; }
        public decimal totalwithTax { get; set; }
        public decimal subTotalNet { get; set; } // with invoice discount 
        public decimal itemunitProfit { get; set; }
        public decimal invoiceProfit { get; set; }
        public decimal shippingCost { get; set; }
        public decimal realShippingCost { get; set; }
        public decimal shippingProfit { get; set; }
        public decimal totalNoShip { get; set; }
        public decimal totalNetNoShip { get; set; }
        public string itemType { get; set; }
        //  public Nullable<decimal> ITdiscountpercent { get; set; }
        //net profit
        public long cashTransId { get; set; }
        public string transType { get; set; }
        public string transNum { get; set; }

        public string side { get; set; }
        public string processType { get; set; }
        public Nullable<long> cardId { get; set; }
    
    }

    public class ItemTransferInvoiceTax
    {// new properties
        public Nullable<System.DateTime> updateDate { get; set; }
       
        
        public string agentCompany { get; set; }

        // ItemTransfer
        public long ITitemsTransId { get; set; }
        public Nullable<long> ITitemUnitId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }

        public Nullable<decimal> ITprice { get; set; }

        public string ITnotes { get; set; }

        public string ITbarcode { get; set; }

        //invoice
        public long invoiceId { get; set; }

        public Nullable<long> agentId { get; set; }

        public string invType { get; set; }
        public string discountType { get; set; }

        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }

        public Nullable<long> IupdateUserId { get; set; }

        public string invCase { get; set; }

        public string Inotes { get; set; }
        public string vendorInvNum { get; set; }

        public string branchName { get; set; }
        public string posName { get; set; }
        public Nullable<System.DateTime> vendorInvDate { get; set; }
        public Nullable<long> branchId { get; set; }


        public Nullable<int> taxtype { get; set; }
        public Nullable<long> posId { get; set; }

        public string ITtype { get; set; }

        public string branchType { get; set; }

        public string posCode { get; set; }
        public string agentName { get; set; }

        public string agentType { get; set; }
        public string agentCode { get; set; }

        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public Nullable<decimal> itemUnitPrice { get; set; }


        public Nullable<decimal> subTotalTax { get; set; }


        public Nullable<decimal> OneitemUnitTax { get; set; }


        public Nullable<decimal> OneItemOfferVal { get; set; }
        public Nullable<decimal> OneItemPriceNoTax { get; set; }

        public Nullable<decimal> OneItemPricewithTax { get; set; }

        public Nullable<decimal> itemsTaxvalue { get; set; }


        //invoice

        public Nullable<decimal> tax { get; set; }//نسبة الضريبة
        public Nullable<decimal> totalwithTax { get; set; }//قيمة الفاتورة النهائية Totalnet
        public Nullable<decimal> totalNoTax { get; set; }//قيمة الفاتورة قبل الضريبة total
        public Nullable<decimal> invTaxVal { get; set; }//قيمة ضريبة الفاتورة TAX
        public Nullable<int> itemsRowsCount { get; set; }//عدداسطر الفاتورة

        //item
        public string ITitemName { get; set; }//اسم العنصر
        public string ITunitName { get; set; }//وحدة العنصر

        public Nullable<long> ITquantity { get; set; }//الكمية
        public Nullable<decimal> subTotalNotax { get; set; }//سعر العناصر قبل الضريبة Price
        public Nullable<decimal> itemUnitTaxwithQTY { get; set; }//قيم الضريبة للعناصر
        public string invNumber { get; set; }//رقم الفاتورة//item
        public string invBarcode { get; set; }//barcode الفاتورة//item
        public Nullable<System.DateTime> IupdateDate { get; set; }//تاريخ الفاتورة//item

        public Nullable<decimal> ItemTaxes { get; set; }//نسبة ضريبة العنصر

        //public string invNumber { get; set; }//رقم الفاتورة
        //public Nullable<System.DateTime> IupdateDate { get; set; }//تاريخ الفاتورة
        //public Nullable<decimal> tax { get; set; }//نسبة الضريبة
        //public Nullable<decimal> totalwithTax { get; set; }//قيمة الفاتورة النهائية Totalnet
        //public Nullable<decimal> totalNoTax { get; set; }//قيمة الفاتورة قبل الضريبة total
        //public Nullable<decimal> invTaxVal { get; set; }//قيمة ضريبة الفاتورة TAX
        //public Nullable<int> itemsRowsCount { get; set; }//عدداسطر الفاتورة
        // public Nullable<decimal> totalNet { get; set; }

    }

    public class OpenClosOperatinModel
    {
        //new prop
        public string invNumber { get; set; }
        public string MaininvNumber { get; set; }
        public string invType { get; set; }
        public Nullable<decimal> commissionValue { get; set; }
        public Nullable<decimal> commissionRatio { get; set; }
        //
        public long cashTransId { get; set; }
        public string transType { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> userId { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<long> invId { get; set; }
        public string transNum { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<decimal> cash { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string notes { get; set; }
        public Nullable<long> posIdCreator { get; set; }
        public Nullable<byte> isConfirm { get; set; }
        public Nullable<long> cashTransIdSource { get; set; }
        public string side { get; set; }
        public string opSideNum { get; set; }
        public string docName { get; set; }
        public string docNum { get; set; }
        public string docImage { get; set; }
        public Nullable<long> bankId { get; set; }
        public string bankName { get; set; }
        public string agentName { get; set; }
        public string usersName { get; set; }
        public string usersLName { get; set; }
        public string posName { get; set; }
        public string posCreatorName { get; set; }
        public Nullable<byte> isConfirm2 { get; set; }
        public long cashTrans2Id { get; set; }
        public Nullable<long> pos2Id { get; set; }

        public string pos2Name { get; set; }
        public string processType { get; set; }
        public Nullable<long> cardId { get; set; }
        public Nullable<long> bondId { get; set; }
        public string createUserName { get; set; }
        public string updateUserName { get; set; }
        public string updateUserJob { get; set; }
        public string updateUserAcc { get; set; }
        public string createUserJob { get; set; }
        public string createUserLName { get; set; }
        public string updateUserLName { get; set; }
        public string cardName { get; set; }
        public Nullable<System.DateTime> bondDeserveDate { get; set; }
        public Nullable<byte> bondIsRecieved { get; set; }
        public string agentCompany { get; set; }
        public Nullable<long> shippingCompanyId { get; set; }
        public string shippingCompanyName { get; set; }
        public string userAcc { get; set; }

        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorname { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branch2Id { get; set; }
        public string branch2Name { get; set; }




    }
    public class POSOpenCloseModel
    {
        public long cashTransId { get; set; }
        public string transType { get; set; }
        public Nullable<long> posId { get; set; }

        public string transNum { get; set; }

        public Nullable<decimal> cash { get; set; }

        public string notes { get; set; }

        public Nullable<byte> isConfirm { get; set; }
        public Nullable<long> cashTransIdSource { get; set; }
        public string side { get; set; }

        public string posName { get; set; }



        public string processType { get; set; }


        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<System.DateTime> openDate { get; set; }
        public Nullable<decimal> openCash { get; set; }
        public Nullable<long> openCashTransId { get; set; }



    }

    public class ItemTransferInvoiceSTS
    {// new properties

        public Nullable<long> membershipId { get; set; }
        public string membershipsName { get; set; }
        public string membershipsCode { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }

        public string agentCompany { get; set; }

        // ItemTransfer
        public long ITitemsTransId { get; set; }
        public Nullable<long> ITitemUnitId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }

        public Nullable<decimal> ITprice { get; set; }

        public string ITnotes { get; set; }

        public string ITbarcode { get; set; }

        //invoice
        public long invoiceId { get; set; }

        public Nullable<long> agentId { get; set; }

        public string invType { get; set; }
        public string discountType { get; set; }

        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }

        public Nullable<long> IupdateUserId { get; set; }

        public string invCase { get; set; }

        public string Inotes { get; set; }
        public string vendorInvNum { get; set; }

        public string branchName { get; set; }
        public string posName { get; set; }
        public Nullable<System.DateTime> vendorInvDate { get; set; }
        public Nullable<long> branchId { get; set; }


        public Nullable<int> taxtype { get; set; }
        public Nullable<long> posId { get; set; }

        public string ITtype { get; set; }

        public string branchType { get; set; }

        public string posCode { get; set; }
        public string agentName { get; set; }

        public string agentType { get; set; }
        public string agentCode { get; set; }

        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public Nullable<decimal> itemUnitPrice { get; set; }


        public Nullable<decimal> subTotalTax { get; set; }


        public Nullable<decimal> OneitemUnitTax { get; set; }


        public Nullable<decimal> OneItemOfferVal { get; set; }
        public Nullable<decimal> OneItemPriceNoTax { get; set; }

        public Nullable<decimal> OneItemPricewithTax { get; set; }

        public Nullable<decimal> itemsTaxvalue { get; set; }


        //invoice

        public Nullable<decimal> tax { get; set; }//نسبة الضريبة
        public Nullable<decimal> totalwithTax { get; set; }//قيمة الفاتورة النهائية Totalnet
        public Nullable<decimal> totalNoTax { get; set; }//قيمة الفاتورة قبل الضريبة total
        public Nullable<decimal> invTaxVal { get; set; }//قيمة ضريبة الفاتورة TAX
        public Nullable<int> itemsRowsCount { get; set; }//عدداسطر الفاتورة

        //item
        public string ITitemName { get; set; }//اسم العنصر
        public string ITunitName { get; set; }//وحدة العنصر

        public Nullable<long> ITquantity { get; set; }//الكمية
        public Nullable<decimal> subTotalNotax { get; set; }//سعر العناصر قبل الضريبة Price
        public Nullable<decimal> itemUnitTaxwithQTY { get; set; }//قيم الضريبة للعناصر
        public string invNumber { get; set; }//رقم الفاتورة//item
        public Nullable<System.DateTime> IupdateDate { get; set; }//تاريخ الفاتورة//item

        public Nullable<decimal> ItemTaxes { get; set; }//نسبة ضريبة العنصر

        public List<invoiceClassDiscount> invoiceClassDiscountList { get; set; }

    }

    public class SalesMembership
    {

        //invoice
        public long invoiceId { get; set; }
        public string invNumber { get; set; }

        public string invBarcode { get; set; }
        public string invType { get; set; }
        public string discountType { get; set; }

        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }

        public Nullable<long> invoiceMainId { get; set; }
        public string invCase { get; set; }
        public Nullable<System.TimeSpan> invTime { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }//
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<byte> isApproved { get; set; }
        public Nullable<decimal> tax { get; set; }

        public Nullable<long> updateUserId { get; set; }
        public int count { get; set; }



        //pos
        public Nullable<long> posId { get; set; }
        public string posName { get; set; }
        public string posCode { get; set; }
        //branch

        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public string branchType { get; set; }


        //agent
        public Nullable<long> agentId { get; set; }
        public string agentCompany { get; set; }
        public string agentName { get; set; }

        public string agentType { get; set; }
        public string agentCode { get; set; }
        public string vendorInvNum { get; set; }


        public Nullable<System.DateTime> vendorInvDate { get; set; }
        //user
        public Nullable<long> createUserId { get; set; }
        public string cuserName { get; set; }
        public string cuserLast { get; set; }
        public string cUserAccName { get; set; }
        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public Nullable<long> userId { get; set; }
        //membership

        public Nullable<long> membershipId { get; set; }
        public string membershipsCode { get; set; }
        public string membershipsName { get; set; }
        public List<CouponInvoiceModel> CouponInvoiceList { get; set; }
        public List<ItemTransferModel> itemsTransferList { get; set; }
        public List<invoicesClassModel> invoiceClassDiscountList { get; set; }
        public decimal invclassDiscount { get; set; }
        public decimal couponDiscount { get; set; }
        public decimal offerDiscount { get; set; }
        public decimal totalDiscount { get; set; }

        public Nullable<System.DateTime> endDate { get; set; }
        public string subscriptionType { get; set; }
        public AgentMembershipCashModel agentMembershipcashobj { get; set; }
        public List<AgentMembershipCashModel> agentMembershipcashobjList { get; set; }

        public string invoicesClassName { get; set; }
        public Nullable<long> invClassDiscountId { get; set; }

        public Nullable<long> invClassId { get; set; }
        public byte invClassdiscountType { get; set; }
        public decimal invClassdiscountValue { get; set; }
        public decimal finalDiscount { get; set; }

    }

    public class ItemTransferInvoice
    {// new properties
        public Nullable<decimal> totalNetRep { get; set; }
        public string mainInvNumber { get; set; }
        public string processType0 { get; set; }
        public decimal totalNet0 { get; set; }
        public int archived { get; set; }
        public double? itemAvg { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public string causeFalls { get; set; }
        public string causeDestroy { get; set; }
        public string userdestroy { get; set; }
        public string userFalls { get; set; }
        public Nullable<int> userId { get; set; }
        public string inventoryNum { get; set; }
        public string inventoryType { get; set; }
        public Nullable<DateTime> inventoryDate { get; set; }
        public long itemCount { get; set; }
        public Nullable<decimal> subTotal { get; set; }
        public string agentCompany { get; set; }
        public string itemName { get; set; }
        public string unitName { get; set; }
        public long itemsTransId { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }
        public Nullable<long> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
        public string barcode { get; set; }

        // ItemTransfer
        public int ITitemsTransId { get; set; }
        public Nullable<int> ITitemUnitId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public Nullable<int> ITitemId { get; set; }
        public Nullable<int> ITunitId { get; set; }
        public string ITitemName { get; set; }
        public string ITunitName { get; set; }
        private string ITitemUnitName;
        public Nullable<long> ITquantity { get; set; }
        public Nullable<decimal> ITprice { get; set; }


        public Nullable<System.DateTime> ITcreateDate { get; set; }
        public Nullable<System.DateTime> ITupdateDate { get; set; }
        public Nullable<int> ITcreateUserId { get; set; }
        public Nullable<int> ITupdateUserId { get; set; }
        public string ITnotes { get; set; }

        public string ITbarcode { get; set; }
        public string ITCreateuserName { get; set; }
        public string ITCreateuserLName { get; set; }
        public string ITCreateuserAccName { get; set; }

        public string ITUpdateuserName { get; set; }
        public string ITUpdateuserLName { get; set; }
        public string ITUpdateuserAccName { get; set; }
        //invoice
        public Nullable<int> sliceId { get; set; }
        public string sliceName { get; set; }
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public Nullable<int> agentId { get; set; }
        public Nullable<int> createUserId { get; set; }
        public string invType { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> ITdiscountValue { get; set; }
        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        public Nullable<System.DateTime> IupdateDate { get; set; }
        public Nullable<int> IupdateUserId { get; set; }
        public Nullable<int> invoiceMainId { get; set; }
        public string invCase { get; set; }
        public Nullable<System.TimeSpan> invTime { get; set; }
        public string Inotes { get; set; }
        public string vendorInvNum { get; set; }
        public string name { get; set; }
        public string branchName { get; set; }
        public Nullable<System.DateTime> vendorInvDate { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<int> itemsCount { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<int> taxtype { get; set; }
        public Nullable<int> posId { get; set; }
        public Nullable<byte> isApproved { get; set; }
        public Nullable<int> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public string ITtype { get; set; }
        private string invTypeNumber;//number
                                     //public string InvTypeNumber { get => invTypeNumber = invType + "-" + invNumber; set => invTypeNumber = value; }


        // for report
        //public int countP { get; set; }
        //public int countS { get; set; }
        public int count { get; set; }

        public Nullable<decimal> totalS { get; set; }
        public Nullable<decimal> totalNetS { get; set; }
        public Nullable<decimal> totalP { get; set; }
        public Nullable<decimal> totalNetP { get; set; }
        public string branchType { get; set; }
        public string posName { get; set; }
        public string posCode { get; set; }
        public string agentName { get; set; }


        public string agentType { get; set; }
        public string agentCode { get; set; }
        public string cuserName { get; set; }
        public string cuserLast { get; set; }
        public string cUserAccName { get; set; }
        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }


        public int countPb { get; set; }
        public int countD { get; set; }
        public Nullable<decimal> totalPb { get; set; }
        public Nullable<decimal> totalD { get; set; }
        public Nullable<decimal> totalNetPb { get; set; }
        public Nullable<decimal> totalNetD { get; set; }


        public Nullable<decimal> paidPb { get; set; }
        public Nullable<decimal> deservedPb { get; set; }
        public Nullable<decimal> discountValuePb { get; set; }
        public Nullable<decimal> paidD { get; set; }
        public Nullable<decimal> deservedD { get; set; }
        public Nullable<decimal> discountValueD { get; set; }
        // coupon


        public int CopcId { get; set; }
        public string Copname { get; set; }
        public string Copcode { get; set; }
        public Nullable<byte> CopisActive { get; set; }
        public Nullable<byte> CopdiscountType { get; set; }
        public Nullable<decimal> CopdiscountValue { get; set; }
        public Nullable<System.DateTime> CopstartDate { get; set; }
        public Nullable<System.DateTime> CopendDate { get; set; }
        public string Copnotes { get; set; }
        public Nullable<int> Copquantity { get; set; }
        public Nullable<int> CopremainQ { get; set; }
        public Nullable<decimal> CopinvMin { get; set; }
        public Nullable<decimal> CopinvMax { get; set; }
        public Nullable<System.DateTime> CopcreateDate { get; set; }
        public Nullable<System.DateTime> CopupdateDate { get; set; }
        public Nullable<int> CopcreateUserId { get; set; }
        public Nullable<int> CopupdateUserId { get; set; }
        public string Copbarcode { get; set; }
        public Nullable<decimal> couponTotalValue { get; set; }
        // offer

        public int OofferId { get; set; }
        public string Oname { get; set; }
        public string Ocode { get; set; }
        public Nullable<byte> OisActive { get; set; }
        public string OdiscountType { get; set; }
        public Nullable<decimal> OdiscountValue { get; set; }
        public Nullable<System.DateTime> OstartDate { get; set; }
        public Nullable<System.DateTime> OendDate { get; set; }
        public Nullable<System.DateTime> OcreateDate { get; set; }
        public Nullable<System.DateTime> OupdateDate { get; set; }
        public Nullable<int> OcreateUserId { get; set; }
        public Nullable<int> OupdateUserId { get; set; }
        public string Onotes { get; set; }
        public Nullable<int> Oquantity { get; set; }
        public int Oitemofferid { get; set; }
        public Nullable<decimal> offerTotalValue { get; set; }

        //external
        public int movbranchid { get; set; }
        public string movbranchname { get; set; }
        // internal
        public string exportBranch { get; set; }
        public string importBranch { get; set; }
        public int exportBranchId { get; set; }
        public int importBranchId { get; set; }

        public int invopr { get; set; }

        public string processType { get; set; }

        public List<itemsTransfer> invoiceItems { get; set; }
   
        public List<CashTransferModel> cachTransferList { get; set; }
        public List<InvoiceModel> returnInvList { get; set; }
        public InvoiceModel ChildInvoice { get; set; }


        /////////////////////


        public int isPrePaid { get; set; }

        public string notes { get; set; }

        public decimal cashReturn { get; set; }

        public Nullable<int> shippingCompanyId { get; set; }
        public string shipCompanyName { get; set; }
        public Nullable<int> shipUserId { get; set; }
        public string shipUserName { get; set; }
        public string status { get; set; }
        public int invStatusId { get; set; }
        public decimal manualDiscountValue { get; set; }
        public string manualDiscountType { get; set; }
        public string createrUserName { get; set; }
        public decimal shippingCost { get; set; }
        public decimal realShippingCost { get; set; }
        public bool isActive { get; set; }
        public string payStatus { get; set; }

        // for report

        public int printedcount { get; set; }
        public bool isOrginal { get; set; }
        public string agentAddress { get; set; }
        public string agentMobile { get; set; }
        //
        public Nullable<int> DBAgentId { get; set; }
        public Nullable<decimal> DBDiscountValue { get; set; }
        public string sales_invoice_note { get; set; }
        public string itemtax_note { get; set; }
        public bool isExpired { get; set; }
        public int alertDays { get; set; }
        public string CreateuserName { get; set; }
        public string CreateuserLName { get; set; }
        public string CreateuserAccName { get; set; }
        public string UpdateuserName { get; set; }
        public string UpdateuserLName { get; set; }
        public string UpdateuserAccName { get; set; }
        public bool performed { get; set; }
    }
}