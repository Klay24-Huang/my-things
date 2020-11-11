using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】判斷使用者群組編號是否存在
    /// </summary>
    public class BE_CheckUserGroupController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 判斷使用者群組編號是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_CheckUserGroup(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_CheckUserGroupController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_CheckUserGroup apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_CheckUserGroup>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.UserGroupID };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);


            }
            #endregion
            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_CheckUserGroup);
                SPInput_BE_CheckUserGroup spInput = new SPInput_BE_CheckUserGroup()
                {
                    LogID = LogID,
                    UserGroupID = apiInput.UserGroupID,
                    OperatorID=apiInput.OperatorID,
                    UserID = apiInput.UserID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_CheckUserGroup, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_CheckUserGroup, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
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

