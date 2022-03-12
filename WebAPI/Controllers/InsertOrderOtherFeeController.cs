﻿using Domain.Common;
using Domain.SP.Input.Bill;
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
    /// <summary>
    /// 營損明細新增
    /// </summary>
    public class InsertOrderOtherFeeController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 營損明細新增
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoInsertOrderOtherFee(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "InsertOrderOtherFeeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_InsertOrderOtherFee apiInput = null;
            NullOutput apiOutput = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_InsertOrderOtherFee>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.IRENTORDNO };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {

                    if (flag)
                    {
                        if (apiInput.IRENTORDNO.IndexOf("H") < 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            flag = Int64.TryParse(apiInput.IRENTORDNO.Replace("H", ""), out tmpOrder);
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

                string spName = "usp_InsertOrderOtherFee";
                SPInput_InsertOrderOtherFee spInput = new SPInput_InsertOrderOtherFee()
                {
                    LogID = LogID,
                    UserID = apiInput.UserID,
                    IRENTORDNO = int.Parse(apiInput.IRENTORDNO.Substring(1)),
                    CarDispatch = apiInput.CarDispatch,
                    DispatchRemark = apiInput.DispatchRemark,
                    CNTRNO = apiInput.CNTRNO,
                    ParkingFee = apiInput.ParkingFee,
                    ParkingFeeRemark = apiInput.ParkingFeeRemark
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_InsertOrderOtherFee, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsertOrderOtherFee, SPOutput_Base>(connetStr);
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