using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
//using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using POS_Server.Controllers;

namespace POS_Server.Models
{
    public class APIResult
    {
        //public string Message { get; set; }
        //public string Status { get; set; }
        public static string APIUri = "https://141.95.1.58:44730/api/";

        //  public static string APIUri = "https://localhost:443/api/";

        //var pathCheck = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp"), );
        private static string Secret =
            "EREMN05OPLoDvbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHlSXKP+oi2+bXr6CUYTR==";

        public static async Task<IEnumerable<Claim>> getList(
            string method,
            Dictionary<string, string> parameters = null
        )
        {
            #region generate token to send it to api
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature
            );

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            var nbf = DateTime.UtcNow.AddSeconds(-1);
            var exp = DateTime.UtcNow.AddSeconds(120);
            var payload = new JwtPayload(null, "", new List<Claim>(), nbf, exp);

            if (parameters != null)
                for (int i = 0; i < parameters.Count; i++)
                {
                    payload.Add(parameters.Keys.ToList()[i], parameters.Values.ToList()[i]);
                }
            // add userLogInID to parameters
            //  payload.Add("userLogInID", MainWindow.userLogInID);

            var token = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use post it to api
            string getToken = handler.WriteToken(token);
            var encryptedToken = EncryptThenCompress(getToken);

            //encryptedToken = HttpUtility.UrlPathEncode(encryptedToken);

            string tmpPath = writeToTmpFile(encryptedToken);
            //string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            //string tmpPath = Path.Combine(dir, Global.TMPFolder);
            // tmpPath = Path.Combine(tmpPath, "tmp.txt");
            FileStream fs = new FileStream(tmpPath, FileMode.Open, FileAccess.Read);
            #endregion
            ServicePointManager.ServerCertificateValidationCallback += (
                sender,
                cert,
                chain,
                sslPolicyErrors
            ) => true;
            ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIUri);
                client.Timeout = System.TimeSpan.FromSeconds(3600);
                string boundary = string.Format(
                    "----WebKitFormBoundary{0}",
                    DateTime.Now.Ticks.ToString("x")
                );
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StreamContent(fs);

                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    FileName = "tmp.txt"
                };
                content.Headers.Add("client", "true");

                form.Add(content, "fileToUpload");

                var response = await client.PostAsync(@method + "?token=" + "null", form);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var Sresponse = JsonConvert.DeserializeObject<string>(jsonString);
                    try
                    {
                        fs.Dispose();
                        File.Delete(tmpPath);
                    }
                    catch { }
                    if (Sresponse != "")
                    {
                        var decryptedToken = DeCompressThenDecrypt(Sresponse);
                        var jwtToken = new JwtSecurityToken(decryptedToken);
                        var s = jwtToken.Claims.ToArray();
                        IEnumerable<Claim> claims = jwtToken.Claims;
                        string validAuth = claims
                            .Where(f => f.Type == "scopes")
                            .Select(x => x.Value)
                            .FirstOrDefault();
                        if (validAuth != null && s[2].Value == "-7") // invalid authintication
                            return null;
                        else
                            // MainWindow.go_out = true;
                            return claims;
                    }
                }
                else
                {
                    try
                    {
                        fs.Dispose();
                        File.Delete(tmpPath);
                    }
                    catch { }
                }
            }
            return null;
        }

        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                bool inuse = false;

                inuse = IsFileInUse(fileName);
                if (inuse == false)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch { }
                }
            }
        }

        private static bool IsFileInUse(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                //throw new ArgumentException("'path' cannot be null or empty.", "path");
                return true;
            }

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite)) { }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static string writeToTmpFile(string text)
        {
            string tmpPath = System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp");

            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
            else
            {
                ProcessDirectory(tmpPath);
            }
            //check if file in use
            string fileName = "";
            //int fileNum = 0;
            string filePath = "";
            Random rnd = new Random();
            int fileNum = rnd.Next();
            while (true)
            {
                fileName = "tmp" + fileNum.ToString() + ".txt";
                filePath = Path.Combine(tmpPath, fileName);
                if (File.Exists(filePath))
                {
                    fileNum++;
                }
                else
                {
                    File.WriteAllText(@filePath, text);
                    break;
                }
            }
            //tmpPath = Path.Combine(tmpPath, "tmp.txt");

            return filePath;
        }

        public static async Task<int> post(string method, Dictionary<string, string> parameters)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature
            );

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            var nbf = DateTime.UtcNow.AddSeconds(-1);
            var exp = DateTime.UtcNow.AddSeconds(120);
            var payload = new JwtPayload(null, "", new List<Claim>(), nbf, exp);

            for (int i = 0; i < parameters.Count; i++)
            {
                payload.Add(parameters.Keys.ToList()[i], parameters.Values.ToList()[i]);
            }
            // add userLogInID to parameters
            //   payload.Add("userLogInID", MainWindow.userLogInID);
            //
            var token = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use post it to api
            string postToken = handler.WriteToken(token);
            var encryptedToken = EncryptThenCompress(postToken);
            string tmpPath = writeToTmpFile(encryptedToken);
            //string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            //string tmpPath = Path.Combine(dir, Global.TMPFolder);
            //tmpPath = Path.Combine(tmpPath, "tmp.txt");
            FileStream fs = new FileStream(tmpPath, FileMode.Open, FileAccess.Read);

            ServicePointManager.ServerCertificateValidationCallback += (
                sender,
                cert,
                chain,
                sslPolicyErrors
            ) => true;
            ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIUri);
                client.Timeout = System.TimeSpan.FromSeconds(3600);
                string boundary = string.Format(
                    "----WebKitFormBoundary{0}",
                    DateTime.Now.Ticks.ToString("x")
                );
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StreamContent(fs);

                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    FileName = "tmp.txt"
                };
                content.Headers.Add("client", "true");

                form.Add(content, "fileToUpload");

                var response = await client.PostAsync(@method + "?token=" + "null", form);

                try
                {
                    fs.Dispose();
                    File.Delete(tmpPath);
                }
                catch { }
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var Sresponse = JsonConvert.DeserializeObject<string>(jsonString);
                    if (Sresponse != "")
                    {
                        var decryptedToken = DeCompressThenDecrypt(Sresponse);
                        var jwtToken = new JwtSecurityToken(decryptedToken);
                        var s = jwtToken.Claims.ToArray();
                        IEnumerable<Claim> claims = jwtToken.Claims;

                        string validAuth = claims
                            .Where(f => f.Type == "scopes")
                            .Select(x => x.Value)
                            .FirstOrDefault();
                        if (validAuth != null && s[2].Value == "-8")
                        { // MainWindow.go_out = true;
                        }

                        foreach (Claim c in claims)
                        {
                            if (c.Type == "scopes")
                            {
                                return int.Parse(c.Value);
                            }
                        }
                    }
                }
            }
            return 0;
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
                Stream s = new GZipStream(ms, CompressionMode.Compress);
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
            //string strResult = "";
            //int totalLength = 0;
            //byte[] writeData = new byte[100000];

            //Stream s2 = new GZipStream(new MemoryStream(bytInput), CompressionMode.Decompress,true);

            ////try
            ////{
            //    while (true)
            //{
            //    int size = s2.Read(writeData, 0, writeData.Length);
            //    if (size > 0)
            //    {
            //        totalLength += size;
            //        strResult += System.Text.Encoding.Unicode.GetString(writeData, 0, size);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            //s2.Close();
            //return Encoding.Unicode.GetBytes(strResult);
            Stream ms = new MemoryStream(bytInput);
            Stream decodedStream = new MemoryStream();
            byte[] buffer = new byte[4096];

            using (Stream inGzipStream = new GZipStream(ms, CompressionMode.Decompress, true))
            {
                int bytesRead;
                while ((bytesRead = inGzipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    decodedStream.Write(buffer, 0, bytesRead);
                }
                using (var memoryStream = new MemoryStream())
                {
                    decodedStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            //}
            //    catch
            //    {
            //        return null;
            //    }
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
            string str1 = Encrypt(text);
            //var bytes = Encoding.Unicode.GetBytes(text);

            //string str2 = Compress(text);
            //string str2 = Encoding.Unicode.GetString(bytes1);
            //return str2;
            //var str2 = ReverseString(str1);
            var bytes = Encoding.UTF8.GetBytes(str1);
            return (Encoding.UTF8.GetString(bytes));
        }

        public static string DeCompressThenDecrypt(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            text = Encoding.UTF8.GetString(bytes);
            //string reversedStr = ReverseString(text);
            //var bytes = Encoding.Unicode.GetBytes(reversedStr);
            //var bytes1 = DeCompress(bytes);
            //string str = Encoding.Unicode.GetString(bytes1);
            return (Decrypt(text));
        }
    }
}
