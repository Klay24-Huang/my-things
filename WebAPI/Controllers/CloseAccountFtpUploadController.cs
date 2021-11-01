using System.Linq;
using System.Net;
using System.Net.Http;

using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;

using Domain.TB.BackEnd;
using Reposotory.Implement;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Web.Hosting;
using FluentFTP;

using WebCommon;

namespace WebAPI.Controllers
{
    public class CloseAccountFtpUploadController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRentT"].ConnectionString;

        //public CloseAccountFtpUploadController()
        //{
        //}

        public string DoCloseAccountFtpUpload()
        {
            HttpContext httpContext = HttpContext.Current;
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            string OutputMSG = "";  //回覆狀況

            List<BE_GetMemberScoreFull> lstData = null;
            MemberRepository repository = new MemberRepository(connetStr);
            lstData = repository.GetMemberScoreFull_EXPORT("S125073792", "", "", "2021-06-07 12:08:15.467", "2021-08-07 12:08:15.467");
            int len = lstData.Count;

            try
            {
                var path = "TempEasyWalletFtpUpload";
                var dataTime = DateTime.Today.ToString("yyyyMMdd");
                var dataTime_yyyyMMddHHmmss = DateTime.Now.ToString("yyyyMMddHHmmss");
                var strUploadFileName = "z.txt";
                var FolderService = new FolderService();
                FolderService.RemoveOldFolder("~/" + path);
                FolderService.CreateFolder("~/" + path);
                FolderService.CreateFolder("~/" + path + "/" + dataTime + @"/");
                var ApplicationPath = HostingEnvironment.ApplicationPhysicalPath;
                // 檔案TXT寫入路徑
                var localFilePath = ApplicationPath + path + @"\\" + dataTime + @"\\" + strUploadFileName;

                if (!File.Exists(localFilePath))
                {
                    // 建立txt
                    using (StreamWriter sw = File.CreateText(localFilePath))
                    {
                        //sw.WriteLine(@"""identityId""" + "," + @"""endDate""" + "," + @"""orderNo""");
                    }
                }

                //寫入txt
                using (StreamWriter sw = File.AppendText(localFilePath))
                {
                    for (int i = 0; i < len; i++)
                    {
                        sw.WriteLine(lstData[i].DAD);
                    }

                    OutputMSG = "寫入成功！";
                }

                //using (FtpClient conn = new FtpClient())
                //{
                //    conn.Host = "ftp://epftp.easycard.com.tw";
                //    conn.Credentials = new NetworkCredential("ecc_irent", "ecc_irent");

                //    conn.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                //    conn.EncryptionMode = FtpEncryptionMode.Implicit;

                //    conn.Connect();
                //    conn.SetWorkingDirectory("/IN");
                //    conn.UploadFile(localFilePath, strUploadFileName);

                //    OutputMSG = "上傳成功！";
                //}
            
            }
            catch (Exception ex)
            {
                OutputMSG = "上傳失敗！"; //回傳訊息

            }


            return OutputMSG;
        }
    }

    public interface IFolderService
    {
        void CreateFolder(string folderName);
        void RemoveFolder(string folderName);
        void RemoveOldFolder(string folderName);
    }
    public class FolderService : IFolderService
    {
        public void CreateFolder(string folderName)
        {
            string ApplicationPath = HostingEnvironment.ApplicationPhysicalPath;
            //bool isExists = Directory.Exists(HttpContext.Current.Server.MapPath(folderName));
            string path = ApplicationPath + folderName.Replace("~/", "").Replace("/", "\\");
            bool isExists = Directory.Exists(path);

            //if (!isExists)
            //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folderName));
            if (!isExists)
                Directory.CreateDirectory(path);
        }
        public void RemoveFolder(string folderName)
        {
            string[] dirList = Directory.GetDirectories(folderName);
            foreach (var sDir in dirList)
            {
                RemoveFolder(sDir);
            }
            string[] fileList = Directory.GetFiles(folderName);
            foreach (var sFile in fileList)
            {
                File.Delete(sFile);
            }
            Directory.Delete(folderName);
        }
        public void RemoveOldFolder(string folderName)
        {
            if (Directory.Exists(HttpContext.Current.Server.MapPath(folderName)))
            {
                string[] dirList = Directory.GetDirectories(HttpContext.Current.Server.MapPath(folderName));
                foreach (var sDir in dirList)
                {
                    if (sDir.IndexOf(DateTime.Today.ToString("yyyyMMdd")) == -1)
                    {
                        RemoveFolder(sDir);
                    }
                }

            }
        }
    }
}
