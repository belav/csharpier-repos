using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Classes;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/dash")]
    public class dashController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // for Dashboard
        //
        [HttpPost]
        [Route("Getdashsalpur")]
        public string Getdashsalpur(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListm = (
                            from I in entity.invoices

                            join BC in entity.branches
                                on I.branchCreatorId equals BC.branchId
                                into JBC
                            //pbw pb  sb
                            from JBCC in JBC.DefaultIfEmpty()
                            where
                                (
                                    brIds.Contains(JBCC.branchId)
                                    && (
                                        I.invType == "p"
                                        || I.invType == "pw"
                                        || I.invType == "s"
                                        || I.invType == "pbw"
                                        || I.invType == "pb"
                                        || I.invType == "sb"
                                        || I.invType == "ss"
                                        || I.invType == "ts"
                                    )
                                )

                            select new
                            {
                                I.invoiceId,
                                // I.invNumber,
                                //  I.agentId,
                                //  I.posId,
                                I.invType,
                                //  I.total,
                                //I.totalNet,


                                //
                                I.branchCreatorId,
                                branchCreatorName = JBCC.name,
                            }
                        ).ToList();

                        //   var group2invlist = invListm.GroupBy(g => new { g.invType, g.branchCreatorId }).Select(g => new

                        var list = invListm
                            .GroupBy(g => g.branchCreatorId)
                            .Select(
                                g =>
                                    new
                                    {
                                        invType = g.FirstOrDefault().invType,
                                        branchCreatorId = g.FirstOrDefault().branchCreatorId,
                                        branchCreatorName = g.FirstOrDefault().branchCreatorName,
                                        purCount = g.Where(
                                                i => (i.invType == "p" || i.invType == "pw")
                                            )
                                            .Count(),
                                        saleCount = g.Where(
                                                i =>
                                                    i.invType == "s"
                                                    || i.invType == "ss"
                                                    || i.invType == "ts"
                                            )
                                            .Count(),
                                        purBackCount = g.Where(
                                                i => (i.invType == "pbw" || i.invType == "pb")
                                            )
                                            .Count(),
                                        saleBackCount = g.Where(i => i.invType == "sb").Count(),
                                    }
                            )
                            .ToList();
                        /*
                        .GroupBy(s =>  s.branchCreatorId ).Select(s => new
                        {
                            invType = s.FirstOrDefault().invType,
                            branchCreatorId = s.FirstOrDefault().branchCreatorId,
                            branchCreatorName = s.FirstOrDefault().branchCreatorName,
                            purCount = s.Where(i => (i.invType == "p" || i.invType == "pw")).Count(),

                            saleCount = s.Where(i => i.invType == "s").Count(),
                        }
                            ).ToList();
                            */

                        /*
                           var result = temp.GroupBy(s => new { s.updateUserId, s.cUserAccName }).Select(s => new
            {
                updateUserId = s.FirstOrDefault().updateUserId,
                cUserAccName = s.FirstOrDefault().cUserAccName,
                count = s.Count()
            });
                         * */
                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetAgentCount")]
        public string GetAgentCount(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                //long mainBranchId = 0;
                //long userId = 0;

                //IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                //foreach (Claim c in claims)
                //{
                //    if (c.Type == "mainBranchId")
                //    {
                //        mainBranchId = long.Parse(c.Value);
                //    }
                //    else if (c.Type == "userId")
                //    {
                //        userId = long.Parse(c.Value);
                //    }

                //}
                try
                {
                    //StatisticsController sts = new StatisticsController();
                    //List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListm = (
                            from A in entity.agents
                            select new
                            {
                                //  A.agentId,
                                A.type,
                            }
                        ).ToList();

                        //   var group2invlist = invListm.GroupBy(g => new { g.invType, g.branchCreatorId }).Select(g => new

                        var list = invListm
                            .GroupBy(g => g.type)
                            .Select(
                                g =>
                                    new
                                    {
                                        type = g.FirstOrDefault().type,
                                        vendorCount = g.Where(i => i.type == "v").Count(),
                                        customerCount = g.Where(i => i.type == "c").Count(),
                                        grp = 1,
                                    }
                            )
                            .ToList()
                            .GroupBy(g => g.grp)
                            .Select(
                                c =>
                                    new
                                    {
                                        vendorCount = c.Sum(d => d.vendorCount),
                                        customerCount = c.Sum(d => d.customerCount),
                                    }
                            )
                            .ToList();

                        //g.FirstOrDefault().type=="v"

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        //عدد المستخدمين المتصلين والغير متصلين  حاليا في كل فرع
        [HttpPost]
        [Route("Getuseronline")]
        public string Getuseronline(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //  int allUsers = entity.users.ToList().Count();
                        /*
                        // get user count in every branch
                        var listUsersinbranch = entity.branchesUsers.Select(s => new
                        {
                            s.branchId,
                            s.userId
                        }).ToList();

                        var listub = listUsersinbranch.GroupBy(g => g.branchId).Select(g => new
                        {
                            usersAll = g.Count(),
                            g.FirstOrDefault().branchId,

                        }).ToList();
                        */

                        var listPosinbranch = entity.pos
                            .Select(
                                s =>
                                    new
                                    {
                                        s.branchId,
                                        s.posId,
                                        s.isActive,
                                        s.branches.name,
                                    }
                            )
                            .ToList();
                        // get Active Pos count in every branch
                        var listposb = listPosinbranch
                            .Where(x => x.isActive == 1)
                            .GroupBy(g => g.branchId)
                            .Select(
                                g =>
                                    new
                                    {
                                        posAll = g.Count(),
                                        g.FirstOrDefault().branchId,
                                        g.FirstOrDefault().name,
                                    }
                            )
                            .ToList();
                        List<UserOnlineCount> list = new List<UserOnlineCount>();

                        foreach (var row in listposb)
                        {
                            UserOnlineCount newrow = new UserOnlineCount();
                            newrow.allPos = row.posAll;
                            newrow.branchId = (long)row.branchId;
                            newrow.branchName = row.name;
                            newrow.offlineUsers = row.posAll;
                            newrow.userOnlineCount = 0;

                            list.Add(newrow);
                        }

                        var invListm = (
                            from log in entity.usersLogs
                            join p in entity.pos on log.posId equals p.posId
                            join u in entity.users on log.userId equals u.userId

                            where (log.sOutDate == null && log.users.isOnline == 1)

                            select new
                            {
                                log.userId,
                                p.branchId,
                                branchName = p.branches.name,
                                branchisActive = p.branches.isActive,
                                log.posId,
                                posName = p.name,
                                posisActive = p.isActive,
                                //
                                //usernameAccount = u.username,
                                //userName = u.name,
                                //lastname = u.lastname,

                                //job = u.job,
                                //phone = u.phone,
                                //mobile = u.mobile,
                                //email = u.email,
                                //address = u.address,
                                //userisActive = u.isActive,
                                //isOnline = u.isOnline,

                                //image = u.image,

                                //
                            }
                        ).ToList();

                        //   var group2invlist = invListm.GroupBy(g => new { g.invType, g.branchCreatorId }).Select(g => new

                        List<userOnlineInfo> grouplist = invListm
                            .GroupBy(g => new { g.branchId, g.userId })
                            .Select(
                                g =>
                                    new userOnlineInfo
                                    {
                                        branchId = g.LastOrDefault().branchId,
                                        branchName = g.LastOrDefault().branchName,
                                        branchisActive = g.LastOrDefault().branchisActive,
                                        posId = g.LastOrDefault().posId,
                                        posName = g.LastOrDefault().posName,
                                        posisActive = g.LastOrDefault().posisActive,
                                        userId = g.LastOrDefault().userId,

                                        //usernameAccount = g.LastOrDefault().usernameAccount,
                                        //userName = g.LastOrDefault().userName,
                                        //lastname = g.LastOrDefault().lastname,
                                        //job = g.LastOrDefault().job,
                                        //phone = g.LastOrDefault().phone,
                                        //mobile = g.LastOrDefault().mobile,
                                        //email = g.LastOrDefault().email,
                                        //address = g.LastOrDefault().address,
                                        //userisActive = g.LastOrDefault().userisActive,
                                        //isOnline = g.LastOrDefault().isOnline,
                                        //image = g.LastOrDefault().image,
                                    }
                            )
                            .ToList();

                        List<UserOnlineCount> grop = grouplist
                            .GroupBy(g => g.branchId)
                            .Select(
                                g =>
                                    new UserOnlineCount
                                    {
                                        userOnlineCount = g.Count(),
                                        allPos = listposb
                                            .Where(b => b.branchId == g.FirstOrDefault().branchId)
                                            .FirstOrDefault()
                                            .posAll,
                                        offlineUsers =
                                            listposb
                                                .Where(
                                                    b => b.branchId == g.FirstOrDefault().branchId
                                                )
                                                .FirstOrDefault()
                                                .posAll - g.Count(), //offline= all -online
                                        branchId = (long)g.FirstOrDefault().branchId,
                                        branchName = g.FirstOrDefault().branchName,
                                        //  userOnlinelist = grouplist.Where(b => b.branchId == g.FirstOrDefault().branchId).ToList(),
                                        //   userOnlinelist = grouplist.ToList(),
                                    }
                            )
                            .ToList();
                        list = list.Where(X => brIds.Contains(X.branchId)).ToList();
                        foreach (UserOnlineCount finalrow in list)
                        {
                            UserOnlineCount temp = new UserOnlineCount();
                            temp = grop.Where(x => x.branchId == finalrow.branchId)
                                .FirstOrDefault();
                            if (temp != null)
                            {
                                finalrow.offlineUsers = temp.offlineUsers;
                                finalrow.userOnlineCount = temp.userOnlineCount;
                            }
                        }

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetuseronlineInfo")]
        public string GetuseronlineInfo(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListm = (
                            from log in entity.usersLogs
                            join p in entity.pos on log.posId equals p.posId
                            join u in entity.users on log.userId equals u.userId

                            where
                                (
                                    brIds.Contains((long)p.branchId)
                                    && (log.sOutDate == null && log.users.isOnline == 1)
                                )

                            select new
                            {
                                log.userId,
                                p.branchId,
                                branchName = p.branches.name,
                                branchisActive = p.branches.isActive,
                                log.posId,
                                posName = p.name,
                                posisActive = p.isActive,
                                //
                                usernameAccount = u.username,
                                userName = u.name,
                                lastname = u.lastname,
                                job = u.job,
                                phone = u.phone,
                                mobile = u.mobile,
                                email = u.email,
                                address = u.address,
                                userisActive = u.isActive,
                                isOnline = u.isOnline,
                                image = u.image,

                                //
                            }
                        ).ToList();

                        List<userOnlineInfo> list = invListm
                            .GroupBy(g => new { g.branchId, g.userId })
                            .Select(
                                g =>
                                    new userOnlineInfo
                                    {
                                        branchId = g.FirstOrDefault().branchId,
                                        branchName = g.LastOrDefault().branchName,
                                        branchisActive = g.LastOrDefault().branchisActive,
                                        posId = g.LastOrDefault().posId,
                                        posName = g.LastOrDefault().posName,
                                        posisActive = g.LastOrDefault().posisActive,
                                        userId = g.LastOrDefault().userId,
                                        usernameAccount = g.LastOrDefault().usernameAccount,
                                        userName = g.LastOrDefault().userName,
                                        lastname = g.LastOrDefault().lastname,
                                        job = g.LastOrDefault().job,
                                        phone = g.LastOrDefault().phone,
                                        mobile = g.LastOrDefault().mobile,
                                        email = g.LastOrDefault().email,
                                        address = g.LastOrDefault().address,
                                        userisActive = g.LastOrDefault().userisActive,
                                        isOnline = g.LastOrDefault().isOnline,
                                        image = g.LastOrDefault().image,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetBrachonline")]
        public string GetBrachonline(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    List<BranchOnlineCount> list = new List<BranchOnlineCount>();
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // int allBranches = entity.branches.ToList().Count();
                        var allBranchesList = entity.branches.ToList();
                        int allBranches = allBranchesList
                            .Select(b => new { b.branchId, b.isActive, })
                            .Where(b => (b.branchId != 1 && b.isActive == 1))
                            .ToList()
                            .Count();
                        var invListm = (
                            from log in entity.usersLogs
                            join p in entity.pos on log.posId equals p.posId
                            where
                                (
                                    brIds.Contains((int)p.branchId)
                                    && (log.sOutDate == null && log.users.isOnline == 1)
                                )

                            select new
                            {
                                log.userId,
                                p.branchId,
                                branchName = p.branches.name,
                            }
                        ).ToList();

                        var grouplist = invListm
                            .GroupBy(g => new { g.branchId, g.userId })
                            .Select(
                                g =>
                                    new
                                    {
                                        g.FirstOrDefault().userId,
                                        g.FirstOrDefault().branchId,
                                        g.FirstOrDefault().branchName,
                                    }
                            )
                            .ToList();
                        List<UserOnlineCount> grop = grouplist
                            .GroupBy(g => g.branchId)
                            .Select(
                                g =>
                                    new UserOnlineCount
                                    {
                                        branchId = (long)g.FirstOrDefault().branchId,
                                        branchName = g.FirstOrDefault().branchName,
                                    }
                            )
                            .ToList();
                        BranchOnlineCount brow = new BranchOnlineCount();
                        brow.branchAll = allBranches;
                        brow.branchOnline = grop.Count();
                        //brow.branchOffline = allBranches - grop.Count();
                        brow.branchOffline = brIds.Count() - grop.Count();
                        list.Add(brow);

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        //عدد الفواتير في اليوم الحالي
        [HttpPost]
        [Route("GetdashsalpurDay")]
        public string GetdashsalpurDay(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)

            long mainBranchId = 0;
            long userId = 0;

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                Calculate calc = new Calculate();
                try
                {
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListmtmp = (
                            from I in entity.invoices

                            join BC in entity.branches
                                on I.branchCreatorId equals BC.branchId
                                into JBC
                            //pbw pb  sb
                            from JBCC in JBC.DefaultIfEmpty()
                            where
                                (
                                    brIds.Contains(JBCC.branchId)
                                    && (
                                        I.invType == "p"
                                        || I.invType == "pw"
                                        || I.invType == "s"
                                        || I.invType == "pbw"
                                        || I.invType == "pb"
                                        || I.invType == "sb"
                                        || I.invType == "ss"
                                        || I.invType == "ts"
                                    )
                                )
                            //   && I.updateDate==DateTime.Now())
                            select new
                            {
                                I.invoiceId,
                                // I.invNumber,
                                //  I.agentId,
                                //  I.posId,
                                I.invType,
                                //  I.total,
                                //I.totalNet,


                                //
                                I.branchCreatorId,
                                branchCreatorName = JBCC.name,
                                I.updateDate,
                                I.invDate,
                            }
                        ).ToList();
                        var invListm = invListmtmp
                            .Where(
                                X =>
                                    DateTime.Compare(
                                        (DateTime)calc.changeDateformat(X.invDate, "yyyy-MM-dd"),
                                        (DateTime)calc.changeDateformat(datenow, "yyyy-MM-dd")
                                    ) == 0
                            )
                            .ToList();

                        var list = invListm
                            .GroupBy(g => g.branchCreatorId)
                            .Select(
                                g =>
                                    new
                                    {
                                        invType = g.FirstOrDefault().invType,
                                        branchCreatorId = g.FirstOrDefault().branchCreatorId,
                                        branchCreatorName = g.FirstOrDefault().branchCreatorName,
                                        purCount = g.Where(
                                                i => (i.invType == "p" || i.invType == "pw")
                                            )
                                            .Count(),
                                        saleCount = g.Where(
                                                i =>
                                                    i.invType == "s"
                                                    || i.invType == "ss"
                                                    || i.invType == "ts"
                                            )
                                            .Count(),
                                        purBackCount = g.Where(
                                                i => (i.invType == "pbw" || i.invType == "pb")
                                            )
                                            .Count(),
                                        saleBackCount = g.Where(i => i.invType == "sb").Count(),
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        //
        // فواتير المبيعات مع العناصر
        [HttpPost]
        [Route("Getbestseller")]
        public string Getbestseller(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                Calculate calc = new Calculate();
                StatisticsController sts = new StatisticsController();
                List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                try
                {
                    DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListmtmp = (
                            from IT in entity.itemsTransfer
                            from I in entity.invoices.Where(I => I.invoiceId == IT.invoiceId)
                            from IU in entity.itemsUnits.Where(IU => IU.itemUnitId == IT.itemUnitId)
                            // join ITCUSER in entity.users on IT.createUserId equals ITCUSER.userId
                            //   join ITUPUSER in entity.users on IT.updateUserId equals ITUPUSER.userId
                            join ITEM in entity.items on IU.itemId equals ITEM.itemId
                            join UNIT in entity.units on IU.unitId equals UNIT.unitId
                            //    join B in entity.branches on I.branchId equals B.branchId into JB
                            join BC in entity.branches
                                on I.branchCreatorId equals BC.branchId
                                into JBC
                            from JBCC in JBC.DefaultIfEmpty()
                            where
                                (
                                    brIds.Contains(JBCC.branchId)
                                    && (I.invType == "s" || I.invType == "ss" || I.invType == "ts")
                                )

                            select new
                            {
                                itemName = ITEM.name,
                                unitName = UNIT.name,
                                itemsTransId = IT.itemsTransId,
                                itemUnitId = IT.itemUnitId,
                                itemId = IU.itemId,
                                unitId = IU.unitId,
                                quantity = IT.quantity,
                                price = IT.price,
                                I.invoiceId,
                                I.invType,
                                I.updateDate,
                                I.branchCreatorId,
                                branchCreatorName = JBCC.name,
                                Totalrow = (IT.price * IT.quantity),
                                //   ITcreateDate = IT.createDate,
                                ITupdateDate = IT.updateDate,
                                I.invDate,
                            }
                        ).ToList();

                        var invListm2 = invListmtmp
                            .Where(
                                X =>
                                    DateTime.Compare(
                                        (DateTime)calc.changeDateformat(X.invDate, "yyyy-MM"),
                                        (DateTime)calc.changeDateformat(datenow, "yyyy-MM")
                                    ) == 0
                            )
                            .ToList();

                        var list = invListm2
                            .GroupBy(g => new { g.branchCreatorId, g.itemUnitId })
                            .Select(
                                g =>
                                    new
                                    {
                                        itemName = g.FirstOrDefault().itemName,
                                        unitName = g.FirstOrDefault().unitName,
                                        // itemsTransId = IT.itemsTransId,
                                        itemUnitId = g.FirstOrDefault().itemUnitId,
                                        itemId = g.FirstOrDefault().itemId,
                                        unitId = g.FirstOrDefault().unitId,
                                        quantity = g.Sum(s => s.quantity),
                                        price = g.FirstOrDefault().price,
                                        //  I.invoiceId,
                                        //  I.invType,
                                        //    I.updateDate,
                                        branchCreatorId = g.FirstOrDefault().branchCreatorId,
                                        branchCreatorName = g.FirstOrDefault().branchCreatorName,
                                        subTotal = g.Sum(s => s.Totalrow),
                                    }
                            )
                            .OrderByDescending(o => o.quantity)
                            .ToList()
                            .Take(10);

                        //.Take(3)



                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        // كمية العناصر في الفروع

        //  [HttpPost]
        [HttpPost]
        [Route("GetIUStorage")]
        public string GetIUStorage(string token)
        {
            // public ResponseVM GetPurinv(string token)string IUList

            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string IUList = "";
                long mainBranchId = 0;
                long userId = 0;
                List<itemsUnits> newiuObj = new List<itemsUnits>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "IUList")
                    {
                        IUList = c.Value.Replace("\\", string.Empty);
                        IUList = IUList.Trim('"');
                        newiuObj = JsonConvert.DeserializeObject<List<itemsUnits>>(
                            IUList,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
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

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    List<long> iuIds = new List<long>();
                    List<IUStorage> list = new List<IUStorage>();
                    List<branches> brlist = new List<branches>();
                    using (incposdbEntities entity1 = new incposdbEntities())
                    {
                        // get branches
                        brlist = entity1.branches.ToList();
                        brlist = brlist
                            .Where(x => (x.branchId != 1 && x.isActive == 1))
                            .Select(
                                b =>
                                    new branches
                                    {
                                        branchId = b.branchId,
                                        name = b.name,
                                        isActive = b.isActive,
                                    }
                            )
                            .ToList();
                        // prepare new list with all branches and all iu.

                        foreach (itemsUnits row in newiuObj)
                        {
                            foreach (branches branchRow in brlist)
                            {
                                IUStorage newrow = new IUStorage();
                                newrow.itemUnitId = row.itemUnitId;
                                newrow.quantity = 0;
                                newrow.branchId = branchRow.branchId;
                                newrow.branchName = branchRow.name;
                                newrow.itemId = entity1.itemsUnits.Find(row.itemUnitId).itemId;
                                newrow.unitId = entity1.itemsUnits.Find(row.itemUnitId).unitId;
                                ;
                                newrow.itemName = entity1.itemsUnits
                                    .Find(row.itemUnitId)
                                    .items.name;
                                newrow.unitName = entity1.itemsUnits
                                    .Find(row.itemUnitId)
                                    .units.name;

                                list.Add(newrow);
                            }

                            //newrow. = 0;
                            iuIds.Add(row.itemUnitId);
                        }
                    }

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // storageCost storageCostsr = new storageCost();

                        var invListmtemp = (
                            from L in entity.locations
                            //  from I in entity.invoices.Where(I => I.invoiceId == IT.invoiceId)


                            join IUL in entity.itemsLocations
                                on L.locationId equals IUL.locationId
                            join IU in entity.itemsUnits on IUL.itemUnitId equals IU.itemUnitId

                            //  join ITCUSER in entity.users on IT.createUserId equals ITCUSER.userId
                            //join ITUPUSER in entity.users on IT.updateUserId equals ITUPUSER.userId
                            join ITEM in entity.items on IU.itemId equals ITEM.itemId
                            join UNIT in entity.units on IU.unitId equals UNIT.unitId
                            //   join S in entity.sections on L.sectionId equals S.sectionId into JS
                            join B in entity.branches on L.branchId equals B.branchId into JB
                            // join UPUSR in entity.users on IUL.updateUserId equals UPUSR.userId into JUPUS
                            //  join U in entity.users on IUL.createUserId equals U.userId into JU

                            from JBB in JB
                            where (brIds.Contains(JBB.branchId) && iuIds.Contains(IU.itemUnitId))
                            //  from JSS in JS.DefaultIfEmpty()
                            // from JUU in JU.DefaultIfEmpty()
                            // from JUPUSS in JUPUS.DefaultIfEmpty()
                            select new
                            {
                                // item unit
                                itemName = ITEM.name,
                                unitName = UNIT.name,
                                IU.itemUnitId,
                                IU.itemId,
                                IU.unitId,
                                branchName = JBB.name,
                                branchType = JBB.type,
                                IUL.itemsLocId,
                                IUL.locationId,
                                IUL.quantity,
                                L.branchId,
                            }
                        ).ToList();

                        List<IUStorage> invListm = invListmtemp
                            .GroupBy(g => new { g.branchId, g.itemUnitId })
                            .Select(
                                s =>
                                    new IUStorage
                                    {
                                        itemName = s.FirstOrDefault().itemName,
                                        unitName = s.FirstOrDefault().unitName,
                                        itemUnitId = s.FirstOrDefault().itemUnitId,
                                        itemId = s.FirstOrDefault().itemId,
                                        unitId = s.FirstOrDefault().unitId,
                                        branchName = s.FirstOrDefault().branchName,
                                        branchId = s.FirstOrDefault().branchId,
                                        quantity = s.Sum(q => q.quantity),
                                    }
                            )
                            .OrderByDescending(x => x.quantity)
                            .ToList();
                        // merge two list
                        foreach (IUStorage finalrow in list)
                        {
                            IUStorage temp = new IUStorage();
                            temp = invListm
                                .Where(
                                    x =>
                                        (
                                            x.branchId == finalrow.branchId
                                            && x.itemUnitId == finalrow.itemUnitId
                                        )
                                )
                                .FirstOrDefault();
                            if (temp != null)
                            {
                                finalrow.quantity = temp.quantity;
                            }
                        }

                        return TokenManager.GenerateToken(list.OrderByDescending(x => x.quantity));
                        //  return new ResponseVM { Status = "Success", Message = TokenManager.GenerateToken(list.OrderByDescending(x => x.quantity)) };
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        // مجموع مبالغ المشتريات والمبيعات اليومي خلال الشهر الحالي لكل فرع
        [HttpPost]
        [Route("GetTotalPurSale")]
        public string GetTotalPurSale(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                Calculate calc = new Calculate();
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    int year;
                    int month;
                    int days;
                    //StatisticsController sts = new StatisticsController();
                    //List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    List<branches> brlist = new List<branches>();
                    List<TotalPurSale> totalinMonth = new List<TotalPurSale>();
                    List<TotalPurSale> totalinday = new List<TotalPurSale>();
                    List<TotalPurSale> list = new List<TotalPurSale>();
                    TotalPurSale totalAllBranchRow = new TotalPurSale();
                    TotalPurSale totalRowtemp = new TotalPurSale();
                    DateTime currentdate = coctrlr.AddOffsetTodate(DateTime.Now);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        brlist = entity.branches.ToList();
                        brlist = brlist
                            .Where(x => (x.branchId != 1 && x.isActive == 1))
                            .Select(
                                b =>
                                    new branches
                                    {
                                        branchId = b.branchId,
                                        name = b.name,
                                        isActive = b.isActive,
                                    }
                            )
                            .ToList();
                    }

                    year = currentdate.Year;
                    month = currentdate.Month;
                    days = calc.getdays(year, month);
                    for (int i = 1; i <= days; i++)
                    {
                        DateTime daydate = new DateTime(year, month, i);
                        totalinday = new List<TotalPurSale>();

                        totalinday = GetTotalPurSaleday(daydate, i, mainBranchId, userId);
                        totalinMonth.AddRange(totalinday);
                    }

                    for (int i = 1; i <= days; i++)
                    {
                        foreach (branches row in brlist)
                        {
                            totalAllBranchRow = new TotalPurSale();
                            totalAllBranchRow.branchCreatorId = row.branchId;
                            totalAllBranchRow.branchCreatorName = row.name;
                            totalAllBranchRow.day = i;
                            totalAllBranchRow.totalPur = 0;
                            totalAllBranchRow.totalSale = 0;
                            totalAllBranchRow.countPur = 0;
                            totalAllBranchRow.countSale = 0;
                            list.Add(totalAllBranchRow);
                        }
                    }

                    foreach (TotalPurSale rowinv in list)
                    {
                        totalRowtemp = new TotalPurSale();
                        totalRowtemp = totalinMonth
                            .Where(
                                b =>
                                    (
                                        b.day == rowinv.day
                                        && b.branchCreatorId == rowinv.branchCreatorId
                                    )
                            )
                            .FirstOrDefault();
                        if (totalRowtemp != null)
                        {
                            rowinv.totalPur = totalRowtemp.totalPur;
                            rowinv.totalSale = totalRowtemp.totalSale;
                            rowinv.countPur = totalRowtemp.countPur;
                            rowinv.countSale = totalRowtemp.countSale;
                        }
                    }

                    return TokenManager.GenerateToken(list);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        //in day

        public List<TotalPurSale> GetTotalPurSaleday(
            DateTime dayDate,
            int number,
            long mainBranchId,
            long userId
        )
        {
            Calculate calc = new Calculate();
            TotalPurSale totalrow = new TotalPurSale();
            List<TotalPurSale> totalList = new List<TotalPurSale>();
            StatisticsController sts = new StatisticsController();
            List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
            using (incposdbEntities entity = new incposdbEntities())
            {
                var invListm1 = (
                    from I in entity.invoices

                    join BC in entity.branches on I.branchCreatorId equals BC.branchId into JBC
                    //pbw pb  sb
                    from JBCC in JBC.DefaultIfEmpty()
                    // where (I.invType == "p" || I.invType == "pw" || I.invType == "s" || I.invType == "pbw" || I.invType == "pb" || I.invType == "sb")
                    where
                        (
                            brIds.Contains(JBCC.branchId)
                            && (
                                I.invType == "p"
                                || I.invType == "pw"
                                || I.invType == "s"
                                || I.invType == "ss"
                                || I.invType == "ts"
                            )
                            && JBCC.branchId != 1
                            && JBCC.isActive == 1
                        )

                    select new
                    {
                        I.invoiceId,
                        // I.invNumber,
                        //  I.agentId,
                        //  I.posId,
                        I.invType,
                        //  I.total,
                        I.totalNet,
                        I.updateDate,
                        I.invDate,
                        //
                        I.branchCreatorId,
                        branchCreatorName = JBCC.name,
                    }
                ).ToList();
                var invListm = invListm1
                    .Where(
                        X =>
                            DateTime.Compare(
                                (DateTime)calc.changeDateformat(X.invDate, "yyyy-MM-dd"),
                                (DateTime)calc.changeDateformat(dayDate, "yyyy-MM-dd")
                            ) == 0
                    )
                    .ToList();

                totalList = invListm
                    .GroupBy(g => g.branchCreatorId)
                    .Select(
                        g =>
                            new TotalPurSale
                            {
                                //  invType = g.FirstOrDefault().invType,
                                branchCreatorId = g.FirstOrDefault().branchCreatorId,
                                branchCreatorName = g.FirstOrDefault().branchCreatorName,
                                totalPur = g.Where(i => (i.invType == "p" || i.invType == "pw"))
                                    .Sum(s => s.totalNet),
                                totalSale = g.Where(
                                        i =>
                                            i.invType == "s"
                                            || i.invType == "ss"
                                            || i.invType == "ts"
                                    )
                                    .Sum(s => s.totalNet),
                                countPur = g.Where(i => (i.invType == "p" || i.invType == "pw"))
                                    .Count(),
                                countSale = g.Where(
                                        i =>
                                            i.invType == "s"
                                            || i.invType == "ss"
                                            || i.invType == "ts"
                                    )
                                    .Count(),
                                //  purBackCount = g.Where(i => (i.invType == "pbw" || i.invType == "pb")).Count(),
                                // saleBackCount = g.Where(i => i.invType == "sb").Count(),
                                day = number,
                            }
                    )
                    .ToList();

                return totalList;
            }
        }

        //الكاش في الفروع

        // يومية الصندوق
        [HttpPost]
        [Route("GetCashBalance")]
        public string GetCashBalance(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var cachlist = (
                            from p in entity.pos
                            join b in entity.branches on p.branchId equals b.branchId

                            //  from jbbo in jbo.DefaultIfEmpty()

                            where (brIds.Contains(b.branchId))
                            select new
                            {
                                p.posId,
                                posName = p.name,
                                posIsActive = p.isActive,
                                posCode = p.code,
                                p.balance,
                                branchName = b.name,
                                b.branchId,
                                branchType = b.type,
                                branchCode = b.code,
                                banchIsActive = b.isActive
                            }
                        ).ToList();

                        List<BranchBalance> cachlist2 = cachlist
                            .GroupBy(X => X.branchId)
                            .Select(
                                X =>
                                    new BranchBalance
                                    {
                                        branchName = X.FirstOrDefault().branchName,
                                        balance = X.Sum(s => s.balance),
                                        branchId = X.FirstOrDefault().branchId,
                                        branchType = X.FirstOrDefault().branchType,
                                        branchCode = X.FirstOrDefault().branchCode,
                                        banchIsActive = X.FirstOrDefault().banchIsActive,
                                    }
                            )
                            .ToList();
                        List<BranchModel> allbList = GetAllbranches();
                        List<BranchBalance> balanceList = new List<BranchBalance>();
                        BranchBalance balanceObj = new BranchBalance();
                        foreach (BranchModel brow in allbList)
                        {
                            balanceObj = new BranchBalance();
                            List<BranchBalance> temp = cachlist2
                                .Where(X => X.branchId == brow.branchId)
                                .ToList();
                            if (temp.Count > 0)
                            {
                                balanceObj = temp.FirstOrDefault();
                            }
                            else
                            {
                                balanceObj.branchName = brow.name;
                                balanceObj.balance = 0;
                                balanceObj.branchId = brow.branchId;
                                balanceObj.branchType = brow.type;
                                balanceObj.branchCode = brow.code;
                                balanceObj.banchIsActive = brow.isActive;
                            }
                            balanceList.Add(balanceObj);
                        }

                        return TokenManager.GenerateToken(balanceList);
                    }
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        [HttpPost]
        [Route("GetBestOf")]
        public string GetBestOf(string token)
        {
            // public string Get(string token)

            // public ResponseVM GetPurinv(string token)




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;
                DateTime dNow = coctrlr.AddOffsetTodate(DateTime.Now);
                List<BranchInvoicedata> invbranch = new List<BranchInvoicedata>();
                List<BranchInvoicedata> branches = new List<BranchInvoicedata>();
                //List<BranchInvoiceCount> finalList = new List<BranchInvoiceCount>();
                List<BestOfCount> finalList = new List<BestOfCount>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    DateTime firstdate = dNow.AddMonths(-12);

                    DateTime first = new DateTime(firstdate.Year, firstdate.Month, 1);

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        //all rows
                        invbranch = (
                            from I in entity.invoices

                            join BC in entity.branches
                                on I.branchCreatorId equals BC.branchId
                                into JBC
                            //pbw pb  sb
                            from JBCC in JBC.DefaultIfEmpty()
                            where
                                (I.invType == "s" || I.invType == "ss" || I.invType == "ts")
                                && (brIds.Contains(JBCC.branchId))
                                && I.invDate >= first
                            select new BranchInvoicedata
                            {
                                invoiceId = I.invoiceId,
                                invType = I.invType,
                                invDate = I.invDate,
                                branchCreatorId = I.branchCreatorId,
                                branchCreatorName = JBCC.name,

                                // I.invNumber,
                                //  I.agentId,
                                //  I.posId,
                                //  I.total,
                                //I.totalNet,
                            }
                        ).ToList();
                    }
                    // branches rows
                    branches = invbranch
                        .GroupBy(g => g.branchCreatorId)
                        .Select(
                            g =>
                                new BranchInvoicedata
                                {
                                    branchCreatorId = g.FirstOrDefault().branchCreatorId,
                                    branchCreatorName = g.FirstOrDefault().branchCreatorName,
                                }
                        )
                        .ToList();
                    foreach (BranchInvoicedata branchrow in branches)
                    {
                        List<BranchInvoicedata> templist = invbranch
                            .Where(X => X.branchCreatorId == branchrow.branchCreatorId)
                            .ToList();
                        BestOfCount finalRow = new BestOfCount();
                        finalRow.branchId = branchrow.branchCreatorId;

                        finalRow.branchName = branchrow.branchCreatorName;
                        finalRow.CountinMonthsList = GetCountinMonths(
                            dNow,
                            (long)branchrow.branchCreatorId,
                            branchrow.branchCreatorName,
                            templist
                        );

                        finalRow.CountinDaysList = GetCountindays(
                            dNow,
                            (long)branchrow.branchCreatorId,
                            branchrow.branchCreatorName,
                            templist
                        );
                        finalRow.CountinHoursList = GetCountinHours(
                            dNow,
                            (long)branchrow.branchCreatorId,
                            branchrow.branchCreatorName,
                            templist
                        );
                        finalList.Add(finalRow);
                    }
                    BestOfCount allRow = new BestOfCount();
                    allRow.branchId = 0;

                    allRow.branchName = "all";
                    allRow.CountinMonthsList = GetCountinMonths(
                        dNow,
                        0,
                        allRow.branchName,
                        invbranch
                    );

                    allRow.CountinDaysList = GetCountindays(dNow, 0, allRow.branchName, invbranch);
                    allRow.CountinHoursList = GetCountinHours(
                        dNow,
                        0,
                        allRow.branchName,
                        invbranch
                    );
                    finalList.Add(allRow);

                    //var list = invbranch.GroupBy(g => g.branchCreatorId).Select(g => new
                    //{
                    //    invType = g.FirstOrDefault().invType,
                    //    branchCreatorId = g.FirstOrDefault().branchCreatorId,
                    //    branchCreatorName = g.FirstOrDefault().branchCreatorName,
                    //    purCount = g.Where(i => (i.invType == "p" || i.invType == "pw")).Count(),
                    //    saleCount = g.Where(i => i.invType == "s" || i.invType == "ss" || i.invType == "ts").Count(),
                    //    purBackCount = g.Where(i => (i.invType == "pbw" || i.invType == "pb")).Count(),
                    //    saleBackCount = g.Where(i => i.invType == "sb").Count(),
                    //}).ToList();

                    return TokenManager.GenerateToken(finalList);
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        public BranchInvoiceCount GetCountbyBranch(
            long branchCreatorId,
            string branchCreatorName,
            DateTime fromDate,
            DateTime toDate,
            List<BranchInvoicedata> Listbranch,
            int dateindex
        )
        {
            BranchInvoiceCount BranchInvoiceobj = new BranchInvoiceCount();
            if (branchCreatorId == 0)
            {
                Listbranch = Listbranch
                    .Where(X => X.invDate >= fromDate && X.invDate < toDate)
                    .ToList();
            }
            else
            {
                Listbranch = Listbranch
                    .Where(
                        X =>
                            X.invDate >= fromDate
                            && X.invDate < toDate
                            && X.branchCreatorId == branchCreatorId
                    )
                    .ToList();
            }
            //Listbranch = Listbranch.Where(X => X.invDate >= fromDate && X.invDate < toDate && X.branchCreatorId == branchCreatorId).ToList();
            BranchInvoiceobj.count = Listbranch.Count();

            BranchInvoiceobj.fromDate = fromDate;
            BranchInvoiceobj.toDate = toDate;
            BranchInvoiceobj.branchId = branchCreatorId;
            //if (Listbranch.Count() > 0)
            //{
            BranchInvoiceobj.branchName = branchCreatorName;
            //}
            //else
            //{
            //    //
            //    BranchInvoiceobj.branchCreatorName = "";
            //}

            BranchInvoiceobj.dateindex = dateindex;
            return BranchInvoiceobj;
        }

        public List<BranchInvoiceCount> GetCountinMonths(
            DateTime dNow,
            long branchCreatorId,
            string branchCreatorName,
            List<BranchInvoicedata> Listbranch
        )
        {
            List<BranchInvoiceCount> List = new List<BranchInvoiceCount>();
            List<BranchInvoiceCount> tempList = new List<BranchInvoiceCount>();
            BranchInvoiceCount rowObj = new BranchInvoiceCount();
            BranchInvoiceCount otherObj = new BranchInvoiceCount();
            DateTime firstdate = dNow.AddMonths(-12);

            DateTime startFromdate = new DateTime(firstdate.Year, firstdate.Month, 1);
            DateTime fromDate = startFromdate;
            DateTime toDate = startFromdate;
            for (int i = 0; i < 12; i++)
            {
                //  rowObj = new BranchInvoiceCount();
                fromDate = startFromdate.AddMonths(i);
                toDate = startFromdate.AddMonths(i + 1);
                //fromDate = new DateTime(fromDate.Year, fromDate.Month + i, 1);
                //toDate = new DateTime(fromDate.Year, fromDate.Month + i + 1, 1);
                rowObj = GetCountbyBranch(
                    branchCreatorId,
                    branchCreatorName,
                    fromDate,
                    toDate,
                    Listbranch,
                    i + 1
                );
                rowObj.duration =
                    rowObj.fromDate.Month.ToString() + "/" + rowObj.fromDate.Year.ToString();
                List.Add(rowObj);
            }
            List = List.OrderByDescending(X => X.count).ToList();
            //get 4+other
            List = filtertoOther(List.ToList(), Listbranch.ToList());

            return List;
        }

        public List<BranchInvoiceCount> filtertoOther(
            List<BranchInvoiceCount> AllList,
            List<BranchInvoicedata> Listbranch
        )
        {
            List<BranchInvoiceCount> List = new List<BranchInvoiceCount>();
            BranchInvoiceCount otherObj = new BranchInvoiceCount();
            List<BranchInvoiceCount> tempList = new List<BranchInvoiceCount>();
            if (AllList.Count > 6)
            {
                tempList = AllList.Take(5).ToList();
                otherObj.count = AllList.Skip(5).ToList().Sum(X => X.count);
                otherObj.branchName = Listbranch.FirstOrDefault().branchCreatorName;
                otherObj.branchId = Listbranch.FirstOrDefault().branchCreatorId;
                otherObj.dateindex = 0;
                otherObj.duration = "other";
                tempList.Add(otherObj);
                List = tempList.ToList();
            }
            return List;
        }

        public List<BranchInvoiceCount> GetCountindays(
            DateTime dNow,
            long branchCreatorId,
            string branchCreatorName,
            List<BranchInvoicedata> Listbranch
        )
        {
            List<BranchInvoiceCount> List = new List<BranchInvoiceCount>();
            BranchInvoiceCount rowObj = new BranchInvoiceCount();
            DateTime firstdate = dNow.AddDays(-7);

            DateTime startFromdate = new DateTime(
                firstdate.Year,
                firstdate.Month,
                firstdate.Day,
                0,
                0,
                0
            );
            DateTime fromDate = startFromdate;
            DateTime toDate = startFromdate;
            for (int i = 0; i < 7; i++)
            {
                //  rowObj = new BranchInvoiceCount();
                fromDate = startFromdate.AddDays(i);
                toDate = startFromdate.AddDays(i + 1);
                //fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day + i);
                //toDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day + i + 1);
                rowObj = GetCountbyBranch(
                    branchCreatorId,
                    branchCreatorName,
                    fromDate,
                    toDate,
                    Listbranch,
                    i + 1
                );
                rowObj.duration = DayOfWeekconvert(rowObj.fromDate);

                List.Add(rowObj);
            }
            List = List.OrderByDescending(X => X.count).ToList();
            List = filtertoOther(List.ToList(), Listbranch.ToList());
            return List;
        }

        public string DayOfWeekconvert(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return "sat";
                case DayOfWeek.Sunday:
                    return "sun";
                case DayOfWeek.Monday:
                    return "mon";
                case DayOfWeek.Tuesday:
                    return "tues";
                case DayOfWeek.Wednesday:
                    return "wed";
                case DayOfWeek.Thursday:
                    return "thur";
                case DayOfWeek.Friday:
                    return "fri";
                default:
                    return "";
                //break;
            }
        }

        public List<BranchInvoiceCount> GetCountinHours(
            DateTime dNow,
            long branchCreatorId,
            string branchCreatorName,
            List<BranchInvoicedata> Listbranch
        )
        {
            List<BranchInvoiceCount> List = new List<BranchInvoiceCount>();
            BranchInvoiceCount rowObj = new BranchInvoiceCount();
            DateTime firstdate = dNow.AddHours(-24);

            DateTime startFromdate = new DateTime(
                firstdate.Year,
                firstdate.Month,
                firstdate.Day,
                firstdate.Hour,
                0,
                0
            );
            DateTime fromDate = startFromdate;
            DateTime toDate = startFromdate;
            for (int i = 0; i < 24; i++)
            {
                //  rowObj = new BranchInvoiceCount();
                fromDate = startFromdate.AddHours(i);

                toDate = startFromdate.AddHours(i + 1);
                //fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour + i, 0, 0);
                //toDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour + i + 1, 0, 0);

                rowObj = GetCountbyBranch(
                    branchCreatorId,
                    branchCreatorName,
                    fromDate,
                    toDate,
                    Listbranch,
                    i + 1
                );
                rowObj.duration = rowObj.fromDate.Hour.ToString() + ":" + "00";
                List.Add(rowObj);
            }
            List = List.OrderByDescending(X => X.count).ToList();
            List = filtertoOther(List.ToList(), Listbranch.ToList());
            return List;
        }

        // عدد الفواتير حسب النوع بكل فرع
        //عدد الفواتير في اليوم الحالي
        [HttpPost]
        [Route("GetCountByInvType")]
        public string GetCountByInvType(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long mainBranchId = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "mainBranchId")
                    {
                        mainBranchId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                List<CountByInvType> list = new List<CountByInvType>();
                DateTime dNow = coctrlr.AddOffsetTodate(DateTime.Now);
                DateTime firstofday = new DateTime(dNow.Year, dNow.Month, dNow.Day);
                DateTime endofday = new DateTime(dNow.Year, dNow.Month, dNow.Day + 1);
                Calculate calc = new Calculate();
                try
                {
                    StatisticsController sts = new StatisticsController();
                    List<long> brIds = sts.AllowedBranchsId(mainBranchId, userId);
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var invListmtmp = (
                            from I in entity.invoices
                            join BC in entity.branches
                                on I.branchCreatorId equals BC.branchId
                                into JBC
                            from JBCC in JBC.DefaultIfEmpty()
                            where
                                (
                                    brIds.Contains(JBCC.branchId)
                                    && (I.invType == "s" || I.invType == "ss" || I.invType == "ts")
                                    && (I.invDate >= firstofday && I.invDate < endofday)
                                )
                            //   && I.updateDate==DateTime.Now())
                            select new
                            {
                                I.invoiceId,
                                I.invType,
                                I.branchCreatorId,
                                branchCreatorName = JBCC.name,
                                I.updateDate,
                                I.invDate,
                            }
                        ).ToList();

                        list = invListmtmp
                            .GroupBy(g => g.branchCreatorId)
                            .Select(
                                g =>
                                    new CountByInvType
                                    {
                                        invType = g.FirstOrDefault().invType,
                                        branchId = g.FirstOrDefault().branchCreatorId,
                                        branchName = g.FirstOrDefault().branchCreatorName,
                                        dhallCount = g.Where(i => i.invType == "s").Count(),
                                        selfCount = g.Where(i => i.invType == "ss").Count(),
                                        tawayCount = g.Where(i => i.invType == "ts").Count(),
                                    }
                            )
                            .ToList();

                        List<BranchModel> allbList = GetAllbranches();
                        List<CountByInvType> listf = new List<CountByInvType>();
                        CountByInvType counObj = new CountByInvType();
                        foreach (BranchModel brow in allbList)
                        {
                            counObj = new CountByInvType();
                            List<CountByInvType> temp = list.Where(X => X.branchId == brow.branchId)
                                .ToList();
                            if (temp.Count > 0)
                            {
                                counObj = temp.FirstOrDefault();
                            }
                            else
                            {
                                counObj.branchId = brow.branchId;
                                counObj.invType = "";

                                counObj.branchName = brow.name;
                                counObj.dhallCount = 0;
                                counObj.selfCount = 0;
                                counObj.tawayCount = 0;
                            }
                            listf.Add(counObj);
                        }

                        return TokenManager.GenerateToken(listf);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public List<BranchModel> GetAllbranches()
        {
            List<BranchModel> brlist = new List<BranchModel>();
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var brlist1 = entity.branches.ToList();
                    brlist = brlist1
                        .Where(x => (x.branchId != 1 && x.isActive == 1))
                        .Select(
                            b =>
                                new BranchModel
                                {
                                    branchId = b.branchId,
                                    name = b.name,
                                    isActive = b.isActive,
                                    type = b.type,
                                }
                        )
                        .ToList();
                }
                return brlist;
            }
            catch
            {
                return brlist;
            }
        }

        [HttpPost]
        [Route("GetMainNotification")]
        public string GetMainNotification(string token)
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
                long userId = 0;
                long posId = 0;
                string alertType = "";
                string cashTransferType = "";
                string cashSide = "";

                string result = "{";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "posId")
                    {
                        posId = int.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = int.Parse(c.Value);
                    }
                    else if (c.Type == "alertType")
                    {
                        alertType = c.Value;
                    }
                    else if (c.Type == "cashTransferType")
                    {
                        cashTransferType = c.Value;
                    }
                    else if (c.Type == "cashSide")
                    {
                        cashSide = c.Value;
                    }
                }
                #endregion
                // try
                {
                    notificationUserController nc = new notificationUserController();
                    int notCount = nc.GetNotUserCount(posId, userId, alertType);
                    result += "UserNotCount:" + notCount;

                    InvoicesController ic = new InvoicesController();
                    int ordersCount = ic.getDeliverOrdersCount(userId);
                    result += ",UseraitingOrderCount:" + ordersCount;

                    CashTransferController cc = new CashTransferController();
                    IEnumerable<CashTransferModel> cashesQuery = cc.GetCashTransferForPosById(
                        posId,
                        cashTransferType,
                        cashSide
                    );
                    cashesQuery = cashesQuery.Where(c => c.posId == posId && c.isConfirm == 0);
                    int posCachTransfers = cashesQuery.Count();
                    result += ",CashTransferCount:" + posCachTransfers;

                    PosController pc = new PosController();
                    PosModel pos = pc.GetPosByID(posId);
                    result += ",PosBalance:" + pos.balance;
                    result += ",BoxState:'" + pos.boxState + "'";

                    result += "}";
                    // return result;
                    return TokenManager.GenerateToken(result);
                }
                //catch
                //{
                //    return TokenManager.GenerateToken("0");
                //}
            }
        }
    }
}
