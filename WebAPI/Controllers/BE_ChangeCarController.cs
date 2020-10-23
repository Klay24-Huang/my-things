using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】換車
    /// </summary>
    public class BE_ChangeCarController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】換車
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_ChangeCar(Dictionary<string, object> value)
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
            string funName = "BE_ChangeCarController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_BookingCancel apiInput = null;
            OAPI_BE_ChangeCar apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            DateTime ED = DateTime.Now;
            Int64 tmpOrder = 0;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_BookingCancel>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo};
                string[] errList = { "ERR900", "ERR900"};
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                  
                    if (flag)
                    {
                        if (apiInput.OrderNo.IndexOf("H") < 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                            if (flag)
                            {
                                if (tmpOrder <= 0)
                                {
                                    flag = false;
                                    errCode = "ERR900";
                                }

                            }
                        }
                    }
                }


            }
            #endregion

            #region TB

            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.BE_ChangeCar);
                SPInput_BE_ChangeCar spInput = new SPInput_BE_ChangeCar()
                {
                    LogID = LogID,

                    OrderNo = tmpOrder,
                    UserID = apiInput.UserID


                };
                SPOutput_BE_ChangeCar spOut = new SPOutput_BE_ChangeCar();
                SQLHelper<SPInput_BE_ChangeCar, SPOutput_BE_ChangeCar> sqlHelp = new SQLHelper<SPInput_BE_ChangeCar, SPOutput_BE_ChangeCar>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput,ref  spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,  spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    apiOutput = new OAPI_BE_ChangeCar()
                    {
                        NewCarNo = spOut.NewCarNo
                    };
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
