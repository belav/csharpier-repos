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

using Newtonsoft.Json.Converters;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/PaperSizeController")]
    public class PaperSizeController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
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
                        var list = (
                            from S in entity.paperSize
                            select new
                            {
                                S.sizeId,
                                S.paperSize1,
                                S.printfor,
                                S.sizeValue,
                            }
                        ).ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //
            //
            //        string token = "";


            //        if (headers.Contains("APIKey"))
            //        {
            //            token = headers.GetValues("APIKey").First();
            //        }
            //        Validation validation = new Validation();
            //        bool valid = validation.CheckApiKey(token);

            //        if (valid) // APIKey is valid
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                var List = (from S in entity.paperSize
            //                            select new
            //                            {
            //                                S.sizeId,
            //                                S.paperSize1,
            //                                S.printfor,
            //                                S.sizeValue ,

            //}).ToList();



            //                if (List == null)
            //                    return NotFound();
            //                else
            //                    return Ok(List);
            //            }
            //        }
            //        //else
            //        return NotFound();
        }

        // GET api/<controller>
        //[HttpPost]
        //[Route("GetByID")]
        //public IHttpActionResult GetByID(int sizeId)
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

        //    if (valid)
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var row = entity.paperSize
        //           .Where(u => u.sizeId == sizeId)
        //           .Select(S => new
        //           {
        //               S.sizeId,
        //               S.paperSize1,
        //               S.printfor,
        //               S.sizeValue,
        //           })
        //           .FirstOrDefault();

        //            if (row == null)
        //                return NotFound();
        //            else
        //                return Ok(row);
        //        }
        //    }
        //    else
        //        return NotFound();
        //}

        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
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
                string Object = "";
                paperSize newObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObj = JsonConvert.DeserializeObject<paperSize>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObj != null)
                {
                    try
                    {
                        paperSize tmpObject;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var locationEntity = entity.Set<paperSize>();
                            if (newObj.sizeId == 0)
                            {
                                locationEntity.Add(newObj);
                                entity.SaveChanges();
                                message = newObj.sizeId.ToString();
                            }
                            else
                            {
                                tmpObject = entity
                                    .paperSize
                                    .Where(p => p.sizeId == newObj.sizeId)
                                    .FirstOrDefault();

                                tmpObject.paperSize1 = newObj.paperSize1;
                                tmpObject.printfor = newObj.printfor;
                                tmpObject.sizeValue = newObj.sizeValue;

                                entity.SaveChanges();

                                message = tmpObject.sizeId.ToString();
                            }
                        }
                        return TokenManager.GenerateToken(message);
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }

                return TokenManager.GenerateToken(message);
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
            //    Object = Object.Replace("\\", string.Empty);
            //    Object = Object.Trim('"');
            //    paperSize newObject = JsonConvert.DeserializeObject<paperSize>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });


            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var locationEntity = entity.Set<paperSize>();
            //            if (newObject.sizeId == 0)
            //            {



            //                locationEntity.Add(newObject);
            //                entity.SaveChanges();
            //                message = newObject.sizeId.ToString();
            //            }
            //            else
            //            {
            //                var tmpObject = entity.paperSize.Where(p => p.sizeId == newObject.sizeId).FirstOrDefault();


            //                tmpObject.paperSize1 = newObject.paperSize1;
            //                tmpObject.printfor  = newObject.printfor;
            //                tmpObject.sizeValue = newObject.sizeValue;

            //                entity.SaveChanges();

            //                message = tmpObject.sizeId.ToString();
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

        //[HttpPost]
        //[Route("Delete")]
        //public string Delete(int sizeId)
        //{


        //    string token = "";
        //    int message = 0;
        //    if (headers.Contains("APIKey"))
        //    {
        //        token = headers.GetValues("APIKey").First();
        //    }

        //    Validation validation = new Validation();
        //    bool valid = validation.CheckApiKey(token);
        //    if (valid)
        //    {

        //        try
        //        {
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                paperSize objectDelete = entity.paperSize.Find(sizeId);

        //                entity.paperSize.Remove(objectDelete);
        //                message = entity.SaveChanges();

        //                return message.ToString();
        //            }
        //        }
        //        catch
        //        {
        //            return "-1";
        //        }

        //    }
        //    else
        //        return "-3";
        //}
    }
}
