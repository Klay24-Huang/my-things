using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Rent;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 延長用車
    /// </summary>
    public class BookingExtendController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBookingExtend(Dictionary<string, object> value)
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
            IAPI_BookingExtend apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
    

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;
            DateTime StopTime=new DateTime();
            DateTime SD = new DateTime();


            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingExtend>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                string[] checkList = { apiInput.OrderNo, apiInput.ED};
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                //2.格式判斷
                if (flag)
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
            //時間判斷
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(apiInput.ED))
                {
                    flag = DateTime.TryParse(apiInput.ED, out StopTime);
                    if (flag == false)
                    {
                        errCode = "ERR176";
                    }
                    else
                    {
                        if (StopTime <= DateTime.Now)
                        {
                            flag = false;
                            errCode = "ERR177";
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
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }
            if (flag)
            {
                SPInput_GetBookingStartTime spInput = new SPInput_GetBookingStartTime()
                {
                   OrderNo= tmpOrder,
                    IDNO = IDNO,
                    LogID =LogID,
                     Token=Access_Token
                };
                SPOutput_GetBookingStartTime spOut = new SPOutput_GetBookingStartTime();
                string spName = new ObjType().GetSPName(ObjType.SPType.GetBookingStartTime);
                SQLHelper<SPInput_GetBookingStartTime, SPOutput_GetBookingStartTime> sqlHelp = new SQLHelper<SPInput_GetBookingStartTime, SPOutput_GetBookingStartTime>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                   

                    if (StopTime.Subtract(spOut.SD).TotalDays == 7)
                    {
                        if (StopTime.Subtract(spOut.SD).TotalHours > 0 || StopTime.Subtract(spOut.SD).TotalMinutes > 0)
                        {
                            flag = false;
                            errCode = "ERR179";
                        }
                    }
                    else if (StopTime.Subtract(spOut.SD).TotalDays > 7)
                    {
                        flag = false;
                        errCode = "ERR179";
                    }
                 
                }
                else
                {
                    errCode = spOut.ErrorCode;
                }
                if (StopTime < spOut.ED)
                {
                    flag = false;
                    errCode = "ERR178";
                }
                if (flag)
                {
                    SD = spOut.ED; //前一次的結束時間
                }
                if (flag)
                {
                    SPInput_BookingExtend spExtend = new SPInput_BookingExtend()
                    {

                        OrderNo = tmpOrder,
                        IDNO = IDNO,
                        Token=Access_Token,
                        LogID = LogID,
                        SD = SD,
                        ED = StopTime,
                        CarNo = spOut.CarNo
                    };
                    SPOutput_Base spOutExtend = new SPOutput_Base();
                    spName= new ObjType().GetSPName(ObjType.SPType.BookingExtend);
                    SQLHelper<SPInput_BookingExtend, SPOutput_Base> sqlHelpCheck = new SQLHelper<SPInput_BookingExtend, SPOutput_Base>(connetStr);
                    flag = sqlHelpCheck.ExecuteSPNonQuery(spName, spExtend, ref spOutExtend, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spOutExtend, ref lstError, ref errCode);
                    
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
