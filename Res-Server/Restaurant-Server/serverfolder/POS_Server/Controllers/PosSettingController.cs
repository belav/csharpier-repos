using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;
using Newtonsoft.Json.Converters;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/PosSetting")]
    public class PosSettingController : ApiController
    {
        CountriesController coctrlr = new CountriesController();
        // GET api/<controller>
        [HttpPost]
        [Route("GetAll")]
        public string GetAll(string token)
        {

            // public ResponseVM GetPurinv(string token)

            //long mainBranchId, long userId    DateTime? date=new DateTime?();



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                //long mainBranchId = 0;
                //long userId = 0;

                //IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                //foreach (Claim c in claims)
                //{
                //    if (c.Type == "mainBranchId")
                //    {
                //        mainBranchId = long.Parse(c.Value);
                //    }
                //    else if (c.Type == "userId")
                //    {
                //        userId = long.Parse(c.Value);
                //    }

                //}

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {


                    using (incposdbEntities entity = new incposdbEntities())
                    {


                        var list = (from S in entity.posSetting
                                    join psal in entity.printers on S.saleInvPrinterId equals psal.printerId into jsale
                                    join prep in entity.printers on S.reportPrinterId equals prep.printerId into jrep
                                    join paper in entity.paperSize on S.saleInvPapersizeId equals paper.sizeId into jpaper
                                    join dpaper in entity.paperSize on S.docPapersizeId equals dpaper.sizeId into jdcpaper

                                    join pkit in entity.printers on S.kitchenPrinterId equals pkit.printerId into jkit
                                    join paperkit in entity.paperSize on S.kitchenPapersizeId equals paperkit.sizeId into jpaperkit

                                    from jjsale in jsale.DefaultIfEmpty()
                                    from jjrep in jrep.DefaultIfEmpty()
                                    from jjpaper in jpaper.DefaultIfEmpty()
                                    from jdocpaper in jdcpaper.DefaultIfEmpty()
                                    from jjkit in jkit.DefaultIfEmpty()
                                    from jjpaperkit in jpaperkit.DefaultIfEmpty()

                                    select new PosSettingModel()
                                    {
                                        posSettingId = S.posSettingId,

                                        posId = S.posId,
                                        saleInvPrinterId = S.saleInvPrinterId,
                                        reportPrinterId = S.reportPrinterId,
                                        saleInvPapersizeId = S.saleInvPapersizeId,
                                        posSerial = S.posSerial,

                                        repprinterId = jjrep.printerId,//printer
                                        repname = jjrep.name,//printer
                                        repprintFor = jjrep.printFor,//printer
                                        salprinterId = jjsale.printerId,//printer
                                        salname = jjsale.name,//printer
                                        salprintFor = jjsale.printFor,//printer
                                        sizeId = jjpaper.sizeId,// paper
                                        paperSize1 = jjpaper.paperSize1,// paper saleInvPapersize
                                        saleSizeValue = jjpaper.sizeValue,// paper sale
                                        docSizeValue = jdocpaper.sizeValue,// paper doc
                                        docPapersize = jdocpaper.paperSize1,// paper
                                        docPapersizeId = S.docPapersizeId,// paper

                                        kitchenPrinterId = S.kitchenPrinterId,//kitchen
                                        kitchenPapersizeId = S.kitchenPapersizeId,
                                        kitchenPrinter = jjkit.name,
                                        kitchenPapersize = jjpaperkit.paperSize1,
                                        kitchenSizeValue = jjpaperkit.sizeValue,

                                        kitchenprintFor = jjkit.printFor,
                                  


                                    }).ToList();


                        return TokenManager.GenerateToken(list);

                    }

                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }

            }



        }


        //[HttpPost]
        //[Route("GetByID")]
        //public IHttpActionResult GetByID(int posSettingId)
        //{
        //   
        //    
        //    string token = "";
        //    if (headers.Contains("APIKey"))
        //    {
        //        token = headers.GetValues("APIKey").First();
        //    }
        //    Validation validation = new Validation();
        //    bool valid = validation.CheckApiKey(token);

        //    if (valid)
        //    {
        //        using (incposdbEntities entity = new incposdbEntities())
        //        {
        //            var row = (from S in entity.posSetting
        //                       join psal in entity.printers on S.saleInvPrinterId equals psal.printerId into jsale
        //                       join prep in entity.printers on S.reportPrinterId equals prep.printerId into jrep
        //                       join paper in entity.paperSize on S.saleInvPapersizeId equals paper.sizeId into jpaper
        //                       join dpaper in entity.paperSize on S.docPapersizeId equals dpaper.sizeId into jdcpaper
        //                       from jdocpaper in jdcpaper.DefaultIfEmpty()

        //                       from jjsale in jsale.DefaultIfEmpty()
        //                       from jjrep in jrep.DefaultIfEmpty()
        //                       from jjpaper in jpaper.DefaultIfEmpty()
        //                       where S.posSettingId == posSettingId
        //                       select new PosSettingModel()
        //                       {
        //                           posSettingId = S.posSettingId,

        //                           posId = S.posId,
        //                           saleInvPrinterId = S.saleInvPrinterId,
        //                           reportPrinterId = S.reportPrinterId,
        //                           saleInvPapersizeId = S.saleInvPapersizeId,
        //                           posSerial = S.posSerial,
        //                           repprinterId = jjrep.printerId,
        //                           repname = jjrep.name,
        //                           repprintFor = jjrep.printFor,
        //                           salprinterId = jjsale.printerId,
        //                           salname = jjsale.name,
        //                           salprintFor = jjsale.printFor,
        //                           sizeId = jjpaper.sizeId,
        //                           paperSize1 = jjpaper.paperSize1,

        //                           docPapersize = jdocpaper.paperSize1,
        //                           docPapersizeId = S.docPapersizeId,
        //                           saleSizeValue = jjpaper.sizeValue,// paper sale
        //                           docSizeValue = jdocpaper.sizeValue,// paper doc
        //                       }).FirstOrDefault();

        //            if (row == null)
        //                return NotFound();
        //            else
        //                return Ok(row);
        //        }
        //    }
        //    else
        //        return NotFound();
        //}


        // get by posId
        private PosSettingModel GetByposId(long posId)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {

                PosSettingModel item = (from S in entity.posSetting
                                        join psal in entity.printers on S.saleInvPrinterId equals psal.printerId into jsale
                                        join prep in entity.printers on S.reportPrinterId equals prep.printerId into jrep
                                        join paper in entity.paperSize on S.saleInvPapersizeId equals paper.sizeId into jpaper
                                        join dpaper in entity.paperSize on S.docPapersizeId equals dpaper.sizeId into jdcpaper
                                        join pkit in entity.printers on S.kitchenPrinterId equals pkit.printerId into jkit
                                        join paperkit in entity.paperSize on S.kitchenPapersizeId equals paperkit.sizeId into jpaperkit

                                        from jdocpaper in jdcpaper.DefaultIfEmpty()
                                        from jjsale in jsale.DefaultIfEmpty()
                                        from jjrep in jrep.DefaultIfEmpty()
                                        from jjpaper in jpaper.DefaultIfEmpty()
                                        from jjkit in jkit.DefaultIfEmpty()
                                        from jjpaperkit in jpaperkit.DefaultIfEmpty()
                                        where S.posId == posId
                                        select new PosSettingModel()
                                        {
                                            posSettingId = S.posSettingId,

                                            posId = S.posId,
                                            saleInvPrinterId = S.saleInvPrinterId,
                                            reportPrinterId = S.reportPrinterId,
                                            saleInvPapersizeId = S.saleInvPapersizeId,
                                            posSerial = S.posSerial,
                                            repprinterId = S.reportPrinterId,
                                            repname = jjrep.name,
                                            repprintFor = jjrep.printFor,
                                            salprinterId = S.saleInvPrinterId,
                                            salname = jjsale.name,
                                            salprintFor = jjsale.printFor,
                                            sizeId = S.saleInvPapersizeId,
                                            paperSize1 = jjpaper.paperSize1,
                                            docPapersize = jdocpaper.paperSize1,
                                            docPapersizeId = S.docPapersizeId,
                                            saleSizeValue = jjpaper.sizeValue,// paper sale
                                            docSizeValue = jdocpaper.sizeValue,// paper doc

                                            kitchenPrinterId = S.kitchenPrinterId,//kitchen
                                            kitchenPapersizeId = S.kitchenPapersizeId,
                                            kitchenPrinter = jjkit.name,
                                            kitchenPapersize = jjpaperkit.paperSize1,
                                            kitchenprintFor = jjkit.printFor,
                                            kitchenSizeValue = jjpaperkit.sizeValue,

                                        }).FirstOrDefault();


                return item;
            }
        }

        [HttpPost]
        [Route("GetByposId")]
        public string GetByposId(string token)
        {
            // public ResponseVM GetItemByID(string token)long posId
            // {



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int message = 0;
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }

                PosSettingModel item = GetByposId(posId);
                //if (item == null)
                //{
                //    item = new PosSettingModel();
                //    posSetting newObject = new posSetting();
                //    newObject.posId = posId;
                //    message = Save(newObject);
                //    if (message > 0)
                //    {
                //        item = GetByposId(posId);
                //    }


                //}



                return TokenManager.GenerateToken(item);
            }



            //var re = Request;
            //
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var row = (from S in entity.posSetting
            //                   join psal in entity.printers on S.saleInvPrinterId equals psal.printerId into jsale
            //                   join prep in entity.printers on S.reportPrinterId equals prep.printerId into jrep
            //                   join paper in entity.paperSize on S.saleInvPapersizeId equals paper.sizeId into jpaper
            //                   join dpaper in entity.paperSize on S.docPapersizeId equals dpaper.sizeId into jdcpaper
            //                   from jdocpaper in jdcpaper.DefaultIfEmpty()
            //                   from jjsale in jsale.DefaultIfEmpty()
            //                   from jjrep in jrep.DefaultIfEmpty()
            //                   from jjpaper in jpaper.DefaultIfEmpty()
            //                   where S.posId == posId
            //                   select new PosSettingModel()
            //                   {
            //                       posSettingId = S.posSettingId,

            //                       posId = S.posId,
            //                       saleInvPrinterId = S.saleInvPrinterId,
            //                       reportPrinterId = S.reportPrinterId,
            //                       saleInvPapersizeId = S.saleInvPapersizeId,
            //                       posSerial = S.posSerial,
            //                       repprinterId = jjrep.printerId,
            //                       repname = jjrep.name,
            //                       repprintFor = jjrep.printFor,
            //                       salprinterId = jjsale.printerId,
            //                       salname = jjsale.name,
            //                       salprintFor = jjsale.printFor,
            //                       sizeId = jjpaper.sizeId,
            //                       paperSize1 = jjpaper.paperSize1,
            //                       docPapersize = jdocpaper.paperSize1,
            //                       docPapersizeId=S.docPapersizeId,
            //                       saleSizeValue = jjpaper.sizeValue,// paper sale
            //                       docSizeValue = jdocpaper.sizeValue,// paper doc
            //                   }).FirstOrDefault();

            //        if (row == null)
            //            return NotFound();
            //        else
            //            return Ok(row);
            //    }
            //}
            //else
            //    return NotFound();
        }
        private long Save(posSetting newObject)
        {
            long message = 0;
            if (newObject != null)
            {


                posSetting tmpObject;
                if (newObject.posId == 0 || newObject.posId == null)
                {
                    Nullable<long> id = null;
                    newObject.posId = id;
                }
                if (newObject.reportPrinterId == 0 || newObject.reportPrinterId == null)
                {
                    Nullable<long> id = null;
                    newObject.reportPrinterId = id;
                }
                if (newObject.saleInvPapersizeId == 0 || newObject.saleInvPapersizeId == null)
                {
                    Nullable<long> id = null;
                    newObject.saleInvPapersizeId = id;
                }
                if (newObject.saleInvPrinterId == 0 || newObject.saleInvPrinterId == null)
                {
                    Nullable<long> id = null;
                    newObject.saleInvPrinterId = id;
                }
                if (newObject.kitchenPrinterId == 0 || newObject.kitchenPrinterId == null)
                {
                    Nullable<long> id = null;
                    newObject.kitchenPrinterId = id;
                }
                if (newObject.kitchenPapersizeId == 0 || newObject.kitchenPapersizeId == null)
                {
                    Nullable<long> id = null;
                    newObject.kitchenPapersizeId = id;
                }
                /*
                 *        public Nullable<int> kitchenPrinterId { get; set; }
        public Nullable<int> kitchenPapersizeId { get; set; }
                 * */

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var locationEntity = entity.Set<posSetting>();
                        if (newObject.posSettingId == 0)
                        {

                            locationEntity.Add(newObject);
                            entity.SaveChanges();
                            message = newObject.posSettingId;
                        }
                        else
                        {
                            tmpObject = entity.posSetting.Where(p => p.posSettingId == newObject.posSettingId).FirstOrDefault();


                            tmpObject.posSettingId = newObject.posSettingId;

                            tmpObject.posId = newObject.posId;
                            tmpObject.saleInvPrinterId = newObject.saleInvPrinterId;
                            tmpObject.reportPrinterId = newObject.reportPrinterId;
                            tmpObject.saleInvPapersizeId = newObject.saleInvPapersizeId;
                            tmpObject.docPapersizeId = newObject.docPapersizeId;
                            tmpObject.kitchenPrinterId = newObject.kitchenPrinterId;
                            tmpObject.kitchenPapersizeId = newObject.kitchenPapersizeId;
                            //  tmpObject.posSerial = newObject.posSerial;      public Nullable<int> docPapersizeId { get; set; }




                            entity.SaveChanges();

                            message = tmpObject.posSettingId;
                        }
                        //  entity.SaveChanges();
                    }
                    return (message);

                }
                catch
                {
                    message = 0;
                    return (message);
                }


            }
            else
            {
                return (-1);
            }


        }
        // add or update location
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            //string Object
            string message = "";



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                posSetting newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<posSetting>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                        break;
                    }
                }
                if (newObject != null)
                {


                    posSetting tmpObject;
                    if (newObject.posId == 0 || newObject.posId == null)
                    {
                        Nullable<long> id = null;
                        newObject.posId = id;
                    }
                    if (newObject.reportPrinterId == 0 || newObject.reportPrinterId == null)
                    {
                        Nullable<long> id = null;
                        newObject.reportPrinterId = id;
                    }
                    if (newObject.saleInvPapersizeId == 0 || newObject.saleInvPapersizeId == null)
                    {
                        Nullable<long> id = null;
                        newObject.saleInvPapersizeId = id;
                    }
                    if (newObject.saleInvPrinterId == 0 || newObject.saleInvPrinterId == null)
                    {
                        Nullable<long> id = null;
                        newObject.saleInvPrinterId = id;
                    }
                    if (newObject.kitchenPrinterId == 0 || newObject.kitchenPrinterId == null)
                    {
                        Nullable<long> id = null;
                        newObject.kitchenPrinterId = id;
                    }
                    if (newObject.kitchenPapersizeId == 0 || newObject.kitchenPapersizeId == null)
                    {
                        Nullable<long> id = null;
                        newObject.kitchenPapersizeId = id;
                    }

                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var locationEntity = entity.Set<posSetting>();
                            if (newObject.posSettingId == 0)
                            {



                                locationEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.posSettingId.ToString();
                            }
                            else
                            {
                                tmpObject = entity.posSetting.Where(p => p.posSettingId == newObject.posSettingId).FirstOrDefault();


                                tmpObject.posSettingId = newObject.posSettingId;

                                tmpObject.posId = newObject.posId;
                                tmpObject.saleInvPrinterId = newObject.saleInvPrinterId;
                                tmpObject.reportPrinterId = newObject.reportPrinterId;
                                tmpObject.saleInvPapersizeId = newObject.saleInvPapersizeId;
                                tmpObject.docPapersizeId = newObject.docPapersizeId;
                                tmpObject.kitchenPrinterId = newObject.kitchenPrinterId;
                                tmpObject.kitchenPapersizeId = newObject.kitchenPapersizeId;
                                //  tmpObject.posSerial = newObject.posSerial;      public Nullable<int> docPapersizeId { get; set; }

                                entity.SaveChanges();

                                message = tmpObject.posSettingId.ToString();
                            }
                            //  entity.SaveChanges();
                        }
                        return TokenManager.GenerateToken(message);

                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }


                }

                return TokenManager.GenerateToken(message);

            }

            //var re = Request;
            //
            //string token = "";
            //string message = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    Object = Object.Replace("\\", string.Empty);
            //    Object = Object.Trim('"');
            //    posSetting newObject = JsonConvert.DeserializeObject<posSetting>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (newObject.posId == 0 || newObject.posId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.posId = id;
            //    }
            //    if (newObject.reportPrinterId == 0 || newObject.reportPrinterId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.reportPrinterId = id;
            //    }
            //    if (newObject.saleInvPapersizeId == 0 || newObject.saleInvPapersizeId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.saleInvPapersizeId = id;
            //    }
            //    if (newObject.saleInvPrinterId == 0 || newObject.saleInvPrinterId == null)
            //    {
            //        Nullable<long> id = null;
            //        newObject.saleInvPrinterId = id;
            //    }

            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var locationEntity = entity.Set<posSetting>();
            //            if (newObject.posSettingId == 0)
            //            {



            //                locationEntity.Add(newObject);
            //                entity.SaveChanges();
            //                message = newObject.posSettingId.ToString();
            //            }
            //            else
            //            {
            //                var tmpObject = entity.posSetting.Where(p => p.posSettingId == newObject.posSettingId).FirstOrDefault();


            //                tmpObject.posSettingId = newObject.posSettingId;

            //                tmpObject.posId = newObject.posId;
            //                tmpObject.saleInvPrinterId = newObject.saleInvPrinterId;
            //                tmpObject.reportPrinterId = newObject.reportPrinterId;
            //                tmpObject.saleInvPapersizeId = newObject.saleInvPapersizeId;
            //                tmpObject.docPapersizeId = newObject.docPapersizeId;
            //                //  tmpObject.posSerial = newObject.posSerial;      public Nullable<int> docPapersizeId { get; set; }




            //                entity.SaveChanges();

            //                message = tmpObject.posSettingId.ToString();
            //            }
            //            //  entity.SaveChanges();
            //        }
            //    }
            //    catch
            //    {
            //        message = "-1";
            //    }
            //}
            //return message;
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            //int posSettingId  Save()
            string message = "";



            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long posSettingId = 0;


                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "posSettingId")
                    {
                        posSettingId = long.Parse(c.Value);
                    }

                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {

                        posSetting objectDelete = entity.posSetting.Find(posSettingId);

                        entity.posSetting.Remove(objectDelete);
                        message = entity.SaveChanges().ToString();


                    }
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }


            }

            //var re = Request;
            //
            //string token = "";
            //int message = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}

            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //if (valid)
            //{

            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            posSetting objectDelete = entity.posSetting.Find(posSettingId);

            //            entity.posSetting.Remove(objectDelete);
            //            message = entity.SaveChanges();

            //            return message.ToString();
            //        }
            //    }
            //    catch
            //    {
            //        return "-1";
            //    }


            //}
            //else
            //    return "-3";
        }

        [HttpPost]
        [Route("SavePrSet")]
        public string SavePrSet(string token)
        {
            //string Object
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                posSetting posscls = null;
                printers repprinterRow = null;
                printers salprinterRow = null;
                printers kitchenprinterRow = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "repprinterRow")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        repprinterRow = JsonConvert.DeserializeObject<printers>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "salprinterRow")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        salprinterRow = JsonConvert.DeserializeObject<printers>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "posscls")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        posscls = JsonConvert.DeserializeObject<posSetting>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    else if (c.Type == "kitchenprinterRow")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        kitchenprinterRow = JsonConvert.DeserializeObject<printers>(Object, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                    }
                    
                }
                if (repprinterRow != null && salprinterRow != null && posscls != null)
                {
                    try
                    {
                        PrinterController prcntrlr = new PrinterController();

                        long reportPrinterId = prcntrlr.Save(repprinterRow);
                        long saleInvPrinterId = prcntrlr.Save(salprinterRow);
                        long kitchenPrinterId = prcntrlr.Save(kitchenprinterRow);
                        
                        posscls.reportPrinterId = reportPrinterId;
                        posscls.saleInvPrinterId = saleInvPrinterId;
                        posscls.kitchenPrinterId = kitchenPrinterId;
                        message = Save(posscls).ToString();


                        return TokenManager.GenerateToken(message);

                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }


                }

                return TokenManager.GenerateToken(message);

            }


        }



    }
}