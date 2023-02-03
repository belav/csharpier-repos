using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/WebDashBoard")]
    public class WebDashBoardController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("GetDashBoardInfo")]
        public string GetDashBoardInfo(string token)
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
                int branchId = 0;
                int userId = 0;
                DateTime nowDT = DateTime.Parse(DateTime.Now.ToString().Split(' ')[0]);
                //DateTime nowDT =   coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime dt = Convert.ToDateTime(DateTime.Today.AddHours(-24));
                //DateTime? endDate = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }
                #endregion

                try
                {
                    WebDashBoardModel dashBoardModel = new WebDashBoardModel();
                    dashBoardModel.branchId = branchId;

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var searchPredicate = PredicateBuilder.New<invoices>();

                        BranchesController bc = new BranchesController();
                        var branchesList = bc.BranchesByUser(userId);
                        var branchIds = branchesList.Select(x => x.branchId).ToList();

                        #region sales count - dining hall - self-service - takeaway
                        searchPredicate = PredicateBuilder.New<invoices>();
                        var invType = new List<string>() { "s", "ts", "ss" };
                        searchPredicate = searchPredicate.And(
                            x => x.isActive == true && invType.Contains(x.invType)
                        );

                        searchPredicate = searchPredicate.And(inv => inv.invDate >= dt);

                        if (branchId != 0)
                            searchPredicate = searchPredicate.And(x => x.branchId == branchId);
                        else if (userId != 2)
                        {
                            searchPredicate = searchPredicate.And(
                                x => branchIds.Contains((int)x.branchId)
                            );
                        }

                        var invList = entity.invoices.Where(searchPredicate).ToList();
                        dashBoardModel.salesCount = invList.Count();
                        dashBoardModel.diningHallCount = invList
                            .Where(x => x.invType == "s")
                            .Count();
                        dashBoardModel.takeawayCount = invList
                            .Where(x => x.invType == "ts")
                            .Count();
                        dashBoardModel.selfServiceCount = invList
                            .Where(x => x.invType == "ss")
                            .Count();

                        #endregion

                        #region opened tables count
                        var tsearchPredicate = PredicateBuilder.New<tables>();
                        tsearchPredicate = tsearchPredicate.And(x => x.isActive == 1);
                        if (branchId != 0)
                            tsearchPredicate = tsearchPredicate.And(x => x.branchId == branchId);
                        else if (userId != 2)
                        {
                            tsearchPredicate = tsearchPredicate.And(
                                x => branchIds.Contains((int)x.branchId)
                            );
                        }
                        var tablesList = (
                            from t in entity.tables.Where(tsearchPredicate)
                            select new TableModel()
                            {
                                tableId = t.tableId,
                                name = t.name,
                                sectionId = t.sectionId,
                                branchId = t.branchId,
                                personsCount = t.personsCount,
                                notes = t.notes,
                                createUserId = t.createUserId,
                                updateUserId = t.updateUserId,
                                createDate = t.createDate,
                                updateDate = t.updateDate,
                                isActive = t.isActive,
                                status = "empty",
                            }
                        ).ToList();

                        foreach (TableModel table in tablesList)
                        {
                            long tableId = table.tableId;

                            var searchPredicate1 = PredicateBuilder.New<invoiceTables>();

                            searchPredicate1 = searchPredicate1.And(
                                x => x.tableId == tableId && x.invoices.invType == "sd"
                            );

                            var invoiceTables = entity.invoiceTables
                                .Where(searchPredicate1)
                                .ToList();
                            if (invoiceTables.Count > 0)
                            {
                                table.status = "opened";
                            }
                        }
                        dashBoardModel.tablesCount = tablesList
                            .Where(x => x.status == "opened")
                            .Count();
                        #endregion

                        #region reservation counts
                        var resSearchPredicate = PredicateBuilder.New<reservations>();

                        resSearchPredicate = resSearchPredicate.And(
                            x => x.isActive == 1 && (x.status == null || x.status == "confirm")
                        );
                        resSearchPredicate = resSearchPredicate.And(
                            x => DbFunctions.TruncateTime(x.reservationTime) == nowDT
                        );
                        //resSearchPredicate = resSearchPredicate.And(x => x.reservationTime >= dt && x.reservationTime <= nowDT);

                        if (branchId != 0)
                            resSearchPredicate = resSearchPredicate.And(
                                x => x.branchId == branchId
                            );
                        else if (userId != 2)
                            resSearchPredicate = resSearchPredicate.And(
                                x => branchIds.Contains((int)x.branchId)
                            );

                        dashBoardModel.reservationsCount = entity.reservations
                            .Where(resSearchPredicate)
                            .ToList()
                            .Count();
                        #endregion

                        var posSearchPredicat = PredicateBuilder.New<pos>();
                        posSearchPredicat = posSearchPredicat.And(x => x.isActive == 1);

                        if (branchId != 0)
                            posSearchPredicat = posSearchPredicat.And(x => x.branchId == branchId);
                        else if (userId != 2)
                        {
                            posSearchPredicat = posSearchPredicat.And(
                                x => branchIds.Contains((int)x.branchId)
                            );
                        }

                        #region online users
                        dashBoardModel.onLineUsersCount = (
                            from log in entity.usersLogs
                            join p in entity.pos.Where(posSearchPredicat)
                                on log.posId equals p.posId
                            join u in entity.users on log.userId equals u.userId
                            where (log.sOutDate == null && log.users.isOnline == 1)
                            select new { log.userId, }
                        )
                            .Distinct()
                            .Count();
                        #endregion

                        #region balance
                        try
                        {
                            dashBoardModel.balance = (decimal)
                                entity.pos.Where(posSearchPredicat).Select(x => x.balance).Sum();
                        }
                        catch
                        {
                            dashBoardModel.balance = 0;
                        }
                        #endregion
                        return TokenManager.GenerateToken(dashBoardModel);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken(new WebDashBoardModel());
                }
            }
        }

        [HttpPost]
        [Route("getBasicSettings")]
        public string getBasicSettings(string token)
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
                    string res = "{";

                    #region accuracy
                    var accuracy = entity.setValues
                        .Where(x => x.setting.name == "accuracy")
                        .Select(x => x.value)
                        .FirstOrDefault();
                    res += "accuracy:" + accuracy + ",";
                    #endregion

                    #region currency
                    var currency = entity.countriesCodes
                        .Where(x => x.isDefault == 1)
                        .Select(
                            c =>
                                new
                                {
                                    c.countryId,
                                    c.code,
                                    c.currency,
                                    c.name,
                                    c.isDefault,
                                    c.currencyId,
                                }
                        )
                        .FirstOrDefault();

                    res += "currency:\"" + currency.currency + "\",";
                    #endregion

                    #region invoice types
                    var dinningHall = entity.setValues
                        .Where(x => x.setting.name == "typesOfService_diningHall")
                        .Select(x => x.value)
                        .FirstOrDefault();

                    if (dinningHall != null)
                        res += "typesOfService_diningHall:" + dinningHall + ",";
                    else
                        res += "typesOfService_diningHall:" + 0 + ",";

                    var takeaway = entity.setValues
                        .Where(x => x.setting.name == "typesOfService_takeAway")
                        .Select(x => x.value)
                        .FirstOrDefault();
                    if (takeaway != null)
                        res += "typesOfService_takeAway:" + takeaway + ",";
                    else
                        res += "typesOfService_takeAway:" + 0 + ",";

                    var selfService = entity.setValues
                        .Where(x => x.setting.name == "typesOfService_selfService")
                        .Select(x => x.value)
                        .FirstOrDefault();
                    if (selfService != null)
                        res += "typesOfService_selfService:" + selfService;
                    else
                        res += "typesOfService_selfService:" + 0;

                    #endregion
                    res += "}";
                    return TokenManager.GenerateToken(res);
                }
            }
        }

        [HttpPost]
        [Route("getUserLanguage")]
        public string getUserLanguage(string token)
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
                int userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var lang = (
                        from sv in entity.setValues.Where(x => x.setting.name == "language")
                        join su in entity.userSetValues.Where(x => x.userId == userId)
                            on sv.valId equals su.valId
                        select new setValuesModel() { value = sv.value, name = sv.setting.name, }
                    ).FirstOrDefault();

                    if (lang != null)
                        return TokenManager.GenerateToken(lang.value);
                    else
                        return TokenManager.GenerateToken("en");
                }
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
                int branchId = 0;
                int userId = 0;

                List<string> invTypeL = new List<string>();

                DateTime dt = Convert.ToDateTime(DateTime.Now.AddHours(-24).ToShortDateString());

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invType")
                    {
                        invType = c.Value;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoices>();

                    searchPredicate = searchPredicate.And(
                        inv => inv.isActive == true && inv.invType == invType
                    );

                    if (branchId != 0)
                        searchPredicate = searchPredicate.And(inv => inv.branchId == branchId);
                    else if (userId != 2)
                    {
                        BranchesController bc = new BranchesController();
                        var branchesList = bc.BranchesByUser(userId);
                        var branchIds = branchesList.Select(x => x.branchId).ToList();

                        searchPredicate = searchPredicate.And(
                            x => branchIds.Contains((int)x.branchId)
                        );
                    }

                    searchPredicate = searchPredicate.And(inv => inv.invDate >= dt);

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
                            itemsCount = entity.itemsTransfer
                                .Where(m => m.invoiceId == b.invoiceId)
                                .ToList()
                                .Count(),
                        }
                    ).ToList();

                    int sequence = 1;
                    foreach (var inv in invoicesList)
                    {
                        inv.invoiceId = sequence;
                        sequence++;
                    }

                    return TokenManager.GenerateToken(invoicesList);
                }
            }
        }

        [HttpPost]
        [Route("getDayReservtions")]
        public string getDayReservtions(string token)
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
                int branchId = 0;
                int userId = 0;

                List<string> invTypeL = new List<string>();
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime nowDT = DateTime.Parse(datenow.ToString().Split(' ')[0]);

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var resSearchPredicate = PredicateBuilder.New<reservations>();

                    resSearchPredicate = resSearchPredicate.And(
                        x => x.isActive == 1 && (x.status == null || x.status == "confirm")
                    );
                    resSearchPredicate = resSearchPredicate.And(
                        x => DbFunctions.TruncateTime(x.reservationTime) == nowDT
                    );

                    if (branchId != 0)
                        resSearchPredicate = resSearchPredicate.And(x => x.branchId == branchId);
                    else if (userId != 2)
                    {
                        BranchesController bc = new BranchesController();
                        var branchesList = bc.BranchesByUser(userId);
                        var branchIds = branchesList.Select(x => x.branchId).ToList();

                        resSearchPredicate = resSearchPredicate.And(
                            x => branchIds.Contains((int)x.branchId)
                        );
                    }

                    var reservations = (
                        from rs in entity.reservations.Where(resSearchPredicate)
                        join a in entity.agents on rs.customerId equals a.agentId into cuj
                        from c in cuj.DefaultIfEmpty()
                        select new ReservationModel()
                        {
                            reservationId = rs.reservationId,
                            code = rs.code,
                            customerId = rs.customerId,
                            customerName = c.name,
                            reservationDate = rs.reservationDate,
                            reservationTime = rs.reservationTime,
                            endTime = rs.endTime,
                            personsCount = rs.personsCount,
                            notes = rs.notes,
                            createUserId = rs.createUserId,
                            updateUserId = rs.updateUserId,
                            createDate = rs.createDate,
                            updateDate = rs.updateDate,
                            isActive = rs.isActive,
                            status = rs.status,
                        }
                    ).ToList();

                    int seq = 1;
                    foreach (var res in reservations)
                    {
                        res.sequence = seq;
                        seq++;
                    }
                    return TokenManager.GenerateToken(reservations);
                }
            }
        }

        [HttpPost]
        [Route("getReservtionById")]
        public string getReservtionById(string token)
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
                int reservationId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "reservationId")
                    {
                        reservationId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var reservation = (
                        from rs in entity.reservations.Where(x => x.reservationId == reservationId)
                        select new ReservationModel()
                        {
                            reservationId = rs.reservationId,
                            code = rs.code,
                            branchId = rs.branchId,
                            customerId = rs.customerId,
                            reservationDate = rs.reservationDate,
                            reservationTime = rs.reservationTime,
                            endTime = rs.endTime,
                            personsCount = rs.personsCount,
                            notes = rs.notes,
                            createUserId = rs.createUserId,
                            updateUserId = rs.updateUserId,
                            createDate = rs.createDate,
                            updateDate = rs.updateDate,
                            isActive = rs.isActive,
                            status = rs.status,
                            tables = (
                                from tr in rs.tablesReservations.Where(
                                    x => x.reservationId == rs.reservationId
                                )
                                join ts in entity.tables on tr.tableId equals ts.tableId
                                select new TableModel()
                                {
                                    tableId = ts.tableId,
                                    name = ts.name,
                                    personsCount = ts.personsCount,
                                    canDelete = false,
                                    isActive = ts.isActive
                                }
                            ).ToList(),
                        }
                    ).FirstOrDefault();

                    return TokenManager.GenerateToken(reservation);
                }
            }
        }

        [HttpPost]
        [Route("getReservationTables")]
        public string getReservationTables(string token)
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
                int reservationId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "reservationId")
                    {
                        reservationId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var tables = (
                        from tr in entity.tablesReservations.Where(
                            x => x.reservationId == reservationId
                        )
                        join ts in entity.tables on tr.tableId equals ts.tableId
                        select new TableModel()
                        {
                            tableId = ts.tableId,
                            name = ts.name,
                            personsCount = ts.personsCount,
                            canDelete = false,
                            isActive = ts.isActive
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(tables);
                }
            }
        }

        [HttpPost]
        [Route("confirmReservation")]
        public async Task<string> confirmReservation(string token)
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
                int reservationId = 0;
                int userId = 0;
                string invoiceObject = "";
                invoices newObject = null;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "reservationId")
                    {
                        reservationId = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
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
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    #region edit reservation status
                    var res = entity.reservations.Find(reservationId);
                    res.status = "confirm";
                    res.updateUserId = userId;
                    res.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    entity.SaveChanges();
                    #endregion
                    #region createInvoice and tables
                    InvoicesController ic = new InvoicesController();
                    long invoiceId = await ic.saveInvoice(newObject);
                    if (invoiceId > 0)
                    {
                        var tables = (
                            from tr in entity.tablesReservations.Where(
                                x => x.reservationId == reservationId
                            )
                            join ts in entity.tables on tr.tableId equals ts.tableId
                            select new TableModel()
                            {
                                tableId = ts.tableId,
                                name = ts.name,
                                personsCount = ts.personsCount,
                                isActive = ts.isActive,
                            }
                        ).ToList();

                        foreach (TableModel t in tables)
                        {
                            invoiceTables tr = new invoiceTables();

                            var tableEntity = entity.Set<tablesReservations>();

                            tr.invoiceId = invoiceId;
                            tr.tableId = t.tableId;
                            tr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tr.updateDate = tr.createDate;
                            tr.isActive = 1;
                            tr.updateUserId = userId;
                            tr.createUserId = userId;

                            tr = entity.invoiceTables.Add(tr);
                            entity.SaveChanges();
                        }
                    }
                    #endregion
                    return TokenManager.GenerateToken("1");
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
                int userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
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

                        searchPredicate = searchPredicate.And(
                            x => x.invType == "ts" || x.invType == "ss"
                        );
                        searchPredicate = searchPredicate.And(
                            x => x.shipUserId == userId && x.deserved > 0
                        );

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
                                itemsCount = entity.itemsTransfer
                                    .Where(x => x.invoiceId == b.invoiceId)
                                    .Count(),
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

                        int seq = 1;
                        foreach (var inv in invoices)
                        {
                            inv.sequence = seq;
                            seq++;
                        }
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
                int invoiceId = 0;
                string statusObject = "";
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invoiceId = int.Parse(c.Value);
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

        private string saveInvoiceStatus(orderPreparingStatus statusObject, long preparingOrderId)
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
        [Route("GetPreparingOrderDetails")]
        public string GetPreparingOrderDetails(string token)
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
                int orderId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderId")
                    {
                        orderId = int.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var prepOrder = (
                            from o in entity.orderPreparing.Where(
                                x => x.orderPreparingId == orderId
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
                                                categoryName = x.itemsUnits.items.categories.name,
                                                itemsTransId = x.itemsTransId,
                                            }
                                    )
                                    .ToList(),
                                status = s.status,
                            }
                        ).FirstOrDefault();

                        #region preparing time from menu list
                        if (prepOrder.status == "Listed")
                        {
                            if (prepOrder.preparingTime == null || prepOrder.preparingTime == 0)
                            {
                                var orderItemUnits = entity.itemOrderPreparing
                                    .Where(x => x.orderPreparingId == prepOrder.orderPreparingId)
                                    .Select(x => x.itemUnitId)
                                    .ToList();
                                prepOrder.preparingTime = entity.menuSettings
                                    .Where(x => orderItemUnits.Contains(x.itemUnitId))
                                    .Select(x => x.preparingTime)
                                    .Max();
                            }
                        }
                        #endregion


                        #region get invoice tables
                        var tables = (
                            from t in entity.tables.Where(x => x.isActive == 1)
                            join it in entity.invoiceTables.Where(
                                x => x.invoiceId == prepOrder.invoiceId
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
                        prepOrder.tables = tablesNames;
                        #endregion

                        #region preparing status date
                        if (prepOrder.status == "Listed")
                            prepOrder.preparingStatusDate = null;
                        else
                        {
                            DateTime createDate = (DateTime)
                                entity.orderPreparingStatus
                                    .Where(
                                        x =>
                                            x.orderPreparingId == prepOrder.orderPreparingId
                                            && x.status == "Preparing"
                                    )
                                    .Select(x => x.createDate)
                                    .SingleOrDefault();
                            prepOrder.preparingStatusDate = (DateTime)createDate;
                        }
                        #endregion

                        // set sequence num to items
                        int index = 1;
                        foreach (itemOrderPreparingModel item in prepOrder.items)
                        {
                            if (item.itemsTransId != null)
                            {
                                long id = (long)item.itemsTransId;

                                item.itemsIngredients = entity.itemsTransferIngredients
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
                                item.itemExtras = (
                                    from t in entity.itemsTransfer.Where(x => x.mainCourseId == id)
                                    join u in entity.itemsUnits on t.itemUnitId equals u.itemUnitId
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

                            item.sequence = index;
                            index++;
                        }

                        return TokenManager.GenerateToken(prepOrder);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
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
                int orderId = 0;
                decimal preparingTime = 0;
                int userId = 0;
                string notes = "";
                List<orderPreparing> preparingOrders = new List<orderPreparing>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderId")
                    {
                        orderId = int.Parse(c.Value);
                    }
                    else if (c.Type == "preparingTime")
                    {
                        preparingTime = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "notes")
                    {
                        notes = c.Value;
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var order = entity.orderPreparing.Find(orderId);
                        order.preparingTime = preparingTime;
                        order.notes = notes;
                        order.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        order.updateUserId = userId;

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
        [Route("EditOrderStatus")]
        public string EditOrderStatus(string token)
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
                int orderId = 0;
                string statusObject = "";
                orderPreparingStatus status = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "orderId")
                    {
                        orderId = int.Parse(c.Value);
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
                        #region edit orders status

                        string res = saveInvoiceStatus(status, orderId);
                        if (res == "0")
                            message = "0";
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
        [Route("GetPermissions")]
        public string GetPermissions(string token)
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
                int userId = 0;
                string dashBoardPermission = "dashboard";
                string salesPermission = "diningHall";
                string reservationPermission = "reservationsUpdate_update";
                string kitchenPermission = "preparingOrders";
                string deliveryJob = "deliveryEmployee";

                string result = "";

                JArray jArray = new JArray();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                }
                #endregion
                using (incposdbEntities entity = new incposdbEntities())
                {
                    result += "{";
                    #region dashBoard Permission
                    var groupObjects = (
                        from GO in entity.groupObject
                        where GO.showOb == 1 && GO.objects.name.Contains(dashBoardPermission)
                        join U in entity.users on GO.groupId equals U.groupId
                        where U.userId == userId
                        select new { GO.id, GO.showOb, }
                    ).FirstOrDefault();

                    result += "showDashBoard:";
                    if (groupObjects != null)
                    {
                        result += "true,";
                    }
                    else
                    {
                        result += "false,";
                    }
                    #endregion
                    #region sales Permission
                    groupObjects = (
                        from GO in entity.groupObject
                        where GO.showOb == 1 && GO.objects.name.Contains(salesPermission)
                        join U in entity.users on GO.groupId equals U.groupId
                        where U.userId == userId
                        select new { GO.id, GO.showOb, }
                    ).FirstOrDefault();

                    result += "showSales:";
                    if (groupObjects != null)
                    {
                        result += "true,";
                    }
                    else
                    {
                        result += "false,";
                    }
                    #endregion
                    #region reservations Permission
                    groupObjects = (
                        from GO in entity.groupObject
                        where GO.showOb == 1 && GO.objects.name.Contains(reservationPermission)
                        join U in entity.users on GO.groupId equals U.groupId
                        where U.userId == userId
                        select new { GO.id, GO.showOb, }
                    ).FirstOrDefault();

                    result += "showReservations:";
                    if (groupObjects != null)
                    {
                        result += "true,";
                    }
                    else
                    {
                        result += "false,";
                    }
                    #endregion
                    #region kitchen Permission
                    groupObjects = (
                        from GO in entity.groupObject
                        where GO.showOb == 1 && GO.objects.name.Contains(kitchenPermission)
                        join U in entity.users on GO.groupId equals U.groupId
                        where U.userId == userId
                        select new { GO.id, GO.showOb, }
                    ).FirstOrDefault();

                    result += "showKitchen:";
                    if (groupObjects != null)
                    {
                        result += "true,";
                    }
                    else
                    {
                        result += "false,";
                    }
                    #endregion
                    #region delivery Permission
                    var deliverUser = (
                        from u in entity.users.Where(
                            u => u.userId == userId && u.isActive == 1 && u.driverIsAvailable == 1
                        )
                        select new UserModel { userId = u.userId, username = u.username, }
                    ).FirstOrDefault();

                    result += "showDelivery:";
                    if (deliverUser != null)
                    {
                        result += "true";
                    }
                    else
                    {
                        result += "false";
                    }

                    #endregion
                    result += "}";
                    return TokenManager.GenerateToken(result);
                }
            }
        }

        [HttpPost]
        [Route("getUserDeliverOrders")]
        public string getUserDeliverOrders(string token)
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
                string status = "";
                int shipUserId = 0;
                List<string> invTypeL = new List<string>();
                List<string> statusL = new List<string>();
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
                    else if (c.Type == "status")
                    {
                        status = c.Value;
                        string[] statusArray = status.Split(',');
                        statusL.AddRange(statusArray);
                    }
                    else if (c.Type == "userId")
                    {
                        shipUserId = int.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<InvoiceModel> lst = new List<InvoiceModel>();

                    foreach (string st in statusL)
                    {
                        var invoicesList = (
                            from b in entity.invoices.Where(
                                x =>
                                    invTypeL.Contains(x.invType)
                                    && x.shipUserId == shipUserId
                                    && x.isActive == true
                                    && x.deserved > 0
                            )
                            join s in entity.invoiceStatus on b.invoiceId equals s.invoiceId
                            where
                                (
                                    s.status == st
                                    && s.invoiceId == b.invoiceId
                                    && s.invStatusId
                                        == entity.invoiceStatus
                                            .Where(x => x.invoiceId == b.invoiceId)
                                            .Max(x => x.invStatusId)
                                )
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
                                shippingCost = b.shippingCost,
                                realShippingCost = b.realShippingCost,
                                status = s.status,
                                itemsCount = entity.itemsTransfer
                                    .Where(x => x.invoiceId == b.invoiceId)
                                    .Select(x => x.itemsTransId)
                                    .ToList()
                                    .Count,
                            }
                        ).ToList();

                        if (invoicesList.Count > 0)
                            lst.AddRange(invoicesList);
                    }

                    lst = lst.OrderBy(x => x.status).ThenBy(x => x.invDate).ToList();

                    int sequence = 1;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        //lst[i].sequence = sequence;
                        sequence++;
                    }
                    return TokenManager.GenerateToken(lst);
                }
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id) { }
    }
}
