using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Subscription;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 立即購買
    /// </summary>
    public class BuyNowController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoBuyNow([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNowController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_BuyNow();
            var outputApi = new OAPI_BuyNow();
            outputApi.PayTypes = new List<OPAI_TypeListParam>();
            outputApi.InvoTypes = new List<OPAI_TypeListParam>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";


            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNow>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }
                    if (flag)
                    {
                        var DoPays = new List<int> { 0, 1 };
                        if (!DoPays.Any(x=>x == apiInput.DoPay))
                        {
                            flag = false;
                            errCode = "ERR266";//DoPay只可為0或1
                        }
                    }
                    if (flag && apiInput.ApiID > 0)
                    {
                        buyNxtCom.ApiID = apiInput.ApiID;
                        buyNxtCom.ApiJson = apiInput.ApiJson;
                        flag = buyNxtCom.CkApiID();
                        errCode = buyNxtCom.errCode;
                    }

                    if (flag)
                    {
                        if(apiInput.DoPay == 1)
                        {
                            if(apiInput.PayTypeId == 0 || apiInput.InvoTypeId == 0)
                            {
                                flag = false;
                                errCode = "ERR268";
                            }

                            if (string.IsNullOrWhiteSpace(apiInput.ProdNm))
                            {
                                flag = false;
                                errCode = "ERR269";
                            }
                        }
                    }

                    trace.FlowList.Add("防呆");
                    trace.traceAdd("InCk",new { flag, errCode });                    
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
                    if (apiInput.DoPay == 0)
                    {
                        var spIn = new SPInput_GetBuyNowInfo()
                        {
                            IDNO = IDNO,
                            LogID = LogID
                        };
                        trace.traceAdd("spIn", spIn);
                        var spList = msp.sp_GetBuyNowInfo(spIn, ref errCode);

                        if (spList != null && spList.Count() > 0)
                        {
                            trace.traceAdd("spList", spList);

                            var payTypes = spList.Where(x => x.CodeGroup == "PayType").ToList();
                            var invoTypes = spList.Where(x => x.CodeGroup == "InvoiceType").ToList();

                            if (payTypes != null && payTypes.Count() > 0)
                            {
                                outputApi.PayTypes = (from a in payTypes
                                                      select new OPAI_TypeListParam
                                                      {
                                                          CodeId = Convert.ToInt32(a.CodeId),
                                                          CodeNm = a.CodeNm
                                                      }).ToList();
                            }

                            if (invoTypes != null && invoTypes.Count() > 0)
                            {
                                outputApi.InvoTypes = (from a in invoTypes
                                                       select new OPAI_TypeListParam
                                                       {
                                                           CodeId = Convert.ToInt32(a.CodeId),
                                                           CodeNm = a.CodeNm
                                                       }).ToList();
                            }

                            outputApi.ProdNm = apiInput.ProdNm;
                            outputApi.ProdPrice = apiInput.ProPrice;
                        }
                    }
                    else if (apiInput.DoPay == 1)//付款
                    {
                        bool PayResult = true;//信用卡交易
                        trace.FlowList.Add("信用卡交易");
                        trace.traceAdd("PayResult", PayResult);
                        if (PayResult)
                        {                            
                            flag = buyNxtCom.exeNxt();
                            errCode = buyNxtCom.errCode;
                            outputApi.PayResult = flag ? 1 : 0;//呼叫api後續動作   

                            trace.FlowList.Add("api後續");
                            trace.traceAdd("apiNxt", flag);
                        }
                        else
                        {
                            flag = false;
                            errCode = "ERR270";//信用卡交易失敗
                        }
                    }               
                }

                #endregion

                #region trace

                trace.traceAdd("apiOut", apiInput);

                if (flag)
                    carRepo.AddTraceLog(181, funName, eumTraceType.mark, trace);
                else
                    carRepo.AddTraceLog(181, funName, eumTraceType.followErr, trace);

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(181, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
