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
using System.Threading.Tasks;
using WebAPI.Models.Param.CusFun.Input;
using System.Threading;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 排程訂閱制履保設定
    /// </summary>
    public class SubsEscrowPayController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoSubsEscrowPay([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var mscom = new MonSubsCommon();
            var buyNxtCom = new BuyNowNxtCommon();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SubsEscrowPay";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_SubsEscrowPay();
            var outputApi = new OAPI_SubsEscrowPay();
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

                value = value ?? new Dictionary<string, object>();

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SubsEscrowPay>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                }

                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_GetSubsEscrowPay();
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var splist = msp.sp_GetSubsEscrowPay(spIn, ref errCode);
                    trace.traceAdd("splist", flag);
                    trace.FlowList.Add("sp呼叫");

                    if(splist != null && splist.Count() > 0)
                    {
                        var filter = splist.Where(x => !string.IsNullOrWhiteSpace(x.IDNO)
                          && x.OrderNo > 0
                          && !string.IsNullOrWhiteSpace(x.MemberID)
                          && !string.IsNullOrWhiteSpace(x.AccountId)                         
                          && x.Amount > 0).ToList();

                        int loop = 0;
                        if(filter != null && filter.Count()>0)
                        {
                            foreach(var f in filter)
                            {        
                                if(loop >0)
                                    Thread.Sleep(1000);
                                try
                                {
                                    var outapi_in = new ICF_TSIB_Escrow_PayTransaction()
                                    {
                                        IDNO = f.IDNO,
                                        OrderNo = f.OrderNo,
                                        MemberID = f.MemberID,
                                        AccountId = f.AccountId,
                                        Email = f.Email,
                                        PhoneNo = f.PhoneNo,
                                        Amount = Convert.ToInt16(Math.Floor(f.Amount)),
                                        CreateDate = f.CreateDate,
                                        EcStatus = f.Status,
                                        PRGID=funName
                                    };
                                    mscom.TSIB_Escrow_PayTransaction(outapi_in, ref errCode);
                                    trace.traceAdd("TSIB_Escrow_PayTransaction", new { outapi_in, errCode });                                  
                                }
                                catch(Exception ex)
                                {
                                    trace.traceAdd("Escrow_Exceptiom", new { ex });
                                }

                                loop += 1;
                            }
                            trace.FlowList.Add("履保處理");
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(198, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
