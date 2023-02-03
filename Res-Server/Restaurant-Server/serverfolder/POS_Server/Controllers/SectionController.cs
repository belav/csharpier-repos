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
    [RoutePrefix("api/Sections")]
    public class SectionController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        // GET api/<controller>
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
            Boolean canDelete = false;
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var sectionList =(from L in entity.sections join b in entity.branches on L.branchId equals b.branchId into lj
                                      from v in lj.DefaultIfEmpty()
                                      select new SectionModel()
                                        {
                                          sectionId=  L.sectionId,
                                            name=   L.name,
                                            isActive=  L.isActive,
                                            isFreeZone=  L.isFreeZone,
                                          branchId =  L.branchId,
                                            notes=   L.notes,
                                            branchName = v.name,
                                            createDate=  L.createDate,
                                            updateDate=    L.updateDate,
                                            createUserId=  L.createUserId,
                                            updateUserId=  L.updateUserId,
                                            type=L.type,
                       
                                        })
                                        .ToList();

                    if (sectionList.Count > 0)
                    {// for each 
                        for (int i = 0; i < sectionList.Count; i++)
                        {
                            if (sectionList[i].isActive == 1)
                            {
                                long sectionId = (long)sectionList[i].sectionId;
                                var LocationL = entity.locations.Where(x => x.sectionId == sectionId).Select(b => new { b.locationId }).FirstOrDefault();
                                //var itemsTransferL = entity.itemsTransfer.Where(x => x.locationIdNew == locationId || x.locationIdOld == locationId).Select(x => new { x.itemsTransId }).FirstOrDefault();
                               
                                if ((LocationL is null)  )
                                    canDelete = true;
                            }
                            sectionList[i].canDelete = canDelete;
                        }
                    }
                     
                    return TokenManager.GenerateToken(sectionList);
                }
            }
        }
        [HttpPost]
        [Route("getBranchSections")]
        public string getBranchSections(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            Boolean canDelete = false;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long branchId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        branchId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var sectionList =(from L in entity.sections where L.branchId == branchId && L.isFreeZone != 1 && L.isKitchen != 1
                                      join b in entity.branches on L.branchId equals b.branchId into lj
                                      from v in lj.DefaultIfEmpty()
                                      select new SectionModel()
                                        {
                                          sectionId=  L.sectionId,
                                            name=   L.name,
                                            isActive=  L.isActive,
                                            isFreeZone=  L.isFreeZone,
                                          branchId =  L.branchId,
                                            notes=   L.notes,
                                            branchName = v.name,
                                            createDate=  L.createDate,
                                            updateDate=    L.updateDate,
                                            createUserId=  L.createUserId,
                                            updateUserId=  L.updateUserId,
                                          type = L.type,

                                      })
                                        .ToList();

                    if (sectionList.Count > 0)
                    {// for each 
                        for (int i = 0; i < sectionList.Count; i++)
                        {
                            if (sectionList[i].isActive == 1)
                            {
                                long sectionId = (long)sectionList[i].sectionId;
                                var LocationL = entity.locations.Where(x => x.sectionId == sectionId).Select(b => new { b.locationId }).FirstOrDefault();
                                //var itemsTransferL = entity.itemsTransfer.Where(x => x.locationIdNew == locationId || x.locationIdOld == locationId).Select(x => new { x.itemsTransId }).FirstOrDefault();
                               
                                if ((LocationL is null)  )
                                    canDelete = true;
                            }
                            sectionList[i].canDelete = canDelete;
                        }
                    }

                    return TokenManager.GenerateToken(sectionList);
                }
            }
        }
        // GET api/<controller>
        [HttpPost]
        [Route("GetSectionByID")]
        public string GetSectionByID(string token)
        {
token = TokenManager.readToken(HttpContext.Current.Request);
var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long sectionId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        sectionId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var location = entity.sections
                   .Where(u => u.sectionId == sectionId)
                   .Select(L => new SectionModel
                   {
                       sectionId = L.sectionId,
                       name = L.name,
                       isActive = L.isActive,
                       isFreeZone = L.isFreeZone,
                       branchId = L.branchId,
                       notes = L.notes,
                      
                       createDate = L.createDate,
                       updateDate = L.updateDate,
                       createUserId = L.createUserId,
                       updateUserId = L.updateUserId,
                       type = L.type,

                   })
                   .FirstOrDefault();
                    return TokenManager.GenerateToken(location);
                }
            }
         }
        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
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
                string sectionObject = "";
                sections newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        sectionObject = c.Value.Replace("\\", string.Empty);
                        sectionObject = sectionObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<sections>(sectionObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }
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
                if (newObject.branchId == 0 || newObject.branchId == null)
                {
                    Nullable<long> id = null;
                    newObject.branchId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var sectionEntity = entity.Set<sections>();
                        if (newObject.sectionId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            sectionEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.sectionId.ToString();
                        }
                        else
                        {
                            var tmpSection = entity.sections.Where(p => p.sectionId == newObject.sectionId).FirstOrDefault();
                            tmpSection.name = newObject.name;
                            tmpSection.branchId = newObject.branchId;
                            tmpSection.isActive = newObject.isActive;
                            tmpSection.isFreeZone = newObject.isFreeZone;
                            tmpSection.notes = newObject.notes;
                            tmpSection.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpSection.updateUserId = newObject.updateUserId;
                            tmpSection.type = newObject.type;
                            entity.SaveChanges();
                            message = tmpSection.sectionId.ToString(); 
                        }
                      
                    }
                }
                catch
                {
                    message = "-1";
                }
            }
            return TokenManager.GenerateToken(message);
        }
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
                long sectionId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        sectionId = long.Parse(c.Value);
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
                            sections sectionDelete = entity.sections.Find(sectionId);
                            entity.sections.Remove(sectionDelete);
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
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            sections sectionDelete = entity.sections.Find(sectionId);

                            sectionDelete.isActive = 0;
                            sectionDelete.updateUserId = userId;
                            sectionDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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
            }
        }
    }
}
