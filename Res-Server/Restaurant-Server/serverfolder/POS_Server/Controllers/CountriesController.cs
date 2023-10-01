using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Countries")]
    public class CountriesController : ApiController
    {
        // GET api/<controller>
        [HttpPost]
        [Route("GetAllCountries")]
        public string GetAllCountries(string token)
        {
            // public ResponseVM GetPurinv(string token)

            //long mainBranchId, long userId    DateTime? date=new DateTime?();



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

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity
                            .countriesCodes
                            .Select(c => new { c.countryId, c.code, })
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";

            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var countrylist = entity.countriesCodes
            //             .Select(c => new {
            //                 c.countryId,
            //                 c.code,
            //             }).ToList();


            //        if (countrylist == null)
            //           return NotFound();
            //        else
            //            return Ok(countrylist);
            //    }
            //}
            //else
            //    return NotFound();
        }

        //[HttpPost]
        //[Route("GetAllCities")]
        //public IHttpActionResult GetAllCities()
        //{
        //
        //
        //    string token = "";

        //    if (headers.Contains("APIKey"))
        //    {
        //        token = headers.GetValues("APIKey").First();
        //    }
        //    Validation validation = new Validation();
        //    bool valid = validation.CheckApiKey(token);

        //    if (valid) // APIKey is valid
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var countrylist = entity.countriesCodes
        //                 .Select(c => new {
        //                     c.countryId,
        //                     c.code,
        //                     c.isDefault,
        //                 }).ToList();


        //            if (countrylist == null)
        //            { return Ok(countrylist); }
        //            //return ("no");
        //            //return NotFound();
        //            else
        //            { return Ok(countrylist);}

        //        }
        //    }
        //    else
        //        return NotFound();
        //}



        [HttpPost]
        [Route("GetAllRegion")]
        public string GetAllRegion(string token)
        {
            // public ResponseVM GetPurinv(string token)

            //long mainBranchId, long userId    DateTime? date=new DateTime?();



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
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity
                            .countriesCodes
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
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";

            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var countrylist = entity.countriesCodes
            //             .Select(c => new {
            //                 c.countryId,
            //                 c.code,
            //                 c.currency,
            //                 c.name,
            //                 c.isDefault,
            //                 c.currencyId,

            //             }).ToList();


            //        if (countrylist == null)
            //            return NotFound();
            //        else
            //            return Ok(countrylist);
            //    }
            //}
            //else
            //    return NotFound();
        }

        [HttpPost]
        [Route("UpdateIsdefault")]
        public string UpdateIsdefault(string token)
        {
            //int countryId
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long countryId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "countryId")
                    {
                        countryId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // reset all to 0
                        List<countriesCodes> objectlist = entity
                            .countriesCodes
                            .Where(x => x.isDefault == 1)
                            .ToList();
                        if (objectlist.Count > 0)
                        {
                            for (int i = 0; i < objectlist.Count; i++)
                            {
                                objectlist[i].isDefault = 0;
                            }
                            entity.SaveChanges();
                        }
                        // set is selected to isdefault=1
                        countriesCodes objectrow = entity.countriesCodes.Find(countryId);

                        if (objectrow != null)
                        {
                            objectrow.isDefault = 1;

                            long res = entity.SaveChanges();
                            if (res > 0)
                            {
                                message = objectrow.countryId.ToString();
                            }
                            else
                            {
                                return TokenManager.GenerateToken("0");
                            }
                        }
                        else
                        {
                            return TokenManager.GenerateToken("0");
                        }
                        //  entity.SaveChanges();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //string message = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{


            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            // reset all to 0
            //            List<countriesCodes> objectlist = entity.countriesCodes.Where(x=>x.isDefault==1).ToList();
            //            if (objectlist.Count > 0)
            //            {
            //                for(int i=0;i< objectlist.Count; i++)
            //                {
            //                    objectlist[i].isDefault = 0;

            //                }
            //                entity.SaveChanges();
            //            }
            //            // set is selected to isdefault=1
            //            countriesCodes objectrow = entity.countriesCodes.Find(countryId);

            //            if (objectrow != null)
            //            {
            //                objectrow.isDefault = 1;

            //                message = objectrow.countryId.ToString();
            //                entity.SaveChanges();
            //            }
            //            else
            //            {
            //                message = "-1";
            //            }
            //            //  entity.SaveChanges();
            //        }
            //    }
            //    catch
            //    {
            //        message = "-1";
            //    }
            //}
            //return message;
        }

        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            // public ResponseVM GetPurinv(string token)

            //long mainBranchId, long userId    DateTime? date=new DateTime?();



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;
                // long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Id")
                    {
                        Id = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity
                            .countriesCodes
                            .Where(c => c.countryId == Id)
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

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //long cId = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("Id"))
            //{
            //    cId = Convert.ToInt32(headers.GetValues("Id").First());
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var list = entity.countriesCodes
            //       .Where(c => c.countryId == cId)
            //       .Select(c => new {
            //           c.countryId,
            //           c.code,
            //           c.currency,
            //           c.name,
            //           c.isDefault,
            //           c.currencyId,
            //       })
            //       .FirstOrDefault();

            //        if (list == null)
            //            return NotFound();
            //        else
            //            return Ok(list);
            //    }
            //}
            //else
            //    return NotFound();
        }

        [HttpPost]
        [Route("GetisDefault")]
        public string GetisDefault(string token)
        {
            // public ResponseVM GetPurinv(string token)

            //long mainBranchId, long userId    DateTime? date=new DateTime?();



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                //long mainBranchId = 0;
                int isDefault = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "isDefault")
                    {
                        isDefault = int.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity
                            .countriesCodes
                            .Where(c => c.isDefault == isDefault)
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

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //long cId = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("isDefault"))
            //{
            //    cId = Convert.ToInt32(headers.GetValues("isDefault").First());
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var list = entity.countriesCodes
            //       .Where(c => c.isDefault == cId)
            //       .Select(c => new {
            //           c.countryId,
            //           c.code,
            //           c.currency,
            //           c.name,
            //           c.isDefault,
            //           c.currencyId,
            //       })
            //       .FirstOrDefault();

            //        if (list == null)
            //            return NotFound();
            //        else
            //            return Ok(list);
            //    }
            //}
            //else
            //    return NotFound();
        }

        public string TimeZoneDiff(string tzone1name, string tzone2name)
        {
            //program-servertimez
            var tzone1 = TimeZoneInfo.FindSystemTimeZoneById(tzone1name);
            var tzone2 = TimeZoneInfo.FindSystemTimeZoneById(tzone2name);
            var now = DateTimeOffset.UtcNow;
            TimeSpan tzone1span = tzone1.GetUtcOffset(now);
            TimeSpan tzone2span = tzone2.GetUtcOffset(now);
            TimeSpan difference = tzone1span - tzone2span;

            return difference.ToString();
        }

        public TimeSpan offsetTime()
        {
            CountryModel country = new CountryModel();

            //server timezone
            TimeZone serverTimeZone = TimeZone.CurrentTimeZone;
            string ServerStandardName = serverTimeZone.StandardName;
            //program time zone
            country = GetDefaultCountry();
            string programStandardName = country.timeZoneName;
            string timeoffset = TimeZoneDiff(programStandardName, ServerStandardName);
            TimeSpan offset = TimeSpan.Parse(timeoffset);
            return offset;
        }

        public DateTime AddOffsetTodate(DateTime date)
        {
            TimeSpan ts = new TimeSpan();
            ts = offsetTime();
            date = date.AddHours(ts.TotalHours);
            return date;
        }

        public CountryModel GetDefaultCountry()
        {
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    CountryModel item = entity
                        .countriesCodes
                        .Where(c => c.isDefault == 1)
                        .Select(
                            c =>
                                new CountryModel
                                {
                                    countryId = c.countryId,
                                    code = c.code,
                                    currency = c.currency,
                                    name = c.name,
                                    isDefault = c.isDefault,
                                    currencyId = c.currencyId,
                                    timeZoneName = c.timeZoneName,
                                    timeZoneOffset = c.timeZoneOffset,
                                }
                        )
                        .FirstOrDefault();

                    return item;
                }
            }
            catch
            {
                CountryModel cntry = new CountryModel();
                return cntry;
            }
        }
    }
}
