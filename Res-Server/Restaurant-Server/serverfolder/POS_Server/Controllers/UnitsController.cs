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
    [RoutePrefix("api/Units")]
    public class UnitsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        List<long> unitsIds = new List<long>();

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
                    var unitsList = (
                        from u in entity.units
                        join b in entity.units
                            on new { UnitModel = u.smallestId } equals new
                            {
                                UnitModel = (long?)b.unitId
                            }
                            into lj
                        from x in lj.DefaultIfEmpty()
                        select new UnitModel()
                        {
                            unitId = u.unitId,
                            name = u.name,
                            isSmallest = u.isSmallest,
                            smallestUnit = x.name,
                            parentid = u.parentid,
                            smallestId = u.smallestId,
                            notes = u.notes,
                            createDate = u.createDate,
                            createUserId = u.createUserId,
                            updateDate = u.updateDate,
                            updateUserId = u.updateUserId,
                            isActive = u.isActive,
                        }
                    ).ToList();

                    if (unitsList.Count > 0)
                    {
                        for (int i = 0; i < unitsList.Count; i++)
                        {
                            canDelete = false;
                            if (unitsList[i].isActive == 1)
                            {
                                long unitId = (long)unitsList[i].unitId;
                                var itemsL = entity
                                    .items
                                    .Where(x => x.minUnitId == unitId)
                                    .Select(b => new { b.itemId })
                                    .ToList();
                                var itemsMatL = entity
                                    .itemsMaterials
                                    .Where(x => x.unitId == unitId)
                                    .Select(x => new { x.itemMatId })
                                    .FirstOrDefault();
                                var itemsUnitL = entity
                                    .itemsUnits
                                    .Where(x => x.unitId == unitId)
                                    .Select(x => new { x.itemUnitId })
                                    .FirstOrDefault();
                                var unitsL = entity
                                    .units
                                    .Where(x => x.parentid == unitId)
                                    .Select(x => new { x.unitId })
                                    .FirstOrDefault();
                                if (
                                    (itemsL.Count == 0)
                                    && (itemsMatL is null)
                                    && (itemsUnitL is null)
                                    && (unitsL is null)
                                )
                                    canDelete = true;
                            }

                            unitsList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(unitsList);
                }
            }
        }

        [HttpPost]
        [Route("GetU")]
        public string GetU(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);

            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var unitsList = (
                        from u in entity.units

                        select new
                        {
                            unitId = u.unitId,
                            name = u.name,
                            isSmallest = u.isSmallest,
                            parentid = u.parentid,
                            smallestId = u.smallestId,
                            notes = u.notes,
                            createDate = u.createDate,
                            createUserId = u.createUserId,
                            updateDate = u.updateDate,
                            updateUserId = u.updateUserId,
                            isActive = u.isActive,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(unitsList);
                }
            }
        }

        [HttpPost]
        [Route("getSmallUnits")]
        public string getSmallUnits(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long itemId = 0;
                long unitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemId = long.Parse(c.Value);
                    }
                    else if (c.Type == "unitId")
                    {
                        unitId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    // get all sub categories of categoryId
                    List<itemsUnits> unitsList = entity
                        .itemsUnits
                        .ToList()
                        .Where(x => x.itemId == itemId)
                        .Select(
                            p =>
                                new itemsUnits
                                {
                                    unitId = p.unitId,
                                    //name = p.name,
                                    subUnitId = p.subUnitId,
                                }
                        )
                        .ToList();

                    unitsIds = new List<long>();

                    var result = Recursive(unitsList, unitId);

                    var units = (
                        from u in entity.units
                        select new UnitModel()
                        {
                            unitId = u.unitId,
                            name = u.name,
                            isSmallest = u.isSmallest,
                            parentid = u.parentid,
                            smallestId = u.smallestId,
                            notes = u.notes,
                            createDate = u.createDate,
                            createUserId = u.createUserId,
                            updateDate = u.updateDate,
                            updateUserId = u.updateUserId,
                            isActive = u.isActive,
                        }
                    ).Where(p => !unitsIds.Contains((long)p.unitId)).ToList();
                    return TokenManager.GenerateToken(units);
                }
            }
        }

        [HttpPost]
        [Route("GetActive")]
        public string GetActive(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var unitsList = entity
                        .units
                        .Where(u => u.isActive == 1)
                        .Select(
                            u =>
                                new
                                {
                                    u.createDate,
                                    u.createUserId,
                                    u.isSmallest,
                                    u.name,
                                    u.parentid,
                                    u.smallestId,
                                    u.unitId,
                                    u.updateDate,
                                    u.updateUserId,
                                    u.notes,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(unitsList);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetUnitByID")]
        public string GetUnitByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long unitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        unitId = long.Parse(c.Value);
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var unit = entity
                        .units
                        .Where(u => u.unitId == unitId)
                        .Select(
                            u =>
                                new
                                {
                                    u.createDate,
                                    u.createUserId,
                                    u.isSmallest,
                                    u.name,
                                    u.parentid,
                                    u.smallestId,
                                    u.unitId,
                                    u.updateDate,
                                    u.updateUserId,
                                    u.notes,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(unit);
                }
            }
        }

        // add or update unit
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
                string unitObject = "";
                units Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        unitObject = c.Value.Replace("\\", string.Empty);
                        unitObject = unitObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<units>(
                            unitObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        units tmpUnit = new units();
                        var unitEntity = entity.Set<units>();
                        if (Object.unitId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;

                            unitEntity.Add(Object);
                            tmpUnit = unitEntity.Add(Object);
                            entity.SaveChanges();
                            message = tmpUnit.unitId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            tmpUnit = entity
                                .units
                                .Where(p => p.unitId == Object.unitId)
                                .FirstOrDefault();
                            tmpUnit.name = Object.name;
                            tmpUnit.notes = Object.notes;
                            tmpUnit.isSmallest = Object.isSmallest;
                            tmpUnit.smallestId = Object.smallestId;
                            tmpUnit.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpUnit.updateUserId = Object.updateUserId;
                            tmpUnit.parentid = Object.parentid;
                            tmpUnit.isActive = Object.isActive;
                            entity.SaveChanges();
                            message = tmpUnit.unitId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                catch
                {
                    message = "0";
                }
                return TokenManager.GenerateToken(message);
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
                long unitId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        unitId = long.Parse(c.Value);
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
                            units unitDelete = entity.units.Find(unitId);
                            entity.units.Remove(unitDelete);
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
                            units unitDelete = entity.units.Find(unitId);
                            unitDelete.isActive = 0;
                            unitDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            unitDelete.updateUserId = userId;
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

        public IEnumerable<itemsUnits> Recursive(List<itemsUnits> unitsList, long smallLevelid)
        {
            List<itemsUnits> inner = new List<itemsUnits>();

            foreach (var t in unitsList.Where(item => item.subUnitId == smallLevelid))
            {
                if (t.unitId.Value != smallLevelid)
                {
                    unitsIds.Add(t.unitId.Value);
                    inner.Add(t);
                }
                if (t.unitId.Value == smallLevelid)
                    return inner;
                inner = inner.Union(Recursive(unitsList, t.unitId.Value)).ToList();
            }

            return inner;
        }
        //private IEnumerable<units> Traverse(IEnumerable<itemsUnits> units)
        //{
        //    using (incposdbEntities entity = new incposdbEntities())
        //    {
        //        foreach (var category in units)
        //        {
        //            var subCategories = entity.itemsUnits.Where(x => x.subUnitId == category.unitId).ToList();
        //            category.Children = subCategories;
        //            category.Children = Traverse(category.Children).ToList();
        //        }
        //    }
        //    return categories;
        //}
    }
}
