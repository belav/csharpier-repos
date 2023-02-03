using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using POS_Server.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace POS_Server.Models.VM
{
    public class TokenManager
    {
        public static string Secret = "EREMN05OPLoDvbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHlSXKP+oi2+bXr6CUYTR==";
        private static int blackListSize = 10000;//

        public static string GenerateToken(object res)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                            (securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            var nbf = DateTime.UtcNow.AddSeconds(-1);
            var exp = DateTime.UtcNow.AddSeconds(120);
            var payload = new JwtPayload(null, "", new List<Claim>(), nbf, exp);
            payload.Add("scopes", res);
            //
            var token = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var jwtToken =  handler.WriteToken(token);
            // Token to String so you can use it in your client
            return EncryptThenCompress(jwtToken);
        }
        public static IEnumerable<Claim> getTokenClaims(string token)
        {
            var decryptedtToken = DeCompressThenDecrypt(token);

            var jwtToken = new JwtSecurityToken(decryptedtToken);
            //var s = jwtToken.Claims.ToArray();
            IEnumerable<Claim> claims = jwtToken.Claims;
            return claims;
        }
        public static string GetPrincipal(string token)
        {
            try
            {
                token = DeCompressThenDecrypt(token);
                var symmetricKey = Convert.FromBase64String(Secret);
                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var handler = new JwtSecurityTokenHandler();
                handler.InboundClaimTypeMap.Clear();

                SecurityToken securityToken;
                var jwtToken = new JwtSecurityToken(token);
                IEnumerable<Claim> claims = jwtToken.Claims;

                string requestToken = claims.Where(x => x.Type == "requestToken").Select(x => x.Value).FirstOrDefault();

                var principal = handler.ValidateToken(token, validationParameters, out securityToken);
                if (principal == null)
                {
                    //remove request token from black list
                    removeTokenFromBlackList(requestToken);

                    return "-7"; // no authorization
                }

                
                // check if any user logged in with same user account 
                string userLogInID = claims.Where(x => x.Type == "userLogInID").Select(x => x.Value).FirstOrDefault();
                long logInID = 0;
                if (userLogInID != null && userLogInID != "")
                    logInID = long.Parse(userLogInID);
                if (logInID != 0)
                {
                    UsersLogsController uc = new UsersLogsController();
                   bool isLoggedOut =  uc.checkLogByID(logInID);
                    if (isLoggedOut == true)
                        return "-8"; // log out
                }

                #region confirm request token
                bool requestDone = requestExecuted(requestToken);

                if (requestDone)// request already executed 
                    return "-9";

                #endregion

                return "0"; // every thing is OK
            }

            catch (Exception ex)
            {
                return null;
            }
        }
        #region encryption & decryption
        public static string Encrypt(string Text)
        {
            byte[] b = ConvertToBytes(Text);
            b = Encrypt(b);
            return ConvertToText(b);
        }
        private static byte[] ConvertToBytes(string text)
        {
            return System.Text.Encoding.Unicode.GetBytes(text);

        }
        public static byte[] Encrypt(byte[] ordinary)
        {
            BitArray bits = ToBits(ordinary);
            BitArray LHH = SubBits(bits, 0, bits.Length / 2);
            BitArray RHH = SubBits(bits, bits.Length / 2, bits.Length / 2);
            BitArray XorH = LHH.Xor(RHH);
            RHH = RHH.Not();
            XorH = XorH.Not();
            BitArray encr = ConcateBits(XorH, RHH);
            byte[] b = new byte[encr.Length / 8];
            encr.CopyTo(b, 0);
            return b;
        }
        public static string Decrypt(string EncryptedText)
        {
            byte[] b = ConvertToBytes(EncryptedText);
            b = Decrypt(b);
            return ConvertToText(b);
        }
        public static byte[] Decrypt(byte[] Encrypted)
        {
            BitArray enc = ToBits(Encrypted);
            BitArray XorH = SubBits(enc, 0, enc.Length / 2);
            XorH = XorH.Not();
            BitArray RHH = SubBits(enc, enc.Length / 2, enc.Length / 2);
            RHH = RHH.Not();
            BitArray LHH = XorH.Xor(RHH);
            BitArray bits = ConcateBits(LHH, RHH);
            byte[] decr = new byte[bits.Length / 8];
            bits.CopyTo(decr, 0);
            return decr;
        }
        private static string ConvertToText(byte[] ByteAarry)
        {
            return System.Text.Encoding.Unicode.GetString(ByteAarry);
        }
        private static BitArray ToBits(byte[] Bytes)
        {
            BitArray bits = new BitArray(Bytes);
            return bits;
        }
        private static BitArray SubBits(BitArray Bits, int Start, int Length)
        {
            BitArray half = new BitArray(Length);
            for (int i = 0; i < half.Length; i++)
                half[i] = Bits[i + Start];
            return half;
        }
        private static BitArray ConcateBits(BitArray LHH, BitArray RHH)
        {
            BitArray bits = new BitArray(LHH.Length + RHH.Length);
            for (int i = 0; i < LHH.Length; i++)
                bits[i] = LHH[i];
            for (int i = 0; i < RHH.Length; i++)
                bits[i + LHH.Length] = RHH[i];
            return bits;
        }
        #endregion

        #region Compress & Decompress
        public static byte[] Compress(byte[] bytData)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                Stream s = new GZipStream(ms, CompressionMode.Compress,true);
                s.Write(bytData, 0, bytData.Length);
                s.Close();
                byte[] compressedData = (byte[])ms.ToArray();
                return compressedData;

            }
            catch
            {
                return null;
            }
        }
        public static byte[] DeCompress(byte[] bytInput)
        {
            string strResult = "";
            int totalLength = 0;
            byte[] writeData = new byte[100000];

            Stream s2 = new GZipStream(new MemoryStream(bytInput), CompressionMode.Decompress);
            //try
            //{
            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    totalLength += size;
                    strResult += System.Text.Encoding.Unicode.GetString(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            return Encoding.Unicode.GetBytes(strResult);
            //}
            //catch
            //{
            //    return null;
            //}
        }
        #endregion
        #region reverse string
        static string ReverseString(string str)
        {
            int Length;
            string reversestring = "";
            Length = str.Length - 1;
            while (Length >= 0)
            {
                reversestring = reversestring + str[Length];
                Length--;
            }
            return reversestring;
        }
        #endregion
        public static string EncryptThenCompress(string text)
        {
            text = HttpUtility.UrlDecode(text);
            string str1 = Encrypt(text);
            //var bytes = Encoding.Unicode.GetBytes(str1);
            //byte[] bytes1 = Compress(bytes);
            //string str2 = Encoding.Unicode.GetString(bytes1);
            //var str2 = ReverseString(str1);
            var bytes = Encoding.UTF8.GetBytes(str1);
            return (Encoding.UTF8.GetString(bytes));
        }

        public static string DeCompressThenDecrypt(string text)
        {
            text = HttpUtility.UrlDecode(text);
            var bytes = Encoding.UTF8.GetBytes(text);
            text = Encoding.UTF8.GetString(bytes);
            // string reversedStr = ReverseString(text);           
            // var bytes = Encoding.Unicode.GetBytes(text);
            //var bytes1 = DeCompress(bytes);

            //string str = Encoding.Unicode.GetString(bytes1);
            // return str;
            return (Decrypt(text));
        }
        public static string readToken(HttpRequest Request)
        {
            var filePath = "";
            int fileNum = 0;
            string fileName = "";
            string text = "";
            Random rnd = new Random();
            foreach (string file in Request.Files)
            {
                var postedFile = Request.Files[file];

                fileNum = rnd.Next();

                while (true)
                {
                    fileName = "tmp" + fileNum.ToString() + ".txt";
                    var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~\\tmpFiles"), fileName);
                    if (files.Length > 0)
                    {
                        fileNum = rnd.Next();
                    }
                    else
                    {
                        filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~\\tmpFiles"), fileName);
                        try
                        {
                            postedFile.SaveAs(filePath);
                            text = File.ReadAllText(filePath);
                            try
                            {
                                File.Delete(filePath);
                            }
                            catch { }
                        }
                        catch
                        {
                            text = readToken(Request);
                            if (text != "")
                                break;

                        }
                        if (text != "")
                            break;
                    }
                }

            }


            return text;
        }
        // public static string readToken(HttpRequest Request)
        //{  
        //    var filePath = "";
        //    int fileNum = 0;
        //    string fileName = "";
        //    Random rnd = new Random();
        //    foreach (string file in Request.Files)
        //    {
        //        var postedFile = Request.Files[file];
        //        fileNum = rnd.Next();
        //        while (true)
        //        {
        //            fileName = "tmp" + fileNum.ToString() + ".txt";
        //            var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~\\tmpFiles"), fileName);
        //            if (files.Length > 0)
        //            {
        //                fileNum = rnd.Next();
        //            }
        //            else
        //            {
        //                filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~\\tmpFiles"), fileName);
        //                postedFile.SaveAs(filePath);
        //                break;
        //            }
        //        }

        //    }

        //   string text = File.ReadAllText(filePath);
        //    try
        //    {
        //        File.Delete(filePath);
        //    }
        //    catch { }
        //    return text;
        //}
        public static void deleteDirectoryFiles()
        {
            // Delete all files in a directory   
            string rootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~\\tmpFiles");
            string[] files = System.IO.Directory.GetFiles(rootFolder, "*.txt");
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch { }
            }
        }
        public static void deleteDirectoryFiles(string rootFolder)
        {
            // Delete all files in a directory   

            string[] files = System.IO.Directory.GetFiles(rootFolder, "*.txt");
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch { }
            }
        }

        private static void removeTokenFromBlackList(string requestToken)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var token = entity.TokensTable.Where(x => x.token == requestToken).FirstOrDefault();
                if (token != null)
                {
                    entity.TokensTable.Remove(token);
                    entity.SaveChanges();
                }
            }
        }


        private static bool requestExecuted(string requestToken)
        {
            using (incposdbEntities entity = new incposdbEntities())
            {
                var token = entity.TokensTable.Where(x => x.token == requestToken).FirstOrDefault();

                if (token != null)
                {
                    return true;
                }
                else
                {
                    int count = entity.TokensTable.Count();
                    if (count > blackListSize)
                    {
                        var lastN = entity.TokensTable
                       .OrderBy(x => x.createDate).Take(count - blackListSize + blackListSize / 2);
                        try
                        {
                            entity.TokensTable.RemoveRange(lastN);
                        }
                        catch { }

                    }

                    var newToken = new TokensTable()
                    {

                        token = requestToken,
                        createDate = DateTime.Now,
                    };
                    entity.TokensTable.Add(newToken);
                    entity.SaveChanges();
                    return false;
                }
            }

        }

    }
}
