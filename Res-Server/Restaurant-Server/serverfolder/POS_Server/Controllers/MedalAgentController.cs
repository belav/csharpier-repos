using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/medalAgent")]
    public class MedalAgentController : ApiController
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
                    var medalsList = (
                        from MA in entity.medalAgent
                        join M in entity.medals on MA.medalId equals M.medalId into JM
                        join A in entity.agents on MA.agentId equals A.agentId into JA

                        join U in entity.users on MA.createUserId equals U.userId into JU
                        from JMM in JM.DefaultIfEmpty()
                        from JAA in JA.DefaultIfEmpty()
                        from JUU in JU.DefaultIfEmpty()

                        select new MedalAgentModel()
                        {
                            id = MA.id,
                            medalId = MA.medalId,
                            agentId = MA.agentId,
                            notes = MA.notes,
                            isActive = MA.isActive,
                            createDate = MA.createDate,
                            updateDate = MA.updateDate,
                            createUserId = MA.createUserId,
                            updateUserId = MA.updateUserId,
                            agentName = JAA.name,
                            medalName = JMM.name,
                            createUserName = JUU.username,
                        }
                    ).Select(c => new MedalAgentModel() { }).ToList();

                    /*
                     *
                      id
                     medalId
                     agentId
                      offerId
                     couponId
                     notes
                      isActive
                     createDate
                       updateDate
                     createUserId
                     updateUserId
                     * */
                    // can delet or not


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
        [Route("GetByID")]
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
            if (headers.Contains("id"))
            {
                cId = Convert.ToInt32(headers.GetValues("id").First());
            }
            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);

            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var medal = entity.medalAgent
                        .Where(c => c.medalId == cId)
                        .Select(
                            c =>
                                new
                                {
                                    c.id,
                                    c.medalId,
                                    c.agentId,
                                    c.notes,
                                    c.isActive,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId,
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

        // add or update medal
        [HttpPost]
        [Route("Save")]
        public bool Save(string medalAgentObj)
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
                medalAgentObj = medalAgentObj.Replace("\\", string.Empty);
                medalAgentObj = medalAgentObj.Trim('"');
                medalAgent Object = JsonConvert.DeserializeObject<medalAgent>(
                    medalAgentObj,
                    new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }
                );
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var medalEntity = entity.Set<medalAgent>();
                        if (Object.medalId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            medalEntity.Add(Object);
                            //  message = "medal Is Added Successfully";
                        }
                        else
                        {
                            var tmpmedal = entity.medalAgent
                                .Where(p => p.medalId == Object.medalId)
                                .FirstOrDefault();

                            tmpmedal.id = Object.id;
                            tmpmedal.medalId = Object.medalId;
                            tmpmedal.agentId = Object.agentId;

                            tmpmedal.notes = Object.notes;

                            tmpmedal.createDate = Object.createDate;
                            tmpmedal.updateDate = Object.updateDate;
                            tmpmedal.createUserId = Object.createUserId;
                            tmpmedal.updateUserId = Object.updateUserId;
                            tmpmedal.isActive = Object.isActive;
                            tmpmedal.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date;
                            tmpmedal.updateUserId = Object.updateUserId;

                            //message = "medal Is Updated Successfully";
                        }
                        entity.SaveChanges();
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete(long Id, long userId, Boolean final)
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
                            medalAgent medalObj = entity.medalAgent.Find(Id);

                            entity.medalAgent.Remove(medalObj);
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
                            medalAgent medalObj = entity.medalAgent.Find(Id);

                            medalObj.isActive = 0;
                            medalObj.updateUserId = userId;
                            medalObj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            entity.SaveChanges();

                            return Ok("Offer is Deleted Successfully");
                        }
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
            }
            else
                return NotFound();
        }

        #region
        [HttpPost]
        [Route("UpdateAgentsByMedalId")]
        public int UpdateAgentsByMedalId(string newagentlist)
        {
            long userId = 0;
            long medalId = 0;
            var re = Request;
            var headers = re.Headers;
            int res = 0;
            string token = "";
            if (headers.Contains("APIKey"))
            {
                token = headers.GetValues("APIKey").First();
            }
            if (headers.Contains("medalId"))
            {
                medalId = Convert.ToInt64(headers.GetValues("medalId").First());
            }
            if (headers.Contains("userId"))
            {
                userId = Convert.ToInt64(headers.GetValues("userId").First());
            }
            Validation validation = new Validation();
            bool valid = validation.CheckApiKey(token);
            newagentlist = newagentlist.Replace("\\", string.Empty);
            newagentlist = newagentlist.Trim('"');
            List<medalAgent> newmagObj = JsonConvert.DeserializeObject<List<medalAgent>>(
                newagentlist,
                new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }
            );
            if (valid)
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var oldlist = entity.medalAgent.Where(p => p.medalId == medalId);
                    if (oldlist.Count() > 0)
                    {
                        entity.medalAgent.RemoveRange(oldlist);
                    }
                    if (newmagObj.Count() > 0)
                    {
                        foreach (medalAgent newrow in newmagObj)
                        {
                            newrow.medalId = medalId;

                            if (newrow.createUserId == null || newrow.createUserId == 0)
                            {
                                newrow.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newrow.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);

                                newrow.createUserId = userId;
                                newrow.updateUserId = userId;
                            }
                            else
                            {
                                newrow.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newrow.updateUserId = userId;
                            }
                        }
                        entity.medalAgent.AddRange(newmagObj);
                    }
                    res = entity.SaveChanges();

                    return res;
                }
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }
}
