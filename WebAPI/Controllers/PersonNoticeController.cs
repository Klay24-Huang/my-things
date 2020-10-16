﻿using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Other;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// 個人訊息
    /// </summary>
    public class PersonNoticeController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoPersonNotice(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "PersonNoticeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_PersonNotice apiInput = null;
            OAPI_PersonNotice outputApi = null;
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            Int16 tmpType = 0;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_PersonNotice>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                //類型判斷
                if (flag)
                {
                    if (null == apiInput.type)
                    {
                        apiInput.type = 0;
                    }
                    else
                    {
                        string tmp = apiInput.type.ToString();

                        flag = Int16.TryParse(tmp, out tmpType);
                        if (false == flag)
                        {
                            errCode = "ERR900";
                        }
                        else
                        {
                            if (tmpType < 0 || tmpType > 1)
                            {
                                errCode = "ERR900";
                                flag = false;
                            }
                            else
                            {
                                apiInput.type = tmpType;
                            }
                        }
                    }
                }

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
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }
            if (flag)
            {
                SPInput_GetNotificationList spInput = new SPInput_GetNotificationList()
                {
                    Token=Access_Token,
                     LogID=LogID,
                    IDNO = IDNO,
                    Type = tmpType
                };
                List<GetNotification> lstOut = new List<GetNotification>();
                string spName = new ObjType().GetSPName(ObjType.SPType.PersonNotice);
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetNotificationList, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetNotificationList, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref lstOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                if (flag)
                {
                    outputApi = new OAPI_PersonNotice();
                    outputApi.PersonNoticeObj = new List<GetNotification>();
                    outputApi.PersonNoticeObj = lstOut;
                }
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