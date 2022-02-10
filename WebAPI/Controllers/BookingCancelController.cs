using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Notification;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取消預約
    /// </summary>
    public class BookingCancelController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBookingCancel(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BookingCancelController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingCancel apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingCancel>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
                {
                    if (apiInput.OrderNo.IndexOf("H") < 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                        if (flag)
                        {
                            if (tmpOrder <= 0)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
                    }
                }
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion

            #region TB
            #region Token判斷
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            //開始做取消預約
            if (flag)
            {
                SPInput_BookingCancel spInput = new SPInput_BookingCancel()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token,
                    Descript = "",
                    Cancel_Status_in = 0,
                    CheckToken =1
                };
                string SPName = "usp_BookingCancel_U01";
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BookingCancel, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BookingCancel, SPOutput_Base>(connetStr);
                List<SPOutput_BookingCancel> ListOut = new List<SPOutput_BookingCancel>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    #region 共同承租人發推播通知
                    if (ListOut != null && ListOut.Count > 0)
                    {
                        string notificationBaseUrl = ConfigurationManager.AppSettings["JointRentInviteeNotificationBaseUrl"].Trim();
                        string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
                        string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();

                        foreach (var list in ListOut)
                        {
                            string ReqParam = AESEncrypt.EncryptAES128("OrderNo=" + list.Order_number.ToString() + "&InviteeId=" + list.MEMIDNO, KEY, IV);
                            string urlEncodeString = HttpUtility.UrlEncode(ReqParam);

                            var title = string.Format("【共同承租】{0}{1}",list.MEMCNAME, "取消邀請了唷!!");

                            string spName = "usp_InsPersonNotification_I01";
                            SPInput_InsPersonNotification SPinput = new SPInput_InsPersonNotification
                            {
                                OrderNo = list.Order_number,
                                IDNO = list.MEMIDNO,
                                NType = 19,
                                STime = DateTime.Now.AddSeconds(10),
                                Title = title,
                                Message = title,
                                url = $"{notificationBaseUrl}?{urlEncodeString}",
                                imageurl = "",
                                LogID = LogID
                            };
                            SPOutput_Base SPout = new SPOutput_Base();
                            flag = new SQLHelper<SPInput_InsPersonNotification, SPOutput_Base>(connetStr).ExecuteSPNonQuery(spName, SPinput, ref SPout, ref lstError);
                            baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
