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
    [RoutePrefix("api/Points")]
    public class PointsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        // GET api/<controller>
        [HttpPost]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            bool canDelete = false;

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
                    var List = (from S in  entity.Points                                         
                                         select new PointsModel()
                                         {
                                            pointId=S.pointId,
                                       
                                            notes=S.notes,
                                            isActive=S.isActive,
                                            createDate = S.createDate,
                                            updateDate = S.updateDate,
                                            createUserId = S.createUserId,
                                            updateUserId=S.updateUserId,
                                           
                                            Cash=S.Cash,
                                            CashPoints=S.CashPoints,
                                            invoiceCount=S.invoiceCount,
                                            invoiceCountPoints=S.invoiceCountPoints,
                                            CashArchive=S.CashArchive,
                                            CashPointsArchive=S.CashPointsArchive,
                                            invoiceCountArchive=S.invoiceCountArchive,
                                            invoiceCountPoinstArchive=S.invoiceCountPoinstArchive,
                                            agentId=S.agentId,



                                         }).ToList();
                    /*
       public int pointId { get; set; }
        public Nullable<decimal> Cash { get; set; }
        public Nullable<int> CashPoints { get; set; }
        public Nullable<int> invoiceCount { get; set; }
        public Nullable<int> invoiceCountPoints { get; set; }
        public Nullable<decimal> CashArchive { get; set; }
        public Nullable<int> CashPointsArchive { get; set; }
        public Nullable<int> invoiceCountArchive { get; set; }
        public Nullable<int> invoiceCountPoinstArchive { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<byte> isActive { get; set; }
        public Nullable<long> agentId { get; set; }

pointId
Cash
CashPoints
invoiceCount
invoiceCountPoints
CashArchive
CashPointsArchive
invoiceCountArchive
invoiceCountPoinstArchive
agentId
notes
createDate
updateDate
createUserId
updateUserId
isActive


                    */

                    if (List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            if (List[i].isActive == 1)
                            {
                                long pointId = (long)List[i].pointId;
                                var itemsI= entity.agents.Where(x => x.pointId == pointId).Select(b => new { b.agentId }).FirstOrDefault();
                               
                                if ((itemsI is null)  )
                                    canDelete = true;
                            }
                            List[i].canDelete = canDelete;
                        }
                    }

                    if (List == null)
                        return NotFound();
                    else
                        return Ok(List);
                }
            }
            //else
            return NotFound();
        }

        // GET api/<controller>
        [HttpPost]
        [Route("GetByID")]
        public IHttpActionResult GetByID(int pointId)
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
                    var row = entity.Points
                   .Where(u => u.pointId == pointId)
                   .Select(S => new
                   {
                           S.pointId,
                           S.Cash,
                           S.CashPoints,
                           S.invoiceCount,
                           S.invoiceCountPoints,
                           S.CashArchive,
                           S.CashPointsArchive,
                           S.invoiceCountArchive,
                           S.invoiceCountPoinstArchive,
                           S.agentId,
                           S.notes,
                           S.createDate,
                           S.updateDate,
                           S.createUserId,
                           S.updateUserId,
                           S.isActive,
                          

                   })
                   .FirstOrDefault();

                    if (row == null)
                        return NotFound();
                    else
                        return Ok(row);
                }
            }
            else
                return NotFound();
        }

        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string Object)
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
                Object = Object.Replace("\\", string.Empty);
                Object = Object.Trim('"');
                Points newObject = JsonConvert.DeserializeObject<Points>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
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
                if (newObject.agentId == 0 || newObject.agentId == null)
                {
                    Nullable<long> id = null;
                    newObject.agentId = id;
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<Points>();
                        if (newObject.pointId == 0)
                        {
                            newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                         
                      
                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.pointId.ToString();
                        }
                        else
                        {
                            var tmpObject = entity.Points.Where(p => p.pointId == newObject.pointId).FirstOrDefault();

                            tmpObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpObject.updateUserId = newObject.updateUserId;

                        
                            tmpObject.notes  =newObject.notes;
                            tmpObject.isActive=newObject.isActive;

                            tmpObject.pointId=newObject.pointId;
                           tmpObject.Cash=newObject.Cash;
                           tmpObject.CashPoints=newObject.CashPoints;
                           tmpObject.invoiceCount=newObject.invoiceCount;
                           tmpObject.invoiceCountPoints=newObject.invoiceCountPoints;
                           tmpObject.CashArchive=newObject.CashArchive;
                           tmpObject.CashPointsArchive=newObject.CashPointsArchive;
                           tmpObject.invoiceCountArchive=newObject.invoiceCountArchive;
                           tmpObject.invoiceCountPoinstArchive=newObject.invoiceCountPoinstArchive;
                           tmpObject.agentId=newObject.agentId;
                          

                            entity.SaveChanges();

                            message = tmpObject.pointId.ToString();
                        }
                      //  entity.SaveChanges();
                    }
                }
                catch
                {
                    message = "-1";
                }
            }
            return message;
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(long pointId, long userId,bool final)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            int message = 0;
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
                            Points objectDelete = entity.Points.Find(pointId);

                            entity.Points.Remove(objectDelete);
                        message=    entity.SaveChanges();
                          
                            return message.ToString();
                        }
                    }
                    catch
                    {
                        return "-1";
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            Points objectDelete = entity.Points.Find(pointId);

                            objectDelete.isActive = 0;
                            objectDelete.updateUserId = userId;
                            objectDelete.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);

                            message = entity.SaveChanges();

                            return message.ToString(); ;
                        }
                    }
                    catch
                    {
                        return "-2";
                    }
                }
            }
            else
                return "-3";
        }



    }
}