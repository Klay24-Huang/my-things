using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
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
    /// 短租補傳
    /// </summary>
    public class BE_HiEasyRentRetryController : ApiController
    {

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】短租補傳
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
            string funName = "BE_HiEasyRentRetryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HiEasyRetry apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            HiEasyRentRepository rentRepository = new HiEasyRentRepository(connetStr);
            HiEasyRentAPI WebAPI = new HiEasyRentAPI();
            int retryMode = 0;
            string ORDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HiEasyRetry>(Contentjson);
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
                if (flag)
                {
                    if (apiInput.Type < 1 || apiInput.Type > 3)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleHiEasyRentRetry);
                SPInput_BE_HandleHiEasyRentRetry spInput = new SPInput_BE_HandleHiEasyRentRetry()
                {
                    LogID = LogID,
                     Mode=Convert.ToInt16(apiInput.Type),
                    OrderNo = tmpOrder,
                    UserID = apiInput.UserID


                };
                SPOutput_BE_HandleHiEasyRentRetry spOut = new SPOutput_BE_HandleHiEasyRentRetry();
                SQLHelper<SPInput_BE_HandleHiEasyRentRetry, SPOutput_BE_HandleHiEasyRentRetry> sqlHelp = new SQLHelper<SPInput_BE_HandleHiEasyRentRetry, SPOutput_BE_HandleHiEasyRentRetry>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    retryMode = spOut.ReturnMode;
                }
                if (flag)
                {
                    if (retryMode == 1)
                    {
                        BE_BookingControl obj = rentRepository.GetBookingControl(tmpOrder);
                        if (obj != null)
                        {
                            WebAPIInput_NPR060Save input = new WebAPIInput_NPR060Save()
                            {
                                CARNO = obj.CARNO,
                                CARRIERID = obj.CARRIERID,
                                CARTYPE = obj.CARTYPE,
                                DISRATE = obj.DISRATE.ToString(),
                                EBONUS = obj.EBONUS.ToString(),
                                GIVEDATE = obj.GIVEDATE,
                                GIVETIME = obj.GIVETIME,
                                INBRNH = obj.INBRNH,
                                INSUAMT = obj.INSUAMT.ToString(),
                                INVKIND = obj.INVKIND.ToString(),
                                INVTITLE = obj.INVTITLE,
                                NETRPRICE = obj.NETRPRICE.ToString(),
                                NOCAMT = obj.NOCAMT.ToString(),
                                NPOBAN = obj.NPOBAN,
                                ODCUSTID = obj.ODCUSTID,
                                ODCUSTNM = obj.ODCUSTNM,
                                ODDATE = obj.ODDATE,
                                ORDAMT = obj.ORDAMT.ToString(),
                                ORDNO = string.Format("H{0}",obj.OrderNo.ToString().PadLeft(7,'0')),
                                OUTBRNH = obj.OUTBRNH,
                                PAYAMT = obj.PAYAMT.ToString(),
                                PROCD =obj.PROCD,
                                PROJTYPE = obj.PROJTYPE,
                                REMARK = obj.REMARK,
                                RENTDAY = obj.RENTDAY.ToString(),
                                RINV = obj.RINV.ToString(),
                                RNTAMT = obj.RNTAMT.ToString(),
                                RNTDATE = obj.RNTDATE,
                                RNTTIME = obj.RNTTIME,
                                RPRICE = obj.RPRICE.ToString(),
                                TEL1 = obj.TEL1,
                                TEL2 = obj.TEL2,
                                TEL3 = obj.TEL3,
                                TSEQNO = obj.TSEQNO.ToString(),
                                TYPE = obj.TYPE.ToString(),
                                UNIMNO = obj.UNIMNO
                                
                            };
                            WebAPIOutput_NPR060Save output = new WebAPIOutput_NPR060Save();
                            flag = WebAPI.NPR060Save(input, ref output);
                            if (output.Result == false)
                            {
                                flag = false;
                                errCode = "ERR";
                                errMsg = output.Message;

                            }
                            else
                            {
                                ORDNO = output.Data[0].ORDNO;
                            }

                            bool saveFlag = DoSave060Data(tmpOrder, ORDNO, Convert.ToInt16((output.Result)?1:0), LogID, ref lstError, ref errCode);
                            if (saveFlag)
                            {
                                if (apiInput.Type == 2)
                                {

                                }
                            }
                        }
                        else
                        {
                            flag = false;
                            errCode = "ERR";
                        }
                    }else if (retryMode == 2) //125
                    {
                        BE_LandControl obj= rentRepository.GetLandControl(tmpOrder);
                        if (obj != null)
                        {
                            WebAPIInput_NPR125Save input = new WebAPIInput_NPR125Save()
                            {
                                BIRTH = obj.BIRTH,
                                CARNO = obj.CARNO,
                                CARRIERID = obj.CARRIERID,
                                CARTYPE = obj.CARTYPE,
                                CUSTID = obj.CUSTID,
                                CUSTNM = obj.CUSTNM,
                                CUSTTYPE = "1",
                                DISRATE = "1",
                                GIVEDATE = obj.GIVEDATE,
                                GIVEKM = obj.GIVEKM.ToString(),
                                GIVETIME = obj.GIVETIME,
                                INBRNHCD = obj.INBRNHCD.ToString(),
                                INVADDR = obj.INVADDR,
                                INVKIND = obj.INVKIND.ToString(),
                                INVTITLE = obj.INVTITLE,
                                IRENTORDNO = string.Format("H{0}", obj.IRENTORDNO.ToString().PadLeft(7, '0')),
                                LOSSAMT2 = obj.LOSSAMT2.ToString(),
                                NOCAMT = obj.NOCAMT.ToString(),
                                NPOBAN = obj.NPOBAN,
                                ODCUSTID = obj.CUSTID,
                                ORDNO = obj.ORDNO,
                                OUTBRNHCD = obj.OUTBRNHCD,
                                OVERAMT2 = obj.OVERAMT2.ToString(),
                                OVERHOURS = obj.OVERHOURS.ToString(),
                                PROCD = obj.PROCD,
                                PROJID = obj.PROJID,
                                REMARK = "",
                                RENTAMT = obj.RENTAMT.ToString(),
                                RENTDAYS = obj.RENTDAYS.ToString(),
                                RINSU = "0",
                                RNTAMT = obj.RPRICE.ToString(),
                                RNTDATE = obj.RNTDATE,
                                RNTKM = obj.GIVEKM.ToString(),
                                RNTTIME = obj.RNTTIME,
                                RPRICE = obj.RPRICE.ToString(),
                                TSEQNO = obj.TSEQNO,
                                UNIMNO = obj.UNIMNO

                            };
                            WebAPIOutput_NPR125Save output = new WebAPIOutput_NPR125Save();
                            flag = WebAPI.NPR125Save(input, ref output);
                            if (flag)
                            {
                                if (output.Result == false)
                                {
                                    flag = false;
                                    errCode = "ERR";
                                    errMsg = output.Message;

                                }
                               

                                bool saveFlag = DoSave125Data(tmpOrder, Convert.ToInt16((output.Result) ? 1 : 0), LogID, ref lstError, ref errCode);
                            }
                        }
                    }
                    else if(retryMode == 3) //130
                    {

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
        private bool DoSave060Data(Int64 OrderNo,string ORDNO,Int16 IsSuccess,Int64 LogID,ref List<ErrorInfo> lstError,ref string errCode)
        {
            bool flag = true;
            string spName = new ObjType().GetSPName(ObjType.SPType.BE_BookingControlSuccess);
            SPInput_BE_BookingControlSuccess spInput = new SPInput_BE_BookingControlSuccess()
            {
                IsSuccess = IsSuccess,
                LogID = LogID,
                OrderNo = OrderNo,
                ORDNO = ORDNO
            };
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_BE_BookingControlSuccess, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_BookingControlSuccess, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;

        }
        private bool DoSave125Data(Int64 OrderNo,Int16 IsSuccess, Int64 LogID, ref List<ErrorInfo> lstError, ref string errCode)
        {
            bool flag = true;
            string spName = new ObjType().GetSPName(ObjType.SPType.BE_LandControlSuccess);
            SPInput_BE_LandControlSuccess spInput = new SPInput_BE_LandControlSuccess()
            {
                IsSuccess = IsSuccess,
                LogID = LogID,
                OrderNo = OrderNo
            };
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_BE_LandControlSuccess, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_LandControlSuccess, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;

        }

    }
}
