using Domain.Common;
using Domain.SP.Input.Hotai;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.Input.CTBCPOS;
using Domain.WebAPI.output.CTBCPOS;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
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
    /// 查詢中信訂單狀態
    /// </summary>
    public class CTBCInquiryController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        private CommonFunc BaseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoCTBCInquiry(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "CTBCInquiryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string Contentjson = "";
            bool isGuest = true;

            NullOutput apiOutput = new NullOutput();
            Token token = null;
            BaseVerify = new CommonFunc();
            IAPI_CTBCInquiry apiInput = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<WebAPIInput_InquiryByLidm> inquiryList = null;
            CTBCPosAPI posAPI = new CTBCPosAPI();
            #endregion
            #region 防呆
            bool flag = BaseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CTBCInquiry>(Contentjson);
                string ClientIP = BaseVerify.GetClientIp(Request);
                flag = BaseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);
            }
            if (flag)
            {

                apiInput.QueryBgn = apiInput.QueryBgn == "" ? "20220101" : apiInput.QueryBgn;
                apiInput.QueryEnd = apiInput.QueryEnd == "" ? DateTime.Now.ToString("yyyyMMdd") : apiInput.QueryEnd;

                if (!DateTime.TryParseExact(apiInput.QueryBgn, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime SD) || !DateTime.TryParseExact(apiInput.QueryEnd, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime ED) || DateTime.Compare(ED, SD) < 0)
                {
                    flag = false;
                    errCode = "ERR907";
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                inquiryList = GetCTBCQueryList(apiInput, funName, ref flag, ref lstError, ref errCode);
            }

            if (flag)
            {
                foreach (var inquiry in inquiryList)
                {
                    try
                    {
                        WebAPIInput_InquiryByLidm quiryInput = new WebAPIInput_InquiryByLidm()
                        {
                            OrderID = inquiry.OrderID
                        };

                        WebAPIOutput_InquiryByLidm quiryOutput = new WebAPIOutput_InquiryByLidm();
                        flag = posAPI.QueryCTBCTransaction(quiryInput, out quiryOutput);


                        if (flag)
                        {
                            SP_Input_CTBCInquiry spInput = new SP_Input_CTBCInquiry()
                            {
                                PRGName = funName,
                                BatchId = quiryOutput.BatchId,
                                BatchSeq = quiryOutput.BatchSeq,
                                Xid = quiryOutput.XID,
                                CurrentState = quiryOutput.CurrentState
                            };

                            flag = UpdateCTBCQueryStatus(spInput, ref lstError, ref errCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("CTBCQueryListloop Error:" + ex.Message);
                    }
                }

            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                BaseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            BaseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        //取得查詢訂單清單
        private List<WebAPIInput_InquiryByLidm> GetCTBCQueryList(IAPI_CTBCInquiry input, string funName, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var capList = new List<WebAPIInput_InquiryByLidm>();

            SP_Input_GetCTBCInquiryList spInput = new SP_Input_GetCTBCInquiryList()
            {
                PRGName = funName,
                QueryBgn = input.QueryBgn,
                QueryEnd = input.QueryEnd
            };

            string SPName = "usp_CTBCInquiry_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SP_Input_CTBCCapBase, SPOutput_Base> sqlHelpQuery = new SQLHelper<SP_Input_CTBCCapBase, SPOutput_Base>(connetStr);

            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref capList, ref ds, ref lstError);
            BaseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (capList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203"; //請款找不到符合的訂單編號
                }
            }
            return capList;
        }

        //更新訂單狀態
        private bool UpdateCTBCQueryStatus(SP_Input_CTBCInquiry input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_CTBCInquiry_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SP_Input_CTBCInquiry, SPOutput_Base> SQLPayHelp = new SQLHelper<SP_Input_CTBCInquiry, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            BaseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }

    }
}