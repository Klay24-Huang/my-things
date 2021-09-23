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
    public class WalletTransferCheckController : ApiController
    {
        //private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost()]
        public Dictionary<string, object> DoWalletTransferCheck([FromBody] Dictionary<string, object> value)
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
            string funName = "WalletTransferCheck";
            Int64 LogID = 0;
            var apiInput = new IAPI_WalletTransferCheck();
            var outputApi = new OAPI_WalletTransferCheck();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletTransferCheck>(Contentjson);
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

                    if (apiInput == null || string.IsNullOrWhiteSpace(apiInput.IDNO_Phone) )
                    {
                        flag = false;
                        errMsg = "未輸入主要查詢KEY值";
                        errCode = "ERR257";//未輸入主要查詢KEY值
                    }
                    else
                    {
                        if (Int32.TryParse(apiInput.IDNO_Phone, out int intPhoneNo))//判斷是否為全部數字
                            inPhoneNo = intPhoneNo.ToString();
                        else
                            inIDNO = apiInput.IDNO_Phone;
                    }

                    trace.FlowList.Add("判斷輸入為電話或身分證");
                    trace.traceAdd("InputCheck", new { flag, errCode, errMsg});
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

                var CkFrom = new SPOut_WalletTransferCheck();
                var CkTo = new SPOut_WalletTransferCheck();
                if (flag)
                {
                    string sp1ErrCode = "", sp2ErrCode="";

                    #region 贈與人檢查

                    var sp1In = new SPInput_WalletTransferCheck()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                    };
                    var sp1_list = wsp.sp_WalletTransferCheck(sp1In, ref sp1ErrCode);
                    if (sp1_list != null && sp1_list.Count() > 0)
                        CkFrom = sp1_list.FirstOrDefault();
                    else
                        CkFrom = null;

                    if (sp1ErrCode != "0000")
                    {
                        flag = false;
                        errCode = sp1ErrCode;
                    }

                    trace.traceAdd("sp1_lnfo", new { sp1In,  sp1_list, sp1ErrCode});
                    trace.FlowList.Add("贈與人檢查");

                    #endregion

                    #region 受贈人檢查

                    if (flag)
                    {
                        var sp2In = new SPInput_WalletTransferCheck()
                        {
                            IDNO = inIDNO,//受贈人
                            PhoneNo = inPhoneNo,//受贈人
                            LogID = LogID,
                        };
                        var sp2_list = wsp.sp_WalletTransferCheck(sp2In, ref sp2ErrCode);
                        if (sp2_list != null && sp2_list.Count() > 0) 
                        {
                            CkTo = sp2_list.FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(inPhoneNo))
                                outputApi.ShowValue = CkTo.ShowValue;
                            else
                                outputApi.ShowValue = CkTo.ShowName;
                        }
                        else
                            CkTo = null;

                        if (sp2ErrCode != "0000")
                        {
                            flag = false;
                            errCode = sp2ErrCode;
                        }

                        trace.traceAdd("sp2_lnfo", new { sp2In, sp2_list, sp2ErrCode });
                        trace.FlowList.Add("受贈人檢查");
                    }

                    #endregion

                    #region 商業邏輯檢查

                    /*
                    if(flag)
                    {
                        if (CkFrom != null && CkTo != null)
                        {
                            //轉贈人錢包是否足夠
                            if (apiInput.Amount > CkFrom.WalletAmount)
                            {
                                flag = false;
                                errMsg = "轉贈金額超過錢包金額";
                                errCode = "ERR281";
                            }
                            //被轉贈人錢包總額是否超過5萬
                            if (flag)
                            {
                                if ((apiInput.Amount + CkTo.WalletAmount) > 50000)
                                {
                                    flag = false;
                                    errMsg = "錢包金額超過上限";
                                    errCode = "ERR282";
                                }
                            }

                            //被轉贈人金流是否超過30萬    
                            if(flag)
                            {
                                if ((apiInput.Amount + CkTo.MonTransIn) > 300000)
                                {
                                    flag = false;
                                    errMsg = "金流超過上限";
                                    errCode = "ERR280";
                                }
                            }
                            trace.FlowList.Add("商業邏輯檢查");
                            
                        }
                    }
                    */

                    #endregion
                }

                outputApi.CkResult = flag ? 1 : 0;
               // outputApi.Amount = apiInput.Amount; --2021/09/23 UPD BY YANKEY 不須判斷錢包餘額

                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                outputApi.CkResult = 0;
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(98, funName, trace, flag);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

        #region mark

        //[HttpPost]
        //public Dictionary<string, object> DoWalletTransferCheck(Dictionary<string, object> value)
        //{
        //    #region 初始宣告
        //    HttpContext httpContext = HttpContext.Current;
        //    //string[] headers=httpContext.Request.Headers.AllKeys;
        //    string Access_Token = "";
        //    string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
        //    var objOutput = new Dictionary<string, object>();    //輸出
        //    bool flag = true;
        //    bool isWriteError = false;
        //    string errMsg = "Success"; //預設成功
        //    string errCode = "000000"; //預設成功
        //    string funName = "WalletTransferCheckController";
        //    Int64 LogID = 0;
        //    Int16 ErrType = 0;
        //    IAPI_WalletTransferCheck apiInput = null;
        //    OAPI_WalletTransferCheck outputApi = null;
        //    Token token = null;
        //    CommonFunc baseVerify = new CommonFunc();
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();
        //    string Contentjson = "";
        //    bool isGuest = true;
        //    string IDNO = "";
        //    #endregion

        //    #region 防呆
        //    flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
        //    if (flag)
        //    {
        //        apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletTransferCheck>(Contentjson);
        //        //寫入API Log
        //        string ClientIP = baseVerify.GetClientIp(Request);
        //        flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
        //    }
        //    #endregion

        //    #region TB
        //    //Token判斷
        //    if (flag)
        //    {
        //        flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
        //    }

        //    if (flag)
        //    {
        //        string SPName = new ObjType().GetSPName(ObjType.SPType.GetMemberInfo);
        //        SPInput_GetMemberName SPInput = new SPInput_GetMemberName()
        //        {
        //            LoginIDNO = IDNO,
        //            DonateIDNO = apiInput.IDNO,
        //            Token = Access_Token,
        //            LogID = LogID
        //        };
        //        SPOutput_GetMemberName SPOutput = new SPOutput_GetMemberName();
        //        SQLHelper<SPInput_GetMemberName, SPOutput_GetMemberName> sqlHelp = new SQLHelper<SPInput_GetMemberName, SPOutput_GetMemberName>(connetStr);
        //        flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
        //        baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
        //        if (string.IsNullOrEmpty(SPOutput.Name))
        //        {
        //            flag = false;
        //            errCode = "ERR201";
        //        }

        //        if (flag)
        //        {
        //            outputApi = new OAPI_WalletTransferCheck
        //            {
        //                Name = SPOutput.Name,
        //                PhoneNo = SPOutput.PhoneNo,
        //                Amount = apiInput.Amount
        //            };
        //        }
        //    }
        //    #endregion

        //    #region 寫入錯誤Log
        //    if (flag == false && isWriteError == false)
        //    {
        //        baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
        //    }
        //    #endregion

        //    #region 輸出
        //    baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
        //    return objOutput;
        //    #endregion
        //}

        #endregion
    }
}