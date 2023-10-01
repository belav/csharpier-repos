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
    [RoutePrefix("api/Locations")]
    public class LocationsController : ApiController
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
                    var locationsList = (
                        from L in entity.locations
                        join s in entity.sections on L.sectionId equals s.sectionId into lj
                        from v in lj.DefaultIfEmpty()
                        select new LocationModel()
                        {
                            locationId = L.locationId,
                            x = L.x,
                            y = L.y,
                            z = L.z,
                            createDate = L.createDate,
                            updateDate = L.updateDate,
                            createUserId = L.createUserId,
                            updateUserId = L.updateUserId,
                            isActive = L.isActive,
                            isFreeZone = L.isFreeZone,
                            isKitchen = L.isKitchen,
                            branchId = L.branchId,
                            sectionId = L.sectionId,
                            sectionName = v.name,
                            branchName = L.branches.name,
                            notes = L.notes,
                        }
                    ).ToList();

                    if (locationsList.Count > 0)
                    {
                        for (int i = 0; i < locationsList.Count; i++)
                        {
                            if (locationsList[i].isActive == 1)
                            {
                                long locationId = (long)locationsList[i].locationId;
                                var itemsLocationL = entity
                                    .itemsLocations
                                    .Where(x => x.locationId == locationId)
                                    .Select(b => new { b.itemsLocId })
                                    .FirstOrDefault();

                                if ((itemsLocationL is null))
                                    canDelete = true;
                            }
                            locationsList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(locationsList);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetLocationByID")]
        public string GetLocationByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long locationId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        locationId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var location = entity
                        .locations
                        .Where(u => u.locationId == locationId)
                        .Select(
                            L =>
                                new
                                {
                                    L.locationId,
                                    L.x,
                                    L.y,
                                    L.z,
                                    L.createDate,
                                    L.updateDate,
                                    L.createUserId,
                                    L.updateUserId,
                                    L.isActive,
                                    L.isFreeZone,
                                    L.isKitchen,
                                    L.branchId,
                                    L.sectionId,
                                    note = L.notes,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(location);
                }
            }
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetLocsByBranchID")]
        public string GetLocsByBranchID(string token)
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
                    var locationsList = (
                        from L in entity.locations
                        join s in entity.sections on L.sectionId equals s.sectionId into lj
                        join b in entity.branches on L.branchId equals b.branchId into bj
                        from v in lj.DefaultIfEmpty()
                        from bbj in bj.DefaultIfEmpty()
                        where L.branchId == branchId
                        select new LocationModel()
                        {
                            locationId = L.locationId,
                            x = L.x,
                            y = L.y,
                            z = L.z,
                            createDate = L.createDate,
                            updateDate = L.updateDate,
                            createUserId = L.createUserId,
                            updateUserId = L.updateUserId,
                            isActive = L.isActive,
                            isFreeZone = L.isFreeZone,
                            isKitchen = L.isKitchen,
                            branchId = L.branchId,
                            sectionId = L.sectionId,
                            sectionName = v.name,
                            notes = L.notes,
                        }
                    ).ToList();

                    if (locationsList.Count > 0)
                    {
                        for (int i = 0; i < locationsList.Count; i++)
                        {
                            if (locationsList[i].isActive == 1)
                            {
                                long locationId = (long)locationsList[i].locationId;
                                var itemsLocationL = entity
                                    .itemsLocations
                                    .Where(x => x.locationId == locationId)
                                    .Select(b => new { b.itemsLocId })
                                    .FirstOrDefault();

                                if ((itemsLocationL is null))
                                    canDelete = true;
                            }
                            locationsList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(locationsList);
                }
            }
        }

        [HttpPost]
        [Route("GetLocsBySectionId")]
        public string GetLocsBySectionId(string token)
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
                    var locationsList = (
                        from L in entity.locations
                        where L.sectionId == sectionId
                        join s in entity.sections on L.sectionId equals s.sectionId into lj
                        from v in lj.DefaultIfEmpty()

                        select new LocationModel()
                        {
                            locationId = L.locationId,
                            x = L.x,
                            y = L.y,
                            z = L.z,
                            createDate = L.createDate,
                            updateDate = L.updateDate,
                            createUserId = L.createUserId,
                            updateUserId = L.updateUserId,
                            isActive = L.isActive,
                            isFreeZone = L.isFreeZone,
                            isKitchen = L.isKitchen,
                            branchId = L.branchId,
                            sectionId = L.sectionId,
                            sectionName = v.name,
                            notes = L.notes,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(locationsList);
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
                string locationObject = "";
                locations newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        locationObject = c.Value.Replace("\\", string.Empty);
                        locationObject = locationObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<locations>(
                            locationObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
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
                if (newObject.sectionId == 0 || newObject.sectionId == null)
                {
                    Nullable<long> id = null;
                    newObject.sectionId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<locations>();
                        if (newObject.locationId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.locationId.ToString();
                        }
                        else
                        {
                            var tmpLocation = entity
                                .locations
                                .Where(p => p.locationId == newObject.locationId)
                                .FirstOrDefault();
                            tmpLocation.x = newObject.x;
                            tmpLocation.y = newObject.y;
                            tmpLocation.z = newObject.z;
                            tmpLocation.branchId = newObject.branchId;
                            tmpLocation.isFreeZone = newObject.isFreeZone;
                            //tmpLocation.isKitchen = newObject.isKitchen;

                            tmpLocation.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpLocation.updateUserId = newObject.updateUserId;
                            tmpLocation.sectionId = newObject.sectionId;
                            tmpLocation.notes = newObject.notes;
                            entity.SaveChanges();

                            message = tmpLocation.locationId.ToString();
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
                long locationId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        locationId = long.Parse(c.Value);
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
                            locations locationDelete = entity.locations.Find(locationId);

                            entity.locations.Remove(locationDelete);
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
                            locations locationDelete = entity.locations.Find(locationId);

                            locationDelete.isActive = 0;
                            locationDelete.updateUserId = userId;
                            locationDelete.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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

        #region
        [HttpPost]
        [Route("UpdateLocBySecId")]
        public string UpdateLocationBySecId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            long sectionId = 0;
            int res = 0;
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string newloclist = "";
                List<locations> newlocObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        newloclist = c.Value.Replace("\\", string.Empty);
                        newloclist = newloclist.Trim('"');
                        newlocObj = JsonConvert.DeserializeObject<List<locations>>(
                            newloclist,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        break;
                    }
                }

                using (incposdbEntities entity = new incposdbEntities())
                {
                    var oldloc = entity.locations.Where(p => p.sectionId == sectionId);
                    if (oldloc.Count() > 0)
                    {
                        entity.locations.RemoveRange(oldloc);
                    }
                    if (newlocObj.Count() > 0)
                    {
                        foreach (locations newlocrow in newlocObj)
                        {
                            newlocrow.sectionId = sectionId;
                            if (newlocrow.createDate == null)
                            {
                                newlocrow.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newlocrow.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newlocrow.updateUserId = newlocrow.createUserId;
                            }
                            else
                            {
                                newlocrow.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            }
                        }
                        entity.locations.AddRange(newlocObj);
                    }
                    res = entity.SaveChanges();
                    return TokenManager.GenerateToken(res);
                }
            }
        }
        #endregion


        // add or update List of locations
        [HttpPost]
        [Route("AddLocationsToSection")]
        public string AddLocationsToSection(string token)
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
                string locationsObject = "";
                List<locations> Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        locationsObject = c.Value.Replace("\\", string.Empty);
                        locationsObject = locationsObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<List<locations>>(
                            locationsObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        //break;
                    }
                    else if (c.Type == "sectionId")
                    {
                        sectionId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var oldList = entity
                        .locations
                        .Where(x => x.sectionId == sectionId)
                        .Select(x => new { x.locationId })
                        .ToList();
                    for (int i = 0; i < oldList.Count; i++)
                    {
                        long locationId = (long)oldList[i].locationId;
                        var loc = entity.locations.Find(locationId);

                        if (Object != null && Object.Count > 0)
                        {
                            var isExist = Object.Find(x => x.locationId == oldList[i].locationId);
                            if (isExist == null) // unlink location to section
                            {
                                loc.sectionId = null;
                                loc.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                loc.updateUserId = userId;
                            }
                            else // edit location info
                            { }
                        }
                        else // clear section from location
                        {
                            loc.sectionId = null;
                            loc.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            loc.updateUserId = userId;
                        }
                    }
                    foreach (locations loc in Object) // loop to add new locations
                    {
                        Boolean isInList = false;
                        if (oldList != null)
                        {
                            var old = oldList.ToList().Find(x => x.locationId == loc.locationId);
                            if (old != null)
                            {
                                isInList = true;
                            }

                            if (!isInList)
                            {
                                var loc1 = entity.locations.Find(loc.locationId);
                                if (loc1.updateUserId == 0 || loc1.updateUserId == null)
                                {
                                    Nullable<long> id = null;
                                    loc1.updateUserId = id;
                                }
                                if (loc1.createUserId == 0 || loc1.createUserId == null)
                                {
                                    Nullable<long> id = null;
                                    loc1.createUserId = id;
                                }
                                loc1.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                loc1.sectionId = sectionId;
                                loc.updateUserId = userId;
                                //entity.SaveChanges();
                            }
                        }
                        try
                        {
                            entity.SaveChanges();
                        }
                        catch
                        {
                            message = "0";
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    entity.SaveChanges();
                }
            }
            message = "1";
            return TokenManager.GenerateToken(message);
        }
    }
}
