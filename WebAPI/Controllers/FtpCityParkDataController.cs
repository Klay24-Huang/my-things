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
using NLog;
using System.Globalization;
using System.Net.Security;
using FluentFTP;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 城市車旅Ftp資料寫入 TB_CityParkingChk
    /// </summary>
    public class FtpCityParkDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string FTPuserID = ConfigurationManager.AppSettings["FtpCityPark_UserID"].ToString();
        private readonly string FTPpassWord = ConfigurationManager.AppSettings["FtpCityPark_PassWord"].ToString();
        private readonly string IP_address = ConfigurationManager.AppSettings["FtpCityPark_IPaddress"].ToString();

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
            OAPI_FtpCityParkData apiOutput = null;

            string filePath = "/CityParkingStation/CityPark_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".txt";

            //string filePath = "/CityParkingStation/CityPark_20220505.txt";
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
                    //先檢查是FTP上否有資料
                    bool exist = CheckFtpFile(filePath);
                    if (exist == false)
                    {
                        flag = false;
                        errCode = "ERR911";
                    }
                    else
                    {
                        // 取得檔案&處理檔案文字
                        List<FtpCityParkData> ftpCityParkDataList = GetFileList(filePath);

                        if (ftpCityParkDataList.Count() > 0)
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
                        }
                        else
                        {
                            flag = false;
                            errCode = "ERR911";
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

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
        #region 檢查FTP檔案
        /// <summary>
        /// 檢查FTP檔案
        /// </summary>
        /// <param name="tarUrl"></param>
        /// <param name="USERID"></param>
        /// <param name="PASSWORD"></param>
        /// <returns></returns>
        private bool CheckFtpFile(string filePath)
        {
            try
            {
                FtpClient client = new FtpClient(IP_address);
                // 設定連線為FTPS 隱式模式
                client.EncryptionMode = FtpEncryptionMode.Implicit;
                client.Credentials = new NetworkCredential(FTPuserID, FTPpassWord);
                client.AutoConnect();
                bool checkFile = client.FileExists(filePath);
                client.Disconnect();
                return checkFile;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw ex;
            }
        }
        #endregion
        #region 檔案獲取及處理
        /// <summary>
        /// 從ftp伺服器上獲取檔案並處理傳回List
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dir"></param>
        /// <returns></returns> 
        private List<FtpCityParkData> GetFileList(string filePath)
        {
            FtpClient client = new FtpClient(IP_address);
            // 設定連線為FTPS 隱式模式
            client.EncryptionMode = FtpEncryptionMode.Implicit;
            client.Credentials = new NetworkCredential(FTPuserID, FTPpassWord);
            client.AutoConnect();
            try
            {
                Stream ftpStream = client.OpenRead(filePath);
                StreamReader reader = new StreamReader(ftpStream);
                #region 資料處理
                string line;
                List<FtpCityParkData> dataList = new List<FtpCityParkData>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lines = line.Split(',');
                    dataList.Add(new FtpCityParkData
                    {
                        Ftp_facility_id = lines[0],
                        Ftp_entrance_uuid = lines[1],
                        Ftp_license_plate_number = lines[2],
                        Ftp_entered_at = DateTime.Parse(lines[3], CultureInfo.InvariantCulture, DateTimeStyles.None),
                        Ftp_entrance_id = lines[4],
                        Ftp_left_at = DateTime.Parse(lines[5], CultureInfo.InvariantCulture, DateTimeStyles.None),
                        Ftp_exit_id = lines[6],
                        Ftp_amount = lines[7],
                    });
                }
                #endregion
                reader.Close();
                ftpStream.Close();
                client.Disconnect();
                return dataList;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}