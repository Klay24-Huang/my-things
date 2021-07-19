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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace WebAPI.Controllers
{
    public class MultiCallApiController : ApiController
    {
        [HttpPost()]
        public async Task< Dictionary<string, object>> DoMultiCallApi([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var fix_com = new DataFixCommon();
            var fix_sp = new DataFixSp();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "DoMultiCallApi";
            Int64 LogID = 0;
            var apiInput = new IAPI_MultiCallApi();
            var outputApi = new OAPI_MultiCallApi();
            outputApi.ApiErrList = new List<ApiErrMsg>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;

            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                value = value ?? new Dictionary<string, object>();

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MultiCallApi>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                }

                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_GetDataFix();
                    trace.traceAdd("spIn", spIn);
                    //取出修正列表
                    var splist = fix_sp.sp_GetDataFix(spIn, ref errCode);
                    trace.traceAdd("splist", flag);
                    trace.FlowList.Add("sp呼叫");

                    var ProjTypes = new List<int>() { 0, 3, 4 };

                    if (splist != null && splist.Count() > 0)
                    {
                        var filter = splist.Where(x => !string.IsNullOrWhiteSpace(x.PROJID)
                          && ProjTypes.Any(y=>y == x.PROJTYPE)
                          && (x.PROJTYPE == 4 ? x.MinuteOfPrice >0: (x.PRICE > 0 && x.PRICE_H > 0))
                          ).ToList();

                        int loop = 0;
                        if (filter != null && filter.Count() > 0)
                        {
                            bool xFlag = false;
                            foreach (var f in filter)
                            {
                                if (loop > 0)
                                    Thread.Sleep(1000);
                                try
                                {
                                    xFlag = false;
                                    var api_in = new IAPI_ReDoGetPayDetail()
                                    {
                                        IDNO = f.IDNO,
                                        OrderNo = "H" + f.OrderNo.ToString(),
                                        Discount = f.Discount,
                                        MotorDiscount = f.MotorDiscount,
                                        PROJID = f.PROJID,
                                        PROJTYPE = f.PROJTYPE,
                                        PRICE = Convert.ToInt32(f.PRICE/10),
                                        PRICE_H = Convert.ToInt32(f.PRICE_H/10),
                                        MinuteOfPrice = f.MinuteOfPrice
                                    };

                                    var api_out = new OAPI_ReDoGetPayDetail();
                                    var pos_re = await fix_com.CallReDoGetPayDetail(api_in);
                                    if (pos_re != null)
                                    {
                                        xFlag = pos_re.Item1;
                                        api_out = pos_re.Item2;
                                    }

                                    if (!xFlag) 
                                    {
                                        var apiErr = new ApiErrMsg()
                                        {
                                            OrderNo = api_in.OrderNo,
                                            InJson = JsonConvert.SerializeObject(api_in),
                                            OutJson = JsonConvert.SerializeObject(api_out.ApiResult)
                                        };
                                        outputApi.ApiErrList.Add(apiErr);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var apiErr = new ApiErrMsg()
                                    {
                                        OrderNo = f.OrderNo.ToString(),
                                        InJson = JsonConvert.SerializeObject(f),
                                        OutJson = ex.Message
                                    };
                                    outputApi.ApiErrList.Add(apiErr);
                                }

                                loop += 1;
                            }
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
