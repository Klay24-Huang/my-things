using Domain.Common;
using Domain.SP.Input;
using Domain.SP.Output;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetBannerController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpGet]
        public Dictionary<string, object> DoGetBanner()
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetBannerController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            OAPI_GetBanner OutBannerList = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            #endregion
            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("no Input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB
            if (flag)
            {
                try
                {
                    string spName = new ObjType().GetSPName(ObjType.SPType.GetBanner);
                    SPInput_Base spInput = new SPInput_Base()
                    {
                        LogID = LogID
                    };
                    SPOutput_Base spOut = new SPOutput_Base();
                    SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
                    List<BannerData> BannerList = new List<BannerData>();
                    DataSet ds = new DataSet();
                    flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref BannerList, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    if (flag)
                    {
                        OutBannerList = new OAPI_GetBanner
                        {
                            BannerObj = BannerList
                        };
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errMsg = ex.Message;
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, OutBannerList, token);
            return objOutput;
            #endregion
        }
    }
}