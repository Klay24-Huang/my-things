using Domain.Common;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class SetMonthlyRentDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string key = ConfigurationManager.AppSettings["apikey"].ToString();
        private string salt = ConfigurationManager.AppSettings["salt"].ToString();

        /// <summary>
        /// 儲存月租訂閱制資料
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> SetMonthlyRentData(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SetMonthlyRentDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MonthlyRent apiInput = null;
            NullOutput outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            DateTime Birth = DateTime.Now;
            string Contentjson = "";
            string FileName = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_MonthlyRent>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = {                    
                    apiInput.CUSTID,
                    apiInput.CUSTNM,
                    apiInput.EMAIL,
                    apiInput.MonProjID,
                    apiInput.MonProPeriod.ToString(),
                    apiInput.ShortDays.ToString(),
                    apiInput.SDATE,
                    apiInput.EDATE,
                    apiInput.IsMoto.ToString(),
                    apiInput.RCVAMT.ToString(),
                    apiInput.UNIMNO,
                    apiInput.CARDNO,
                    apiInput.AUTHCODE,
                    apiInput.NORDNO,
                    apiInput.INVKIND,
                    apiInput.CARRIERID,
                    apiInput.NPOBAN,
                    apiInput.INVTITLE,
                    apiInput.INVADDR,
                    apiInput.tbPaymentDetail.ToString() };
                
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion

            #region TB
            if (flag)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();

                WebAPIInput_MonthlyRentSave spInput = new WebAPIInput_MonthlyRentSave()
                {
                    CUSTID = apiInput.CUSTID,
                    CUSTNM = apiInput.CUSTNM,
                    EMAIL = apiInput.EMAIL,
                    MonProjID = apiInput.MonProjID,
                    MonProPeriod = apiInput.MonProPeriod,
                    ShortDays = apiInput.ShortDays,
                    SDATE = apiInput.SDATE,
                    EDATE = apiInput.EDATE,
                    IsMoto = apiInput.IsMoto,
                    RCVAMT = apiInput.RCVAMT,
                    UNIMNO = apiInput.UNIMNO,
                    CARDNO = apiInput.CARDNO,
                    AUTHCODE = apiInput.AUTHCODE,
                    NORDNO = apiInput.NORDNO,
                    INVKIND = apiInput.INVKIND,
                    CARRIERID = apiInput.CARRIERID,
                    NPOBAN = apiInput.NPOBAN,
                    INVTITLE = apiInput.INVTITLE,
                    INVADDR = apiInput.INVADDR,
                    tbPaymentDetail = apiInput.tbPaymentDetail

                };
                flag = hiEasyRentAPI.MonthlyRentSave(spInput, ref wsOutput);
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
    }