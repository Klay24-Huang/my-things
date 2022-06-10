using Domain.Common;
using Domain.SP.Input.Contract;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_SaveContractSettingHisController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> doBE_SaveContractSettingHis(Dictionary<string, object> value)
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
            string funName = "BE_SaveContractSettingHisController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_SaveContractSettingHis apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            //DateTime SD = DateTime.Now, ED = DateTime.Now;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_SaveContractSettingHis>(Contentjson);              
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo.ToString(), apiInput.type.ToString(), apiInput.returnDate.ToString(), apiInput.mode.ToString()};
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900"};
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

            }
            #endregion
            #region TB

            if (flag)
            {

                string spName = "usp_BE_InsContractSettingHis";
                SPInput_InsContractSettingHis spInput = new SPInput_InsContractSettingHis()
                {
                    A_USER = apiInput.UserID,
                    OrderNo = apiInput.OrderNo,
                    Type = apiInput.type,
                    ReturnTime = apiInput.returnDate,
                    Reason = apiInput.mode,
                    Memo = apiInput.mode_input,
                    DiscountType = apiInput.Discount,
                    Discount_C = apiInput.timeDiscount_car,
                    Discount_M = apiInput.timeDiscount_motor,
                    MemberScore = apiInput.score_input,
                    MemberScoreType = apiInput.score,
                    CostRelief_Cost = apiInput.costRelife_cost,
                    CostRelief_Minute = apiInput.costRelife_minute,
                    CostRelief_Memo = apiInput.costRelife_input,
                    NotUseCar = apiInput.notUseCar ? 1 : 0,
                    CancelOvertime = apiInput.cancelOvertime ? 1 : 0,
                    CarrierType = apiInput.bill_option,
                    Business_No = apiInput.unified_business_no,
                    Parking = apiInput.parkingSpace

                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_InsContractSettingHis, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsContractSettingHis, SPOutput_Base>(connetStr);
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