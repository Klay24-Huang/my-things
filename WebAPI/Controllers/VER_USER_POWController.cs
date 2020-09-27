using Domain.Common;
using Domain.SP.Input.Battle;
using Domain.SP.Output.Battle;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 維護人員及廠商驗證
    /// </summary>
    public class VER_USER_POWController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> VER_USER_POW([FromBody] Dictionary<string, object> value)
        {
            #region 初值設定
            var objOutput = new Dictionary<string, object>();
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "VER_USER_POWController";
            string spName = "";
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            IAPI_VER_USER_POW apiInput = null;
            OAPI_VER_USER_POW outAPI = null;
            Token token = null;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string Contentjson = "";
            string SPName = "";
            #endregion
            #region 防呆
            string ClientIP = baseVerify.GetClientIp(Request);

            flag = baseVerify.baseCheck(value, ref errCode, funName);
            if (flag)
            {
                //寫入API Log

                flag = baseVerify.InsAPLog(value["para"].ToString(), ClientIP, funName, ref errCode, ref LogID);
                Contentjson = value["para"].ToString();
            }

            if (flag)
            {
                string[] checkList = { apiInput.VerID, apiInput.VerPwd };
                string[] errList = { "003", "003" };
                //必填判斷
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
               
            }


            #endregion
            #region 查詢金鑰
            if (flag)
            {
                try
                {
                  
                    spName = new ObjType().GetSPName(ObjType.SPType.GetMaintainKey);
                    SPInput_GetKey spInput = new SPInput_GetKey()
                    {
                        VerID = apiInput.VerID,
                        VerPwd = apiInput.VerPwd
                    };
                    SPOutput_GetKey spOutput = new SPOutput_GetKey();
                    SQLHelper<SPInput_GetKey, SPOutput_GetKey> sqlHelper = new SQLHelper<SPInput_GetKey, SPOutput_GetKey>(connetStr);
                    //  List<BookingControl> lstbookControl = new List<BookingControl>();
                    flag = sqlHelper.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOutput.Error, spOutput.ErrorCode, ref lstError, ref errCode);
                    if (flag)
                    {
                        outAPI = new OAPI_VER_USER_POW()
                        {
                            CheckKey = spOutput.Key
                        };

                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errMsg = ex.InnerException.Message;
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                    isWriteError = true;
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outAPI, token);
            return objOutput;
            #endregion
        }
    }
}
