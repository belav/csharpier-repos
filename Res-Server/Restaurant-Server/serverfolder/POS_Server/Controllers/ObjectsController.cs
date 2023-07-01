using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/Object")]
    public class ObjectsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all Objects
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                bool canDelete = false;
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var List = entity.objects
                            .Select(
                                c =>
                                    new ObjectsModel
                                    {
                                        objectId = c.objectId,
                                        name = c.name,
                                        parentObjectName = c.parentObjectName,
                                        //parentObjectId = c.parentObjectId,
                                        objectType = c.objectType,
                                        translate = c.translate,
                                        icon = c.icon,
                                        translateHint = c.translateHint,
                                    }
                            )
                            .ToList();
                        if (List.Count > 0)
                        {
                            for (int i = 0; i < List.Count; i++)
                            {
                                canDelete = false;
                                if (List[i].isActive == 1)
                                {
                                    long objectId = (long)List[i].objectId;
                                    var operationsL = entity.groupObject
                                        .Where(x => x.objectId == objectId)
                                        .Select(b => new { b.id })
                                        .FirstOrDefault();

                                    if (operationsL is null)
                                        canDelete = true;
                                }
                                List[i].canDelete = canDelete;
                            }
                        }

                        return TokenManager.GenerateToken(List);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            // public string GetUsersByGroupId(string token)
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

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity.objects
                            .Where(c => c.objectId == Id)
                            .Select(
                                c =>
                                    new
                                    {
                                        c.objectId,
                                        c.name,
                                        // c.parentObjectId,
                                        c.parentObjectName,
                                        c.objectType,
                                        c.translate,
                                        c.icon,
                                        c.translateHint,
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
        }

        // add or update
        [HttpPost]
        [Route("Save")]
        public String Save(string token)
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
                string Object = "";
                objects newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<objects>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    //   bondes tmpObject = null;


                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var sEntity = entity.Set<objects>();
                            if (newObject.objectId == 0)
                            {
                                sEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.objectId.ToString();
                            }
                            else
                            {
                                var tmps = entity.objects
                                    .Where(p => p.objectId == newObject.objectId)
                                    .FirstOrDefault();

                                tmps.objectId = newObject.objectId;
                                tmps.name = newObject.name;

                                //  tmps.parentObjectId = newObject.parentObjectId;
                                tmps.objectType = newObject.objectType;
                                tmps.translate = newObject.translate;
                                tmps.translateHint = newObject.translateHint;
                                tmps.parentObjectName = newObject.parentObjectName;
                                tmps.icon = newObject.icon;

                                entity.SaveChanges();
                                message = tmps.objectId.ToString();
                            }
                        }
                        // return message; ;
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
            //string message ="";
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
            //    objects Object = JsonConvert.DeserializeObject<objects>(newObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    try
            //    {


            //        if (Object.updateUserId == 0 || Object.updateUserId == null)
            //        {
            //            Nullable<long> id = null;
            //            Object.updateUserId = id;
            //        }
            //        if (Object.createUserId == 0 || Object.createUserId == null)
            //        {
            //            Nullable<long> id = null;
            //            Object.createUserId = id;
            //        }
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var sEntity = entity.Set<objects>();
            //            if (Object.objectId == 0)
            //            {
            //                Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Object.updateUserId = Object.createUserId;
            //                sEntity.Add(Object);
            //                entity.SaveChanges();
            //                 message = Object.objectId.ToString();
            //            }
            //            else
            //            {

            //                var tmps = entity.objects.Where(p => p.objectId == Object.objectId).FirstOrDefault();

            //                tmps.objectId=Object.objectId;
            //                tmps.name = Object.name;
            //                tmps.note = Object.note;
            //                tmps.note=Object.note;

            //                tmps.createDate=Object.createDate;
            //                tmps.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
            //                tmps.parentObjectId = Object.parentObjectId;
            //                tmps.objectType = Object.objectType;
            //                tmps.updateUserId = Object.updateUserId;
            //                entity.SaveChanges();
            //                message = tmps.objectId.ToString();
            //            }


            //        }
            //        return message; ;
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
            //int objectId, long userId, bool final

            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long objectId = 0;
                long userId = 0;
                bool final = false;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "objectId")
                    {
                        objectId = long.Parse(c.Value);
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
                            objects Deleterow = entity.objects.Find(objectId);
                            entity.objects.Remove(Deleterow);
                            message = entity.SaveChanges().ToString();
                            //  return Ok("OK");
                            return TokenManager.GenerateToken(message);

                            // return Ok("OK");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            objects Obj = entity.objects.Find(objectId);

                            message = entity.SaveChanges().ToString();
                            //  return Ok("OK");
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
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

            //    if (final)
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {

            //                objects Deleterow = entity.objects.Find(objectId);
            //                entity.objects.Remove(Deleterow);
            //                entity.SaveChanges();
            //                return Ok("OK");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {

            //                objects Obj = entity.objects.Find(objectId);
            //                Obj.isActive = 0;
            //                Obj.updateUserId = userId;
            //                Obj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                entity.SaveChanges();
            //                return Ok("Ok");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }
            //    }



            //}
            //else
            //    return NotFound();
        }
    }
}
