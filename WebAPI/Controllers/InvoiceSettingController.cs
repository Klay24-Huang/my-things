using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.SP.Output.Common;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 發票設定
    /// </summary>
    public class InvoiceSettingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoInvoiceSetting(Dictionary<string, object> value)
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
            string funName = "InvoiceSettingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_InvoiceSetting apiInput = null;
            NullOutput outputApi = null;

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int64 tmpOrder = 0;
            Int16 SettingMode = 0;  //設定發票
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_InvoiceSetting>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (flag)
                {
                    if (!string.IsNullOrWhiteSpace(apiInput.OrderNo))
                    {
                        SettingMode = 1;
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
                //判斷發票類型
                if (flag)
                {
                    flag = baseVerify.regexStr(apiInput.InvoiceType.ToString(), CommonFunc.CheckType.InvoiceType);
                    if (false == flag)
                    {
                        errCode = "ERR192";
                    }
                }
              
             
                #region 各類型判斷
                if (flag)
                {
                    switch (apiInput.InvoiceType)
                    {
                        case 4: //判斷統編
                            if (string.IsNullOrWhiteSpace(apiInput.UniCode))
                            {
                                flag = false;
                                errCode = "ERR190";
                            }
                            else
                            {
                                flag = baseVerify.checkUniNum(apiInput.UniCode);
                                if (false == flag)
                                {
                                    flag = false;
                                    errCode = "ERR191";
                                }
                            }
                            break;
                        case 5:
                        case 6:
                            if (string.IsNullOrWhiteSpace(apiInput.CARRIERID))
                            {
                                flag = false;
                                errCode = "ERR193";
                            }
                            else
                            {

                                flag = new HiEasyRentAPI().CheckEinvBiz(apiInput.CARRIERID, ref errCode);
                              
                            }
                            break;
                    }
                }
                #endregion

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
            #region 更新會員資料表或是寫入訂單內
            if (flag)
            {
                
                 string SPName = new ObjType().GetSPName(ObjType.SPType.SettingInvoice);
                SPInput_SettingInvoice SPInput = new SPInput_SettingInvoice()
                {

                    LogID = LogID,
                    CARRIERID = apiInput.CARRIERID,
                    IDNO = IDNO,
                    InvoiceType = apiInput.InvoiceType,
                    NPOBAN = apiInput.NOBAN,
                    SettingMode = SettingMode,
                    UniCode = apiInput.UniCode,
                    OrderNo = tmpOrder,
                    Token = Access_Token
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SettingInvoice, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SettingInvoice, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
               
            }
            #endregion
            #endregion
        }
    }
}
