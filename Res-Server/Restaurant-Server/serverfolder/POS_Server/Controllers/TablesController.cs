using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Tables")]
    public class TablesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        List<string> reservationClose = new List<string>() { "close", "cancle" };
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            Boolean canDelete = false;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                long sectionId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "sectionId")
                    {
                        sectionId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<tables>();
                    searchPredicate = searchPredicate.And(x => true);
                    if (branchId != 0)
                        searchPredicate.And(x => x.branchId == branchId);
                    if (sectionId != 0)
                        searchPredicate.And(x => x.sectionId == sectionId);
                    var tablesList = entity.tables.Where(searchPredicate).Select(S => new TableModel()
                    {
                        tableId = S.tableId,
                        name = S.name,
                        sectionId = S.sectionId,
                        branchId = S.branchId,
                        status = S.status,
                        personsCount = S.personsCount,
                        notes = S.notes,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        isActive = S.isActive,
                        sectionName = S.hallSections.name,
                        branchName = S.branches.name,

                    }).ToList();

                    // can delet or not
                    if (tablesList.Count > 0)
                    {
                        foreach (TableModel item in tablesList)
                        {
                            canDelete = false;
                            if (item.isActive == 1)
                            {
                                long cId = (long)item.tableId;
                                var invTable = entity.invoiceTables.Where(x => x.tableId == cId).FirstOrDefault();
                                var reservTable = entity.tablesReservations.Where(x => x.tableId == cId).FirstOrDefault();

                                if (invTable is null && reservTable is null)
                                    canDelete = true;
                            }
                            item.canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(tablesList);
                }
            }
        }
        [HttpPost]
        [Route("GetTablesStatusInfo")]
        public string GetTablesStatusInfo(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                long branchId = 0;
                DateTime dateSearch = DateTime.Parse(datenow.ToString().Split(' ')[0]);
                DateTime startTimeSearch = datenow;
                DateTime endTimeSearch = new DateTime();
                DateTime startDate = datenow;
                DateTime endDate = datenow;
                Boolean searchForStartTime = false;
                Boolean searchForEndTime = false;
                Boolean searchForDate = false;
                #region parameters
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "dateSearch")
                    {
                       
                        if (c.Value != "")
                        {
                            searchForDate = true;
                            dateSearch = DateTime.Parse(c.Value);
                            if (dateSearch == null)
                                dateSearch = DateTime.Parse(datenow.ToString().Split(' ')[0]);
                        }
                    }
                    else if (c.Type == "startTimeSearch")
                    {
                        if (c.Value != "")
                        {
                            searchForStartTime = true;
                            startDate = DateTime.Parse(c.Value);
                            startTimeSearch = startDate;

                        }
                    }
                    else if (c.Type == "endTimeSearch")
                    {
                        if (c.Value != "")
                        {
                            searchForEndTime = true;
                            endDate = DateTime.Parse(c.Value);
                            if (endDate != null)
                                endTimeSearch = endDate;
                        }
                    }
                }
                #endregion
                // return startTimeSearch.ToString();
                using (incposdbEntities entity = new incposdbEntities())
                {
                    #region get time staying
                    var timeStayingSet = entity.setValues.Where(x => x.setting.name == "time_staying").Select(x => x.value).SingleOrDefault();
                    double decTimeStaying = 0;
                    try
                    {
                        decTimeStaying = double.Parse(timeStayingSet);
                    }
                    catch { }
                    TimeSpan timeStaying = TimeSpan.FromHours(decTimeStaying);
                    #endregion                   

                    var tablesList = (from t in entity.tables.Where(x => x.isActive == 1 && x.branchId == branchId)
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
                                      }).ToList();

                    foreach (TableModel table in tablesList)
                    {
                        #region check reservation status                      
                        long tableId = table.tableId;
                        bool isOpen = false;
                        var reservPredicate = PredicateBuilder.New<reservations>();

                        if (searchForDate)
                            reservPredicate = reservPredicate.And(x => x.reservationDate == dateSearch && !reservationClose.Contains(x.status) && x.isActive == 1);
                        else
                            reservPredicate = reservPredicate.And(x => x.reservationDate >= dateSearch && !reservationClose.Contains(x.status) && x.isActive == 1);

                        var reservation = (from rs in entity.reservations.Where(reservPredicate)
                                           join tr in entity.tablesReservations.Where(x => x.tableId == tableId) on rs.reservationId equals tr.reservationId
                                           select new ReservationModel()
                                           {
                                               reservationId = rs.reservationId,
                                               code = rs.code,
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
                                           }).ToList().OrderBy(x => x.reservationDate).ThenBy(x => x.reservationTime);

                        foreach (ReservationModel reserv in reservation)
                        {
                            if (searchForEndTime && searchForStartTime)
                            {
                                if (startTimeSearch >= reserv.reservationTime && endTimeSearch <= reserv.endTime)
                                {
                                    table.status = "reserved";
                                }
                            }
                            else if (searchForEndTime)
                            {
                                if (endTimeSearch <= reserv.endTime && DateTime.Parse(reserv.endTime.ToString().Split(' ')[0]) == dateSearch)
                                {
                                    table.status = "reserved";
                                }
                            }
                            else
                            if (startTimeSearch >= reserv.reservationTime)
                            {
                                table.status = "reserved";
                            }

                            // check if table is open
                            var invoice = entity.invoices.Where(x => x.reservationId == reserv.reservationId && x.invType == "sd" && x.isActive == true).FirstOrDefault();
                            if (invoice != null)
                                isOpen = true;
                        }
                        if (isOpen && reservation.Count() > 0)
                            table.status = "openedReserved";

                        #endregion
                        #region check opened tables without reservation

                        var searchPredicate = PredicateBuilder.New<invoiceTables>();
                        if (searchForDate)
                            searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.invoices.invDate == dateSearch && x.invoices.invType == "sd" && x.isActive == 1);
                        else
                            searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.invoices.invDate >= dateSearch && x.invoices.invType == "sd" && x.isActive == 1);

                        var invoiceTables = entity.invoiceTables.Where(searchPredicate).ToList();

                        foreach (invoiceTables invTable in invoiceTables)
                        {
                            var invoice = entity.invoices.Find(invTable.invoiceId);
                            DateTime invTime = (DateTime)invoice.invDate;
                            // return startTimeSearch.ToString() + "  " + invTime.ToString() + "   " + invTime.Add(timeStaying).ToString();

                            if (searchForEndTime)
                            {
                                if (invTime <= startTimeSearch && invTime.Add(timeStaying) <= endTimeSearch)
                                {
                                    if (table.status == "empty")
                                        table.status = "opened";
                                    else if (table.status == "reserved")
                                        table.status = "openedReserved";
                                }
                            }
                            else if (invTime <= startTimeSearch && startTimeSearch <= invTime.Add(timeStaying))
                            {

                                if (table.status == "empty")
                                    table.status = "opened";
                                else if (table.status == "reserved")
                                    table.status = "openedReserved";
                            }
                        }
                        #endregion
                    }
                    return TokenManager.GenerateToken(tablesList);
                }
            }
        }
        [HttpPost]
        [Route("GetTablesForDinning")]
        public string GetTablesForDinning(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                long branchId = 0;
                DateTime dateSearch = DateTime.Parse(datenow.ToString().Split(' ')[0]);
                DateTime startTime = datenow;

                #region parameters
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "dateSearch")
                    {
                        dateSearch = DateTime.Parse(c.Value);
                        startTime = dateSearch;
                        dateSearch = DateTime.Parse(dateSearch.ToString().Split(' ')[0]);
                    }
                }
                #endregion
                // return startTimeSearch.ToString();
                using (incposdbEntities entity = new incposdbEntities())
                {
                    #region get time staying
                    var timeStayingSet = entity.setValues.Where(x => x.setting.name == "time_staying").Select(x => x.value).SingleOrDefault();
                    double decTimeStaying = 0;
                    try
                    {
                        decTimeStaying = double.Parse(timeStayingSet);
                    }
                    catch { }
                    TimeSpan timeStaying = TimeSpan.FromHours(decTimeStaying);
                    #endregion                   

                    var tablesList = (from t in entity.tables.Where(x => x.isActive == 1 && x.branchId == branchId)
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
                                      }).ToList();

                    foreach (TableModel table in tablesList)
                    {
                        #region check reservation status                      
                        long tableId = table.tableId;
                        bool isOpen = false;

                        var reservPredicate = PredicateBuilder.New<reservations>();

                        //reservPredicate = reservPredicate.And(x => x.reservationDate >= dateSearch && !reservationClose.Contains(x.status));
                        reservPredicate = reservPredicate.And(x => x.isActive == 1 && !reservationClose.Contains(x.status));

                        var reservation = (from rs in entity.reservations.Where(reservPredicate)
                                           join tr in entity.tablesReservations.Where(x => x.tableId == tableId) on rs.reservationId equals tr.reservationId
                                           select new ReservationModel()
                                           {
                                               reservationId = rs.reservationId,
                                               code = rs.code,
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
                                           }).ToList().OrderBy(x => x.reservationDate).ThenBy(x => x.reservationTime).ToList();

                        foreach (ReservationModel reserv in reservation)
                        {
                            //return startTime.ToString() + " --" + reserv.reservationTime.ToString() + "---" + reserv.endTime.ToString();
                            //if (startTime >= reserv.reservationTime && startTime <= reserv.endTime)
                            {
                                table.status = "reserved";
                            }

                            // check if table is open
                            var invoice = entity.invoices.Where(x => x.reservationId == reserv.reservationId && x.invType == "sd" && x.isActive == true).FirstOrDefault();
                            if (invoice != null)
                                isOpen = true;
                        }
                        if (isOpen && reservation.Count() > 0)
                            table.status = "openedReserved";

                        #endregion
                        #region check opened tables without reservation
                        var searchPredicate = PredicateBuilder.New<invoiceTables>();

                        searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.invoices.invType == "sd");

                        var invoiceTables = entity.invoiceTables.Where(searchPredicate).ToList();
                        if (invoiceTables.Count > 0)
                        {
                            if (table.status == "empty")
                                table.status = "opened";
                            else if (table.status == "reserved")
                                table.status = "openedReserved";
                        }
                        #endregion
                    }
                    return TokenManager.GenerateToken(tablesList);
                }
            }
        }
        [HttpPost]
        [Route("GetTablesStatistics")]
        public string GetTablesStatistics(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime dateSearch = DateTime.Parse(datenow.ToString().Split(' ')[0]);
                DateTime startTime = datenow;

                #region parameters
                long mainBranchId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "dateSearch")
                    {
                        dateSearch = DateTime.Parse(c.Value);
                        startTime = dateSearch;
                        dateSearch = DateTime.Parse(dateSearch.ToString().Split(' ')[0]);
                    }
                    else if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                #endregion
                // return startTimeSearch.ToString();
                using (incposdbEntities entity = new incposdbEntities())
                {
                    #region get time staying
                    var timeStayingSet = entity.setValues.Where(x => x.setting.name == "time_staying").Select(x => x.value).SingleOrDefault();
                    double decTimeStaying = 0;
                    try
                    {
                        decTimeStaying = double.Parse(timeStayingSet);
                    }
                    catch { }
                    TimeSpan timeStaying = TimeSpan.FromHours(decTimeStaying);
                    #endregion                   

                    var tablesList = (from t in entity.tables.Where(x => x.isActive == 1)
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
                                      }).ToList();

                    foreach (TableModel table in tablesList)
                    {
                        #region check reservation status                      
                        long tableId = table.tableId;
                        bool isOpen = false;

                        var reservPredicate = PredicateBuilder.New<reservations>();

                        //reservPredicate = reservPredicate.And(x => x.reservationDate >= dateSearch && !reservationClose.Contains(x.status));
                        reservPredicate = reservPredicate.And(x => x.isActive == 1 && !reservationClose.Contains(x.status));

                        var reservation = (from rs in entity.reservations.Where(reservPredicate)
                                           join tr in entity.tablesReservations.Where(x => x.tableId == tableId) on rs.reservationId equals tr.reservationId
                                           select new ReservationModel()
                                           {
                                               reservationId = rs.reservationId,
                                               code = rs.code,
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
                                           }).ToList().OrderBy(x => x.reservationDate).ThenBy(x => x.reservationTime).ToList();

                        foreach (ReservationModel reserv in reservation)
                        {
                            //return startTime.ToString() + " --" + reserv.reservationTime.ToString() + "---" + reserv.endTime.ToString();
                            //if (startTime >= reserv.reservationTime && startTime <= reserv.endTime)
                            {
                                table.status = "reserved";
                            }

                            // check if table is open
                            var invoice = entity.invoices.Where(x => x.reservationId == reserv.reservationId && x.invType == "sd" && x.isActive == true).FirstOrDefault();
                            if (invoice != null)
                                isOpen = true;
                        }
                        if (isOpen && reservation.Count() > 0)
                            table.status = "openedReserved";

                        #endregion
                        #region check opened tables without reservation
                        var searchPredicate = PredicateBuilder.New<invoiceTables>();

                        searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.invoices.invType == "sd");

                        var invoiceTables = entity.invoiceTables.Where(searchPredicate).ToList();
                        if (invoiceTables.Count > 0)
                        {
                            if (table.status == "empty")
                                table.status = "opened";
                            else if (table.status == "reserved")
                                table.status = "openedReserved";
                        }
                        #endregion
                    }

                    var branches = entity.branches.Where(x => x.isActive == 1).ToList();

                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    branches = branches.Where(X => brIds.Contains(X.branchId)).ToList();

                    List<TablesStatisticsModel> tablesStatistics = new List<TablesStatisticsModel>();

                    foreach (branches br in branches)
                    {
                        var table = new TablesStatisticsModel()
                        {
                            branchId = br.branchId,
                            branchName = br.name,
                            openedCount = tablesList.Where(x => x.branchId == br.branchId && (x.status == "opened" || x.status == "openedReserved")).ToList().Count(),
                            emptyCount = tablesList.Where(x => x.branchId == br.branchId && x.status != "opened" && x.status != "openedReserved").ToList().Count(),
                            reservedCount = tablesList.Where(x => x.branchId == br.branchId && x.status == "reserved").ToList().Count(),
                        };
                        tablesStatistics.Add(table);
                    }
                    return TokenManager.GenerateToken(tablesStatistics);
                }
            }
        }
        [HttpPost]
        [Route("checkTableAvailabiltiy")]
        public string checkTableAvailabiltiy(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                long tableId = 0;
                long branchId = 0;
                long reservationId = 0;
                long invoiceId = 0;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime dateSearch = DateTime.Parse(datenow.ToString().Split(' ')[0]);
                DateTime startDate = datenow;
                DateTime endDate = datenow;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "reservationId")
                    {
                        reservationId = long.Parse(c.Value);
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "reservationDate")
                    {
                        if (c.Value != "")
                        {
                            dateSearch = DateTime.Parse(c.Value);
                            dateSearch = DateTime.Parse(dateSearch.ToString().Split(' ')[0]);
                        }
                    }
                    else if (c.Type == "startTime")
                    {
                        if (c.Value != "")
                        {
                            startDate = DateTime.Parse(c.Value);
                        }
                    }
                    else if (c.Type == "endTime")
                    {
                        if (c.Value != "")
                        {
                            endDate = DateTime.Parse(c.Value);
                        }
                    }
                }
                //return dateSearch.ToString() + "dddd" + startDate.ToString() + "dddd" + endDate.ToString();
                using (incposdbEntities entity = new incposdbEntities())
                {
                    #region get time staying
                    var timeStayingSet = entity.setValues.Where(x => x.setting.name == "time_staying").Select(x => x.value).SingleOrDefault();
                    double decTimeStaying = 0;
                    try
                    {
                        decTimeStaying = double.Parse(timeStayingSet);
                    }
                    catch { }
                    TimeSpan timeStaying = TimeSpan.FromHours(decTimeStaying);

                    #endregion
                    #region check reservation status                      
                    var reservPredicate = PredicateBuilder.New<reservations>();
                    reservPredicate = reservPredicate.And(x => DbFunctions.TruncateTime(x.reservationDate) == dateSearch && x.branchId == branchId
                                                            && !reservationClose.Contains(x.status) && x.isActive == 1);
                    if (reservationId != 0)
                        reservPredicate = reservPredicate.And(x => x.reservationId != reservationId);

                    var reservation = (from r in entity.reservations.Where(reservPredicate)
                                       join tr in entity.tablesReservations.Where(x => x.tableId == tableId)
                                        on r.reservationId equals tr.reservationId
                                       select new ReservationModel()
                                       {
                                           reservationId = r.reservationId,
                                           code = r.code,
                                           customerId = r.customerId,
                                           reservationDate = r.reservationDate,
                                           reservationTime = r.reservationTime,
                                           endTime = r.endTime,
                                           personsCount = r.personsCount,
                                           notes = r.notes,
                                           createUserId = r.createUserId,
                                           updateUserId = r.updateUserId,
                                           createDate = r.createDate,
                                           updateDate = r.updateDate,
                                           isActive = r.isActive,
                                       }).ToList();

                    foreach (ReservationModel reserv in reservation)
                    {
                        //return "start date: " + startDate + "end date: " + endDate.ToString() + "start time: " + reserv.reservationTime + "end time: " + reserv.endTime;
                        if ((startDate >= reserv.reservationTime && startDate <= reserv.endTime) || (endDate >= reserv.reservationTime && endDate <= reserv.endTime))
                        {
                            return TokenManager.GenerateToken("0");
                        }
                    }
                    #endregion
                    #region check opened tables without reservation
                    var searchPredicate = PredicateBuilder.New<invoiceTables>();
                    searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.createDate >= dateSearch && x.invoices.invType == "sd");

                    if (invoiceId != 0)
                        searchPredicate = searchPredicate.And(x => x.invoiceId != invoiceId);

                    var invoiceTables = entity.invoiceTables.Where(searchPredicate).ToList();

                    foreach (invoiceTables invTable in invoiceTables)
                    {
                        var invoice = entity.invoices.Find(invTable.invoiceId);
                        DateTime invTime = (DateTime)invoice.invDate;

                        if ((startDate <= invTime.Add(timeStaying) && startDate >= invTime) || (invTime.Add(timeStaying) <= endDate && endDate >= invTime))
                        {
                            return TokenManager.GenerateToken("0");

                        }
                    }
                    #endregion
                    return TokenManager.GenerateToken("1");

                }
            }
        }
        [HttpPost]
        [Route("checkOpenedTable")]
        public string checkOpenedTable(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long tableId = 0;
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<invoiceTables>();
                    searchPredicate = searchPredicate.And(x => x.tableId == tableId && x.isActive == 1 && x.invoices.invType == "sd");

                    var invoiceTables = entity.invoiceTables.Where(searchPredicate).ToList();
                    if (invoiceTables.Count > 0)
                        return TokenManager.GenerateToken("0");
                    else
                        return TokenManager.GenerateToken("1");

                }
            }
        }
        [HttpPost]
        [Route("GetReservations")]
        public string GetReservations(string token)
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

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var reservations = (from rs in entity.reservations.Where(x => x.branchId == branchId && !reservationClose.Contains(x.status))
                                        join cu in entity.agents on rs.customerId equals cu.agentId into cuj
                                        from c in cuj.DefaultIfEmpty()
                                        where !entity.invoices.Any(m => m.isActive == true && m.reservationId == rs.reservationId)
                                        select new ReservationModel()
                                        {
                                            reservationId = rs.reservationId,
                                            code = rs.code,
                                            customerId = rs.customerId,
                                            customerName = c.name,
                                            branchId = rs.branchId,
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
                                            tables = (from tr in rs.tablesReservations.Where(x => x.reservationId == rs.reservationId)
                                                      join ts in entity.tables on tr.tableId equals ts.tableId
                                                      select new TableModel()
                                                      {
                                                          tableId = ts.tableId,
                                                          name = ts.name,
                                                          personsCount = ts.personsCount,
                                                          canDelete = false,
                                                          isActive = ts.isActive
                                                      }).ToList(),
                                        }).ToList().OrderBy(x => x.reservationDate).ThenBy(x => x.reservationTime);
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    var warningTimeForLate = entity.setValues.Where(x => x.setting.name == "warningTimeForLateReservation").Select(x => x.value).FirstOrDefault();
                    TimeSpan exceedTime = new TimeSpan(0, int.Parse(warningTimeForLate), 0);
                    foreach (ReservationModel res in reservations)
                    {
                        var startPlusWarning = res.reservationTime + exceedTime;
                        if (startPlusWarning < datenow)
                            res.isExceed = "exceed";
                        else
                            res.isExceed = "";
                    }

                    return TokenManager.GenerateToken(reservations);
                }
            }
        }
        [HttpPost]
        [Route("GetTableInvoice")]
        public string GetTableInvoice(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long tableId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }

                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var tableInvoice = (from rs in entity.invoices.Where(x => x.invType == "sd")
                                        join rt in entity.invoiceTables.Where(x => x.tableId == tableId) on rs.invoiceId equals rt.invoiceId
                                        join cu in entity.agents on rs.agentId equals cu.agentId into cuj
                                        from c in cuj.DefaultIfEmpty()
                                        select new InvoiceModel()
                                        {
                                            invNumber = rs.invNumber,
                                            invDate = rs.invDate,
                                            agentId = rs.agentId,
                                            agentName = c.name,
                                            branchId = rs.branchId,
                                            posId = rs.posId,
                                            branchCreatorId = rs.branchCreatorId,
                                            invType = rs.invType,
                                            total = rs.total,
                                            totalNet = rs.totalNet,
                                            discountType = rs.discountType,
                                            discountValue = rs.discountValue,
                                            invoiceId = rs.invoiceId,
                                            reservationId = rs.reservationId,
                                            waiterId = rs.waiterId,
                                            tax = rs.tax,
                                            membershipId = rs.membershipId,
                                            tables = (from it in entity.invoiceTables.Where(x => x.invoiceId == rs.invoiceId && x.isActive == 1)
                                                      join ts in entity.tables on it.tableId equals ts.tableId
                                                      select new TableModel()
                                                      {
                                                          tableId = it.tableId,
                                                          name = ts.name,
                                                          canDelete = false,
                                                          isActive = it.isActive,
                                                          createUserId = ts.createUserId,
                                                          updateUserId = ts.updateUserId,
                                                      }).ToList(),
                                        }).OrderBy(x => x.invDate).FirstOrDefault();


                    return TokenManager.GenerateToken(tableInvoice);
                }
            }
        }
        [HttpPost]
        [Route("getInvoiceTables")]
        public string getInvoiceTables(string token)
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

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoiceTables = (from rt in entity.invoiceTables.Where(x => x.invoiceId == invoiceId)
                                         join t in entity.tables on rt.tableId equals t.tableId
                                         select new TableModel()
                                         {
                                             tableId = t.tableId,
                                             name = t.name,
                                             personsCount = t.personsCount,
                                             createDate = t.createDate,
                                             updateDate = t.updateDate,
                                         }).ToList();


                    return TokenManager.GenerateToken(invoiceTables);
                }
            }
        }
        [HttpPost]
        [Route("GetActive")]
        public string GetActive(string token)
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
                long sectionId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "sectionId")
                    {
                        sectionId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<tables>();
                    if (branchId != 0)
                        searchPredicate.And(x => x.branchId == branchId);
                    if (sectionId != 0)
                        searchPredicate.And(x => x.sectionId == sectionId);
                    var tablesList = entity.tables.Where(x => x.isActive == 1).Select(S => new TableModel()
                    {
                        tableId = S.tableId,
                        name = S.name,
                        sectionId = S.sectionId,
                        branchId = S.branchId,
                        status = S.status,
                        personsCount = S.personsCount,
                        notes = S.notes,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        isActive = S.isActive,
                    }).ToList();

                    return TokenManager.GenerateToken(tablesList);
                }
            }
        }
        // add or update table
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
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
                string itemObject = "";
                tables Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<tables>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        tables tmpObject = new tables();
                        if (Object.tableId == 0)
                        {
                            Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            entity.tables.Add(Object);
                        }
                        else
                        {
                            tmpObject = entity.tables.Find(Object.tableId);
                            tmpObject.name = Object.name;
                            tmpObject.status = Object.status;
                            tmpObject.personsCount = Object.personsCount;
                            tmpObject.notes = Object.notes;
                            tmpObject.isActive = Object.isActive;
                            tmpObject.updateUserId = Object.updateUserId;
                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        }
                        message = entity.SaveChanges().ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
            }
        }
        [HttpPost]
        [Route("addReservation")]
        public string addReservation(string token)
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
                string itemObject = "";
                reservations Object = null;
                List<tables> lstTables = new List<tables>(); ;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<reservations>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "tables")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        lstTables = JsonConvert.DeserializeObject<List<tables>>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        Object.updateUserId = Object.createUserId;
                        entity.reservations.Add(Object);
                        entity.SaveChanges();
                        long reservationId = Object.reservationId;
                        #region add tables to reservation
                        if (reservationId > 0)
                        {
                            foreach (tables tbl in lstTables)
                            {
                                tablesReservations tableR = new tablesReservations();
                                tableR.tableId = tbl.tableId;
                                tableR.reservationId = reservationId;
                                tableR.createUserId = Object.createUserId;
                                tableR.updateUserId = Object.createUserId;
                                tableR.createDate = tableR.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                tableR.isActive = 1;

                                entity.tablesReservations.Add(tableR);
                            }
                            entity.SaveChanges();
                        }
                        #endregion
                        message = reservationId.ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
            }
        }
        [HttpPost]
        [Route("updateReservation")]
        public string updateReservation(string token)
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
                string itemObject = "";
                reservations Object = null;
                List<tables> lstTables = new List<tables>(); ;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<reservations>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                    else if (c.Type == "tables")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        lstTables = JsonConvert.DeserializeObject<List<tables>>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var reservation = entity.reservations.Find(Object.reservationId);
                        reservation.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        reservation.updateUserId = Object.createUserId;
                        reservation.customerId = Object.customerId;
                        reservation.reservationDate = Object.reservationDate;
                        reservation.reservationTime = Object.reservationTime;
                        reservation.endTime = Object.endTime;
                        reservation.personsCount = Object.personsCount;
                        reservation.notes = Object.notes;
                        entity.SaveChanges();
                        long reservationId = Object.reservationId;
                        #region delete tables in old reservation
                        var reservationTables = entity.tablesReservations.Where(x => x.reservationId == reservationId).ToList();
                        entity.tablesReservations.RemoveRange(reservationTables);
                        entity.SaveChanges();
                        #endregion
                        #region add tables to reservation
                        if (reservationId > 0)
                        {
                            foreach (tables tbl in lstTables)
                            {
                                tablesReservations tableR = new tablesReservations();
                                tableR.tableId = tbl.tableId;
                                tableR.reservationId = reservationId;
                                tableR.createUserId = Object.createUserId;
                                tableR.updateUserId = Object.createUserId;
                                tableR.createDate = tableR.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                tableR.isActive = 1;

                                entity.tablesReservations.Add(tableR);
                            }
                            entity.SaveChanges();
                        }
                        #endregion
                        message = reservationId.ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
            }
        }
        [HttpPost]
        [Route("updateReservationStatus")]
        public string updateReservationStatus(string token)
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
                long reservationId = 0;
                long userId = 0;
                string status = "";
                List<tables> lstTables = new List<tables>(); ;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "reservationId")
                    {
                        reservationId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "status")
                    {
                        status = c.Value;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var reservation = entity.reservations.Find(reservationId);
                        reservation.status = status;
                        reservation.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        reservation.updateUserId = userId;

                        entity.SaveChanges();
                        message = reservationId.ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
            }
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
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
                long tableId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "final")
                    {
                        final = bool.Parse(c.Value);
                    }
                }
                if (final)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            tables tableObj = entity.tables.Find(tableId);
                            entity.tables.Remove(tableObj);
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
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            tables tableObj = entity.tables.Find(tableId);

                            tableObj.isActive = 0;
                            tableObj.updateUserId = userId;
                            tableObj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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
        }

        [HttpPost]
        [Route("AddTablesToSection")]
        public string AddTablesToSection(string token)
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
                long sectionId = 0;
                long userId = 0;
                string locationsObject = "";
                List<tables> Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        locationsObject = c.Value.Replace("\\", string.Empty);
                        locationsObject = locationsObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<List<tables>>(locationsObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
                        //break;
                    }
                    else if (c.Type == "sectionId")
                    {
                        sectionId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var oldList = entity.tables.Where(x => x.sectionId == sectionId).Select(x => new { x.tableId }).ToList();
                    for (int i = 0; i < oldList.Count; i++)
                    {
                        long locationId = (long)oldList[i].tableId;
                        var loc = entity.tables.Find(locationId);

                        if (Object != null && Object.Count > 0)
                        {
                            var isExist = Object.Find(x => x.tableId == oldList[i].tableId);
                            if (isExist == null)// unlink location to section
                            {
                                loc.sectionId = null;
                                loc.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                loc.updateUserId = userId;
                            }
                            else// edit location info
                            {

                            }
                        }
                        else // clear section from location
                        {
                            loc.sectionId = null;
                            loc.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            loc.updateUserId = userId;
                        }
                    }
                    foreach (tables loc in Object)// loop to add new locations
                    {
                        Boolean isInList = false;
                        if (oldList != null)
                        {
                            var old = oldList.ToList().Find(x => x.tableId == loc.tableId);
                            if (old != null)
                            {
                                isInList = true;

                            }

                            if (!isInList)
                            {
                                var loc1 = entity.tables.Find(loc.tableId);
                                if (loc1.updateUserId == 0 || loc1.updateUserId == null)
                                {
                                    Nullable<long> id = null;
                                    loc1.updateUserId = id;
                                }
                                if (loc1.createUserId == 0 || loc1.createUserId == null)
                                {
                                    Nullable<long> id = null;
                                    loc1.createUserId = id;
                                }
                                loc1.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                                loc1.sectionId = sectionId;
                                loc.updateUserId = userId;
                                //entity.SaveChanges();
                            }
                        }
                        try
                        {
                            entity.SaveChanges();
                        }
                        catch
                        {
                            message = "0";
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    entity.SaveChanges();
                }
            }
            message = "1";
            return TokenManager.GenerateToken(message);
        }
        [HttpPost]
        [Route("GetLastNumOfReserv")]
        public string GetLastNumOfReserv(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string reservCode = "";
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "reservCode")
                    {
                        reservCode = c.Value;
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
                    numberList = entity.reservations.Where(b => b.code.Contains(reservCode + "-") && b.branchId == branchId).Select(b => b.code).ToList();

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
    }
}
