using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using POS_Server.Models.VM;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/bonds")]
    public class BondsController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        // GET api/<controller> get all bonds
        [HttpPost]
        [Route("Get")]
        public string Get(string token)
        {
            //  public string Get(string token)




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
                    bool canDelete = false;
                    List<BondsModel> listb = new List<BondsModel>();
                    List<BondsModel> list = new List<BondsModel>();

                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        listb = ( // from C in entity.cashTransfer
                            from bo in entity.bondes
                            from C in entity.cashTransfer.Where(c => c.bondId == bo.bondId)
                            // join C in entity.cashTransfer on bo.bondId equals C.bondId into jc
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            join uc in entity.users on C.createUserId equals uc.userId into juc
                            join cr in entity.cards on C.cardId equals cr.cardId into jcr
                            join sh in entity.shippingCompanies
                                on C.shippingCompanyId equals sh.shippingCompanyId
                                into jsh
                            //  join b in entity.bondes on C.bondId  equals b.bondId  into jb


                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            from jucc in juc.DefaultIfEmpty()
                            from jcrd in jcr.DefaultIfEmpty()
                            from jshd in jsh.DefaultIfEmpty()
                            where (C.side != "bnd")
                            // from jc in C.DefaultIfEmpty()
                            //   where C.bondId==jbbo.bondId
                            orderby bo.bondId
                            select new BondsModel
                            {
                                bondId = bo.bondId,
                                number = bo.number,
                                amount = bo.amount,
                                deserveDate = bo.deserveDate,
                                type = bo.type,
                                isRecieved = bo.isRecieved,
                                notes = bo.notes,
                                createDate = bo.createDate,
                                updateDate = bo.updateDate,
                                createUserId = bo.createUserId,
                                updateUserId = bo.updateUserId,
                                isActive = bo.isActive,
                                cashTransId = bo.cashTransId,
                                // cash trans

                                ctcashTransId = C.cashTransId,
                                cttransType = C.transType,
                                ctposId = C.posId,
                                ctuserId = C.userId,
                                ctagentId = C.agentId,
                                ctinvId = C.invId,
                                cttransNum = C.transNum,
                                ctcreateDate = C.createDate,
                                ctupdateDate = C.updateDate,
                                ctcash = C.cash,
                                ctupdateUserId = C.updateUserId,
                                ctcreateUserId = C.createUserId,
                                ctnotes = C.notes,
                                ctposIdCreator = C.posIdCreator,
                                ctisConfirm = C.isConfirm,
                                ctcashTransIdSource = C.cashTransIdSource,
                                ctside = C.side,
                                ctdocName = C.docName,
                                ctdocNum = C.docNum,
                                ctdocImage = C.docImage,
                                ctbankId = C.bankId,
                                ctprocessType = C.processType,
                                ctcardId = C.cardId,
                                ctbondId = C.bondId,
                                // other tables

                                ctbankName = jbb.name,
                                ctagentName = jaa.name,
                                ctusersName = juu.name,
                                ctusersLName = juu.lastname,
                                ctposName = jpp.name,
                                ctposCreatorName = jpcc.name,
                                ctcreateUserName = jucc.name,
                                ctcreateUserJob = jucc.job,
                                ctcreateUserLName = jucc.lastname,
                                ctcardName = jcrd.name,
                                ctshippingCompanyId = jshd.shippingCompanyId,
                                ctshippingCompanyName = jshd.name
                            }
                        ).ToList();

                        // can delet or not
                        if (listb.Count > 0)
                        {
                            foreach (BondsModel bonditem in listb)
                            {
                                canDelete = false;
                                if (bonditem.isActive == 1)
                                {
                                    long cId = (long)bonditem.bondId;
                                    var casht = entity.cashTransfer
                                        .Where(x => x.bondId == cId)
                                        .Select(x => new { x.bondId })
                                        .FirstOrDefault();

                                    if ((casht is null))
                                        canDelete = true;
                                }
                                bonditem.canDelete = canDelete;
                            }

                            list = listb
                                .GroupBy(X => X.bondId)
                                .Select(
                                    x =>
                                        new BondsModel
                                        {
                                            bondId = x.FirstOrDefault().bondId,
                                            number = x.FirstOrDefault().number,
                                            amount = x.FirstOrDefault().amount,
                                            deserveDate = x.FirstOrDefault().deserveDate,
                                            type = x.FirstOrDefault().type,
                                            isRecieved = x.FirstOrDefault().isRecieved,
                                            notes = x.FirstOrDefault().notes,
                                            createDate = x.FirstOrDefault().createDate,
                                            updateDate = x.FirstOrDefault().updateDate,
                                            createUserId = x.FirstOrDefault().createUserId,
                                            updateUserId = x.FirstOrDefault().updateUserId,
                                            isActive = x.FirstOrDefault().isActive,
                                            cashTransId = x.FirstOrDefault().cashTransId,
                                            // cash trans

                                            ctcashTransId = x.FirstOrDefault().ctcashTransId,
                                            cttransType = x.FirstOrDefault().cttransType,
                                            ctposId = x.FirstOrDefault().ctposId,
                                            ctuserId = x.FirstOrDefault().ctuserId,
                                            ctagentId = x.FirstOrDefault().ctagentId,
                                            ctinvId = x.FirstOrDefault().ctinvId,
                                            cttransNum = x.FirstOrDefault().cttransNum,
                                            ctcreateDate = x.FirstOrDefault().ctcreateDate,
                                            ctupdateDate = x.FirstOrDefault().ctupdateDate,
                                            ctcash = x.FirstOrDefault().ctcash,
                                            ctupdateUserId = x.FirstOrDefault().ctupdateUserId,
                                            ctcreateUserId = x.FirstOrDefault().ctcreateUserId,
                                            ctnotes = x.FirstOrDefault().ctnotes,
                                            ctposIdCreator = x.FirstOrDefault().ctposIdCreator,
                                            ctisConfirm = x.FirstOrDefault().ctisConfirm,
                                            ctcashTransIdSource =
                                                x.FirstOrDefault().ctcashTransIdSource,
                                            ctside = x.FirstOrDefault().ctside,
                                            ctdocName = x.FirstOrDefault().ctdocName,
                                            ctdocNum = x.FirstOrDefault().ctdocNum,
                                            ctdocImage = x.FirstOrDefault().ctdocImage,
                                            ctbankId = x.FirstOrDefault().ctbankId,
                                            ctprocessType = x.FirstOrDefault().ctprocessType,
                                            ctcardId = x.FirstOrDefault().ctcardId,
                                            ctbondId = x.FirstOrDefault().ctbondId,
                                            // other tables

                                            ctbankName = x.FirstOrDefault().ctbankName,
                                            ctagentName = x.FirstOrDefault().ctagentName,
                                            ctusersName = x.FirstOrDefault().ctusersName,
                                            ctusersLName = x.FirstOrDefault().ctusersLName,
                                            ctposName = x.FirstOrDefault().ctposName,
                                            ctposCreatorName = x.FirstOrDefault().ctposCreatorName,
                                            ctcreateUserName = x.FirstOrDefault().ctcreateUserName,
                                            ctcreateUserJob = x.FirstOrDefault().ctcreateUserJob,
                                            ctcreateUserLName =
                                                x.FirstOrDefault().ctcreateUserLName,
                                            ctcardName = x.FirstOrDefault().ctcardName,
                                            ctshippingCompanyId =
                                                x.FirstOrDefault().ctshippingCompanyId,
                                            ctshippingCompanyName =
                                                x.FirstOrDefault().ctshippingCompanyName
                                        }
                                )
                                .ToList();
                        }
                    }
                    return TokenManager.GenerateToken(list);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //Boolean canDelete = false;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid) // APIKey is valid
            //{
            //    List<BondsModel> listb = new List<BondsModel>();
            //    List<BondsModel> listb2 = new List<BondsModel>();

            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        listb = (// from C in entity.cashTransfer
            //           from bo in entity.bondes
            //           from C in entity.cashTransfer.Where(c => c.bondId == bo.bondId)
            //               // join C in entity.cashTransfer on bo.bondId equals C.bondId into jc
            //       join b in entity.banks on C.bankId equals b.bankId into jb
            //           join a in entity.agents on C.agentId equals a.agentId into ja
            //           join p in entity.pos on C.posId equals p.posId into jp
            //           join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
            //           join u in entity.users on C.userId equals u.userId into ju
            //           join uc in entity.users on C.createUserId equals uc.userId into juc
            //           join cr in entity.cards on C.cardId equals cr.cardId into jcr
            //           join sh in entity.shippingCompanies on C.shippingCompanyId equals sh.shippingCompanyId into jsh

            //           //  join b in entity.bondes on C.bondId  equals b.bondId  into jb


            //           from jbb in jb.DefaultIfEmpty()
            //           from jaa in ja.DefaultIfEmpty()
            //           from jpp in jp.DefaultIfEmpty()
            //           from juu in ju.DefaultIfEmpty()
            //           from jpcc in jpcr.DefaultIfEmpty()
            //           from jucc in juc.DefaultIfEmpty()
            //           from jcrd in jcr.DefaultIfEmpty()
            //           from jshd in jsh.DefaultIfEmpty()
            //           where (C.side != "bnd")
            //       // from jc in C.DefaultIfEmpty()
            //       //   where C.bondId==jbbo.bondId
            //       orderby bo.bondId
            //           select new BondsModel
            //           {

            //               bondId = bo.bondId,
            //               number = bo.number,
            //               amount = bo.amount,
            //               deserveDate = bo.deserveDate,
            //               type = bo.type,
            //               isRecieved = bo.isRecieved,
            //               notes = bo.notes,

            //               createDate = bo.createDate,
            //               updateDate = bo.updateDate,
            //               createUserId = bo.createUserId,
            //               updateUserId = bo.updateUserId,
            //               isActive = bo.isActive,
            //               cashTransId = bo.cashTransId,
            //           // cash trans

            //           ctcashTransId = C.cashTransId,
            //               cttransType = C.transType,
            //               ctposId = C.posId,
            //               ctuserId = C.userId,
            //               ctagentId = C.agentId,
            //               ctinvId = C.invId,
            //               cttransNum = C.transNum,
            //               ctcreateDate = C.createDate,
            //               ctupdateDate = C.updateDate,
            //               ctcash = C.cash,
            //               ctupdateUserId = C.updateUserId,
            //               ctcreateUserId = C.createUserId,
            //               ctnotes = C.notes,
            //               ctposIdCreator = C.posIdCreator,
            //               ctisConfirm = C.isConfirm,
            //               ctcashTransIdSource = C.cashTransIdSource,
            //               ctside = C.side,
            //               ctdocName = C.docName,
            //               ctdocNum = C.docNum,
            //               ctdocImage = C.docImage,
            //               ctbankId = C.bankId,

            //               ctprocessType = C.processType,
            //               ctcardId = C.cardId,
            //               ctbondId = C.bondId,
            //           // other tables

            //           ctbankName = jbb.name,
            //               ctagentName = jaa.name,
            //               ctusersName = juu.name,
            //               ctusersLName = juu.lastname,
            //               ctposName = jpp.name,
            //               ctposCreatorName = jpcc.name,

            //               ctcreateUserName = jucc.name,
            //               ctcreateUserJob = jucc.job,
            //               ctcreateUserLName = jucc.lastname,
            //               ctcardName = jcrd.name,

            //               ctshippingCompanyId = jshd.shippingCompanyId,
            //               ctshippingCompanyName = jshd.name
            //           }).ToList();


            //        // can delet or not
            //        if (listb.Count > 0)
            //        {
            //            foreach (BondsModel bonditem in listb)
            //            {
            //                canDelete = false;
            //                if (bonditem.isActive == 1)
            //                {
            //                    long cId = (int)bonditem.bondId;
            //                    var casht = entity.cashTransfer.Where(x => x.bondId == cId).Select(x => new { x.bondId }).FirstOrDefault();

            //                    if ((casht is null))
            //                        canDelete = true;
            //                }
            //                bonditem.canDelete = canDelete;
            //            }

            //            listb2 = listb.GroupBy(X => X.bondId).Select(x => new BondsModel
            //            {
            //                bondId = x.FirstOrDefault().bondId,
            //                number = x.FirstOrDefault().number,
            //                amount = x.FirstOrDefault().amount,
            //                deserveDate = x.FirstOrDefault().deserveDate,
            //                type = x.FirstOrDefault().type,
            //                isRecieved = x.FirstOrDefault().isRecieved,
            //                notes = x.FirstOrDefault().notes,

            //                createDate = x.FirstOrDefault().createDate,
            //                updateDate = x.FirstOrDefault().updateDate,
            //                createUserId = x.FirstOrDefault().createUserId,
            //                updateUserId = x.FirstOrDefault().updateUserId,
            //                isActive = x.FirstOrDefault().isActive,
            //                cashTransId = x.FirstOrDefault().cashTransId,
            //                // cash trans

            //                ctcashTransId = x.FirstOrDefault().ctcashTransId,
            //                cttransType = x.FirstOrDefault().cttransType,
            //                ctposId = x.FirstOrDefault().ctposId,
            //                ctuserId = x.FirstOrDefault().ctuserId,
            //                ctagentId = x.FirstOrDefault().ctagentId,
            //                ctinvId = x.FirstOrDefault().ctinvId,
            //                cttransNum = x.FirstOrDefault().cttransNum,
            //                ctcreateDate = x.FirstOrDefault().ctcreateDate,
            //                ctupdateDate = x.FirstOrDefault().ctupdateDate,
            //                ctcash = x.FirstOrDefault().ctcash,
            //                ctupdateUserId = x.FirstOrDefault().ctupdateUserId,
            //                ctcreateUserId = x.FirstOrDefault().ctcreateUserId,
            //                ctnotes = x.FirstOrDefault().ctnotes,
            //                ctposIdCreator = x.FirstOrDefault().ctposIdCreator,
            //                ctisConfirm = x.FirstOrDefault().ctisConfirm,
            //                ctcashTransIdSource = x.FirstOrDefault().ctcashTransIdSource,
            //                ctside = x.FirstOrDefault().ctside,
            //                ctdocName = x.FirstOrDefault().ctdocName,
            //                ctdocNum = x.FirstOrDefault().ctdocNum,
            //                ctdocImage = x.FirstOrDefault().ctdocImage,
            //                ctbankId = x.FirstOrDefault().ctbankId,

            //                ctprocessType = x.FirstOrDefault().ctprocessType,
            //                ctcardId = x.FirstOrDefault().ctcardId,
            //                ctbondId = x.FirstOrDefault().ctbondId,
            //                // other tables

            //                ctbankName = x.FirstOrDefault().ctbankName,
            //                ctagentName = x.FirstOrDefault().ctagentName,
            //                ctusersName = x.FirstOrDefault().ctusersName,
            //                ctusersLName = x.FirstOrDefault().ctusersLName,
            //                ctposName = x.FirstOrDefault().ctposName,
            //                ctposCreatorName = x.FirstOrDefault().ctposCreatorName,

            //                ctcreateUserName = x.FirstOrDefault().ctcreateUserName,
            //                ctcreateUserJob = x.FirstOrDefault().ctcreateUserJob,
            //                ctcreateUserLName = x.FirstOrDefault().ctcreateUserLName,
            //                ctcardName = x.FirstOrDefault().ctcardName,
            //                ctshippingCompanyId =  x.FirstOrDefault().ctshippingCompanyId,
            //                ctshippingCompanyName = x.FirstOrDefault().ctshippingCompanyName

            //            }).ToList();

            //}

            /*


* */
            //        if (listb2 == null)
            //            return NotFound();
            //        else
            //            return Ok(listb2);
            //    }
            //}
            ////else
            //    return NotFound();
        }

        // GET api/<controller>  Get card By ID
        [HttpPost]
        [Route("GetbondByID")]
        public string GetbondByID(string token)
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
                    if (c.Type == "bondId")
                    {
                        Id = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var item = entity.bondes
                            .Where(c => c.bondId == Id)
                            .Select(
                                c =>
                                    new
                                    {
                                        c.bondId,
                                        c.number,
                                        c.amount,
                                        c.deserveDate,
                                        c.type,
                                        c.isRecieved,
                                        c.notes,
                                        c.createDate,
                                        c.updateDate,
                                        c.createUserId,
                                        c.updateUserId,
                                        c.isActive,
                                        c.cashTransId,
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
            //var headers = re.Headers;
            //string token = "";
            //long cId = 0;
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //if (headers.Contains("bondId"))
            //{
            //    cId = Convert.ToInt32(headers.GetValues("bondId").First());
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var bond = entity.bondes
            //       .Where(c => c.bondId == cId)
            //       .Select(c => new
            //       {
            //           c.bondId,
            //           c.number,
            //           c.amount,
            //           c.deserveDate,
            //           c.type,
            //           c.isRecieved,
            //           c.notes,
            //           c.createDate,
            //           c.updateDate,
            //           c.createUserId,
            //           c.updateUserId,
            //           c.isActive,
            //           c.cashTransId,

            //       })
            //       .FirstOrDefault();



            //        if (bond == null)
            //            return NotFound();
            //        else
            //            return Ok(bond);
            //    }
            //}
            //else
            //    return NotFound();
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
                byte isActive = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "isActive")
                    {
                        isActive = byte.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var list = entity.bondes
                            .Where(c => c.isActive == isActive)
                            .Select(
                                c =>
                                    new
                                    {
                                        c.bondId,
                                        c.number,
                                        c.amount,
                                        c.deserveDate,
                                        c.type,
                                        c.isRecieved,
                                        c.notes,
                                        c.createDate,
                                        c.updateDate,
                                        c.createUserId,
                                        c.updateUserId,
                                        c.isActive,
                                        c.cashTransId,
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
            //    var re = Request;
            //    var headers = re.Headers;
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
            //            var card = entity.bondes
            //           .Where(c => c.isActive == isActive)
            //           .Select(c => new
            //           {
            //               c.bondId,
            //               c.number,
            //               c.amount,
            //               c.deserveDate,
            //               c.type,
            //               c.isRecieved,
            //               c.notes,
            //               c.createDate,
            //               c.updateDate,
            //               c.createUserId,
            //               c.updateUserId,
            //               c.isActive,
            //               c.cashTransId,
            //           })
            //           .ToList();

            //            if (card == null)
            //                return NotFound();
            //            else
            //                return Ok(card);
            //        }
            //    }
            //    else
            //        return NotFound();
        }

        // add or update card
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
                bondes newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<bondes>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                if (newObject != null)
                {
                    bondes tmpObject = null;

                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var bondEntity = entity.Set<bondes>();
                            if (newObject.bondId == 0 || newObject.bondId == null)
                            {
                                newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                newObject.updateUserId = newObject.createUserId;
                                bondEntity.Add(newObject);
                                entity.SaveChanges();
                                message = newObject.bondId.ToString();
                            }
                            else
                            {
                                var tmpbond = entity.bondes
                                    .Where(p => p.bondId == newObject.bondId)
                                    .FirstOrDefault();

                                tmpbond.bondId = newObject.bondId;
                                tmpbond.number = newObject.number;
                                tmpbond.amount = newObject.amount;
                                tmpbond.deserveDate = newObject.deserveDate;
                                tmpbond.type = newObject.type;
                                tmpbond.isRecieved = newObject.isRecieved;
                                tmpbond.notes = newObject.notes;
                                tmpbond.createDate = newObject.createDate;
                                tmpbond.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date;
                                tmpbond.createUserId = newObject.createUserId;
                                tmpbond.updateUserId = newObject.updateUserId;
                                tmpbond.isActive = newObject.isActive;
                                tmpbond.cashTransId = newObject.cashTransId;

                                //message = "card Is Updated Successfully";
                                entity.SaveChanges();
                                message = tmpbond.bondId.ToString();
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
            //var headers = re.Headers;
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
            //    bondObject = bondObject.Replace("\\", string.Empty);
            //    bondObject = bondObject.Trim('"');
            //    bondes Object = JsonConvert.DeserializeObject<bondes>(bondObject, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    try
            //    {
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var bondEntity = entity.Set<bondes>();
            //            if (Object.bondId == 0 || Object.bondId == null)
            //            {

            //                Object.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Object.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Object.updateUserId = Object.createUserId;
            //                bondEntity.Add(Object);
            //                entity.SaveChanges();
            //                message = Object.bondId.ToString();
            //            }
            //            else
            //            {

            //                var tmpbond = entity.bondes.Where(p => p.bondId == Object.bondId).FirstOrDefault();

            //                tmpbond.bondId = Object.bondId;
            //                tmpbond.number = Object.number;
            //                tmpbond.amount = Object.amount;
            //                tmpbond.deserveDate = Object.deserveDate;
            //                tmpbond.type = Object.type;
            //                tmpbond.isRecieved = Object.isRecieved;
            //                tmpbond.notes = Object.notes;
            //                tmpbond.createDate = Object.createDate;
            //                tmpbond.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date;
            //                tmpbond.createUserId = Object.createUserId;
            //                tmpbond.updateUserId = Object.updateUserId;
            //                tmpbond.isActive = Object.isActive;
            //                tmpbond.cashTransId = Object.cashTransId;

            //                //message = "card Is Updated Successfully";
            //                entity.SaveChanges();
            //                message = tmpbond.bondId.ToString();

            //            }


            //        }
            //        return message;
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
        [Route("Delete")]
        public string Delete(string token)
        {
            // int bondId, long userId, Boolean final
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
                long bondId = 0;
                long userId = 0;
                bool final = false;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "bondId")
                    {
                        bondId = long.Parse(c.Value);
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
                            bondes bondObj = entity.bondes.Find(bondId);

                            entity.bondes.Remove(bondObj);
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
                else
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            bondes Obj = entity.bondes.Find(bondId);

                            Obj.isActive = 0;
                            Obj.updateUserId = userId;
                            Obj.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            message = entity.SaveChanges().ToString();

                            return TokenManager.GenerateToken(message);
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
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
            //    if (final)
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                bondes bondObj = entity.bondes.Find(bondId);

            //                entity.bondes.Remove(bondObj);
            //                entity.SaveChanges();

            //                return Ok("card is Deleted Successfully");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {
            //            using (incposdbEntities entity = new incposdbEntities())
            //            {
            //                bondes Obj = entity.bondes.Find(bondId);

            //                Obj.isActive = 0;
            //                Obj.updateUserId = userId;
            //                Obj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                entity.SaveChanges();

            //                return Ok("Offer is Deleted Successfully");
            //            }
            //        }
            //        catch
            //        {
            //            return NotFound();
            //        }
            //    }
            //}
            //else
            //    return NotFound();
        }
    }
}
