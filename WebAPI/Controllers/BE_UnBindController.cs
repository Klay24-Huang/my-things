using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.WebAPI.Input.CENS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】會員卡號解除
    /// </summary>
    public class BE_UnBindController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】取得車輛列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_UnBind(Dictionary<string, object> value)
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
            string funName = "BE_UnBindController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_UnBind apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_UnBind>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.IDNO, apiInput.OrderNo };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填 //20201208這邊判斷false
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                }
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
            //20201208唐加
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_GetCarMachineAndCheckOrder);
                SPInput_BE_GetCarMachineAndCheckOrder spInput = new SPInput_BE_GetCarMachineAndCheckOrder()
                {
                    LogID = Convert.ToInt32(apiInput.UserID),//LogID, //20201208唐改
                    IDNO = apiInput.IDNO,
                    OrderNo = Convert.ToInt32(apiInput.OrderNo)//tmpOrder //20201208唐改
                };
                SPOutput_BE_GetCarMachineAndCheckOrder spOut = new SPOutput_BE_GetCarMachineAndCheckOrder();
                SQLHelper<SPInput_BE_GetCarMachineAndCheckOrder, SPOutput_BE_GetCarMachineAndCheckOrder> sqlHelp = new SQLHelper<SPInput_BE_GetCarMachineAndCheckOrder, SPOutput_BE_GetCarMachineAndCheckOrder>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    #region 車機
                    CarCommonFunc CarComm = new CarCommonFunc();
                    if (spOut.IsCens == 1)
                    {
                        SendCarNoData[] sendCarNoDatas = new SendCarNoData[1];
                        SendCarNoData obj = new SendCarNoData()
                        {
                            CardNo = spOut.CardNo,
                            CardType = 1
                        };
                        sendCarNoDatas[0] = obj;
                        flag = CarComm.DoSetCensCustomerCard(spOut.CID, sendCarNoDatas, 0, ref errCode);
                    }
                    else
                    {
                        flag = CarComm.DoSetFETCustomerCard(spOut.CID, spOut.deviceToken, new string[] { spOut.CardNo }, 0, LogID, ref errCode);
                    }
                    #endregion
                }
            }
            #endregion
            //20201208這邊會寫入log，但沒看到寫入，要研究commonfunc，我先跳過
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