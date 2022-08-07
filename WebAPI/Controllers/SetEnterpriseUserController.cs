﻿using Domain.Common;
using Domain.SP.Input.Enterprise;
using Domain.SP.Output;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class SetEnterpriseUserController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoSetEnterpriseUser(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SetEnterpriseUserController";
            Int64 LogID = 0;

            IAPI_SetEnterpriseUserMode apiInput = null;
            NullOutput outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_SetEnterpriseUserMode>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
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
            //統一編號未輸入
            if (flag)
            {
                if (string.IsNullOrEmpty(apiInput.TaxID))
                {
                    flag = false;
                    errCode = "ERR190";
                }
                else
                {
                    if (apiInput.TaxID.Length == 8)
                    {
                        flag = baseVerify.checkUniNum(apiInput.TaxID);
                        if (!flag)
                        {
                            flag = false;
                            errCode = "ERR191";
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR191";
                    }
                }
            }
            if (flag)
            {
                if (string.IsNullOrEmpty(apiInput.MEMCNAME))
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            //公司名稱未輸入
            if (flag)
            {
                if (string.IsNullOrEmpty(apiInput.CompanyName))
                {
                    flag = false;
                    errCode = "ERR406";
                }
            }
            #endregion

            #region TB
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            if (flag)
            {
                string SpName = "usp_SetEnterpriseUser";
                SPInput_EnterpriseUser SPInput = new SPInput_EnterpriseUser()
                {
                    Token = Access_Token,
                    IDNO = IDNO,
                    ApiName = funName,
                    LogID = LogID,
                    CompanyName = apiInput.CompanyName,
                    Depart = apiInput.Depart,
                    EmployeeID = apiInput.EmployeeID,
                    MEMCNAME = apiInput.MEMCNAME,
                    TaxID = apiInput.TaxID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_EnterpriseUser, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_EnterpriseUser, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SpName, SPInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 寫入錯誤Log
            if (!flag)
            {
                baseVerify.InsErrorLog(funName, errCode, 0, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}