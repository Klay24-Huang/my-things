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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Bill;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Domain.WebAPI.output.Taishin;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 立即購買
    /// </summary>
    public class BuyNowController : ApiController
    {
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoBuyNow([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
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
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNowController";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNow();
            var outputApi = new OAPI_BuyNow();
            outputApi.PayTypes = new List<OPAI_TypeListParam>();
            outputApi.InvoTypes = new List<OPAI_TypeListParam>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int ProdPrice = 0;

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

                    if (flag)
                    {
                        if(apiInput.ApiID == 190)//月租欠費不顯示產品名稱
                        {
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(apiInput.ProdNm))
                            {
                                flag = false;
                                errCode = "ERR269";
                            }
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
                            if(apiInput.ApiID == 190)//月租欠費只需要付款方式
                            {
                                if (apiInput.PayTypeId == 0)
                                {
                                    flag = false;
                                    errCode = "ERR268";
                                }
                            }
                            else
                            {
                                if (apiInput.PayTypeId == 0 || apiInput.InvoTypeId == 0)
                                {
                                    flag = false;
                                    errCode = "ERR268";
                                }
                            }
                        }
                    }

                    if (flag)
                    {
                        if (apiInput.ProdPrice > 0)
                            ProdPrice = apiInput.ProdPrice;
                    }

                    trace.FlowList.Add("防呆");
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

                            var payTypes = spList.Where(x => x.CodeGroup == "PayType").OrderBy(y=>y.Sort).ToList();
                            var invoTypes = spList.Where(x => x.CodeGroup == "InvoiceType").OrderBy(y=>y.Sort).ToList();

                            if (payTypes != null && payTypes.Count() > 0)
                            {
                                outputApi.PayTypes = (from a in payTypes
                                                      select new OPAI_TypeListParam
                                                      {
                                                          CodeId = Convert.ToInt32(a.CodeId),
                                                          CodeNm = a.CodeNm,
                                                          IsBind = a.IsBind
                                                      }).ToList();
                            }

                            if (invoTypes != null && invoTypes.Count() > 0)
                            {
                                outputApi.InvoTypes = (from a in invoTypes
                                                       select new OPAI_TypeListParam
                                                       {
                                                           CodeId = Convert.ToInt32(a.CodeId),
                                                           CodeNm = a.CodeNm,
                                                           IsBind = a.IsBind
                                                       }).ToList();
                            }

                            outputApi.ProdNm = apiInput.ProdNm;
                            outputApi.ProdPrice = apiInput.ProdPrice;
                        }
                    }
                    else if (apiInput.DoPay == 1)//付款
                    {
                        #region 信用卡交易

                        var WsOut = new WebAPIOutput_Auth();
                        if (ProdPrice > 0) //有價格才進行信用卡交易
                        {
                            trace.traceAdd("CarTradeIn", new { IDNO, ProdPrice, errCode });
                            try
                            {
                                if(apiInput.ApiID == 190)//月租欠費
                                    flag = mscom.MonArrears_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);
                                else
                                    flag = mscom.Month_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);

                                if (WsOut != null)
                                    trace.traceAdd("CarTradeResult", new { WsOut });
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                errCode = "ERR270";
                                trace.BaseMsg = ex.Message;
                                throw new Exception("TSIBTrade Fail");
                            }

                            trace.FlowList.Add("信用卡交易");
                        }

                        #endregion

                        if (flag)
                        {           
                            if(apiInput.ApiID > 0)
                            {
                                flag = buyNxtCom.exeNxt();
                                errCode = buyNxtCom.errCode;
                                trace.FlowList.Add("後續api處理");
                            }
                        }
                        outputApi.PayResult = flag ? 1 : 0;
                    }               
                }

                #endregion

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }            

            carRepo.AddTraceLog(181, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    
    }
}
