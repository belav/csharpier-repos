using LinqKit;
using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Core.Objects;
using POS_Server.Models.VM;
using System.Security.Claims;
using Newtonsoft.Json.Converters;
using System.Web;
using POS_Server.Classes;
using System.Threading.Tasks;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Invoices")]
    public class InvoicesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        ItemsTransferController itc = new ItemsTransferController();
        List<string> salesType = new List<string>() { "ssd", "ss", "tsd", "ts", "sd", "s" };
        List<string> purchaseType = new List<string>() { "pd", "p", "pbd", "pb" };
        List<string> spendingOrderType = new List<string>() { "sr", "srd" };

        [HttpPost]
        [Route("GetPurNot")]
        public string GetPurNot(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string result = "{";
                string invType = "";

                long userId = 0;
                int draftDuration = 0;
                int invoiceDuration = 0;
                List<string> invTypeL = new List<string>();
                List<string> draftTypeL = new List<string>();
                List<string> orderTypeL = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "draftType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            draftTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "orderType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            orderTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "draftDuration")
                    {
                        draftDuration = int.Parse(c.Value);
                    }
                    else if (c.Type == "invoiceDuration")
                    {
                        invoiceDuration = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                #endregion

                int count = GetCountByCreator(draftTypeL, draftDuration, userId);
                result += "PurchaseDraftCount:" + count;
                count = GetCountByCreator(invTypeL, invoiceDuration, userId);
                result += ",InvoiceCount:" + count;
                count = GetCountUnHandeledOrders(orderTypeL, 0, userId);
                result += ",OrdersCount:" + count;

                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var banksList = entity.invoices
                        .Select(
                            b =>
                                new
                                {
                                    b.invoiceId,
                                    b.invNumber,
                                    b.agentId,
                                    b.invType,
                                    b.discountType,
                                    b.discountValue,
                                    b.total,
                                    b.totalNet,
                                    b.paid,
                                    b.deserved,
                                    b.deservedDate,
                                    b.invDate,
                                    b.invoiceMainId,
                                    b.invCase,
                                    b.invTime,
                                    b.notes,
                                    b.vendorInvNum,
                                    b.vendorInvDate,
                                    b.createUserId,
                                    b.updateDate,
                                    b.updateUserId,
                                    b.branchId,
                                    b.tax,
                                    b.taxtype,
                                    b.name,
                                    b.isApproved,
                                    b.branchCreatorId,
                                    b.shippingCompanyId,
                                    b.shipUserId,
                                    b.userId,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(banksList);
                }
            }
        }

        [HttpPost]
        [Route("GetAvgItemPrice")]
        public string GetAvgItemPrice(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            decimal price = 0;
            int totalNum = 0;
            decimal smallUnitPrice = 0;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemUnitId = 0;
                long itemId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                    else if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var itemUnits = (
                        from i in entity.itemsUnits
                        where (i.itemId == itemId)
                        select (i.itemUnitId)
                    ).ToList();

                    price += getItemUnitSumPrice(itemUnits);

                    totalNum = getItemUnitTotalNum(itemUnits);

                    if (totalNum != 0)
                        smallUnitPrice = price / totalNum;

                    var smallestUnitId = (
                        from iu in entity.itemsUnits
                        where (itemUnits.Contains((long)iu.itemUnitId) && iu.unitId == iu.subUnitId)
                        select iu.itemUnitId
                    ).FirstOrDefault();

                    if (smallestUnitId == null || smallestUnitId == 0)
                    {
                        smallestUnitId = (
                            from u in entity.itemsUnits
                            where !entity.itemsUnits.Any(y => u.subUnitId == y.unitId)
                            where (itemUnits.Contains((long)u.itemUnitId))
                            select u.itemUnitId
                        ).FirstOrDefault();
                    }
                    if (
                        itemUnitId == smallestUnitId
                        || smallestUnitId == null
                        || smallestUnitId == 0
                    )
                        return TokenManager.GenerateToken(smallUnitPrice);
                    else
                    {
                        smallUnitPrice =
                            smallUnitPrice * getUpperUnitValue(smallestUnitId, itemUnitId);
                        return TokenManager.GenerateToken(smallUnitPrice);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetByInvoiceId")]
        public string GetByInvoiceId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var banksList = entity.invoices
                        .Where(b => b.invoiceId == invoiceId)
                        .Select(
                            b =>
                                new InvoiceModel
                                {
                                    invoiceId = b.invoiceId,
                                    invNumber = b.invNumber,
                                    agentId = b.agentId,
                                    invType = b.invType,
                                    total = b.total,
                                    totalNet = b.totalNet,
                                    paid = b.paid,
                                    deserved = b.deserved,
                                    deservedDate = b.deservedDate,
                                    invDate = b.invDate,
                                    invoiceMainId = b.invoiceMainId,
                                    invCase = b.invCase,
                                    invTime = b.invTime,
                                    notes = b.notes,
                                    vendorInvNum = b.vendorInvNum,
                                    vendorInvDate = b.vendorInvDate,
                                    createUserId = b.createUserId,
                                    updateDate = b.updateDate,
                                    updateUserId = b.updateUserId,
                                    branchId = b.branchId,
                                    discountType = b.discountType,
                                    discountValue = b.discountValue,
                                    tax = b.tax,
                                    taxtype = b.taxtype,
                                    name = b.name,
                                    isApproved = b.isApproved,
                                    branchCreatorId = b.branchCreatorId,
                                    shippingCompanyId = b.shippingCompanyId,
                                    shipUserId = b.shipUserId,
                                    userId = b.userId,
                                    printedcount = b.printedcount,
                                    isOrginal = b.isOrginal,
                                    waiterId = b.waiterId,
                                    shippingCost = b.shippingCost,
                                    realShippingCost = b.realShippingCost,
                                    reservationId = b.reservationId,
                                    orderTime = b.orderTime,
                                    shippingCostDiscount = b.shippingCostDiscount,
                                    membershipId = b.membershipId,
                                    invBarcode = b.invBarcode,
                                    itemsCount = entity.itemsTransfer
                                        .Where(x => x.invoiceId == invoiceId)
                                        .Select(x => x.itemsTransId)
                                        .ToList()
                                        .Count,
                                    performed =
                                        (
                                            entity.invoices
                                                .Where(y => y.invoiceMainId == b.invoiceId)
                                                .FirstOrDefault() == null
                                        )
                                            ? false
                                            : true,
                                }
                        )
                        .FirstOrDefault();
                    var dis = entity.couponsInvoices
                        .Where(C => C.InvoiceId == invoiceId)
                        .Select(
                            C =>
                                new
                                {
                                    C.id,
                                    C.couponId,
                                    C.InvoiceId,
                                    C.discountValue,
                                    C.discountType,
                                    C.forAgents,
                                }
                        )
                        .ToList();
                    banksList.discountValue =
                        (
                            banksList.discountType == "2"
                                ? banksList.discountValue * banksList.total / 100
                                : banksList.discountValue
                        )
                        + dis.Sum(
                            C =>
                                C.discountType == 2
                                    ? (C.discountValue * banksList.total / 100)
                                    : C.discountValue
                        );
                    banksList.discountType = "1";

                    return TokenManager.GenerateToken(banksList);
                }
            }
        }

        [HttpPost]
        [Route("getgeneratedInvoice")]
        public string getgeneratedInvoice(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainInvoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        mainInvoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var banksList = entity.invoices
                        .Where(b => b.invoiceMainId == mainInvoiceId)
                        .Select(
                            b =>
                                new
                                {
                                    b.invoiceId,
                                    b.invNumber,
                                    b.agentId,
                                    b.invType,
                                    b.total,
                                    b.totalNet,
                                    b.paid,
                                    b.deserved,
                                    b.deservedDate,
                                    b.invDate,
                                    b.invoiceMainId,
                                    b.invCase,
                                    b.invTime,
                                    b.notes,
                                    b.vendorInvNum,
                                    b.vendorInvDate,
                                    b.createUserId,
                                    b.updateDate,
                                    b.updateUserId,
                                    b.branchId,
                                    b.discountType,
                                    b.discountValue,
                                    b.tax,
                                    b.taxtype,
                                    b.name,
                                    b.isApproved,
                                    b.branchCreatorId,
                                    b.shippingCompanyId,
                                    b.shipUserId,
                                    b.userId,
                                    b.invBarcode
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(banksList);
                }
            }
        }

        [HttpPost]
        [Route("getById")]
        public string GetById(string token)
        {
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var banksList = entity.invoices
                        .Where(b => b.invoiceId == invoiceId)
                        .Select(
                            b =>
                                new
                                {
                                    b.invoiceId,
                                    b.invNumber,
                                    b.agentId,
                                    b.invType,
                                    b.total,
                                    b.totalNet,
                                    b.paid,
                                    b.deserved,
                                    b.deservedDate,
                                    b.invDate,
                                    b.invoiceMainId,
                                    b.invCase,
                                    b.invTime,
                                    b.notes,
                                    b.vendorInvNum,
                                    b.vendorInvDate,
                                    b.createUserId,
                                    b.updateDate,
                                    b.updateUserId,
                                    b.branchId,
                                    b.discountType,
                                    b.discountValue,
                                    b.tax,
                                    b.taxtype,
                                    b.name,
                                    b.isApproved,
                                    b.branchCreatorId,
                                    b.shippingCompanyId,
                                    b.shipUserId,
                                    b.userId,
                                    b.cashReturn,
                                    b.invBarcode
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(banksList);
                }
            }
        }

        [HttpPost]
        [Route("GetByInvNum")]
        public string GetByInvNum(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invNum = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invNum")
                    {
                        invNum = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (branchId == 0)
                    {
                        var banksList = (
                            from b in entity.invoices.Where(b => b.invNumber == invNum)
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                invBarcode = b.invBarcode,
                            }
                        ).FirstOrDefault();
                        return TokenManager.GenerateToken(banksList);
                    }
                    else
                    {
                        var banksList = (
                            from b in entity.invoices.Where(
                                b => b.invNumber == invNum && b.branchId == branchId
                            )
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                invBarcode = b.invBarcode,
                            }
                        ).FirstOrDefault();
                        return TokenManager.GenerateToken(banksList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetByInvoiceType")]
        public string GetByInvoiceType(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                long branchId = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (branchId == 0)
                    {
                        var invoicesList = (
                            from b in entity.invoices.Where(x => invTypeL.Contains(x.invType))
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                cashReturn = b.cashReturn,
                                invBarcode = b.invBarcode,
                            }
                        ).ToList();
                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                    else
                    {
                        var invoicesList = (
                            from b in entity.invoices.Where(
                                x => invTypeL.Contains(x.invType) && x.branchId == branchId
                            )
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                invBarcode = b.invBarcode,
                            }
                        ).ToList();
                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("getExportInvoices")]
        public string getExportInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                long branchId = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();
                    if (branchId != 0)
                        searchPredicate = searchPredicate.Or(
                            inv =>
                                inv.branchId == branchId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    var invoicesList = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorName = entity.invoices
                                .Where(m => m.invoiceId == b.invoiceMainId)
                                .Select(m => m.branches.name)
                                .FirstOrDefault(),
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            cashReturn = b.cashReturn,
                            shippingCost = b.shippingCost,
                            realShippingCost = b.realShippingCost,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();
                    if (invoicesList != null)
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("getExportImportInvoices")]
        public string getExportImportInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                long branchId = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    if (branchId != 0)
                        searchPredicate = searchPredicate.Or(
                            inv =>
                                inv.branchId == branchId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    var invoicesList = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorName =
                                b.invoiceMainId != null
                                    ? (
                                        from i in entity.invoices.Where(
                                            m => m.invoiceId == b.invoiceMainId
                                        )
                                        join b in entity.branches on i.branchId equals b.branchId
                                        select b.name
                                    ).FirstOrDefault()
                                    : (
                                        from i in entity.invoices.Where(
                                            m => m.invoiceMainId == b.invoiceId
                                        )
                                        join b in entity.branches on i.branchId equals b.branchId
                                        select b.name
                                    ).FirstOrDefault(),
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            cashReturn = b.cashReturn,
                            shippingCost = b.shippingCost,
                            realShippingCost = b.realShippingCost,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();
                    if (invoicesList != null)
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("GetInvoicesByCreator")]
        public string GetInvoicesByCreator(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invType = "";
                long createUserId = 0;
                int duration = 0;
                int hours = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "createUserId")
                    {
                        createUserId = long.Parse(c.Value);
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                    else if (c.Type == "hours")
                    {
                        hours = int.Parse(c.Value);
                    }
                }
                #endregion

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    searchPredicate = searchPredicate.And(inv => invTypeL.Contains(inv.invType));
                    searchPredicate = searchPredicate.And(inv => inv.createUserId == createUserId);
                    searchPredicate = searchPredicate.And(inv => inv.isActive == true);

                    if (duration > 0)
                    {
                        DateTime dt = Convert.ToDateTime(
                            DateTime.Today.AddDays(-duration).ToShortDateString()
                        );
                        searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    }
                    if (hours > 0)
                    {
                        DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-hours));
                        searchPredicate = searchPredicate.And(x => x.invDate >= dt);
                    }

                    var invoicesList = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            agentName = b.agents.name,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchName = x.name,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            cashReturn = b.cashReturn,
                            shippingCost = b.shippingCost,
                            shippingCostDiscount = b.shippingCostDiscount,
                            realShippingCost = b.realShippingCost,
                            orderTime = b.orderTime,
                            membershipId = b.membershipId,
                            invBarcode = b.invBarcode,
                            tables = (
                                from it in entity.invoiceTables.Where(
                                    y => y.invoiceId == b.invoiceId && y.isActive == 1
                                )
                                join ts in entity.tables on it.tableId equals ts.tableId
                                select new TableModel()
                                {
                                    tableId = it.tableId,
                                    name = ts.name,
                                    canDelete = false,
                                    isActive = it.isActive,
                                    createUserId = ts.createUserId,
                                    updateUserId = ts.updateUserId,
                                }
                            ).ToList(),
                        }
                    ).ToList();

                    if (invoicesList != null)
                    {
                        //string complete = "ready";
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            //complete = "ready";
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("GetInvoicesForAdmin")]
        public string GetInvoicesForAdmin(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                int duration = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    if (duration > 0)
                    {
                        DateTime dt = Convert.ToDateTime(
                            DateTime.Today.AddDays(-duration).ToShortDateString()
                        );
                        searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    }
                    searchPredicate = searchPredicate.And(inv => invTypeL.Contains(inv.invType));
                    searchPredicate = searchPredicate.And(inv => inv.isActive == true);

                    var invoicesList = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchName = x.name,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            cashReturn = b.cashReturn,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    if (invoicesList != null)
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("GetCountInvoicesForAdmin")]
        public string GetCountInvoicesForAdmin(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                int duration = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    if (duration > 0)
                    {
                        DateTime dt = Convert.ToDateTime(
                            DateTime.Today.AddDays(-duration).ToShortDateString()
                        );
                        searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    }
                    searchPredicate = searchPredicate.And(inv => invTypeL.Contains(inv.invType));
                    searchPredicate = searchPredicate.And(inv => inv.isActive == true);

                    var invoicesCount = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel() { invoiceId = b.invoiceId, }
                    )
                        .ToList()
                        .Count;
                    return TokenManager.GenerateToken(invoicesCount);
                }
            }
        }

        [HttpPost]
        [Route("GetCountByCreator")]
        public string GetCountByCreator(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invType = "";
                long createUserId = 0;
                int duration = 0;
                int hours = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "createUserId")
                    {
                        createUserId = long.Parse(c.Value);
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                    else if (c.Type == "hours")
                    {
                        hours = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    //#region search conditions
                    //var searchPredicate = PredicateBuilder.New<invoices>();

                    //if (duration > 0)
                    //{
                    //    DateTime dt = Convert.ToDateTime(DateTime.Today.AddDays(-duration).ToShortDateString());
                    //    searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    //}
                    //if (hours > 0)
                    //{
                    //    DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-hours));
                    //    searchPredicate = searchPredicate.And(inv => inv.invDate >= dt);
                    //}
                    //searchPredicate = searchPredicate.And(inv => invTypeL.Contains(inv.invType));
                    //searchPredicate = searchPredicate.And(inv => inv.createUserId == createUserId);
                    //searchPredicate = searchPredicate.And(inv => inv.isActive == true);
                    //#endregion

                    //var invoicesCount = (from b in entity.invoices.Where(searchPredicate)
                    //                     join l in entity.branches on b.branchId equals l.branchId into lj
                    //                     from x in lj.DefaultIfEmpty()
                    //                     select new InvoiceModel()
                    //                     {
                    //                         invoiceId = b.invoiceId,
                    //                         invNumber = b.invNumber,
                    //                         agentId = b.agentId,
                    //                         invType = b.invType,
                    //                         total = b.total,
                    //                         totalNet = b.totalNet,
                    //                         paid = b.paid,
                    //                         deserved = b.deserved,
                    //                         deservedDate = b.deservedDate,
                    //                         invDate = b.invDate,
                    //                         invoiceMainId = b.invoiceMainId,
                    //                         invCase = b.invCase,
                    //                         invTime = b.invTime,
                    //                         notes = b.notes,
                    //                         vendorInvNum = b.vendorInvNum,
                    //                         vendorInvDate = b.vendorInvDate,
                    //                         createUserId = b.createUserId,
                    //                         updateDate = b.updateDate,
                    //                         updateUserId = b.updateUserId,
                    //                         branchId = b.branchId,
                    //                         discountValue = b.discountValue,
                    //                         discountType = b.discountType,
                    //                         tax = b.tax,
                    //                         taxtype = b.taxtype,
                    //                         name = b.name,
                    //                         isApproved = b.isApproved,
                    //                         branchName = x.name,
                    //                         branchCreatorId = b.branchCreatorId,
                    //                         shippingCompanyId = b.shippingCompanyId,
                    //                         shipUserId = b.shipUserId,
                    //                         userId = b.userId,
                    //                         manualDiscountType = b.manualDiscountType,
                    //                         manualDiscountValue = b.manualDiscountValue,
                    //                        invBarcode= b.invBarcode,
                    //                     }).ToList().Count;

                    var invoicesCount = GetCountByCreator(invTypeL, duration, createUserId);
                    return TokenManager.GenerateToken(invoicesCount);
                }
            }
        }

        private int GetCountByCreator(List<string> invTypeL, long duration, long createUserId)
        {
            int hours = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                #region search conditions
                var searchPredicate = PredicateBuilder.New<invoices>();

                if (duration > 0)
                {
                    DateTime dt = Convert.ToDateTime(
                        DateTime.Today.AddDays(-duration).ToShortDateString()
                    );
                    searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                }
                if (hours > 0)
                {
                    DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-hours));
                    searchPredicate = searchPredicate.And(inv => inv.invDate >= dt);
                }
                searchPredicate = searchPredicate.And(inv => invTypeL.Contains(inv.invType));
                searchPredicate = searchPredicate.And(inv => inv.createUserId == createUserId);
                searchPredicate = searchPredicate.And(inv => inv.isActive == true);
                #endregion

                var invoicesCount = (
                    from b in entity.invoices.Where(searchPredicate)
                    join l in entity.branches on b.branchId equals l.branchId into lj
                    from x in lj.DefaultIfEmpty()
                    select new InvoiceModel()
                    {
                        invoiceId = b.invoiceId,
                        invNumber = b.invNumber,
                        agentId = b.agentId,
                        invType = b.invType,
                        total = b.total,
                        totalNet = b.totalNet,
                        paid = b.paid,
                        deserved = b.deserved,
                        deservedDate = b.deservedDate,
                        invDate = b.invDate,
                        invoiceMainId = b.invoiceMainId,
                        invCase = b.invCase,
                        invTime = b.invTime,
                        notes = b.notes,
                        vendorInvNum = b.vendorInvNum,
                        vendorInvDate = b.vendorInvDate,
                        createUserId = b.createUserId,
                        updateDate = b.updateDate,
                        updateUserId = b.updateUserId,
                        branchId = b.branchId,
                        discountValue = b.discountValue,
                        discountType = b.discountType,
                        tax = b.tax,
                        taxtype = b.taxtype,
                        name = b.name,
                        isApproved = b.isApproved,
                        branchName = x.name,
                        branchCreatorId = b.branchCreatorId,
                        shippingCompanyId = b.shippingCompanyId,
                        shipUserId = b.shipUserId,
                        userId = b.userId,
                    }
                ).ToList().Count;
                return invoicesCount;
            }
        }

        [HttpPost]
        [Route("getBranchInvoices")]
        public string getBranchInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invType = "";
                long branchCreatorId = 0;
                long branchId = 0;
                int duration = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchCreatorId")
                    {
                        branchCreatorId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();
                    if (branchCreatorId != 0)
                        searchPredicate = searchPredicate.And(
                            inv =>
                                inv.branchCreatorId == branchCreatorId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    if (branchId != 0)
                        searchPredicate = searchPredicate.Or(
                            inv =>
                                inv.branchId == branchId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    if (duration > 0)
                    {
                        DateTime dt = Convert.ToDateTime(
                            DateTime.Today.AddDays(-duration).ToShortDateString()
                        );
                        searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    }

                    var invoicesList = (
                        from b in entity.invoices.Where(searchPredicate)
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchName = x.name,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            cashReturn = b.cashReturn,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();
                    if (invoicesList != null)
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("GetInvoicesByBarcodeAndUser")]
        public string GetInvoicesByBarcodeAndUser(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                BranchesController bc = new BranchesController();
                string invNum = "";
                long branchId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invNum")
                    {
                        invNum = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    //get user branches permission
                    var branches = bc.BrListByBranchandUser(branchId, userId);
                    List<long> branchesIds = new List<long>();
                    for (int i = 0; i < branches.Count; i++)
                        branchesIds.Add(branches[i].branchId);

                    var invoice = (
                        from b in entity.invoices.Where(
                            b => b.invNumber == invNum && branchesIds.Contains((int)b.branchId)
                        )
                        join l in entity.branches on b.branchId equals l.branchId into lj
                        from x in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchName = x.name,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            realShippingCost = b.realShippingCost,
                            shippingCost = b.shippingCost,
                            invBarcode = b.invBarcode,
                        }
                    ).FirstOrDefault();
                    return TokenManager.GenerateToken(invoice);
                }
            }
        }

        [HttpPost]
        [Route("getUnHandeldOrders")]
        public string getUnHandeldOrders(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                long branchCreatorId = 0;
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                    else if (c.Type == "branchCreatorId")
                    {
                        branchCreatorId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                var invoicesList = getUnhandeledOrdersList(invType, branchCreatorId, branchId);

                return TokenManager.GenerateToken(invoicesList);
            }
        }

        [NonAction]
        public int GetCountUnHandeledOrders(
            List<string> invTypeL,
            int duration,
            long userId = 0,
            long branchCreatorId = 0,
            long branchId = 0
        )
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var searchPredicate = PredicateBuilder.New<invoices>();
                searchPredicate = searchPredicate.And(
                    inv => inv.isActive == true && invTypeL.Contains(inv.invType)
                );
                if (branchCreatorId != 0)
                    searchPredicate = searchPredicate.And(
                        inv =>
                            inv.branchCreatorId == branchCreatorId
                            && inv.isActive == true
                            && invTypeL.Contains(inv.invType)
                    );

                if (branchId != 0)
                    searchPredicate = searchPredicate.And(inv => inv.branchId == branchId);
                if (duration > 0)
                {
                    DateTime dt = Convert.ToDateTime(
                        DateTime.Today.AddDays(-duration).ToShortDateString()
                    );
                    searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                }
                if (userId != 0)
                    searchPredicate = searchPredicate.And(inv => inv.createUserId == userId);

                var invoicesCount = (
                    from b in entity.invoices.Where(searchPredicate)
                    join l in entity.branches on b.branchId equals l.branchId into lj
                    from x in lj.DefaultIfEmpty()
                    where !entity.invoices.Any(y => y.invoiceMainId == b.invoiceId)
                    select new InvoiceModel() { invoiceId = b.invoiceId, invNumber = b.invNumber, }
                )
                    .ToList()
                    .Count;
                return invoicesCount;
            }
        }

        [HttpPost]
        [Route("GetCountBranchInvoices")]
        public string GetCountBranchInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invType = "";
                long branchCreatorId = 0;
                long branchId = 0;
                int duration = 0;
                List<string> invTypeL = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchCreatorId")
                    {
                        branchCreatorId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                }
                #endregion

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    if (branchCreatorId != 0)
                        searchPredicate = searchPredicate.And(
                            inv =>
                                inv.branchCreatorId == branchCreatorId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    if (branchId != 0)
                        searchPredicate = searchPredicate.Or(
                            inv =>
                                inv.branchId == branchId
                                && inv.isActive == true
                                && invTypeL.Contains(inv.invType)
                        );

                    if (duration > 0)
                    {
                        DateTime dt = Convert.ToDateTime(
                            DateTime.Today.AddDays(-duration).ToShortDateString()
                        );
                        searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                    }

                    var invoicesCount = (
                        from b in entity.invoices.Where(searchPredicate)
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                        }
                    )
                        .ToList()
                        .Count;

                    return TokenManager.GenerateToken(invoicesCount);
                }
            }
        }

        [HttpPost]
        [Route("getDeliverOrders")]
        public string getDeliverOrders(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                long shipUserId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        shipUserId = long.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    searchPredicate = searchPredicate.And(
                        x => x.invType == "ts" || x.invType == "ss"
                    );
                    searchPredicate = searchPredicate.And(x => x.shipUserId == shipUserId);

                    var invoices = (
                        from b in entity.invoices.Where(searchPredicate)
                        join u in entity.users on b.shipUserId equals u.userId into lj
                        from y in lj.DefaultIfEmpty()
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            agentName = b.agents.name,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    foreach (InvoiceModel inv in invoices)
                    {
                        var prepOrders = (
                            from o in entity.orderPreparing.Where(x => x.invoiceId == inv.invoiceId)
                            join s in entity.orderPreparingStatus
                                on o.orderPreparingId equals s.orderPreparingId
                            where
                                (
                                    s.orderStatusId
                                    == entity.orderPreparingStatus
                                        .Where(x => x.orderPreparingId == o.orderPreparingId)
                                        .Max(x => x.orderStatusId)
                                )
                            select new OrderPreparingModel()
                            {
                                orderPreparingId = o.orderPreparingId,
                                status = s.status,
                            }
                        ).ToList();

                        foreach (OrderPreparingModel o in prepOrders)
                        {
                            #region set inv status
                            if (o.status == "Collected")
                            {
                                inv.status = "Collected";
                                break;
                            }
                            else if (o.status == "InTheWay")
                            {
                                inv.status = "InTheWay";
                                break;
                            }
                            else if (o.status == "Done")
                            {
                                inv.status = "Done";
                                break;
                            }
                            else if (o.status == "Listed" || o.status == "Preparing")
                            {
                                inv.status = "Listed";
                                break;
                            }
                            else
                                inv.status = "Ready";
                            #endregion
                        }
                        var itemList = itc.Get(inv.invoiceId);
                        inv.itemsCount = itemList.Count;
                    }

                    #region get orders according to status
                    invoices = invoices.Where(x => x.status == "Ready").ToList();
                    #endregion


                    return TokenManager.GenerateToken(invoices);
                }
            }
        }

        [HttpPost]
        [Route("getDeliverOrdersCount")]
        public string getDeliverOrdersCount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                long shipUserId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        shipUserId = long.Parse(c.Value);
                    }
                }
                #endregion
                int count = getDeliverOrdersCount(shipUserId);

                return TokenManager.GenerateToken(count);
            }
        }

        [NonAction]
        public int getDeliverOrdersCount(long shipUserId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var searchPredicate = PredicateBuilder.New<invoices>();

                searchPredicate = searchPredicate.And(x => x.invType == "ts" || x.invType == "ss");
                searchPredicate = searchPredicate.And(x => x.shipUserId == shipUserId);

                var invoices = (
                    from x in entity.invoices.Where(searchPredicate)
                    join u in entity.users on x.shipUserId equals u.userId into lj
                    from y in lj.DefaultIfEmpty()
                    select new InvoiceModel()
                    {
                        invNumber = x.invNumber,
                        invoiceId = x.invoiceId,
                        shipUserId = x.shipUserId,
                        shipUserName = y.name,
                        shipUserLastName = y.lastname,
                        shippingCompanyId = x.shippingCompanyId,
                        shippingCompanyName = x.shippingCompanies.name,
                        orderTime = x.orderTime,
                    }
                ).ToList();

                foreach (InvoiceModel inv in invoices)
                {
                    var prepOrders = (
                        from o in entity.orderPreparing.Where(x => x.invoiceId == inv.invoiceId)
                        join s in entity.orderPreparingStatus
                            on o.orderPreparingId equals s.orderPreparingId
                        where
                            (
                                s.orderStatusId
                                == entity.orderPreparingStatus
                                    .Where(x => x.orderPreparingId == o.orderPreparingId)
                                    .Max(x => x.orderStatusId)
                            )
                        select new OrderPreparingModel()
                        {
                            orderPreparingId = o.orderPreparingId,
                            status = s.status,
                        }
                    ).ToList();

                    foreach (OrderPreparingModel o in prepOrders)
                    {
                        #region set inv status
                        if (o.status == "Collected")
                        {
                            inv.status = "Collected";
                            break;
                        }
                        else if (o.status == "InTheWay")
                        {
                            inv.status = "InTheWay";
                            break;
                        }
                        else if (o.status == "Done")
                        {
                            inv.status = "Done";
                            break;
                        }
                        else if (o.status == "Listed" || o.status == "Preparing")
                        {
                            inv.status = "Listed";
                            break;
                        }
                        else
                            inv.status = "Ready";
                        #endregion
                    }
                }

                var invoicesCount = invoices.Where(x => x.status == "Ready").Count();
                return invoicesCount;
            }
        }

        [HttpPost]
        [Route("getOrdersForPay")]
        public string getOrdersForPay(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                List<string> statusL = new List<string>();

                statusL.Add("InTheWay");
                statusL.Add("Done");
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoicesList = (
                        from b in entity.invoices.Where(
                            x =>
                                (x.invType == "s" || x.invType == "ts" || x.invType == "ss")
                                && x.branchCreatorId == branchId
                                && x.shipUserId != null
                                && x.isActive == true
                        )
                        join op in entity.orderPreparing on b.invoiceId equals op.invoiceId
                        join s in entity.orderPreparingStatus
                            on op.orderPreparingId equals s.orderPreparingId
                        join u in entity.users on b.shipUserId equals u.userId into lj
                        from y in lj.DefaultIfEmpty()
                        where
                            (
                                statusL.Contains(s.status)
                                && s.orderStatusId
                                    == entity.orderPreparingStatus
                                        .Where(x => x.orderPreparing.invoiceId == b.invoiceId)
                                        .Max(x => x.orderStatusId)
                            )
                        select new InvoiceModel()
                        {
                            invStatusId = s.orderStatusId,
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            agentName = b.agents.name,
                            shipUserName = y.name + " " + y.lastname,
                            status = s.status,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            shippingCost = b.shippingCost,
                            realShippingCost = b.realShippingCost,
                            payStatus =
                                b.deserved == 0
                                    ? "payed"
                                    : (b.deserved == b.totalNet ? "unpayed" : "partpayed"),
                            branchCreatorName = entity.branches
                                .Where(X => X.branchId == b.branchCreatorId)
                                .FirstOrDefault()
                                .name,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();
                    if (invoicesList != null)
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("getAgentInvoices")]
        public string getAgentInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                long agentId = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "agentId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }

                List<string> typesList = new List<string>();
                if (type.Equals("feed"))
                {
                    typesList.Add("pb");
                    typesList.Add("s");
                    typesList.Add("ts");
                    typesList.Add("ss");
                }
                else if (type.Equals("pay"))
                {
                    typesList.Add("p");
                    typesList.Add("sb");
                    typesList.Add("pw");
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoicesList = (
                        from b in entity.invoices.Where(
                            x =>
                                x.agentId == agentId
                                && typesList.Contains(x.invType)
                                && x.deserved > 0
                                && x.branchCreatorId == branchId
                                && (
                                    (x.shippingCompanyId == null && x.shipUserId == null)
                                    || (
                                        x.shippingCompanyId != null
                                        && x.shipUserId == null
                                        && x.isPrePaid == 1
                                    )
                                    || (x.shippingCompanyId != null && x.shipUserId != null)
                                )
                        )
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            realShippingCost = b.realShippingCost,
                            shippingCost = b.shippingCost,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    invoicesList = invoicesList
                        .Where(
                            inv =>
                                inv.invoiceMainId == null
                                || (
                                    inv.invoiceMainId != null
                                    && entity.invoices
                                        .Where(
                                            x =>
                                                x.invoiceId == inv.invoiceMainId
                                                && x.invType != "s"
                                                && x.invType != "p"
                                        )
                                        .FirstOrDefault() != null
                                )
                        )
                        .ToList();
                    //get only with rc status
                    if (type == "feed")
                    {
                        List<InvoiceModel> res = new List<InvoiceModel>();
                        foreach (InvoiceModel inv in invoicesList)
                        {
                            long invoiceId = inv.invoiceId;

                            var statusObj = entity.orderPreparingStatus
                                .Where(
                                    x =>
                                        x.orderPreparing.invoiceId == invoiceId
                                        && x.status == "Done"
                                )
                                .FirstOrDefault();

                            if (statusObj != null)
                            {
                                inv.invoiceItems = itc.Get(invoiceId);
                                inv.itemsCount = inv.invoiceItems.Count;
                                res.Add(inv);
                            }
                        }
                        return TokenManager.GenerateToken(res);
                    }
                    else
                    {
                        for (int i = 0; i < invoicesList.Count; i++)
                        {
                            long invoiceId = invoicesList[i].invoiceId;
                            invoicesList[i].invoiceItems = itc.Get(invoiceId);
                            invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                        }
                        return TokenManager.GenerateToken(invoicesList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("getNotPaidAgentInvoices")]
        public string getNotPaidAgentInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long agentId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoicesList = (
                        from b in entity.invoices.Where(x => x.agentId == agentId && x.deserved > 0)
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("getShipCompanyInvoices")]
        public string getShipCompanyInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long shippingCompanyId = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                List<string> typesList = new List<string>();
                if (type.Equals("feed"))
                {
                    typesList.Add("pb");
                    typesList.Add("s");
                    typesList.Add("ts");
                    typesList.Add("ss");
                }
                else if (type.Equals("pay"))
                {
                    typesList.Add("p");
                    typesList.Add("sb");
                    typesList.Add("pw");
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoicesList = (
                        from b in entity.invoices.Where(
                            x =>
                                x.shippingCompanyId == shippingCompanyId
                                && typesList.Contains(x.invType)
                                && x.deserved > 0
                                && x.shippingCompanyId != null
                                && x.shipUserId == null
                                && x.agentId != null
                                && x.isPrePaid == 0
                        )
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            vendorInvNum = b.vendorInvNum,
                            vendorInvDate = b.vendorInvDate,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            shippingCompanyId = b.shippingCompanyId,
                            shipUserId = b.shipUserId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    invoicesList = invoicesList
                        .Where(
                            inv =>
                                inv.invoiceId
                                == invoicesList
                                    .Where(i => i.invNumber == inv.invNumber)
                                    .ToList()
                                    .OrderBy(i => i.invoiceId)
                                    .FirstOrDefault()
                                    .invoiceId
                        )
                        .ToList();

                    //get only with rc status
                    if (type == "feed")
                    {
                        List<InvoiceModel> res = new List<InvoiceModel>();
                        foreach (InvoiceModel inv in invoicesList)
                        {
                            long invoiceId = inv.invoiceId;

                            var statusObj = entity.orderPreparingStatus
                                .Where(
                                    x =>
                                        x.orderPreparing.invoiceId == invoiceId
                                        && x.status == "Done"
                                )
                                .FirstOrDefault();

                            if (statusObj != null)
                            {
                                inv.invoiceItems = itc.Get(invoiceId);
                                inv.itemsCount = inv.invoiceItems.Count;
                                res.Add(inv);
                            }
                        }
                        return TokenManager.GenerateToken(res);
                    }
                    else
                    {
                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("getUserInvoices")]
        public string getUserInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                long userId = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                List<string> typesList = new List<string>();
                if (type.Equals("feed"))
                {
                    typesList.Add("pb");
                    typesList.Add("s");
                    typesList.Add("ss");
                    typesList.Add("ts");
                }
                else if (type.Equals("pay"))
                {
                    typesList.Add("p");
                    typesList.Add("sb");
                    typesList.Add("pw");
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoicesList = (
                        from b in entity.invoices.Where(
                            x =>
                                x.userId == userId
                                && typesList.Contains(x.invType)
                                && x.deserved > 0
                                && x.branchCreatorId == branchId
                        )
                        select new InvoiceModel()
                        {
                            invoiceId = b.invoiceId,
                            invNumber = b.invNumber,
                            agentId = b.agentId,
                            invType = b.invType,
                            total = b.total,
                            totalNet = b.totalNet,
                            paid = b.paid,
                            deserved = b.deserved,
                            deservedDate = b.deservedDate,
                            invDate = b.invDate,
                            invoiceMainId = b.invoiceMainId,
                            invCase = b.invCase,
                            invTime = b.invTime,
                            notes = b.notes,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            branchId = b.branchId,
                            discountValue = b.discountValue,
                            discountType = b.discountType,
                            tax = b.tax,
                            taxtype = b.taxtype,
                            name = b.name,
                            isApproved = b.isApproved,
                            branchCreatorId = b.branchCreatorId,
                            userId = b.userId,
                            manualDiscountType = b.manualDiscountType,
                            manualDiscountValue = b.manualDiscountValue,
                            invBarcode = b.invBarcode,
                        }
                    ).ToList();

                    //get only with rc status
                    if (type == "feed")
                    {
                        List<InvoiceModel> res = new List<InvoiceModel>();
                        foreach (InvoiceModel inv in invoicesList)
                        {
                            long invoiceId = inv.invoiceId;

                            var statusObj = entity.orderPreparingStatus
                                .Where(
                                    x =>
                                        x.orderPreparing.invoiceId == invoiceId
                                        && x.status == "Done"
                                )
                                .FirstOrDefault();

                            if (statusObj != null)
                            {
                                inv.invoiceItems = itc.Get(invoiceId);
                                inv.itemsCount = inv.invoiceItems.Count;
                                res.Add(inv);
                            }
                        }
                        return TokenManager.GenerateToken(res);
                    }
                    else
                    {
                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetOrderByType")]
        public string GetOrderByType(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                long branchId = 0;
                List<string> invTypeL = new List<string>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (branchId == 0)
                    {
                        var invoicesList = (
                            from b in entity.invoices.Where(
                                x => invTypeL.Contains(x.invType) && x.invoiceMainId == null
                            )
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                cashReturn = b.cashReturn,
                                invBarcode = b.invBarcode,
                            }
                        ).ToList();

                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                    else
                    {
                        var invoicesList = (
                            from b in entity.invoices.Where(
                                x =>
                                    invTypeL.Contains(x.invType)
                                    && x.branchId == branchId
                                    && x.invoiceMainId == null
                            )
                            join l in entity.branches on b.branchId equals l.branchId into lj
                            from x in lj.DefaultIfEmpty()
                            select new InvoiceModel()
                            {
                                invoiceId = b.invoiceId,
                                invNumber = b.invNumber,
                                agentId = b.agentId,
                                invType = b.invType,
                                total = b.total,
                                totalNet = b.totalNet,
                                paid = b.paid,
                                deserved = b.deserved,
                                deservedDate = b.deservedDate,
                                invDate = b.invDate,
                                invoiceMainId = b.invoiceMainId,
                                invCase = b.invCase,
                                invTime = b.invTime,
                                notes = b.notes,
                                vendorInvNum = b.vendorInvNum,
                                vendorInvDate = b.vendorInvDate,
                                createUserId = b.createUserId,
                                updateDate = b.updateDate,
                                updateUserId = b.updateUserId,
                                branchId = b.branchId,
                                discountValue = b.discountValue,
                                discountType = b.discountType,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchName = x.name,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shipUserId = b.shipUserId,
                                userId = b.userId,
                                manualDiscountType = b.manualDiscountType,
                                manualDiscountValue = b.manualDiscountValue,
                                invBarcode = b.invBarcode,
                            }
                        ).ToList();

                        if (invoicesList != null)
                        {
                            for (int i = 0; i < invoicesList.Count; i++)
                            {
                                long invoiceId = invoicesList[i].invoiceId;
                                invoicesList[i].invoiceItems = itc.Get(invoiceId);
                                invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                            }
                        }

                        return TokenManager.GenerateToken(invoicesList);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GetLastNumOfInv")]
        public string GetLastNumOfInv(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invCode = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invCode")
                    {
                        invCode = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                List<string> numberList;
                int lastNum = 0;
                using (incposdbEntities entity = new incposdbEntities())
                {
                    numberList = entity.invoices
                        .Where(b => b.invNumber.Contains(invCode + "-") && b.branchId == branchId)
                        .Select(b => b.invNumber)
                        .ToList();

                    for (int i = 0; i < numberList.Count; i++)
                    {
                        string code = numberList[i];
                        string s = code.Substring(code.LastIndexOf("-") + 1);
                        numberList[i] = s;
                    }
                    if (numberList.Count > 0)
                    {
                        numberList.Sort();
                        lastNum = int.Parse(numberList[numberList.Count - 1]);
                    }
                }
                return TokenManager.GenerateToken(lastNum);
            }
        }

        [HttpPost]
        [Route("GetLastDialyNumOfInv")]
        public async Task<string> GetLastDialyNumOfInv(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invCode = "";
                string invType = "";
                long branchId = 0;
                List<string> invTypeL = new List<string>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "invType")
                    {
                        invType = c.Value;
                        string[] invTypeArray = invType.Split(',');
                        foreach (string s in invTypeArray)
                            invTypeL.Add(s.Trim());
                    }
                }

                int lastNum = 0;
                using (incposdbEntities entity = new incposdbEntities())
                {
                    DateTime dateSearch = DateTime.Parse(DateTime.Now.ToString().Split(' ')[0]);

                    lastNum = entity.invoices
                        .Where(
                            b =>
                                invTypeL.Contains(b.invType)
                                && b.branchId == branchId
                                && b.invDate >= dateSearch
                        )
                        .Select(b => b.invNumber)
                        .Count();
                }
                return TokenManager.GenerateToken(lastNum);
            }
        }

        public async Task<string> GetLastDialyNumOfInv(long branchId)
        {
            int lastNum = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                DateTime dateSearch = DateTime.Parse(DateTime.Now.ToString().Split(' ')[0]);

                lastNum = entity.invoices
                    .Where(
                        b =>
                            salesType.Contains(b.invType)
                            && b.branchId == branchId
                            && b.invDate >= dateSearch
                    )
                    .Select(b => b.invNumber)
                    .Count();
            }
            lastNum++;
            string strSeq = lastNum.ToString();
            if (lastNum <= 9999)
                strSeq = lastNum.ToString().PadLeft(4, '0');
            string invoiceNum = strSeq;
            return invoiceNum;
        }

        [HttpPost]
        [Route("clearInvoiceCouponsAndClasses")]
        public string clearInvoiceCouponsAndClasses(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // remove invoice coupons
                    var oldList = entity.couponsInvoices.Where(p => p.InvoiceId == invoiceId);
                    if (oldList.Count() > 0)
                    {
                        entity.couponsInvoices.RemoveRange(oldList);
                    }

                    // remove inv class
                    var invClass = entity.invoiceClassDiscount
                        .Where(x => x.invoiceId == invoiceId)
                        .FirstOrDefault();
                    if (invClass != null)
                        entity.invoiceClassDiscount.Remove(invClass);

                    entity.SaveChanges();
                }
                message = "1";
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("saveMemberShipClassDis")]
        public string saveMemberShipClassDis(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string classObject = "";
                long invoiceId = 0;
                invoicesClass Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        classObject = c.Value.Replace("\\", string.Empty);
                        classObject = classObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<invoicesClass>(
                            classObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        //break;
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var oldList = entity.invoiceClassDiscount.Where(
                            p => p.invoiceId == invoiceId
                        );
                        if (oldList.Count() > 0)
                        {
                            entity.invoiceClassDiscount.RemoveRange(oldList);
                        }

                        invoiceClassDiscount inCls = new invoiceClassDiscount()
                        {
                            invoiceId = invoiceId,
                            invClassId = Object.invClassId,
                            discountType = Object.discountType,
                            discountValue = Object.discountValue,
                        };

                        entity.invoiceClassDiscount.Add(inCls);

                        entity.SaveChanges();
                        message = inCls.invClassDiscountId.ToString();
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        // for report
        [HttpPost]
        [Route("GetinvCountBydate")]
        public string GetinvCountBydate(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invType = "";
                string branchType = "";
                DateTime startDate = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime endDate = coctrlr.AddOffsetTodate(DateTime.Now);
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                    else if (c.Type == "branchType")
                    {
                        branchType = c.Value;
                    }
                    else if (c.Type == "startDate")
                    {
                        startDate = DateTime.Parse(c.Value);
                    }
                    else if (c.Type == "endDate")
                    {
                        endDate = DateTime.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invListm = (
                        from I in entity.invoices
                        join B in entity.branches on I.branchId equals B.branchId into JB
                        from JBB in JB.DefaultIfEmpty()
                        where //(invtype == "all" ? true : I.invType == invtype)  &&
                            (branchType == "all" ? true : JBB.type == branchType)
                            && System.DateTime.Compare((DateTime)startDate, (DateTime)I.invDate)
                                <= 0
                            && System.DateTime.Compare((DateTime)endDate, (DateTime)I.invDate) >= 0
                        // I.invType == invtype
                        //     && branchType == "all" ? true : JBB.type == branchType

                        //  && startDate <= I.invDate && endDate >= I.invDate
                        // &&  System.DateTime.Compare((DateTime)startDate,  I.invDate) <= 0 && System.DateTime.Compare((DateTime)endDate, I.invDate) >= 0
                        group new { I, JBB } by (I.branchId) into g
                        select new
                        {
                            branchId = g.Key,
                            name = g.Select(t => t.JBB.name).FirstOrDefault(),
                            countP = g.Where(t => t.I.invType == "p").Count(),
                            countS = g.Where(t => t.I.invType == "s").Count(),
                            totalS = g.Where(t => t.I.invType == "s").Sum(S => S.I.total),
                            totalNetS = g.Where(t => t.I.invType == "s").Sum(S => S.I.totalNet),
                            totalP = g.Where(t => t.I.invType == "p").Sum(S => S.I.total),
                            totalNetP = g.Where(t => t.I.invType == "p").Sum(S => S.I.totalNet),
                            paid = g.Sum(S => S.I.paid),
                            deserved = g.Sum(S => S.I.deserved),
                            discountValue = g.Sum(
                                S =>
                                    S.I.discountType == "1"
                                        ? S.I.discountValue
                                        : (S.I.discountType == "2" ? (S.I.discountValue / 100) : 0)
                            ),
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(invListm);
                }
            }
        }

        // add or update bank
        [HttpPost]
        [Route("Save")]
        public async Task<string> Save(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invoiceObject = "";
                invoices newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        break;
                    }
                }
                try
                {
                    long res = await saveInvoice(newObject);
                    message = res.ToString();
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        public string generateSaleInvBarcode(string branchCode, string invNumber)
        {
            return "si-"
                + branchCode
                + "-"
                + DateTime.Now.ToString().Split(' ')[0]
                + "-"
                + invNumber;
        }

        [NonAction]
        public async Task<long> saveSalesInvoice(invoices newObject)
        {
            long res = 0;
            invoices tmpInvoice;

            #region generate InvNumber - invoice barcode
            long branchId = (long)newObject.branchCreatorId;
            string branchCode = "";
            using (incposdbEntities entity = new incposdbEntities())
            {
                var branch = entity.branches.Find(branchId);
                branchCode = branch.code;
            }
            if (newObject.invoiceId == 0 || newObject.invNumber == "")
            {
                string invNumber = await GetLastDialyNumOfInv(branchId);
                newObject.invNumber = invNumber;
            }
            newObject.invBarcode = generateSaleInvBarcode(branchCode, newObject.invNumber);
            #endregion

            ProgramDetailsController pc = new ProgramDetailsController();
            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoiceEntity = entity.Set<invoices>();
                if (newObject.invoiceId == 0)
                {
                    if (
                        newObject.invType == "s"
                        || newObject.invType == "ss"
                        || newObject.invType == "ts"
                    )
                    {
                        ProgramInfo programInfo = new ProgramInfo();
                        int invMaxCount = programInfo.getSaleinvCount();
                        int salesInvCount = pc.getSalesInvCountInMonth();
                        if (salesInvCount >= invMaxCount && invMaxCount != -1)
                        {
                            res = -1;
                        }
                        else
                        {
                            newObject.invDate = datenow;
                            newObject.invTime = datenow.TimeOfDay;
                            newObject.updateDate = datenow;
                            newObject.updateUserId = newObject.createUserId;
                            newObject.isActive = true;
                            newObject.isOrginal = true;
                            tmpInvoice = invoiceEntity.Add(newObject);
                            entity.SaveChanges();
                            res = tmpInvoice.invoiceId;
                        }
                    }
                    else
                    {
                        newObject.invDate = datenow;
                        newObject.invTime = datenow.TimeOfDay;
                        newObject.updateDate = datenow;
                        newObject.updateUserId = newObject.createUserId;
                        newObject.isActive = true;
                        newObject.isOrginal = true;
                        tmpInvoice = invoiceEntity.Add(newObject);
                        entity.SaveChanges();
                        res = tmpInvoice.invoiceId;
                    }
                    return res;
                }
                else
                {
                    tmpInvoice = entity.invoices
                        .Where(p => p.invoiceId == newObject.invoiceId)
                        .FirstOrDefault();
                    tmpInvoice.invNumber = newObject.invNumber;
                    tmpInvoice.agentId = newObject.agentId;
                    tmpInvoice.invType = newObject.invType;
                    tmpInvoice.total = newObject.total;
                    tmpInvoice.totalNet = newObject.totalNet;
                    tmpInvoice.paid = newObject.paid;
                    tmpInvoice.deserved = newObject.deserved;
                    tmpInvoice.deservedDate = newObject.deservedDate;
                    tmpInvoice.invoiceMainId = newObject.invoiceMainId;
                    tmpInvoice.invCase = newObject.invCase;
                    tmpInvoice.notes = newObject.notes;
                    tmpInvoice.vendorInvNum = newObject.vendorInvNum;
                    tmpInvoice.vendorInvDate = newObject.vendorInvDate;
                    tmpInvoice.updateDate = datenow;
                    tmpInvoice.updateUserId = newObject.updateUserId;
                    tmpInvoice.branchId = newObject.branchId;
                    tmpInvoice.discountType = newObject.discountType;
                    tmpInvoice.discountValue = newObject.discountValue;
                    tmpInvoice.tax = newObject.tax;
                    tmpInvoice.taxtype = newObject.taxtype;
                    tmpInvoice.name = newObject.name;
                    tmpInvoice.isApproved = newObject.isApproved;
                    tmpInvoice.branchCreatorId = newObject.branchCreatorId;
                    tmpInvoice.shippingCompanyId = newObject.shippingCompanyId;
                    tmpInvoice.shipUserId = newObject.shipUserId;
                    tmpInvoice.userId = newObject.userId;
                    tmpInvoice.manualDiscountType = newObject.manualDiscountType;
                    tmpInvoice.manualDiscountValue = newObject.manualDiscountValue;
                    tmpInvoice.cashReturn = newObject.cashReturn;
                    tmpInvoice.shippingCost = newObject.shippingCost;
                    tmpInvoice.realShippingCost = newObject.realShippingCost;
                    tmpInvoice.shippingCostDiscount = newObject.shippingCostDiscount;
                    tmpInvoice.reservationId = newObject.reservationId;
                    tmpInvoice.waiterId = newObject.waiterId;
                    tmpInvoice.orderTime = newObject.orderTime;
                    tmpInvoice.membershipId = newObject.membershipId;
                    tmpInvoice.invBarcode = newObject.invBarcode;

                    if (newObject.invDate != null)
                        tmpInvoice.invDate = newObject.invDate;

                    entity.SaveChanges();
                    res = tmpInvoice.invoiceId;
                    return res;
                }
            }
        }

        [NonAction]
        public async Task<long> saveInvoice(invoices newObject)
        {
            long res = 0;
            invoices tmpInvoice;
            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
            ProgramDetailsController pc = new ProgramDetailsController();
            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoiceEntity = entity.Set<invoices>();
                #region generate InvNumber
                int branchId = (int)newObject.branchCreatorId;
                string invNumber = await generateInvNumber(newObject.invNumber, branchId);
                newObject.invNumber = invNumber;
                #endregion

                if (newObject.invoiceId == 0)
                {
                    if (newObject.cashReturn == null)
                        newObject.cashReturn = 0;
                    newObject.invDate = datenow;
                    newObject.invTime = datenow.TimeOfDay;
                    newObject.updateDate = datenow;
                    newObject.updateUserId = newObject.createUserId;
                    newObject.isActive = true;
                    newObject.isOrginal = true;
                    tmpInvoice = invoiceEntity.Add(newObject);
                    entity.SaveChanges();
                    res = tmpInvoice.invoiceId;

                    return res;
                }
                else
                {
                    tmpInvoice = entity.invoices
                        .Where(p => p.invoiceId == newObject.invoiceId)
                        .FirstOrDefault();
                    tmpInvoice.invNumber = newObject.invNumber;
                    tmpInvoice.agentId = newObject.agentId;
                    tmpInvoice.invType = newObject.invType;
                    tmpInvoice.total = newObject.total;
                    tmpInvoice.totalNet = newObject.totalNet;
                    tmpInvoice.paid = newObject.paid;
                    tmpInvoice.deserved = newObject.deserved;
                    tmpInvoice.deservedDate = newObject.deservedDate;
                    tmpInvoice.invoiceMainId = newObject.invoiceMainId;
                    tmpInvoice.invCase = newObject.invCase;
                    tmpInvoice.notes = newObject.notes;
                    tmpInvoice.vendorInvNum = newObject.vendorInvNum;
                    tmpInvoice.vendorInvDate = newObject.vendorInvDate;
                    tmpInvoice.updateDate = datenow;
                    tmpInvoice.updateUserId = newObject.updateUserId;
                    tmpInvoice.branchId = newObject.branchId;
                    tmpInvoice.discountType = newObject.discountType;
                    tmpInvoice.discountValue = newObject.discountValue;
                    tmpInvoice.tax = newObject.tax;
                    tmpInvoice.taxtype = newObject.taxtype;
                    tmpInvoice.name = newObject.name;
                    tmpInvoice.isApproved = newObject.isApproved;
                    tmpInvoice.branchCreatorId = newObject.branchCreatorId;
                    tmpInvoice.shippingCompanyId = newObject.shippingCompanyId;
                    tmpInvoice.shipUserId = newObject.shipUserId;
                    tmpInvoice.userId = newObject.userId;
                    tmpInvoice.manualDiscountType = newObject.manualDiscountType;
                    tmpInvoice.manualDiscountValue = newObject.manualDiscountValue;
                    tmpInvoice.cashReturn = newObject.cashReturn;
                    tmpInvoice.shippingCost = newObject.shippingCost;
                    tmpInvoice.realShippingCost = newObject.realShippingCost;
                    tmpInvoice.shippingCostDiscount = newObject.shippingCostDiscount;
                    tmpInvoice.reservationId = newObject.reservationId;
                    tmpInvoice.waiterId = newObject.waiterId;
                    tmpInvoice.orderTime = newObject.orderTime;
                    tmpInvoice.membershipId = newObject.membershipId;
                    tmpInvoice.invBarcode = newObject.invBarcode;

                    if (newObject.invDate != null)
                        tmpInvoice.invDate = newObject.invDate;

                    entity.SaveChanges();
                    res = tmpInvoice.invoiceId;
                    return res;
                }
            }
        }

        [HttpPost]
        [Route("SaveWithItems")]
        public async Task<string> SaveWithItems(string token)
        {
            ItemsTransferController tc = new ItemsTransferController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string ObjectStr = "";
                long posId = 0;
                invoices newObject = null;
                List<itemsTransfer> items = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceObject")
                    {
                        ObjectStr = c.Value.Replace("\\", string.Empty);
                        ObjectStr = ObjectStr.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            ObjectStr,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemsObject")
                    {
                        ObjectStr = c.Value.Replace("\\", string.Empty);
                        ObjectStr = ObjectStr.Trim('"');
                        items = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            ObjectStr,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    long invoiceId = await saveInvoice(newObject);
                    if (invoiceId > 0)
                    {
                        string res = tc.saveInvoiceItems(items, invoiceId);
                        message = invoiceId.ToString();
                        if (res == "0")
                            message = "0";
                    }
                    else
                        message = "0";
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";
                result += ",InvTime:'" + newObject.invTime + "'";
                result +=
                    ",UpdateDate:'"
                    + DateTime.Parse(newObject.updateDate.ToString()).ToShortDateString()
                    + "'";
                int draftCount = 0;

                #region get purchase draft count

                List<string> invoiceType = new List<string>() { "pd ", "pbd" };
                if (invoiceType.Contains(newObject.invType))
                    draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 1);

                result += ",PurchaseDraftCount:" + draftCount;
                #endregion

                #region return pos Balance

                #region get spending order draft count
                if (spendingOrderType.Contains(newObject.invType))
                    draftCount = getDraftCount(
                        (int)newObject.updateUserId,
                        new List<string>() { "srd" },
                        1
                    );

                result += ",SpendingOrderDraftCount:" + draftCount;
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var pos = entity.pos.Find(posId);
                    result += ",PosBalance:" + pos.balance;
                }
                #endregion
                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("savePurchaseInvoice")]
        public async Task<string> savePurchaseInvoice(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string invoiceObject = "";
                string amountNotStr = "";
                string waitNotStr = "";
                string Object = "";
                int posId = 0;

                invoices newObject = null;
                NotificationUserModel amountNot = null;
                NotificationUserModel waitNotUser = null;
                notification waitNot = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                cashTransfer PosCashTransfer = null;
                List<cashTransfer> listPayments = new List<cashTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "amountNot")
                    {
                        amountNotStr = c.Value.Replace("\\", string.Empty);
                        amountNotStr = amountNotStr.Trim('"');
                        amountNot = JsonConvert.DeserializeObject<NotificationUserModel>(
                            amountNotStr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "waitNot")
                    {
                        waitNotStr = c.Value.Replace("\\", string.Empty);
                        waitNotStr = waitNotStr.Trim('"');
                        waitNotUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            waitNotStr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        waitNot = JsonConvert.DeserializeObject<notification>(
                            waitNotStr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "PosCashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        PosCashTransfer = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "listPayments")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        listPayments = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "posId")
                    {
                        posId = int.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region check pos balance
                        var pos = entity.pos.Find(posId);
                        foreach (var c in listPayments)
                        {
                            if (c.processType == "cash" && pos.balance < c.cash)
                            {
                                message = "-2";
                                result += "Result:" + message;
                                result += "}";
                                return TokenManager.GenerateToken(result);
                            }
                        }
                        #endregion

                        if (newObject.invoiceMainId == 0)
                            newObject.invoiceMainId = null;

                        long invoiceId = await saveInvoice(newObject);
                        message = newObject.invoiceId.ToString();
                        newObject.invoiceId = invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region enter items to store and notification

                            if (newObject.branchCreatorId.Equals(newObject.branchId))
                            {
                                ItemsLocationsController ilc = new ItemsLocationsController();
                                await ilc.receiptInvoice(
                                    (long)newObject.branchId,
                                    transferObject,
                                    (long)newObject.updateUserId,
                                    amountNot.objectName,
                                    amountNotStr
                                );
                                saveAvgPrice(transferObject);
                            }
                            else
                            {
                                NotificationController nc = new NotificationController();
                                nc.save(
                                    waitNot,
                                    waitNotUser.objectName,
                                    waitNotUser.prefix,
                                    (long)waitNotUser.branchId
                                );
                            }
                            #endregion

                            #region save pos cash transfer
                            CashTransferController cc = new CashTransferController();

                            PosCashTransfer.invId = invoiceId;
                            //PosCashTransfer.transNum = await cc.generateCashNumber(PosCashTransfer.transNum);

                            await cc.addCashTransfer(PosCashTransfer);
                            #endregion

                            #region save payments
                            var inv = entity.invoices.Find(invoiceId);

                            foreach (var item in listPayments)
                            {
                                item.invId = invoiceId;
                                await savePurchaseCash(newObject, item, posId);
                            }

                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";
                result += ",InvTime:'" + newObject.invTime + "'";
                result +=
                    ",UpdateDate:'"
                    + DateTime.Parse(newObject.updateDate.ToString()).ToShortDateString()
                    + "'";
                #region get purchase draft count
                List<string> invoiceType = new List<string>() { "pd ", "pbd" };
                int draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 1);

                result += ",PurchaseDraftCount:" + draftCount;
                #endregion

                #region return pos Balance
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var pos = entity.pos.Find(posId);
                    result += ",PosBalance:" + pos.balance;
                }
                #endregion
                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        private async Task savePurchaseCash(invoices inv, cashTransfer cashTransfer, int posId)
        {
            CashTransferController cc = new CashTransferController();
            //cashTransfer.transNum = await cc.generateCashNumber(cashTransfer.transNum);

            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoice = entity.invoices.Find(inv.invoiceId);
                switch (cashTransfer.processType)
                {
                    case "cash": // cash: update pos balance
                        var pos = entity.pos.Find(posId);
                        if (pos.balance > 0)
                        {
                            if (pos.balance >= cashTransfer.cash)
                            {
                                pos.balance -= cashTransfer.cash;
                                invoice.paid = cashTransfer.cash;
                                invoice.deserved -= cashTransfer.cash;
                            }
                            else
                            {
                                invoice.paid = pos.balance;
                                cashTransfer.cash = pos.balance;
                                invoice.deserved -= pos.balance;
                                pos.balance = 0;
                            }
                            entity.SaveChanges();
                            await cc.addCashTransfer(cashTransfer); //add cash transfer
                        }
                        break;
                    case "balance": // balance: update customer balance
                        await recordConfiguredAgentCash(invoice, "pi", cashTransfer, posId);

                        break;
                    case "card": // card
                        await cc.addCashTransfer(cashTransfer); //add cash transfer
                        invoice.paid += cashTransfer.cash;
                        invoice.deserved -= cashTransfer.cash;
                        entity.SaveChanges();
                        break;
                }
            }
        }

        [HttpPost]
        [Route("savePurchaseBounce")]
        public async Task<string> savePurchaseBounce(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                int posId = 0;
                invoices newObject = null;
                NotificationUserModel notificationUser = null;
                notification notification = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "notification")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        notificationUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        notification = JsonConvert.DeserializeObject<notification>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "posId")
                    {
                        posId = int.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        if (newObject.invoiceMainId == 0)
                            newObject.invoiceMainId = null;

                        long invoiceId = await saveInvoice(newObject);
                        newObject.invoiceId = invoiceId;
                        message = invoiceId.ToString();
                        //int invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region save notification
                            NotificationController nc = new NotificationController();
                            notification.updateUserId = notification.createUserId;

                            nc.save(
                                notification,
                                notificationUser.objectName,
                                notificationUser.prefix,
                                (int)notificationUser.branchId
                            );
                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                #region get sales draft count
                List<string> invoiceType = new List<string>() { "pd ", "pbd" };
                int draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 1);

                result += ",PurchaseDraftCount:" + draftCount;
                #endregion

                #region return pos Balance
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var pos = entity.pos.Find(posId);
                    result += ",PosBalance:" + pos.balance;
                }
                #endregion

                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("SaveImportOrder")]
        public async Task<string> SaveImportOrder(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                bool final = false;
                invoices newObject = null;
                invoices sentExportInvoice = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                NotificationUserModel notUser = new NotificationUserModel();
                notification not = new notification();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "exportInvoice")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        sentExportInvoice = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        notUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        not = JsonConvert.DeserializeObject<notification>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "final")
                        final = bool.Parse(c.Value);
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //save import invoice
                        newObject = await AddImportInvoice(
                            newObject,
                            sentExportInvoice,
                            transferObject,
                            notUser,
                            not,
                            final
                        );

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                #region get sales draft count
                List<string> invoiceType = new List<string>() { "imd ", "exd" };
                int draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 2);
                result += ",ImExpDraftCount:" + draftCount;
                #endregion


                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        private async Task<invoices> AddImportInvoice(
            invoices newObject,
            invoices sentExportInvoice,
            List<itemsTransfer> transferObject,
            NotificationUserModel notUser = null,
            notification not = null,
            bool final = true
        )
        {
            string message = "";
            invoices tmpInvoice;
            invoices exportInvoice = new invoices();
            string exportInvNumber = "";
            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoiceEntity = entity.Set<invoices>();
                #region generate InvNumber

                int branchId = (int)newObject.branchCreatorId;
                string invNumber = await generateInvNumber(newObject.invNumber, branchId);

                #endregion

                #region export invoice

                if (newObject.invoiceId != 0)
                {
                    exportInvoice = entity.invoices
                        .Where(x => x.invoiceMainId == newObject.invoiceId)
                        .FirstOrDefault();
                    exportInvoice.branchId = sentExportInvoice.branchId;
                    exportInvoice.updateUserId = sentExportInvoice.updateUserId;
                    exportInvoice.invType = sentExportInvoice.invType;
                }
                else
                {
                    exportInvoice = sentExportInvoice;
                    exportInvNumber = await generateInvNumber(exportInvoice.invNumber, branchId);
                }
                #endregion

                #region save import invoice
                if (newObject.invoiceMainId == 0)
                    newObject.invoiceMainId = null;
                if (newObject.invoiceId == 0)
                {
                    if (newObject.cashReturn == null)
                        newObject.cashReturn = 0;
                    newObject.invDate = datenow;
                    newObject.invTime = datenow.TimeOfDay;
                    newObject.updateDate = datenow;
                    newObject.updateUserId = newObject.createUserId;
                    newObject.isActive = true;
                    newObject.isOrginal = true;
                    newObject.invNumber = invNumber;

                    tmpInvoice = invoiceEntity.Add(newObject);
                    entity.SaveChanges();

                    message = tmpInvoice.invoiceId.ToString();
                }
                else
                {
                    tmpInvoice = entity.invoices
                        .Where(p => p.invoiceId == newObject.invoiceId)
                        .FirstOrDefault();
                    tmpInvoice.invNumber = invNumber;
                    tmpInvoice.agentId = newObject.agentId;
                    tmpInvoice.invType = newObject.invType;
                    tmpInvoice.total = newObject.total;
                    tmpInvoice.totalNet = newObject.totalNet;
                    tmpInvoice.paid = newObject.paid;
                    tmpInvoice.deserved = newObject.deserved;
                    tmpInvoice.deservedDate = newObject.deservedDate;
                    tmpInvoice.invoiceMainId = newObject.invoiceMainId;
                    tmpInvoice.invCase = newObject.invCase;
                    tmpInvoice.notes = newObject.notes;
                    tmpInvoice.vendorInvNum = newObject.vendorInvNum;
                    tmpInvoice.vendorInvDate = newObject.vendorInvDate;
                    tmpInvoice.updateDate = datenow;
                    tmpInvoice.updateUserId = newObject.updateUserId;
                    tmpInvoice.branchId = newObject.branchId;
                    tmpInvoice.discountType = newObject.discountType;
                    tmpInvoice.discountValue = newObject.discountValue;
                    tmpInvoice.tax = newObject.tax;
                    tmpInvoice.taxtype = newObject.taxtype;
                    tmpInvoice.name = newObject.name;
                    tmpInvoice.isApproved = newObject.isApproved;
                    tmpInvoice.branchCreatorId = newObject.branchCreatorId;
                    tmpInvoice.shippingCompanyId = newObject.shippingCompanyId;
                    tmpInvoice.shipUserId = newObject.shipUserId;
                    tmpInvoice.userId = newObject.userId;
                    tmpInvoice.manualDiscountType = newObject.manualDiscountType;
                    tmpInvoice.manualDiscountValue = newObject.manualDiscountValue;
                    tmpInvoice.cashReturn = newObject.cashReturn;
                    tmpInvoice.shippingCost = newObject.shippingCost;
                    tmpInvoice.realShippingCost = newObject.realShippingCost;
                    entity.SaveChanges();
                    message = tmpInvoice.invoiceId.ToString();
                }
                #endregion
            }
            if (!tmpInvoice.Equals(0))
            {
                using (incposdbEntities entity1 = new incposdbEntities())
                {
                    exportInvoice.invoiceMainId = tmpInvoice.invoiceId;

                    #region save export invoice
                    if (exportInvoice.invoiceId == 0)
                    {
                        if (exportInvoice.cashReturn == null)
                            exportInvoice.cashReturn = 0;
                        exportInvoice.invDate = datenow;
                        exportInvoice.invTime = datenow.TimeOfDay;
                        exportInvoice.updateDate = datenow;
                        exportInvoice.updateUserId = exportInvoice.createUserId;
                        exportInvoice.isActive = true;
                        exportInvoice.isOrginal = true;
                        exportInvoice.invNumber = exportInvNumber;

                        exportInvoice = entity1.invoices.Add(exportInvoice);
                        entity1.SaveChanges();
                    }
                    else
                    {
                        var exportInvoiceTmp = entity1.invoices
                            .Where(p => p.invoiceId == exportInvoice.invoiceId)
                            .FirstOrDefault();
                        // exportInvoice.invNumber = invNumber;
                        exportInvoiceTmp.agentId = exportInvoice.agentId;
                        exportInvoiceTmp.invType = exportInvoice.invType;
                        exportInvoiceTmp.total = exportInvoice.total;
                        exportInvoiceTmp.totalNet = exportInvoice.totalNet;
                        exportInvoiceTmp.paid = exportInvoice.paid;
                        exportInvoiceTmp.deserved = exportInvoice.deserved;
                        exportInvoiceTmp.deservedDate = exportInvoice.deservedDate;
                        exportInvoiceTmp.invoiceMainId = exportInvoice.invoiceMainId;
                        exportInvoiceTmp.invCase = exportInvoice.invCase;
                        exportInvoiceTmp.notes = exportInvoice.notes;
                        exportInvoiceTmp.vendorInvNum = exportInvoice.vendorInvNum;
                        exportInvoiceTmp.vendorInvDate = exportInvoice.vendorInvDate;
                        exportInvoiceTmp.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        exportInvoiceTmp.updateUserId = exportInvoice.updateUserId;
                        exportInvoiceTmp.branchId = exportInvoice.branchId;
                        exportInvoiceTmp.discountType = exportInvoice.discountType;
                        exportInvoiceTmp.discountValue = exportInvoice.discountValue;
                        exportInvoiceTmp.tax = exportInvoice.tax;
                        exportInvoiceTmp.taxtype = exportInvoice.taxtype;
                        exportInvoiceTmp.name = exportInvoice.name;
                        exportInvoiceTmp.isApproved = exportInvoice.isApproved;
                        exportInvoiceTmp.branchCreatorId = exportInvoice.branchCreatorId;
                        exportInvoiceTmp.shippingCompanyId = exportInvoice.shippingCompanyId;
                        exportInvoiceTmp.shipUserId = exportInvoice.shipUserId;
                        exportInvoiceTmp.userId = exportInvoice.userId;
                        exportInvoiceTmp.manualDiscountType = exportInvoice.manualDiscountType;
                        exportInvoiceTmp.manualDiscountValue = exportInvoice.manualDiscountValue;
                        exportInvoiceTmp.cashReturn = exportInvoice.cashReturn;
                        exportInvoiceTmp.shippingCost = exportInvoice.shippingCost;
                        exportInvoiceTmp.realShippingCost = exportInvoice.realShippingCost;
                        entity1.SaveChanges();
                    }
                    #endregion
                }
                //save items transfer to import invoice
                ItemsTransferController it = new ItemsTransferController();
                it.saveImExItems(transferObject, tmpInvoice.invoiceId, exportInvoice.invoiceId);

                //send notification
                if (not != null && final == true)
                {
                    NotificationController nc = new NotificationController();
                    nc.save(not, notUser.objectName, notUser.prefix, (int)notUser.branchId);
                }
            }

            return tmpInvoice;
        }

        [HttpPost]
        [Route("GenerateExport")]
        public async Task<string> GenerateExport(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notObject = "";
                long branchId = 0;
                long toBranch = 0;
                long userId = 0;
                bool final = false;
                invoices newObject = null;
                invoices sentExportInvoice = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                List<itemsLocations> itemsLoc = new List<itemsLocations>();
                NotificationUserModel notUser = new NotificationUserModel();
                notification not = new notification();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "exportInvoice")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        sentExportInvoice = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notObject = c.Value.Replace("\\", string.Empty);
                        notObject = notObject.Trim('"');
                        notUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        not = JsonConvert.DeserializeObject<notification>(
                            notObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "ItemsLoc")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemsLoc = JsonConvert.DeserializeObject<List<itemsLocations>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "final")
                    {
                        final = bool.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    #region check items quantity in store
                    if (final == true)
                    {
                        ItemsLocationsController itc = new ItemsLocationsController();
                        toBranch = (long)notUser.branchId;

                        string res = itc.checkItemsAmounts(billDetails, branchId, 0);

                        if (!res.Equals(""))
                        {
                            message = "-3";
                            result += "Result:" + message;

                            res = System.Web.Helpers.Json
                                .Encode(res)
                                .Substring(1, System.Web.Helpers.Json.Encode(res).Length - 2);
                            result += ",Message:'" + res + "'";
                            result += "}";

                            return TokenManager.GenerateToken(result);
                        }
                    }
                    #endregion

                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //save import invoice
                        newObject = await AddImportInvoice(
                            newObject,
                            sentExportInvoice,
                            transferObject
                        );

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;

                        //reciept invoice
                        if (final == true)
                        {
                            ItemsLocationsController ilc = new ItemsLocationsController();
                            ilc.receiptOrder(
                                itemsLoc,
                                transferObject,
                                toBranch,
                                userId,
                                notUser.objectName,
                                notObject
                            );
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                #region get sales draft count
                List<string> invoiceType = new List<string>() { "imd ", "exd" };
                int draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 2);
                result += ",ImExpDraftCount:" + draftCount;
                #endregion


                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("AcceptWaitingImport")]
        public async Task<string> AcceptWaitingImport(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notObject = "";
                long branchId = 0;
                long toBranch = 0;
                long userId = 0;
                invoices newObject = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                List<itemsLocations> itemsLoc = new List<itemsLocations>();
                NotificationUserModel notUser = new NotificationUserModel();
                notification not = new notification();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "ItemsLoc")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemsLoc = JsonConvert.DeserializeObject<List<itemsLocations>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notObject = c.Value.Replace("\\", string.Empty);
                        notObject = notObject.Trim('"');
                        notUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region check items quantity in store
                        ItemsLocationsController itc = new ItemsLocationsController();
                        toBranch = (long)notUser.branchId;

                        string res = itc.checkItemsAmounts(billDetails, branchId, 0);

                        if (!res.Equals(""))
                        {
                            message = "-3";
                            result += "Result:" + message;

                            res = System.Web.Helpers.Json
                                .Encode(res)
                                .Substring(1, System.Web.Helpers.Json.Encode(res).Length - 2);
                            result += ",Message:'" + res + "'";
                            result += "}";

                            return TokenManager.GenerateToken(result);
                        }
                        #endregion

                        //edit export invoice
                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();

                        long invoiceId = newObject.invoiceId;

                        if (!invoiceId.Equals(0))
                        {
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveImExItems(
                                transferObject,
                                newObject.invoiceId,
                                (int)newObject.invoiceMainId
                            );

                            //reciept invoice
                            ItemsLocationsController ilc = new ItemsLocationsController();
                            ilc.receiptOrder(
                                itemsLoc,
                                transferObject,
                                toBranch,
                                userId,
                                notUser.objectName,
                                notObject
                            );
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("AcceptSpendingOrder")]
        public async Task<string> AcceptSpendingOrder(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notObject = "";
                long branchId = 0;
                long toBranch = 0;
                long userId = 0;
                invoices newObject = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                List<itemsLocations> itemsLoc = new List<itemsLocations>();
                NotificationUserModel notUser = new NotificationUserModel();
                notification not = new notification();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "ItemsLoc")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemsLoc = JsonConvert.DeserializeObject<List<itemsLocations>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notObject = c.Value.Replace("\\", string.Empty);
                        notObject = notObject.Trim('"');
                        notUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        not = JsonConvert.DeserializeObject<notification>(
                            notObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region check items quantity in store
                        ItemsLocationsController itc = new ItemsLocationsController();
                        toBranch = (long)notUser.branchId;

                        string res = itc.checkItemsAmounts(billDetails, branchId, 0);

                        if (!res.Equals(""))
                        {
                            message = "-3";
                            result += "Result:" + message;

                            res = System.Web.Helpers.Json
                                .Encode(res)
                                .Substring(1, System.Web.Helpers.Json.Encode(res).Length - 2);
                            result += ",Message:'" + res + "'";
                            result += "}";

                            return TokenManager.GenerateToken(result);
                        }
                        #endregion

                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();

                        long invoiceId = newObject.invoiceId;

                        if (!invoiceId.Equals(0))
                        {
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, newObject.invoiceId);

                            //transfer items to kitchen
                            ItemsLocationsController ilc = new ItemsLocationsController();
                            ilc.transferToKitchen(itemsLoc, toBranch, userId);

                            //send notification to reciept branch
                            NotificationController nc = new NotificationController();
                            nc.save(
                                not,
                                notUser.objectName,
                                notUser.prefix,
                                (long)notUser.branchId
                            );
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                #region get spending orders draft count
                List<string> invoiceType = new List<string> { "srd" };
                int count = getDraftCount(userId, invoiceType, 2);
                result += ",SpendingOrderDraftCount:" + count;
                #endregion
                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("SaveConsumptionOrder")]
        public async Task<string> SaveConsumptionOrder(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                long branchId = 0;
                long userId = 0;
                invoices newObject = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();

                notification not = new notification();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        userId = (long)newObject.updateUserId;
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                try
                {
                    branchId = (long)newObject.branchId;

                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region check items quantity in store
                        ItemsLocationsController itc = new ItemsLocationsController();

                        string res = itc.checkItemsAmounts(billDetails, branchId, 1);

                        if (!res.Equals(""))
                        {
                            message = "-3";
                            result += "Result:" + message;

                            res = System.Web.Helpers.Json
                                .Encode(res)
                                .Substring(1, System.Web.Helpers.Json.Encode(res).Length - 2);
                            result += ",Message:'" + res + "'";
                            result += "}";

                            return TokenManager.GenerateToken(result);
                        }
                        #endregion

                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();

                        long invoiceId = newObject.invoiceId;

                        if (!invoiceId.Equals(0))
                        {
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, newObject.invoiceId);

                            //transfer items to kitchen

                            ItemsLocationsController ilc = new ItemsLocationsController();
                            ilc.decreaseAmountsInKitchen(transferObject, branchId, userId);
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";

                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        private int getDraftCount(long createUserId, List<string> invoiceType, int duration)
        {
            int draftCount = GetCountByCreator(invoiceType, createUserId, duration);
            return draftCount;
        }

        public async Task<string> generateInvNumber(string invoiceCode, int branchId)
        {
            #region check if last of code is num
            var num = invoiceCode.Substring(invoiceCode.LastIndexOf("-") + 1);

            if (!num.Equals(invoiceCode))
                return invoiceCode;

            #endregion
            int sequence = 0;
            string branchCode = "";

            using (incposdbEntities entity = new incposdbEntities())
            {
                var branch = entity.branches.Find(branchId);

                branchCode = branch.code;

                var numberList = entity.invoices
                    .Where(
                        b =>
                            b.invNumber.Contains(invoiceCode + "-") && b.branchCreatorId == branchId
                    )
                    .Select(b => b.invNumber)
                    .ToList();
                for (int i = 0; i < numberList.Count; i++)
                {
                    string code = numberList[i];
                    string s = code.Substring(code.LastIndexOf("-") + 1);

                    numberList[i] = s;
                }
                if (numberList.Count > 0)
                {
                    numberList.Sort();
                    try
                    {
                        sequence = int.Parse(numberList[numberList.Count - 1]);
                    }
                    catch
                    {
                        sequence = 0;
                    }
                }
            }
            sequence++;

            string strSeq = sequence.ToString();
            if (sequence <= 999999)
                strSeq = sequence.ToString().PadLeft(6, '0');
            string invoiceNum = invoiceCode + "-" + branchCode + "-" + strSeq;
            return invoiceNum;
        }

        [HttpPost]
        [Route("saveSalesInvoice")]
        public async Task<string> saveSalesInvoice(string token)
        {
            ItemsTransferController tc = new ItemsTransferController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string invoiceObject = "";
                string itemsObject = "";
                string Object = "";
                long posId = 0;
                invoices newObject = null;
                List<itemsTransfer> items = null;
                List<ItemTransferModel> itemsModel = null;
                cashTransfer PosCashTransfer = null;
                List<cashTransfer> listPayments = new List<cashTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemsObject")
                    {
                        itemsObject = c.Value.Replace("\\", string.Empty);
                        itemsObject = itemsObject.Trim('"');
                        items = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            itemsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        itemsModel = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            itemsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "PosCashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        PosCashTransfer = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "listPayments")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        listPayments = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "posId")
                        posId = long.Parse(c.Value);
                }
                #endregion
                try
                {
                    long invoiceId = await saveSalesInvoice(newObject);
                    if (invoiceId > 0)
                    {
                        //save items transfer
                        string res = tc.saveSalesInvoiceItems(items, itemsModel, invoiceId);

                        #region save pos cash transfer
                        if (PosCashTransfer != null)
                        {
                            CashTransferController cc = new CashTransferController();

                            PosCashTransfer.invId = invoiceId;
                            //PosCashTransfer.transNum = await cc.generateCashNumber(PosCashTransfer.transNum);

                            await cc.addCashTransfer(PosCashTransfer);
                        }
                        #endregion

                        #region save payments
                        if (listPayments != null)
                        {
                            if (
                                newObject.shippingCompanyId == null
                                || (
                                    newObject.shippingCompanyId != null
                                    && newObject.shipUserId == null
                                )
                            )
                                await savePayments(invoiceId, listPayments, posId);
                        }
                        #endregion
                        message = invoiceId.ToString();
                        if (res == "0")
                            message = "0";
                    }
                    else
                        message = "0";
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        private async Task savePayments(long invoiceId, List<cashTransfer> listPayments, long posId)
        {
            using (var entity = new incposdbEntities())
            {
                var inv = entity.invoices.Find(invoiceId);

                foreach (var item in listPayments)
                {
                    await ConfiguredCashTrans(inv, item, posId);
                    // yasin code
                    if (item.processType != "balance")
                    {
                        inv.paid += item.cash;
                        inv.deserved -= item.cash;
                    }
                }
                entity.SaveChanges();
            }
        }

        private async Task<cashTransfer> ConfiguredCashTrans(
            invoices invoice,
            cashTransfer cashTransfer,
            long posId
        )
        {
            CashTransferController cc = new CashTransferController();
            cashTransfer.createUserId = invoice.updateUserId;
            switch (cashTransfer.processType)
            {
                case "cash": // cash: update pos balance
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var pos = entity.pos.Find(posId);
                        pos.balance += invoice.totalNet;
                        entity.SaveChanges();
                    }

                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = "dc";
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = invoice.updateUserId;
                    await cc.addCashTransfer(cashTransfer);
                    break;
                case "balance": // balance: update customer balance

                    //if (invoice.shippingCompanyId != null)
                    //{
                    //    cashTransfer = await recordComSpecificPaidCash(invoice, cashTransfer, posId);
                    //    if (cashTransfer.cash > 0)
                    //    {
                    //        cc.addCashTransfer(cashTransfer); //add cash transfer
                    //    }
                    //}
                    //else
                    {
                        await recordConfiguredAgentCash(invoice, "si", cashTransfer, posId);
                    }
                    break;
                case "card": // card
                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = "dc";
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = invoice.updateUserId;
                    await cc.addCashTransfer(cashTransfer); //add cash transfer

                    break;
                case "admin": // admin
                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = "dc";
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = invoice.updateUserId;
                    await cc.addCashTransfer(cashTransfer); //add cash transfer
                    break;
            }

            return cashTransfer;
        }

        public async Task<invoices> recordConfiguredAgentCash(
            invoices invoice,
            string invType,
            cashTransfer cashTransfer,
            long posId
        )
        {
            CashTransferController cc = new CashTransferController();
            decimal newBalance = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var agent = entity.agents.Find(invoice.agentId);
                var inv = entity.invoices.Find(invoice.invoiceId);

                #region agent Cash transfer
                cashTransfer.posId = posId;
                cashTransfer.agentId = invoice.agentId;
                cashTransfer.invId = invoice.invoiceId;
                cashTransfer.createUserId = invoice.createUserId;
                #endregion
                switch (invType)
                {
                    #region purchase
                    case "pi": //purchase invoice
                    case "sb": //sale bounce
                        cashTransfer.transType = "p";
                        if (invType.Equals("pi"))
                        {
                            cashTransfer.side = "v"; // vendor
                            cashTransfer.transNum = "pv";
                        }
                        else
                        {
                            cashTransfer.side = "c"; // customer
                            cashTransfer.transNum = "pc";
                        }
                        if (agent.balanceType == 1)
                        {
                            if (cashTransfer.cash <= (decimal)agent.balance)
                            {
                                newBalance = (decimal)agent.balance - (decimal)cashTransfer.cash;
                                agent.balance = newBalance;

                                // yasin code
                                inv.paid += cashTransfer.cash;
                                inv.deserved -= cashTransfer.cash;
                                ////
                                entity.SaveChanges();
                                ///
                            }
                            else
                            {
                                // yasin code
                                inv.paid += (decimal)agent.balance;
                                inv.deserved -= (decimal)agent.balance;
                                //////
                                ///
                                newBalance = (decimal)cashTransfer.cash - (decimal)agent.balance;
                                agent.balance = newBalance;
                                agent.balanceType = 0;
                                entity.SaveChanges();
                            }
                            cashTransfer.transType = "p"; //pull

                            if (cashTransfer.processType != "balance")
                                await cc.addCashTransfer(cashTransfer); //add agent cash transfer
                        }
                        else if (agent.balanceType == 0)
                        {
                            newBalance = (decimal)agent.balance + (decimal)cashTransfer.cash;
                            agent.balance = newBalance;
                            entity.SaveChanges();
                        }

                        break;
                    #endregion
                    #region purchase bounce
                    case "pb": //purchase bounce invoice
                    case "si": //sale invoice
                        cashTransfer.transType = "d";

                        if (invType.Equals("pb"))
                        {
                            cashTransfer.side = "v"; // vendor
                            cashTransfer.transNum = "dv";
                        }
                        else
                        {
                            cashTransfer.side = "c"; // customer
                            cashTransfer.transNum = "dc";
                        }
                        if (agent.balanceType == 0)
                        {
                            if (cashTransfer.cash <= (decimal)agent.balance)
                            {
                                // yasin code
                                inv.paid += cashTransfer.cash;
                                inv.deserved -= cashTransfer.cash;

                                newBalance = (decimal)agent.balance - (decimal)cashTransfer.cash;
                                agent.balance = newBalance;

                                entity.SaveChanges();
                            }
                            else
                            {
                                inv.paid += (decimal)agent.balance;
                                inv.deserved -= (decimal)agent.balance;

                                //////
                                newBalance = (decimal)cashTransfer.cash - (decimal)agent.balance;
                                agent.balance = newBalance;
                                agent.balanceType = 1;
                                entity.SaveChanges();
                            }
                            cashTransfer.transType = "d"; //deposit

                            if (cashTransfer.cash > 0 && cashTransfer.processType != "balance")
                            {
                                await cc.addCashTransfer(cashTransfer); //add cash transfer
                            }
                        }
                        else if (agent.balanceType == 1)
                        {
                            newBalance = (decimal)agent.balance + (decimal)cashTransfer.cash;
                            agent.balance = newBalance;
                            entity.SaveChanges();
                        }

                        break;
                        #endregion
                }
            }

            return invoice;
        }

        [HttpPost]
        [Route("returnPurInvoice")]
        public async Task<string> returnPurInvoice(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string invoiceObject = "";
                string amountNotStr = "";
                string Object = "";
                long branchId = 0;
                long posId = 0;

                invoices newObject = null;
                invoiceStatus invoiceStatus = null;
                NotificationUserModel amountNot = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                List<itemsLocations> readyItemsLoc = new List<itemsLocations>();
                cashTransfer PosCashTransfer = null;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "invoiceStatus")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        invoiceStatus = JsonConvert.DeserializeObject<invoiceStatus>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "amountNot")
                    {
                        amountNotStr = c.Value.Replace("\\", string.Empty);
                        amountNotStr = amountNotStr.Trim('"');
                        amountNot = JsonConvert.DeserializeObject<NotificationUserModel>(
                            amountNotStr,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "PosCashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        PosCashTransfer = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "readyItemsLoc")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        readyItemsLoc = JsonConvert.DeserializeObject<List<itemsLocations>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        if (newObject.invoiceMainId == 0)
                            newObject.invoiceMainId = null;

                        #region check items quantity in store
                        ItemsLocationsController itc = new ItemsLocationsController();
                        string res = itc.checkItemsAmounts(billDetails, branchId, 0);

                        if (!res.Equals(""))
                        {
                            message = "-3";
                            result += "Result:" + message;

                            res = System.Web.Helpers.Json
                                .Encode(res)
                                .Substring(1, System.Web.Helpers.Json.Encode(res).Length - 2);
                            result += ",Message:'" + res + "'";
                            result += "}";

                            return TokenManager.GenerateToken(result);
                        }
                        #endregion

                        newObject.invoiceId = await saveInvoice(newObject);
                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region decrease amount
                            foreach (var itemLoc in readyItemsLoc)
                            {
                                long itemLocId = itemLoc.itemsLocId;
                                int quantity = (int)itemLoc.quantity;
                                itc.decreaseItemLocationQuantity(
                                    itemLocId,
                                    quantity,
                                    (int)newObject.updateUserId,
                                    amountNot.objectName,
                                    amountNotStr
                                );
                            }
                            #endregion

                            #region save pos cash transfer
                            CashTransferController cc = new CashTransferController();

                            PosCashTransfer.invId = invoiceId;
                            //PosCashTransfer.transNum = await cc.generateCashNumber(PosCashTransfer.transNum);

                            await cc.addCashTransfer(PosCashTransfer);
                            #endregion

                            #region save payments
                            if (newObject.agentId != null)
                            {
                                cashTransfer cashTrasnfer = new cashTransfer();
                                cashTrasnfer.cash = newObject.totalNet;
                                cashTrasnfer.processType = "balance";
                                await recordConfiguredAgentCash(
                                    newObject,
                                    "pb",
                                    cashTrasnfer,
                                    posId
                                );
                            }
                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";
                result += ",InvTime:'" + newObject.invTime + "'";
                result +=
                    ",UpdateDate:'"
                    + DateTime.Parse(newObject.updateDate.ToString()).ToShortDateString()
                    + "'";
                #region get sales draft count
                result += ",PurchaseDraftCount:";

                List<string> invoiceType = new List<string>() { "isd" };
                int draftCount = getDraftCount((int)newObject.updateUserId, invoiceType, 2);

                result += draftCount;
                #endregion
                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("saveInvoiceWithItemsAndTables")]
        public async Task<string> saveInvoiceWithItemsAndTables(string token)
        {
            ItemsTransferController tc = new ItemsTransferController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invoiceObject = "";
                string itemsObject = "";
                string tablesObject = "";
                invoices newObject = null;
                List<itemsTransfer> items = null;
                List<tables> tables = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemsObject")
                    {
                        itemsObject = c.Value.Replace("\\", string.Empty);
                        itemsObject = itemsObject.Trim('"');
                        items = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            itemsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "tablesObject")
                    {
                        tablesObject = c.Value.Replace("\\", string.Empty);
                        tablesObject = tablesObject.Trim('"');
                        tables = JsonConvert.DeserializeObject<List<tables>>(
                            tablesObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    long invoiceId = await saveInvoice(newObject);
                    if (invoiceId > 0)
                    {
                        string res = tc.saveInvoiceItems(items, invoiceId);
                        if (res == "0")
                            message = "0";
                        else
                        {
                            res = saveInvoiceTables(tables, invoiceId, (int)newObject.updateUserId);
                            if (res == "0")
                                message = "0";
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("saveInvoiceWithTables")]
        public async Task<string> saveInvoiceWithTables(string token)
        {
            ItemsTransferController tc = new ItemsTransferController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string invoiceObject = "";
                string tablesObject = "";
                invoices newObject = null;
                List<tables> tables = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "tablesObject")
                    {
                        tablesObject = c.Value.Replace("\\", string.Empty);
                        tablesObject = tablesObject.Trim('"');
                        tables = JsonConvert.DeserializeObject<List<tables>>(
                            tablesObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    long branchId = (long)newObject.branchCreatorId;

                    if (newObject.invNumber == null || newObject.invNumber.Equals(""))
                    {
                        newObject.invNumber = await GetLastDialyNumOfInv(branchId);
                    }

                    long invoiceId = await saveInvoice(newObject);
                    message = invoiceId.ToString();
                    if (invoiceId > 0)
                    {
                        string res = saveInvoiceTables(
                            tables,
                            invoiceId,
                            (int)newObject.updateUserId
                        );
                        if (res == "0")
                            message = "0";
                    }
                }
                catch
                {
                    message = "0";
                }
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("updateInvoiceTables")]
        public string updateInvoiceTables(string token)
        {
            ItemsTransferController tc = new ItemsTransferController();
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invoiceId = 0;
                long? reservationId = null;
                long userId = 0;
                string tablesObject = "";
                List<tables> tables = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "reservationId")
                    {
                        try
                        {
                            reservationId = long.Parse(c.Value);
                        }
                        catch
                        {
                            reservationId = null;
                        }
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "tablesObject")
                    {
                        tablesObject = c.Value.Replace("\\", string.Empty);
                        tablesObject = tablesObject.Trim('"');
                        tables = JsonConvert.DeserializeObject<List<tables>>(
                            tablesObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    reservations reservation = new reservations();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invTables = entity.invoiceTables
                            .Where(x => x.invoiceId == invoiceId)
                            .ToList();
                        entity.invoiceTables.RemoveRange(invTables);
                        entity.SaveChanges();

                        if (reservationId != null)
                        {
                            reservation = entity.reservations.Find(reservationId);
                            var resTables = entity.tablesReservations
                                .Where(x => x.reservationId == reservationId)
                                .ToList();
                            entity.tablesReservations.RemoveRange(resTables);
                            entity.SaveChanges();
                        }
                    }

                    message = saveInvoiceTables(tables, invoiceId, userId);
                    if (reservationId != null)
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (tables tbl in tables)
                            {
                                tablesReservations tableR = new tablesReservations();
                                tableR.tableId = tbl.tableId;
                                tableR.reservationId = (long)reservationId;
                                tableR.createUserId = userId;
                                tableR.updateUserId = userId;
                                tableR.createDate = tableR.updateDate = coctrlr.AddOffsetTodate(
                                    DateTime.Now
                                );
                                tableR.isActive = 1;

                                entity.tablesReservations.Add(tableR);
                            }
                            entity.SaveChanges();
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                return TokenManager.GenerateToken(message);
            }
        }

        public string saveInvoiceTables(List<tables> newObject, long invoiceId, long userId)
        {
            string message = "";
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<invoiceTables> iol = entity.invoiceTables
                        .Where(x => x.invoiceId == invoiceId)
                        .ToList();
                    entity.invoiceTables.RemoveRange(iol);
                    entity.SaveChanges();

                    var invoice = entity.invoices.Find(invoiceId);
                    for (int i = 0; i < newObject.Count; i++)
                    {
                        invoiceTables tr = new invoiceTables();
                        if (newObject[i].createUserId == 0 || newObject[i].createUserId == null)
                        {
                            Nullable<long> id = null;
                            newObject[i].createUserId = id;
                        }

                        var tableEntity = entity.Set<tablesReservations>();

                        tr.invoiceId = invoiceId;
                        tr.tableId = newObject[i].tableId;
                        tr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        tr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        tr.isActive = 1;
                        tr.updateUserId = userId;
                        tr.createUserId = userId;

                        tr = entity.invoiceTables.Add(tr);
                        entity.SaveChanges();
                    }
                    entity.SaveChanges();
                    message = "1";
                }
            }
            catch
            {
                message = "0";
            }
            return message;
        }

        [HttpPost]
        [Route("updateprintstat")]
        public string updateprintstat(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long id = 0;
                int countstep = 0;
                bool isOrginal = false;
                bool updateOrginalstate = false;

                string invoiceObject = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "id")
                    {
                        id = long.Parse(c.Value);
                    }
                    else if (c.Type == "countstep")
                    {
                        countstep = int.Parse(c.Value);
                    }
                    else if (c.Type == "isOrginal")
                    {
                        isOrginal = bool.Parse(c.Value);
                    }
                    else if (c.Type == "updateOrginalstate")
                    {
                        updateOrginalstate = bool.Parse(c.Value);
                    }
                }

                try
                {
                    invoices tmpInvoice;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // var invoiceEntity = entity.Set<invoices>();
                        if (id == 0)
                        {
                            return TokenManager.GenerateToken("0");
                        }
                        else
                        {
                            tmpInvoice = entity.invoices
                                .Where(p => p.invoiceId == id)
                                .FirstOrDefault();
                            int res = tmpInvoice.printedcount + countstep;
                            if (res < 0)
                            {
                                res = 0;
                            }
                            tmpInvoice.printedcount = res;
                            if (updateOrginalstate)
                            {
                                tmpInvoice.isOrginal = isOrginal;
                            }

                            entity.SaveChanges();
                            message = tmpInvoice.invoiceId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "0";
                    // return TokenManager.GenerateToken(message);
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        [HttpPost]
        [Route("delete")]
        public string delete(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                try
                {
                    long invoiceId = 0;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "itemId")
                        {
                            invoiceId = long.Parse(c.Value);
                        }
                    }

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var inv = entity.invoices.Find(invoiceId);
                        inv.isActive = false;
                        message = entity.SaveChanges().ToString();
                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [HttpPost]
        [Route("deleteOrder")]
        public string deleteOrder(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            ItemsLocationsController ilc = new ItemsLocationsController();
            string message = "";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                try
                {
                    long invoiceId = 0;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "itemId")
                        {
                            invoiceId = long.Parse(c.Value);
                        }
                    }
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // desactive invoice
                        var inv = entity.invoices.Find(invoiceId);
                        inv.isActive = false;
                        entity.SaveChanges();

                        // unlockItems
                        var itemsLocations = entity.itemsLocations
                            .Where(x => x.invoiceId == invoiceId)
                            .ToList();
                        foreach (itemsLocations il in itemsLocations)
                        {
                            var itemLoc = (
                                from b in entity.itemsLocations
                                where
                                    b.invoiceId == null
                                    && b.itemUnitId == il.itemUnitId
                                    && b.locationId == il.locationId
                                    && b.startDate == il.startDate
                                    && b.endDate == il.endDate
                                select new ItemLocationModel { itemsLocId = b.itemsLocId, }
                            ).FirstOrDefault();
                            var orderItem = entity.itemsLocations.Find(il.itemsLocId);
                            if (orderItem.quantity == il.quantity)
                                entity.itemsLocations.Remove(orderItem);
                            else
                                orderItem.quantity -= il.quantity;

                            if (itemLoc == null)
                            {
                                var loc = new itemsLocations()
                                {
                                    locationId = il.locationId,
                                    quantity = il.quantity,
                                    createDate = datenow,
                                    updateDate = datenow,
                                    createUserId = il.createUserId,
                                    updateUserId = il.createUserId,
                                    startDate = il.startDate,
                                    endDate = il.endDate,
                                    itemUnitId = il.itemUnitId,
                                    notes = il.notes,
                                };
                                entity.itemsLocations.Add(loc);
                            }
                            else
                            {
                                var loc = entity.itemsLocations.Find(itemLoc.itemsLocId);
                                loc.quantity += il.quantity;
                                loc.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                loc.updateUserId = il.updateUserId;
                            }
                            entity.SaveChanges();
                        }
                        message = "1";
                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [HttpPost]
        [Route("savePurchaseDraft")]
        public async Task<string> savePurchaseDraft(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                long posId = 0;
                invoices newObject = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        if (newObject.invoiceMainId == 0)
                            newObject.invoiceMainId = null;

                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";
                result += ",InvTime:'" + newObject.invTime + "'";
                result +=
                    ",UpdateDate:'"
                    + DateTime.Parse(newObject.updateDate.ToString()).ToString()
                    + "'";

                #region return pos Balance
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var pos = entity.pos.Find(posId);
                    result += ",PosBalance:" + pos.balance;
                }
                #endregion

                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("saveOrderPayments")]
        public async Task<string> saveOrderPayments(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
            string result = "{";
            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string invoiceObject = "";
                string Object = "";

                long branchId = 0;
                long posId = 0;

                invoices newObject = null;
                orderPreparingStatus invoiceStatus = null;
                List<cashTransfer> listPayments = new List<cashTransfer>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        invoiceObject = c.Value.Replace("\\", string.Empty);
                        invoiceObject = invoiceObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            invoiceObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "invoiceStatus")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        invoiceStatus = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "listPayments")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        listPayments = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = int.Parse(c.Value);
                    }
                    else if (c.Type == "posId")
                    {
                        posId = int.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        long invoiceId = newObject.invoiceId;
                        message = invoiceId.ToString();

                        #region update order prepairing status
                        var orderPpList = entity.orderPreparing
                            .Where(x => x.invoiceId == invoiceId)
                            .ToList();
                        foreach (var order in orderPpList)
                        {
                            long orderPreparingId = order.orderPreparingId;
                            OrderPreparingController isc = new OrderPreparingController();
                            isc.saveInvoiceStatus(invoiceStatus, orderPreparingId);
                        }
                        #endregion

                        #region save payments

                        decimal paid = 0;
                        decimal deserved = 0;
                        foreach (var item in listPayments)
                        {
                            await OrderPaymentCashTrans(newObject, item, posId);

                            if (item.processType != "balance")
                            {
                                paid += item.cash;
                                deserved += item.cash;
                            }
                        }
                        var inv = entity.invoices.Find(invoiceId);
                        inv.paid += paid;
                        inv.deserved -= deserved;
                        entity.SaveChanges();

                        #endregion
                    }
                }
                catch
                {
                    message = "0";
                }
                result += "Result:" + message;
                string temp = System.Web.Helpers.Json
                    .Encode(newObject.invNumber)
                    .Substring(1, System.Web.Helpers.Json.Encode(newObject.invNumber).Length - 2);
                result += ",Message:'" + temp + "'";
                result += ",InvTime:'" + newObject.invTime + "'";
                result +=
                    ",UpdateDate:'"
                    + DateTime.Parse(newObject.updateDate.ToString()).ToShortDateString()
                    + "'";

                #region return pos Balance
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var pos = entity.pos.Find(posId);
                    result += ",PosBalance:" + pos.balance;
                }
                #endregion
                result += "}";
                return TokenManager.GenerateToken(result);
            }
        }

        [HttpPost]
        [Route("recordCompanyCashTransfer")]
        public async Task<string> recordCompanyCashTransfer(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";

            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                long invoiceId = 0;

                cashTransfer cashTransfer = new cashTransfer();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTransfer = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        CashTransferController cc = new CashTransferController();
                        //cashTransfer.transNum = await cc.generateCashNumber(cashTransfer.transNum);

                        var invoice = entity.invoices.Find(invoiceId);
                        long companyId = (long)invoice.shippingCompanyId;
                        var company = entity.shippingCompanies.Find(companyId);

                        decimal newBalance = 0;
                        if (company.balanceType == 0)
                        {
                            if (invoice.totalNet <= (decimal)company.balance)
                            {
                                invoice.paid = invoice.totalNet;
                                invoice.deserved = 0;
                                newBalance = (decimal)company.balance - (decimal)invoice.totalNet;
                                company.balance = newBalance;
                            }
                            else
                            {
                                invoice.paid = (decimal)company.balance;
                                invoice.deserved = invoice.totalNet - (decimal)company.balance;
                                newBalance = (decimal)invoice.totalNet - company.balance;
                                company.balance = newBalance;
                                company.balanceType = 1;
                            }

                            cashTransfer.cash = invoice.paid;
                            cashTransfer.transType = "d"; //deposit
                            if (invoice.paid > 0)
                            {
                                await cc.addCashTransfer(cashTransfer);
                            }
                        }
                        else if (company.balanceType == 1)
                        {
                            newBalance = (decimal)company.balance + (decimal)invoice.totalNet;
                            company.balance = newBalance;
                        }
                        entity.SaveChanges();
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("recordConfiguredAgentCash")]
        public async Task<string> recordConfiguredAgentCash(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "1";

            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string invType = "";
                long invoiceId = 0;
                long posId = 0;

                cashTransfer cashTransfer = new cashTransfer();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTransfer = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                    else if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invoice = entity.invoices.Find(invoiceId);
                        recordConfiguredAgentCash(invoice, invType, cashTransfer, posId);
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        private async Task<cashTransfer> OrderPaymentCashTrans(
            invoices invoice,
            cashTransfer cashTransfer,
            long posId
        )
        {
            CashTransferController cc = new CashTransferController();
            cashTransfer.createUserId = invoice.updateUserId;
            switch (cashTransfer.processType)
            {
                case "cash": // cash: update pos balance
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var pos = entity.pos.Find(posId);
                        //if (pos.balance == null)
                        //  pos.balance = 0;
                        pos.balance += invoice.totalNet;
                        entity.SaveChanges();
                    }

                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = "dc";
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = invoice.updateUserId;
                    await cc.addCashTransfer(cashTransfer);
                    break;
                case "balance": // balance: update customer balance

                    await recordConfiguredAgentCash(invoice, "si", cashTransfer, posId);

                    break;
                case "card": // card
                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = "dc";
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = invoice.updateUserId;
                    await cc.addCashTransfer(cashTransfer); //add cash transfer

                    break;
            }

            return cashTransfer;
        }

        [HttpPost]
        [Route("saveAvgPrice")]
        public string saveAvgPrice(string token)
        {
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                List<itemsTransfer> newObject = new List<itemsTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                try
                {
                    saveAvgPrice(newObject);
                    message = "1";
                    return TokenManager.GenerateToken(message);
                    //using (incposdbEntities entity = new incposdbEntities())
                    //{
                    //    var set = entity.setting.Where(x => x.name == "Pur_inv_avg_count").FirstOrDefault();
                    //    string invoiceNum = "0";
                    //    if (set != null)
                    //        invoiceNum = entity.setValues.Where(x => x.settingId == (int)set.settingId).Select(x => x.value).Single();
                    //    foreach (itemsTransfer item in newObject)
                    //    {
                    //        var itemId = entity.itemsUnits.Where(x => x.itemUnitId == (int)item.itemUnitId).Select(x => x.itemId).Single();

                    //        decimal price = GetAvgPrice((int)item.itemUnitId, (int)itemId, int.Parse(invoiceNum));

                    //        var itemO = entity.items.Find(itemId);
                    //        itemO.avgPurchasePrice = price;
                    //    }
                    //    entity.SaveChanges();
                    //    message = "1";
                    //    return TokenManager.GenerateToken(message);
                    //}
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [NonAction]
        public void saveAvgPrice(List<itemsTransfer> newObject)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var set = entity.setting.Where(x => x.name == "item_cost").FirstOrDefault();
                string invoiceNum = "0";
                if (set != null)
                    invoiceNum = entity.setValues
                        .Where(x => x.settingId == (int)set.settingId)
                        .Select(x => x.value)
                        .Single();
                foreach (itemsTransfer item in newObject)
                {
                    var itemId = entity.itemsUnits
                        .Where(x => x.itemUnitId == (int)item.itemUnitId)
                        .Select(x => x.itemId)
                        .Single();

                    decimal price = GetAvgPrice(
                        (int)item.itemUnitId,
                        (int)itemId,
                        int.Parse(invoiceNum)
                    );
                    var itemO = entity.items.Find(itemId);
                    itemO.avgPurchasePrice = price;
                }
                entity.SaveChanges();
            }
        }

        private decimal GetAvgPrice(long itemUnitId, long itemId, int numInvoice)
        {
            decimal price = 0;
            int totalNum = 0;
            decimal smallUnitPrice = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemUnits = (
                    from i in entity.itemsUnits
                    where (i.itemId == itemId)
                    select (i.itemUnitId)
                ).ToList();
                List<long> invoicesIds = new List<long>();
                if (numInvoice == 0)
                {
                    invoicesIds = (
                        from p in entity.invoices
                        where p.isActive == true && (p.invType == "p" || p.invType == "is")
                        select p
                    )
                        .Select(x => x.invoiceId)
                        .ToList();
                }
                else
                {
                    var invoices = (
                        from p in entity.invoices
                        where p.isActive == true && p.invType == "p"
                        orderby p.invDate descending
                        select p
                    ).Take(numInvoice);
                    invoicesIds = invoices.Select(x => x.invoiceId).ToList();
                }
                price += getLastPrice(itemUnits, invoicesIds);
                totalNum = getItemUnitLastNum(itemUnits, invoicesIds);
                if (totalNum != 0)
                    smallUnitPrice = price / totalNum;
                return smallUnitPrice;
            }
        }

        private int getUpperUnitValue(long itemUnitId, long basicItemUnitId)
        {
            int unitValue = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == itemUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit == null)
                    return 1;
                if (upperUnit.itemUnitId == basicItemUnitId)
                    return (int)upperUnit.unitValue;
                else
                    unitValue *= getUpperUnitValue(upperUnit.itemUnitId, basicItemUnitId);
                return unitValue;
            }
        }

        private decimal getItemUnitSumPrice(List<long> itemUnits)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var sumPrice = (
                    from b in entity.invoices
                    where b.invType == "p"
                    join s in entity.itemsTransfer.Where(x => itemUnits.Contains((int)x.itemUnitId))
                        on b.invoiceId equals s.invoiceId
                    select s.quantity * s.price
                ).Sum();

                if (sumPrice != null)
                    return (decimal)sumPrice;
                else
                    return 0;
            }
        }

        private decimal getLastPrice(List<long> itemUnits, List<long> invoiceIds)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var sumPrice = (
                    from s in entity.itemsTransfer.Where(
                        x =>
                            itemUnits.Contains((int)x.itemUnitId)
                            && invoiceIds.Contains((int)x.invoiceId)
                    )
                    select s.quantity * s.price
                ).Sum();

                if (sumPrice != null)
                    return (decimal)sumPrice;
                else
                    return 0;
            }
        }

        private int getItemUnitLastNum(List<long> itemUnits, List<long> invoiceIds)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var smallestUnitId = (
                    from iu in entity.itemsUnits
                    where (itemUnits.Contains((int)iu.itemUnitId) && iu.unitId == iu.subUnitId)
                    select iu.itemUnitId
                ).FirstOrDefault();

                if (smallestUnitId == null || smallestUnitId == 0)
                {
                    smallestUnitId = (
                        from u in entity.itemsUnits
                        where !entity.itemsUnits.Any(y => u.subUnitId == y.unitId)
                        where (itemUnits.Contains((int)u.itemUnitId))
                        select u.itemUnitId
                    ).FirstOrDefault();
                }
                var lst = entity.itemsTransfer
                    .Where(
                        x => x.itemUnitId == smallestUnitId && invoiceIds.Contains((int)x.invoiceId)
                    )
                    .Select(t => new ItemLocationModel { quantity = t.quantity, })
                    .ToList();
                long sumNum = 0;
                if (lst.Count > 0)
                    sumNum = lst.Sum(x => x.quantity);

                if (sumNum == null)
                    sumNum = 0;

                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == smallestUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null && upperUnit.itemUnitId != smallestUnitId)
                    sumNum +=
                        (int)upperUnit.unitValue * getLastNum(upperUnit.itemUnitId, invoiceIds);

                try
                {
                    return (int)sumNum;
                }
                catch
                {
                    return 0;
                }
            }
        }

        private long getLastNum(long itemUnitId, List<long> invoiceIds)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                //var sumNum = (from s in entity.itemsTransfer.Where(x => x.itemUnitId == itemUnitId && invoiceIds.Contains((int)x.invoiceId))
                //              select s.quantity).Sum();
                var lst = entity.itemsTransfer
                    .Where(x => x.itemUnitId == itemUnitId && invoiceIds.Contains((int)x.invoiceId))
                    .Select(t => new ItemLocationModel { quantity = t.quantity, })
                    .ToList();
                long sumNum = 0;
                if (lst.Count > 0)
                    sumNum = lst.Sum(x => x.quantity);
                if (sumNum == null)
                    sumNum = 0;

                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == itemUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null)
                    sumNum +=
                        (int)upperUnit.unitValue * getLastNum(upperUnit.itemUnitId, invoiceIds);

                if (sumNum != null)
                    return (long)sumNum;
                else
                    return 0;
            }
        }

        private long getItemUnitNum(long itemUnitId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var sumNum = (
                    from b in entity.invoices
                    where b.invType.Contains("p")
                    join s in entity.itemsTransfer.Where(x => x.itemUnitId == itemUnitId)
                        on b.invoiceId equals s.invoiceId
                    select s.quantity
                ).Sum();

                if (sumNum == null)
                    sumNum = 0;

                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == itemUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null)
                    sumNum += (int)upperUnit.unitValue * getItemUnitNum(upperUnit.itemUnitId);

                if (sumNum != null)
                    return (long)sumNum;
                else
                    return 0;
            }
        }

        private long getItemUnitNum(long itemUnitId, int invoiceNum)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var sumNum = (
                    from b in entity.invoices
                    where b.invType.Contains("p")
                    join s in entity.itemsTransfer.Where(x => x.itemUnitId == itemUnitId)
                        on b.invoiceId equals s.invoiceId
                    select s.quantity
                ).Sum();

                if (sumNum == null)
                    sumNum = 0;

                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == itemUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null)
                    sumNum += (int)upperUnit.unitValue * getItemUnitNum(upperUnit.itemUnitId);

                if (sumNum != null)
                    return (long)sumNum;
                else
                    return 0;
            }
        }

        private int getItemUnitTotalNum(List<long> itemUnits)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var smallestUnitId = (
                    from iu in entity.itemsUnits
                    where (itemUnits.Contains((int)iu.itemUnitId) && iu.unitId == iu.subUnitId)
                    select iu.itemUnitId
                ).FirstOrDefault();

                if (smallestUnitId == null || smallestUnitId == 0)
                {
                    smallestUnitId = (
                        from u in entity.itemsUnits
                        where !entity.itemsUnits.Any(y => u.subUnitId == y.unitId)
                        where (itemUnits.Contains((int)u.itemUnitId))
                        select u.itemUnitId
                    ).FirstOrDefault();
                }
                var sumNum = (
                    from b in entity.invoices
                    where b.invType == "p"
                    join s in entity.itemsTransfer.Where(x => x.itemUnitId == smallestUnitId)
                        on b.invoiceId equals s.invoiceId
                    select s.quantity
                ).Sum();

                if (sumNum == null)
                    sumNum = 0;

                var unit = entity.itemsUnits
                    .Where(x => x.itemUnitId == smallestUnitId)
                    .Select(x => new { x.unitId, x.itemId })
                    .FirstOrDefault();
                var upperUnit = entity.itemsUnits
                    .Where(
                        x =>
                            x.subUnitId == unit.unitId
                            && x.itemId == unit.itemId
                            && x.subUnitId != x.unitId
                    )
                    .Select(x => new { x.unitValue, x.itemUnitId })
                    .FirstOrDefault();

                if (upperUnit != null && upperUnit.itemUnitId != smallestUnitId)
                    sumNum += (int)upperUnit.unitValue * getItemUnitNum(upperUnit.itemUnitId);

                if (sumNum != null)
                    return (int)sumNum;
                else
                    return 0;
            }
        }

        public List<InvoiceModel> getUnhandeledOrdersList(
            string invType,
            long branchCreatorId,
            long branchId,
            int duration = 0,
            long userId = 0
        )
        {
            string[] invTypeArray = invType.Split(',');
            List<string> invTypeL = new List<string>();
            foreach (string s in invTypeArray)
                invTypeL.Add(s.Trim());

            using (incposdbEntities entity = new incposdbEntities())
            {
                var searchPredicate = PredicateBuilder.New<invoices>();
                searchPredicate = searchPredicate.And(
                    inv => inv.isActive == true && invTypeL.Contains(inv.invType)
                );
                if (duration > 0)
                {
                    DateTime dt = Convert.ToDateTime(
                        DateTime.Today.AddDays(-duration).ToShortDateString()
                    );
                    searchPredicate = searchPredicate.And(inv => inv.updateDate >= dt);
                }
                if (branchCreatorId != 0)
                    searchPredicate = searchPredicate.And(
                        inv =>
                            inv.branchCreatorId == branchCreatorId
                            && inv.isActive == true
                            && invTypeL.Contains(inv.invType)
                    );

                if (branchId != 0)
                    searchPredicate = searchPredicate.And(inv => inv.branchId == branchId);
                if (userId != 0)
                    searchPredicate = searchPredicate.And(inv => inv.createUserId == userId);
                var invoicesList = (
                    from b in entity.invoices.Where(searchPredicate)
                    join u in entity.users on b.createUserId equals u.userId into uj
                    from us in uj.DefaultIfEmpty()
                    join l in entity.branches on b.branchId equals l.branchId into lj
                    from x in lj.DefaultIfEmpty()
                    join y in entity.branches on b.branchCreatorId equals y.branchId into yj
                    from z in yj.DefaultIfEmpty()
                    join a in entity.agents on b.agentId equals a.agentId into aj
                    from ag in aj.DefaultIfEmpty()
                    where !entity.invoices.Any(y => y.invoiceMainId == b.invoiceId)
                    select new InvoiceModel()
                    {
                        invoiceId = b.invoiceId,
                        invNumber = b.invNumber,
                        agentId = b.agentId,
                        agentName = ag.name,
                        invType = b.invType,
                        tax = b.tax,
                        taxtype = b.taxtype,
                        name = b.name,
                        branchName = x.name,
                        branchCreatorName = z.name,
                        createrUserName = us.name + " " + us.lastname,
                        totalNet = b.totalNet,
                        total = b.total,
                        discountType = b.discountType,
                        discountValue = b.discountValue,
                        manualDiscountType = b.manualDiscountType,
                        manualDiscountValue = b.manualDiscountValue,
                        realShippingCost = b.realShippingCost,
                        shippingCost = b.shippingCost,
                        updateUserId = b.updateUserId,
                        isApproved = b.isApproved,
                        branchId = b.branchId,
                        invBarcode = b.invBarcode,
                    }
                ).ToList();
                if (invoicesList != null)
                {
                    for (int i = 0; i < invoicesList.Count(); i++)
                    {
                        long invoiceId = invoicesList[i].invoiceId;
                        invoicesList[i].invoiceItems = itc.Get(invoiceId);
                        invoicesList[i].itemsCount = invoicesList[i].invoiceItems.Count;
                    }
                }
                return invoicesList;
            }
        }

        public decimal AvgItemPurPrice(long itemUnitId, long itemId)
        {
            decimal price = 0;
            int totalNum = 0;
            decimal smallUnitPrice = 0;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var itemUnits = (
                    from i in entity.itemsUnits
                    where (i.itemId == itemId)
                    select (i.itemUnitId)
                ).ToList();

                price += getItemUnitSumPrice(itemUnits);

                totalNum = getItemUnitTotalNum(itemUnits);

                if (totalNum != 0)
                    smallUnitPrice = price / totalNum;

                var smallestUnitId = (
                    from iu in entity.itemsUnits
                    where (itemUnits.Contains((int)iu.itemUnitId) && iu.unitId == iu.subUnitId)
                    select iu.itemUnitId
                ).FirstOrDefault();

                if (smallestUnitId == null || smallestUnitId == 0)
                {
                    smallestUnitId = (
                        from u in entity.itemsUnits
                        where !entity.itemsUnits.Any(y => u.subUnitId == y.unitId)
                        where (itemUnits.Contains((int)u.itemUnitId))
                        select u.itemUnitId
                    ).FirstOrDefault();
                }
                if (itemUnitId == smallestUnitId || smallestUnitId == null || smallestUnitId == 0)
                    return smallUnitPrice;
                else
                {
                    smallUnitPrice = smallUnitPrice * getUpperUnitValue(smallestUnitId, itemUnitId);
                    return smallUnitPrice;
                }
            }
        }

        [HttpPost]
        [Route("GetDailyShortage")]
        public string GetDailyShortage(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                long branchId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                #endregion

                using (incposdbEntities entity = new incposdbEntities())
                {
                    DateTime dt = Convert.ToDateTime(DateTime.Today.ToShortDateString());

                    var invoicesList = (
                        from I in entity.invoices.Where(
                            x =>
                                x.branchCreatorId == branchId
                                && x.invType == "sh"
                                && x.isActive == true
                                && x.createUserId == userId
                                && x.invDate >= dt
                        )
                        join IT in entity.itemsTransfer on I.invoiceId equals IT.invoiceId
                        from IU in entity.itemsUnits.Where(IU => IU.itemUnitId == IT.itemUnitId)

                        join BC in entity.branches on I.branchCreatorId equals BC.branchId into JBC
                        join U in entity.users on I.createUserId equals U.userId into JU
                        join du in entity.users on I.userId equals du.userId into Dusr
                        from JUU in JU.DefaultIfEmpty()
                        from duu in Dusr.DefaultIfEmpty()
                        from JBCC in JBC.DefaultIfEmpty()

                        select new ItemTransferInvoice
                        {
                            causeDestroy =
                                IT.inventoryItemLocation.fallCause != null
                                    ? IT.inventoryItemLocation.fallCause
                                    : IT.cause,
                            userdestroy = duu.username,
                            itemName = IU.items.name,
                            unitName = IU.units.name,
                            itemUnitId = IT.itemUnitId,
                            itemId = IU.itemId,
                            unitId = IU.unitId,
                            quantity = IT.quantity,
                            invoiceId = I.invoiceId,
                            invNumber = I.invNumber,
                            total = I.totalNet,
                            IupdateDate = I.updateDate,
                            branchName = JBCC.name,
                            branchId = I.branchCreatorId,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("shortageItem")]
        public async Task<string> shortageItem(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";

            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notificationObj = "";
                invoices newObject = null;
                inventoryItemLocation itemLocationInv = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                NotificationUserModel notificationUser = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "itemLocationInv")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemLocationInv = JsonConvert.DeserializeObject<inventoryItemLocation>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notificationObj = c.Value.Replace("\\", string.Empty);
                        notificationObj = notificationObj.Trim('"');
                        notificationUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notificationObj,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();

                    #region check item quantity
                    ItemsLocationsController ilc = new ItemsLocationsController();
                    string res = ilc.checkItemsAmounts(
                        billDetails,
                        (long)newObject.branchCreatorId,
                        0
                    );

                    if (!res.Equals(""))
                    {
                        return TokenManager.GenerateToken("-3");
                    }
                    #endregion
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region shortage item
                            InventoryItemLocationController iic =
                                new InventoryItemLocationController();
                            iic.fallItem(itemLocationInv);
                            #endregion
                            #region decrease item quantity
                            ilc.decreaseItemLocationQuantity(
                                (long)itemLocationInv.itemLocationId,
                                (int)itemLocationInv.amount,
                                (long)newObject.createUserId,
                                notificationUser.objectName,
                                notificationObj
                            );
                            #endregion
                            #region record cash transfer
                            if (newObject.userId != null)
                            {
                                var paid = depositFromUserBalance(
                                    (long)newObject.userId,
                                    newObject.invoiceId
                                );

                                CashTransferController cc = new CashTransferController();

                                cashTransfer cashTrasnfer = new cashTransfer();
                                cashTrasnfer.cash = newObject.total;
                                cashTrasnfer.paid = 0;
                                cashTrasnfer.deserved = newObject.total;
                                cashTrasnfer.posId = newObject.posId;
                                cashTrasnfer.userId = (long)newObject.userId;
                                cashTrasnfer.invId = newObject.invoiceId;
                                cashTrasnfer.createUserId = newObject.createUserId;
                                cashTrasnfer.processType = "shortage";
                                cashTrasnfer.isCommissionPaid = 0;
                                cashTrasnfer.side = "u"; // user
                                cashTrasnfer.transType = "p"; //deposit
                                cashTrasnfer.transNum = "pu";
                                await cc.addCashTransfer(cashTrasnfer);
                            }
                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("destroyItem")]
        public async Task<string> destroyItem(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";

            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notificationObj = "";
                invoices newObject = null;
                inventoryItemLocation itemLocationInv = null;
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                NotificationUserModel notificationUser = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "itemLocationInv")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemLocationInv = JsonConvert.DeserializeObject<inventoryItemLocation>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notificationObj = c.Value.Replace("\\", string.Empty);
                        notificationObj = notificationObj.Trim('"');
                        notificationUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notificationObj,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                try
                {
                    #region check item quantity
                    ItemsLocationsController ilc = new ItemsLocationsController();
                    string res = ilc.checkItemsAmounts(
                        billDetails,
                        (long)newObject.branchCreatorId,
                        0
                    );

                    if (!res.Equals(""))
                    {
                        return TokenManager.GenerateToken("-3");
                    }
                    #endregion
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region destroy item
                            InventoryItemLocationController iic =
                                new InventoryItemLocationController();
                            iic.destroyItem(itemLocationInv);
                            #endregion

                            #region decrease item quantity

                            ilc.decreaseItemLocationQuantity(
                                (long)itemLocationInv.itemLocationId,
                                (int)itemLocationInv.amountDestroyed,
                                (long)newObject.createUserId,
                                notificationUser.objectName,
                                notificationObj
                            );
                            #endregion
                            #region record cash transfer
                            if (newObject.userId != null)
                            {
                                var paid = depositFromUserBalance(
                                    (long)newObject.userId,
                                    newObject.invoiceId
                                );

                                CashTransferController cc = new CashTransferController();

                                cashTransfer cashTrasnfer = new cashTransfer();
                                cashTrasnfer.cash = newObject.total;
                                cashTrasnfer.paid = 0;
                                cashTrasnfer.deserved = newObject.total;
                                cashTrasnfer.posId = newObject.posId;
                                cashTrasnfer.userId = (long)newObject.userId;
                                cashTrasnfer.invId = newObject.invoiceId;
                                cashTrasnfer.createUserId = newObject.createUserId;
                                cashTrasnfer.processType = "destroy";
                                cashTrasnfer.isCommissionPaid = 0;
                                cashTrasnfer.side = "u"; // user
                                cashTrasnfer.transType = "p"; //deposit
                                cashTrasnfer.transNum = "pu";
                                await cc.addCashTransfer(cashTrasnfer);
                            }
                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("manualDestroyItem")]
        public async Task<string> manualDestroyItem(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";

            var strP = TokenManager.GetPrincipal(token);

            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string notificationObj = "";
                invoices newObject = null;
                List<itemsLocations> itemsLoc = new List<itemsLocations>();
                List<itemsTransfer> transferObject = new List<itemsTransfer>();
                List<ItemTransferModel> billDetails = new List<ItemTransferModel>();
                NotificationUserModel notificationUser = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<invoices>(
                            Object,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "itemTransferObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        transferObject = JsonConvert.DeserializeObject<List<itemsTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        billDetails = JsonConvert.DeserializeObject<List<ItemTransferModel>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "itemsLoc")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        itemsLoc = JsonConvert.DeserializeObject<List<itemsLocations>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "not")
                    {
                        notificationObj = c.Value.Replace("\\", string.Empty);
                        notificationObj = notificationObj.Trim('"');
                        notificationUser = JsonConvert.DeserializeObject<NotificationUserModel>(
                            notificationObj,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                try
                {
                    ProgramDetailsController pc = new ProgramDetailsController();
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region check items quantity in store
                        ItemsLocationsController itc = new ItemsLocationsController();
                        foreach (var itemLoc in itemsLoc)
                        {
                            long itemLocId = itemLoc.itemsLocId;
                            long quantity = itemLoc.quantity;
                            string res = itc.checkLocationAmounts(itemLocId, quantity);
                            if (!res.Equals(""))
                            {
                                return TokenManager.GenerateToken("-3");
                            }
                        }

                        #endregion
                        newObject.invoiceId = await saveInvoice(newObject);

                        message = newObject.invoiceId.ToString();
                        long invoiceId = newObject.invoiceId;
                        if (!invoiceId.Equals(0))
                        {
                            //save items transfer
                            ItemsTransferController it = new ItemsTransferController();
                            it.saveInvoiceItems(transferObject, invoiceId);

                            #region decrease item quantity

                            foreach (var itemLoc in itemsLoc)
                            {
                                long itemLocId = itemLoc.itemsLocId;
                                int quantity = (int)itemLoc.quantity;
                                itc.decreaseItemLocationQuantity(
                                    (long)itemLocId,
                                    quantity,
                                    (long)newObject.createUserId,
                                    notificationUser.objectName,
                                    notificationObj
                                );
                            }
                            #endregion
                            #region record cash transfer
                            if (newObject.userId != null)
                            {
                                var paid = depositFromUserBalance(
                                    (long)newObject.userId,
                                    newObject.invoiceId
                                );

                                CashTransferController cc = new CashTransferController();

                                cashTransfer cashTrasnfer = new cashTransfer();
                                cashTrasnfer.cash = newObject.total;
                                cashTrasnfer.paid = 0;
                                cashTrasnfer.deserved = newObject.total;
                                cashTrasnfer.posId = newObject.posId;
                                cashTrasnfer.userId = (long)newObject.userId;
                                cashTrasnfer.invId = newObject.invoiceId;
                                cashTrasnfer.createUserId = newObject.createUserId;
                                cashTrasnfer.processType = "destroy";
                                cashTrasnfer.side = "u"; // user
                                cashTrasnfer.transType = "p"; //deposit
                                cashTrasnfer.transNum = "pu";
                                cashTrasnfer.isCommissionPaid = 0;

                                await cc.addCashTransfer(cashTrasnfer);
                            }
                            #endregion
                        }
                    }
                }
                catch
                {
                    message = "0";
                }

                return TokenManager.GenerateToken(message);
            }
        }

        public decimal depositFromUserBalance(long userId, long invoiceId)
        {
            decimal paid = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var user = entity.users.Find(userId);
                var invoice = entity.invoices.Find(invoiceId);

                if (user.balanceType == 0)
                {
                    if (invoice.totalNet <= (decimal)user.balance)
                    {
                        invoice.paid = invoice.totalNet;
                        invoice.deserved = 0;
                        user.balance -= (decimal)invoice.totalNet;
                    }
                    else
                    {
                        invoice.paid = (decimal)user.balance;
                        invoice.deserved = invoice.totalNet - (decimal)user.balance;
                        decimal newBalance = (decimal)invoice.totalNet - (decimal)user.balance;
                        user.balance = newBalance;
                        user.balanceType = 1;
                    }

                    paid = (decimal)invoice.paid;

                    entity.SaveChanges();
                }
                else if (user.balanceType == 1)
                {
                    decimal newBalance = (decimal)user.balance + (decimal)invoice.totalNet;
                    user.balance = newBalance;
                    entity.SaveChanges();
                }
                return paid;
            }
        }

        [HttpPost]
        [Route("GetDailyDestructive")]
        public string GetDailyDestructive(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                long branchId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                #endregion

                using (incposdbEntities entity = new incposdbEntities())
                {
                    DateTime dt = Convert.ToDateTime(DateTime.Today.ToShortDateString());

                    var invoicesList = (
                        from I in entity.invoices.Where(
                            x =>
                                x.branchCreatorId == branchId
                                && x.invType == "d"
                                && x.isActive == true
                                && x.createUserId == userId
                                && x.invDate >= dt
                        )
                        join IT in entity.itemsTransfer on I.invoiceId equals IT.invoiceId
                        from IU in entity.itemsUnits.Where(IU => IU.itemUnitId == IT.itemUnitId)

                        join BC in entity.branches on I.branchCreatorId equals BC.branchId into JBC
                        join U in entity.users on I.createUserId equals U.userId into JU
                        join du in entity.users on I.userId equals du.userId into Dusr
                        from JUU in JU.DefaultIfEmpty()
                        from duu in Dusr.DefaultIfEmpty()
                        from JBCC in JBC.DefaultIfEmpty()

                        select new ItemTransferInvoice
                        {
                            causeDestroy =
                                IT.inventoryItemLocation.fallCause != null
                                    ? IT.inventoryItemLocation.fallCause
                                    : IT.cause,
                            userdestroy = duu.username,
                            itemName = IU.items.name,
                            unitName = IU.units.name,
                            itemUnitId = IT.itemUnitId,
                            itemId = IU.itemId,
                            unitId = IU.unitId,
                            quantity = IT.quantity,
                            invoiceId = I.invoiceId,
                            invNumber = I.invNumber,
                            total = I.totalNet,
                            IupdateDate = I.updateDate,
                            branchName = JBCC.name,
                            branchId = I.branchCreatorId,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }
    }
}
