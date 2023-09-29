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
    [RoutePrefix("api/city")]
    public class cityController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("Get")]
        public string Get(string token)
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
                        var list = entity.cities
                            .Select(
                                c =>
                                    new
                                    {
                                        c.cityId,
                                        c.cityCode,
                                        c.countryId
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
            //        var cityList = entity.cities

            //       .Select(c => new {
            //        c.cityId,
            //        c.cityCode,
            //        c.countryId
            //       })
            //       .ToList();

            //        if (cityList == null)
            //            return NotFound();
            //        else
            //            return Ok(cityList);
            //    }
            //}
            ////else
            //    return NotFound();
        }
    }
}
