using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    [RoutePrefix("api/dishIngredients")]
    public class dishIngredientsController : ApiController
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
                try
                {

                using (incposdbEntities entity = new incposdbEntities())
                {
                        var List1 = entity.dishIngredients.ToList();
                        var List = List1.Select(S => new dishIngredients
                       
                    {

                        dishIngredId = S.dishIngredId,
                        name = S.name,
                        itemUnitId = S.itemUnitId,
                        notes = S.notes,
                        isActive = S.isActive,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                        isBasic=S.isBasic,

                    }).ToList();

                  
                    return TokenManager.GenerateToken(List);

                }

                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        [HttpPost]
        [Route("GetByItemUnitId")]
        public string GetByItemUnitId(string token)
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
                long itemUnitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var List1 = entity.dishIngredients.ToList();
                    var List = List1.Where(S => S.itemUnitId == itemUnitId).Select(S => new dishIngredients
                    {

                        dishIngredId = S.dishIngredId,
                        name = S.name,
                        itemUnitId = S.itemUnitId,
                        notes = S.notes,
                        isActive = S.isActive,
                        createDate = S.createDate,
                        updateDate = S.updateDate,
                        createUserId = S.createUserId,
                        updateUserId = S.updateUserId,
                        isBasic=S.isBasic,


                    }).ToList();


                    return TokenManager.GenerateToken(List);

                }
            }
        }
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
                long itemUnitId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemUnitId")
                    {
                        itemUnitId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<menuSettings>();
                    searchPredicate.And(x => x.isActive == 1);
                    if (itemUnitId != 0)
                        searchPredicate.And(x => x.itemUnitId == itemUnitId);

                    var menuList = entity.menuSettings.Where(searchPredicate).ToList();

                    return TokenManager.GenerateToken(menuList);
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
                long dishIngredId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        dishIngredId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var bank = entity.dishIngredients
                   .Where(S => S.dishIngredId == dishIngredId)
                   .Select(S => new
                   {
                       S.dishIngredId,
                       S.name,
                       S.itemUnitId,
                       S.notes,
                       S.isActive,
                       S.createDate,
                       S.updateDate,
                       S.createUserId,
                       S.updateUserId,
                       S.isBasic,

                   })
                   .FirstOrDefault();
                    return TokenManager.GenerateToken(bank);

                }
            }
        }

        // add or update menu settings 
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
                dishIngredients newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        itemObject = c.Value.Replace("\\", string.Empty);
                        itemObject = itemObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<dishIngredients>(itemObject, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
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
                if (newObject.itemUnitId == 0 || newObject.itemUnitId == null)
                {
                    Nullable<long> id = null;
                    newObject.itemUnitId = id;
                }
                try
                {

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        dishIngredients tmpObject = new dishIngredients();
                        if (newObject.dishIngredId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            entity.dishIngredients.Add(newObject);
                        }
                        else
                        {
                            tmpObject = entity.dishIngredients.Find(newObject.dishIngredId);
                            tmpObject.name = newObject.name;
                            tmpObject.itemUnitId = newObject.itemUnitId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.isActive = newObject.isActive;
                            tmpObject.updateUserId = newObject.createUserId;
                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.isBasic = newObject.isBasic;
                        }
                        message = entity.SaveChanges().ToString();
                    }
                    return TokenManager.GenerateToken(message);
                }
                catch { return TokenManager.GenerateToken("0"); }
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
                long dishIngredId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        dishIngredId = long.Parse(c.Value);
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

                            dishIngredients objDelete = entity.dishIngredients.Find(dishIngredId);
                            entity.dishIngredients.Remove(objDelete);
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

                            dishIngredients objDelete = entity.dishIngredients.Find(dishIngredId);
                            objDelete.isActive = 0;
                            objDelete.updateUserId = userId;
                            objDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
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
