using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 一次性開門結束
    /// </summary>
    public class OpenDoorFinishController : ApiController
    {
        public class OpenDoorController : ApiController
        {
            private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
            [HttpPost]
            public Dictionary<string, object> DoBookingStart(Dictionary<string, object> value)
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
                string funName = "BookingStartController";
                Int64 LogID = 0;
                Int16 ErrType = 0;
                IAPI_BookingStart apiInput = null;
                OAPI_BookingStart outputApi = new OAPI_BookingStart();
                Int64 tmpOrder = -1;
                Token token = null;
                CommonFunc baseVerify = new CommonFunc();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                CarInfo info = new CarInfo();

                Int16 APPKind = 2;
                string Contentjson = "";
                bool isGuest = true;

                string IDNO = "";
                string CID = "";
                string deviceToken = "";
                int IsMotor = 0;
                int IsCens = 0;
                double mil = 0;
                DateTime StopTime;
                List<CardList> lstCardList = new List<CardList>();

                #endregion
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingStart>(Contentjson);
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
                if (flag)
                {
                    if (!string.IsNullOrWhiteSpace(apiInput.ED))
                    {
                        flag = DateTime.TryParse(apiInput.ED, out StopTime);
                        if (flag == false)
                        {
                            errCode = "ERR173";
                        }
                        else
                        {
                            if (StopTime <= DateTime.Now)
                            {
                                flag = false;
                                errCode = "ERR174";
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
                #endregion
                #region 寫入錯誤Log
                if (false == flag && false == isWriteError)
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
}
