using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using POS_Server.Models;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/medal")]
    public class MedalController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all medals
        [HttpPost]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            Boolean canDelete = false;
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }
            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid) // APIKey is valid
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var medalsList = entity.medals
                        .Select(
                            c =>
                                new MedalModel()
                                {
                                    medalId = c.medalId,
                                    name = c.name,
                                    isActive = c.isActive,
                                    notes = c.notes,
                                    createUserId = c.createUserId,
                                    updateUserId = c.updateUserId,
                                    createDate = c.createDate,
                                    updateDate = c.updateDate,
                                    symbol = c.symbol,
                                    CashPointsRequired = c.CashPointsRequired,
                                    invoiceCountPointsRequired = c.invoiceCountPointsRequired,
                                }
                        )
                        .ToList();

                    /*
                     *
                      medalId
                      name
                      isActive
                      notes
                      createUserId
                      updateUserId
                      createDate
                      updateDate
                              public string symbol { get; set; }
        public Nullable<int> CashPointsRequired { get; set; }
        public Nullable<int> invoiceCountPointsRequired { get; set; }
                     * */
                    // can delet or not
                    if (medalsList.Count > 0)
                    {
                        foreach (MedalModel medalitem in medalsList)
                        {
                            canDelete = false;
                            if (medalitem.isActive == 1)
                            {
                                long cId = (long)medalitem.medalId;
                                var casht = entity.medalAgent
                                    .Where(x => x.medalId == cId)
                                    .Select(x => new { x.medalId })
                                    .FirstOrDefault();

                                if ((casht is null))
                                    canDelete = true;
                            }
                            medalitem.canDelete = canDelete;
                        }
                    }

                    if (medalsList == null)
                        return NotFound();
                    else
                        return Ok(medalsList);
                }
            }
            //else
            return NotFound();
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetmedalByID")]
        public IHttpActionResult GetmedalByID()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            long cId = 0;
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }
            if (headers.Contains("medalId"))
            {
                cId = Convert.ToInt32(headers.GetValues("medalId").First());
            }
            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var medal = entity.medals
                        .Where(c => c.medalId == cId)
                        .Select(
                            c =>
                                new
                                {
                                    c.medalId,
                                    c.name,
                                    c.isActive,
                                    c.notes,
                                    c.createUserId,
                                    c.updateUserId,
                                    c.createDate,
                                    c.updateDate,
                                    c.symbol,
                                    c.CashPointsRequired,
                                    c.invoiceCountPointsRequired,
                                }
                        )
                        .FirstOrDefault();

                    if (medal == null)
                        return NotFound();
                    else
                        return Ok(medal);
                }
            }
            else
                return NotFound();
        }

        // GET api/<controller>  Get medal By is active
        [HttpPost]
        [Route("GetByisActive")]
        public IHttpActionResult GetByisActive(byte isActive)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var medal = entity.medals
                        .Where(c => c.isActive == isActive)
                        .Select(
                            c =>
                                new
                                {
                                    c.medalId,
                                    c.name,
                                    c.isActive,
                                    c.notes,
                                    c.createUserId,
                                    c.updateUserId,
                                    c.createDate,
                                    c.updateDate,
                                    c.symbol,
                                    c.CashPointsRequired,
                                    c.invoiceCountPointsRequired,
                                }
                        )
                        .ToList();

                    if (medal == null)
                        return NotFound();
                    else
                        return Ok(medal);
                }
            }
            else
                return NotFound();
        }

        // add or update medal
        [HttpPost]
        [Route("Save")]
        public string Save(string medalObject)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            string message = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }
            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                medalObject = medalObject.Replace("\\", string.Empty);
                medalObject = medalObject.Trim('"');
                medals Object = JsonConvert.DeserializeObject<medals>(
                    medalObject,
                    new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }
                );
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var medalEntity = entity.Set<medals>();
                        if (Object.medalId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            medalEntity.Add(Object);
                            entity.SaveChanges();
                            message = Object.medalId.ToString();
                        }
                        else
                        {
                            var tmpmedal = entity.medals
                                .Where(p => p.medalId == Object.medalId)
                                .FirstOrDefault();

                            tmpmedal.medalId = Object.medalId;

                            tmpmedal.name = Object.name;

                            tmpmedal.notes = Object.notes;

                            tmpmedal.createDate = Object.createDate;
                            tmpmedal.updateDate = Object.updateDate;
                            tmpmedal.createUserId = Object.createUserId;
                            tmpmedal.updateUserId = Object.updateUserId;
                            tmpmedal.isActive = Object.isActive;
                            tmpmedal.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date;
                            tmpmedal.updateUserId = Object.updateUserId;
                            tmpmedal.symbol = Object.symbol;
                            tmpmedal.CashPointsRequired = Object.CashPointsRequired;
                            tmpmedal.invoiceCountPointsRequired = Object.invoiceCountPointsRequired;

                            entity.SaveChanges();

                            message = tmpmedal.medalId.ToString();
                        }
                    }
                    return message;
                }
                catch
                {
                    return "-1";
                }
            }
            else
                return "-1";
        }

        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete(long medalId, long userId, bool final)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }

            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);
            if (valid)
            {
                if (final)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            medals medalObj = entity.medals.Find(medalId);

                            entity.medals.Remove(medalObj);
                            entity.SaveChanges();

                            return Ok("medal is Deleted Successfully");
                        }
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            medals medalObj = entity.medals.Find(medalId);

                            medalObj.isActive = 0;
                            medalObj.updateUserId = userId;
                            medalObj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            entity.SaveChanges();

                            return Ok("Deleted Successfully");
                        }
                    }
                    catch
                    {
                        return Ok("Not Deleted");
                    }
                }
            }
            else
                return Ok("isActive Not changed");
        }
    }
}
