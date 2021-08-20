using Domain.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.SP.Output.Member;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 拋轉官網會員同意資料
    /// </summary>
    public class TransWebMemCMKController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoTransWebMemCMK(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "TransWebMemCMKController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_TransWebMemCMK apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            DateTime AgreeDate = DateTime.Now;
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_TransWebMemCMK>(Contentjson);
                // 寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.VerType, apiInput.Version, apiInput.Source, apiInput.AgreeDate, apiInput.TEL, apiInput.SMS, apiInput.EMAIL, apiInput.POST };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                // 判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    // 檢查日期格式
                    flag = DateTime.TryParse(apiInput.AgreeDate, out AgreeDate);
                    if (!flag)
                    {
                        errCode = "ERR900";
                    }
                }
            }
            #endregion

            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.SetMemberCMK);
                SPInput_SetMemberCMK spInput = new SPInput_SetMemberCMK()
                {
                    IDNO = apiInput.IDNO,
                    Source = apiInput.Source,
                    AgreeDate = AgreeDate,
                    VerType = apiInput.VerType,
                    Version = apiInput.Version,
                    TEL = apiInput.TEL,
                    SMS = apiInput.SMS,
                    EMAIL = apiInput.EMAIL,
                    POST = apiInput.POST,
                    APIName = funName,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SetMemberCMK, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetMemberCMK, SPOutput_Base>(connetStr);
                List<MemberCMKList> ListOut = new List<MemberCMKList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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
