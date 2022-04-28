using Domain.Common;
using Domain.SP.Input;
using Domain.SP.Input.Other;
using Domain.SP.Output;
using Domain.Sync.Input;
using Domain.TB.Sync;
using Newtonsoft.Json;
using NLog;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    public class SendAlertEmailJobController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger(); // Log
        private static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString; // DB Info
        private static bool flag = true;

        public Dictionary<string, object> doSendAlertEmailJob(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "SendAlertEmailJobController";
            string spName = "";
            string Contentjson = "";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            NullOutput outputApi = new NullOutput();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            bool HasInput = false;

            SPInput_GetAlertMailLog spInput = new SPInput_GetAlertMailLog();
            SPOutput_Base spOutput = new SPOutput_Base();

            List<Sync_SendEventMessage> SPOutList = new List<Sync_SendEventMessage>();
            SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            #endregion

            #region 防呆
            if (value != null)
                HasInput = true;

            //flag = baseVerify.baseCheck(ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, HasInput);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("No Input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB
            #region 寫入 告警
            if (flag)
            {
                spName = "usp_InsAlertEvent";
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOutput, ref SPOutList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOutput.Error, spOutput.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 刪除/整理/寄送 告警資料
            #region 刪除
            //// 刪除 過期未發送資料
            spName = "usp_DeleteAlertMailLog";
            spInput.LogID = LogID;
            flag = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOutput.Error, spOutput.ErrorCode, ref lstError, ref errCode);
            #endregion
            if (flag)
            {
                #region 整理
                // 取出 發告警資料 
                spName = "usp_GetAlertMailLog";
                ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOutput, ref SPOutList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOutput.Error, spOutput.ErrorCode, ref lstError, ref errCode);
                #endregion
                #region 寄送
                // 發送 告警EMAIL
                if (flag && SPOutList != null)
                {
                    //if (SPOutList != null && SPOutList.Count > 0)
                    //{
                    //    foreach (Sync_SendEventMessage SSEM in SPOutList)
                    //    {
                    //        logger.Info(string.Format("{0}:事件類別:{1}，需寄發MAIL數量:{2}", DateTime.Now.ToString(strTimeType), SSEM.EventType, SPOutList.Count));
                    //        int SendFlag = 0;
                    //        //SendGridGroupSendMail(SSEM.EventType, SSEM.Receiver, SPOutList, ref lstError);
                    //        object[] objparms = new object[SPOutList.Count() == 0 ? 1 : SPOutList.Count()];
                    //        if (SPOutList.Count() > 0)
                    //        {
                    //            for (int i = 0; i < SPOutList.Count(); i++)
                    //            {
                    //                objparms[i] = new
                    //                {
                    //                    AlertID = SPOutList[i].AlertID,
                    //                    EventType = SPOutList[i].EventType
                    //                };
                    //            }
                    //        }
                    //        else
                    //        {
                    //            objparms[0] = new
                    //            {
                    //                AlertID = 0,
                    //                EventType = 0
                    //            };
                    //        }

                    //        object[][] parms1 = {
                    //                new object[] {
                    //                    "SendGuid",
                    //                    SendFlag == 0 ? 1 :2,
                    //                    LogID
                    //                },
                    //                objparms
                    //            };

                    //        DataSet ds1 = new DataSet();
                    //        string returnMessage = "";
                    //        string messageLevel = "";
                    //        string messageType = "";
                    //        string result = ""; // 執行結果

                    //        spName = "usp_AlertMailLog_U01";
                    //        ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                    //        if (ds1.Tables.Count == 0)
                    //        {
                    //            flag = false;
                    //            if (SendFlag == 1)
                    //                SendFlag = 2;   //發送失敗，更新失敗
                    //            else
                    //                SendFlag = 3;   //發送成功，更新失敗
                    //        }
                    //        else
                    //        {
                    //            if (!string.IsNullOrEmpty(returnMessage))
                    //            {
                    //                result = returnMessage;
                    //            }
                    //        }
                    //        ds1.Dispose();

                    //        switch (SendFlag)
                    //        {
                    //            case 0:
                    //                result = "發送成功，更新成功";
                    //                break;
                    //            case 1:
                    //                result = "發送失敗，更新成功";
                    //                break;
                    //            case 2:
                    //                result = "發送失敗，更新失敗";
                    //                break;
                    //            case 3:
                    //                result = "發送成功，更新失敗";
                    //                break;
                    //        }

                    //        logger.Info(string.Format("{0}:事件類別:{1},執行結果:{2}", DateTime.Now.ToString(strTimeType), SSEM.EventType, result));
                    //    }
                    //}
                }
                #endregion
            }
            #endregion
            #endregion

            #region 寫入錯誤Log
            if (!flag)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        #region SendGrid-依事件類型發送
        /// <summary>
        /// SendGrid-依事件類型發送
        /// </summary>
        /// <param name="EventType">事件代碼</param>
        /// <param name="Receiver">收件者</param>
        /// <param name="ToSendList">寄送清單</param>
        /// <param name="lstError">錯誤訊息</param>
        /// <returns></returns>
        private static int SendGridGroupSendMail(int EventType, string Receiver, List<Sync_SendEventMessage> ToSendList, ref List<ErrorInfo> lstError)
        {
            flag = true;
            int SendFlag = 0;

            try
            {
                //發mail
                SendMail send = new SendMail();
                string Title = string.Empty;
                string Body = string.Empty;
                string Table = string.Empty;

                switch (EventType)
                {
                    case 1:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，但有時速");
                        break;
                    case 6:
                        Title = string.Format("異常告警：{0} 事件名單", "低電量通知");
                        break;
                    case 7:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，車門被打開");
                        break;
                    case 8:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，電門被啟動");
                        break;
                    case 9:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，引擎被發動");
                        break;
                    case 10:
                        Title = string.Format("異常告警：{0} 事件名單", "車機失聯1小時");
                        break;
                    case 11:
                        Title = string.Format("異常告警：{0} 事件名單", "超過15分鐘未完成還車作業");
                        break;
                    case 12:
                        Title = string.Format("異常告警：{0} 事件名單", "超過預約還車時間30分鐘未還車");
                        break;
                    case 13:
                        Title = string.Format("異常告警：{0} 事件名單", "取車1小時前沒有車");
                        break;
                    case 14:
                        Title = string.Format("異常告警：{0} 事件名單", "三日未出租");
                        break;
                }

                // 依照事件類型調整TABLE內容
                switch (EventType)
                {
                    case 11:    // 超過15分鐘未完成還車作業
                    case 12:    // 超過預約還車時間30分鐘未還車
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>訂單編號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ToSend.CarNo, string.Format("H{0}", ToSend.OrderNo), ToSend.MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));
                        }

                        Table += "</table>";

                        break;

                    case 13:    // 取車1小時前沒有車
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>被影響合約編號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ToSend.CarNo, string.Format("H{0}", ToSend.OrderNo), ToSend.MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));
                        }

                        Table += "</table>";

                        break;

                    case 14:    // 三日未出租
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>據點</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", ToSend.CarNo, ToSend.StationID);
                        }

                        Table += "</table>";

                        Body += string.Format("<p>{0}{1}</p>", "資料時間：", ToSendList.FirstOrDefault().MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));

                        break;

                    default:
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", ToSend.CarNo, ToSend.MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));
                        }

                        Table += "</table>";

                        break;
                }

                Body += string.Format("<p>{0}</p>", Table);

                flag = Task.Run(() => send.DoSendMail(Title, Body, Receiver)).Result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SendFlag = 1;
            }

            return SendFlag;
        }
        #endregion
    }
}