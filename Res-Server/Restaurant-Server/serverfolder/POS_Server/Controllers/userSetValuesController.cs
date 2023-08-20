using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/userSetValues")]
    public class userSetValuesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all userSetValues
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            //public stringGetPurinv(string token)




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
                        var list = entity.userSetValues
                            .Select(
                                c =>
                                    new
                                    {
                                        c.id,
                                        c.userId,
                                        c.valId,
                                        c.notes,
                                        c.createDate,
                                        c.updateDate,
                                        c.createUserId,
                                        c.updateUserId,
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
            //var headers = re.Headers;
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
            //        var List = entity.userSetValues

            //       .Select(c => new  {
            //           c.id,
            //           c.userId,
            //           c.valId,
            //           c.note,
            //           c.createDate,
            //           c.updateDate,
            //           c.createUserId,
            //           c.updateUserId,

            //       })
            //       .ToList();



            //        if (List == null)
            //            return NotFound();
            //        else
            //            return Ok(List);
            //    }
            //}
            ////else
            //    return NotFound();
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            //public stringGetPurinv(string token)long emailId




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;

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
                        var item = entity.userSetValues
                            .Where(c => c.valId == Id)
                            .Select(
                                c =>
                                    new
                                    {
                                        c.id,
                                        c.userId,
                                        c.valId,
                                        c.notes,
                                        c.createDate,
                                        c.updateDate,
                                        c.createUserId,
                                        c.updateUserId,
                                    }
                            )
                            .FirstOrDefault();
                        return TokenManager.GenerateToken(item);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //var headers = re.Headers;
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
            //        var list = entity.userSetValues
            //       .Where(c => c.valId == cId)
            //       .Select(c => new {
            //           c.id,
            //           c.userId,
            //           c.valId,
            //           c.note,
            //           c.createDate,
            //           c.updateDate,
            //           c.createUserId,
            //           c.updateUserId,


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
        [Route("Saveu")]
        public string Save(string token)
        {
            //string Object string newObject
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
                userSetValues newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<userSetValues>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    userSetValues tmpObject = null;

                    try
                    {
                        if (newObject.updateUserId == 0 || newObject.updateUserId == null)
                        {
                            Nullable<long> id = null;
                            newObject.updateUserId = id;
                        }
                        if (newObject.createUserId == 0 || newObject.createUserId == null)
                        {
                            Nullable<long> id = null;
                            newObject.createUserId = id;
                        }
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var locationEntity = entity.Set<userSetValues>();
                            if (newObject.id == 0)
                            {
                                newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateUserId = newObject.createUserId;

                                locationEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.id.ToString();
                            }
                            else
                            {
                                tmpObject = entity.userSetValues
                                    .Where(p => p.id == newObject.id)
                                    .FirstOrDefault();

                                tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                tmpObject.updateUserId = newObject.updateUserId;

                                tmpObject.valId = newObject.valId;
                                tmpObject.userId = newObject.userId;
                                tmpObject.notes = newObject.notes;
                                tmpObject.settingId = newObject.settingId;
                                tmpObject.Value = newObject.Value;
                                entity.SaveChanges();

                                message = tmpObject.id.ToString();
                            }
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

                return TokenManager.GenerateToken(message);
            }
            //var re = Request;
            //var headers = re.Headers;
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
            //    // return Object.ToString();
            //    Object = Object.Replace("\\", string.Empty);
            //    Object = Object.Trim('"');
            //    userSetValues newObject = JsonConvert.DeserializeObject<userSetValues>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (newObject.updateUserId == 0 || newObject.updateUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.updateUserId = id;
            //    }
            //    if (newObject.createUserId == 0 || newObject.createUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.createUserId = id;
            //    }

            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var locationEntity = entity.Set<userSetValues>();
            //            if (newObject.id == 0)
            //            {
            //                newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateUserId = newObject.createUserId;


            //                locationEntity.Add(newObject);
            //                entity.SaveChanges();
            //                message = newObject.id.ToString();
            //            }
            //            else
            //            {
            //                var tmpObject = entity.userSetValues.Where(p => p.id == newObject.id).FirstOrDefault();

            //                tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                tmpObject.updateUserId = newObject.updateUserId;

            //                tmpObject.valId = newObject.valId;
            //                tmpObject.userId = newObject.userId;
            //                tmpObject.note = newObject.note;

            //                entity.SaveChanges();

            //                message = tmpObject.id.ToString();
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
        [Route("Delete")]
        public string Delete(string token)
        {
            //public stringDelete(string token)long Id, long userId
            //long Id, long userId
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Id")
                    {
                        Id = long.Parse(c.Value);
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
                        userSetValues sObj = entity.userSetValues.Find(Id);

                        entity.userSetValues.Remove(sObj);
                        message = entity.SaveChanges().ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
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
            //            userSetValues sObj = entity.userSetValues.Find(Id);

            //            entity.userSetValues.Remove(sObj);
            //            entity.SaveChanges();

            //            return Ok(" Deleted Successfully");
            //        }
            //    }
            //    catch
            //    {
            //        return NotFound();
            //    }




            //}
            //else
            //    return NotFound();
        }
    }
}
