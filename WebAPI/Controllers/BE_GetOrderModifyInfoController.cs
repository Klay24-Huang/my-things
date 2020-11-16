using Domain.CarMachine;
using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】修改合約前取得資料
    /// </summary>
    public class BE_GetOrderModifyInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】修改合約前取得資料
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_GetOrderModifyInfo(Dictionary<string, object> value)
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
            string funName = "BE_GetOrderModifyInfoController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_GetOrderModifyInfo apiInput = null;
            OAPI_BE_GetOrderModifyInfo apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            bool clearFlag = false;
            DateTime SD = DateTime.Now, ReturnDate = DateTime.Now;
            List<BE_CarScheduleTimeLog> lstOrder = null;

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_GetOrderModifyInfo>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

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
            #endregion

            #region TB

            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderInfoBeforeModify);
                SPOutput_BE_GetOrderInfoBeforeModify spOut = new SPOutput_BE_GetOrderInfoBeforeModify();
                SPInput_BE_GetOrderInfoBeforeModify spInput = new SPInput_BE_GetOrderInfoBeforeModify()
                {
                   UserID=apiInput.UserID,
                    LogID = LogID,
                    OrderNo = tmpOrder
                };
              
                SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetOrderInfoBeforeModify> sqlHelp = new SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetOrderInfoBeforeModify>(connetStr);
                //flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                List<BE_GetFullOrderData> OrderDataLists = new List<BE_GetFullOrderData>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref OrderDataLists, ref ds, ref lstError);
                new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    apiOutput = new OAPI_BE_GetOrderModifyInfo()
                    {
                         LastOrderData=new LastOrderInfo()
                         {
                              LastEndMile=spOut.LastEndMile,
                               LastStartTime=spOut.LastStartTime,
                                LastStopTime=spOut.LastStopTime
                         },
                        ModifyLog = new ModifyInfo()
                        {
                            hasModify = spOut.hasModify,
                            ModifyTime = spOut.ModifyTime,
                            ModifyUserID = spOut.ModifyUserID
                        },
                        
                        OrderData = new BE_GetFullOrderData()
                    };
                    if (OrderDataLists != null)
                    {
                        if (OrderDataLists.Count > 0)
                        {
                            apiOutput.OrderData = OrderDataLists[0];
                            
                        }
                    }
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
