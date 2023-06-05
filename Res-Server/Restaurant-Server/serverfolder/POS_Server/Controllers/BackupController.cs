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
using System.Data.SqlClient;
using System.IO;
using System.Collections;
using Newtonsoft.Json.Converters;
using POS_Server.Classes;

namespace POS_Server.Controllers
{
    [RoutePrefix("api/Backup")]
    public class BackupController : ApiController
    {
        CountriesController coctrlr = new CountriesController();

        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;
        private string sql = "";
        private string connectionstring = "";
        private string databaseName = "";
        private string backupFilename = "";
        private string restoreFilename = "";

        private void setConn()
        {
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[
                "incposdbEntities"
            ].ConnectionString;
            if (connectionstring.ToLower().StartsWith("metadata="))
            {
                System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder =
                    new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(
                        connectionstring
                    );
                connectionstring = efBuilder.ProviderConnectionString;
            }

            var connectionBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(
                connectionstring
            );

            databaseName = connectionBuilder.InitialCatalog;

            connection = new SqlConnection(connectionstring);
        }

        public string backupDB(string filePath)
        {
            int res = 0;
            setConn();
            connection.Open();

            sql = "BACKUP DATABASE " + databaseName + " TO DISK = '" + filePath + "'";

            command = new SqlCommand(sql, connection);

            res = command.ExecuteNonQuery();
            //providerName="System.Data.SqlClient"

            connection.Close();
            connection.Dispose();
            if (res == -1)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        public string restoreDB(string filePath)
        {
            int res = 0;
            setConn();
            connection.Open();
            sql = "USE master;";

            sql += "ALTER DATABASE " + databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
            sql +=
                "RESTORE DATABASE "
                + databaseName
                + "  FROM  DISK = '"
                + filePath
                + "' WITH REPLACE;";

            sql += " ALTER DATABASE " + databaseName + " SET Multi_User ;";

            command = new SqlCommand(sql, connection);
            res = command.ExecuteNonQuery();

            connection.Close();
            connection.Dispose();

            if (res == -1)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        // GET api/<controller> get all Group
        [HttpPost]
        [Route("getbackup")]
        public string getbackup(string token)
        {
            //  public string Get(string token)
            string message = "";
            token = TokenManager.readToken(HttpContext.Current.Request);
            var strP = TokenManager.GetPrincipal(token);
            if (strP != "0") //invalid authorization
            {
                return TokenManager.GenerateToken(strP);
            }
            else
            {
                string path = "";
                string filename = "back" + DateTime.Now.ToFileTime() + ".bak";
                //  string filename = "back.bak";

                path = Path.Combine(
                    System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\"),
                    filename
                );
                var files = Directory.GetFiles(
                    System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\"),
                    filename
                );
                if (files.Length > 0)
                {
                    File.Delete(files[0]);
                }

                message = backupDB(path);

                return TokenManager.GenerateToken(message);
            }
        }

        private string createBackup()
        {
            string message = "";

            string path = "";
            backupFilename = "back" + DateTime.Now.ToFileTime() + ".bak";
            //  string filename = "back.bak";
            string dirpath = System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\");
            path = Path.Combine(dirpath, backupFilename);
            var files = Directory.GetFiles(dirpath, backupFilename);
            if (files.Length > 0)
            {
                File.Delete(files[0]);
            }

            message = backupDB(path);

            return message;
        }

        [HttpPost]
        [Route("getrestore")]
        public string getrestore(string token)
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
                long logId = 0;

                UsersLogsController logcntrlr = new UsersLogsController();
                usersLogs logITEM = new usersLogs();
                BackupModel restoremodel = new BackupModel();

                IEnumerable<Claim> claims = TokenManager.getTokenClaims(token);
                foreach (Claim c in claims)
                {
                    if (c.Type == "logId")
                    {
                        logId = long.Parse(c.Value);
                    }
                    if (c.Type == "fileName")
                    {
                        restoreFilename = c.Value;
                    }
                }
                //get log row befor restore operation
                logITEM = logcntrlr.GetByID(logId);

                try
                {
                    //decode
                    string sourcpath = "";
                    string destpath = "";

                    bool decres = false;
                    string direpath = System.Web.Hosting.HostingEnvironment.MapPath(
                        "~\\images\\temp\\"
                    );
                    string destfile = "file" + DateTime.Now.ToFileTime() + ".bak";

                    sourcpath = Path.Combine(direpath, restoreFilename);
                    destpath = Path.Combine(direpath, destfile);
                    //  var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\"), filename);


                    if (File.Exists(sourcpath))
                    {
                        //decode file
                        //   decres=decodefile()
                        decres = decodefile(sourcpath, destpath);
                        // delete after decode
                        File.Delete(sourcpath);

                        if (File.Exists(destpath) && decres == true)
                        {
                            //restore
                            message = restoreDB(destpath);
                            // delete after restore
                            File.Delete(destpath);
                            if (message == "1")
                            {
                                logITEM.logId = 0;
                                //save newlog row and return the logId
                                logId = long.Parse(logcntrlr.Save(logITEM));
                            }
                        }
                        else
                        {
                            message = "0";
                        }
                    }
                    else
                    {
                        message = "0";
                    }

                    restoremodel.logId = logId;
                    restoremodel.result = message;
                    //restoremodel.fileName = filename;

                    return TokenManager.GenerateToken(restoremodel);
                }
                catch (Exception ex)
                {
                    restoremodel.logId = logId;
                    restoremodel.result = ex.ToString();
                    //  restoremodel.fileName = filename;
                    return TokenManager.GenerateToken(restoremodel);
                }
            }
        }

        [HttpGet]
        [Route("GetFile")]
        public HttpResponseMessage GetFile()
        {
            string message = "";
            bool encres = false;
            string dirpath = System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            ProcessDirectory(dirpath);

            message = createBackup();
            if (message == "1")
            {
                //encode file
                string filename = "back1" + DateTime.Now.ToFileTime() + ".inc";
                string destpath = Path.Combine(dirpath, filename);
                string sourcPath;

                sourcPath = Path.Combine(dirpath, backupFilename);
                encres = encodefile(sourcPath, destpath);

                // send file to client

                if (String.IsNullOrEmpty(filename))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                File.Delete(sourcPath);
                if (encres)
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(
                        new FileStream(destpath, FileMode.Open, FileAccess.Read)
                    );
                    response.Content.Headers.ContentDisposition =
                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = filename;

                    return response;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                bool inuse = false;

                inuse = IsFileInUse(fileName);
                if (inuse == false)
                {
                    File.Delete(fileName);
                }

                //ProcessFile(fileName);
            }
        }

        private bool IsFileInUse(string path)
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

        // upload file
        [Route("uploadfile")]
        public IHttpActionResult uploadfile()
        {
            try
            {
                string dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    restoreFilename = postedFile.FileName;
                    //   string imageWithNoExt = Path.GetFileNameWithoutExtension(postedFile.FileName);

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        //    int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB

                        IList<string> AllowedFileExtensions = new List<string> { ".bak", ".inc" };
                        //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = Path.GetExtension(postedFile.FileName);

                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = "-1";
                            return Ok(message);
                        }
                        var files = Directory.GetFiles(dirPath, restoreFilename);
                        if (files.Length > 0)
                        {
                            File.Delete(files[0]);
                        }

                        // myfolder name where i want to save my file
                        var filePath = Path.Combine(dirPath, restoreFilename);
                        postedFile.SaveAs(filePath);

                        // restore to DB

                        //}
                    }

                    // var message1 = string.Format("Image Updated Successfully.");
                    return Ok(string.Format("1"));
                }
                return Ok(string.Format("1"));
                //  var res = string.Format("Please Upload a image.");

                // return Ok("2");
            }
            catch (Exception ex)
            {
                var res = string.Format("-2");

                return Ok(res);
            }
        }

        public bool encodefile(string source, string dest)
        {
            try
            {
                //string source = System.IO.Path.Combine(@"D:/temp/", "backlocal2.bak");
                //string dest = System.IO.Path.Combine(@"D:/temp/", "backlocal3.bak");
                //end test
                //message = backupDB(path);//remove comment
                int firstpartLength = 100;
                int reverslength = 50;
                int injlength;
                byte[] result = System.Text.Encoding.ASCII.GetBytes("null");
                injlength = result.Length;
                //    byte[] arr = File.ReadAllBytes(path);
                byte[] arr = File.ReadAllBytes(source);
                byte[] arr1 = new byte[firstpartLength];
                Buffer.BlockCopy(arr, 0, arr1, 0, firstpartLength);
                byte[] arr2 = new byte[arr.Length - firstpartLength];
                Buffer.BlockCopy(arr, firstpartLength, arr2, 0, arr.Length - firstpartLength);
                byte[] arrtorvrs = new byte[reverslength];
                Buffer.BlockCopy(arr1, reverslength, arrtorvrs, 0, reverslength);

                arrtorvrs = arrtorvrs.Reverse().ToArray();
                Buffer.BlockCopy(arrtorvrs, 0, arr1, reverslength, reverslength);

                byte[] arr3 = new byte[arr1.Length + result.Length];
                arr3 = Combine(arr1, result);

                // string str2 = str.Substring(50, 50);

                arr = Combine(arr3, arr2);

                arr = arr.Reverse().ToArray();
                arr = Encrypt(arr);

                File.WriteAllBytes(dest, arr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool decodefile(string source, string dest)
        {
            try
            {
                //string source = System.IO.Path.Combine(@"D:/temp/", "backlocal3.bak");
                //string dest = System.IO.Path.Combine(@"D:/temp/", "backlocal4.bak");
                int firstpartLength = 100;
                int resfirstpartLength;
                int resreverslength = 50;
                int resinjlength;
                byte[] resresult = System.Text.Encoding.ASCII.GetBytes("null");
                resinjlength = resresult.Length;
                resfirstpartLength = firstpartLength + resinjlength; //100+4
                byte[] restorearr = File.ReadAllBytes(source);
                restorearr = Decrypt(restorearr);

                restorearr = restorearr.Reverse().ToArray();

                byte[] resarr1 = new byte[resfirstpartLength];
                int secondpartLength = restorearr.Length - resfirstpartLength;
                byte[] resarr2 = new byte[secondpartLength]; //120
                Buffer.BlockCopy(restorearr, 0, resarr1, 0, resfirstpartLength);

                Buffer.BlockCopy(restorearr, resfirstpartLength, resarr2, 0, secondpartLength);

                byte[] resreversarr = new byte[resreverslength];
                Buffer.BlockCopy(resarr1, resreverslength, resreversarr, 0, resreverslength);
                resreversarr = resreversarr.Reverse().ToArray();
                Buffer.BlockCopy(resreversarr, 0, resarr1, resreverslength, resreverslength);
                byte[] resarrnoinj = new byte[firstpartLength]; //120
                Buffer.BlockCopy(resarr1, 0, resarrnoinj, 0, firstpartLength);
                restorearr = Combine(resarrnoinj, resarr2);

                File.WriteAllBytes(dest, restorearr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Reverse(string text)
        {
            if (text == null)
                return null;

            // this was posted by petebob as well
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        //encript decript
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

        public string autoBackup()
        {
            try
            {
                Calculate calc = new Calculate();
                setValuesController setvcntrlr = new setValuesController();
                setValues setvaltime = new setValues();
                setValues setvalenable = new setValues();
                setvaltime = setvcntrlr.GetRowBySettingName("backup_time");
                setvalenable = setvcntrlr.GetRowBySettingName("backup_daily_enabled");

                DateTime backupdate = DateTime.Now;
                DateTime datenow = coctrlr.AddOffsetTodate(DateTime.Now);

                TimeSpan backuptime = new TimeSpan(datenow.Hour, datenow.Minute, datenow.Second);
                TimeSpan nowtime = new TimeSpan(datenow.Hour, datenow.Minute, datenow.Second);

                string message = "";
                bool encres = false;
                string dirpath = System.Web.Hosting.HostingEnvironment.MapPath("~\\images\\temp\\");
                string destDirpath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Backups\\");
                //DateTime fileCreatedDate = File.GetCreationTime(@"C:\Example\MyTest.txt");
                if (setvalenable == null || setvalenable.value == "" || setvalenable.value != "1")
                {
                    //not enabled
                }
                else
                {
                    if (setvaltime == null || setvaltime.value == "")
                    {
                        //default
                        backupdate = Convert.ToDateTime("2022-01-01 00:00:00");
                    }
                    else
                    {
                        backupdate = Convert.ToDateTime(setvaltime.value);
                    }
                    backuptime = new TimeSpan(
                        backupdate.Hour,
                        backupdate.Minute,
                        backupdate.Second
                    );
                    int period = (int)(nowtime.TotalMinutes - backuptime.TotalMinutes);
                    //check if the backup time is ready
                    double timerperiod = UsersLogsController.Repeattime / 60000; //convert to minute

                    if (period >= 0 && period < timerperiod) //=10 is timer period in minutes
                    {
                        //hetre is backup operation
                        if (!Directory.Exists(dirpath))
                        {
                            Directory.CreateDirectory(dirpath);
                        }
                        if (!Directory.Exists(destDirpath))
                        {
                            Directory.CreateDirectory(destDirpath);
                        }
                        //destination file
                        string destfilename =
                            "backup-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".inc";
                        var strDirectory = System.IO.Path.GetDirectoryName(destDirpath);
                        bool isCreated = false;
                        foreach (
                            System.IO.FileInfo file in new System.IO.DirectoryInfo(
                                strDirectory
                            ).GetFiles("*.*")
                        )
                        {
                            string filename = file.Name;
                            //get date of creation
                            if (file.Name.Length == 27)
                            {
                                filename = filename.Substring(7, 10); //"yyyy-MM-dd"

                                double updateperiod = (
                                    file.LastWriteTime - file.CreationTime
                                ).TotalSeconds;
                                if (calc.IsDateTime(filename))
                                {
                                    string datenowstring = datenow.Date.ToString("yyyy-MM-dd");

                                    if (
                                        (datenow.Date == file.CreationTime.Date) //  created today
                                        && updateperiod < 5 //not edited
                                        && updateperiod > 0 //not edited
                                        && (filename == datenowstring)
                                        && file.Extension == ".inc"
                                    ) // created today  and the extension is correct
                                    {
                                        isCreated = true;
                                        // break;
                                    }
                                }
                                else
                                {
                                    //delete other files not backup
                                    File.Delete(file.FullName);
                                }
                            }
                            else
                            {
                                File.Delete(file.FullName);
                            }
                        }
                        if (isCreated == false)
                        {
                            string destpath = Path.Combine(destDirpath, destfilename);
                            //end  destination file
                            ProcessDirectory(dirpath);

                            message = createBackup();
                            if (message == "1")
                            {
                                //encode file
                                string filename = "back1" + DateTime.Now.ToFileTime() + ".inc";

                                string sourcPath;

                                sourcPath = Path.Combine(dirpath, backupFilename);

                                encres = encodefile(sourcPath, destpath);
                                File.Delete(sourcPath);
                            }
                        }
                        //delet old 10 files
                        deletOldFiles(strDirectory, 10);

                        //end delete
                    }
                }
                return message;
            }
            catch
            {
                return "0";
            }
            /*
            backup - 2022 - 09 - 27 - 17 - 26.inc
filename = backup - 2022 - 09 - 27 - 17 - 26.inc
filename sub = 2022 - 09 - 27
file.Extension =.inc
*/
        }

        public void deletOldFiles(string strDirectory, int filesCount)
        {
            List<System.IO.FileInfo> flist = new System.IO.DirectoryInfo(strDirectory)
                .GetFiles("*.*")
                .OrderBy(f => f.CreationTime)
                .ToList();

            if (flist.Count() > filesCount)
            {
                List<System.IO.FileInfo> deleteList = flist.Take(filesCount).ToList();
                foreach (System.IO.FileInfo file in deleteList)
                {
                    string filename = file.Name;
                    DateTime dt = file.CreationTime;
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
