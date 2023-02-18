using Newtonsoft.Json;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;
using Newtonsoft.Json.Converters;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/setValues")]
    public class setValuesController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all setValues
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            // public ResponseVM GetPurinv(string token)




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
                        var list = entity.setValues
                            .Select(
                                c =>
                                    new
                                    {
                                        c.valId,
                                        c.value,
                                        c.isDefault,
                                        c.isSystem,
                                        c.notes,
                                        c.settingId,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
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

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var setValuesList = entity.setValues

            //       .Select(c => new  {
            //           c.valId,
            //          c.value,
            //          c.isDefault,
            //          c.isSystem,
            //          c.notes,
            //          c.settingId,

            //       })
            //       .ToList();

            //        /*
            //         *
            //          valId
            //          value
            //          isDefault
            //          isSystem
            //          notes
            //          settingId
            //         * */

            //        if (setValuesList == null)
            //            return NotFound();
            //        else
            //            return Ok(setValuesList);
            //    }
            //}
            ////else
            //    return NotFound();
        }

        // email
        [HttpPost]
        [Route("GetBySetName")]
        public string GetBySetName(string token)
        {
            // public ResponseVM GetPurinv(string token)name




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string name = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "name")
                    {
                        name = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        setting sett = entity.setting.Where(s => s.name == name).FirstOrDefault();
                        var list = entity.setValues
                            .Where(x => sett.settingId == x.settingId)
                            .Select(
                                X =>
                                    new
                                    {
                                        X.valId,
                                        X.value,
                                        X.isDefault,
                                        X.isSystem,
                                        X.settingId,
                                        X.notes,
                                    }
                            )
                            .ToList();
                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
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

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        setting sett = entity.setting.Where(s => s.name == name).FirstOrDefault();
            //       var setValuesList = entity.setValues.Where(x => sett.settingId == x.settingId)
            //            .Select(X=> new { X.valId,
            //                X.value,
            //                X.isDefault,
            //                X.isSystem,
            //                X.settingId,
            //                X.notes,

            //            })
            //            .ToList();

            //        if (setValuesList == null)
            //            return NotFound();
            //        else
            //            return Ok(setValuesList);
            //    }
            //}
            ////else
            //return NotFound();
        }

        [HttpPost]
        [Route("GetBySetNameAndUserId")]
        public string GetBySetNameAndUserId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string name = "";
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "name")
                    {
                        name = c.Value;
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        setting sett = entity.setting.Where(s => s.name == name).FirstOrDefault();
                        var list = (
                            from s in entity.setValues
                            where sett.settingId == s.settingId
                            join us in entity.userSetValues on s.valId equals us.valId
                            where us.userId == userId
                            select new setValuesModel()
                            {
                                valId = s.valId,
                                value = s.value,
                                isDefault = s.isDefault,
                                isSystem = s.isSystem,
                                settingId = s.settingId,
                                notes = s.notes,
                            }
                        ).FirstOrDefault();
                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public string GetBySettingName(string settingName)
        {
            setValues sv = new setValues();
            List<setValues> svl = new List<setValues>();

            // DateTime cmpdate = DateTime.Now.AddDays(newdays);
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    setting sett = entity.setting
                        .Where(s => s.name == settingName)
                        .FirstOrDefault();

                    var svlv = entity.setValues.ToList();
                    svl = svlv.Where(x => sett.settingId == x.settingId)
                        .Select(
                            X =>
                                new setValues
                                {
                                    valId = X.valId,
                                    value = X.value,
                                    isDefault = X.isDefault,
                                    isSystem = X.isSystem,
                                    settingId = X.settingId,
                                    notes = X.notes,
                                }
                        )
                        .ToList();
                    sv = svl.FirstOrDefault();
                    return sv.value;
                }
            }
            catch
            {
                // return ex.ToString();
                return "0";
            }
        }

        public setValues GetRowBySettingName(string settingName)
        {
            setValues sv = new setValues();
            List<setValues> svl = new List<setValues>();

            // DateTime cmpdate = DateTime.Now.AddDays(newdays);
            try
            {
                using (incposdbEntities entity = new incposdbEntities())
                {
                    setting sett = entity.setting
                        .Where(s => s.name == settingName)
                        .FirstOrDefault();

                    var svlv = entity.setValues.ToList();
                    svl = svlv.Where(x => sett.settingId == x.settingId)
                        .Select(
                            X =>
                                new setValues
                                {
                                    valId = X.valId,
                                    value = X.value,
                                    isDefault = X.isDefault,
                                    isSystem = X.isSystem,
                                    settingId = X.settingId,
                                    notes = X.notes,
                                }
                        )
                        .ToList();
                    sv = svl.FirstOrDefault();
                    return sv;
                }
            }
            catch
            {
                sv = new setValues();
                // return ex.ToString();
                return sv;
            }
        }

        [HttpPost]
        [Route("GetBySetvalNote")]
        public string GetBySetvalNote(string token)
        {
            // public ResponseVM GetPurinv(string token)setvalnote




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string setvalnote = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "setvalnote")
                    {
                        setvalnote = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        // setting sett = entity.setting.Where(s => s.name == name).FirstOrDefault();
                        var list = entity.setValues
                            .ToList()
                            .Where(x => x.notes == setvalnote)
                            .Select(
                                X =>
                                    new
                                    {
                                        X.valId,
                                        X.value,
                                        X.isDefault,
                                        X.isSystem,
                                        X.settingId,
                                        X.notes,
                                        name = entity.setting
                                            .ToList()
                                            .Where(s => s.settingId == X.settingId)
                                            .FirstOrDefault()
                                            .name,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(list);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
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

            //if (valid) // APIKey is valid
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        //setting sett = entity.setting.Where(s => s.name == name).FirstOrDefault();
            //        var setValuesList = entity.setValues.ToList().Where(x => x.notes == setvalnote)
            //             .Select(X => new {
            //                 X.valId,
            //                 X.value,
            //                 X.isDefault,
            //                 X.isSystem,
            //                 X.settingId,
            //                 X.notes,
            //                 name= entity.setting.ToList().Where(s => s.settingId == X.settingId).FirstOrDefault().name,

            //    })
            //             .ToList();

            //        if (setValuesList == null)
            //            return NotFound();
            //        else
            //            return Ok(setValuesList);
            //    }
            //}
            ////else
            //return NotFound();
        }

        // GET api/<controller>  Get medal By ID
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            // public ResponseVM GetPurinv(string token)Id




            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Id")
                    {
                        Id = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var item = entity.setValues
                            .Where(c => c.valId == Id)
                            .Select(
                                c =>
                                    new
                                    {
                                        c.valId,
                                        c.value,
                                        c.isDefault,
                                        c.isSystem,
                                        c.notes,
                                        c.settingId,
                                    }
                            )
                            .FirstOrDefault();
                        return TokenManager.GenerateToken(item);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            //var re = Request;
            //
            //string token = "";
            //long cId = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("Id"))
            //{
            //    cId = Convert.ToInt32(headers.GetValues("Id").First());
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var list = entity.setValues
            //       .Where(c => c.valId == cId)
            //       .Select(c => new {
            //           c.valId,
            //           c.value,
            //           c.isDefault,
            //           c.isSystem,
            //           c.notes,
            //           c.settingId,


            //       })
            //       .FirstOrDefault();

            //        if (list == null)
            //            return NotFound();
            //        else
            //            return Ok(list);
            //    }
            //}
            //else
            //    return NotFound();
        }

        // add or update medal
        [HttpPost]
        [Route("Save")]
        public string Save(string token)
        {
            //string Object string newObject
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
                setValues newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<setValues>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    setValues tmpObject = null;

                    try
                    {
                        if (newObject.settingId == 0 || newObject.settingId == null)
                        {
                            Nullable<long> id = null;
                            newObject.settingId = id;
                        }
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var sEntity = entity.Set<setValues>();
                            setValues defItem = entity.setValues
                                .Where(p => p.settingId == newObject.settingId && p.isDefault == 1)
                                .FirstOrDefault();

                            if (newObject.valId == 0)
                            {
                                if (newObject.isDefault == 1)
                                { // get the row with same settingId of newObject
                                    if (defItem != null)
                                    {
                                        defItem.isDefault = 0;
                                        entity.SaveChanges();
                                    }
                                }
                                else //Object.isDefault ==0
                                {
                                    if (defItem == null) //other values isDefault not 1
                                    {
                                        newObject.isDefault = 1;
                                    }
                                }
                                sEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.valId.ToString();
                            }
                            else
                            {
                                if (newObject.isDefault == 1)
                                {
                                    defItem.isDefault = 0; //reset the other default to 0 if exist
                                }
                                tmpObject = entity.setValues
                                    .Where(p => p.valId == newObject.valId)
                                    .FirstOrDefault();
                                tmpObject.valId = newObject.valId;
                                tmpObject.notes = newObject.notes;
                                tmpObject.value = newObject.value;
                                tmpObject.isDefault = newObject.isDefault;
                                tmpObject.isSystem = newObject.isSystem;

                                tmpObject.settingId = newObject.settingId;
                                entity.SaveChanges();
                                message = tmpObject.valId.ToString();
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

                return TokenManager.GenerateToken(message);
            }

            //var re = Request;
            //
            //string token = "";
            //string message ="";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    newObject = newObject.Replace("\\", string.Empty);
            //    newObject = newObject.Trim('"');
            //    setValues Object = JsonConvert.DeserializeObject<setValues>(newObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    try
            //    {
            //        if (Object.settingId == 0 || Object.settingId == null)
            //        {
            //            Nullable<long> id = null;
            //            Object.settingId = id;
            //        }
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var sEntity = entity.Set<setValues>();
            //            setValues defItem = entity.setValues.Where(p => p.settingId == Object.settingId && p.isDefault == 1).FirstOrDefault();

            //            if (Object.valId == 0)
            //            {
            //                if (Object.isDefault == 1 )
            //                { // get the row with same settingId of newObject
            //                     if (defItem != null)
            //                    {
            //                        defItem.isDefault = 0;
            //                        entity.SaveChanges();
            //                    }
            //                }
            //                else //Object.isDefault ==0
            //                {
            //                    if (defItem == null)//other values isDefault not 1
            //                    {
            //                        Object.isDefault =1;
            //                    }

            //                }
            //                    sEntity.Add(Object);
            //              message = Object.valId.ToString();
            //                entity.SaveChanges();
            //            }
            //            else
            //            {
            //                if (Object.isDefault == 1)
            //                {
            //                    defItem.isDefault = 0;//reset the other default to 0 if exist
            //                }
            //                var tmps = entity.setValues.Where(p => p.valId == Object.valId).FirstOrDefault();
            //                tmps.valId = Object.valId;
            //                tmps.notes = Object.notes;
            //                tmps.value = Object.value;
            //                tmps.isDefault=Object.isDefault;
            //                tmps.isSystem=Object.isSystem;

            //                tmps.settingId=Object.settingId;
            //                entity.SaveChanges();
            //                message = tmps.valId.ToString();
            //            }


            //        }
            //        return message; ;
            //    }

            //    catch
            //    {
            //        return "-1";
            //    }
            //}
            //else
            //    return "-1";
        }

        [HttpPost]
        [Route("SaveList")]
        public string SaveList(string token)
        {
            //string Object string newObject
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
                List<setValues> newObject = new List<setValues>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<List<setValues>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    try
                    {
                        long res = 0;
                        if (newObject.Count() > 0)
                        {
                            foreach (setValues valrow in newObject)
                            {
                                res = Save(valrow);
                            }
                        }

                        return TokenManager.GenerateToken(res.ToString());
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

        //email temp
        [HttpPost]
        [Route("SaveValueByNotes")]
        public string SaveValueByNotes(string token)
        {
            //string Object string newObject
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
                setValues newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<setValues>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    setValues tmpObject = null;

                    try
                    {
                        if (newObject.settingId == 0 || newObject.settingId == null)
                        {
                            Nullable<long> id = null;
                            newObject.settingId = id;
                        }
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            setValues defItem = new setValues();
                            var sEntity = entity.Set<setValues>();

                            defItem = entity.setValues
                                .Where(p => p.settingId == newObject.settingId)
                                .FirstOrDefault();

                            if (newObject.valId == 0)
                            {
                                if (newObject.isDefault == 1)
                                {
                                    // get the row with same settingId of newObject
                                    if (defItem != null)
                                    {
                                        defItem.isDefault = 0;
                                        entity.SaveChanges();
                                    }
                                }
                                else //newObject.isDefault ==0
                                {
                                    if (defItem == null) //other values isDefault not 1
                                    {
                                        newObject.isDefault = 1;
                                    }
                                }
                                sEntity.Add(newObject);
                                message = newObject.valId.ToString();
                                entity.SaveChanges();
                            }
                            else
                            {
                                if (newObject.isDefault == 1)
                                {
                                    defItem.isDefault = 0; //reset the other default to 0 if exist
                                }
                                var tmps1 = sEntity.ToList();
                                tmpObject = tmps1
                                    .Where(
                                        p =>
                                            p.notes == newObject.notes
                                            && p.settingId == newObject.settingId
                                            && p.valId == newObject.valId
                                    )
                                    .FirstOrDefault();
                                //   tmpObject.valId = newObject.valId;
                                // tmpObject.notes = newObject.notes;
                                tmpObject.value = newObject.value;
                                tmpObject.isDefault = newObject.isDefault;
                                tmpObject.isSystem = newObject.isSystem;

                                tmpObject.settingId = newObject.settingId;
                                entity.SaveChanges();
                                message = tmpObject.valId.ToString();
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
            //    newObject = newObject.Replace("\\", string.Empty);
            //    newObject = newObject.Trim('"');
            //    setValues Object = JsonConvert.DeserializeObject<setValues>(newObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    try
            //    {
            //        if (Object.settingId == 0 || Object.settingId == null)
            //        {
            //            Nullable<long> id = null;
            //            Object.settingId = id;
            //        }
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            setValues defItem = new setValues();
            //            var sEntity = entity.Set<setValues>();

            //                defItem = entity.setValues.Where(p => p.settingId == Object.settingId ).FirstOrDefault();



            //            if (Object.valId == 0)
            //            {
            //                if (Object.isDefault == 1)
            //                {
            //                    // get the row with same settingId of newObject
            //                    if (defItem != null)
            //                    {
            //                        defItem.isDefault = 0;
            //                        entity.SaveChanges();
            //                    }
            //                }
            //                else //Object.isDefault ==0
            //                {
            //                    if (defItem == null)//other values isDefault not 1
            //                    {
            //                        Object.isDefault = 1;
            //                    }

            //                }
            //                sEntity.Add(Object);
            //                message = Object.valId.ToString();
            //                entity.SaveChanges();
            //            }
            //            else
            //            {
            //                if (Object.isDefault == 1)
            //                {
            //                    defItem.isDefault = 0;//reset the other default to 0 if exist
            //                }
            //                var tmps1 = sEntity.ToList();
            //                var tmps = tmps1.Where(p => p.notes == Object.notes &&  p.settingId == Object.settingId && p.valId == Object.valId).FirstOrDefault();
            //             //   tmps.valId = Object.valId;
            //               // tmps.notes = Object.notes;
            //                tmps.value = Object.value;
            //                tmps.isDefault = Object.isDefault;
            //                tmps.isSystem = Object.isSystem;

            //                tmps.settingId = Object.settingId;
            //                entity.SaveChanges();
            //                message = tmps.valId.ToString();
            //            }


            //        }
            //        return message; ;
            //    }

            //    catch (Exception ex)
            //    {
            //        return ex.ToString();
            //    }
            //}
            //else
            //    return "-2";
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
        {
            // public ResponseVM Delete(string token)long Id, long userId
            //long Id, long userId
            string message = "";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long Id = 0;
                long userId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Id")
                    {
                        Id = long.Parse(c.Value);
                    }
                    else if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        setValues sObj = entity.setValues.Find(Id);

                        entity.setValues.Remove(sObj);
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
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}

            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);
            //if (valid)
            //{

            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                setValues sObj = entity.setValues.Find(Id);

            //                entity.setValues.Remove(sObj);
            //                entity.SaveChanges();

            //                return Ok("medal is Deleted Successfully");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }




            //}
            //else
            //    return NotFound();
        }

        // image
        #region Image

        [Route("PostImage")]
        public IHttpActionResult PostUserImage()
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
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\setvalues"
                                ),
                                imageWithNoExt
                            );
                            var files = Directory.GetFiles(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    "~\\images\\setvalues"
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
                                    "~\\images\\setvalues"
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
                        System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\setValues"),
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

        // update database record
        [HttpPost]
        [Route("UpdateImage")]
        public string UpdateImage(string token)
        {
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
                setValues newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<setValues>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                if (newObject != null)
                {
                    try
                    {
                        setValues Setvalue;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var Entity = entity.Set<setValues>();
                            Setvalue = entity.setValues
                                .Where(p => p.valId == newObject.valId)
                                .First();
                            Setvalue.value = newObject.value;
                            entity.SaveChanges();
                        }
                        // return Setvalue.valId;
                        return TokenManager.GenerateToken(Setvalue.valId.ToString());
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
        }

        #endregion
        public long Save(setValues newObject)
        {
            string message = "";
            long res = 0;

            if (newObject != null)
            {
                setValues tmpObject = null;

                try
                {
                    if (newObject.settingId == 0 || newObject.settingId == null)
                    {
                        Nullable<long> id = null;
                        newObject.settingId = id;
                    }
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var sEntity = entity.Set<setValues>();
                        setValues defItem = entity.setValues
                            .Where(p => p.settingId == newObject.settingId && p.isDefault == 1)
                            .FirstOrDefault();

                        if (newObject.valId == 0)
                        {
                            if (newObject.isDefault == 1)
                            { // get the row with same settingId of newObject
                                if (defItem != null)
                                {
                                    defItem.isDefault = 0;
                                    entity.SaveChanges();
                                }
                            }
                            else //Object.isDefault ==0
                            {
                                if (defItem == null) //other values isDefault not 1
                                {
                                    newObject.isDefault = 1;
                                }
                            }
                            sEntity.Add(newObject);
                            res = newObject.valId;

                            message = res.ToString();
                            entity.SaveChanges();
                        }
                        else
                        {
                            //update
                            if (newObject.isDefault == 1)
                            {
                                defItem.isDefault = 0; //reset the other default to 0 if exist
                            }
                            tmpObject = entity.setValues
                                .Where(p => p.valId == newObject.valId)
                                .FirstOrDefault();
                            tmpObject.valId = newObject.valId;
                            tmpObject.notes = newObject.notes;
                            tmpObject.value = newObject.value;
                            tmpObject.isDefault = newObject.isDefault;
                            tmpObject.isSystem = newObject.isSystem;

                            tmpObject.settingId = newObject.settingId;
                            entity.SaveChanges();
                            res = tmpObject.valId;
                            message = res.ToString();
                        }
                    }

                    return (res);
                }
                catch
                {
                    message = "0";
                    return 0;
                }
            }

            return res;
        }
    }
}
