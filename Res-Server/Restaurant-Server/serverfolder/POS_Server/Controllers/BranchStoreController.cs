using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/BranchStore")]
    public class BranchStoreController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all Objects
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
            bool canDelete = false;
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (from S in entity.branchStore
                                 join B in entity.branches on S.branchId equals B.branchId into JBB 
                                 join BB in entity.branches on S.storeId equals BB.branchId into JSB
                                from JBBR in JBB.DefaultIfEmpty()
                                from JSBB in JSB.DefaultIfEmpty()
                                select new BranchStoreModel {
                      id = S.id,
                       branchId = S.branchId,
                       storeId = S.storeId,
                       notes = S.notes,
                    
                       createDate = S.createDate,
                       updateDate = S.updateDate,
                       createUserId = S.createUserId,
                       updateUserId = S.updateUserId,
                       isActive=S.isActive,
                       canDelete = true,
                                    // branch
                                    bbranchId = JSBB.branchId,
                                    bcode = JBBR.code,
                                    bname = JBBR.name,
                                    baddress = JBBR.address,
                                    bemail = JBBR.email,
                                    bphone = JBBR.phone,
                                    bmobile = JBBR.mobile,
                                    bcreateDate = JBBR.createDate,
                                    bupdateDate = JBBR.updateDate,
                                    bcreateUserId = JBBR.createUserId,
                                    bupdateUserId = JBBR.updateUserId,
                                    bnotes = JBBR.notes,
                                    bparentId = JBBR.parentId,
                                    bisActive = JBBR.isActive,
                                    btype = JBBR.type,
                                    //store
                                    sbranchId = JSBB.branchId,
                                    scode = JSBB.code,
                                    sname = JSBB.name,
                                    saddress = JSBB.address,
                                    semail = JSBB.email,
                                    sphone = JSBB.phone,
                                    smobile = JSBB.mobile,
                                    screateDate = JSBB.createDate,
                                    supdateDate = JSBB.updateDate,
                                    screateUserId = JSBB.createUserId,
                                    supdateUserId = JSBB.updateUserId,
                                    snotes = JSBB.notes,
                                    sparentId = JSBB.parentId,
                                    sisActive = JSBB.isActive,
                                    stype = JSBB.type,

                                }).ToList();
                    return TokenManager.GenerateToken(List);

                }
            }
        }

        [HttpPost]
        [Route("GetByBranchId")]
        public string GetByBranchId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            bool canDelete = false;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int branchId = 0;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        branchId = int.Parse(c.Value);
                    }
                    else if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List = (from S in entity.branchStore
                                join B in entity.branches on S.branchId equals B.branchId into JBB
                                join BB in entity.branches on S.storeId equals BB.branchId into JSB
                                from JBBR in JBB.DefaultIfEmpty()
                                from JSBB in JSB.DefaultIfEmpty()
                                where S.branchId == branchId && JSBB.type == type

                                select new BranchStoreModel
                                {
                                    id = S.id,
                                    branchId = S.branchId,
                                    storeId = S.storeId,
                                    notes = S.notes,

                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    isActive = S.isActive,
                                    canDelete = true,
                                    // branch
                                    bbranchId = JSBB.branchId,
                                    bcode = JBBR.code,
                                    bname = JBBR.name,
                                    baddress = JBBR.address,
                                    bemail = JBBR.email,
                                    bphone = JBBR.phone,
                                    bmobile = JBBR.mobile,
                                    bcreateDate = JBBR.createDate,
                                    bupdateDate = JBBR.updateDate,
                                    bcreateUserId = JBBR.createUserId,
                                    bupdateUserId = JBBR.updateUserId,
                                    bnotes = JBBR.notes,
                                    bparentId = JBBR.parentId,
                                    bisActive = JBBR.isActive,
                                    btype = JBBR.type,
                                    //store
                                    sbranchId = JSBB.branchId,
                                    scode = JSBB.code,
                                    sname = JSBB.name,
                                    saddress = JSBB.address,
                                    semail = JSBB.email,
                                    sphone = JSBB.phone,
                                    smobile = JSBB.mobile,
                                    screateDate = JSBB.createDate,
                                    supdateDate = JSBB.updateDate,
                                    screateUserId = JSBB.createUserId,
                                    supdateUserId = JSBB.updateUserId,
                                    snotes = JSBB.notes,
                                    sparentId = JSBB.parentId,
                                    sisActive = JSBB.isActive,
                                    stype = JSBB.type,

                                }).ToList();
                    return TokenManager.GenerateToken(List);
                }
            }
        }


        // GET api/<controller>  Get medal By ID 
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long cId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        cId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var list = entity.branchStore
                   .Where(c => c.id == cId)
                   .Select(c => new {
                    c.id,
                    c.branchId,
                    c.storeId,
                    c.notes,
                    c.createDate,
                    c.updateDate,
                    c.createUserId,
                    c.updateUserId,
                    c.isActive,
                   })
                   .FirstOrDefault();

                    return TokenManager.GenerateToken(list);
                }
            }
        }


        // add or update 
        [HttpPost]
        [Route("Save")]
        public String Save(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
            string message ="";
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Objects = "";
                branchStore Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Objects = c.Value.Replace("\\", string.Empty);
                        Objects = Objects.Trim('"');
                        Object = JsonConvert.DeserializeObject<branchStore>(Objects, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }

                try
                {
                    if (Object.branchId == 0 || Object.branchId == null)
                    {
                        Nullable<long> id = null;
                        Object.branchId = id;
                    }

                    if (Object.storeId == 0 || Object.storeId == null)
                    {
                        Nullable<long> id = null;
                        Object.storeId = id;
                    }
                    if (Object.updateUserId == 0 || Object.updateUserId == null)
                    {
                        Nullable<long> id = null;
                        Object.updateUserId = id;
                    }
                    if (Object.createUserId == 0 || Object.createUserId == null)
                    {
                        Nullable<long> id = null;
                        Object.createUserId = id;
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var sEntity = entity.Set<branchStore>();
                        if (Object.id == 0)
                        {
                            Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            sEntity.Add(Object);
                             message = Object.id.ToString();
                            entity.SaveChanges();
                        }
                        else
                        {

                            var tmps = entity.branchStore.Where(p => p.id == Object.id).FirstOrDefault();

                            tmps.id = Object.id;
                            tmps.branchId = Object.branchId;
                            tmps.storeId = Object.storeId;
                            tmps.notes = Object.notes;
                            tmps.isActive = Object.isActive;
                            tmps.notes = Object.notes;
                          
                            tmps.createDate=Object.createDate;
                            tmps.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
                            
                            tmps.updateUserId = Object.updateUserId;
                            entity.SaveChanges();
                            message = tmps.id.ToString();
                        }
                       
                       
                    }
                    return TokenManager.GenerateToken(message);
                }

                catch
                {
                    message =  "-1";
                    return TokenManager.GenerateToken(message);

                }
            }
        }
        //
        [HttpPost]
        [Route("UpdateStoresById")]
        public string UpdateStoresById(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {

                string branchStoreObject = "";
                List<branchStore> newListObj = null;
                long branchId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "newList")
                    {
                        branchStoreObject = c.Value.Replace("\\", string.Empty);
                        branchStoreObject = branchStoreObject.Trim('"');
                        newListObj = JsonConvert.DeserializeObject<List<branchStore>>(branchStoreObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        //break;
                    }
                    else if (c.Type == "branchId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                    else   if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                if (newListObj != null)
                {

               
                // delete old invoice items
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<branchStore> items = entity.branchStore.Where(x => x.branchId == branchId).ToList();
                    entity.branchStore.RemoveRange(items);
                    try { entity.SaveChanges(); }
                    catch { }

                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    for (int i = 0; i < newListObj.Count; i++)
                    {
                        if (newListObj[i].updateUserId == 0 || newListObj[i].updateUserId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].updateUserId = id;
                        }
                        if (newListObj[i].createUserId == 0 || newListObj[i].createUserId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].createUserId = id;
                        }
                        if (newListObj[i].branchId == 0 || newListObj[i].branchId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].branchId = id;
                        }
                        if (newListObj[i].storeId == 0 || newListObj[i].storeId == null)
                        {
                            Nullable<long> id = null;
                            newListObj[i].storeId = id;
                        }
                        var branchEntity = entity.Set<branchStore>();

                        newListObj[i].createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        newListObj[i].updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                        newListObj[i].updateUserId = newListObj[i].createUserId;
                        newListObj[i].branchId = branchId;
                        branchEntity.Add(newListObj[i]);

                    }
                    try
                    {
                      message=  entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }

                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                }
            }

           // message = "1";
            return TokenManager.GenerateToken(message);
        }

        //
        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
            string message = "";
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        Id = long.Parse(c.Value);
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

                            branchStore Deleterow = entity.branchStore.Find(Id);
                            entity.branchStore.Remove(Deleterow);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
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

                            branchStore Obj = entity.branchStore.Find(Id);
                            Obj.isActive = 0;
                            Obj.updateUserId = userId;
                            Obj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }



            }
        }
    }
}
