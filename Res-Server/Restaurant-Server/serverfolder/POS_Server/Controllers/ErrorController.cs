using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/errorcontroller")]
    public class ErrorController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller>
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
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
                    var List = (
                        from S in entity.error
                        select new ErrorModel()
                        {
                            errorId = S.errorId,
                            num = S.num,
                            msg = S.msg,
                            stackTrace = S.stackTrace,
                            targetSite = S.targetSite,
                            posId = S.posId,
                            branchId = S.branchId,
                            createDate = S.createDate,
                            createUserId = S.createUserId,
                            source = S.source,
                            method = S.method,
                        }
                    ).ToList();
                    return TokenManager.GenerateToken(List);
                }
            }
        }

        // GET api/<controller>
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
                long errorId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        errorId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var row = entity.error
                        .Where(u => u.errorId == errorId)
                        .Select(
                            S =>
                                new
                                {
                                    S.errorId,
                                    S.num,
                                    S.msg,
                                    S.stackTrace,
                                    S.targetSite,
                                    S.posId,
                                    S.branchId,
                                    S.createDate,
                                    S.createUserId,
                                    S.source,
                                    S.method,
                                }
                        )
                        .FirstOrDefault();

                    return TokenManager.GenerateToken(row);
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
                string Object = "";
                error newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<error>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }

                if (newObject.createUserId == 0 || newObject.createUserId == null)
                {
                    Nullable<long> id = null;
                    newObject.createUserId = id;
                }
                if (newObject.posId == 0 || newObject.posId == null)
                {
                    Nullable<long> posId = null;
                    newObject.posId = posId;
                }
                if (newObject.branchId == 0 || newObject.branchId == null)
                {
                    Nullable<long> branchId = null;
                    newObject.branchId = branchId;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<error>();
                        if (newObject.errorId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.errorId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            var tmpObject = entity.error
                                .Where(p => p.errorId == newObject.errorId)
                                .FirstOrDefault();

                            // newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);

                            tmpObject.num = newObject.num;
                            tmpObject.msg = newObject.msg;
                            tmpObject.stackTrace = newObject.stackTrace;
                            tmpObject.targetSite = newObject.targetSite;
                            tmpObject.posId = newObject.posId;
                            tmpObject.branchId = newObject.branchId;

                            tmpObject.createUserId = newObject.createUserId;
                            tmpObject.source = newObject.source;
                            tmpObject.method = newObject.method;

                            entity.SaveChanges();

                            message = tmpObject.errorId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
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
                try
                {
                    long errorId = 0;
                    IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                    foreach (Claim c in claims)
                    {
                        if (c.Type == "itemId")
                        {
                            errorId = long.Parse(c.Value);
                        }
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        error objectDelete = entity.error.Find(errorId);

                        entity.error.Remove(objectDelete);
                        message = entity.SaveChanges().ToString();

                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    message = "-1";
                    return TokenManager.GenerateToken(message);
                }
            }
        }
    }
}
