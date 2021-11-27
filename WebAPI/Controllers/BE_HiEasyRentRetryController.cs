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
using System.Data;
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
            string CNTRNO = "";
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
                    Mode = Convert.ToInt16(apiInput.Type),
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
                                ORDNO = string.Format("H{0}", obj.OrderNo.ToString().PadLeft(8, '0')),
                                OUTBRNH = obj.OUTBRNH,
                                PAYAMT = obj.PAYAMT.ToString(),
                                PROCD = obj.PROCD,
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

                            bool saveFlag = DoSave060Data(tmpOrder, ORDNO, Convert.ToInt16((output.Result) ? 1 : 0), LogID, ref lstError, ref errCode);
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
                    }
                    else if (retryMode == 2) //125
                    {
                        BE_LandControl obj = rentRepository.GetLandControl(tmpOrder);
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
                                IRENTORDNO = string.Format("H{0}", obj.IRENTORDNO.ToString().PadLeft(8, '0')),
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
                                else
                                {
                                    CNTRNO = output.Data[0].CNTRNO;     //20201127 ADD BY ADAM REASON.增加短租合約回存
                                }

                                //20201127 ADD BY ADAM REASON.增加短租合約回存
                                bool saveFlag = DoSave125Data(tmpOrder, CNTRNO, Convert.ToInt16((output.Result) ? 1 : 0), LogID, ref lstError, ref errCode);
                            }
                        }
                    }
                    else if (retryMode == 3) //130
                    {
                        //BE_ReturnControl obj = rentRepository.GetReturnControl(tmpOrder);

                        string spName2 = new ObjType().GetSPName(ObjType.SPType.BE_GetReturnCarControl);
                        SPInput_BE_GetReturnCarControl spInput2 = new SPInput_BE_GetReturnCarControl()
                        {
                            OrderNo = tmpOrder,
                            UserID = apiInput.UserID,
                            LogID = LogID
                        };
                        SPOutput_Base spOut2 = new SPOutput_Base();
                        List<BE_ReturnControl> ReturnControlList = new List<BE_ReturnControl>();
                        DataSet ds = new DataSet();
                        SQLHelper<SPInput_BE_GetReturnCarControl, SPOutput_Base> sqlHelp2 = new SQLHelper<SPInput_BE_GetReturnCarControl, SPOutput_Base>(connetStr);
                        flag = sqlHelp2.ExeuteSP(spName2, spInput2, ref spOut2, ref ReturnControlList, ref ds, ref lstError);
                        baseVerify.checkSQLResult(ref flag, spOut2.Error, spOut2.ErrorCode, ref lstError, ref errCode);

                        BE_ReturnControl obj = ReturnControlList[0];

                        if (obj != null)
                        {
                            WebAPIInput_NPR130Save input = new WebAPIInput_NPR130Save()
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
                                INVADDR = obj.INVADDR,
                                INVKIND = obj.INVKIND.ToString(),
                                INVTITLE = obj.INVTITLE,
                                IRENTORDNO = string.Format("H{0}", obj.IRENTORDNO.ToString().PadLeft(8, '0')),
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
                                REMARK = obj.REMARK,
                                RENTAMT = obj.RENTAMT.ToString(),
                                RENTDAYS = obj.RENTDAYS.ToString(),
                                RINSU = "0",
                                RNTAMT = obj.RPRICE.ToString(),
                                RNTDATE = obj.RNTDATE,
                                RNTKM = obj.RNTKM.ToString(),       //20210713 ADD BY ADAM REASON.
                                RNTTIME = obj.RNTTIME,
                                RPRICE = obj.RPRICE.ToString(),
                                TSEQNO = obj.TSEQNO,
                                UNIMNO = obj.UNIMNO,
                                AUTHCODE = obj.AUTHCODE,
                                CARDNO = obj.CARDNO,
                                GIFT = obj.GIFT,
                                GIFT_MOTO = obj.GIFT_MOTO,
                                PAYAMT = obj.PAYAMT.ToString(),
                                INBRNHCD = obj.OUTBRNHCD,
                                PARKINGAMT2 = obj.PARKINGAMT2.ToString()   //20210818 ADD BY ADAM REASON.補上停車費
                            };

                            if (obj.PAYAMT > 0)
                            {
                                input.tbPaymentDetail = new PaymentDetail[obj.eTag > 0 ? ReturnControlList.Count+1: ReturnControlList.Count];

                                for (int z = 0; z < ReturnControlList.Count; z++)
                                {
                                    //最後一筆處理ETAG
                                    if (obj.eTag > 0 && z == ReturnControlList.Count -1)
                                    {
                                        //input.tbPaymentDetail = new PaymentDetail[2];
                                        input.tbPaymentDetail[0] = new PaymentDetail()
                                        {
                                            //PAYAMT = obj.PAYAMT.ToString(),     //20210112 ADD BY ADAM REASON.在view那邊就已經有減掉etag，故排除
                                            PAYAMT = (ReturnControlList[z].CloseAmout- obj.eTag).ToString(),     //20210112 ADD BY ADAM REASON.在view那邊就已經有減掉etag，故排除
                                            PAYTYPE = "1",
                                            PAYMENTTYPE = "1",
                                            PAYMEMO = "租金",
                                            //PORDNO = obj.REMARK
                                            PORDNO = ReturnControlList[z].REMARK
                                        };
                                        //input.tbPaymentDetail[1] = new PaymentDetail()
                                        input.tbPaymentDetail[z+1] = new PaymentDetail()
                                        {
                                            PAYAMT = (obj.eTag).ToString(),
                                            PAYTYPE = "2",
                                            PAYMENTTYPE = "1",
                                            PAYMEMO = "eTag",
                                            //PORDNO = obj.REMARK
                                            PORDNO = ReturnControlList[z].REMARK
                                        };
                                    }
                                    else
                                    {
                                        //input.tbPaymentDetail = new PaymentDetail[1];
                                        //input.tbPaymentDetail[0] = new PaymentDetail()
                                        input.tbPaymentDetail[z] = new PaymentDetail()
                                        {
                                            //PAYAMT = obj.PAYAMT.ToString(),     //20210112 ADD BY ADAM REASON.在view那邊就已經有減掉etag，故排除
                                            PAYAMT = ReturnControlList[z].CloseAmout.ToString(),     //20210112 ADD BY ADAM REASON.在view那邊就已經有減掉etag，故排除
                                            PAYTYPE = "1",
                                            PAYMENTTYPE = "1",
                                            PAYMEMO = "租金",
                                            PORDNO = ReturnControlList[z].REMARK
                                        };
                                    }
                                }
                            }
                            else
                            {
                                //至少塞一筆空的
                                input.tbPaymentDetail = new PaymentDetail[1];
                                input.tbPaymentDetail[0] = new PaymentDetail()
                                {
                                    PAYAMT = "0",
                                    PAYTYPE = "",
                                    PAYMENTTYPE = "",
                                    PAYMEMO = "",
                                    PORDNO = ""
                                };
                            }
                            WebAPIOutput_NPR130Save output = new WebAPIOutput_NPR130Save();
                            flag = WebAPI.NPR130Save(input, ref output);
                            if (flag)
                            {
                                int INVAMT = 0, HasPaid = 0;
                                string INVNO = "", INVDATE = "";
                                if (output.Result == false)
                                {
                                    flag = false;
                                    errCode = "ERR";
                                    errMsg = output.Message;
                                }
                                else
                                {
                                    if (obj.PAYAMT > 0)
                                    {
                                        HasPaid = 1;
                                        INVAMT = output.Data[0].INVAMT;
                                        INVNO = output.Data[0].INVNO;
                                        INVDATE = output.Data[0].INVDATE;
                                    }
                                }

                                bool saveFlag = DoSave130Data(tmpOrder, Convert.ToInt16((output.Result) ? 1 : 0), INVNO, INVDATE, INVAMT, HasPaid, LogID, ref lstError, ref errCode);
                            }
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
        private bool DoSave060Data(Int64 OrderNo, string ORDNO, Int16 IsSuccess, Int64 LogID, ref List<ErrorInfo> lstError, ref string errCode)
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
        private bool DoSave125Data(Int64 OrderNo, string CNTRNO, Int16 IsSuccess, Int64 LogID, ref List<ErrorInfo> lstError, ref string errCode)
        {
            bool flag = true;
            string spName = new ObjType().GetSPName(ObjType.SPType.BE_LandControlSuccess);
            SPInput_BE_LandControlSuccess spInput = new SPInput_BE_LandControlSuccess()
            {
                IsSuccess = IsSuccess,
                CNTRNO = CNTRNO,        //20201127 ADD BY ADAM REASON.增加短租合約回存
                LogID = LogID,
                OrderNo = OrderNo
            };
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_BE_LandControlSuccess, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_LandControlSuccess, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;

        }
        private bool DoSave130Data(Int64 OrderNo, Int16 IsSuccess, string INVNO, string INVDATE, int INVAMT, int HasPaid, Int64 LogID, ref List<ErrorInfo> lstError, ref string errCode)
        {
            bool flag = true;
            string spName = new ObjType().GetSPName(ObjType.SPType.BE_ReturnControlSuccess);
            SPInput_BE_ReturnControlSuccess spInput = new SPInput_BE_ReturnControlSuccess()
            {
                IsSuccess = IsSuccess,
                LogID = LogID,
                OrderNo = OrderNo,
                INVAMT = INVAMT,
                INVDATE = INVDATE,
                HasPaid = HasPaid,
                INVNO = INVNO
            };
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_BE_ReturnControlSuccess, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_ReturnControlSuccess, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;

        }

    }
}