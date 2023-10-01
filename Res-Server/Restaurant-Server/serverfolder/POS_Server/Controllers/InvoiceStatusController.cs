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
    [RoutePrefix("api/InvoiceStatus")]
    public class InvoiceStatusController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

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
                long invoiceId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var invoiceStatus = entity
                        .invoiceStatus
                        .Where(x => x.invoiceId == invoiceId)
                        .Select(
                            c =>
                                new InvoiceStatusModel()
                                {
                                    invStatusId = c.invStatusId,
                                    invoiceId = c.invoiceId,
                                    status = c.status,
                                    createDate = c.createDate,
                                    updateDate = c.updateDate,
                                    createUserId = c.createUserId,
                                    updateUserId = c.updateUserId,
                                    notes = c.notes,
                                    isActive = c.isActive,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(invoiceStatus);
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
                string statusObject = "";
                invoiceStatus Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        statusObject = c.Value.Replace("\\", string.Empty);
                        statusObject = statusObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<invoiceStatus>(
                            statusObject,
                            new JsonSerializerSettings
                            {
                                DateParseHandling = DateParseHandling.None
                            }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        invoiceStatus tmpStatus = new invoiceStatus();
                        var statusEntity = entity.Set<invoiceStatus>();
                        if (Object.invStatusId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            tmpStatus = statusEntity.Add(Object);
                            entity.SaveChanges();
                            message = tmpStatus.invStatusId.ToString();
                            return TokenManager.GenerateToken(message);
                        }
                        else
                        {
                            tmpStatus = entity
                                .invoiceStatus
                                .Where(p => p.invStatusId == Object.invStatusId)
                                .FirstOrDefault();
                            tmpStatus.notes = Object.notes;
                            tmpStatus.status = Object.status;
                            tmpStatus.createDate = Object.createDate;
                            tmpStatus.updateDate = Object.updateDate;
                            tmpStatus.updateUserId = Object.updateUserId;
                            tmpStatus.isActive = Object.isActive;
                            tmpStatus.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date;
                            entity.SaveChanges();
                            message = tmpStatus.invStatusId.ToString();
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
    }
}
