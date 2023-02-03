using LinqKit;
using Newtonsoft.Json;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/OrderPreparing")]
    public class OrderPreparingController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("GetInvoicePreparingOrders")]
        public string GetInvoicePreparingOrders(string token)
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
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var prepOrders = (
                            from o in entity.orderPreparing.Where(x => x.invoiceId == invoiceId)
                            join i in entity.itemOrderPreparing
                                on o.orderPreparingId equals i.orderPreparingId
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
                                invoiceId = o.invoiceId,
                                notes = o.notes,
                                orderNum = o.orderNum,
                                preparingTime = o.preparingTime,
                                updateDate = o.updateDate,
                                updateUserId = o.updateUserId,
                                createDate = o.createDate,
                                createUserId = o.createUserId,
                                itemName = i.itemsUnits.items.name,
                                quantity = (int)i.quantity,
                                status = s.status,
                                itemUnitId = i.itemUnitId,
                                itemsTransId = i.itemsTransId,
                            }
                        ).OrderBy(x => x.orderNum).ToList();

                        int index = 1;
                        foreach (OrderPreparingModel o in prepOrders)
                        {
                            o.num = index;
                            index++;
                            #region preparing status date
                            if (o.status == "Listed")
                                o.preparingStatusDate = null;
                            else
                            {
                                DateTime createDate = (DateTime)
                                    entity.orderPreparingStatus
                                        .Where(
                                            x =>
                                                x.orderPreparingId == o.orderPreparingId
                                                && x.status == "Preparing"
                                        )
                                        .Select(x => x.createDate)
                                        .SingleOrDefault();
                                o.preparingStatusDate = (DateTime)createDate;
                            }
                            #endregion
                            //#region calculate remaining time
                            //if (o.preparingTime != null)
                            //{
                            //   // DateTime createDate = (DateTime)o.updateDate;
                            //    DateTime createDate = (DateTime)entity.orderPreparingStatus
                            //                                       .Where(x => x.orderPreparingId == o.orderPreparingId && x.status == "Preparing")
                            //                                       .Select(x => x.createDate).SingleOrDefault();
                            //    createDate = createDate.AddMinutes((double)o.preparingTime);

                            //    if (createDate > DateTime.Now)
                            //    {
                            //        TimeSpan remainingTime =  createDate -  coctrlr.AddOffsetTodate(DateTime.Now);
                            //        o.remainingTime = (decimal)remainingTime.TotalMinutes;
                            //    }
                            //    else
                            //    {
                            //        o.remainingTime = 0;

                            //    }
                            //}
                            //if (o.status == "Listed")
                            //{
                            //    var orderItemUnits = entity.itemOrderPreparing.Where(x => x.orderPreparingId == o.orderPreparingId).Select(x => x.itemUnitId).ToList();
                            //    o.remainingTime = (decimal) entity.menuSettings.Where(x => orderItemUnits.Contains(x.itemUnitId)).Select(x => x.preparingTime).Max();
                            //}
                            //#endregion
                        }
                        return TokenManager.GenerateToken(prepOrders);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetKitchenPreparingOrders")]
        public string GetKitchenPreparingOrders(string token)
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
                string statusStr = "";
                List<string> statusL = new List<string>();
                long branchId = 0;
                int duration = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "status")
                    {
                        statusStr = c.Value;
                        string[] statusArray = statusStr.Split(',');
                        foreach (string s in statusArray)
                            statusL.Add(s.Trim());
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region search predicate
                        var searchPredicate = PredicateBuilder.New<invoices>();
                        searchPredicate = searchPredicate.And(x => x.branchId == branchId);

                        if (duration > 0)
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-duration));
                            searchPredicate = searchPredicate.And(x => x.updateDate >= dt);
                            // return dt.ToString();
                        }
                        #endregion
                        var invoices = entity.invoices
                            .Where(searchPredicate)
                            .Select(
                                x =>
                                    new InvoiceModel()
                                    {
                                        invNumber = x.invNumber,
                                        invoiceId = x.invoiceId,
                                        orderTime = x.orderTime,
                                        agentName = x.agents.name,
                                    }
                            )
                            .ToList();

                        List<OrderPreparingModel> finalRrepOrders = new List<OrderPreparingModel>();
                        foreach (InvoiceModel inv in invoices)
                        {
                            #region get invoice tables
                            var tables = (
                                from t in entity.tables.Where(x => x.isActive == 1)
                                join it in entity.invoiceTables.Where(
                                    x => x.invoiceId == inv.invoiceId
                                )
                                    on t.tableId equals it.tableId
                                select new TableModel() { tableId = t.tableId, name = t.name, }
                            ).ToList();
                            string tablesNames = "";
                            foreach (TableModel tabl in tables)
                            {
                                if (tablesNames == "")
                                    tablesNames += tabl.name;
                                else
                                    tablesNames += ", " + tabl.name;
                            }
                            #endregion

                            var prepOrders = (
                                from o in entity.orderPreparing.Where(
                                    x => x.invoiceId == inv.invoiceId
                                )
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
                                    invoiceId = o.invoiceId,
                                    notes = o.notes,
                                    orderNum = o.orderNum,
                                    preparingTime = o.preparingTime,
                                    updateDate = o.updateDate,
                                    updateUserId = o.updateUserId,
                                    createDate = o.createDate,
                                    createUserId = o.createUserId,
                                    invNum = o.invoices.invNumber,
                                    invType = o.invoices.invType,
                                    shippingCompanyId = o.invoices.shippingCompanyId,
                                    waiter = entity.users
                                        .Where(x => x.userId == o.invoices.waiterId)
                                        .Select(x => x.name)
                                        .FirstOrDefault(),
                                    items = entity.itemOrderPreparing
                                        .Where(x => x.orderPreparingId == o.orderPreparingId)
                                        .Select(
                                            x =>
                                                new itemOrderPreparingModel()
                                                {
                                                    itemOrderId = x.itemOrderId,
                                                    itemName = x.itemsUnits.items.name,
                                                    itemId = x.itemsUnits.items.itemId,
                                                    itemUnitId = x.itemUnitId,
                                                    quantity = x.quantity,
                                                    createDate = x.createDate,
                                                    updateDate = x.updateDate,
                                                    createUserId = x.createUserId,
                                                    updateUserId = x.updateUserId,
                                                    categoryId = x.itemsUnits
                                                        .items
                                                        .categories
                                                        .categoryId,
                                                    categoryName = x.itemsUnits
                                                        .items
                                                        .categories
                                                        .name,
                                                    itemsTransId = x.itemsTransId,
                                                }
                                        )
                                        .ToList(),
                                    status = s.status,
                                }
                            ).ToList();

                            #region get orders according to status
                            if (statusStr != "")
                                prepOrders = prepOrders
                                    .Where(x => statusL.Contains(x.status))
                                    .OrderBy(x => x.orderNum)
                                    .ToList();
                            #endregion

                            decimal longestPreparingTime = 0;
                            foreach (OrderPreparingModel o in prepOrders)
                            {
                                #region preparing time from menu list
                                if (o.status == "Listed")
                                {
                                    if (o.preparingTime == null || o.preparingTime == 0)
                                    {
                                        var orderItemUnits = entity.itemOrderPreparing
                                            .Where(x => x.orderPreparingId == o.orderPreparingId)
                                            .Select(x => x.itemUnitId)
                                            .ToList();
                                        o.preparingTime = entity.menuSettings
                                            .Where(x => orderItemUnits.Contains(x.itemUnitId))
                                            .Select(x => x.preparingTime)
                                            .Max();
                                    }
                                }
                                #endregion

                                #region invoice tables
                                o.tables = tablesNames;
                                #endregion

                                #region preparing status date
                                if (o.status == "Listed")
                                    o.preparingStatusDate = null;
                                else
                                {
                                    DateTime createDate = (DateTime)
                                        entity.orderPreparingStatus
                                            .Where(
                                                x =>
                                                    x.orderPreparingId == o.orderPreparingId
                                                    && x.status == "Preparing"
                                            )
                                            .Select(x => x.createDate)
                                            .SingleOrDefault();
                                    o.preparingStatusDate = (DateTime)createDate;
                                }
                                #endregion

                                #region extra items - ingredients

                                foreach (var it in o.items)
                                {
                                    if (it.itemsTransId != null)
                                    {
                                        long id = (long)it.itemsTransId;

                                        it.itemsIngredients = entity.itemsTransferIngredients
                                            .Where(x => x.itemsTransId == id)
                                            .Select(
                                                x =>
                                                    new itemsTransferIngredientsModel()
                                                    {
                                                        dishIngredId = x.dishIngredId,
                                                        isActive = x.isActive,
                                                        DishIngredientName = x.dishIngredients.name,
                                                        itemUnitId = x.itemsTransfer.itemUnitId,
                                                        itemsTransId = x.itemsTransId,
                                                        itemsTransIngredId = x.itemsTransIngredId
                                                    }
                                            )
                                            .ToList();

                                        //extras
                                        it.itemExtras = (
                                            from t in entity.itemsTransfer.Where(
                                                x => x.mainCourseId == id
                                            )
                                            join u in entity.itemsUnits
                                                on t.itemUnitId equals u.itemUnitId
                                            join i in entity.items on u.itemId equals i.itemId
                                            join un in entity.units on u.unitId equals un.unitId
                                            select new ItemTransferModel()
                                            {
                                                itemsTransId = t.itemsTransId,
                                                itemId = i.itemId,
                                                itemName = i.name,
                                                quantity = t.quantity,
                                                notes = t.notes,
                                                price = t.price,
                                                unitName = un.name,
                                            }
                                        ).ToList();
                                    }
                                }

                                #endregion


                                // set sequence num to items
                                int index = 1;
                                foreach (itemOrderPreparingModel item in o.items)
                                {
                                    item.sequence = index;
                                    index++;
                                }

                                #region get longest preparing time
                                if (inv.orderTime != null && longestPreparingTime < o.preparingTime)
                                {
                                    longestPreparingTime = (decimal)o.preparingTime;
                                }
                                #endregion
                            }

                            if (inv.orderTime == null) // add all invoice orders to result
                                finalRrepOrders.AddRange(prepOrders);
                            else
                            {
                                DateTime dt = Convert.ToDateTime(
                                    DateTime.Now.AddMinutes((double)longestPreparingTime)
                                );
                                if (dt >= inv.orderTime) // add only in time orders
                                    finalRrepOrders.AddRange(prepOrders);
                            }
                        }

                        return TokenManager.GenerateToken(finalRrepOrders);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetHallOrdersWithStatus")]
        public string GetHallOrdersWithStatus(string token)
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
                string statusStr = "";
                List<string> statusL = new List<string>();
                long branchId = 0;
                int duration = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "status")
                    {
                        statusStr = c.Value;
                        string[] statusArray = statusStr.Split(',');
                        foreach (string s in statusArray)
                            statusL.Add(s.Trim());
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<orderPreparing>();
                        searchPredicate = searchPredicate.And(x => x.invoices.branchId == branchId);

                        if (duration > 0)
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-duration));
                            searchPredicate = searchPredicate.And(x => x.createDate >= dt);
                        }

                        var prepOrders = (
                            from o in entity.orderPreparing.Where(searchPredicate)
                            join i in entity.invoices on o.invoiceId equals i.invoiceId
                            join it in entity.invoiceTables on i.invoiceId equals it.invoiceId
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
                                invoiceId = o.invoiceId,
                                notes = o.notes,
                                orderNum = o.orderNum,
                                preparingTime = o.preparingTime,
                                updateDate = o.updateDate,
                                updateUserId = o.updateUserId,
                                createDate = o.createDate,
                                createUserId = o.createUserId,
                                invNum = o.invoices.invNumber,
                                waiter = entity.users
                                    .Where(x => x.userId == o.invoices.waiterId)
                                    .Select(x => x.name)
                                    .FirstOrDefault(),
                                items = entity.itemOrderPreparing
                                    .Where(x => x.orderPreparingId == o.orderPreparingId)
                                    .Select(
                                        x =>
                                            new itemOrderPreparingModel()
                                            {
                                                itemOrderId = x.itemOrderId,
                                                itemName = x.itemsUnits.items.name,
                                                itemId = x.itemsUnits.items.itemId,
                                                itemUnitId = x.itemUnitId,
                                                quantity = x.quantity,
                                                createDate = x.createDate,
                                                updateDate = x.updateDate,
                                                createUserId = x.createUserId,
                                                updateUserId = x.updateUserId,
                                                categoryId = x.itemsUnits
                                                    .items
                                                    .categories
                                                    .categoryId,
                                                categoryName = x.itemsUnits.items.categories.name,
                                            }
                                    )
                                    .ToList(),
                                status = s.status,
                            }
                        ).ToList();

                        #region get orders according to status
                        if (statusStr != "")
                            prepOrders = prepOrders
                                .Where(x => statusL.Contains(x.status))
                                .OrderBy(x => x.orderNum)
                                .ToList();
                        #endregion

                        foreach (OrderPreparingModel o in prepOrders)
                        {
                            #region get invoice tables
                            var tables = (
                                from t in entity.tables.Where(x => x.isActive == 1)
                                join it in entity.invoiceTables.Where(
                                    x => x.invoiceId == o.invoiceId
                                )
                                    on t.tableId equals it.tableId
                                select new TableModel() { tableId = t.tableId, name = t.name, }
                            ).ToList();
                            string tablesNames = "";
                            foreach (TableModel tabl in tables)
                            {
                                if (tablesNames == "")
                                    tablesNames += tabl.name;
                                else
                                    tablesNames += ", " + tabl.name;
                            }
                            o.tables = tablesNames;
                            #endregion
                        }
                        return TokenManager.GenerateToken(prepOrders);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetOrdersByTypeWithStatus")]
        public string GetOrdersByTypeWithStatus(string token)
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
                long branchId = 0;
                int duration = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "duration")
                    {
                        duration = int.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<invoices>();
                        searchPredicate = searchPredicate.And(x => x.branchId == branchId);

                        searchPredicate = searchPredicate.And(x => x.invType == type);
                        if (duration > 0)
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-duration));
                            searchPredicate = searchPredicate.And(x => x.invDate >= dt);
                        }

                        var invoices = entity.invoices
                            .Where(searchPredicate)
                            .Select(
                                x =>
                                    new InvoiceModel()
                                    {
                                        invNumber = x.invNumber,
                                        invoiceId = x.invoiceId,
                                        shippingCompanyName = x.shippingCompanies.name,
                                        agentName = x.agents.name,
                                    }
                            )
                            .ToList();

                        foreach (InvoiceModel inv in invoices)
                        {
                            var prepOrders = (
                                from o in entity.orderPreparing.Where(
                                    x => x.invoiceId == inv.invoiceId
                                )
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
                                    invoiceId = o.invoiceId,
                                    notes = o.notes,
                                    orderNum = o.orderNum,
                                    preparingTime = o.preparingTime,
                                    updateDate = o.updateDate,
                                    updateUserId = o.updateUserId,
                                    createDate = o.createDate,
                                    createUserId = o.createUserId,
                                    invNum = o.invoices.invNumber,
                                    status = s.status,
                                }
                            ).ToList();

                            //string status = "";
                            foreach (OrderPreparingModel o in prepOrders)
                            {
                                #region get invoice tables
                                var tables = (
                                    from t in entity.tables.Where(x => x.isActive == 1)
                                    join it in entity.invoiceTables.Where(
                                        x => x.invoiceId == o.invoiceId
                                    )
                                        on t.tableId equals it.tableId
                                    select new TableModel() { tableId = t.tableId, name = t.name, }
                                ).ToList();
                                string tablesNames = "";
                                foreach (TableModel tabl in tables)
                                {
                                    if (tablesNames == "")
                                        tablesNames += tabl.name;
                                    else
                                        tablesNames += ", " + tabl.name;
                                }
                                o.tables = tablesNames;
                                #endregion

                                #region set inv status
                                if (o.status == "Listed" || o.status == "preparing")
                                {
                                    inv.status = "Listed";
                                    break;
                                }
                                else if (o.status == "Ready")
                                    inv.status = "Ready";
                                else
                                    inv.status = "";
                                // status += inv.status;
                                #endregion
                            }
                            //return status;
                        }

                        List<string> statusLst = new List<string>()
                        {
                            "Listed",
                            "Preparing",
                            "Ready"
                        };
                        invoices = invoices.Where(x => statusLst.Contains(x.status)).ToList();
                        return TokenManager.GenerateToken(invoices);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetOrdersWithDelivery")]
        public string GetOrdersWithDelivery(string token)
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
                string statusStr = "";
                List<string> statusL = new List<string>();
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "status")
                    {
                        statusStr = c.Value;
                        string[] statusArray = statusStr.Split(',');
                        foreach (string s in statusArray)
                            statusL.Add(s.Trim());
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<invoices>();
                        searchPredicate = searchPredicate.And(x => x.branchId == branchId);

                        searchPredicate = searchPredicate.And(
                            x => x.invType == "ts" || x.invType == "ss"
                        );
                        searchPredicate = searchPredicate.And(x => x.shippingCompanyId != null);

                        var invoices = (
                            from b in entity.invoices.Where(searchPredicate)
                            join u in entity.users on b.shipUserId equals u.userId into lj
                            from y in lj.DefaultIfEmpty()
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
                                discountType = b.discountType,
                                discountValue = b.discountValue,
                                tax = b.tax,
                                taxtype = b.taxtype,
                                name = b.name,
                                isApproved = b.isApproved,
                                branchCreatorId = b.branchCreatorId,
                                shippingCompanyId = b.shippingCompanyId,
                                shippingCompanyName = b.shippingCompanies.name,
                                shipUserId = b.shipUserId,
                                shipUserName = y.name,
                                shipUserLastName = y.lastname,
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
                                //agent
                                agentName = b.agents.name,
                                agentAddress = b.agents.address,
                                agentMobile = b.agents.mobile,
                                agentResSectorsName = b.agents.residentialSectors.name,
                            }
                        ).ToList();

                        foreach (InvoiceModel inv in invoices)
                        {
                            var prepOrders = (
                                from o in entity.orderPreparing.Where(
                                    x => x.invoiceId == inv.invoiceId
                                )
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
                                    invoiceId = o.invoiceId,
                                    notes = o.notes,
                                    orderNum = o.orderNum,
                                    createDate = o.createDate,
                                    createUserId = o.createUserId,
                                    invNum = o.invoices.invNumber,
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

                        #region get orders according to status
                        if (statusStr != "")
                            invoices = invoices
                                .Where(x => statusL.Contains(x.status))
                                .OrderBy(x => x.invNumber)
                                .ToList();
                        #endregion

                        return TokenManager.GenerateToken(invoices);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetCountHallOrders")]
        public string GetCountHallOrders(string token)
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
                string statusStr = "";
                List<string> statusL = new List<string>();
                long branchId = 0;
                int duration = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "status")
                    {
                        statusStr = c.Value;
                        string[] statusArray = statusStr.Split(',');
                        foreach (string s in statusArray)
                            statusL.Add(s.Trim());
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
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<orderPreparing>();
                        searchPredicate = searchPredicate.And(x => x.invoices.branchId == branchId);

                        if (duration > 0)
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-duration));
                            searchPredicate = searchPredicate.And(x => x.createDate >= dt);
                        }

                        var prepOrders = (
                            from o in entity.orderPreparing.Where(searchPredicate)
                            join i in entity.invoices on o.invoiceId equals i.invoiceId
                            join it in entity.invoiceTables on i.invoiceId equals it.invoiceId
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

                        #region get orders according to status
                        if (statusStr != "")
                            prepOrders = prepOrders
                                .Where(x => statusL.Contains(x.status))
                                .OrderBy(x => x.orderNum)
                                .ToList();
                        #endregion


                        return TokenManager.GenerateToken(prepOrders.Count());
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("SaveWithItemsAndStatus")]
        public string SaveWithItemsAndStatus(string token)
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
                string orderObject = "";
                string itemsObject = "";
                string statusObject = "";
                orderPreparing newObject = null;
                List<itemOrderPreparing> items = null;
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderObject")
                    {
                        orderObject = c.Value.Replace("\\", string.Empty);
                        orderObject = orderObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<orderPreparing>(
                            orderObject,
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
                        items = JsonConvert.DeserializeObject<List<itemOrderPreparing>>(
                            itemsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    long orderId = savePreparingOrder(newObject);
                    if (orderId > 0)
                    {
                        string res = savePreparingOrderItems(items, orderId);
                        if (res == "0")
                            message = "0";
                        else
                        {
                            res = saveInvoiceStatus(status, orderId);
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
        [Route("SaveOrdersWithItemsAndStatus")]
        public string SaveOrdersWithItemsAndStatus(string token)
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
                string orderObject = "";
                string itemsObject = "";
                string statusObject = "";
                string statusesOfPreparingOrder = "";
                long branchId = 0;
                orderPreparing newObject = null;
                List<itemOrderPreparing> items = null;
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderObject")
                    {
                        orderObject = c.Value.Replace("\\", string.Empty);
                        orderObject = orderObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<orderPreparing>(
                            orderObject,
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
                        items = JsonConvert.DeserializeObject<List<itemOrderPreparing>>(
                            itemsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "statusesOfPreparingOrder")
                    {
                        statusesOfPreparingOrder = c.Value;
                    }
                }

                try
                {
                    string branchCode = "";
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        branchCode = entity.branches
                            .Where(x => x.branchId == branchId)
                            .Select(x => x.code)
                            .FirstOrDefault();
                    }
                    foreach (itemOrderPreparing item in items)
                    {
                        #region orderNum

                        var orderNum = GetLastNumOfOrder("ko", branchId);
                        orderNum++;
                        string strSeq = orderNum.ToString();
                        if (orderNum <= 999999)
                            strSeq = orderNum.ToString().PadLeft(6, '0');
                        string invoiceNum = "ko" + "-" + branchCode + "-" + strSeq;
                        newObject.orderNum = invoiceNum;
                        #endregion
                        newObject.orderPreparingId = 0;
                        long orderId = savePreparingOrder(newObject);
                        message = orderId.ToString();
                        if (orderId > 0)
                        {
                            List<itemOrderPreparing> orderItem = new List<itemOrderPreparing>();
                            orderItem.Add(item);
                            string res = savePreparingOrderItems(orderItem, orderId);
                            if (res == "0")
                                message = "0";
                            else
                            {
                                res = saveInvoiceStatus(status, orderId);

                                if (statusesOfPreparingOrder == "directlyPrint")
                                {
                                    #region save status = Preparing
                                    var status1 = new orderPreparingStatus();
                                    status1.orderPreparingId = orderId;
                                    status1.status = "Preparing";
                                    status1.createUserId = status.createUserId;

                                    saveInvoiceStatus(status1, orderId);
                                    #endregion

                                    #region save status = Ready
                                    status1 = new orderPreparingStatus();
                                    status1.orderPreparingId = orderId;
                                    status1.status = "Ready";
                                    status1.createUserId = status.createUserId;

                                    saveInvoiceStatus(status1, orderId);
                                    #endregion

                                    using (incposdbEntities entity = new incposdbEntities())
                                    {
                                        var invoice = entity.invoices.Find(newObject.invoiceId);

                                        #region save status = Done if no shipping
                                        if (invoice.shippingCompanyId == null)
                                        {
                                            status1 = new orderPreparingStatus();
                                            status1.orderPreparingId = orderId;
                                            status1.status = "Done";
                                            status1.createUserId = status.createUserId;

                                            saveInvoiceStatus(status1, orderId);
                                        }
                                    }
                                        #endregion
                                }
                                if (res == "0")
                                    message = "0";
                            }
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
        [Route("EditPreparingOrdersPrepTime")]
        public string EditPreparingOrdersPrepTime(string token)
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
                string orderObject = "";

                decimal preparingTime = 0;
                long userId = 0;
                List<orderPreparing> preparingOrders = new List<orderPreparing>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderObject")
                    {
                        orderObject = c.Value.Replace("\\", string.Empty);
                        orderObject = orderObject.Trim('"');
                        preparingOrders = JsonConvert.DeserializeObject<List<orderPreparing>>(
                            orderObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "preparingTime")
                    {
                        preparingTime = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        foreach (orderPreparing or in preparingOrders)
                        {
                            var order = entity.orderPreparing.Find(or.orderPreparingId);
                            order.preparingTime = preparingTime;
                            order.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            order.updateUserId = userId;
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
        [Route("EditOrderListAndStatus")]
        public string EditOrderListAndStatus(string token)
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
                string orderObject = "";
                string statusObject = "";
                List<orderPreparing> preparingOrders = null;
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderObject")
                    {
                        orderObject = c.Value.Replace("\\", string.Empty);
                        orderObject = orderObject.Trim('"');
                        preparingOrders = JsonConvert.DeserializeObject<List<orderPreparing>>(
                            orderObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    foreach (orderPreparing or in preparingOrders)
                    {
                        long orderId = savePreparingOrder(or);
                        if (orderId > 0)
                        {
                            string res = saveInvoiceStatus(status, orderId);
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
        [Route("EditInvoiceOrdersStatus")]
        public string EditInvoiceOrdersStatus(string token)
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
                #region params
                long invoiceId = 0;
                long? shipUserId = 0;
                long shippingCompanyId = 0;
                string statusObject = "";
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "shipUserId")
                    {
                        try
                        {
                            shipUserId = long.Parse(c.Value);
                        }
                        catch
                        {
                            shipUserId = null;
                        }
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                    else if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region edit shipping info
                        var inv = entity.invoices.Find(invoiceId);
                        inv.shipUserId = shipUserId;
                        inv.shippingCompanyId = shippingCompanyId;
                        inv.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        entity.SaveChanges();
                        #endregion

                        #region edit orders status
                        var orders = entity.orderPreparing
                            .Where(x => x.invoiceId == invoiceId)
                            .ToList();

                        foreach (orderPreparing o in orders)
                        {
                            long orderId = o.orderPreparingId;
                            string res = saveInvoiceStatus(status, orderId);
                            if (res == "0")
                                message = "0";
                        }
                        #endregion
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
        [Route("EditInvoiceDelivery")]
        public string EditInvoiceDelivery(string token)
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
                #region params
                long invoiceId = 0;
                long? shipUserId = 0;
                long shippingCompanyId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "shipUserId")
                    {
                        try
                        {
                            shipUserId = long.Parse(c.Value);
                        }
                        catch
                        {
                            shipUserId = null;
                        }
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        #region edit shipping info
                        var inv = entity.invoices.Find(invoiceId);
                        inv.shipUserId = shipUserId;
                        inv.shippingCompanyId = shippingCompanyId;
                        inv.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        entity.SaveChanges();
                        #endregion
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
        [Route("finishInvoiceOrders")]
        public string finishInvoiceOrders(string token)
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
                long invoiceId = 0;
                long userId = 0;
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var inv = entity.invoices.Find(invoiceId);

                        #region edit orders status
                        var orders = (
                            from o in entity.orderPreparing.Where(x => x.invoiceId == invoiceId)
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

                        foreach (OrderPreparingModel o in orders)
                        {
                            long orderId = o.orderPreparingId;
                            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                            var statusObject = new orderPreparingStatus()
                            {
                                orderPreparingId = orderId,
                                isActive = 1,
                                createUserId = userId,
                                updateUserId = userId,
                                updateDate = datenow,
                                createDate = datenow,
                            };
                            switch (o.status)
                            {
                                case "Listed":
                                    statusObject.status = "Preparing";
                                    entity.orderPreparingStatus.Add(statusObject);
                                    entity.SaveChanges();

                                    statusObject.status = "Ready";
                                    entity.orderPreparingStatus.Add(statusObject);
                                    entity.SaveChanges();
                                    break;
                                case "Preparing":
                                    statusObject.status = "Ready";
                                    entity.orderPreparingStatus.Add(statusObject);
                                    entity.SaveChanges();
                                    break;
                            }
                            if (inv.shipUserId != null)
                            {
                                statusObject.status = "Collected";
                            }
                            else
                            {
                                statusObject.status = "Done";
                            }
                            entity.orderPreparingStatus.Add(statusObject);

                            entity.SaveChanges();
                            message = "1";
                        }
                        #endregion
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
        [Route("updateOrderStatus")]
        public string updateOrderStatus(string token)
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
                string statusObject = "";
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var order = entity.orderPreparing.Find(status.orderPreparingId);
                        order.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        order.updateUserId = status.createUserId;
                        entity.SaveChanges();
                    }
                    message = saveInvoiceStatus(status, (long)status.orderPreparingId);
                }
                catch
                {
                    message = "0";
                }
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("updateListOrdersStatus")]
        public string updateListOrdersStatus(string token)
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
                string statusObject = "";
                string ordersObject = "";

                orderPreparingStatus status = null;
                List<orderPreparing> preparingOrders = new List<orderPreparing>();
                ;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "statusObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        status = JsonConvert.DeserializeObject<orderPreparingStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                    else if (c.Type == "preparingOrders")
                    {
                        ordersObject = c.Value.Replace("\\", string.Empty);
                        ordersObject = ordersObject.Trim('"');
                        preparingOrders = JsonConvert.DeserializeObject<List<orderPreparing>>(
                            ordersObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        foreach (orderPreparing or in preparingOrders)
                        {
                            var order = entity.orderPreparing.Find(or.orderPreparingId);

                            order.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            order.updateUserId = status.createUserId;
                            entity.SaveChanges();

                            message = saveInvoiceStatus(status, or.orderPreparingId);
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

        private long savePreparingOrder(orderPreparing newObject)
        {
            long res = 0;
            orderPreparing tmpInvoice;
            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoiceEntity = entity.Set<orderPreparing>();
                if (newObject.orderPreparingId == 0)
                {
                    newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    newObject.updateUserId = newObject.createUserId;
                    tmpInvoice = entity.orderPreparing.Add(newObject);
                    entity.SaveChanges();
                    res = tmpInvoice.orderPreparingId;
                }
                else
                {
                    tmpInvoice = entity.orderPreparing.Find(newObject.orderPreparingId);
                    tmpInvoice.orderNum = newObject.orderNum;
                    tmpInvoice.notes = newObject.notes;
                    tmpInvoice.preparingTime = newObject.preparingTime;
                    tmpInvoice.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    tmpInvoice.updateUserId = newObject.updateUserId;
                    entity.SaveChanges();
                    res = tmpInvoice.orderPreparingId;
                }
                return res;
            }
        }

        public string savePreparingOrderItems(
            List<itemOrderPreparing> newObject,
            long orderPreparingId
        )
        {
            string message = "";
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<itemOrderPreparing> iol = entity.itemOrderPreparing
                        .Where(x => x.orderPreparingId == orderPreparingId)
                        .ToList();
                    entity.itemOrderPreparing.RemoveRange(iol);
                    entity.SaveChanges();

                    var invoice = entity.invoices.Find(orderPreparingId);
                    for (int i = 0; i < newObject.Count; i++)
                    {
                        itemOrderPreparing t;
                        if (newObject[i].createUserId == 0 || newObject[i].createUserId == null)
                        {
                            Nullable<long> id = null;
                            newObject[i].createUserId = id;
                        }

                        newObject[i].orderPreparingId = orderPreparingId;
                        newObject[i].createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject[i].updateUserId = newObject[i].createUserId;

                        t = entity.itemOrderPreparing.Add(newObject[i]);
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

        public string saveInvoiceStatus(orderPreparingStatus statusObject, long preparingOrderId)
        {
            string message = "0";
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    statusObject.orderPreparingId = preparingOrderId;
                    statusObject.isActive = 1;
                    statusObject.updateUserId = statusObject.createUserId;
                    statusObject.createDate = statusObject.updateDate = coctrlr.AddOffsetTodate(
                        DateTime.Now
                    );
                    entity.orderPreparingStatus.Add(statusObject);
                    entity.SaveChanges();
                    message = statusObject.orderStatusId.ToString();
                }
            }
            catch
            {
                message = "0";
            }
            return message;
        }

        [HttpPost]
        [Route("GetLastNumOfOrder")]
        public string GetLastNumOfOrder(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string orderCode = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderCode")
                    {
                        orderCode = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                int lastNum = GetLastNumOfOrder(orderCode, branchId);
                return TokenManager.GenerateToken(lastNum);
            }
        }

        private int GetLastNumOfOrder(string orderCode, long branchId)
        {
            List<string> numberList;
            int lastNum = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                numberList = entity.orderPreparing
                    .Where(
                        b => b.orderNum.Contains(orderCode + "-") && b.invoices.branchId == branchId
                    )
                    .Select(b => b.orderNum)
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
            return lastNum;
        }

        [HttpPost]
        [Route("GetOrdersByInvoiceId")]
        public string GetOrdersByInvoiceId(string token)
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
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var prepOrders = (
                            from o in entity.orderPreparing.Where(x => x.invoiceId == invoiceId)
                            join i in entity.itemOrderPreparing
                                on o.orderPreparingId equals i.orderPreparingId
                            // join s in entity.orderPreparingStatus on o.orderPreparingId equals s.orderPreparingId
                            join INV in entity.invoices on o.invoiceId equals INV.invoiceId
                            join WT in entity.users on INV.waiterId equals WT.userId into JWT
                            from W in JWT.DefaultIfEmpty()
                            //join ITB in entity.invoiceTables on INV.invoiceId equals ITB.invoiceId into JITB

                            //from TABL in JITB.DefaultIfEmpty()
                            //   where (s.orderStatusId == entity.orderPreparingStatus.Where(x => x.orderPreparingId == o.orderPreparingId).Max(x => x.orderStatusId))

                            select new OrderPreparingModel()
                            {
                                orderPreparingId = o.orderPreparingId,
                                invoiceId = o.invoiceId,
                                notes = o.notes,
                                orderNum = o.orderNum,
                                preparingTime = o.preparingTime,
                                updateDate = o.updateDate,
                                updateUserId = o.updateUserId,
                                createDate = o.createDate,
                                createUserId = o.createUserId,
                                itemName = i.itemsUnits.items.name,
                                quantity = (int)i.quantity,
                                // status = s.status,
                                itemUnitId = i.itemUnitId,
                                waiter = W.name + " " + W.lastname,
                                invType = INV.invType,
                                invNum = INV.invNumber,
                                branchId = INV.branchId,
                                branchName = INV.branches.name,
                                invDate = INV.invDate,
                                invTime = INV.invTime,
                                categoryId = i.itemsUnits.items.categoryId,
                                categoryCode = i.itemsUnits.items.categories.categoryCode,
                                categoryName = i.itemsUnits.items.categories.name,
                            }
                        ).OrderBy(x => new { x.categoryId, x.orderPreparingId }).ToList();

                        //   int index = 1;
                        foreach (OrderPreparingModel o in prepOrders)
                        {
                            #region get invoice tables
                            var tables = (
                                from t in entity.tables.Where(x => x.isActive == 1)
                                join it in entity.invoiceTables.Where(
                                    x => x.invoiceId == o.invoiceId
                                )
                                    on t.tableId equals it.tableId
                                select new TableModel()
                                {
                                    tableId = t.tableId,
                                    name = t.name,
                                    sectionName = t.hallSections.name,
                                    sectionId = t.hallSections.sectionId,
                                }
                            ).ToList();
                            string tablesNames = "";
                            string sectables = "";
                            foreach (TableModel tabl in tables)
                            {
                                if (tablesNames == "")
                                    tablesNames += tabl.name;
                                else
                                    tablesNames += ", " + tabl.name;

                                if (sectables == "")
                                    sectables +=
                                        (tabl.sectionId == null ? "-" : tabl.sectionName)
                                        + "/"
                                        + tabl.name;
                                else
                                    sectables +=
                                        ", "
                                        + (tabl.sectionId == null ? "-" : tabl.sectionName)
                                        + "/"
                                        + tabl.name;
                            }
                            o.tables = tablesNames;
                            o.sectionTable = sectables;

                            #endregion
                        }
                        return TokenManager.GenerateToken(prepOrders);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetOrdersforPrintByInvoiceId")]
        public string GetOrdersforPrintByInvoiceId(string token)
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
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var prepOrders = (
                            from o in entity.orderPreparing.Where(x => x.invoiceId == invoiceId)
                            join i in entity.itemOrderPreparing
                                on o.orderPreparingId equals i.orderPreparingId
                            // join s in entity.orderPreparingStatus on o.orderPreparingId equals s.orderPreparingId
                            join INV in entity.invoices on o.invoiceId equals INV.invoiceId
                            join WT in entity.users on INV.waiterId equals WT.userId into JWT
                            from W in JWT.DefaultIfEmpty()
                            //join ITB in entity.invoiceTables on INV.invoiceId equals ITB.invoiceId into JITB

                            //from TABL in JITB.DefaultIfEmpty()
                            //   where (s.orderStatusId == entity.orderPreparingStatus.Where(x => x.orderPreparingId == o.orderPreparingId).Max(x => x.orderStatusId))

                            select new OrderPreparingModel()
                            {
                                orderPreparingId = o.orderPreparingId,
                                invoiceId = o.invoiceId,
                                notes = o.notes,
                                orderNum = o.orderNum,
                                preparingTime = o.preparingTime,
                                updateDate = o.updateDate,
                                updateUserId = o.updateUserId,
                                createDate = o.createDate,
                                createUserId = o.createUserId,
                                itemName = i.itemsUnits.items.name,
                                quantity = (int)i.quantity,
                                // status = s.status,
                                itemUnitId = i.itemUnitId,
                                waiter = W.name + " " + W.lastname,
                                invType = INV.invType,
                                invNum = INV.invNumber,
                                branchId = INV.branchId,
                                branchName = INV.branches.name,
                                invDate = INV.invDate,
                                invTime = INV.invTime,
                                categoryId = i.itemsUnits.items.categoryId,
                                categoryCode = i.itemsUnits.items.categories.categoryCode,
                                categoryName = i.itemsUnits.items.categories.name,
                                items = entity.itemOrderPreparing
                                    .Where(x => x.orderPreparingId == o.orderPreparingId)
                                    .Select(
                                        x =>
                                            new itemOrderPreparingModel()
                                            {
                                                itemOrderId = x.itemOrderId,
                                                itemName = x.itemsUnits.items.name,
                                                itemId = x.itemsUnits.items.itemId,
                                                itemUnitId = x.itemUnitId,
                                                quantity = x.quantity,
                                                createDate = x.createDate,
                                                updateDate = x.updateDate,
                                                createUserId = x.createUserId,
                                                updateUserId = x.updateUserId,
                                                categoryId = x.itemsUnits
                                                    .items
                                                    .categories
                                                    .categoryId,
                                                categoryName = x.itemsUnits.items.categories.name,
                                                itemsTransId = x.itemsTransId,
                                            }
                                    )
                                    .ToList(),
                            }
                        ).OrderBy(x => new { x.categoryId, x.orderPreparingId }).ToList();

                        //   int index = 1;
                        //tables
                        foreach (OrderPreparingModel o in prepOrders)
                        {
                            #region get invoice tables
                            var tables = (
                                from t in entity.tables.Where(x => x.isActive == 1)
                                join it in entity.invoiceTables.Where(
                                    x => x.invoiceId == o.invoiceId
                                )
                                    on t.tableId equals it.tableId
                                select new TableModel()
                                {
                                    tableId = t.tableId,
                                    name = t.name,
                                    sectionName = t.hallSections.name,
                                    sectionId = t.hallSections.sectionId,
                                }
                            ).ToList();
                            string tablesNames = "";
                            string sectables = "";
                            foreach (TableModel tabl in tables)
                            {
                                if (tablesNames == "")
                                    tablesNames += tabl.name;
                                else
                                    tablesNames += ", " + tabl.name;

                                if (sectables == "")
                                    sectables +=
                                        (tabl.sectionId == null ? "-" : tabl.sectionName)
                                        + "/"
                                        + tabl.name;
                                else
                                    sectables +=
                                        ", "
                                        + (tabl.sectionId == null ? "-" : tabl.sectionName)
                                        + "/"
                                        + tabl.name;
                            }
                            o.tables = tablesNames;
                            o.sectionTable = sectables;

                            #endregion
                            #region extra items - ingredients

                            foreach (var it in o.items)
                            {
                                if (it.itemsTransId != null)
                                {
                                    long id = (long)it.itemsTransId;

                                    it.itemsIngredients = entity.itemsTransferIngredients
                                        .Where(x => x.itemsTransId == id)
                                        .Select(
                                            x =>
                                                new itemsTransferIngredientsModel()
                                                {
                                                    dishIngredId = x.dishIngredId,
                                                    isActive = x.isActive,
                                                    DishIngredientName = x.dishIngredients.name,
                                                    itemUnitId = x.itemsTransfer.itemUnitId,
                                                    itemsTransId = x.itemsTransId,
                                                    itemsTransIngredId = x.itemsTransIngredId
                                                }
                                        )
                                        .ToList();

                                    //extras
                                    it.itemExtras = (
                                        from t in entity.itemsTransfer.Where(
                                            x => x.mainCourseId == id
                                        )
                                        join u in entity.itemsUnits
                                            on t.itemUnitId equals u.itemUnitId
                                        join i in entity.items on u.itemId equals i.itemId
                                        join un in entity.units on u.unitId equals un.unitId
                                        select new ItemTransferModel()
                                        {
                                            itemsTransId = t.itemsTransId,
                                            itemId = i.itemId,
                                            itemName = i.name,
                                            quantity = t.quantity,
                                            notes = t.notes,
                                            price = t.price,
                                            unitName = un.name,
                                        }
                                    ).ToList();
                                }
                            }

                            #endregion
                        }

                        return TokenManager.GenerateToken(prepOrders);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }
    }
}
