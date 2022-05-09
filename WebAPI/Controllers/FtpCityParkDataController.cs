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
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 城市車旅Ftp資料寫入 TB_CityParkingChk
    /// </summary>
    public class FtpCityParkDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoFtpCityParkData(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "FtpCityParkDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            #endregion

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    trace.FlowList.Add("寫入API Log");

                }
                #endregion

                #region TB

                #region 城市車旅檢驗比對
                if (flag)
                {
                    string line;
                    try
                    {
                        StreamReader sr = new StreamReader("D:\\CityPark_20220426.txt");
                        //Read the first line of text
                        line = sr.ReadLine();
                        //Continue to read until you reach end of file

                        //宣告存放ftp資料的List
                        List<FtpCityParkData> ftpDataLists = new List<FtpCityParkData>();

                        while (line != null)
                        {
                            //write the lie to console window
                            Console.WriteLine(line);
                            //Read the next line
                            line = sr.ReadLine();

                            string[] columns = line.Split(',');

                            string SPName = "usp_FtpCityParkData_I01";

                            FtpCityParkData Data = new FtpCityParkData();

                            object[] objparms = new object[ftpDataLists.Count == 0 ? 1 : ftpDataLists.Count];

                            if (ftpDataLists.Count > 0)
                            {
                                for (int i = 0; i < ftpDataLists.Count; i++)
                                {
                                    objparms[i] = new
                                    {
                                        Ftp_facility_id = ftpDataLists[i].Ftp_facility_id,
                                        Ftp_entrance_uuid = ftpDataLists[i].Ftp_entrance_uuid,
                                        Ftp_license_plate_number = ftpDataLists[i].Ftp_license_plate_number,
                                        Ftp_entered_at = ftpDataLists[i].Ftp_entered_at,
                                        Ftp_entrance_id = ftpDataLists[i].Ftp_entrance_id,
                                        Ftp_left_at = ftpDataLists[i].Ftp_left_at,
                                        Ftp_exit_id = ftpDataLists[i].Ftp_exit_id,
                                        Ftp_amount = ftpDataLists[i].Ftp_amount
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
                                //if (flag)
                                //{
                                //    if (ds1.Tables[0].Rows.Count > 0)
                                //    {
                                //        RewardPoint = Convert.ToInt32(ds1.Tables[0].Rows[0]["Reward"]);
                                //    }
                                //}
                            }
                            trace.traceAdd("FtpCityParkData_SaveObject", parms1);
                            trace.traceAdd("usp_FtpCityParkData_I01", new { flag, errCode });

                        }
                        //close the file
                        sr.Close();
                        Console.ReadLine();
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
                #endregion

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
                //ProcessedJobCount2.Inc();//唐加prometheus
                //SetCount("NUM_CreditAuth_Fail");
            }

            #region TraceLog
            if (string.IsNullOrWhiteSpace(trace.BaseMsg))
            {
                if (flag)
                    carRepo.AddTraceLog(84, funName, eumTraceType.mark, trace);
                else
                    carRepo.AddTraceLog(84, funName, eumTraceType.followErr, trace);
            }
            else
                carRepo.AddTraceLog(84, funName, eumTraceType.exception, trace);
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }


    }
}