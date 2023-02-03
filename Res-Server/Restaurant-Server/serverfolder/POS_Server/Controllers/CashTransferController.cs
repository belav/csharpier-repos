using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS_Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using POS_Server.Models.VM;
using System.Security.Claims;
using System.Web;
using Newtonsoft.Json.Converters;
using POS_Server.Classes;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Cashtransfer")]
    public class CashTransferController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        List<string> hiddenCashes = new List<string>()
        {
            "inv",
            "deliver",
            "commissionAgent",
            "commissionCard"
        };

        public async Task<string> addCashTransfer(cashTransfer newObject)
        {
            string message = "";
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
            if (newObject.invId == 0 || newObject.invId == null)
            {
                Nullable<long> id = null;
                newObject.invId = id;
            }
            if (newObject.posIdCreator == 0 || newObject.posIdCreator == null)
            {
                Nullable<long> id = null;
                newObject.posIdCreator = id;
            }

            if (newObject.cashTransIdSource == 0 || newObject.cashTransIdSource == null)
            {
                Nullable<long> id = null;
                newObject.cashTransIdSource = id;
            }
            if (newObject.bankId == 0 || newObject.bankId == null)
            {
                Nullable<long> id = null;
                newObject.bankId = id;
            }

            cashTransfer cashtr;
            DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);
            using (incposdbEntities entity = new incposdbEntities())
            {
                var cEntity = entity.Set<cashTransfer>();
                if (newObject.cashTransId == 0)
                {
                    if (newObject.processType == "statement")
                    {
                        DateTime DT = new DateTime(datenow.Year, 1, 1, 0, 0, 0);
                        newObject.createDate = DT;
                        newObject.updateDate = DT;
                    }
                    else
                    {
                        newObject.createDate = datenow;
                        newObject.updateDate = datenow;
                    }

                    newObject.transNum = await generateCashNumber(newObject.transNum);

                    //newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    //newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    newObject.updateUserId = newObject.createUserId;
                    cashtr = cEntity.Add(newObject);
                }
                else
                {
                    cashtr = entity.cashTransfer
                        .Where(p => p.cashTransId == newObject.cashTransId)
                        .First();
                    cashtr.transType = newObject.transType;
                    cashtr.posId = newObject.posId;
                    cashtr.userId = newObject.userId;
                    cashtr.agentId = newObject.agentId;
                    cashtr.invId = newObject.invId;
                    cashtr.transNum = newObject.transNum;
                    cashtr.createDate = newObject.createDate;
                    cashtr.updateDate = datenow; // server current date
                    cashtr.cash = newObject.cash;
                    cashtr.updateUserId = newObject.updateUserId;
                    cashtr.notes = newObject.notes;
                    cashtr.posIdCreator = newObject.posIdCreator;
                    cashtr.isConfirm = newObject.isConfirm;
                    cashtr.cashTransIdSource = newObject.cashTransIdSource;
                    cashtr.side = newObject.side;

                    cashtr.docName = newObject.docName;
                    cashtr.docNum = newObject.docNum;
                    cashtr.docImage = newObject.docImage;
                    cashtr.bankId = newObject.bankId;
                    //  cashtr.updateDate = datenow;// server current date
                    cashtr.processType = newObject.processType;
                    cashtr.cardId = newObject.cardId;
                    cashtr.bondId = newObject.bondId;
                    cashtr.shippingCompanyId = newObject.shippingCompanyId;
                }
                entity.SaveChanges();
            }
            message = cashtr.cashTransId.ToString();
            return message;
        }

        [HttpPost]
        [Route("GetBytypeandSide")]
        public string GetBytypeAndSide(string token)
        {
            //string type, string side

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                string side = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<CashTransferModel> cachlist = (
                            from C in entity.cashTransfer
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            join uc in entity.users on C.createUserId equals uc.userId into juc
                            join cr in entity.cards on C.cardId equals cr.cardId into jcr
                            join bo in entity.bondes on C.bondId equals bo.bondId into jbo
                            join sh in entity.shippingCompanies
                                on C.shippingCompanyId equals sh.shippingCompanyId
                                into jsh
                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            from jucc in juc.DefaultIfEmpty()
                            from jcrd in jcr.DefaultIfEmpty()
                            from jbbo in jbo.DefaultIfEmpty()
                            from jssh in jsh.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                bankName = jbb.name,
                                agentName = jaa.name,
                                usersName = juu.name, // side =u
                                posName = jpp.name,
                                posCreatorName = jpcc.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                usersLName = juu.lastname, // side =u
                                createUserName = jucc.name,
                                createUserLName = jucc.lastname,
                                createUserJob = jucc.job,
                                cardName = jcrd.name,
                                bondDeserveDate = jbbo.deserveDate,
                                //  bondIsRecieved = jbbo.isRecieved,
                                shippingCompanyId = C.shippingCompanyId,
                                shippingCompanyName = jssh.name,
                                isConfirm2 = 0,
                            }
                        ).Where(C => ((type == "all") ? true : C.transType == type) && (C.processType != "balance") && ((side == "all") ? true : C.side == side) && !(C.agentId == null && C.userId == null && C.shippingCompanyId == null)).ToList();

                        if (cachlist.Count > 0 && side == "p")
                        {
                            CashTransferModel tempitem = null;
                            foreach (CashTransferModel cashtItem in cachlist)
                            {
                                tempitem = this.Getpostransmodel(cashtItem.cashTransId)
                                    .Where(C => C.cashTransId != cashtItem.cashTransId)
                                    .FirstOrDefault();
                                cashtItem.cashTrans2Id = tempitem.cashTransId;
                                cashtItem.pos2Id = tempitem.posId;
                                cashtItem.pos2Name = tempitem.posName;
                                cashtItem.isConfirm2 = tempitem.isConfirm;
                                // cashtItem.posCreatorName = tempitem.posName;
                            }
                        }

                        return TokenManager.GenerateToken(cachlist);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
            //       var re = Request;
            //       var headers = re.Headers;
            //       string token = "";

            //       /*
            //       string type = "";
            //       string side = "";
            //       */
            //       if (headers.Contains("APIKey"))
            //       {
            //           token = headers.GetValues("APIKey").First();
            //       }



            //       Validation validation = new Validation();
            //       bool valid = validation.CheckApiKey(token);
            //       /*
            //        *   cashTransId = ,
            //                 transType = ,
            //                 posId = ,
            //                 userId = ,
            //                 agentId = ,
            //                 invId = ,
            //                 transNum = ,
            //                 createDate = ,
            //                 updateDate = ,
            //                 cash = ,
            //                 updateUserId = ,
            //                 createUserId = ,
            //                 notes = ,
            //                 posIdCreator = ,
            //                isConfirm = ,
            //                 cashTransIdSource = ,
            //                 side = ,
            //                 opSideNum = ,
            //                 docName = ,
            //                 docNum = ,
            //                 docImage = ,
            //                 bankId = ,
            //        * */
            //       if (valid)
            //       {
            //           using (incposdbEntities entity = new incposdbEntities())
            //           {
            //               /*
            //                var cachlist =  entity.cashTransfer

            //  .Where(C => ((type == "all") ? true : C.transType == type)
            //         && ((side == "all") ? true : C.side == side))

            //    .Select(C => new CashTransferModel
            //                    {
            //      cashTransId = C.cashTransId,
            //      transType = C.transType,
            //      posId = C.posId,
            //      userId =C.userId,
            //      agentId =C.agentId,
            //      invId =C.invId,
            //      transNum =C.transNum,
            //      createDate = C.createDate,
            //      updateDate =C.updateDate,
            //      cash =C.cash,
            //      updateUserId =C.updateUserId,
            //      createUserId =C.createUserId,
            //      notes =C.notes,
            //      posIdCreator = C.posIdCreator,
            //     isConfirm =C.isConfirm,
            //      cashTransIdSource =C.cashTransIdSource,
            //      side =C.side,
            //      opSideNum =C.opSideNum,
            //      docName =C.docName,
            //      docNum =C.docNum,
            //      docImage =C.docImage,
            //      bankId =C.bankId,

            //}).ToList();
            //                */
            //               List<CashTransferModel> cachlist = (from C in entity.cashTransfer
            //                                                   join b in entity.banks on C.bankId equals b.bankId into jb
            //                                                   join a in entity.agents on C.agentId equals a.agentId into ja
            //                                                   join p in entity.pos on C.posId equals p.posId into jp
            //                                                   join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
            //                                                   join u in entity.users on C.userId equals u.userId into ju
            //                                                   join uc in entity.users on C.createUserId equals uc.userId into juc
            //                                                   join cr in entity.cards on C.cardId equals cr.cardId into jcr
            //                                                   join bo in entity.bondes on C.bondId equals bo.bondId into jbo
            //                                                   join sh in entity.shippingCompanies on C.shippingCompanyId equals sh.shippingCompanyId into jsh
            //                                                   from jbb in jb.DefaultIfEmpty()
            //                                                   from jaa in ja.DefaultIfEmpty()
            //                                                   from jpp in jp.DefaultIfEmpty()
            //                                                   from juu in ju.DefaultIfEmpty()
            //                                                   from jpcc in jpcr.DefaultIfEmpty()
            //                                                   from jucc in juc.DefaultIfEmpty()
            //                                                   from jcrd in jcr.DefaultIfEmpty()
            //                                                   from jbbo in jbo.DefaultIfEmpty()
            //                                                   from jssh in jsh.DefaultIfEmpty()
            //                                                   select new CashTransferModel()
            //                                                   {
            //                                                       cashTransId = C.cashTransId,
            //                                                       transType = C.transType,
            //                                                       posId = C.posId,
            //                                                       userId = C.userId,
            //                                                       agentId = C.agentId,
            //                                                       invId = C.invId,
            //                                                       transNum = C.transNum,
            //                                                       createDate = C.createDate,
            //                                                       updateDate = C.updateDate,
            //                                                       cash = C.cash,
            //                                                       updateUserId = C.updateUserId,
            //                                                       createUserId = C.createUserId,
            //                                                       notes = C.notes,
            //                                                       posIdCreator = C.posIdCreator,
            //                                                       isConfirm = C.isConfirm,
            //                                                       cashTransIdSource = C.cashTransIdSource,
            //                                                       side = C.side,

            //                                                       docName = C.docName,
            //                                                       docNum = C.docNum,
            //                                                       docImage = C.docImage,
            //                                                       bankId = C.bankId,
            //                                                       bankName = jbb.name,
            //                                                       agentName = jaa.name,
            //                                                       usersName = juu.name,// side =u

            //                                                       posName = jpp.name,
            //                                                       posCreatorName = jpcc.name,
            //                                                       processType = C.processType,
            //                                                       cardId = C.cardId,
            //                                                       bondId = C.bondId,
            //                                                       usersLName = juu.lastname,// side =u
            //                                                       createUserName = jucc.name,
            //                                                       createUserLName = jucc.lastname,
            //                                                       createUserJob = jucc.job,
            //                                                       cardName = jcrd.name,
            //                                                       bondDeserveDate = jbbo.deserveDate,
            //                                                       bondIsRecieved = jbbo.isRecieved,
            //                                                       shippingCompanyId = C.shippingCompanyId,
            //                                                       shippingCompanyName = jssh.name

            //                                                   }).Where(C => ((type == "all") ? true : C.transType == type)
            //       && ((side == "all") ? true : C.side == side) && !(C.agentId == null && C.userId == null && C.shippingCompanyId == null)).ToList();

            //               if (cachlist.Count > 0 && side == "p")
            //               {
            //                   CashTransferModel tempitem = null;
            //                   foreach (CashTransferModel cashtItem in cachlist)
            //                   {
            //                       tempitem = this.Getpostransmodel(cashtItem.cashTransId)
            //                           .Where(C => C.cashTransId != cashtItem.cashTransId).FirstOrDefault();
            //                       cashtItem.cashTrans2Id = tempitem.cashTransId;
            //                       cashtItem.pos2Id = tempitem.posId;
            //                       cashtItem.pos2Name = tempitem.posName;
            //                       cashtItem.isConfirm2 = tempitem.isConfirm;
            //                       // cashtItem.posCreatorName = tempitem.posName;


            //                   }

            //               }




            //               if (cachlist == null)
            //                   return NotFound();
            //               else
            //                   return Ok(cachlist);

            //           }
            //       }
            //       else
            //           return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("GetCashBond")]
        public string GetCashBond(string token)
        {
            //string type, string side

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                string side = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<CashTransferModel> cachlist = (
                            from C in entity.cashTransfer
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            join uc in entity.users on C.createUserId equals uc.userId into juc
                            join cr in entity.cards on C.cardId equals cr.cardId into jcr
                            join bo in entity.bondes on C.bondId equals bo.bondId into jbo
                            join sh in entity.shippingCompanies
                                on C.shippingCompanyId equals sh.shippingCompanyId
                                into jsh
                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            from jucc in juc.DefaultIfEmpty()
                            from jcrd in jcr.DefaultIfEmpty()
                            from jbbo in jbo.DefaultIfEmpty()
                            from jssh in jsh.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                bankName = jbb.name,
                                agentName = jaa.name,
                                usersName = juu.name, // side =u
                                posName = jpp.name,
                                posCreatorName = jpcc.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                usersLName = juu.lastname, // side =u
                                createUserName = jucc.name,
                                createUserLName = jucc.lastname,
                                createUserJob = jucc.job,
                                cardName = jcrd.name,
                                bondDeserveDate = jbbo.deserveDate,
                                //  bondIsRecieved = jbbo.isRecieved,
                                shippingCompanyId = C.shippingCompanyId,
                                shippingCompanyName = jssh.name,
                                isConfirm2 = 0,
                                isInvPurpose = C.isInvPurpose,
                                purpose = C.purpose,
                                paid = C.paid
                            }
                        ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side)).ToList();

                        return TokenManager.GenerateToken(cachlist);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetPayedByInvId")]
        public string GetPayedByInvId(string token)
        {
            //  string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invId")
                    {
                        invId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<CashTransferModel> cachtrans = new List<CashTransferModel>();
                        cachtrans = (
                            from C in entity.cashTransfer
                            join b in entity.cards on C.cardId equals b.cardId into jb
                            from Card in jb.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                invId = C.invId,
                                cash = C.cash,
                                cardName = Card.name,
                                processType = C.processType,
                                cardId = C.cardId,
                            }
                        ).Where(C => C.invId == invId && (C.processType == "card" || C.processType == "cash")).ToList();

                        int i = 0;
                        var cachtranslist = cachtrans
                            .GroupBy(x => x.cardId)
                            .Select(
                                x =>
                                    new
                                    {
                                        processType = x.FirstOrDefault().processType,
                                        cash = x.Sum(c => c.cash),
                                        cardId = x.FirstOrDefault().cardId,
                                        cardName = x.FirstOrDefault().processType == "card"
                                            ? x.FirstOrDefault().cardName
                                            : "cash",
                                        sequenc = x.FirstOrDefault().processType == "cash"
                                            ? 0
                                            : ++i,
                                    }
                            )
                            .OrderBy(c => c.cardId)
                            .ToList();

                        return TokenManager.GenerateToken(cachtranslist);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("GetBytypeAndSideForPos")]
        public string GetBytypeAndSideForPos(string token)
        {
            //string type, string side

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                string side = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        IEnumerable<CashTransferModel> cachlist = (
                            from C in entity.cashTransfer
                            //  join b in entity.banks on C.bankId equals b.bankId into jb
                            //  join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            join uc in entity.users on C.createUserId equals uc.userId into juc
                            //  join cr in entity.cards on C.cardId equals cr.cardId into jcr
                            //  join bo in entity.bondes on C.bondId equals bo.bondId into jbo
                            //  join sh in entity.shippingCompanies on C.shippingCompanyId equals sh.shippingCompanyId into jsh
                            // from jbb in jb.DefaultIfEmpty()
                            // from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            from jucc in juc.DefaultIfEmpty()
                            //  from jcrd in jcr.DefaultIfEmpty()
                            // from jbbo in jbo.DefaultIfEmpty()
                            // from jssh in jsh.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                // bankName = jbb.name,
                                // agentName = jaa.name,
                                usersName = juu.name, // side =u
                                posName = jpp.name,
                                posCreatorName = jpcc.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                //   bondId = C.bondId,
                                usersLName = juu.lastname, // side =u
                                createUserName = jucc.name,
                                createUserLName = jucc.lastname,
                                createUserJob = jucc.job,
                                //cardName = jcrd.name,
                                // bondDeserveDate = jbbo.deserveDate,
                                //bondIsRecieved =  jbbo.isRecieved,
                                //   shippingCompanyId = C.shippingCompanyId,
                                //   shippingCompanyName = jssh.name,

                                isConfirm2 = 0,
                            }
                        ).Where(C => ((type == "all") ? true : C.transType == type) && (C.processType != "balance") && ((side == "all") ? true : C.side == side)).ToList();

                        if (cachlist.Count() > 0 && side == "p")
                        {
                            CashTransferModel tempitem = new CashTransferModel();
                            foreach (CashTransferModel cashtItem in cachlist)
                            {
                                tempitem = this.Getpostransmodel(cashtItem.cashTransId)
                                    .Where(C => C.cashTransId != cashtItem.cashTransId)
                                    .FirstOrDefault();
                                cashtItem.cashTrans2Id = tempitem.cashTransId;
                                cashtItem.pos2Id = tempitem.posId;
                                cashtItem.pos2Name = tempitem.posName;
                                cashtItem.isConfirm2 = tempitem.isConfirm;
                                //  cashtItem.bondIsRecieved = cashtItem.bondIsRecieved == null ? Convert.ToByte(0) : cashtItem.bondIsRecieved;
                            }
                        }

                        return TokenManager.GenerateToken(cachlist);
                    }
                }
                catch (Exception ex)
                {
                    return TokenManager.GenerateToken("0");
                    // return TokenManager.GenerateToken(ex.ToString());
                }
            }
        }

        [HttpPost]
        [Route("GetCashTransferForPosById")]
        public string GetCashTransferForPosById(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string type = "";
                string side = "";
                long posId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                    else if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                #endregion
                try
                {
                    var cachlist = GetCashTransferForPosById(posId, type, side);
                    return TokenManager.GenerateToken(cachlist);
                    //    using (incposdbEntities entity = new incposdbEntities())
                    //    {
                    //        List<CashTransferModel> cachlist = (from C in entity.cashTransfer
                    //                                            //join b in entity.banks on C.bankId equals b.bankId into jb
                    //                                            //join a in entity.agents on C.agentId equals a.agentId into ja
                    //                                            join p in entity.pos on C.posId equals p.posId into jp
                    //                                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                    //                                            join u in entity.users on C.userId equals u.userId into ju
                    //                                            join uc in entity.users on C.createUserId equals uc.userId into juc
                    //                                            //join cr in entity.cards on C.cardId equals cr.cardId into jcr
                    //                                            //join bo in entity.bondes on C.bondId equals bo.bondId into jbo
                    //                                            //join sh in entity.shippingCompanies on C.shippingCompanyId equals sh.shippingCompanyId into jsh
                    //                                            //from jbb in jb.DefaultIfEmpty()
                    //                                            //from jaa in ja.DefaultIfEmpty()
                    //                                            from jpp in jp.DefaultIfEmpty()
                    //                                            from juu in ju.DefaultIfEmpty()
                    //                                            from jpcc in jpcr.DefaultIfEmpty()
                    //                                            from jucc in juc.DefaultIfEmpty()
                    //                                            //from jcrd in jcr.DefaultIfEmpty()
                    //                                            //from jbbo in jbo.DefaultIfEmpty()
                    //                                            //from jssh in jsh.DefaultIfEmpty()
                    //                                            select new CashTransferModel()
                    //                                            {
                    //                                                cashTransId = C.cashTransId,
                    //                                                transType = C.transType,
                    //                                                posId = C.posId,
                    //                                                userId = C.userId,
                    //                                                agentId = C.agentId,
                    //                                                invId = C.invId,
                    //                                                transNum = C.transNum,
                    //                                                createDate = C.createDate,
                    //                                                updateDate = C.updateDate,
                    //                                                cash = C.cash,
                    //                                                updateUserId = C.updateUserId,
                    //                                                createUserId = C.createUserId,
                    //                                                notes = C.notes,
                    //                                                posIdCreator = C.posIdCreator,
                    //                                                isConfirm = C.isConfirm,
                    //                                                cashTransIdSource = C.cashTransIdSource,
                    //                                                side = C.side,

                    //                                                docName = C.docName,
                    //                                                docNum = C.docNum,
                    //                                                docImage = C.docImage,
                    //                                                bankId = C.bankId,
                    //                                                //bankName = jbb.name,
                    //                                                //agentName = jaa.name,
                    //                                                usersName = juu.name,// side =u

                    //                                                posName = jpp.name,
                    //                                                posCreatorName = jpcc.name,
                    //                                                processType = C.processType,
                    //                                                cardId = C.cardId,
                    //                                                bondId = C.bondId,
                    //                                                usersLName = juu.lastname,// side =u
                    //                                                createUserName = jucc.name,
                    //                                                createUserLName = jucc.lastname,
                    //                                                createUserJob = jucc.job,
                    //                                                //cardName = jcrd.name,
                    //                                                //bondDeserveDate = jbbo.deserveDate,
                    //                                               // bondIsRecieved = jbbo.isRecieved,
                    //                                                shippingCompanyId = C.shippingCompanyId,
                    //                                                //shippingCompanyName = jssh.name
                    //                                                        isConfirm2 = 0,
                    //                                            }).Where(C => ((type == "all") ? true : C.transType == type) && (C.processType != "balance")
                    //&& ((side == "all") ? true : C.side == side)).ToList();

                    //        if (cachlist.Count > 0 && side == "p")
                    //        {
                    //            CashTransferModel tempitem = null;
                    //            foreach (CashTransferModel cashtItem in cachlist)
                    //            {
                    //                tempitem = this.Getpostransmodel(cashtItem.cashTransId)
                    //                    .Where(C => C.cashTransId != cashtItem.cashTransId).FirstOrDefault();
                    //                cashtItem.cashTrans2Id = tempitem.cashTransId;
                    //                cashtItem.pos2Id = tempitem.posId;
                    //                cashtItem.pos2Name = tempitem.posName;
                    //                cashtItem.isConfirm2 = tempitem.isConfirm;

                    //            }

                    //        }
                    //        cachlist = cachlist.Where(C => C.posId == posId || C.pos2Id == posId || C.posIdCreator == posId).ToList();

                    //        return TokenManager.GenerateToken(cachlist);


                    //    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public List<CashTransferModel> GetCashTransferForPosById(
            long posId,
            string type,
            string side
        )
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                List<CashTransferModel> cachlist = (
                    from C in entity.cashTransfer
                    //join b in entity.banks on C.bankId equals b.bankId into jb
                    //join a in entity.agents on C.agentId equals a.agentId into ja
                    join p in entity.pos on C.posId equals p.posId into jp
                    join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                    join u in entity.users on C.userId equals u.userId into ju
                    join uc in entity.users on C.createUserId equals uc.userId into juc
                    //join cr in entity.cards on C.cardId equals cr.cardId into jcr
                    //join bo in entity.bondes on C.bondId equals bo.bondId into jbo
                    //join sh in entity.shippingCompanies on C.shippingCompanyId equals sh.shippingCompanyId into jsh
                    //from jbb in jb.DefaultIfEmpty()
                    //from jaa in ja.DefaultIfEmpty()
                    from jpp in jp.DefaultIfEmpty()
                    from juu in ju.DefaultIfEmpty()
                    from jpcc in jpcr.DefaultIfEmpty()
                    from jucc in juc.DefaultIfEmpty()
                    //from jcrd in jcr.DefaultIfEmpty()
                    //from jbbo in jbo.DefaultIfEmpty()
                    //from jssh in jsh.DefaultIfEmpty()
                    select new CashTransferModel()
                    {
                        cashTransId = C.cashTransId,
                        transType = C.transType,
                        posId = C.posId,
                        userId = C.userId,
                        agentId = C.agentId,
                        invId = C.invId,
                        transNum = C.transNum,
                        createDate = C.createDate,
                        updateDate = C.updateDate,
                        cash = C.cash,
                        updateUserId = C.updateUserId,
                        createUserId = C.createUserId,
                        notes = C.notes,
                        posIdCreator = C.posIdCreator,
                        isConfirm = C.isConfirm,
                        cashTransIdSource = C.cashTransIdSource,
                        side = C.side,
                        docName = C.docName,
                        docNum = C.docNum,
                        docImage = C.docImage,
                        bankId = C.bankId,
                        //bankName = jbb.name,
                        //agentName = jaa.name,
                        usersName = juu.name, // side =u
                        posName = jpp.name,
                        posCreatorName = jpcc.name,
                        processType = C.processType,
                        cardId = C.cardId,
                        bondId = C.bondId,
                        usersLName = juu.lastname, // side =u
                        createUserName = jucc.name,
                        createUserLName = jucc.lastname,
                        createUserJob = jucc.job,
                        //cardName = jcrd.name,
                        //bondDeserveDate = jbbo.deserveDate,
                        // bondIsRecieved = jbbo.isRecieved,
                        shippingCompanyId = C.shippingCompanyId,
                        //shippingCompanyName = jssh.name
                        isConfirm2 = 0,
                    }
                ).Where(C => ((type == "all") ? true : C.transType == type) && (C.processType != "balance") && ((side == "all") ? true : C.side == side)).ToList();

                if (cachlist.Count > 0 && side == "p")
                {
                    CashTransferModel tempitem = null;
                    foreach (CashTransferModel cashtItem in cachlist)
                    {
                        tempitem = this.Getpostransmodel(cashtItem.cashTransId)
                            .Where(C => C.cashTransId != cashtItem.cashTransId)
                            .FirstOrDefault();
                        cashtItem.cashTrans2Id = tempitem.cashTransId;
                        cashtItem.pos2Id = tempitem.posId;
                        cashtItem.pos2Name = tempitem.posName;
                        cashtItem.isConfirm2 = tempitem.isConfirm;
                    }
                }
                cachlist = cachlist
                    .Where(C => C.posId == posId || C.pos2Id == posId || C.posIdCreator == posId)
                    .ToList();

                return cachlist;
            }
        }

        // get by bondId
        [HttpPost]
        [Route("GetBybondId")]
        public string GetBybondId(string token)
        {
            //int bondId string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long bondId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "bondId")
                    {
                        bondId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<CashTransferModel> cachlist = (
                            from C in entity.cashTransfer
                            where C.bondId == bondId
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                shippingCompanyId = C.shippingCompanyId,
                            }
                        ).ToList();

                        return TokenManager.GenerateToken(cachlist);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
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
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {

            //        List<CashTransferModel> cachlist = (from C in entity.cashTransfer
            //                                            where C.bondId==bondId
            //                                            select new CashTransferModel()
            //                                            {
            //                                                cashTransId = C.cashTransId,
            //                                                transType = C.transType,
            //                                                posId = C.posId,
            //                                                userId = C.userId,
            //                                                agentId = C.agentId,
            //                                                invId = C.invId,
            //                                                transNum = C.transNum,
            //                                                createDate = C.createDate,
            //                                                updateDate = C.updateDate,
            //                                                cash = C.cash,
            //                                                updateUserId = C.updateUserId,
            //                                                createUserId = C.createUserId,
            //                                                notes = C.notes,
            //                                                posIdCreator = C.posIdCreator,
            //                                                isConfirm = C.isConfirm,
            //                                                cashTransIdSource = C.cashTransIdSource,
            //                                                side = C.side,

            //                                                docName = C.docName,
            //                                                docNum = C.docNum,
            //                                                docImage = C.docImage,
            //                                                bankId = C.bankId,

            //                                                processType = C.processType,
            //                                                cardId = C.cardId,
            //                                                bondId = C.bondId,
            //                                                shippingCompanyId = C.shippingCompanyId,
            //                                            }).ToList();





            //        if (cachlist == null)
            //            return NotFound();
            //        else
            //            return Ok(cachlist);

            //    }
            //}
            //else
            //    return NotFound();
            #endregion
        }

        // GET api/agent/5
        [HttpPost]
        [Route("GetByID")]
        public string GetByID(string token)
        {
            //string type, string side string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long cTId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransId")
                    {
                        cTId = long.Parse(c.Value);
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var cacht = entity.cashTransfer
                            .Where(C => C.cashTransId == cTId)
                            .Select(
                                C =>
                                    new CashTransferModel
                                    {
                                        cashTransId = C.cashTransId,
                                        transType = C.transType,
                                        posId = C.posId,
                                        userId = C.userId,
                                        agentId = C.agentId,
                                        invId = C.invId,
                                        transNum = C.transNum,
                                        createDate = C.createDate,
                                        updateDate = C.updateDate,
                                        cash = C.cash,
                                        updateUserId = C.updateUserId,
                                        createUserId = C.createUserId,
                                        notes = C.notes,
                                        posIdCreator = C.posIdCreator,
                                        isConfirm = C.isConfirm,
                                        cashTransIdSource = C.cashTransIdSource,
                                        side = C.side,
                                        docName = C.docName,
                                        docNum = C.docNum,
                                        docImage = C.docImage,
                                        bankId = C.bankId,
                                        processType = C.processType,
                                        cardId = C.cardId,
                                        bondId = C.bondId,
                                        shippingCompanyId = C.shippingCompanyId,
                                    }
                            )
                            .FirstOrDefault();

                        return TokenManager.GenerateToken(cacht);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
            //      var re = Request;
            //      var headers = re.Headers;
            //      string token = "";
            //      int cTId = 0;
            //      if (headers.Contains("APIKey"))
            //      {
            //          token = headers.GetValues("APIKey").First();
            //      }
            //      if (headers.Contains("cashTransId"))
            //      {
            //          cTId = Convert.ToInt32(headers.GetValues("cashTransId").First());
            //      }
            //      Validation validation = new Validation();
            //      bool valid = validation.CheckApiKey(token);

            //      if (valid)
            //      {
            //          using (incposdbEntities entity = new incposdbEntities())
            //          {

            //              var cacht = entity.cashTransfer

            //.Where(C => C.cashTransId == cTId)

            //  .Select(C => new CashTransferModel
            //  {
            //      cashTransId = C.cashTransId,
            //      transType = C.transType,
            //      posId = C.posId,
            //      userId = C.userId,
            //      agentId = C.agentId,
            //      invId = C.invId,
            //      transNum = C.transNum,
            //      createDate = C.createDate,
            //      updateDate = C.updateDate,
            //      cash = C.cash,
            //      updateUserId = C.updateUserId,
            //      createUserId = C.createUserId,
            //      notes = C.notes,
            //      posIdCreator = C.posIdCreator,
            //      isConfirm = C.isConfirm,
            //      cashTransIdSource = C.cashTransIdSource,
            //      side = C.side,

            //      docName = C.docName,
            //      docNum = C.docNum,
            //      docImage = C.docImage,
            //      bankId = C.bankId,
            //      processType = C.processType,
            //      cardId = C.cardId,
            //      bondId = C.bondId,
            //      shippingCompanyId=C.shippingCompanyId,

            //  }).FirstOrDefault();

            //              if (cacht == null)
            //                  return NotFound();
            //              else
            //                  return Ok(cacht);

            //          }
            //      }
            //      else
            //          return NotFound();
            #endregion
        }

        // GET api/Agent
        [HttpPost]
        [Route("Search")]
        public string Search(string token)
        {
            //string type, string side string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "";
                string side = "";
                string searchwords = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "type")
                    {
                        type = c.Value;
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                    else if (c.Type == "searchwords")
                    {
                        searchwords = c.Value;
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var cashList = entity.cashTransfer
                            .Where(
                                C =>
                                    ((type == "all") ? true : C.transType == type)
                                        && ((side == "all") ? true : C.side == side)
                                        && C.transNum.Contains(searchwords)
                                    || C.notes.Contains(searchwords)
                                    || C.docName.Contains(searchwords)
                                    || C.docNum.Contains(searchwords)
                            )
                            .Select(
                                C =>
                                    new
                                    {
                                        C.cashTransId,
                                        C.transType,
                                        C.posId,
                                        C.userId,
                                        C.agentId,
                                        C.invId,
                                        C.transNum,
                                        C.createDate,
                                        C.updateDate,
                                        C.cash,
                                        C.updateUserId,
                                        C.createUserId,
                                        C.notes,
                                        C.posIdCreator,
                                        C.isConfirm,
                                        C.cashTransIdSource,
                                        C.side,
                                        C.docName,
                                        C.docNum,
                                        C.docImage,
                                        C.bankId,
                                        C.processType,
                                        C.cardId,
                                        C.bondId,
                                        C.shippingCompanyId,
                                    }
                            )
                            .ToList();

                        return TokenManager.GenerateToken(cashList);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region
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
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var cashList = entity.cashTransfer
            //       .Where(C => ((type == "all") ? true : C.transType == type)
            //&& ((side == "all") ? true : C.side == side) &&
            // C.transNum.Contains(searchwords) || C.notes.Contains(searchwords)
            // || C.docName.Contains(searchwords) || C.docNum.Contains(searchwords))
            //       .Select(C => new
            //       {
            //           C.cashTransId,
            //           C.transType,
            //           C.posId,
            //           C.userId,
            //           C.agentId,
            //           C.invId,
            //           C.transNum,
            //           C.createDate,
            //           C.updateDate,
            //           C.cash,
            //           C.updateUserId,
            //           C.createUserId,
            //           C.notes,
            //           C.posIdCreator,
            //           C.isConfirm,
            //           C.cashTransIdSource,
            //           C.side,

            //           C.docName,
            //           C.docNum,
            //           C.docImage,
            //           C.bankId,
            //           C.processType,
            //           C.cardId,
            //           C.bondId,
            //           C.shippingCompanyId,
            //       })
            //       .ToList();

            //        if (cashList == null)
            //            return NotFound();
            //        else
            //            return Ok(cashList);

            //    }
            //}
            //else
            //    return NotFound();
            #endregion
        }

        // add or update agent
        [HttpPost]
        [Route("Save")]
        public async Task<string> Save(string token)
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
                cashTransfer newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
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
                        message = await addCashTransfer(newObject);

                        return TokenManager.GenerateToken(message);
                    }
                    catch (Exception ex)
                    {
                        return TokenManager.GenerateToken(ex.ToString());
                    }
                }
                message = "0";
                return TokenManager.GenerateToken(message);
            }

            #region old
            // return TokenManager.GenerateToken(message);


            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //Object = Object.Replace("\\", string.Empty);
            //Object = Object.Trim('"');

            //cashTransfer Obj = JsonConvert.DeserializeObject<cashTransfer>(Object, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //if (Obj.updateUserId == 0 || Obj.updateUserId == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.updateUserId = id;
            //}
            //if (Obj.createUserId == 0 || Obj.createUserId == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.createUserId = id;
            //}

            //if (Obj.agentId == 0 || Obj.agentId == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.agentId = id;
            //}
            //if (Obj.invId == 0 || Obj.invId == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.invId = id;
            //}
            //if (Obj.posIdCreator == 0 || Obj.posIdCreator == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.posIdCreator = id;
            //}

            //if (Obj.cashTransIdSource == 0 || Obj.cashTransIdSource == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.cashTransIdSource = id;
            //}
            //if (Obj.bankId == 0 || Obj.bankId == null)
            //{
            //    Nullable<long> id = null;
            //    Obj.bankId = id;
            //}

            //if (valid)
            //{
            //    try
            //    {
            //        cashTransfer cashtr;
            //        using (incposdbEntities entity = new incposdbEntities())
            //        {
            //            var cEntity = entity.Set<cashTransfer>();
            //            if (Obj.cashTransId == 0)
            //            {
            //                Obj.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Obj.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                Obj.updateUserId = Obj.createUserId;
            //                cashtr = cEntity.Add(Obj);
            //            }
            //            else
            //            {
            //                cashtr = entity.cashTransfer.Where(p => p.cashTransId == Obj.cashTransId).First();
            //                cashtr.transType = Obj.transType;
            //                cashtr.posId = Obj.posId;
            //                cashtr.userId = Obj.userId;
            //                cashtr.agentId = Obj.agentId;
            //                cashtr.invId = Obj.invId;
            //                cashtr.transNum = Obj.transNum;
            //                cashtr.createDate = Obj.createDate;
            //                cashtr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
            //                cashtr.cash = Obj.cash;
            //                cashtr.updateUserId = Obj.updateUserId;
            //                // cashtr.createUserId = Obj. ;
            //                cashtr.notes = Obj.notes;
            //                cashtr.posIdCreator = Obj.posIdCreator;
            //                cashtr.isConfirm = Obj.isConfirm;
            //                cashtr.cashTransIdSource = Obj.cashTransIdSource;
            //                cashtr.side = Obj.side;

            //                cashtr.docName = Obj.docName;
            //                cashtr.docNum = Obj.docNum;
            //                cashtr.docImage = Obj.docImage;
            //                cashtr.bankId = Obj.bankId;
            //                cashtr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
            //                cashtr.processType = Obj.processType;
            //                cashtr.cardId = Obj.cardId;
            //                cashtr.bondId = Obj.bondId;
            //                cashtr.shippingCompanyId = Obj.shippingCompanyId;

            //            }
            //            entity.SaveChanges();
            //        }
            //        return cashtr.cashTransId;
            //    }

            //    catch
            //    {
            //        return 0;
            //    }
            //}
            //else
            //    return 0;
            #endregion
        }

        ///
        [HttpPost]
        [Route("GetbySourcId")]
        public string GetbySourcId(string token)
        {
            //string type, string side string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long sourceId = 0;
                string side = "";

                string type = "all";
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "sourceId")
                    {
                        sourceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "side")
                    {
                        side = c.Value;
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var cachlist = (
                            from C in entity.cashTransfer
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join u in entity.users on C.userId equals u.userId into ju
                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                bankName = jbb.name,
                                agentName = jaa.name,
                                usersName = juu.username,
                                posName = jpp.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                shippingCompanyId = C.shippingCompanyId,
                                isConfirm2 = 0,
                            }
                        ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side) && (C.cashTransId == sourceId || C.cashTransIdSource == sourceId)).ToList();

                        // one row mean type=d
                        if (cachlist.Count == 1)
                        {
                            long? pullposcashtransid = cachlist.First().cashTransIdSource;

                            //
                            var cachadd = (
                                from C in entity.cashTransfer
                                join b in entity.banks on C.bankId equals b.bankId into jb
                                join a in entity.agents on C.agentId equals a.agentId into ja
                                join p in entity.pos on C.posId equals p.posId into jp
                                join u in entity.users on C.userId equals u.userId into ju
                                from jbb in jb.DefaultIfEmpty()
                                from jaa in ja.DefaultIfEmpty()
                                from jpp in jp.DefaultIfEmpty()
                                from juu in ju.DefaultIfEmpty()
                                select new CashTransferModel()
                                {
                                    cashTransId = C.cashTransId,
                                    transType = C.transType,
                                    posId = C.posId,
                                    userId = C.userId,
                                    agentId = C.agentId,
                                    invId = C.invId,
                                    transNum = C.transNum,
                                    createDate = C.createDate,
                                    updateDate = C.updateDate,
                                    cash = C.cash,
                                    updateUserId = C.updateUserId,
                                    createUserId = C.createUserId,
                                    notes = C.notes,
                                    posIdCreator = C.posIdCreator,
                                    isConfirm = C.isConfirm,
                                    cashTransIdSource = C.cashTransIdSource,
                                    side = C.side,
                                    docName = C.docName,
                                    docNum = C.docNum,
                                    docImage = C.docImage,
                                    bankId = C.bankId,
                                    bankName = jbb.name,
                                    agentName = jaa.name,
                                    usersName = juu.username,
                                    posName = jpp.name,
                                    processType = C.processType,
                                    cardId = C.cardId,
                                    bondId = C.bondId,
                                    isConfirm2 = 0,
                                }
                            ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side) && (C.cashTransId == pullposcashtransid)).ToList();

                            //

                            if (cachadd.Count > 0)
                            {
                                cachlist.AddRange(cachadd);
                            }
                        }

                        return TokenManager.GenerateToken(cachlist);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            #region old
            //string type = "all";

            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            ///*
            //string type = "";
            //string side = "";
            //*/
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

            //        var cachlist = (from C in entity.cashTransfer
            //                        join b in entity.banks on C.bankId equals b.bankId into jb
            //                        join a in entity.agents on C.agentId equals a.agentId into ja
            //                        join p in entity.pos on C.posId equals p.posId into jp
            //                        join u in entity.users on C.userId equals u.userId into ju
            //                        from jbb in jb.DefaultIfEmpty()
            //                        from jaa in ja.DefaultIfEmpty()
            //                        from jpp in jp.DefaultIfEmpty()
            //                        from juu in ju.DefaultIfEmpty()

            //                        select new CashTransferModel()
            //                        {
            //                            cashTransId = C.cashTransId,
            //                            transType = C.transType,
            //                            posId = C.posId,
            //                            userId = C.userId,
            //                            agentId = C.agentId,
            //                            invId = C.invId,
            //                            transNum = C.transNum,
            //                            createDate = C.createDate,
            //                            updateDate = C.updateDate,
            //                            cash = C.cash,
            //                            updateUserId = C.updateUserId,
            //                            createUserId = C.createUserId,
            //                            notes = C.notes,
            //                            posIdCreator = C.posIdCreator,
            //                            isConfirm = C.isConfirm,
            //                            cashTransIdSource = C.cashTransIdSource,
            //                            side = C.side,

            //                            docName = C.docName,
            //                            docNum = C.docNum,
            //                            docImage = C.docImage,
            //                            bankId = C.bankId,
            //                            bankName = jbb.name,
            //                            agentName = jaa.name,
            //                            usersName = juu.username,
            //                            posName = jpp.name,
            //                            processType = C.processType,
            //                            cardId = C.cardId,
            //                            bondId = C.bondId,
            //                            shippingCompanyId = C.shippingCompanyId,
            //                        }).Where(C => ((type == "all") ? true : C.transType == type)
            //                    && ((side == "all") ? true : C.side == side) && (C.cashTransId == sourceId || C.cashTransIdSource == sourceId)).ToList();


            //        // one row mean type=d
            //        if (cachlist.Count == 1)
            //        {
            //            int? pullposcashtransid = cachlist.First().cashTransIdSource;

            //            //
            //            var cachadd = (from C in entity.cashTransfer
            //                           join b in entity.banks on C.bankId equals b.bankId into jb
            //                           join a in entity.agents on C.agentId equals a.agentId into ja
            //                           join p in entity.pos on C.posId equals p.posId into jp
            //                           join u in entity.users on C.userId equals u.userId into ju
            //                           from jbb in jb.DefaultIfEmpty()
            //                           from jaa in ja.DefaultIfEmpty()
            //                           from jpp in jp.DefaultIfEmpty()
            //                           from juu in ju.DefaultIfEmpty()

            //                           select new CashTransferModel()
            //                           {
            //                               cashTransId = C.cashTransId,
            //                               transType = C.transType,
            //                               posId = C.posId,
            //                               userId = C.userId,
            //                               agentId = C.agentId,
            //                               invId = C.invId,
            //                               transNum = C.transNum,
            //                               createDate = C.createDate,
            //                               updateDate = C.updateDate,
            //                               cash = C.cash,
            //                               updateUserId = C.updateUserId,
            //                               createUserId = C.createUserId,
            //                               notes = C.notes,
            //                               posIdCreator = C.posIdCreator,
            //                               isConfirm = C.isConfirm,
            //                               cashTransIdSource = C.cashTransIdSource,
            //                               side = C.side,

            //                               docName = C.docName,
            //                               docNum = C.docNum,
            //                               docImage = C.docImage,
            //                               bankId = C.bankId,
            //                               bankName = jbb.name,
            //                               agentName = jaa.name,
            //                               usersName = juu.username,
            //                               posName = jpp.name,
            //                               processType = C.processType,
            //                               cardId = C.cardId,
            //                               bondId = C.bondId,
            //                           }).Where(C => ((type == "all") ? true : C.transType == type)
            //   && ((side == "all") ? true : C.side == side) && (C.cashTransId == pullposcashtransid)).ToList();

            //            //

            //            if (cachadd.Count > 0)
            //            {
            //                cachlist.AddRange(cachadd);

            //            }
            //        }

            //        if (cachlist == null)
            //            return NotFound();
            //        else
            //        {

            //            return Ok(cachlist);
            //        }
            //    }

            //}
            //else
            //    return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("Delete")]
        public string Delete(string token)
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
                long cashTransId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransId")
                    {
                        cashTransId = long.Parse(c.Value);
                    }
                }

                List<CashTransferModel> delList = null;
                List<CashTransferModel> allList = null;
                cashTransfer cashobject = new cashTransfer();

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        allList = this.Getpostransmodel(cashTransId).ToList();
                        delList = allList.Where(C => C.isConfirm == 1).ToList();
                        if (delList != null)
                        {
                            if (delList.Count == 2)
                            {
                                message = "0";
                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                foreach (CashTransferModel ctitem in allList)
                                {
                                    cashobject = entity.cashTransfer
                                        .Where(C => C.cashTransId == ctitem.cashTransId)
                                        .FirstOrDefault();
                                    entity.cashTransfer.Remove(cashobject);
                                }
                                long res = entity.SaveChanges();
                                if (res > 0)
                                {
                                    message = "1";
                                }
                            }
                        }

                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
            #region old
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //List<CashTransferModel> delList = null;
            //List<CashTransferModel> allList = null;
            //cashTransfer cashobject = new cashTransfer();
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
            //            allList = this.Getpostransmodel(cashTransId).ToList();
            //            delList = allList.Where(C => C.isConfirm == 1).ToList();
            //            if (delList != null)
            //            {
            //                if (delList.Count == 2)
            //                {

            //                    return Ok("0");
            //                }
            //                else
            //                {

            //                    foreach (CashTransferModel ctitem in allList)
            //                    {
            //                        cashobject = entity.cashTransfer.Where(C => C.cashTransId == ctitem.cashTransId).FirstOrDefault();
            //                        entity.cashTransfer.Remove(cashobject);

            //                    }
            //                    entity.SaveChanges();


            //                }

            //            }






            //            return Ok("1");
            //        }
            //    }
            //    catch
            //    {
            //        return NotFound();
            //    }


            //}
            //else
            //    return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("Confirm")]
        public async Task<string> Confirm(string token)
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
                cashTransfer newObject = null;
                CashTransferModel cashTransferModel = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        cashTransferModel = JsonConvert.DeserializeObject<CashTransferModel>(
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
                        var allList = this.Getpostransmodel(newObject.cashTransId).ToList();
                        var delList = allList.Where(C => C.isConfirm == 1).ToList();
                        if (delList != null)
                        {
                            if (delList.Count == 2)
                            {
                                message = "-2";
                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                int res = await Confirm(newObject, cashTransferModel);
                                return TokenManager.GenerateToken(res.ToString());
                            }
                        }
                    }
                    catch
                    {
                        message = "-1";
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("MovePosCash")]
        public string MovePosCash(string token)
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
                long cashTransId = 0;
                long userIdD = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransId")
                    {
                        cashTransId = long.Parse(c.Value);
                    }
                    else if (c.Type == "userIdD")
                    {
                        userIdD = long.Parse(c.Value);
                    }
                }

                List<CashTransferModel> tempList = null;
                List<CashTransferModel> allList = null;
                CashTransferModel cashobject = new CashTransferModel();
                cashTransfer ctObject = new cashTransfer();

                pos posobject = new pos();
                pos posobjectD = new pos();
                long? posidPull = 0;
                long? posidD = 0;
                decimal? cash = 0;

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        allList = Getpostransmodel(cashTransId).ToList();
                        if (allList.Count > 0)
                        {
                            //check if first pos is confirm
                            tempList = allList
                                .Where(C => C.transType == "p" && C.isConfirm == 1)
                                .ToList();

                            if (tempList != null)
                            {
                                if (tempList.Count >= 1)
                                {
                                    cashobject = tempList.FirstOrDefault();
                                    cash = cashobject.cash;
                                    posidPull = cashobject.posId;
                                    posobject = entity.pos
                                        .Where(p => p.posId == posidPull)
                                        .FirstOrDefault();
                                    if (cashobject.cash <= posobject.balance)
                                    {
                                        //in "d" set confirm to 1
                                        //get row of type d
                                        cashobject = allList
                                            .Where(C => C.transType == "d")
                                            .FirstOrDefault();
                                        ctObject = entity.cashTransfer
                                            .Where(C => C.cashTransId == cashobject.cashTransId)
                                            .FirstOrDefault();
                                        ctObject.isConfirm = 1;
                                        ctObject.updateUserId = userIdD;
                                        ctObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        ctObject.userId = userIdD;
                                        // END in "d" set confirm to 1

                                        //START decreas balance from pull pos
                                        posidD = ctObject.posId;
                                        posobject = entity.pos
                                            .Where(p => p.posId == posidPull)
                                            .FirstOrDefault();

                                        posobject.balance =
                                            (decimal)posobject.balance - (decimal)cash;
                                        posobject.updateUserId = userIdD;
                                        posobject.updateDate = coctrlr.AddOffsetTodate(
                                            DateTime.Now
                                        );
                                        // end
                                        //increase balance from d pos
                                        posobjectD = entity.pos
                                            .Where(p => p.posId == posidD)
                                            .FirstOrDefault();
                                        if (posobjectD.balance == null)
                                        {
                                            posobjectD.balance = 0;
                                        }
                                        posobjectD.balance =
                                            (decimal)posobjectD.balance + (decimal)cash;
                                        posobjectD.updateUserId = userIdD;
                                        posobjectD.updateDate = coctrlr.AddOffsetTodate(
                                            DateTime.Now
                                        );
                                        entity.SaveChanges();
                                        // return Ok("transdone");
                                        return TokenManager.GenerateToken("1");
                                    }
                                    else
                                    {
                                        //  return Ok("nobalanceinpullpos");
                                        return TokenManager.GenerateToken("2");
                                    }
                                }
                                else
                                {
                                    //return Ok("pullposnotconfirmed");
                                    return TokenManager.GenerateToken("3");
                                }
                            }
                            else
                            {
                                //  return Ok("nopullidornotconfirmed");
                                return TokenManager.GenerateToken("4");
                            }
                        }
                        else
                        {
                            //  return Ok("idnotfound");
                            return TokenManager.GenerateToken("5");
                        }
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //List<CashTransferModel> tempList = null;
            //List<CashTransferModel> allList = null;
            //CashTransferModel cashobject = new CashTransferModel();
            //cashTransfer ctObject = new cashTransfer();

            //pos posobject = new pos();
            //pos posobjectD = new pos();
            //int? posidPull = 0;
            //int? posidD = 0;
            //decimal? cash = 0;

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
            //            allList = this.Getpostransmodel(cashTransId).ToList();
            //            if (allList.Count > 0)
            //            {
            //                //check if first pos is confirm
            //                tempList = allList.Where(C => C.transType == "p" && C.isConfirm == 1).ToList();

            //                if (tempList != null)
            //                {
            //                    if (tempList.Count >= 1)
            //                    {
            //                        cashobject = tempList.FirstOrDefault();
            //                        cash = cashobject.cash;
            //                        posidPull = cashobject.posId;
            //                        posobject = entity.pos.Where(p => p.posId == posidPull).FirstOrDefault();
            //                        if (cashobject.cash <= posobject.balance)
            //                        {
            //                            //in "d" set confirm to 1
            //                            //get row of type d
            //                            cashobject = allList.Where(C => C.transType == "d").FirstOrDefault();
            //                            ctObject = entity.cashTransfer.Where(C => C.cashTransId == cashobject.cashTransId).FirstOrDefault();
            //                            ctObject.isConfirm = 1;
            //                            ctObject.updateUserId = userIdD;
            //                            ctObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                            ctObject.userId = userIdD;
            //                            // END in "d" set confirm to 1

            //                            //START decreas balance from pull pos
            //                            posidD = ctObject.posId;
            //                            posobject = entity.pos.Where(p => p.posId == posidPull).FirstOrDefault();

            //                            posobject.balance = posobject.balance - cash;
            //                            posobject.updateUserId = userIdD;
            //                            posobject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                            // end
            //                            //increase balance from d pos
            //                            posobjectD = entity.pos.Where(p => p.posId == posidD).FirstOrDefault();
            //                            if (posobjectD.balance == null)
            //                            {
            //                                posobjectD.balance = 0;
            //                            }
            //                            posobjectD.balance = posobjectD.balance + cash;
            //                            posobjectD.updateUserId = userIdD;
            //                            posobjectD.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                            entity.SaveChanges();
            //                            return Ok("transdone");
            //                        }
            //                        else
            //                        {
            //                            return Ok("nobalanceinpullpos");
            //                        }


            //                    }
            //                    else
            //                    {
            //                        return Ok("pullposnotconfirmed");
            //                    }
            //                }
            //                else
            //                {
            //                    return Ok("nopullidornotconfirmed");
            //                }
            //            }
            //            else
            //            {
            //                return Ok("idnotfound");
            //            }

            //        }
            //    }
            //    catch
            //    {
            //        return NotFound();
            //    }


            //}
            //else
            //    return NotFound();
            #endregion
        }

        public List<CashTransferModel> Getpostransmodel(long cashTransId)
        {
            string side = "p";
            string type = "all";
            using (incposdbEntities entity = new incposdbEntities())
            {
                var cachlist = (
                    from C in entity.cashTransfer
                    join b in entity.banks on C.bankId equals b.bankId into jb
                    join a in entity.agents on C.agentId equals a.agentId into ja
                    join p in entity.pos on C.posId equals p.posId into jp
                    join u in entity.users on C.userId equals u.userId into ju
                    from jbb in jb.DefaultIfEmpty()
                    from jaa in ja.DefaultIfEmpty()
                    from jpp in jp.DefaultIfEmpty()
                    from juu in ju.DefaultIfEmpty()
                    select new CashTransferModel()
                    {
                        cashTransId = C.cashTransId,
                        transType = C.transType,
                        posId = C.posId,
                        userId = C.userId,
                        agentId = C.agentId,
                        invId = C.invId,
                        transNum = C.transNum,
                        createDate = C.createDate,
                        updateDate = C.updateDate,
                        cash = C.cash,
                        updateUserId = C.updateUserId,
                        createUserId = C.createUserId,
                        notes = C.notes,
                        posIdCreator = C.posIdCreator,
                        isConfirm = C.isConfirm,
                        cashTransIdSource = C.cashTransIdSource,
                        side = C.side,
                        docName = C.docName,
                        docNum = C.docNum,
                        docImage = C.docImage,
                        bankId = C.bankId,
                        bankName = jbb.name,
                        agentName = jaa.name,
                        usersName = juu.username,
                        posName = jpp.name,
                        processType = C.processType,
                        cardId = C.cardId,
                        bondId = C.bondId,
                        shippingCompanyId = C.shippingCompanyId,
                    }
                ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side) && (C.cashTransId == cashTransId || C.cashTransIdSource == cashTransId)).ToList();

                // one row mean type=d
                if (cachlist.Count == 1)
                {
                    long? pullposcashtransid = cachlist.First().cashTransIdSource;

                    //
                    var cachadd = (
                        from C in entity.cashTransfer
                        join b in entity.banks on C.bankId equals b.bankId into jb
                        join a in entity.agents on C.agentId equals a.agentId into ja
                        join p in entity.pos on C.posId equals p.posId into jp
                        join u in entity.users on C.userId equals u.userId into ju
                        from jbb in jb.DefaultIfEmpty()
                        from jaa in ja.DefaultIfEmpty()
                        from jpp in jp.DefaultIfEmpty()
                        from juu in ju.DefaultIfEmpty()
                        select new CashTransferModel()
                        {
                            cashTransId = C.cashTransId,
                            transType = C.transType,
                            posId = C.posId,
                            userId = C.userId,
                            agentId = C.agentId,
                            invId = C.invId,
                            transNum = C.transNum,
                            createDate = C.createDate,
                            updateDate = C.updateDate,
                            cash = C.cash,
                            updateUserId = C.updateUserId,
                            createUserId = C.createUserId,
                            notes = C.notes,
                            posIdCreator = C.posIdCreator,
                            isConfirm = C.isConfirm,
                            cashTransIdSource = C.cashTransIdSource,
                            side = C.side,
                            docName = C.docName,
                            docNum = C.docNum,
                            docImage = C.docImage,
                            bankId = C.bankId,
                            bankName = jbb.name,
                            agentName = jaa.name,
                            usersName = juu.username,
                            posName = jpp.name,
                            processType = C.processType,
                            cardId = C.cardId,
                            bondId = C.bondId,
                            shippingCompanyId = C.shippingCompanyId,
                        }
                    ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side) && (C.cashTransId == pullposcashtransid)).ToList();

                    //

                    if (cachadd.Count > 0)
                    {
                        cachlist.AddRange(cachadd);
                    }
                }

                return cachlist;
            }
        }

        [HttpPost]
        [Route("GetByInvId")]
        public string GetByInvId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string type = "all";
                string side = "all";
                long invId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invId")
                    {
                        invId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        CashTransferModel cachtrans = new CashTransferModel();
                        cachtrans = (
                            from C in entity.cashTransfer
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                bankName = jbb.name,
                                agentName = jaa.name,
                                usersName = juu.username,
                                posName = jpp.name,
                                posCreatorName = jpcc.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                shippingCompanyId = C.shippingCompanyId,
                            }
                        ).Where(C => ((type == "all") ? true : C.transType == type) && ((side == "all") ? true : C.side == side) && C.invId == invId && C.processType != "inv").FirstOrDefault();

                        return TokenManager.GenerateToken(cachtrans);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //string type = "all";
            //string side = "all";
            ///*
            //string type = "";
            //string side = "";
            //*/
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

            //        CashTransferModel cachtrans = (from C in entity.cashTransfer
            //                                       join b in entity.banks on C.bankId equals b.bankId into jb
            //                                       join a in entity.agents on C.agentId equals a.agentId into ja
            //                                       join p in entity.pos on C.posId equals p.posId into jp
            //                                       join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
            //                                       join u in entity.users on C.userId equals u.userId into ju
            //                                       from jbb in jb.DefaultIfEmpty()
            //                                       from jaa in ja.DefaultIfEmpty()
            //                                       from jpp in jp.DefaultIfEmpty()
            //                                       from juu in ju.DefaultIfEmpty()
            //                                       from jpcc in jpcr.DefaultIfEmpty()
            //                                       select new CashTransferModel()
            //                                       {
            //                                           cashTransId = C.cashTransId,
            //                                           transType = C.transType,
            //                                           posId = C.posId,
            //                                           userId = C.userId,
            //                                           agentId = C.agentId,
            //                                           invId = C.invId,
            //                                           transNum = C.transNum,
            //                                           createDate = C.createDate,
            //                                           updateDate = C.updateDate,
            //                                           cash = C.cash,
            //                                           updateUserId = C.updateUserId,
            //                                           createUserId = C.createUserId,
            //                                           notes = C.notes,
            //                                           posIdCreator = C.posIdCreator,
            //                                           isConfirm = C.isConfirm,
            //                                           cashTransIdSource = C.cashTransIdSource,
            //                                           side = C.side,

            //                                           docName = C.docName,
            //                                           docNum = C.docNum,
            //                                           docImage = C.docImage,
            //                                           bankId = C.bankId,
            //                                           bankName = jbb.name,
            //                                           agentName = jaa.name,
            //                                           usersName = juu.username,
            //                                           posName = jpp.name,
            //                                           posCreatorName = jpcc.name,
            //                                           processType = C.processType,
            //                                           cardId = C.cardId,
            //                                           bondId = C.bondId,
            //                                           shippingCompanyId = C.shippingCompanyId,
            //                                       }).Where(C => ((type == "all") ? true : C.transType == type)
            //                                                              && ((side == "all") ? true : C.side == side) && C.invId == invId && C.processType != "inv").FirstOrDefault();




            //        if (cachtrans == null)
            //            return NotFound();
            //        else
            //            return Ok(cachtrans);

            //    }
            //}
            //else
            //    return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("GetListByInvId")]
        public string GetListByInvId(string token)
        {
            //  string token

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invId")
                    {
                        invId = long.Parse(c.Value);
                    }
                }

                // DateTime cmpdate = DateTime.Now.AddDays(newdays);
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        List<CashTransferModel> cachtrans = new List<CashTransferModel>();
                        cachtrans = (
                            from C in entity.cashTransfer
                            join b in entity.banks on C.bankId equals b.bankId into jb
                            join a in entity.agents on C.agentId equals a.agentId into ja
                            join p in entity.pos on C.posId equals p.posId into jp
                            join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
                            join u in entity.users on C.userId equals u.userId into ju
                            from jbb in jb.DefaultIfEmpty()
                            from jaa in ja.DefaultIfEmpty()
                            from jpp in jp.DefaultIfEmpty()
                            from juu in ju.DefaultIfEmpty()
                            from jpcc in jpcr.DefaultIfEmpty()
                            select new CashTransferModel()
                            {
                                cashTransId = C.cashTransId,
                                transType = C.transType,
                                posId = C.posId,
                                userId = C.userId,
                                agentId = C.agentId,
                                invId = C.invId,
                                transNum = C.transNum,
                                createDate = C.createDate,
                                updateDate = C.updateDate,
                                cash = C.cash,
                                updateUserId = C.updateUserId,
                                createUserId = C.createUserId,
                                notes = C.notes,
                                posIdCreator = C.posIdCreator,
                                isConfirm = C.isConfirm,
                                cashTransIdSource = C.cashTransIdSource,
                                side = C.side,
                                docName = C.docName,
                                docNum = C.docNum,
                                docImage = C.docImage,
                                bankId = C.bankId,
                                bankName = jbb.name,
                                agentName = jaa.name,
                                usersName = juu.username,
                                posName = jpp.name,
                                posCreatorName = jpcc.name,
                                processType = C.processType,
                                cardId = C.cardId,
                                bondId = C.bondId,
                                shippingCompanyId = C.shippingCompanyId,
                            }
                        ).Where(C => C.invId == invId && C.processType != "inv").ToList();

                        return TokenManager.GenerateToken(cachtrans);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
            #region old
            //var re = Request;
            //var headers = re.Headers;
            //string token = "";

            ///*
            //string type = "";
            //string side = "";
            //*/
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

            //        List<CashTransferModel> cachtrans = (from C in entity.cashTransfer
            //                                             join b in entity.banks on C.bankId equals b.bankId into jb
            //                                             join a in entity.agents on C.agentId equals a.agentId into ja
            //                                             join p in entity.pos on C.posId equals p.posId into jp
            //                                             join pc in entity.pos on C.posIdCreator equals pc.posId into jpcr
            //                                             join u in entity.users on C.userId equals u.userId into ju
            //                                             from jbb in jb.DefaultIfEmpty()
            //                                             from jaa in ja.DefaultIfEmpty()
            //                                             from jpp in jp.DefaultIfEmpty()
            //                                             from juu in ju.DefaultIfEmpty()
            //                                             from jpcc in jpcr.DefaultIfEmpty()

            //                                             select new CashTransferModel()
            //                                             {
            //                                                 cashTransId = C.cashTransId,
            //                                                 transType = C.transType,
            //                                                 posId = C.posId,
            //                                                 userId = C.userId,
            //                                                 agentId = C.agentId,
            //                                                 invId = C.invId,
            //                                                 transNum = C.transNum,
            //                                                 createDate = C.createDate,
            //                                                 updateDate = C.updateDate,
            //                                                 cash = C.cash,
            //                                                 updateUserId = C.updateUserId,
            //                                                 createUserId = C.createUserId,
            //                                                 notes = C.notes,
            //                                                 posIdCreator = C.posIdCreator,
            //                                                 isConfirm = C.isConfirm,
            //                                                 cashTransIdSource = C.cashTransIdSource,
            //                                                 side = C.side,

            //                                                 docName = C.docName,
            //                                                 docNum = C.docNum,
            //                                                 docImage = C.docImage,
            //                                                 bankId = C.bankId,
            //                                                 bankName = jbb.name,
            //                                                 agentName = jaa.name,
            //                                                 usersName = juu.username,
            //                                                 posName = jpp.name,
            //                                                 posCreatorName = jpcc.name,
            //                                                 processType = C.processType,
            //                                                 cardId = C.cardId,
            //                                                 bondId = C.bondId,
            //                                                 shippingCompanyId = C.shippingCompanyId,
            //                                             }).Where(C => C.invId == invId && C.processType != "inv").ToList();




            //        if (cachtrans == null)
            //            return NotFound();
            //        else
            //            return Ok(cachtrans);

            //    }
            //}
            //else
            //    return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("GetCountByInvId")]
        public string GetCountByInvId(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long invId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoiceId")
                    {
                        invId = long.Parse(c.Value);
                    }
                }
                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        long cachtrans = entity.cashTransfer
                            .Where(C => C.invId == invId && C.processType != "inv")
                            .ToList()
                            .Count();
                        return TokenManager.GenerateToken(cachtrans);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [NonAction]
        public int GetCountByInvId(long invId)
        {
            InvoicesController ic = new InvoicesController();
            long invoiceId = invId;

            using (incposdbEntities entity = new incposdbEntities())
            {
                var invoice = entity.invoices
                    .Where(x => x.invoiceId == invId)
                    .Select(
                        x =>
                            new InvoiceModel()
                            {
                                invoiceId = x.invoiceId,
                                invoiceMainId = x.invoiceMainId,
                                invType = x.invType,
                            }
                    )
                    .FirstOrDefault();
                if (invoice.invType.Equals("s") || invoice.invType.Equals("p"))
                {
                    while (invoice != null)
                    {
                        invoiceId = invoice.invoiceId;

                        if (invoice.invoiceMainId != null)
                            invoice = ic.GetParentInv((long)invoice.invoiceId);
                        else
                            break;
                    }
                }
                int cachtrans = entity.cashTransfer
                    .Where(
                        C =>
                            C.invId == invoiceId
                            && C.processType != "balance"
                            && !hiddenCashes.Any(str => str.Contains(C.processType))
                    )
                    .ToList()
                    .Count();
                return cachtrans;
            }
        }

        [HttpPost]
        [Route("payByAmount")]
        public async Task<string> payByAmount(string token)
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
                // bondes newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                long agentId = 0;
                decimal amount = 0;
                string payType = "";
                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "agentId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "payType")
                    {
                        payType = c.Value;
                    }
                }
                if (cashTr != null)
                {
                    try
                    {
                        List<string> typesList = new List<string>();
                        string cashIds = "";
                        switch (payType)
                        {
                            case "pay": //get pw,pi,sb invoices

                                typesList.Add("pw");
                                typesList.Add("p");
                                typesList.Add("sb");
                                break;
                            case "feed": //get si, pb

                                typesList.Add("pb");
                                typesList.Add("s");
                                typesList.Add("ts");
                                typesList.Add("ss");
                                break;
                        }
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var invList = (
                                from b in entity.invoices.Where(
                                    x =>
                                        x.agentId == agentId
                                        && typesList.Contains(x.invType)
                                        && x.isActive == true
                                        && x.deserved > 0
                                        && (
                                            (
                                                x.shippingCompanyId == null
                                                && x.shipUserId == null
                                                && x.agentId != null
                                            )
                                            || (
                                                x.shippingCompanyId != null
                                                && x.shipUserId != null
                                                && x.agentId != null
                                            )
                                            || x.shippingCompanyId != null
                                                && x.shipUserId == null
                                                && x.agentId != null
                                                && x.isPrePaid == 1
                                        )
                                )
                                select new InvoiceModel()
                                {
                                    invoiceId = b.invoiceId,
                                    invNumber = b.invNumber,
                                    agentId = b.agentId,
                                    invType = b.invType,
                                    total = b.total,
                                    totalNet = b.totalNet,
                                    paid = b.paid,
                                    deserved = b.deserved,
                                    deservedDate = b.deservedDate,
                                    invDate = b.invDate,
                                    invoiceMainId = b.invoiceMainId,
                                    invCase = b.invCase,
                                    invTime = b.invTime,
                                    notes = b.notes,
                                    vendorInvNum = b.vendorInvNum,
                                    vendorInvDate = b.vendorInvDate,
                                    createUserId = b.createUserId,
                                    updateDate = b.updateDate,
                                    updateUserId = b.updateUserId,
                                    branchId = b.branchId,
                                    discountValue = b.discountValue,
                                    discountType = b.discountType,
                                    tax = b.tax,
                                    taxtype = b.taxtype,
                                    name = b.name,
                                    isApproved = b.isApproved,
                                    branchCreatorId = b.branchCreatorId,
                                    shippingCompanyId = b.shippingCompanyId,
                                    shipUserId = b.shipUserId,
                                }
                            ).ToList().OrderBy(b => b.deservedDate);

                            List<InvoiceModel> res = new List<InvoiceModel>();
                            agents agent;
                            //get only with rc status
                            if (payType == "feed")
                            {
                                foreach (InvoiceModel inv in invList)
                                {
                                    long invoiceId = inv.invoiceId;

                                    var statusObj = entity.orderPreparingStatus
                                        .Where(
                                            x =>
                                                x.orderPreparing.invoiceId == invoiceId
                                                && x.status == "Done"
                                        )
                                        .FirstOrDefault();

                                    if (statusObj != null)
                                    {
                                        int itemCount = entity.itemsTransfer
                                            .Where(x => x.invoiceId == invoiceId)
                                            .Select(x => x.itemsTransId)
                                            .ToList()
                                            .Count;
                                        inv.itemsCount = itemCount;
                                        res.Add(inv);
                                    }
                                }
                            }
                            else
                            {
                                res.AddRange(invList);
                            }

                            if (res.ToList().Count > 0)
                            {
                                switch (payType)
                                {
                                    #region payments
                                    case "pay": //get pw,p,sb invoices

                                        foreach (InvoiceModel inv in res)
                                        {
                                            decimal paid = 0;
                                            agent = entity.agents.Find(agentId);
                                            decimal agentBalance = (decimal)agent.balance;
                                            var invObj = entity.invoices.Find(inv.invoiceId);
                                            cashTr.invId = inv.invoiceId;
                                            if (amount >= inv.deserved)
                                            {
                                                paid = (decimal)inv.deserved;
                                                invObj.paid += inv.deserved;
                                                invObj.deserved = 0;
                                                amount -= (decimal)inv.deserved;
                                            }
                                            else
                                            {
                                                paid = amount;
                                                invObj.paid = invObj.paid + amount;
                                                invObj.deserved -= amount;
                                                amount = 0;
                                            }
                                            cashTr.cash = paid;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            cashTr.transNum = await generateCashNumber(
                                                cashTr.transNum
                                            );
                                            entity.cashTransfer.Add(cashTr);

                                            // increase agent balance
                                            if (agent.balanceType == 0)
                                            {
                                                if (paid <= (decimal)agent.balance)
                                                {
                                                    agent.balance = agentBalance - paid;
                                                }
                                                else
                                                {
                                                    agent.balance = paid - agentBalance;
                                                    agent.balanceType = 1;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance = agentBalance + paid;
                                            }

                                            entity.SaveChanges();

                                            if (amount == 0)
                                                break;
                                        }
                                        if (amount > 0) // save remain amount
                                        {
                                            agent = entity.agents.Find(agentId);
                                            decimal agentBalance = (decimal)agent.balance;
                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            cashTr.transNum = await generateCashNumber(
                                                cashTr.transNum
                                            );
                                            entity.cashTransfer.Add(cashTr);

                                            // increase agent balance
                                            if (agent.balanceType == 0)
                                            {
                                                if (amount <= (decimal)agent.balance)
                                                {
                                                    agent.balance = agentBalance - amount;
                                                }
                                                else
                                                {
                                                    agent.balance = amount - agentBalance;
                                                    agent.balanceType = 1;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance = agentBalance + amount;
                                            }
                                            entity.SaveChanges();
                                        }
                                        break;
                                    #endregion
                                    #region feed
                                    case "feed": //get s, pb
                                        foreach (InvoiceModel inv in res)
                                        {
                                            agent = entity.agents.Find(agentId);

                                            decimal paid = 0;
                                            var invObj = entity.invoices.Find(inv.invoiceId);
                                            cashTr.invId = inv.invoiceId;
                                            if (amount >= inv.deserved)
                                            {
                                                paid = (decimal)inv.deserved;
                                                invObj.paid = invObj.paid + inv.deserved;
                                                invObj.deserved = 0;
                                                amount -= (decimal)inv.deserved;
                                            }
                                            else
                                            {
                                                paid = amount;
                                                invObj.paid = invObj.paid + amount;
                                                invObj.deserved -= amount;
                                                amount = 0;
                                            }
                                            cashTr.cash = paid;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            cashTr.transNum = await generateCashNumber(
                                                cashTr.transNum
                                            );
                                            cashTr = entity.cashTransfer.Add(cashTr);

                                            // decrease agent balance
                                            if (agent.balanceType == 1)
                                            {
                                                if (paid <= (decimal)agent.balance)
                                                {
                                                    agent.balance -= paid;
                                                }
                                                else
                                                {
                                                    agent.balance = paid - agent.balance;
                                                    agent.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance += paid;
                                            }

                                            entity.SaveChanges();

                                            if (cashTr.processType == "card")
                                                AddCardCommission(cashTr);

                                            if (amount == 0)
                                                break;
                                        }
                                        if (amount > 0) // save remain amount
                                        {
                                            agent = entity.agents.Find(agentId);

                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // decrease agent balance
                                            if (agent.balanceType == 1)
                                            {
                                                if (amount <= (decimal)agent.balance)
                                                {
                                                    agent.balance = agent.balance - amount;
                                                }
                                                else
                                                {
                                                    agent.balance = amount - agent.balance;
                                                    agent.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance += amount;
                                            }

                                            entity.SaveChanges();
                                        }
                                        break;
                                    #endregion
                                }
                                //return Ok(cashIds);
                                return TokenManager.GenerateToken("1");
                            }
                            else
                            {
                                if (amount > 0) // save remain amount
                                {
                                    switch (payType)
                                    {
                                        case "pay":
                                            agent = entity.agents.Find(agentId);
                                            decimal agentBalance = (decimal)agent.balance;
                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // increase agent balance
                                            if (agent.balanceType == 0)
                                            {
                                                if (amount <= (decimal)agent.balance)
                                                {
                                                    agent.balance = agentBalance - amount;
                                                }
                                                else
                                                {
                                                    agent.balance = amount - agentBalance;
                                                    agent.balanceType = 1;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance = agentBalance + amount;
                                            }
                                            entity.SaveChanges();
                                            break;
                                        case "feed":
                                            agent = entity.agents.Find(agentId);

                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // decrease agent balance
                                            if (agent.balanceType == 1)
                                            {
                                                if (amount <= (decimal)agent.balance)
                                                {
                                                    agent.balance = agent.balance - amount;
                                                }
                                                else
                                                {
                                                    agent.balance = amount - agent.balance;
                                                    agent.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                agent.balance += amount;
                                            }

                                            entity.SaveChanges();
                                            break;
                                    }
                                }
                                return TokenManager.GenerateToken("1");
                            }
                        }
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }

                return TokenManager.GenerateToken("0");
            }
        }

        //

        /// </summary>
        /// <param name="shippingCompanyId"></param>
        /// <param name="amount"></param>
        /// <param name="payType">{feed}</param>
        /// <param name="cashTransfer"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        [Route("payShippingCompanyByAmount")]
        public async Task<string> payShippingCompanyByAmount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string Object = "";
                // bondes newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                decimal amount = 0;
                long shippingCompanyId = 0;

                string payType = "";
                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "payType")
                    {
                        payType = c.Value;
                    }
                }
                if (cashTr != null)
                {
                    try
                    {
                        List<string> typesList = new List<string>();
                        string cashIds = "";
                        switch (payType)
                        {
                            case "feed": //get si, pb

                                typesList.Add("pb");
                                typesList.Add("s");
                                typesList.Add("ts");
                                typesList.Add("ss");
                                break;
                        }
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            //var invList = (from b in entity.invoices.Where(x => x.shippingCompanyId == shippingCompanyId && typesList.Contains(x.invType) && x.deserved > 0)
                            var invList = (
                                from b in entity.invoices.Where(
                                    x =>
                                        x.shippingCompanyId == shippingCompanyId
                                        && typesList.Contains(x.invType)
                                        && x.deserved > 0
                                        && x.shippingCompanyId != null
                                        && x.shipUserId == null
                                        && x.agentId != null
                                )
                                select new InvoiceModel()
                                {
                                    invoiceId = b.invoiceId,
                                    invNumber = b.invNumber,
                                    agentId = b.agentId,
                                    invType = b.invType,
                                    total = b.total,
                                    totalNet = b.totalNet,
                                    paid = b.paid,
                                    deserved = b.deserved,
                                    deservedDate = b.deservedDate,
                                    invDate = b.invDate,
                                    invoiceMainId = b.invoiceMainId,
                                    invCase = b.invCase,
                                    invTime = b.invTime,
                                    notes = b.notes,
                                    vendorInvNum = b.vendorInvNum,
                                    vendorInvDate = b.vendorInvDate,
                                    createUserId = b.createUserId,
                                    updateDate = b.updateDate,
                                    updateUserId = b.updateUserId,
                                    branchId = b.branchId,
                                    discountValue = b.discountValue,
                                    discountType = b.discountType,
                                    tax = b.tax,
                                    taxtype = b.taxtype,
                                    name = b.name,
                                    isApproved = b.isApproved,
                                    branchCreatorId = b.branchCreatorId,
                                    shippingCompanyId = b.shippingCompanyId,
                                    shipUserId = b.shipUserId,
                                    userId = b.userId
                                }
                            ).ToList().OrderBy(b => b.deservedDate);
                            List<InvoiceModel> res = new List<InvoiceModel>();
                            cashTransfer ct;
                            shippingCompanies shippingCompany;
                            //get only with rc status
                            if (payType == "feed")
                            {
                                foreach (InvoiceModel inv in invList)
                                {
                                    long invoiceId = inv.invoiceId;

                                    var statusObj = entity.orderPreparingStatus
                                        .Where(
                                            x =>
                                                x.orderPreparing.invoiceId == invoiceId
                                                && x.status == "Done"
                                        )
                                        .FirstOrDefault();

                                    if (statusObj != null)
                                    {
                                        int itemCount = entity.itemsTransfer
                                            .Where(x => x.invoiceId == invoiceId)
                                            .Select(x => x.itemsTransId)
                                            .ToList()
                                            .Count;
                                        inv.itemsCount = itemCount;
                                        res.Add(inv);
                                    }
                                }
                            }
                            else
                            {
                                res.AddRange(invList);
                            }

                            if (res.ToList().Count > 0)
                            {
                                switch (payType)
                                {
                                    case "feed": //get s, pb
                                        foreach (InvoiceModel inv in res)
                                        {
                                            shippingCompany = entity.shippingCompanies.Find(
                                                shippingCompanyId
                                            );

                                            decimal paid = 0;
                                            var invObj = entity.invoices.Find(inv.invoiceId);
                                            cashTr.invId = inv.invoiceId;
                                            if (amount >= inv.deserved)
                                            {
                                                paid = (decimal)inv.deserved;
                                                invObj.paid =
                                                    (decimal)invObj.paid + (decimal)inv.deserved;
                                                invObj.deserved = 0;
                                                amount -= (decimal)inv.deserved;
                                            }
                                            else
                                            {
                                                paid = amount;
                                                invObj.paid = invObj.paid + amount;
                                                invObj.deserved -= amount;
                                                amount = 0;
                                            }
                                            cashTr.cash = paid;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // decrease shipping balance
                                            if (shippingCompany.balanceType == 1)
                                            {
                                                if (paid <= (decimal)shippingCompany.balance)
                                                {
                                                    shippingCompany.balance -= paid;
                                                }
                                                else
                                                {
                                                    shippingCompany.balance =
                                                        paid - shippingCompany.balance;
                                                    shippingCompany.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                shippingCompany.balance += paid;
                                            }

                                            entity.SaveChanges();

                                            if (amount == 0)
                                                break;
                                        }
                                        if (amount > 0) // save remain amount
                                        {
                                            shippingCompany = entity.shippingCompanies.Find(
                                                shippingCompanyId
                                            );

                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // decrease shipping balance
                                            if (shippingCompany.balanceType == 1)
                                            {
                                                if (amount <= (decimal)shippingCompany.balance)
                                                {
                                                    shippingCompany.balance =
                                                        shippingCompany.balance - amount;
                                                }
                                                else
                                                {
                                                    shippingCompany.balance =
                                                        amount - shippingCompany.balance;
                                                    shippingCompany.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                shippingCompany.balance += amount;
                                            }

                                            entity.SaveChanges();
                                        }
                                        break;
                                }

                                TokenManager.GenerateToken(cashIds.ToString());
                            }
                            else
                            {
                                if (amount > 0) // save remain amount
                                {
                                    switch (payType)
                                    {
                                        case "feed":
                                            shippingCompany = entity.shippingCompanies.Find(
                                                shippingCompanyId
                                            );

                                            cashTr.cash = amount;
                                            cashTr.invId = null;
                                            cashTr.createDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateDate = coctrlr.AddOffsetTodate(
                                                DateTime.Now
                                            );
                                            cashTr.updateUserId = cashTr.createUserId;
                                            await addCashTransfer(cashTr);

                                            // decrease shipping balance
                                            if (shippingCompany.balanceType == 1)
                                            {
                                                if (amount <= (decimal)shippingCompany.balance)
                                                {
                                                    shippingCompany.balance =
                                                        shippingCompany.balance - amount;
                                                }
                                                else
                                                {
                                                    shippingCompany.balance =
                                                        amount - shippingCompany.balance;
                                                    shippingCompany.balanceType = 0;
                                                }
                                            }
                                            else
                                            {
                                                shippingCompany.balance += amount;
                                            }

                                            entity.SaveChanges();
                                            break;
                                    }
                                }
                                //  return Ok("-1");
                                TokenManager.GenerateToken("1");
                            }
                        }

                        // return Ok("false");
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("0");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
                return TokenManager.GenerateToken("0");
            }
            #region old
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
            //    List<string> typesList = new List<string>();
            //    string cashIds = "";
            //    switch (payType)
            //    {
            //        //case "pay"://get pw,pi,sb invoices

            //        //    typesList.Add("pw");
            //        //    typesList.Add("p");
            //        //    typesList.Add("sb");
            //        //    break;
            //        case "feed": //get si, pb

            //            typesList.Add("pb");
            //            typesList.Add("s");
            //            break;
            //    }
            //    cashTransfer cashTr = JsonConvert.DeserializeObject<cashTransfer>(cashTransfer, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        var invList = (from b in entity.invoices.Where(x => x.shippingCompanyId == shippingCompanyId && typesList.Contains(x.invType) && x.deserved > 0)
            //                       select new InvoiceModel()
            //                       {
            //                           invoiceId = b.invoiceId,
            //                           invNumber = b.invNumber,
            //                           agentId = b.agentId,
            //                           invType = b.invType,
            //                           total = b.total,
            //                           totalNet = b.totalNet,
            //                           paid = b.paid,
            //                           deserved = b.deserved,
            //                           deservedDate = b.deservedDate,
            //                           invDate = b.invDate,
            //                           invoiceMainId = b.invoiceMainId,
            //                           invCase = b.invCase,
            //                           invTime = b.invTime,
            //                           notes = b.notes,
            //                           vendorInvNum = b.vendorInvNum,
            //                           vendorInvDate = b.vendorInvDate,
            //                           createUserId = b.createUserId,
            //                           updateDate = b.updateDate,
            //                           updateUserId = b.updateUserId,
            //                           branchId = b.branchId,
            //                           discountValue = b.discountValue,
            //                           discountType = b.discountType,
            //                           tax = b.tax,
            //                           taxtype = b.taxtype,
            //                           name = b.name,
            //                           isApproved = b.isApproved,
            //                           branchCreatorId = b.branchCreatorId,
            //                           shippingCompanyId = b.shippingCompanyId,
            //                           shipUserId = b.shipUserId,
            //                           userId = b.userId
            //                       }).ToList().OrderBy(b => b.deservedDate);

            //        cashTransfer ct;
            //        shippingCompanies shippingCompany;
            //        if (invList.ToList().Count > 0)
            //        {
            //            switch (payType)
            //            {
            //                case "feed": //get s, pb
            //                    foreach (InvoiceModel inv in invList)
            //                    {
            //                        shippingCompany = entity.shippingCompanies.Find(shippingCompanyId);

            //                        decimal paid = 0;
            //                        var invObj = entity.invoices.Find(inv.invoiceId);
            //                        cashTr.invId = inv.invoiceId;
            //                        if (amount >= inv.deserved)
            //                        {
            //                            paid = (decimal)inv.deserved;
            //                            invObj.paid = invObj.paid + inv.deserved;
            //                            invObj.deserved = 0;
            //                            amount -= (decimal)inv.deserved;
            //                        }
            //                        else
            //                        {
            //                            paid = amount;
            //                            invObj.paid = invObj.paid + amount;
            //                            invObj.deserved -= amount;
            //                            amount = 0;
            //                        }
            //                        cashTr.cash = paid;
            //                        cashTr.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateUserId = cashTr.createUserId;
            //                        ct = entity.cashTransfer.Add(cashTr);

            //                        // decrease shipping balance
            //                        if (shippingCompany.balanceType == 1)
            //                        {
            //                            if (paid <= (decimal)shippingCompany.balance)
            //                            {
            //                                shippingCompany.balance -= paid;
            //                            }
            //                            else
            //                            {
            //                                shippingCompany.balance = paid - shippingCompany.balance;
            //                                shippingCompany.balanceType = 0;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            shippingCompany.balance += paid;
            //                        }

            //                        entity.SaveChanges();
            //                        cashIds += ct.cashTransId + ",";
            //                        if (amount == 0)
            //                            break;
            //                    }
            //                    if (amount > 0) // save remain amount
            //                    {
            //                        shippingCompany = entity.shippingCompanies.Find(shippingCompanyId);

            //                        cashTr.cash = amount;
            //                        cashTr.invId = null;
            //                        cashTr.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateUserId = cashTr.createUserId;
            //                        ct = entity.cashTransfer.Add(cashTr);
            //                        // decrease shipping balance
            //                        if (shippingCompany.balanceType == 1)
            //                        {
            //                            if (amount <= (decimal)shippingCompany.balance)
            //                            {
            //                                shippingCompany.balance = shippingCompany.balance - amount;
            //                            }
            //                            else
            //                            {
            //                                shippingCompany.balance = amount - shippingCompany.balance;
            //                                shippingCompany.balanceType = 0;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            shippingCompany.balance += amount;
            //                        }

            //                        entity.SaveChanges();
            //                    }
            //                    break;
            //                    #endregion
            //            }
            //            return Ok(cashIds);
            //        }
            //        else
            //        {
            //            if (amount > 0) // save remain amount
            //            {
            //                switch (payType)
            //                {
            //                    case "feed":
            //                        shippingCompany = entity.shippingCompanies.Find(shippingCompanyId);

            //                        cashTr.cash = amount;
            //                        cashTr.invId = null;
            //                        cashTr.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                        cashTr.updateUserId = cashTr.createUserId;
            //                        ct = entity.cashTransfer.Add(cashTr);
            //                        // decrease shipping balance
            //                        if (shippingCompany.balanceType == 1)
            //                        {
            //                            if (amount <= (decimal)shippingCompany.balance)
            //                            {
            //                                shippingCompany.balance = shippingCompany.balance - amount;
            //                            }
            //                            else
            //                            {
            //                                shippingCompany.balance = amount - shippingCompany.balance;
            //                                shippingCompany.balanceType = 0;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            shippingCompany.balance += amount;
            //                        }

            //                        entity.SaveChanges();
            //                        break;
            //                }
            //            }
            //            return Ok("-1");
            //        }
            //    }
            //}
            //else
            //    return Ok("false");
            #endregion
        }

        /// <summary>
        /// //////////////
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="amount"></param>
        /// <param name="payType">{pay , feed}</param>
        /// <param name="cashTransfer"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("payListOfInvoices")]
        public async Task<string> payListOfInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                string listObject = "";
                List<invoices> invoiceList = new List<invoices>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                long agentId = 0;

                string payType = "";
                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoices")
                    {
                        listObject = c.Value.Replace("\\", string.Empty);
                        listObject = listObject.Trim('"');
                        invoiceList = JsonConvert.DeserializeObject<List<invoices>>(
                            listObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "agentId")
                    {
                        agentId = long.Parse(c.Value);
                    }
                    else if (c.Type == "payType")
                    {
                        payType = c.Value;
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            agents agent = entity.agents.Find(agentId);

                            switch (payType)
                            {
                                case "pay": //get pw,p,sb invoices
                                    foreach (invoices inv in invoiceList)
                                    {
                                        decimal paid = 0;
                                        var invObj = entity.invoices.Find(inv.invoiceId);
                                        cashTr.invId = inv.invoiceId;

                                        paid = (decimal)inv.deserved;
                                        invObj.paid = invObj.paid + inv.deserved;
                                        invObj.deserved = 0;

                                        cashTr.cash = paid;
                                        cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        cashTr.updateUserId = cashTr.createUserId;
                                        cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                        cashTr = entity.cashTransfer.Add(cashTr);
                                        // increase agent balance
                                        if (agent.balanceType == 0)
                                        {
                                            if (paid <= (decimal)agent.balance)
                                            {
                                                agent.balance = agent.balance - paid;
                                            }
                                            else
                                            {
                                                agent.balance = paid - agent.balance;
                                                agent.balanceType = 1;
                                            }
                                        }
                                        else if (agent.balanceType == 1)
                                        {
                                            agent.balance += paid;
                                        }
                                        entity.SaveChanges();
                                    }
                                    entity.SaveChanges();
                                    break;
                                case "feed": //get s, pb
                                    foreach (invoices inv in invoiceList)
                                    {
                                        decimal paid = 0;
                                        var invObj = entity.invoices.Find(inv.invoiceId);
                                        cashTr.invId = inv.invoiceId;

                                        paid = (decimal)inv.deserved;
                                        invObj.paid = invObj.paid + inv.deserved;
                                        invObj.deserved = 0;

                                        cashTr.cash = paid;
                                        cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                        cashTr.updateUserId = cashTr.createUserId;
                                        cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                        cashTr = entity.cashTransfer.Add(cashTr);

                                        if (agent.balanceType == 1)
                                        {
                                            if (paid <= (decimal)agent.balance)
                                            {
                                                agent.balance = agent.balance - paid;
                                            }
                                            else
                                            {
                                                agent.balance = paid - agent.balance;
                                                agent.balanceType = 0;
                                            }
                                        }
                                        else if (agent.balanceType == 0)
                                        {
                                            agent.balance += paid;
                                        }
                                        entity.SaveChanges();

                                        if (cashTr.processType == "card")
                                            AddCardCommission(cashTr);
                                    }
                                    entity.SaveChanges();
                                    break;
                            }

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-2");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
            }

            #region old
            //                var re = Request;
            //var headers = re.Headers;
            //string token = "";
            //string cashIds = "";
            //if (headers.Contains("APIKey"))
            //{
            //    token = headers.GetValues("APIKey").First();
            //}
            //Validation validation = new Validation();
            //bool valid = validation.CheckApiKey(token);

            //if (valid)
            //{
            //    invoices = invoices.Replace("\\", string.Empty);
            //    invoices = invoices.Trim('"');

            //    List<invoices> invoiceList = JsonConvert.DeserializeObject<List<invoices>>(invoices, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            //    cashTransfer cashTr = JsonConvert.DeserializeObject<cashTransfer>(cashTransfer, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        agents agent = entity.agents.Find(agentId);

            //        switch (payType)
            //        {
            //            case "pay"://get pw,p,sb invoices
            //                foreach (invoices inv in invoiceList)
            //                {
            //                    decimal paid = 0;
            //                    cashTransfer ct;
            //                    var invObj = entity.invoices.Find(inv.invoiceId);
            //                    cashTr.invId = inv.invoiceId;

            //                    paid = (decimal)inv.deserved;
            //                    invObj.paid = invObj.paid + inv.deserved;
            //                    invObj.deserved = 0;

            //                    cashTr.cash = paid;
            //                    cashTr.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                    cashTr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                    cashTr.updateUserId = cashTr.createUserId;
            //                    ct = entity.cashTransfer.Add(cashTr);
            //                    cashIds += ct.cashTransId + ",";
            //                    // increase agent balance
            //                    if (agent.balanceType == 1)
            //                    {
            //                        if (paid <= (decimal)agent.balance)
            //                        {
            //                            agent.balance = agent.balance - paid;
            //                        }
            //                        else
            //                        {
            //                            agent.balance = paid - agent.balance;
            //                            agent.balanceType = 0;
            //                        }
            //                    }
            //                    else if (agent.balanceType == 0)
            //                    {
            //                        agent.balance += paid;
            //                    }
            //                    entity.SaveChanges();
            //                }
            //                entity.SaveChanges();
            //                break;
            //            case "feed": //get s, pb
            //                foreach (invoices inv in invoiceList)
            //                {
            //                    decimal paid = 0;
            //                    cashTransfer ct;
            //                    var invObj = entity.invoices.Find(inv.invoiceId);
            //                    cashTr.invId = inv.invoiceId;

            //                    paid = (decimal)inv.deserved;
            //                    invObj.paid = invObj.paid + inv.deserved;
            //                    invObj.deserved = 0;

            //                    cashTr.cash = paid;
            //                    cashTr.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                    cashTr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
            //                    cashTr.updateUserId = cashTr.createUserId;
            //                    ct = entity.cashTransfer.Add(cashTr);
            //                    cashIds += ct.cashTransId + ",";
            //                    // decrease agent balance
            //                    if (agent.balanceType == 0)
            //                    {
            //                        if (paid <= (decimal)agent.balance)
            //                        {
            //                            agent.balance = agent.balance - paid;
            //                        }
            //                        else
            //                        {
            //                            agent.balance = paid - agent.balance;
            //                            agent.balanceType = 1;
            //                        }
            //                    }
            //                    else if (agent.balanceType == 1)
            //                    {
            //                        agent.balance += paid;
            //                    }
            //                    entity.SaveChanges();
            //                }
            //                entity.SaveChanges();
            //                break;
            //        }
            //        return Ok(cashIds);
            //    }
            //}
            //else
            //    return Ok("false");
            #endregion
        }

        [HttpPost]
        [Route("SaveCashWithCommission")]
        public async Task<string> SaveCashWithCommission(string token)
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
                cashTransfer newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
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
                        message = await addCashTransfer(newObject);
                        newObject.cashTransId = int.Parse(message);

                        if (newObject.processType == "card")
                            AddCardCommission(newObject);
                        return TokenManager.GenerateToken(message);
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }

        [NonAction]
        public async void AddCardCommission(cashTransfer basicCash)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var card = entity.cards.Find(basicCash.cardId);

                decimal commissionValue = 0;
                if (card.commissionValue > 0)
                    commissionValue += (decimal)card.commissionValue;
                if (card.commissionRatio > 0)
                {
                    Calculate calc = new Calculate();
                    commissionValue += calc.percentValue(basicCash.cash, card.commissionRatio);
                }

                if (commissionValue > 0)
                {
                    var cashTransfer = new cashTransfer()
                    {
                        invId = basicCash.invId,
                        cashTransIdSource = basicCash.cashTransId,
                        cardId = basicCash.cardId,
                        posId = basicCash.posId,
                        cash = commissionValue,
                        deserved = 0,
                        paid = commissionValue,
                        processType = "commissionCard",
                        transType = "d",
                        side = "card",
                        isCommissionPaid = 1,
                        commissionValue = card.commissionValue,
                        commissionRatio = card.commissionRatio,
                        transNum = await generateCashNumber("d" + "u"),
                        createUserId = basicCash.createUserId,
                        updateUserId = basicCash.createUserId,
                        createDate = coctrlr.AddOffsetTodate(DateTime.Now),
                        updateDate = coctrlr.AddOffsetTodate(DateTime.Now),
                    };

                    entity.cashTransfer.Add(cashTransfer);
                    entity.SaveChanges();
                }
            }
        }

        /// <summary>
        /// //////////////
        /// </summary>
        /// <param name="shippingCompanyId"></param>
        /// <param name="invoices"></param>
        /// <param name="payType">{feed}</param>
        /// <param name="cashTransfer"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("payShippingCompanyListOfInvoices")]
        public async Task<string> payShippingCompanyListOfInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region parameters
                string Object = "";
                string listObject = "";
                List<invoices> invoiceList = new List<invoices>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                long shippingCompanyId = 0;

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoices")
                    {
                        listObject = c.Value.Replace("\\", string.Empty);
                        listObject = listObject.Trim('"');
                        invoiceList = JsonConvert.DeserializeObject<List<invoices>>(
                            listObject,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            shippingCompanies shippingCompany = entity.shippingCompanies.Find(
                                shippingCompanyId
                            );

                            foreach (invoices inv in invoiceList)
                            {
                                decimal paid = 0;
                                //cashTransfer ct;
                                var invObj = entity.invoices.Find(inv.invoiceId);
                                cashTr.invId = inv.invoiceId;

                                paid = (decimal)inv.deserved;
                                invObj.paid = invObj.paid + inv.deserved;
                                invObj.deserved = 0;

                                cashTr.cash = paid;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr = entity.cashTransfer.Add(cashTr);

                                if (shippingCompany.balanceType == 1)
                                {
                                    if (paid <= (decimal)shippingCompany.balance)
                                    {
                                        shippingCompany.balance = shippingCompany.balance - paid;
                                    }
                                    else
                                    {
                                        shippingCompany.balance = paid - shippingCompany.balance;
                                        shippingCompany.balanceType = 0;
                                    }
                                }
                                else if (shippingCompany.balanceType == 0)
                                {
                                    shippingCompany.balance += paid;
                                }
                                //entity.SaveChanges();

                                #region make order praparing status as "Done"
                                var preparingOrders = entity.orderPreparing
                                    .Where(x => x.invoiceId == inv.invoiceId)
                                    .ToList();

                                foreach (var row in preparingOrders)
                                {
                                    orderPreparingStatus ops = new orderPreparingStatus();
                                    ops.status = "Done";
                                    ops.orderPreparingId = row.orderPreparingId;
                                    ops.isActive = 1;
                                    entity.orderPreparingStatus.Add(ops);
                                }

                                #endregion
                                entity.SaveChanges();
                            }
                            entity.SaveChanges();
                            //add card commission
                            if (cashTr.processType == "card")
                                AddCardCommission(cashTr);

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-2");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        [HttpPost]
        [Route("payListShortageCashes")]
        public async Task<string> payListShortageCashes(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                List<cashTransfer> cashesList = new List<cashTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashesList")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashesList = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (var cash in cashesList)
                            {
                                decimal paid = 0;
                                var deliverCash = entity.cashTransfer.Find(cash.cashTransId);
                                deliverCash.paid += deliverCash.deserved;
                                paid = (decimal)deliverCash.deserved;
                                deliverCash.deserved = 0;
                                deliverCash.isCommissionPaid = 1;

                                cashTr.cash = paid;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr = entity.cashTransfer.Add(cashTr);

                                entity.SaveChanges();
                                increaseUserBalance((long)deliverCash.userId, paid);

                                //add card commission
                                if (cashTr.processType == "card")
                                    AddCardCommission(cashTr);
                            }

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [HttpPost]
        [Route("payShortageCashesByAmount")]
        public async Task<string> payShortageCashesByAmount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                long userId = 0;
                decimal amount = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var cashesList = entity.cashTransfer
                                .Where(
                                    x =>
                                        x.userId == userId
                                        && (
                                            x.processType.Trim() == "destroy"
                                            || x.processType.Trim() == "shortage"
                                        )
                                        && x.deserved > 0
                                )
                                .ToList();

                            decimal basicAmount = amount;
                            foreach (var cash in cashesList)
                            {
                                decimal paid = 0;
                                if (amount >= cash.deserved)
                                {
                                    paid = (decimal)cash.deserved;
                                    amount -= (decimal)cash.deserved;
                                    cash.paid += cash.deserved;
                                    cash.deserved = 0;
                                    cash.isCommissionPaid = 1;
                                }
                                else
                                {
                                    paid = amount;
                                    cash.paid = cash.paid + amount;
                                    cash.deserved -= amount;
                                    amount = 0;
                                }
                                entity.SaveChanges();
                                cashTr.cash = paid;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr.cashTransIdSource = cash.cashTransId;
                                cashTr = entity.cashTransfer.Add(cashTr);

                                entity.SaveChanges();

                                //add card commission
                                if (cashTr.processType == "card")
                                    AddCardCommission(cashTr);

                                if (amount == 0)
                                    break;
                            }

                            if (amount > 0)
                            {
                                cashTr.cash = amount;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr.cashTransIdSource = null;
                                cashTr = entity.cashTransfer.Add(cashTr);
                                entity.SaveChanges();
                                //add card commission
                                if (cashTr.processType == "card")
                                    AddCardCommission(cashTr);
                            }

                            increaseUserBalance(userId, basicAmount);

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [NonAction]
        public void increaseUserBalance(long userId, decimal paid)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var user = entity.users.Find(userId);
                if (user.balanceType == 1)
                {
                    if (paid <= (decimal)user.balance)
                    {
                        user.balance = user.balance - paid;
                    }
                    else
                    {
                        user.balance = paid - user.balance;
                        user.balanceType = 0;
                    }
                }
                else if (user.balanceType == 0)
                {
                    user.balance += paid;
                }
                entity.SaveChanges();
            }
        }

        [HttpPost]
        [Route("payOrderInvoice")]
        public async Task<string> payOrderInvoice(string token)
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
                List<invoices> invoiceList = new List<invoices>();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                long invoiceId = 0;
                long invStatusId = 0;
                string payType = "";
                decimal amount = 0;
                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "invoiceId")
                    {
                        invoiceId = long.Parse(c.Value);
                    }
                    else if (c.Type == "invStatusId")
                    {
                        invStatusId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "payType")
                    {
                        payType = c.Value;
                    }
                }
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            invoices inv = entity.invoices.Find(invoiceId);
                            agents agent = entity.agents.Find(inv.agentId);
                            invoiceStatus invStatus = entity.invoiceStatus.Find(invStatusId);

                            //update invoice type
                            invStatus.status = "tr";
                            //add cashtransfer
                            cashTr.invId = inv.invoiceId;
                            cashTr.cash = amount;
                            cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            cashTr.updateUserId = cashTr.createUserId;

                            cashTransfer ct;
                            //update agent
                            switch (payType)
                            {
                                case "0": // cash - card - cheque - doc

                                    //update invoice
                                    inv.paid += amount;
                                    inv.deserved -= amount;
                                    await addCashTransfer(cashTr);
                                    //ct = entity.cashTransfer.Add(cashTr);

                                    // decrease agent balance
                                    if (agent.balanceType == 0)
                                    {
                                        if (amount <= (decimal)agent.balance)
                                        {
                                            agent.balance -= amount;
                                        }
                                        else
                                        {
                                            agent.balance = amount - agent.balance;
                                            agent.balanceType = 1;
                                        }
                                    }
                                    else if (agent.balanceType == 1)
                                    {
                                        if (amount <= (decimal)agent.balance)
                                        {
                                            agent.balance -= amount;
                                        }
                                        else
                                        {
                                            agent.balance = amount - agent.balance;
                                            agent.balanceType = 0;
                                        }
                                    }
                                    break;
                                case "1": // balance
                                    decimal newBalance = 0;
                                    if (agent.balanceType == 0)
                                    {
                                        //cash
                                        cashTr.transType = "balance";
                                        if (amount <= (decimal)agent.balance)
                                        {
                                            //invoice
                                            inv.paid += amount;
                                            inv.deserved -= amount;
                                            //agent
                                            newBalance = (decimal)agent.balance - amount;
                                            agent.balance = newBalance;
                                        }
                                        else
                                        {
                                            //invoice
                                            inv.paid += agent.balance;
                                            inv.deserved -= agent.balance;
                                            //agent
                                            newBalance = (decimal)amount - (decimal)agent.balance;
                                            agent.balance = newBalance;
                                            agent.balanceType = 1;
                                            //cash
                                            cashTr.cash = newBalance;
                                        }
                                        await addCashTransfer(cashTr);
                                        // ct = entity.cashTransfer.Add(cashTr);
                                    }
                                    else if (agent.balanceType == 1)
                                    {
                                        newBalance = (decimal)agent.balance + amount;
                                        agent.balance = newBalance;
                                    }
                                    break;
                            }
                            message = entity.SaveChanges().ToString();
                            return TokenManager.GenerateToken(message);

                            // return Ok("true");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-2");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("0");
                }
                //  return TokenManager.GenerateToken("0");
            }
        }

        [HttpPost]
        [Route("GetLastNumOfCash")]
        public string GetLastNumOfCash(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string cashCode = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "cashCode")
                    {
                        cashCode = c.Value;
                    }
                }

                try
                {
                    List<string> numberList;
                    int lastNum = 0;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        numberList = entity.cashTransfer
                            .Where(b => b.transNum.Contains(cashCode + "-"))
                            .Select(b => b.transNum)
                            .ToList();

                        for (int i = 0; i < numberList.Count; i++)
                        {
                            string code = numberList[i];
                            string s = code.Substring(code.LastIndexOf("-") + 1);
                            numberList[i] = s;
                        }
                        if (numberList.Count > 0)
                        {
                            numberList.Sort();
                            lastNum = int.Parse(numberList[numberList.Count - 1]);
                        }
                    }
                    //  return Ok(lastNum);
                    return TokenManager.GenerateToken(lastNum.ToString());
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }

                //  return TokenManager.GenerateToken("0");
            }

            #region old
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
            //    List<string> numberList;
            //    int lastNum = 0;
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        numberList = entity.cashTransfer.Where(b => b.transNum.Contains(cashCode + "-")).Select(b => b.transNum).ToList();

            //        for (int i = 0; i < numberList.Count; i++)
            //        {
            //            string code = numberList[i];
            //            string s = code.Substring(code.LastIndexOf("-") + 1);
            //            numberList[i] = s;
            //        }
            //        if (numberList.Count > 0)
            //        {
            //            numberList.Sort();
            //            lastNum = int.Parse(numberList[numberList.Count - 1]);
            //        }
            //    }
            //    return Ok(lastNum);
            //}
            //return NotFound();
            #endregion
        }

        [HttpPost]
        [Route("GetLastNumOfDocNum")]
        public string GetLastNumOfDocNum(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string docNum = "";

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                foreach (Claim c in claims)
                {
                    if (c.Type == "docNum")
                    {
                        docNum = c.Value;
                    }
                }

                try
                {
                    List<string> numberList;
                    int lastNum = 0;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        numberList = entity.cashTransfer
                            .Where(b => b.docNum.Contains(docNum + "-"))
                            .Select(b => b.docNum)
                            .ToList();

                        for (int i = 0; i < numberList.Count; i++)
                        {
                            string code = numberList[i];
                            string s = code.Substring(code.LastIndexOf("-") + 1);
                            numberList[i] = s;
                        }
                        if (numberList.Count > 0)
                        {
                            numberList.Sort();
                            lastNum = int.Parse(numberList[numberList.Count - 1]);
                        }
                    }
                    //  return Ok(lastNum);
                    return TokenManager.GenerateToken(lastNum.ToString());
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }

                //  return TokenManager.GenerateToken("0");
            }
            #region old
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
            //    List<string> numberList;
            //    int lastNum = 0;
            //    using (incposdbEntities entity = new incposdbEntities())
            //    {
            //        numberList = entity.cashTransfer.Where(b => b.docNum.Contains(docNum + "-")).Select(b => b.docNum).ToList();

            //        for (int i = 0; i < numberList.Count; i++)
            //        {
            //            string code = numberList[i];
            //            string s = code.Substring(code.LastIndexOf("-") + 1);
            //            numberList[i] = s;
            //        }
            //        if (numberList.Count > 0)
            //        {
            //            numberList.Sort();
            //            lastNum = int.Parse(numberList[numberList.Count - 1]);
            //        }
            //    }
            //    return Ok(lastNum);
            //}
            //return NotFound();
            #endregion
        }

        #region old

        //public string Save(cashTransfer newObject)
        //{

        //    //string Object
        //    int message = 0;



        //    if (newObject != null)
        //    {

        //        try
        //        {

        //            if (newObject.updateUserId == 0 || newObject.updateUserId == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.updateUserId = id;
        //            }
        //            if (newObject.createUserId == 0 || newObject.createUserId == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.createUserId = id;
        //            }

        //            if (newObject.agentId == 0 || newObject.agentId == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.agentId = id;
        //            }
        //            if (newObject.invId == 0 || newObject.invId == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.invId = id;
        //            }
        //            if (newObject.posIdCreator == 0 || newObject.posIdCreator == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.posIdCreator = id;
        //            }

        //            if (newObject.cashTransIdSource == 0 || newObject.cashTransIdSource == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.cashTransIdSource = id;
        //            }
        //            if (newObject.bankId == 0 || newObject.bankId == null)
        //            {
        //                Nullable<long> id = null;
        //                newObject.bankId = id;
        //            }

        //            cashTransfer cashtr;
        //            using (incposdbEntities entity = new incposdbEntities())
        //            {
        //                var cEntity = entity.Set<cashTransfer>();
        //                if (newObject.cashTransId == 0)
        //                {
        //                    newObject.createDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);
        //                    newObject.updateUserId = newObject.createUserId;
        //                    cashtr = cEntity.Add(newObject);
        //                }
        //                else
        //                {
        //                    cashtr = entity.cashTransfer.Where(p => p.cashTransId == newObject.cashTransId).First();
        //                    cashtr.transType = newObject.transType;
        //                    cashtr.posId = newObject.posId;
        //                    cashtr.userId = newObject.userId;
        //                    cashtr.agentId = newObject.agentId;
        //                    cashtr.invId = newObject.invId;
        //                    cashtr.transNum = newObject.transNum;
        //                    cashtr.createDate = newObject.createDate;
        //                    cashtr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
        //                    cashtr.cash = newObject.cash;
        //                    cashtr.updateUserId = newObject.updateUserId;
        //                    // cashtr.createUserId = newObject. ;
        //                    cashtr.notes = newObject.notes;
        //                    cashtr.posIdCreator = newObject.posIdCreator;
        //                    cashtr.isConfirm = newObject.isConfirm;
        //                    cashtr.cashTransIdSource = newObject.cashTransIdSource;
        //                    cashtr.side = newObject.side;

        //                    cashtr.docName = newObject.docName;
        //                    cashtr.docNum = newObject.docNum;
        //                    cashtr.docImage = newObject.docImage;
        //                    cashtr.bankId = newObject.bankId;
        //                    cashtr.updateDate =  coctrlr.AddOffsetTodate(DateTime.Now);// server current date
        //                    cashtr.processType = newObject.processType;
        //                    cashtr.cardId = newObject.cardId;
        //                    cashtr.bondId = newObject.bondId;
        //                    cashtr.shippingCompanyId = newObject.shippingCompanyId;

        //                }
        //                entity.SaveChanges();
        //            }
        //            message = cashtr.cashTransId;

        //            return message.ToString();

        //        }
        //        catch (Exception ex)
        //        {

        //            //message =-ex;
        //          //  return ex.ToString();

        //              return  ex.ToString() ;
        //        }


        //    }
        //    message = -214;
        //    return message.ToString();

        //}
        #endregion

        public long Save(cashTransfer newObject)
        {
            //string Object
            long message = 0;

            if (newObject != null)
            {
                try
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

                    if (newObject.agentId == 0 || newObject.agentId == null)
                    {
                        Nullable<long> id = null;
                        newObject.agentId = id;
                    }
                    if (newObject.invId == 0 || newObject.invId == null)
                    {
                        Nullable<long> id = null;
                        newObject.invId = id;
                    }
                    if (newObject.posIdCreator == 0 || newObject.posIdCreator == null)
                    {
                        Nullable<long> id = null;
                        newObject.posIdCreator = id;
                    }

                    if (newObject.cashTransIdSource == 0 || newObject.cashTransIdSource == null)
                    {
                        Nullable<long> id = null;
                        newObject.cashTransIdSource = id;
                    }
                    if (newObject.bankId == 0 || newObject.bankId == null)
                    {
                        Nullable<long> id = null;
                        newObject.bankId = id;
                    }

                    cashTransfer cashtr;
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        var cEntity = entity.Set<cashTransfer>();
                        if (newObject.cashTransId == 0)
                        {
                            newObject.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                            newObject.updateUserId = newObject.createUserId;
                            cashtr = cEntity.Add(newObject);
                        }
                        else
                        {
                            cashtr = entity.cashTransfer
                                .Where(p => p.cashTransId == newObject.cashTransId)
                                .First();
                            cashtr.transType = newObject.transType;
                            cashtr.posId = newObject.posId;
                            cashtr.userId = newObject.userId;
                            cashtr.agentId = newObject.agentId;
                            cashtr.invId = newObject.invId;
                            cashtr.transNum = newObject.transNum;
                            cashtr.createDate = newObject.createDate;
                            cashtr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date
                            cashtr.cash = newObject.cash;
                            cashtr.updateUserId = newObject.updateUserId;
                            // cashtr.createUserId = newObject. ;
                            cashtr.notes = newObject.notes;
                            cashtr.posIdCreator = newObject.posIdCreator;
                            cashtr.isConfirm = newObject.isConfirm;
                            cashtr.cashTransIdSource = newObject.cashTransIdSource;
                            cashtr.side = newObject.side;

                            cashtr.docName = newObject.docName;
                            cashtr.docNum = newObject.docNum;
                            cashtr.docImage = newObject.docImage;
                            cashtr.bankId = newObject.bankId;
                            cashtr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now); // server current date
                            cashtr.processType = newObject.processType;
                            cashtr.cardId = newObject.cardId;
                            cashtr.bondId = newObject.bondId;
                            cashtr.shippingCompanyId = newObject.shippingCompanyId;
                        }
                        entity.SaveChanges();
                    }
                    message = cashtr.cashTransId;

                    return message;
                }
                catch (Exception ex)
                {
                    message = -213;
                    return message;

                    //  return TokenManager.GenerateToken(ex.ToString());
                }
            }
            message = -214;
            return message;
        }

        [HttpPost]
        [Route("getLastOpenTransNum")]
        public string getLastOpenTransNum(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long posId = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "posId")
                    {
                        posId = long.Parse(c.Value);
                    }
                }
                try
                {
                    string numberList = "";
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        numberList = entity.cashTransfer
                            .Where(b => b.posId == posId && b.transType == "o")
                            .ToList()
                            .OrderBy(b => b.cashTransId)
                            .LastOrDefault()
                            .transNum;
                    }
                    return TokenManager.GenerateToken(numberList);
                }
                catch
                {
                    return TokenManager.GenerateToken("0");
                }
            }
        }

        public async Task<string> generateCashNumber(string cashCode)
        {
            #region check if last of code is num
            string num = cashCode.Substring(cashCode.LastIndexOf("-") + 1);

            if (!num.Equals(cashCode))
                return cashCode;

            #endregion

            List<string> numberList;
            int sequence = 0;
            using (incposdbEntities entity = new incposdbEntities())
            {
                numberList = entity.cashTransfer
                    .Where(b => b.transNum.Contains(cashCode + "-"))
                    .Select(b => b.transNum)
                    .ToList();

                for (int i = 0; i < numberList.Count; i++)
                {
                    string code = numberList[i];
                    string s = code.Substring(code.LastIndexOf("-") + 1);
                    numberList[i] = s;
                }
                if (numberList.Count > 0)
                {
                    numberList.Sort();
                    sequence = int.Parse(numberList[numberList.Count - 1]);
                }
            }
            sequence++;

            string strSeq = sequence.ToString();
            if (sequence <= 999999)
                strSeq = sequence.ToString().PadLeft(6, '0');
            string transNum = cashCode + "-" + strSeq;
            return transNum;
        }

        [HttpPost]
        [Route("SaveWithBalanceCheck")]
        public async Task<string> SaveWithBalanceCheck(string token)
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
                #region params
                string Object = "";
                cashTransfer newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        break;
                    }
                }
                #endregion
                if (newObject != null)
                {
                    try
                    {
                        using (var entity = new incposdbEntities())
                        {
                            var pos = entity.pos.Find(newObject.posId);

                            if (pos.balance < newObject.cash)
                            {
                                return TokenManager.GenerateToken("-3");
                            }
                        }
                        message = await addCashTransfer(newObject);
                        return TokenManager.GenerateToken(message);
                    }
                    catch
                    {
                        message = "0";
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "0";
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("MakeDeposit")]
        public async Task<string> MakeDeposit(string token)
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
                cashTransfer newObject = null;
                CashTransferModel cashTransferModel = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        cashTransferModel = JsonConvert.DeserializeObject<CashTransferModel>(
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
                        var allList = this.Getpostransmodel(newObject.cashTransId).ToList();
                        var delList = allList.Where(C => C.isConfirm == 1).ToList();
                        if (delList != null)
                        {
                            if (delList.Count == 2)
                            {
                                message = "-2";
                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                pos pos;
                                using (incposdbEntities entity = new incposdbEntities())
                                {
                                    pos = entity.pos.Find(newObject.posId);
                                    var pos2 = entity.pos.Find(cashTransferModel.pos2Id);

                                    if (pos.balance < newObject.cash)
                                    {
                                        return TokenManager.GenerateToken("-3");
                                    }

                                    pos.balance -= newObject.cash;

                                    pos2.balance += newObject.cash;

                                    entity.SaveChanges();
                                }
                                newObject.isConfirm = 1;
                                decimal res = await Confirm(newObject, cashTransferModel);
                                if (res > 0)
                                    res = (decimal)pos.balance;

                                return TokenManager.GenerateToken(res.ToString());
                            }
                        }
                    }
                    catch
                    {
                        message = "-1";
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "-1";
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("MakePull")]
        public async Task<string> MakePull(string token)
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
                cashTransfer newObject = null;
                CashTransferModel cashTransferModel = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                        cashTransferModel = JsonConvert.DeserializeObject<CashTransferModel>(
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
                        var allList = this.Getpostransmodel(newObject.cashTransId).ToList();
                        var delList = allList.Where(C => C.isConfirm == 1).ToList();
                        if (delList != null)
                        {
                            if (delList.Count == 2)
                            {
                                message = "-2";
                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                pos pos;
                                using (incposdbEntities entity = new incposdbEntities())
                                {
                                    pos = entity.pos.Find(newObject.posId);
                                    var pos2 = entity.pos.Find(cashTransferModel.pos2Id);

                                    if (pos2.balance < newObject.cash)
                                    {
                                        return TokenManager.GenerateToken("-3");
                                    }

                                    pos2.balance -= newObject.cash;

                                    pos.balance += newObject.cash;

                                    entity.SaveChanges();
                                }
                                newObject.isConfirm = 1;
                                decimal res = await Confirm(newObject, cashTransferModel);
                                if (res > 0)
                                {
                                    res = pos.balance;
                                }
                                return TokenManager.GenerateToken(res.ToString());
                            }
                        }
                    }
                    catch
                    {
                        message = "-1";
                        return TokenManager.GenerateToken(message);
                    }
                }

                return TokenManager.GenerateToken(message);
            }
        }

        private async Task<int> Confirm(cashTransfer newObject, CashTransferModel cashTransferModel)
        {
            try
            {
                await addCashTransfer(newObject);
                using (incposdbEntities entity = new incposdbEntities())
                {
                    //edit updatedate for cashTrans 2
                    var cashTrans2 = entity.cashTransfer.Find(cashTransferModel.cashTrans2Id);
                    cashTrans2.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                    entity.SaveChanges();
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("transferPosBalance")]
        public async Task<string> transferPosBalance(string token)
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
                cashTransfer cash1 = null;
                cashTransfer cash2 = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "cash1")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cash1 = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cash2")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cash2 = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        long posId = (long)cash1.posId;
                        var pos = entity.pos.Find(posId);

                        cash1.cash = cash2.cash = pos.balance;

                        message = await addCashTransfer(cash1);
                        if (int.Parse(message) > 0)
                        {
                            cash2.cashTransIdSource = int.Parse(message);
                            await addCashTransfer(cash2);
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
        [Route("confirmBankTransfer")]
        public async Task<string> confirmBankTransfer(string token)
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
                cashTransfer newObject = null;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "Object")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        newObject = JsonConvert.DeserializeObject<cashTransfer>(
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
                        decimal amount = (decimal)newObject.cash;
                        pos pos;

                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            pos = entity.pos.Find(newObject.posId);

                            if (newObject.transType.Equals("d"))
                            {
                                amount *= -1;

                                if (pos.balance < (decimal)newObject.cash)
                                    return TokenManager.GenerateToken("-3");
                            }

                            message = await addCashTransfer(newObject);

                            pos.balance += amount;
                            entity.SaveChanges();
                        }
                        return TokenManager.GenerateToken(pos.balance.ToString());
                    }
                    catch
                    {
                        message = "-1";
                        return TokenManager.GenerateToken(message);
                    }
                }
                message = "-1";
                return TokenManager.GenerateToken(message);
            }
        }

        [HttpPost]
        [Route("Cancle")]
        public string Cancle(string token)
        {
            string message = "-1";

            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long cashTransId1 = 0;
                long cashTransId2 = 0;

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashTransId1")
                    {
                        cashTransId1 = long.Parse(c.Value);
                    }
                    else if (c.Type == "cashTransId2")
                    {
                        cashTransId2 = long.Parse(c.Value);
                    }
                }

                List<CashTransferModel> delList = null;
                List<CashTransferModel> allList = null;
                cashTransfer cashobject = new cashTransfer();

                try
                {
                    using (incposdbEntities entity = new incposdbEntities())
                    {
                        allList = this.Getpostransmodel(cashTransId1).ToList();
                        delList = allList.Where(C => C.isConfirm == 1).ToList();
                        if (delList != null)
                        {
                            if (delList.Count == 2)
                            {
                                message = "-2";
                                return TokenManager.GenerateToken(message);
                            }
                            else
                            {
                                var cash1 = entity.cashTransfer.Find(cashTransId1);
                                var cash2 = entity.cashTransfer.Find(cashTransId2);

                                cash1.isConfirm = 2;
                                cash2.isConfirm = 2;
                                entity.SaveChanges();

                                message = "1";
                            }
                        }

                        return TokenManager.GenerateToken(message);
                    }
                }
                catch
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [HttpPost]
        [Route("payListCommissionCashes")]
        public async Task<string> payListCommissionCashes(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                List<cashTransfer> cashesList = new List<cashTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "cashesList")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashesList = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (var cash in cashesList)
                            {
                                decimal paid = 0;

                                var deliverCash = entity.cashTransfer.Find(cash.cashTransId);
                                deliverCash.paid += deliverCash.deserved;
                                paid = (decimal)deliverCash.deserved;
                                deliverCash.deserved = 0;
                                deliverCash.isCommissionPaid = 1;
                                entity.SaveChanges();

                                cashTr.cash = paid;
                                cashTr.cashTransIdSource = cash.cashTransId;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                entity.cashTransfer.Add(cashTr);
                                entity.SaveChanges();

                                decreaseUserBalance((long)deliverCash.userId, paid);
                            }

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [HttpPost]
        [Route("payCommissionCashesByAmount")]
        public async Task<string> payCommissionCashesByAmount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                long userId = 0;
                decimal amount = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                    else if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var cashesList = entity.cashTransfer
                                .Where(
                                    x =>
                                        x.userId == userId
                                        && x.processType.Trim() == "commissionAgent"
                                        && x.deserved > 0
                                )
                                .ToList();

                            decimal basicAmount = amount;
                            foreach (var cash in cashesList)
                            {
                                decimal paid = 0;
                                if (amount >= cash.deserved)
                                {
                                    paid = (decimal)cash.deserved;
                                    amount -= (decimal)cash.deserved;
                                    cash.paid += cash.deserved;
                                    cash.deserved = 0;
                                    cash.isCommissionPaid = 1;
                                }
                                else
                                {
                                    paid = amount;
                                    cash.paid = cash.paid + amount;
                                    cash.deserved -= amount;
                                    amount = 0;
                                }
                                entity.SaveChanges();
                                cashTr.cash = paid;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr.cashTransIdSource = cash.cashTransId;
                                entity.cashTransfer.Add(cashTr);

                                entity.SaveChanges();

                                if (amount == 0)
                                    break;
                            }

                            if (amount > 0)
                            {
                                cashTr.cash = amount;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                cashTr.cashTransIdSource = null;

                                entity.cashTransfer.Add(cashTr);
                                entity.SaveChanges();
                            }
                            decreaseUserBalance(userId, basicAmount);

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [NonAction]
        public void decreaseUserBalance(long userId, decimal paid)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var user = entity.users.Find(userId);
                if (user.balanceType == 0)
                {
                    if (paid <= (decimal)user.balance)
                    {
                        user.balance = user.balance - paid;
                    }
                    else
                    {
                        user.balance = paid - user.balance;
                        user.balanceType = 1;
                    }
                }
                else if (user.balanceType == 1)
                {
                    user.balance += paid;
                }
                entity.SaveChanges();
            }
        }

        [HttpPost]
        [Route("payDeliveryCostOfInvoices")]
        public async Task<string> payDeliveryCostOfInvoices(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                List<cashTransfer> cashesList = new List<cashTransfer>();
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                long shippingCompanyId = 0;

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "invoices")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashesList = JsonConvert.DeserializeObject<List<cashTransfer>>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            foreach (var cash in cashesList)
                            {
                                decimal paid = 0;
                                var deliverCash = entity.cashTransfer.Find(cash.cashTransId);
                                deliverCash.paid += deliverCash.deserved;
                                paid = (decimal)deliverCash.deserved;
                                deliverCash.deserved = 0;
                                deliverCash.isCommissionPaid = 1;

                                cashTr.cash = paid;
                                cashTr.cashTransIdSource = cash.cashTransId;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                entity.cashTransfer.Add(cashTr);

                                entity.SaveChanges();
                                decreaseShippingComBalance((long)cash.shippingCompanyId, paid);
                            }

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [NonAction]
        public void decreaseShippingComBalance(long shippingComId, decimal paid)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var com = entity.shippingCompanies.Find(shippingComId);
                if (com.balanceType == 0)
                {
                    if (paid <= (decimal)com.balance)
                    {
                        com.balance = com.balance - paid;
                    }
                    else
                    {
                        com.balance = paid - com.balance;
                        com.balanceType = 1;
                    }
                }
                else if (com.balanceType == 1)
                {
                    com.balance += paid;
                }
                entity.SaveChanges();
            }
        }

        [HttpPost]
        [Route("payDeliveryCostByAmount")]
        public async Task<string> payDeliveryCostByAmount(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                #region params
                string Object = "";
                decimal amount = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);

                long shippingCompanyId = 0;

                cashTransfer cashTr = new cashTransfer();
                foreach (Claim c in claims)
                {
                    if (c.Type == "amount")
                    {
                        amount = decimal.Parse(c.Value);
                    }
                    else if (c.Type == "cashTransfer")
                    {
                        Object = c.Value.Replace("\\", string.Empty);
                        Object = Object.Trim('"');
                        cashTr = JsonConvert.DeserializeObject<cashTransfer>(
                            Object,
                            new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                        );
                    }
                    else if (c.Type == "shippingCompanyId")
                    {
                        shippingCompanyId = long.Parse(c.Value);
                    }
                }
                #endregion
                if (cashTr != null)
                {
                    try
                    {
                        decimal basicAmount = amount;
                        using (incposdbEntities entity = new incposdbEntities())
                        {
                            var cashesList = entity.cashTransfer
                                .Where(
                                    x =>
                                        x.shippingCompanyId == shippingCompanyId
                                        && x.processType.Trim() == "deliver"
                                        && x.deserved > 0
                                )
                                .ToList();

                            foreach (var cash in cashesList)
                            {
                                var statusObj = entity.orderPreparingStatus
                                    .Where(
                                        x =>
                                            x.orderPreparing.invoiceId == cash.invId
                                            && x.status == "Done"
                                    )
                                    .FirstOrDefault();

                                if (statusObj != null)
                                {
                                    decimal paid = 0;
                                    if (amount >= cash.deserved)
                                    {
                                        paid = (decimal)cash.deserved;
                                        amount -= (decimal)cash.deserved;
                                        cash.paid += cash.deserved;
                                        cash.deserved = 0;
                                        cash.isCommissionPaid = 1;
                                    }
                                    else
                                    {
                                        paid = amount;
                                        cash.paid = cash.paid + amount;
                                        cash.deserved -= amount;
                                        amount = 0;
                                    }

                                    cashTr.cash = paid;
                                    cashTr.cashTransIdSource = cash.cashTransId;
                                    cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                    cashTr.updateUserId = cashTr.createUserId;
                                    cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                    entity.cashTransfer.Add(cashTr);

                                    entity.SaveChanges();
                                }
                            }
                            if (amount > 0)
                            {
                                cashTr.cash = amount;
                                cashTr.cashTransIdSource = null;
                                cashTr.createDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateDate = coctrlr.AddOffsetTodate(DateTime.Now);
                                cashTr.updateUserId = cashTr.createUserId;
                                cashTr.transNum = await generateCashNumber(cashTr.transNum);
                                entity.cashTransfer.Add(cashTr);

                                entity.SaveChanges();
                            }
                            decreaseShippingComBalance(shippingCompanyId, basicAmount);

                            return TokenManager.GenerateToken("1");
                        }
                    }
                    catch
                    {
                        return TokenManager.GenerateToken("-1");
                    }
                }
                else
                {
                    return TokenManager.GenerateToken("-1");
                }
            }
        }

        [Route("getNotPaidAgentCommission")]
        [HttpPost]
        public string getNotPaidAgentCommission(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
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
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var cashes = (
                        from b in entity.cashTransfer.Where(
                            x =>
                                x.userId == userId
                                && x.processType == "commissionAgent"
                                && x.deserved > 0
                        )
                        select new CashTransferModel()
                        {
                            cashTransId = b.cashTransId,
                            invId = b.invId,
                            agentId = b.agentId,
                            cash = b.cash,
                            deserved = b.deserved,
                            paid = b.paid,
                            notes = b.notes,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            shippingCompanyId = b.shippingCompanyId,
                            transNum = b.transNum,
                            isConfirm = 0,
                            isConfirm2 = 0,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(cashes);
                }
            }
        }

        [Route("getNotPaidShortage")]
        [HttpPost]
        public string getNotPaidShortage(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
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
                    if (c.Type == "userId")
                    {
                        userId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    var cashes = (
                        from b in entity.cashTransfer.Where(
                            x =>
                                x.userId == userId
                                && (
                                    x.processType.Trim() == "destroy"
                                    || x.processType.Trim() == "shortage"
                                )
                                && x.deserved > 0
                        )
                        select new CashTransferModel()
                        {
                            cashTransId = b.cashTransId,
                            invId = b.invId,
                            agentId = b.agentId,
                            cash = b.cash,
                            deserved = b.deserved,
                            paid = b.paid,
                            notes = b.notes,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            shippingCompanyId = b.shippingCompanyId,
                            transNum = b.transNum,
                            isConfirm = 0,
                            isConfirm2 = 0,
                        }
                    ).ToList();

                    return TokenManager.GenerateToken(cashes);
                }
            }
        }

        [Route("getNotPaidDeliverCashes")]
        [HttpPost]
        public string getNotPaidDeliverCashes(string token)
        {
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                long shippingComId = 0;
                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "shippingComId")
                    {
                        shippingComId = long.Parse(c.Value);
                    }
                }
                using (incposdbEntities entity = new incposdbEntities())
                {
                    List<CashTransferModel> res = new List<CashTransferModel>();
                    var cashes = (
                        from b in entity.cashTransfer.Where(
                            x =>
                                x.shippingCompanyId == shippingComId
                                && x.processType.Trim() == "deliver"
                                && x.deserved > 0
                        )
                        select new CashTransferModel()
                        {
                            cashTransId = b.cashTransId,
                            invId = b.invId,
                            agentId = b.agentId,
                            cash = b.cash,
                            deserved = b.deserved,
                            paid = b.paid,
                            notes = b.notes,
                            createUserId = b.createUserId,
                            updateDate = b.updateDate,
                            updateUserId = b.updateUserId,
                            shippingCompanyId = b.shippingCompanyId,
                            transNum = b.transNum,
                            isConfirm = 0,
                            isConfirm2 = 0,
                        }
                    ).ToList();

                    foreach (var cash in cashes)
                    {
                        var statusObj = entity.orderPreparingStatus
                            .Where(
                                x => x.orderPreparing.invoiceId == cash.invId && x.status == "Done"
                            )
                            .FirstOrDefault();
                        if (statusObj != null)
                        {
                            res.Add(cash);
                        }
                    }
                    return TokenManager.GenerateToken(res);
                }
            }
        }
    }
}
