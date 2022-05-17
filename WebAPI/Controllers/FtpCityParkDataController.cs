using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Rent;
using Domain.TB;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;
using System.IO;
using System.Text;
using WebAPI.Utils;
using System.Net;
using System.Threading;
using System.Configuration;
using WebAPI.Models.Param.Output.PartOfParam;
using NLog;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 城市車旅Ftp資料寫入 TB_CityParkingChk
    /// </summary>
    public class FtpCityParkDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        //FTP路徑
        static private string Url = @"ftp://13.67.53.117/CityParkingStation/";
        //儲存檔案路徑
        static private string savePath = @"D:\";
        //日期轉換
        static private string  DOCdate= "CityPark_"+ DateTime.Now.ToString("yyyyMMdd")+".txt";
        //FTP路徑+檔案位置
        //static private string tarUrl = Url + DOCdate;
        static private string tarUrl = Url + "CityPark_20220505.txt";
        //儲存路徑+檔案位置
        static private string SaveFilePath = savePath + DOCdate;

        //ftp用戶名
        static private string USERID = "ctbcvmftp";
        //ftp密碼
        static private string PASSWORD = "H3eVpnDav2sH";

        [HttpPost]
        public Dictionary<string, object> DoFtpCityParkData(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "FtpCityParkDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            IAPI_FtpCityParkData apiInput = null;
            OAPI_FtpCityParkData apiOutput = null;
            #endregion

            try
            {
                #region 防呆
                if (flag)
                {
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog("no Input", ClientIP, funName, ref errCode, ref LogID);


                }
                #endregion

                #region TB
                
                if (flag)
                {
                    bool exist = CheckFtpFile(tarUrl, USERID, PASSWORD);
                    if (exist == false)
                    {
                        flag = false;
                        errCode = "ERR911";
                    }
                    else
                    {
                        //List<FtpCityParkData> ftpCityParkDataList = GetFileList();
                        string DataList = GetFileStr(tarUrl, USERID, PASSWORD);
                        List<FtpCityParkData> ftpCityParkDataList = new List<FtpCityParkData>();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] resultTxt = DataList.Split(stringSeparators, StringSplitOptions.None);
                        //string[] resultTxt = DataList.ToString().Split('\n');
                        foreach (string d in resultTxt)
                        {
                            string[] lines = d.Split(',');
                            FtpCityParkData cityParkData = new FtpCityParkData();
                            cityParkData.Ftp_facility_id = lines[0];
                            cityParkData.Ftp_entrance_uuid = lines[1];
                            cityParkData.Ftp_license_plate_number = lines[2];
                            cityParkData.Ftp_entered_at = lines[3];
                            cityParkData.Ftp_entrance_id = lines[4];
                            cityParkData.Ftp_left_at = lines[5];
                            cityParkData.Ftp_exit_id = lines[6];
                            cityParkData.Ftp_amount = lines[7];

                            ftpCityParkDataList.Add(cityParkData);
                        }
                        try
                        {
                            string SPName = "usp_FtpCityParkData_I01";

                            object[] objparms = new object[ftpCityParkDataList.Count == 0 ? 1 : ftpCityParkDataList.Count];

                            if (ftpCityParkDataList.Count > 0)
                            {
                                for (int i = 0; i < ftpCityParkDataList.Count; i++)
                                {
                                    objparms[i] = new
                                    {
                                        Ftp_facility_id = ftpCityParkDataList[i].Ftp_facility_id,
                                        Ftp_entrance_uuid = ftpCityParkDataList[i].Ftp_entrance_uuid,
                                        Ftp_license_plate_number = ftpCityParkDataList[i].Ftp_license_plate_number,
                                        Ftp_entered_at = ftpCityParkDataList[i].Ftp_entered_at,
                                        Ftp_entrance_id = ftpCityParkDataList[i].Ftp_entrance_id,
                                        Ftp_left_at = ftpCityParkDataList[i].Ftp_left_at,
                                        Ftp_exit_id = ftpCityParkDataList[i].Ftp_exit_id,
                                        Ftp_amount = ftpCityParkDataList[i].Ftp_amount
                                    };
                                }
                            }
                            else
                            {
                                objparms[0] = new
                                {
                                    Ftp_facility_id = 0,
                                    Ftp_entrance_uuid = 0,
                                    Ftp_license_plate_number = 0,
                                    Ftp_entered_at = 0,
                                    Ftp_entrance_id = 0,
                                    Ftp_left_at = 0,
                                    Ftp_exit_id = 0,
                                    Ftp_amount = 0
                                };
                            }
                            object[][] parms1 = {
                                new object[] {
                                    LogID,
                                    funName
                                },
                                objparms
                            };

                            DataSet ds1 = new DataSet();
                            string returnMessage = "";
                            string messageLevel = "";
                            string messageType = "";

                            ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                            if (ds1.Tables.Count == 0)
                            {
                                flag = false;
                                errCode = "ERR999";
                                errMsg = returnMessage;
                            }
                            else
                            {
                                baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[1].Rows[0]["Error"]), ds1.Tables[1].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                            }
                            trace.traceAdd("FtpCityParkData_SaveObject", parms1);
                            trace.traceAdd("usp_FtpCityParkData_I01", new { flag, errCode });


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: " + e.Message);
                        }
                        finally
                        {
                            Console.WriteLine("Executing finally block.");
                        }
                    }
                }

                #endregion

                #region 寫入錯誤Log
                if (false == flag && false == isWriteError)
                {
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                }
                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                trace.BaseMsg = ex.Message;
            }

            #region TraceLog
            //if (string.IsNullOrWhiteSpace(trace.BaseMsg))
            //{
            //    if (flag)
            //        carRepo.AddTraceLog(84, funName, eumTraceType.mark, trace);
            //    else
            //        carRepo.AddTraceLog(84, funName, eumTraceType.followErr, trace);
            //}
            //else
            //    carRepo.AddTraceLog(84, funName, eumTraceType.exception, trace);
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        /// <summary>
        /// 檢查FTP檔案
        /// </summary>
        /// <param name="tarUrl"></param>
        /// <param name="USERID"></param>
        /// <param name="PASSWORD"></param>
        /// <returns></returns>
        public static bool CheckFtpFile(string tarUrl, string USERID, string PASSWORD)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(tarUrl);
            request.Credentials = new NetworkCredential(USERID, PASSWORD);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 從ftp伺服器上獲取檔案並將內容全部轉換成List返回
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns> 
        public static List<FtpCityParkData> GetFileList()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                List<FtpCityParkData> FtpCityParkDatas = new List<FtpCityParkData>();
                StreamReader reader = new StreamReader(SaveFilePath);
                string line = reader.ReadLine();

                while (line != null)
                {
                    txt.Append(line);
                    txt.Append("\n");
                    line = reader.ReadLine();
                }

                txt.Remove(txt.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                string[] resultTxt = txt.ToString().Split('\n');
                List<FtpCityParkData> cityParkDatas = new List<FtpCityParkData>();
                foreach (var d in resultTxt)
                {
                    string[] lines = d.Split(',');
                    FtpCityParkData cityParkData = new FtpCityParkData();
                    cityParkData.Ftp_facility_id = lines[0];
                    cityParkData.Ftp_entrance_uuid = lines[1];
                    cityParkData.Ftp_license_plate_number = lines[2];
                    cityParkData.Ftp_entered_at = lines[3];
                    cityParkData.Ftp_entrance_id = lines[4];
                    cityParkData.Ftp_left_at = lines[5];
                    cityParkData.Ftp_exit_id = lines[6];
                    cityParkData.Ftp_amount = lines[7];

                    cityParkDatas.Add(cityParkData);
                }

                return cityParkDatas;


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// 下載FTP檔案
        /// </summary>
        /// <returns></returns>
        public static bool downloadFtpFile()
        {
            bool result = true;
            try
            {
                FileStream newFile = new FileStream(SaveFilePath, FileMode.Create);
                byte[] downloadedData = download(tarUrl, USERID, PASSWORD);
                newFile.Write(downloadedData, 0, downloadedData.Length);
                newFile.Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static byte[] download(string tarUrl, string USERID, string PASSWORD)
        {
            byte[] buffer = new byte[1024];
            byte[] downloadedData = null;
            try
            {
                FtpWebRequest ftpReq = (FtpWebRequest)FtpWebRequest.Create(tarUrl);
                ftpReq.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpReq.Credentials = new NetworkCredential(USERID, PASSWORD);
                FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();
                Stream reader = response.GetResponseStream();
                MemoryStream memStream = new MemoryStream();

                while (true)
                {
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;      //已經沒有資料, 結束下載
                    }
                    else
                    {
                        //資料寫入
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
                downloadedData = memStream.ToArray();
                memStream.Dispose();
                reader.Dispose();
                return downloadedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return downloadedData; //基本上不會跑到這
        }

        /// <summary>
        /// 從ftp伺服器上獲取檔案並將內容全部轉換成string返回
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dir"></param>
        /// <returns></returns> 
        public static string GetFileStr(string tarUrl, string USERID, string PASSWORD)
        {
            FtpWebRequest reqFTP;
            StringBuilder txt = new StringBuilder();
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(tarUrl);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.Credentials = new NetworkCredential(USERID, PASSWORD);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(ftpStream);
                string fileStr = reader.ReadToEnd();

                reader.Close();
                ftpStream.Close();
                response.Close();
                return fileStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("獲取ftp檔案並讀取內容失敗︰" + ex.Message);
                return null;
            }
        }

    }
}