using Domain.Common;
using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
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
    /// 興聯車機讀卡回報
    /// </summary>
    public class ReadCardInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private Int64 LogID = 0;
        [HttpPost]
        public Dictionary<string, object> ReadCardInfoInsert([FromBody] Dictionary<string, object> value)
        {
            #region 初值設定
            var objOutput = new Dictionary<string, object>();
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ReadCardInfoController";
            string spName = "";
          
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Token token = null;
            IAPI_CENS_ReadCardInfo apiInput = null;
            OAPI_Base outAPI = null;

            #endregion
            #region 基本邏輯判斷
            //0.判斷有無參數傳入
            flag = baseVerify.baseCheck(value, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CENS_ReadCardInfo>(value["para"].ToString());
                //寫入ap log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(value["para"].ToString(), ClientIP, funName, ref errCode, ref LogID);
                //  string[] checkList = { apiInput.CheckKey, apiInput.CarNo };
                //  string[] errList = { "003", "003" };
                //必填判斷
                //  flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName);

            }
            #endregion
            #region 寫入tb
            flag = InsReadCard(apiInput, ref errCode, ref errMsg, ref lstError);
            #endregion
            //邏輯判斷結束
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outAPI, token);
            //       baseVerify.InsAPIOutput(funName, baseVerify.GetClientIp(Request), value["para"].ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(objOutput));
            return objOutput;
        }

        private bool InsReadCard(IAPI_CENS_ReadCardInfo objReadCard, ref string errCode, ref string errMsg, ref List<ErrorInfo> lstError)
        {


            SPOutput_Base spOut = new SPOutput_Base();
            string spName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.InsReadCard);
            DateTime GPSTime = DateTime.Now;
            if (objReadCard.GPSTime != null)
            {
                if (objReadCard.GPSTime >= GPSTime.AddDays(-2) && objReadCard.GPSTime < GPSTime.AddHours(-8))
                {
                    GPSTime = objReadCard.GPSTime.AddHours(8);
                }
                else
                {
                    GPSTime = objReadCard.GPSTime;
                }
            }
            SPInput_InsReadCard spInput = new SPInput_InsReadCard()
            {
                CardNo = objReadCard.CardNo,
                CID = objReadCard.CID,
                GPSTime = GPSTime,
                 Status="",
                 LogID=LogID
            };
            SQLHelper<SPInput_InsReadCard, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsReadCard, SPOutput_Base>(connetStr);

            bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            new CommonFunc().checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            return flag;
        }
    }
}
