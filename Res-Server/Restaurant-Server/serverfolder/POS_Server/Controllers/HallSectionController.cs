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
using System.Web;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/HallSection")]
    public class HallSectionController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
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
                    var sectionsList = entity.hallSections
                        .Select(
                            S =>
                                new HallSectionModel()
                                {
                                    name = S.name,
                                    sectionId = S.sectionId,
                                    branchId = S.branchId,
                                    details = S.details,
                                    notes = S.notes,
                                    createUserId = S.createUserId,
                                    updateUserId = S.updateUserId,
                                    createDate = S.createDate,
                                    updateDate = S.updateDate,
                                    isActive = (byte)S.isActive,
                                    branchName = S.branches.name,
                                }
                        )
                        .ToList();

                    // can delet or not
                    if (sectionsList.Count > 0)
                    {
                        foreach (HallSectionModel section in sectionsList)
                        {
                            canDelete = false;
                            if (section.isActive == 1)
                            {
                                long cId = (int)section.sectionId;
                                var tables = entity.tables
                                    .Where(x => x.sectionId == cId)
                                    .FirstOrDefault();

                                if (tables is null)
                                    canDelete = true;
                            }
                            section.canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(sectionsList);
                }
            }
        }

        [HttpPost]
        [Route("getBranchSections")]
        public string getBranchSections(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
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
                    var sectionList = (
                        from L in entity.hallSections
                        where L.branchId == branchId && L.isActive == 1
                        join b in entity.branches on L.branchId equals b.branchId into lj
                        from v in lj.DefaultIfEmpty()
                        select new SectionModel()
                        {
                            sectionId = L.sectionId,
                            name = L.name,
                            isActive = (byte)L.isActive,
                            branchId = L.branchId,
                            notes = L.notes,
                            branchName = v.name,
                            createDate = L.createDate,
                            updateDate = L.updateDate,
                            createUserId = L.createUserId,
                            updateUserId = L.updateUserId,
                            type = L.type,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(sectionList);
                }
            }
        }

        [HttpPost]
        [Route("GetById")]
        public string GetById(string token)
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
                    var location = entity.hallSections
                        .Where(u => u.sectionId == sectionId)
                        .Select(
                            L =>
                                new HallSectionModel
                                {
                                    sectionId = L.sectionId,
                                    name = L.name,
                                    isActive = (byte)L.isActive,
                                    branchId = L.branchId,
                                    notes = L.notes,
                                    details = L.details,
                                    createDate = L.createDate,
                                    updateDate = L.updateDate,
                                    createUserId = L.createUserId,
                                    updateUserId = L.updateUserId,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(location);
                }
            }
        }

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
                string itemObject = "";
                hallSections Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<hallSections>(
                            itemObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        hallSections tmpObject = new hallSections();
                        if (Object.sectionId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            Object.isActive = 1;
                            entity.hallSections.Add(Object);
                        }
                        else
                        {
                            tmpObject = entity.hallSections.Find(Object.sectionId);
                            tmpObject.name = Object.name;
                            tmpObject.branchId = Object.branchId;
                            tmpObject.details = Object.details;
                            tmpObject.notes = Object.notes;
                            tmpObject.isActive = Object.isActive;
                            tmpObject.updateUserId = Object.updateUserId;
                            tmpObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                        }
                        message = entity.SaveChanges().ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
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
                    if (c.Type == "sectionId")
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
                            hallSections tableObj = entity.hallSections.Find(sectionId);
                            entity.hallSections.Remove(tableObj);
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
                            hallSections tableObj = entity.hallSections.Find(sectionId);

                            tableObj.isActive = 0;
                            tableObj.updateUserId = userId;
                            tableObj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
