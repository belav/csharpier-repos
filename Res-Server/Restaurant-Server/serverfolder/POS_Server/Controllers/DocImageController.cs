using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using Newtonsoft.Json;

using Newtonsoft.Json.Converters;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/DocImage")]
    public class DocImageController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            //public string GetByGroupId(string token)string tableName, long tableId
            //{


            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long tableId = 0;
                string tableName = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }
                    else if (c.Type == "tableName")
                    {
                        tableName = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageList = entity.docImages
                            .Where(x => x.tableName == tableName && x.tableId == tableId)
                            .Select(
                                b =>
                                    new
                                    {
                                        b.id,
                                        b.docName,
                                        b.docnum,
                                        b.image,
                                        b.tableName,
                                        b.tableId,
                                        b.notes,
                                        b.createDate,
                                        b.updateDate,
                                        b.createUserId,
                                        b.updateUserId,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(docImageList);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var docImageList = entity.docImages.Where(x => x.tableName == tableName && x.tableId == tableId)
            //            .Select(b => new
            //            {
            //                b.id,
            //                b.docName,
            //                b.docnum,
            //                b.image,
            //                b.tableName,
            //                b.tableId,
            //                b.note,
            //                b.createDate,
            //                b.updateDate,
            //                b.createUserId,
            //                b.updateUserId,
            //            })
            //        .ToList();

            //        if (docImageList == null)
            //            return NotFound();
            //        else
            //            return Ok(docImageList);
            //    }
            //}
            ////else
            //return NotFound();
        }

        [HttpPost]
        [Route("GetCount")]
        public string GetCount(string token)
        {
            //string tableName, long tableId

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long tableId = 0;
                string tableName = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "tableId")
                    {
                        tableId = long.Parse(c.Value);
                    }
                    else if (c.Type == "tableName")
                    {
                        tableName = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var docImageCount = entity.docImages
                            .Where(x => x.tableName == tableName && x.tableId == tableId)
                            .Select(
                                b =>
                                    new
                                    {
                                        b.id,
                                        //b.docName,
                                        //b.docnum,
                                        //b.image,
                                        //b.tableName,
                                        //b.tableId,
                                        //b.note,
                                        //b.createDate,
                                        //b.updateDate,
                                        //b.createUserId,
                                        //b.updateUserId,
                                    }
                            )
                            .ToList()
                            .Count;

                        return TokenManager.GenerateToken(docImageCount.ToString());
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var docImageCount = entity.docImages.Where(x => x.tableName == tableName && x.tableId == tableId)
            //            .Select(b => new
            //            {
            //                b.id,
            //                b.docName,
            //                b.docnum,
            //                b.image,
            //                b.tableName,
            //                b.tableId,
            //                b.note,
            //                b.createDate,
            //                b.updateDate,
            //                b.createUserId,
            //                b.updateUserId,
            //            })
            //        .ToList().Count;

            //        return Ok(docImageCount);
            //    }
            //}
            ////else
            //return NotFound();
        }

        [Route("PostImage")]
        public IHttpActionResult PostImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    string imageName = postedFile.FileName;
                    string imageWithNoExt = Path.GetFileNameWithoutExtension(postedFile.FileName);

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB

                        IList<string> AllowedFileExtensions = new List<string>
                        {
                            ".jpg",
                            ".gif",
                            ".png",
                            ".bmp",
                            ".jpeg",
                            ".tiff",
                            ".jfif"
                        };
                        var ext = postedFile.FileName.Substring(
                            postedFile.FileName.LastIndexOf('.')
                        );
                        var extension = ext.ToLower();

                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format(
                                "Please Upload image of type .jpg,.gif,.png,.jfif,.bmp,.jpeg,.tiff"
                            );
                            return Ok(message);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");

                            return Ok(message);
                        }
                        else
                        {
                            if (
                                !Directory.Exists(
                                    System.Web.Hosting.HostingEnvironment.MapPath(
                                        "~\\images\\docImage"
                                    )
                                )
                            )
                                Directory.CreateDirectory(
                                    System.Web.Hosting.HostingEnvironment.MapPath(
                                        "~\\images\\docImage"
                                    )
                                );
                            //  check if image exist
                            var pathCheck = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\docImage"
                                ),
                                imageWithNoExt
                            );
                            var files = Directory.GetFiles(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\docImage"
                                ),
                                imageWithNoExt + ".*"
                            );
                            if (files.Length > 0)
                            {
                                File.Delete(files[0]);
                            }

                            //Userimage myfolder name where i want to save my image
                            var filePath = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\docImage"
                                ),
                                imageName
                            );
                            postedFile.SaveAs(filePath);
                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Ok(message1);
                }
                var res = string.Format("Please Upload a image.");

                return Ok(res);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");

                return Ok(res);
            }
        }

        [HttpPost]
        [Route("GetImage")]
        public string GetImage(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string imageName = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "imageName")
                    {
                        imageName = c.Value;
                    }
                }
                if (String.IsNullOrEmpty(imageName))
                    return TokenManager.GenerateToken("0");

                string localFilePath;
                try
                {
                    localFilePath = Path.Combine(
                        System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\docImage"),
                        imageName
                    );

                    byte[] b = System.IO.File.ReadAllBytes(localFilePath);
                    return TokenManager.GenerateToken(Convert.ToBase64String(b));
                }
                catch
                {
                    return TokenManager.GenerateToken(null);
                }
            }
        }

        [HttpPost]
        [Route("saveImageDoc")]
        public string saveImageDoc(string token)
        {
            //public String Save(string token)string docImageObject
            //{

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
                docImages newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<docImages>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
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
                    try
                    {
                        docImages docImage;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var imageDocEntity = entity.Set<docImages>();
                            if (newObject.id == 0)
                            {
                                newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateUserId = newObject.createUserId;

                                docImage = imageDocEntity.Add(newObject);
                            }
                            else
                            {
                                docImage = entity.docImages
                                    .Where(p => p.id == newObject.id)
                                    .FirstOrDefault();
                                docImage.docName = newObject.docName;
                                docImage.docnum = newObject.docnum;
                                docImage.image = newObject.image;
                                docImage.notes = newObject.notes;
                                docImage.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                docImage.updateUserId = newObject.updateUserId;
                            }
                            entity.SaveChanges();

                            return TokenManager.GenerateToken(docImage.id.ToString());
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
                    return TokenManager.GenerateToken(message);
                }
            }
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //docImageObject = docImageObject.Replace("\\", string.Empty);
            //docImageObject = docImageObject.Trim('"');

            //if (valid)
            //{
            //    docImages imageDocObj = JsonConvert.DeserializeObject<docImages>(docImageObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (imageDocObj.updateUserId == 0 || imageDocObj.updateUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        imageDocObj.updateUserId = id;
            //    }
            //    if (imageDocObj.createUserId == 0 || imageDocObj.createUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        imageDocObj.createUserId = id;
            //    }
            //    try
            //    {
            //        docImages docImage;
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var imageDocEntity = entity.Set<docImages>();
            //            if (imageDocObj.id == 0)
            //            {
            //                imageDocObj.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                imageDocObj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                imageDocObj.updateUserId = imageDocObj.createUserId;

            //                docImage = imageDocEntity.Add(imageDocObj);
            //            }
            //            else
            //            {
            //                docImage = entity.docImages.Where(p => p.id == imageDocObj.id).FirstOrDefault();
            //                docImage.docName = imageDocObj.docName;
            //                docImage.docnum = imageDocObj.docnum;
            //                docImage.image = imageDocObj.image;
            //                docImage.note = imageDocObj.note;
            //                docImage.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                docImage.updateUserId = imageDocObj.updateUserId;

            //            }
            //            entity.SaveChanges();
            //            return docImage.id;
            //        }
            //    }
            //    catch
            //    {
            //        return 0;
            //    }
            //}
            //else
            //    return 0;
        }

        [HttpPost]
        [Route("UpdateImage")]
        public string UpdateImage(string token)
        {
            //public String Save(string token)string docImageObject
            //{

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
                docImages newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<docImages>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
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
                    try
                    {
                        docImages docImage;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var docImgEntity = entity.Set<docImages>();
                            docImage = entity.docImages.Where(p => p.id == newObject.id).First();
                            docImage.image = newObject.image;
                            entity.SaveChanges();
                        }

                        return TokenManager.GenerateToken(docImage.id.ToString());
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                else
                {
                    return TokenManager.GenerateToken(message);
                }
            }
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //docImageObject = docImageObject.Replace("\\", string.Empty);
            //docImageObject = docImageObject.Trim('"');

            //if (valid)
            //{
            //    docImages docImageObj = JsonConvert.DeserializeObject<docImages>(docImageObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    if (docImageObj.updateUserId == 0 || docImageObj.updateUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        docImageObj.updateUserId = id;
            //    }
            //    if (docImageObj.createUserId == 0 || docImageObj.createUserId == null)
            //    {
            //        Nullable<long> id = null;
            //        docImageObj.createUserId = id;
            //    }
            //    try
            //    {
            //        docImages docImage;
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var docImgEntity = entity.Set<docImages>();
            //            docImage = entity.docImages.Where(p => p.id == docImageObj.id).First();
            //            docImage.image = docImageObj.image;
            //            entity.SaveChanges();
            //        }
            //        return docImage.id;
            //    }
            //    catch { return 0; }
            //}
            //else
            //    return 0;
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            //public String Save(string token)int docId
            //{

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
                long docId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "docId")
                    {
                        docId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        docImages docImageObj = entity.docImages.Find(docId);

                        entity.docImages.Remove(docImageObj);
                        entity.SaveChanges();

                        // delete image from folder
                        //var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\docImage"), docImageObj.image);
                        string tmpPath = System.IO.Path.Combine(
                            System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\docImage"),
                            docImageObj.image
                        );
                        if (File.Exists(tmpPath))
                        {
                            File.Delete(tmpPath);
                        }

                        // return Ok("Serial is Deleted Successfully");
                        return TokenManager.GenerateToken("1");
                    }
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
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
            //            docImages docImageObj = entity.docImages.Find(docId);

            //            entity.docImages.Remove(docImageObj);
            //            entity.SaveChanges();

            //            // delete image from folder
            //            //var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\docImage"), docImageObj.image);
            //            string tmpPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\docImage"), docImageObj.image);
            //            if (File.Exists(tmpPath))
            //            {
            //                File.Delete(tmpPath);
            //            }

            //            return Ok("Serial is Deleted Successfully");
            //        }
            //    }
            //    catch { return NotFound(); }
            //}
            //else
            //    return NotFound();
        }
    }
}
