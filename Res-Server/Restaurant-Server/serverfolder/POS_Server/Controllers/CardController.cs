using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/cards")]
    public class CardController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all cards
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
                    var cardsList = entity.cards
                        .Select(
                            c =>
                                new CardModel()
                                {
                                    cardId = c.cardId,
                                    name = c.name,
                                    notes = c.notes,
                                    createDate = c.createDate,
                                    updateDate = c.updateDate,
                                    createUserId = c.createUserId,
                                    updateUserId = c.updateUserId,
                                    isActive = c.isActive,
                                    hasProcessNum = c.hasProcessNum,
                                    image = c.image,
                                    commissionValue = c.commissionValue,
                                    commissionRatio = c.commissionRatio,
                                }
                        )
                        .ToList();

                    // can delet or not
                    if (cardsList.Count > 0)
                    {
                        foreach (CardModel carditem in cardsList)
                        {
                            canDelete = false;
                            if (carditem.isActive == 1)
                            {
                                long cId = (int)carditem.cardId;
                                var casht = entity.cashTransfer
                                    .Where(x => x.cardId == cId)
                                    .Select(x => new { x.cardId })
                                    .FirstOrDefault();

                                if ((casht is null))
                                    canDelete = true;
                            }
                            carditem.canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(cardsList);
                }
            }
        }

        // GET api/<controller>  Get card By ID
        [HttpPost]
        [Route("GetcardByID")]
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
                long cId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        cId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var card = entity.cards
                        .Where(c => c.cardId == cId)
                        .Select(
                            c =>
                                new
                                {
                                    c.cardId,
                                    c.name,
                                    c.notes,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId,
                                    c.isActive,
                                    c.hasProcessNum,
                                    image = c.image,
                                    c.commissionValue,
                                    c.commissionRatio,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(card);
                }
            }
        }

        // GET api/<controller>  Get card By is active
        [HttpPost]
        [Route("GetByisActive")]
        public string GetByisActive(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                int isActive = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "isActive")
                    {
                        isActive = int.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var card = entity.cards
                        .Where(c => c.isActive == isActive)
                        .Select(
                            c =>
                                new
                                {
                                    c.cardId,
                                    c.name,
                                    c.notes,
                                    c.createDate,
                                    c.updateDate,
                                    c.createUserId,
                                    c.updateUserId,
                                    c.isActive,
                                    c.hasProcessNum,
                                    c.image,
                                    c.commissionValue,
                                    c.commissionRatio,
                                }
                        )
                        .ToList();
                    return TokenManager.GenerateToken(card);
                }
            }
        }

        // add or update card
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
                string cardObject = "";
                cards Object = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        cardObject = c.Value.Replace("\\", string.Empty);
                        cardObject = cardObject.Trim('"');
                        Object = JsonConvert.DeserializeObject<cards>(
                            cardObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        cards tmpcard = new cards();
                        var cardEntity = entity.Set<cards>();
                        if (Object.cardId == 0)
                        {
                            Object.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            Object.updateUserId = Object.createUserId;
                            tmpcard = cardEntity.Add(Object);
                            entity.SaveChanges();
                            message = tmpcard.cardId.ToString();
                        }
                        else
                        {
                            tmpcard = entity.cards
                                .Where(p => p.cardId == Object.cardId)
                                .FirstOrDefault();
                            tmpcard.cardId = Object.cardId;
                            tmpcard.name = Object.name;
                            tmpcard.notes = Object.notes;
                            tmpcard.createDate = Object.createDate;
                            tmpcard.updateDate = Object.updateDate;
                            tmpcard.createUserId = Object.createUserId;
                            tmpcard.updateUserId = Object.updateUserId;
                            tmpcard.isActive = Object.isActive;
                            tmpcard.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date;
                            tmpcard.updateUserId = Object.updateUserId;
                            tmpcard.hasProcessNum = Object.hasProcessNum;
                            tmpcard.image = Object.image;
                            tmpcard.commissionValue = Object.commissionValue;
                            tmpcard.commissionRatio = Object.commissionRatio;

                            entity.SaveChanges();
                            message = tmpcard.cardId.ToString();
                        }
                    }
                    return TokenManager.GenerateToken(message);
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
                long cardId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        cardId = long.Parse(c.Value);
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
                            cards cardObj = entity.cards.Find(cardId);
                            entity.cards.Remove(cardObj);
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
                            cards cardObj = entity.cards.Find(cardId);

                            cardObj.isActive = 0;
                            cardObj.updateUserId = userId;
                            cardObj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
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
                        System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\card"),
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
        [Route("UpdateImage")]
        public string UpdateImage(string token)
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
                string cardObject = "";
                cards cardObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        cardObject = c.Value.Replace("\\", string.Empty);
                        cardObject = cardObject.Trim('"');
                        cardObj = JsonConvert.DeserializeObject<cards>(
                            cardObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    cards card;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var userEntity = entity.Set<cards>();
                        card = entity.cards.Where(p => p.cardId == cardObj.cardId).First();
                        card.image = cardObj.image;
                        entity.SaveChanges();
                    }
                    message = card.cardId.ToString();
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [Route("PostCardImage")]
        public IHttpActionResult PostCardImage()
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
                                "Please Upload image of type .jpg,.gif,.png, .jfif, .bmp , .jpeg ,.tiff"
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
                            //  check if image exist
                            var pathCheck = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\card"),
                                imageWithNoExt
                            );
                            var files = Directory.GetFiles(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\card"),
                                imageWithNoExt + ".*"
                            );
                            if (files.Length > 0)
                            {
                                File.Delete(files[0]);
                            }

                            //Userimage myfolder name where i want to save my image
                            var filePath = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\card"),
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
    }
}
