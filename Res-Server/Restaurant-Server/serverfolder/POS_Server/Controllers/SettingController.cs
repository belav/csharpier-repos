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
    [RoutePrefix("api/setting")]
    public class SettingController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all setting
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
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

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity.setting
                            .Select(
                                c =>
                                    new
                                    {
                                        c.settingId,
                                        c.name,
                                        c.notes,
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
            //                var settingList = entity.setting

            //               .Select(c => new  {
            //                 c.settingId ,
            //                 c.name,
            //                 c.notes,

            //})
            //               .ToList();



            //                if (settingList == null)
            //                    return NotFound();
            //                else
            //                    return Ok(settingList);
            //            }
            //        }
            //        //else
            //            return NotFound();
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            // public ResponseVM GetPurinv(string token)int printerId



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
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var item = entity.setting
                        .Where(c => c.settingId == Id)
                        .Select(
                            c =>
                                new
                                {
                                    c.settingId,
                                    c.name,
                                    c.notes,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(item);
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
            //        var medal = entity.setting
            //       .Where(c => c.settingId == cId)
            //       .Select(c => new {
            //           c.settingId,
            //           c.name,
            //           c.notes,


            //       })
            //       .FirstOrDefault();

            //        if (medal == null)
            //            return NotFound();
            //        else
            //            return Ok(medal);
            //    }
            //}
            //else
            //    return NotFound();
        }

        // GET api/<controller> get all setting
        [HttpPost]
        [Route("GetByNotes")]
        public string GetByNotes(string token)
        {
            // public ResponseVM GetPurinv(string token)string notes



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string notes = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "notes")
                    {
                        notes = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<setting> settingList1 = entity.setting.ToList();
                    var list = settingList1
                        .Where(c => c.notes == notes)
                        .Select(
                            c =>
                                new setting
                                {
                                    settingId = c.settingId,
                                    name = c.name,
                                    notes = c.notes,
                                }
                        )
                        .ToList();

                    return TokenManager.GenerateToken(list);
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
            //        List<setting> settingList1 = entity.setting.ToList();
            //    var    settingList = settingList1.Where(c => c.notes == notes).Select(c => new setting
            //        {
            //            settingId = c.settingId,
            //            name = c.name,
            //            notes = c.notes,
            //        }).ToList();


            //        if (settingList == null)
            //            return NotFound();
            //        else
            //            return Ok(settingList);
            //    }
            //}
            ////else
            //return NotFound();
        }

        // add or update medal
        [HttpPost]
        [Route("Save")]
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
                setting newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<setting>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    setting tmpObject;

                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var sEntity = entity.Set<setting>();
                            if (newObject.settingId == 0)
                            {
                                sEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.settingId.ToString();
                            }
                            else
                            {
                                tmpObject = entity.setting
                                    .Where(p => p.settingId == newObject.settingId)
                                    .FirstOrDefault();

                                tmpObject.settingId = newObject.settingId;
                                tmpObject.name = newObject.name;
                                tmpObject.notes = newObject.notes;

                                entity.SaveChanges();
                                message = tmpObject.settingId.ToString();
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
            //    newObject = newObject.Replace("\\", string.Empty);
            //    newObject = newObject.Trim('"');
            //    setting Object = JsonConvert.DeserializeObject<setting>(newObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var sEntity = entity.Set<setting>();
            //            if (Object.settingId == 0)
            //            {

            //                sEntity.Add(Object);
            //                entity.SaveChanges();
            //                message = Object.settingId.ToString();
            //            }
            //            else
            //            {

            //                var tmps = entity.setting.Where(p => p.settingId == Object.settingId).FirstOrDefault();



            //                tmps.settingId = Object.settingId;
            //                tmps.name = Object.name;
            //           tmps.notes = Object.notes;



            //            entity.SaveChanges();
            //               message = tmps.settingId.ToString();
            //            }

            //        }
            //        return message;
            //    }

            //    catch
            //    {
            //        return "-1";
            //    }
            //}
            //else
            //    return "-1";
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            // public ResponseVM Delete(string token)
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
                        setting sObj = entity.setting.Find(Id);

                        entity.setting.Remove(sObj);
                        message = entity.SaveChanges().ToString();

                        //  return Ok("medal is Deleted Successfully");
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
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}

            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //if (valid)
            //{

            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                setting sObj = entity.setting.Find(Id);

            //                entity.setting.Remove(sObj);
            //                entity.SaveChanges();

            //                return Ok("medal is Deleted Successfully");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }




            //}
            //else
            //    return NotFound();
        }
    }
}
