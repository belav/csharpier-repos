using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/ProgramDetails")]
    public class ProgramDetailsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("getCurrentInfo")]
        public async Task<string> getCurrentInfo(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                try
                {
                    ProgramDetailsModel packrow = new ProgramDetailsModel();
                    packrow = getCurrentInfo();

                    return TokenManager.GenerateToken(packrow);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("updateIsonline")]
        public string updateIsonline(string token)
        {
            //string Object
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                bool isOnlineServer = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "isOnlineServer")
                    {
                        isOnlineServer = bool.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<ProgramDetails>();

                        var packsl = entity.ProgramDetails.ToList();

                        var packs = packsl.FirstOrDefault();
                        packs.isOnlineServer = isOnlineServer;
                        entity.SaveChanges();
                        message = packs.id.ToString();

                        //  entity.SaveChanges();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        public ProgramDetailsModel getCurrentInfo()
        {
            ProgramDetailsModel packs = new ProgramDetailsModel();

            using (incposdbEntities entity = new incposdbEntities())
            {
                packs = (
                    from p in entity.ProgramDetails
                    //  join p in entity.posSetting on S.id equals p.posSerialId
                    select new ProgramDetailsModel
                    {
                        programName = p.programName,
                        branchCount = p.branchCount,
                        posCount = p.posCount,
                        userCount = p.userCount,
                        vendorCount = p.vendorCount,
                        customerCount = p.customerCount,
                        itemCount = p.itemCount,
                        saleinvCount = p.saleinvCount,
                        storeCount = p.storeCount,
                        packageSaleCode = p.packageSaleCode,
                        customerServerCode = p.customerServerCode,
                        expireDate = p.expireDate,
                        isOnlineServer = p.isOnlineServer,
                        updateDate = p.updateDate,
                        isLimitDate = (p.isLimitDate == true) ? true : false,
                        isActive = p.isActive,
                        packageName = p.packageName,
                        versionName = p.versionName,
                        packageNumber = p.packageNumber,
                        customerName = p.customerName,
                        customerLastName = p.customerLastName,
                        agentName = p.agentName,
                        agentLastName = p.agentLastName,
                        agentAccountName = p.agentAccountName,
                        isDemo =
                            p.isDemo == "" || p.isDemo == null || p.isDemo == "1" ? "1" : p.isDemo,
                    }
                ).FirstOrDefault();

                packs.posCountNow = entity.pos.Count();

                packs.branchCountNow = entity.branches.Where(x => x.type == "b").Count();

                packs.storeCountNow = entity.branches.Where(x => x.type == "s").Count();
                packs.userCountNow = entity.users.Count();
                packs.vendorCountNow = entity.agents.Where(x => x.type == "v").Count();
                packs.customerCountNow = entity.agents.Where(x => x.type == "c").Count();
                packs.itemCountNow = entity.items.Count();

                packs.saleinvCountNow = getSalesInvCountInMonth();
                packs.serverDateNow = coctrlr.AddOffsetTodate(DateTime.Now);
            }

            return packs;
        }

        public ProgramDetails getCurrentProgDetail()
        {
            ProgramDetails packs = new ProgramDetails();

            using (incposdbEntities entity = new incposdbEntities())
            {
                packs = entity.ProgramDetails.ToList().FirstOrDefault();
                //  join p in entity.posSetting on S.id equals p.posSerialId
            }

            return packs;
        }

        public ProgramDetailsModel getCustomerServerCode()
        {
            ProgramDetailsModel packs = new ProgramDetailsModel();
            using (incposdbEntities entity = new incposdbEntities())
            {
                packs = (
                    from p in entity.ProgramDetails
                    //  join p in entity.posSetting on S.id equals p.posSerialId
                    select new ProgramDetailsModel { customerServerCode = p.customerServerCode, }
                ).FirstOrDefault();
            }
            return packs;
        }

        public int getSalesInvCountInMonth()
        {
            int invCount = 0;
            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
            using (incposdbEntities entity = new incposdbEntities())
            {
                var expireDate = entity.ProgramDetails.Select(x => x.expireDate).Single();
                int expireDay = Convert.ToDateTime(expireDate).Day;
                int currentMonth = datenow.Month;
                int currentYear = datenow.Year;
                int currentMonthDays = DateTime.DaysInMonth(currentYear, currentMonth);

                if (expireDay > currentMonthDays)
                    expireDay = currentMonthDays;
                DateTime compaireDate2 = new DateTime(currentYear, currentMonth, expireDay);
                DateTime compairDate1 = compaireDate2.AddMonths(-1);

                // get sales imvoice count between compaireDate1 and compairDate2
                invCount = entity.invoices
                    .Where(
                        x =>
                            (x.invType == "s" || x.invType == "ts" || x.invType == "ss")
                            && x.invDate >= compairDate1
                            && x.invDate < compaireDate2
                    )
                    .Count();
            }
            return invCount;
        }

        [HttpPost]
        [Route("getRemainDayes")]
        public async Task<string> getRemainDayes(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                try
                {
                    ProgramDetails packrow = new ProgramDetails();
                    packrow = getCurrentProgDetail();
                    daysremain daysModel = new daysremain();
                    int days = 0;

                    if (packrow.expireDate == null)
                    {
                        daysModel.expirestate = "n";
                        daysModel.days = 0;
                        return TokenManager.GenerateToken(daysModel); //not regester  =>no alert
                    }
                    DateTime expiredate = (DateTime)packrow.expireDate;
                    DateTime nowdate = coctrlr.AddOffsetTodate(DateTime.Now);
                    TimeSpan diffdate = expiredate - nowdate;

                    days = diffdate.Days;

                    // diffdate.Hours;
                    if (packrow.isLimitDate == false)
                    {
                        daysModel.expirestate = "u";
                        daysModel.days = 0;
                        return TokenManager.GenerateToken(daysModel); //unlimited=> no alert
                    }
                    else
                    {
                        // daysModel.hours = diffdate.Hours;
                        daysModel.expirestate = "e";
                        daysModel.days = days;
                        if (days == 0)
                        {
                            daysModel.hours = diffdate.Hours;
                            if (daysModel.hours == 0)
                            {
                                daysModel.minute = diffdate.Minutes;
                            }
                            //  daysModel.hours = diffdate.Hours;
                        }

                        //if (days > 10 )
                        // {
                        //     return TokenManager.GenerateToken("-2");// no alert
                        //     //return TokenManager.GenerateToken(days.ToString());// no alert
                        // }
                        // else
                        // {

                        return TokenManager.GenerateToken(daysModel); //show alert with days
                        //}
                    }
                }
                catch (Exception ex)
                {
                    //  return TokenManager.GenerateToken("0");
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }
    }
}
