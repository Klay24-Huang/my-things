using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
namespace WebAPI.Controllers
{
    /// <summary>
    /// 點數查詢
    /// </summary>
    public class GetEnterpriseListController : ApiController
    {

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost()]
        public Dictionary<string, object> DoGetEnterpriseList([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetEnterpriseListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetEnterpriseList apiInput = null;
            OAPI_GetEnterpriseList outputApi = new OAPI_GetEnterpriseList();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;


            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetEnterpriseList>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.TaxID))
                {
                    flag = false;
                    errCode = "ERR900";
                }
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
            }
            //開始送短租查詢
            if (flag && apiInput.TaxID.Length == 8)
            {
                WebAPIOutput_EnterpriseList wsOutput = new WebAPIOutput_EnterpriseList();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.EnterpriseList(apiInput.TaxID, ref wsOutput);


                if (flag)
                {
                    if (wsOutput.Data != null)
                    {
                        outputApi.TaxID = wsOutput.Data.TaxID;
                        outputApi.CUSTNM = wsOutput.Data.CUSTNM;

                        if (outputApi.TaxID == "")
                        {
                            errCode = "ERR311";
                            errMsg = "查無企業資料";
                        }
                        else
                        if (wsOutput.Data.depList != null && wsOutput.Data.depList.Count > 0)
                        {
                            outputApi.list = new List<OAPI_GetEnterpriseDept_List>();
                            foreach (var row in wsOutput.Data.depList)
                            {
                                OAPI_GetEnterpriseDept_List l_data = new OAPI_GetEnterpriseDept_List();
                                l_data.DeptNo = row.DeptNo;
                                l_data.DeptName = row.DeptName;
                                outputApi.list.Add(l_data);
                            }
                        }
                    }

                }

            }else
            {
                errCode = "ERR312";
                errMsg = "統一編號格式不符";
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
