﻿using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;


namespace WebAPI.Controllers
{
    public class BE_HandleMemberScoreController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】新增/修改使用者
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_HandleMemberScore(Dictionary<string, object> value)
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
            string funName = "BE_HandleMemberScoreController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleMemberScore apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleMemberScore>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.SCORE, apiInput.APP };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                //if (flag)
                //{
                //    if (apiInput.Power.Count > 0)
                //    {
                //        PowerStr = JsonConvert.SerializeObject(apiInput.Power);
                //    }
                //}

            }
            #endregion

            #region TB

            if (flag)
            {

                string spName = "usp_BE_UpdateMemberScore";
                SPInput_BE_MemberScore spInput = new SPInput_BE_MemberScore()
                {
                    IDNO = apiInput.IDNO,
                    SCORE = int.Parse(apiInput.SCORE),
                    APP = apiInput.APP,
                    SON = apiInput.SON,
                    USERID = apiInput.UserID,
                    SEQ = apiInput.SEQ
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_MemberScore, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_MemberScore, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);


            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);

            return objOutput;
            #endregion
        }
    }
}
