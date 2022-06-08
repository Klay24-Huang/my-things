using Domain.Common;
using Domain.SP.Input;
using Domain.SP.Output;
using Domain.TB.Sync;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.Json;
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
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        public Dictionary<string, object> DoSendAlertEmailJob(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "SendAlertEmailJobController";
            Int64 LogID = 0;

            NullOutput apiOutput = new NullOutput();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;

            List<Sync_SendEventMessage> SendList = new List<Sync_SendEventMessage>();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("No Input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion
            #region TB
            #region 寫入告警事件
            if (flag)
            {
                string spName = "usp_InsAlertEvent";
                SPInput_Base spInput = new SPInput_Base()
                {
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion
            #region 刪除過期未發送的資料
            if (flag)
            {
                string spName = "usp_DeleteAlertMailLog";
                SPInput_Base spInput = new SPInput_Base()
                {
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion
            #region 取得要發告警MAIL的清單
            if (flag)
            {
                string spName = "usp_GetAlertMailLog";
                SPInput_Base spInput = new SPInput_Base
                {
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref SendList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion
            #region 發信
            if (flag)
            {
                var GroupHandleList = SendList.GroupBy(x => new { x.EventType, x.Receiver }).Select(x => new { x.Key.EventType, x.Key.Receiver }).OrderBy(x => x.EventType).ThenBy(x => x.Receiver).ToList();

                foreach (var item in GroupHandleList)
                {
                    var ToSendList = SendList.Where(x => x.EventType == item.EventType && x.Receiver == item.Receiver).OrderBy(x => x.MKTime).ThenBy(x => x.CarNo).ToList();
                    logger.Info(string.Format("{0}:事件類別:{1}|收件者:{2}|需寄發MAIL數量:{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.EventType, item.Receiver, ToSendList.Count));

                    int SendFlag = SendGridGroupSendMail(item.EventType, item.Receiver, ToSendList);

                    #region 結果存檔
                    object[] objparms = new object[ToSendList.Count() == 0 ? 1 : ToSendList.Count()];

                    if (ToSendList.Count() > 0)
                    {
                        for (int i = 0; i < ToSendList.Count(); i++)
                        {
                            objparms[i] = new
                            {
                                AlertID = ToSendList[i].AlertID,
                                EventType = ToSendList[i].EventType
                            };
                        }
                    }
                    else
                    {
                        objparms[0] = new
                        {
                            AlertID = 0,
                            EventType = 0
                        };
                    }

                    object[][] parms1 = {
                        new object[] {
                            "SendGuid",
                            SendFlag == 0 ? 1 :2,
                            LogID
                        },
                        objparms
                    };

                    DataSet ds1 = new DataSet();
                    string returnMessage = "";
                    string messageLevel = "";
                    string messageType = "";
                    string result = ""; // 執行結果

                    string SPName2 = "usp_AlertMailLog_U01";
                    ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName2, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                    if (ds1.Tables.Count == 0)
                    {
                        flag = false;
                        if (SendFlag == 1)
                            SendFlag = 2;   //發送失敗，更新失敗
                        else
                            SendFlag = 3;   //發送成功，更新失敗
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(returnMessage))
                        {
                            result = returnMessage;
                        }
                    }
                    ds1.Dispose();

                    switch (SendFlag)
                    {
                        case 0:
                            result = "發送成功，更新成功";
                            break;
                        case 1:
                            result = "發送失敗，更新成功";
                            break;
                        case 2:
                            result = "發送失敗，更新失敗";
                            break;
                        case 3:
                            result = "發送成功，更新失敗";
                            break;
                    }
                    #endregion
                    logger.Info(string.Format("{0}:事件類別:{1}|收件者:{2}|執行結果:{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.EventType, item.Receiver, result));
                }
            }
            #endregion
            #endregion
            #region 寫入錯誤Log
            if (!flag)
            {
                baseVerify.InsErrorLog(funName, errCode, 0, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
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
        /// <returns></returns>
        public int SendGridGroupSendMail(int EventType, string Receiver, List<Sync_SendEventMessage> ToSendList)
        {
            bool flag = true;
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
                    case 15:
                        Title = string.Format("異常告警：{0} 事件名單", "授權失敗");
                        break;
                    case 16:
                        Title = string.Format("異常告警：{0} 事件名單", "滿10小時授權失敗");
                        break;
                    case 17:
                        Title = string.Format("異常告警：{0} 事件名單", "滿13小時授權失敗");
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
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ToSend.CarNo, string.Format("H{0}", ToSend.OrderNo), ToSend.MKTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        }

                        Table += "</table>";
                        break;

                    case 13:    // 取車1小時前沒有車
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>被影響合約編號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ToSend.CarNo, string.Format("H{0}", ToSend.OrderNo), ToSend.MKTime.ToString("yyyy/MM/dd HH:mm:ss"));
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

                        Body += string.Format("<p>{0}{1}</p>", "資料時間：", ToSendList.FirstOrDefault().MKTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        break;

                    case 15:    // 授權失敗
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>訂單編號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", ToSend.OrderNo, ToSend.MKTime);
                        }

                        Table += "</table>";
                        break;

                    case 16:    // 16:滿10小時授權失敗
                    case 17:    // 17:滿13小時授權失敗
                        if (EventType == 16)
                            Body += string.Format("<p>{0}</p>", "訂單10小時授權失敗，請聯絡客戶確認訂單，將於10+3小時再次授權");
                        else
                            Body += string.Format("<p>{0}</p>", "訂單10+3小時授權失敗，客戶用車費用差額付款再次失敗，請聯絡客戶確認用車狀況，請客戶立即還車，我司有權終止合約並收回車輛使用。");

                        Table += string.Format("<table border=1><tr style='background-color:#8DD26F;'><th>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th><th>{4}</th><th>{5}</th><th>{6}</th><th>{7}</th><th>{8}</th><th>{9}</th></tr>"
                                                , "訂單編號", "預約時間", "實際取車時間", "預計還車時間", "車號", "會員帳號", "會員姓名", "手機", "據點名稱", "管轄門市");

                        foreach (var ToSend in ToSendList)
                        {
                            var AlertMailObj = JsonSerializer.Deserialize<AlertMailObj>(ToSend.Remark);
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td></tr>"
                                                    , AlertMailObj.OrderNo, AlertMailObj.BookingDate, AlertMailObj.StartTime, AlertMailObj.StopTime, AlertMailObj.CarNo
                                                    , AlertMailObj.IDNO, AlertMailObj.MEMCNAME, AlertMailObj.MEMTEL, AlertMailObj.StationName, AlertMailObj.ManageStationID);
                        }

                        Table += "</table>";
                        break;

                    default:
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", ToSend.CarNo, ToSend.MKTime.ToString("yyyy/MM/dd HH:mm:ss"));
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