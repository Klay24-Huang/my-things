using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Rent;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 一次性開門申請（含重開發簡訊）
    /// </summary>
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
            string funName = "OpenDoorController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_OpenDoor apiInput = null;
            OAPI_OpenDoor outputApi = null;
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();


            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            DateTime StopTime;

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_OpenDoor>(Contentjson);
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
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.CheckCanOpenDoor);
                SPInput_CheckCanOpenDoor spInput = new SPInput_CheckCanOpenDoor()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                   OrderNo=tmpOrder,
                    Token=Access_Token
                };
                SPOutput_CheckCanOpenDoor spOut = new SPOutput_CheckCanOpenDoor();
                SQLHelper<SPInput_CheckCanOpenDoor, SPOutput_CheckCanOpenDoor> sqlHelp = new SQLHelper<SPInput_CheckCanOpenDoor, SPOutput_CheckCanOpenDoor>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,  spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                  
                    string VerifyCode = baseVerify.getRand(0, 999999);
                    HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                    WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                    string Message = string.Format("您的一次性開門驗證碼是：{0}", VerifyCode);
                    flag = hiEasyRentAPI.NPR260Send(spOut.Mobile, Message, "", ref wsOutput);

                    if (flag)
                    {
                        spName = new ObjType().GetSPName(ObjType.SPType.InsOpenDoorCode);
                        SPInput_InsOpenDoorCode SPInput = new SPInput_InsOpenDoorCode()
                        {
                            LogID = LogID,
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            Token = Access_Token
                        };
                        SPOutput_Base SPOut = new SPOutput_Base();
                        SQLHelper<SPInput_InsOpenDoorCode, SPOutput_Base> sqlInsHelp = new SQLHelper<SPInput_InsOpenDoorCode, SPOutput_Base>(connetStr);
                        flag = sqlInsHelp.ExecuteSPNonQuery(spName, SPInput, ref SPOut, ref lstError);
                        baseVerify.checkSQLResult(ref flag, SPOut.Error, SPOut.ErrorCode, ref lstError, ref errCode);
                        if (flag)
                        {
                            outputApi = new OAPI_OpenDoor()
                            {
                                DeadLine = spOut.DeadLine.ToString("yyyy-MM-dd HH:mm:ss"),
                                VerifyCode = VerifyCode
                            };
                        }
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
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
