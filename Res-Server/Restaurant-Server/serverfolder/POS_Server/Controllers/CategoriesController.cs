using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web;
using System.Web.Http;

using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Categories")]
    public class CategoriesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/category
        List<long> categoriesId = new List<long>();

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
                Boolean canDelete = false;
                string type = "";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var searchPredicate = PredicateBuilder.New<categories>();
                    if (type == "")
                        searchPredicate = searchPredicate.And(x => true);
                    else
                        searchPredicate = searchPredicate.And(x => x.type == type);
                    var categoriesList = entity.categories
                        .Where(searchPredicate)
                        .Select(
                            p =>
                                new CategoryModel()
                                {
                                    categoryId = p.categoryId,
                                    name = p.name,
                                    categoryCode = p.categoryCode,
                                    createDate = p.createDate,
                                    createUserId = p.createUserId,
                                    details = p.details,
                                    image = p.image,
                                    notes = p.notes,
                                    taxes = p.taxes,
                                    updateDate = p.updateDate,
                                    updateUserId = p.updateUserId,
                                    isActive = p.isActive,
                                    type = p.type,
                                }
                        )
                        .ToList();

                    if (categoriesList.Count > 0)
                    {
                        for (int i = 0; i < categoriesList.Count; i++)
                        {
                            canDelete = false;
                            if (categoriesList[i].isActive == 1)
                            {
                                long categoryId = (long)categoriesList[i].categoryId;
                                var items = entity.items
                                    .Where(x => x.categoryId == categoryId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                //  var childCategoryL = entity.categories.Where(x => x.parentId == categoryId).Select(b => new { b.categoryId }).FirstOrDefault();

                                if ((items is null))
                                    canDelete = true;
                            }
                            categoriesList[i].canDelete = canDelete;
                        }
                    }
                    return TokenManager.GenerateToken(categoriesList);
                }
            }
        }

        [HttpPost]
        [Route("GetAllCategories")]
        public string GetAllCategories(string token)
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
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var categoriesList = (
                        from p in entity.categories
                        //   join cu in entity.categoryuser on p.categoryId equals cu.categoryId where cu.userId == userId
                        select new CategoryModel()
                        {
                            categoryId = p.categoryId,
                            name = p.name,
                            categoryCode = p.categoryCode,
                            createDate = p.createDate,
                            createUserId = p.createUserId,
                            details = p.details,
                            image = p.image,
                            notes = p.notes,
                            taxes = p.taxes,
                            updateDate = p.updateDate,
                            updateUserId = p.updateUserId,
                            isActive = p.isActive,
                            //  sequence = cu.sequence,
                            type = p.type,
                        }
                    ).ToList().OrderBy(x => x.sequence).ToList();
                    if (categoriesList.Count > 0)
                    {
                        for (int i = 0; i < categoriesList.Count; i++)
                        {
                            canDelete = false;
                            if (categoriesList[i].isActive == 1)
                            {
                                long categoryId = (long)categoriesList[i].categoryId;
                                var items = entity.items
                                    .Where(x => x.categoryId == categoryId)
                                    .Select(b => new { b.itemId })
                                    .FirstOrDefault();
                                //  var childCategoryL = entity.categories.Where(x => x.parentId == categoryId).Select(b => new { b.categoryId }).FirstOrDefault();

                                if ((items is null))
                                    canDelete = true;
                            }
                            categoriesList[i].canDelete = canDelete;
                        }
                    }

                    return TokenManager.GenerateToken(categoriesList);
                }
            }
        }

        [HttpPost]
        [Route("GetSubCategories")]
        public string GetSubCategories(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (categoryId != 0)
                    {
                        var categoriesList = (
                            from p in entity.categories.Where(x => x.isActive == 1)
                            //join cu in entity.categoryuser on p.categoryId equals cu.categoryId
                            //  where cu.userId == userId
                            select new CategoryModel()
                            {
                                categoryId = p.categoryId,
                                name = p.name,
                                categoryCode = p.categoryCode,
                                createDate = p.createDate,
                                createUserId = p.createUserId,
                                details = p.details,
                                image = p.image,
                                notes = p.notes,
                                // parentId = p.parentId,
                                taxes = p.taxes,
                                updateDate = p.updateDate,
                                updateUserId = p.updateUserId,
                                isActive = p.isActive,
                                // sequence = cu.sequence,
                                type = p.type,
                            }
                        ).ToList().OrderBy(x => x.sequence).ToList();

                        return TokenManager.GenerateToken(categoriesList);
                    }
                    else
                    {
                        var categoriesList = (
                            from p in entity.categories.Where(x => x.isActive == 1)
                            //  join cu in entity.categoryuser on p.categoryId equals cu.categoryId
                            // where cu.userId == userId
                            select new CategoryModel()
                            {
                                categoryId = p.categoryId,
                                name = p.name,
                                categoryCode = p.categoryCode,
                                createDate = p.createDate,
                                createUserId = p.createUserId,
                                details = p.details,
                                image = p.image,
                                notes = p.notes,
                                // parentId = p.parentId,
                                taxes = p.taxes,
                                updateDate = p.updateDate,
                                updateUserId = p.updateUserId,
                                isActive = p.isActive,
                                //  sequence = cu.sequence,
                                type = p.type,
                            }
                        ).ToList().OrderBy(x => x.sequence).ToList();

                        return TokenManager.GenerateToken(categoriesList);
                    }
                }
            }
        }

        // GET api/category/5
        [HttpPost]
        [Route("GetCategoryByID")]
        public string GetCategoryByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var category = entity.categories
                        .Where(c => c.categoryId == categoryId)
                        .Select(
                            p =>
                                new
                                {
                                    p.categoryId,
                                    p.name,
                                    p.categoryCode,
                                    p.createDate,
                                    p.createUserId,
                                    p.details,
                                    p.image,
                                    p.notes,
                                    //  p.parentId,
                                    p.taxes,
                                    p.updateDate,
                                    p.updateUserId,
                                    p.type,
                                    p.isActive,
                                }
                        )
                        .FirstOrDefault();
                    return TokenManager.GenerateToken(category);
                }
            }
        }

        [HttpPost]
        [Route("GetCategoryTreeByID")]
        public string GetCategoryTreeByID(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            List<categories> treecat = new List<categories>();
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryID = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        categoryID = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    long parentid = categoryID; // if want to show the last category
                    while (parentid > 0)
                    {
                        categories tempcate = new categories();
                        var category = entity.categories
                            .Where(c => c.categoryId == parentid)
                            .Select(
                                p =>
                                    new
                                    {
                                        p.categoryId,
                                        p.name,
                                        p.categoryCode,
                                        p.createDate,
                                        p.createUserId,
                                        p.details,
                                        p.image,
                                        p.notes,
                                        // p.parentId,
                                        p.taxes,
                                        p.updateDate,
                                        p.updateUserId,
                                        p.type,
                                        p.isActive,
                                    }
                            )
                            .FirstOrDefault();

                        tempcate.categoryId = category.categoryId;

                        tempcate.name = category.name;
                        tempcate.categoryCode = category.categoryCode;
                        tempcate.createDate = category.createDate;
                        tempcate.createUserId = category.createUserId;
                        tempcate.details = category.details;
                        tempcate.image = category.image;
                        tempcate.notes = category.notes;
                        //   tempcate.parentId = category.parentId;
                        tempcate.taxes = category.taxes;
                        tempcate.updateDate = category.updateDate;
                        tempcate.updateUserId = category.updateUserId;
                        tempcate.isActive = category.isActive;

                        // parentid = (int)tempcate.parentId;

                        treecat.Add(tempcate);
                    }
                    return TokenManager.GenerateToken(treecat);
                }
            }
        }

        [HttpPost]
        [Route("GetSubCategoriesSeq")]
        public string GetSubCategoriesSeq(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                long userId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        categoryId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    if (categoryId != 0)
                    {
                        var categoriesList = (
                            from C in entity.categories
                            // join S in entity.categoryuser on C.categoryId equals S.categoryId into jS
                            //   from jSS in jS.DefaultIfEmpty()
                            select new
                            {
                                C.categoryId,
                                C.name,
                                C.categoryCode,
                                C.createDate,
                                C.createUserId,
                                C.details,
                                C.image,
                                C.notes,
                                // C.parentId,
                                C.taxes,
                                C.updateDate,
                                C.updateUserId,
                                C.isActive,
                                //jSS.sequence,
                                //  jSS.userId,
                                C.type,
                            }
                        ).Where(c => c.isActive == 1).ToList();

                        return TokenManager.GenerateToken(categoriesList);
                    }
                    else
                    {
                        var categoriesList = (
                            from C in entity.categories
                            // join S in entity.categoryuser on C.categoryId equals S.categoryId into jS
                            //   from jSS in jS.DefaultIfEmpty()
                            select new
                            {
                                C.categoryId,
                                C.name,
                                C.categoryCode,
                                C.createDate,
                                C.createUserId,
                                C.details,
                                C.image,
                                C.notes,
                                //   C.parentId,
                                C.taxes,
                                C.updateDate,
                                C.updateUserId,
                                C.isActive,
                                // jSS.sequence,
                                // jSS.userId,
                                C.type,
                            }
                        ).Where(c => c.isActive == 1).ToList();

                        return TokenManager.GenerateToken(categoriesList);
                    }
                }
            }
        }

        // add or update category
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
                string categoryObject = "";
                categories newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        categoryObject = c.Value.Replace("\\", string.Empty);
                        categoryObject = categoryObject.Trim('"');
                        newObject = JsonConvert.DeserializeObject<categories>(
                            categoryObject,
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
                try
                {
                    categories tmpCategory;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var categoryEntity = entity.Set<categories>();
                        //  var catEntity = entity.Set<categoryuser>();
                        if (newObject.categoryId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;

                            tmpCategory = categoryEntity.Add(newObject);
                            entity.SaveChanges();
                        }
                        else
                        {
                            tmpCategory = entity.categories.Find(newObject.categoryId);
                            tmpCategory.categoryCode = newObject.categoryCode;
                            tmpCategory.details = newObject.details;
                            tmpCategory.name = newObject.name;
                            tmpCategory.notes = newObject.notes;
                            tmpCategory.taxes = newObject.taxes;
                            tmpCategory.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpCategory.updateUserId = newObject.updateUserId;
                            tmpCategory.isActive = newObject.isActive;
                            tmpCategory.type = newObject.type;
                            entity.SaveChanges();
                            long categoryId = tmpCategory.categoryId;
                            byte isActivecat = tmpCategory.isActive;
                            long? updateuser = tmpCategory.updateUserId;

                            // disactive items related to selected category and subs
                            var catitems = entity.items
                                .Where(U => U.categoryId == categoryId)
                                .ToList();
                            if (catitems.Count > 0)
                            {
                                for (int i = 0; i < catitems.Count; i++)
                                {
                                    catitems[i].isActive = (byte)isActivecat;
                                    catitems[i].updateUserId = updateuser;
                                    catitems[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    entity.items.AddOrUpdate(catitems[i]);
                                }
                                entity.SaveChanges();
                            }
                        }
                    }
                    message = tmpCategory.categoryId.ToString();
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
            string message = "0";
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long categoryId = 0;
                long userId = 0;
                Boolean final = false;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemId")
                    {
                        categoryId = long.Parse(c.Value);
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
                            var tmpCategory = entity.categories.Find(categoryId);
                            entity.categories.Remove(tmpCategory);

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
                            var tmpCategory = entity.categories.Find(categoryId);
                            tmpCategory.isActive = 0;
                            tmpCategory.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            tmpCategory.updateUserId = userId;
                            entity.categories.AddOrUpdate(tmpCategory);
                            entity.SaveChanges();

                            var catitems = entity.items
                                .Where(U => U.categoryId == categoryId)
                                .ToList();
                            if (catitems.Count > 0)
                            {
                                for (int i = 0; i < catitems.Count; i++)
                                {
                                    catitems[i].isActive = 0;
                                    catitems[i].updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    catitems[i].updateUserId = userId;
                                    entity.items.AddOrUpdate(catitems[i]);
                                }
                                entity.SaveChanges();
                            }
                            message = "1";
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
                string categoryObject = "";
                categories catObj = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "itemObject")
                    {
                        categoryObject = c.Value.Replace("\\", string.Empty);
                        categoryObject = categoryObject.Trim('"');
                        catObj = JsonConvert.DeserializeObject<categories>(
                            categoryObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                try
                {
                    categories category;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var agentEntity = entity.Set<agents>();
                        category = entity.categories
                            .Where(p => p.categoryId == catObj.categoryId)
                            .First();
                        category.image = catObj.image;
                        entity.SaveChanges();
                    }
                    message = category.categoryId.ToString();
                    return TokenManager.GenerateToken(message);
                }
                catch
                {
                    message = "0";
                    return TokenManager.GenerateToken(message);
                }
            }
        }

        [Route("PostCategoryImage")]
        public IHttpActionResult PostCategoryImage()
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
                                "Please Upload image of type .jpg,.gif,.png."
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
                                        "~\\images\\category"
                                    )
                                )
                            )
                                Directory.CreateDirectory(
                                    System.Web.Hosting.HostingEnvironment.MapPath(
                                        "~\\images\\category"
                                    )
                                );
                            //  check if image exist
                            var pathCheck = Path.Combine(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\category"
                                ),
                                imageWithNoExt
                            );
                            var files = Directory.GetFiles(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\category"
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
                                    "~\\images\\category"
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
            catch
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
                        System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\category"),
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
        public IEnumerable<categories> Recursive(List<categories> categoriesList, long toplevelid)
        {
            List<categories> inner = new List<categories>();
            foreach (var t in categoriesList)
            {
                categoriesId.Add(t.categoryId);
                inner.Add(t);
                inner = inner.Union(Recursive(categoriesList, t.categoryId)).ToList();
            }

            return inner;
        }
    }
}
