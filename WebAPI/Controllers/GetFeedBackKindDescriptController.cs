﻿using Domain.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得回饋類別
    /// </summary>
    public class GetFeedBackKindDescriptController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
        [HttpPost]
        public Dictionary<string, object> DoBookingQuery(Dictionary<string, object> value)
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
            string funName = "GetFeedBackKindDescriptController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetFeedBackKindDescript apiInput = null;
            OAPI_GetFeedBackKind outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, true);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetFeedBackKindDescript>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
            }
            if (flag)
            {
                if (flag)
                {
                    if (apiInput.IsMotor < 0 || apiInput.IsMotor > 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
            }
            //不開放訪客
            if (isGuest)
            {
                flag = false;
                errCode = "ERR101";
            }
            #endregion

            #region TB
            //Token判斷
            //if (flag && isGuest == false)
            //{
            //    string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
            //    SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
            //    {
            //        LogID = LogID,
            //        Token = Access_Token
            //    };
            //    SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
            //    SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
            //    flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            //    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            //    if (flag)
            //    {
            //        IDNO = spOut.IDNO;
            //    }
            //}

            if (flag)
            {
                OtherRepository repository = new OtherRepository(connetStr);
                List<GetFeedBackKindData> lstData = repository.GetFeedBackKind(apiInput.IsMotor);
                if (lstData != null)
                {
                    outputApi = new OAPI_GetFeedBackKind()
                    {
                        DescriptObj = new List<FeedBackKind>()
                    };
                    int len = lstData.Count;
                    for (int i = 0; i < len; i++)
                    {
                        outputApi.DescriptObj.Add(new FeedBackKind()
                        {
                            Descript = lstData[i].Descript,
                            Star = lstData[i].Star,
                            FeedBackKindId = lstData[i].FeedBackKindId
                        });
                    }
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