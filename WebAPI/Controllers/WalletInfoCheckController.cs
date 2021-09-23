using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Models.BillFunc;
using Newtonsoft.Json;
using Domain.SP.Input.Wallet;
using WebAPI.Service;
using WebAPI.Utils;
using Domain.SP.Output.Wallet;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 查詢電子錢包資訊
    /// </summary>
    public class WalletInfoCheckController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoWalletInfoCheck([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var wsp = new WalletSp();
            var wMap = new WalletMap();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletInfoCheck";
            Int64 LogID = 0;
            var apiInput = new IAPI_WalletInfoCheck();
            var outputApi = new OAPI_WalletInfoCheck();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            string inIDNO = "";//輸入身分證號
            string inPhoneNo = "";//輸入手機號碼            

            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletInfoCheck>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errMsg = "不開放訪客";
                            errCode = "ERR101";
                        }
                    }

                    if (apiInput == null || string.IsNullOrWhiteSpace(apiInput.IDNO_Phone))
                    {
                        flag = false;
                        errMsg = "參數遺漏";
                        errCode = "ERR257";//參數遺漏
                    }
                    else 
                    {
                        if (Int32.TryParse(apiInput.IDNO_Phone, out int intPhoneNo))
                            inPhoneNo = intPhoneNo.ToString();
                        else
                            inIDNO = apiInput.IDNO_Phone;
                    }

                    trace.FlowList.Add("防呆");
                    trace.traceAdd("InCk", new { flag, errCode, errMsg });
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                    trace.traceAdd("TokenCk", new { flag, errCode });
                }

                #endregion

                #region TB

                if (flag)
                {
                    string sp1ErrCode = "";
                    var sp1In = new SPInput_WalletTransferCheck()
                    {
                        IDNO = inIDNO,
                        LogID = LogID,
                        PhoneNo = inPhoneNo
                    };
                    var sp1_list = wsp.sp_WalletTransferCheck(sp1In, ref sp1ErrCode);
                    if (sp1_list != null && sp1_list.Count() > 0)
                    {
                        var fItem = sp1_list.FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(inPhoneNo))
                            outputApi.Name_Phone = fItem.ShowValue;
                        else
                            outputApi.Name_Phone = fItem.ShowValue;
                    }

                    if (sp1ErrCode != "0000")
                    {
                        flag = false;
                        errCode = sp1ErrCode;
                    }

                    trace.traceAdd("sp1_lnfo", new { sp1In, sp1_list, sp1ErrCode });
                    trace.FlowList.Add("sp查詢");                  
                }

                outputApi.CkResult = flag ? 1 : 0;

                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                outputApi.CkResult = 0;
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(209, funName, trace, flag);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
