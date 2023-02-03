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
    [RoutePrefix("api/itemsProp")]
    public class itemsPropController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            //long itemId
            string message = "";
            long itemId = 0;

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
                    if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var itemPropsList = (
                            from a in entity.itemsProp.Where(i => i.itemId == itemId)
                            join b in entity.propertiesItems
                                on a.propertyItemId equals b.propertyItemId
                            join x in entity.properties on b.propertyId equals x.propertyId
                            select new itemsPropModel()
                            {
                                itemPropId = a.itemPropId,
                                propertyItemId = a.propertyItemId,
                                itemId = a.itemId,
                                propValue = b.name,
                                propName = x.name,
                                createDate = a.createDate,
                                updateDate = a.updateDate,
                                createUserId = a.createUserId,
                                updateUserId = a.updateUserId,
                            }
                        ).ToList();

                        return TokenManager.GenerateToken(itemPropsList);
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
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
            //        var itemPropsList = (from a in entity.itemsProp.Where(i => i.itemId == itemId)
            //        join b in entity.propertiesItems on a.propertyItemId equals b.propertyItemId
            //        join x in entity.properties on b.propertyId equals x.propertyId
            //        select new itemsPropModel() {
            //             itemPropId = a.itemPropId,
            //             propertyItemId = a.propertyItemId,
            //             itemId = a.itemId,
            //             propValue = b.name,
            //             propName = x.name,
            //             createDate = a.createDate,
            //             updateDate = a.updateDate,
            //             createUserId = a.createUserId,
            //             updateUserId = a.updateUserId,
            //        }).ToList();

            //        if (itemPropsList == null)
            //            return NotFound();
            //        else
            //            return Ok(itemPropsList);
            //    }
            //}
            //return NotFound();
        }

        // add or update items property
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            string message = "";

            //string itemsPropObject

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                itemsProp newObject = new itemsProp();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<itemsProp>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
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
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var itemPropEntity = entity.Set<itemsProp>();
                            if (newObject.itemPropId == 0)
                            {
                                newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateUserId = newObject.createUserId;

                                itemPropEntity.Add(newObject);
                                //  message = "Property Is Added To Item Successfully";
                            }
                            else
                            {
                                var tmpLocation = entity.itemsProp
                                    .Where(p => p.itemPropId == newObject.itemPropId)
                                    .FirstOrDefault();
                                tmpLocation.propertyItemId = newObject.propertyItemId;
                                tmpLocation.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                tmpLocation.updateUserId = newObject.updateUserId;

                                // message = "Property Is Updated Successfully";
                            }
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
                    return TokenManager.GenerateToken("0");
                }
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
            //    itemsPropObject = itemsPropObject.Replace("\\", string.Empty);
            //    itemsPropObject = itemsPropObject.Trim('"');
            //    itemsProp newObject = JsonConvert.DeserializeObject<itemsProp>(itemsPropObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
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
            //            var itemPropEntity = entity.Set<itemsProp>();
            //            if (newObject.itemPropId == 0)
            //            {
            //                newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                newObject.updateUserId = newObject.createUserId;

            //                itemPropEntity.Add(newObject);
            //                message = "Property Is Added To Item Successfully";
            //            }
            //            else
            //            {
            //                var tmpLocation = entity.itemsProp.Where(p => p.itemPropId == newObject.itemPropId).FirstOrDefault();
            //                tmpLocation.propertyItemId = newObject.propertyItemId;
            //                tmpLocation.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                tmpLocation.updateUserId = newObject.updateUserId;

            //                message = "Property Is Updated Successfully";
            //            }
            //            entity.SaveChanges();
            //        }
            //    }
            //    catch
            //    {
            //        message = "an error ocurred";
            //    }
            //}
            //return message;
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            //long itemPropId
            string message = "";
            long itemPropId = 0;

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
                    if (c.Type == "itemPropId")
                    {
                        itemPropId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        itemsProp itemPropObject = entity.itemsProp.Find(itemPropId);
                        entity.itemsProp.Remove(itemPropObject);
                        message = entity.SaveChanges().ToString();

                        // return Ok("Item Property is Deleted Successfully");
                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
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
            //            itemsProp itemPropObject = entity.itemsProp.Find(itemPropId);
            //            entity.itemsProp.Remove(itemPropObject);
            //            entity.SaveChanges();

            //            return Ok("Item Property is Deleted Successfully");
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
