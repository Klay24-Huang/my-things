using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Member;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.SP.Output.Member;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 設定密碼
    /// </summary>
    public class Register_Step2Controller : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doRegister_Step2(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "Register_Step2Controller";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_Register_Step2 apiInput = null;
            NullOutput ApiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            int MEMRFNBR = 0;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_Register_Step2>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.PWD, apiInput.DeviceID, apiInput.APPVersion };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                }
                if (flag)
                {
                    if (apiInput.APPVersion.Split('.').Count() < 3)
                    {
                        flag = false;
                        errCode = "ERR104";
                    }
                }
                if (flag)
                {
                    flag = (apiInput.APP.HasValue);
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        APPKind = apiInput.APP.Value;
                        if (APPKind < 0 || APPKind > 1)
                        {
                            flag = false;
                            errCode = "ERR105";
                        }
                    }
                }
            }
            #endregion

            #region TB
            #region 取得MEMRFNBR
            //20201125 ADD BY ADAM REASON.寫入帳號前先去短租那邊取得MEMRFNBR
            if (flag)
            {
                WebAPIOutput_NPR013Reg wsOutput = new WebAPIOutput_NPR013Reg();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR013Reg(apiInput.IDNO, "", ref wsOutput);
                if (flag)
                {
                    if (wsOutput.Data.Length > 0)
                    {
                        MEMRFNBR = wsOutput.Data[0].MEMRFNBR == "" ? 0 : MEMRFNBR_FromStr(wsOutput.Data[0].MEMRFNBR);
                    }
                }
            }
            #endregion
            #region 設定密碼
            if (flag)
            {
                string spName = "usp_Register_Step2_U01";
                SPInput_Register_Step2 spInput = new SPInput_Register_Step2()
                {
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    PWD = apiInput.PWD,
                    MEMRFNBR = MEMRFNBR,
                    DeviceID = apiInput.DeviceID,
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Register_Step2, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Register_Step2, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, ApiOutput, token);
            return objOutput;
            #endregion
        }
        private int MEMRFNBR_FromStr(string sour)
        {
            if (double.TryParse(sour, out double d_sour))
                return Convert.ToInt32(Math.Floor(d_sour));
            else
                throw new Exception("MEMRFNBR格式錯誤");
        }
    }
}