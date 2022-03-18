using Domain.Common;
using Domain.SP.Input.Subscription;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得訂閱月租合約明細
    /// </summary>
    public class GetSubsCNTController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetSubsCNT([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var map = new MonSunsVMMap();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetSubsCNTController";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetSubsCNT();
            var outputApi = new OAPI_GetSubsCNT();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int MonType = 2;//暫時只有訂閱制月租

            #endregion

            if (value == null)
                value = new Dictionary<string, object>();

            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetSubsCNT>(Contentjson);
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
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) || apiInput.MonProPeriod == 0)
                        {
                            flag = false;
                            errCode = "ERR257";
                        }
                    }
                }

                #endregion

                #region token
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    trace.FlowList.Add("Token判斷");
                }
                #endregion

                #region TB
                if (flag)
                {
                    var spIn = new SPInput_GetSubsCNT()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonProjID = apiInput.MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays,
                        SetNow = null
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var sp_re = msp.sp_GetSubsCNT(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);
                    if (sp_re != null)
                    {
                        if (sp_re.NowCard != null)
                            outputApi.NowCard = map.FromSPOut_GetSubsCNT_NowCard(sp_re.NowCard);

                        if (flag)
                        {
                            if (sp_re.NxtCard != null)
                            {
                                outputApi.NxtCard = map.FromSPOut_GetSubsCNT_NxtCard(sp_re.NxtCard);
                                //outputApi.NxtCard.StartDate = sp_re.NxtCard.SD.ToString("yyyy/MM/dd");
                                //outputApi.NxtCard.EndDate = sp_re.NxtCard.ED.ToString("yyyy/MM/dd");
                                //20210611 ADD BY ADAM 
                                outputApi.NxtCard.StartDate = sp_re.NxtCard.SD.ToString("yyyy/MM/dd HH:mm");
                                outputApi.NxtCard.EndDate = sp_re.NxtCard.ED.ToString("HHmm") == "0000" ? sp_re.NxtCard.ED.AddMinutes(-1).ToString("yyyy/MM/dd HH:mm") : sp_re.NxtCard.ED.ToString("yyyy/MM/dd HH:mm");
                            }
                        }
                    }

                    trace.traceAdd("outputApi", outputApi);
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(186, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}