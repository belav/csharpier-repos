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
    [RoutePrefix("api/ItemUnitUser")]
    public class ItemUnitUserController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        //        [HttpPost]
        //        [Route("GetAll")]
        //        public IHttpActionResult GetAll()
        //        {
        //            var re = Request;
        //            var headers = re.Headers;
        //            string token = "";
        //            bool canDelete = false;

        //            if (headers.Contains("APIKey"))
        //            {
        //                token = headers.GetValues("APIKey").First();
        //            }
        //            Validation validation = new Validation();
        //            bool valid = validation.CheckApiKey(token);

        //            if (valid) // APIKey is valid
        //            {
        //                using (incposdbEntities entity = new incposdbEntities())
        //                {
        //                    var List = (from S in entity.itemUnitUser
        //                                select new ItemUnitUserModel()
        //                                {
        //                                    id = S.id,
        //                                    itemUnitId = S.itemUnitId,
        //                                    userId = S.userId,
        //                                    notes = S.notes,
        //                                    createDate = S.createDate,
        //                                    updateDate = S.updateDate,
        //                                    createUserId = S.createUserId,
        //                                    updateUserId = S.updateUserId,
        //                                    isActive = S.isActive,



        //                                }).ToList();
        //                    /*
        //                         public long id { get; set; }
        //        public Nullable<int> itemUnitId { get; set; }
        //        public Nullable<long> userId { get; set; }
        //        public string notes { get; set; }
        //        public Nullable<System.DateTime> createDate { get; set; }
        //        public Nullable<System.DateTime> updateDate { get; set; }
        //        public Nullable<long> createUserId { get; set; }
        //        public Nullable<long> updateUserId { get; set; }
        //        public Nullable<byte> isActive { get; set; }

        // id
        //itemUnitId
        //userId
        //notes
        //createDate
        //updateDate
        //createUserId
        //updateUserId
        //isActive



        //                    */



        //                    if (List == null)
        //                        return NotFound();
        //                    else
        //                        return Ok(List);
        //                }
        //            }
        //            //else
        //            return NotFound();
        //        }

        // GET api/<controller>
        //[HttpPost]
        //[Route("GetByID")]
        //public IHttpActionResult GetByID(long id)
        //{
        //    var re = Request;
        //    var headers = re.Headers;
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
        //            var row = entity.itemUnitUser
        //           .Where(u => u.id == id)
        //           .Select(S => new
        //           {
        //               S.id,
        //               S.itemUnitId,
        //               S.userId,
        //               S.notes,
        //               S.createDate,
        //               S.updateDate,
        //               S.createUserId,
        //               S.updateUserId,
        //               S.isActive,


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


        // GET api/<controller>
        [HttpPost]
        [Route("GetByUserId")]
        public string GetByUserId(string token)
        {
            // public string GetUsersByGroupId(string token)long userId


            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var row = entity
                            .itemUnitUser
                            .Where(u => u.userId == userId)
                            .Select(
                                S =>
                                    new
                                    {
                                        S.id,
                                        S.itemUnitId,
                                        S.userId,
                                        S.notes,
                                        S.createDate,
                                        S.updateDate,
                                        S.createUserId,
                                        S.updateUserId,
                                        S.isActive,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(row);
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

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var row = entity.itemUnitUser
            //       .Where(u => u.userId == userId)
            //       .Select(S => new
            //       {
            //           S.id,
            //           S.itemUnitId,
            //           S.userId,
            //           S.notes,
            //           S.createDate,
            //           S.updateDate,
            //           S.createUserId,
            //           S.updateUserId,
            //           S.isActive,


            //       })
            //       .ToList();

            //        if (row == null)
            //            return NotFound();
            //        else
            //            return Ok(row);
            //    }
            //}
            //else
            //    return NotFound();
        }

        // add or update location
        //[HttpPost]
        //[Route("Save")]
        //public string Save(string Object)
        //{
        //    var re = Request;
        //    var headers = re.Headers;
        //    string token = "";
        //    string message = "";
        //    if (headers.Contains("APIKey"))
        //    {
        //        token = headers.GetValues("APIKey").First();
        //    }
        //    Validation validation = new Validation();
        //    bool valid = validation.CheckApiKey(token);

        //    if (valid)
        //    {



        //        Object = Object.Replace("\\", string.Empty);
        //        Object = Object.Trim('"');
        //        itemUnitUser newObject = JsonConvert.DeserializeObject<itemUnitUser>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        //        if (newObject.updateUserId == 0 || newObject.updateUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.updateUserId = id;
        //        }
        //        if (newObject.createUserId == 0 || newObject.createUserId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.createUserId = id;
        //        }
        //        if (newObject.itemUnitId == 0 || newObject.itemUnitId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.itemUnitId = id;
        //        }

        //        if (newObject.userId == 0 || newObject.userId == null)
        //        {
        //            Nullable<long> id = null;
        //            newObject.userId = id;
        //        }


        //        try
        //        {
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                var locationEntity = entity.Set<itemUnitUser>();
        //                if (newObject.id == 0)
        //                {
        //                    newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateUserId = newObject.createUserId;


        //                    locationEntity.Add(newObject);
        //                    entity.SaveChanges();
        //                    message = newObject.id.ToString();
        //                }
        //                else
        //                {
        //                    var tmpObject = entity.itemUnitUser.Where(p => p.id == newObject.id).FirstOrDefault();

        //                    tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);


        //                    tmpObject.id = newObject.id;
        //                    tmpObject.itemUnitId = newObject.itemUnitId;
        //                    tmpObject.userId = newObject.userId;
        //                    tmpObject.notes = newObject.notes;
        //                    tmpObject.createDate = newObject.createDate;

        //                    // tmpObject.createUserId = newObject.createUserId;
        //                    tmpObject.updateUserId = newObject.updateUserId;
        //                    tmpObject.isActive = newObject.isActive;


        //                    entity.SaveChanges();

        //                    message = tmpObject.id.ToString();
        //                }
        //                //  entity.SaveChanges();
        //            }
        //        }
        //        catch
        //        {
        //            message = "-1";
        //        }
        //    }
        //    return message;
        //}

        #region
        [HttpPost]
        [Route("UpdateList")]
        public string UpdateList(string token)
        {
            // string newlist

            string message = "0";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long userId = 0;
                // long offerId = 0;

                List<itemUnitUser> newObject = new List<itemUnitUser>();
                string Object = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<itemUnitUser>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    int count = 0;

                    using (incposdbEntities entityd = new incposdbEntities())
                    {
                        List<itemUnitUser> objectDelete = entityd.itemUnitUser.ToList();
                        objectDelete = objectDelete.Where(d => d.userId == userId).ToList();
                        if (objectDelete != null)
                        {
                            entityd.itemUnitUser.RemoveRange(objectDelete);
                            message = entityd.SaveChanges().ToString();
                        }
                    }

                    foreach (itemUnitUser newrow in newObject)
                    {
                        message = saveRow(newrow, userId);
                        if (long.Parse(message) > 0)
                        {
                            count++;
                        }
                    }

                    // return count.ToString();
                    return TokenManager.GenerateToken(count.ToString());
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //string message = "";
            //long userId = 0;
            //long offerId = 0;
            //var re = Request;
            //var headers = re.Headers;
            //int res = 0;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("userId"))
            //{
            //    userId = Convert.ToInt32(headers.GetValues("userId").First());
            //}

            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //newlist = newlist.Replace("\\", string.Empty);
            //newlist = newlist.Trim('"');
            //List<itemUnitUser> newitofObj = JsonConvert.DeserializeObject<List<itemUnitUser>>(newlist, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //int count = 0;
            //if (valid)
            //{

            //    using (incposdbEntities entityd = new incposdbEntities())
            //    {
            //        List<itemUnitUser> objectDelete = entityd.itemUnitUser.ToList();
            //        objectDelete = objectDelete.Where(d => d.userId == userId).ToList();
            //        if (objectDelete != null)
            //        {
            //            entityd.itemUnitUser.RemoveRange(objectDelete);
            //            message = entityd.SaveChanges().ToString();
            //        }



            //    }


            //    foreach (itemUnitUser newrow in newitofObj)
            //    {
            //        message = saveRow(newrow, userId);
            //        if (int.Parse(message) > 0)
            //        {
            //            count++;
            //        }

            //    }
            //}
            //return count.ToString();
        }

        public string saveRow(itemUnitUser newObject, long userId)
        {
            string message = "";
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
            if (newObject.itemUnitId == 0 || newObject.itemUnitId == null)
            {
                Nullable<long> id = null;
                newObject.itemUnitId = id;
            }

            if (newObject.userId == 0 || newObject.userId == null)
            {
                Nullable<long> id = null;
                newObject.userId = id;
            }

            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var locationEntity = entity.Set<itemUnitUser>();
                    if (newObject.id == 0)
                    {
                        newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        newObject.updateUserId = userId;
                        newObject.userId = userId;

                        locationEntity.Add(newObject);
                        entity.SaveChanges();
                        message = newObject.id.ToString();
                    }
                    else
                    {
                        var tmpObject = entity
                            .itemUnitUser
                            .Where(p => p.id == newObject.id)
                            .FirstOrDefault();

                        tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                        tmpObject.id = newObject.id;
                        tmpObject.itemUnitId = newObject.itemUnitId;
                        tmpObject.userId = userId;
                        tmpObject.notes = newObject.notes;
                        tmpObject.createDate = newObject.createDate;

                        // tmpObject.createUserId = newObject.createUserId;
                        tmpObject.updateUserId = newObject.updateUserId;
                        tmpObject.isActive = newObject.isActive;

                        entity.SaveChanges();

                        message = tmpObject.id.ToString();
                    }
                    //  entity.SaveChanges();
                }
            }
            catch
            {
                message = "-1";
            }
            return message;
        }
        //[HttpPost]
        //[Route("Delete")]
        //public string Delete(long id)
        //{
        //    var re = Request;
        //    var headers = re.Headers;
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
        //                itemUnitUser objectDelete = entity.itemUnitUser.Find(id);

        //                entity.itemUnitUser.Remove(objectDelete);
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


        #endregion
    }
}
