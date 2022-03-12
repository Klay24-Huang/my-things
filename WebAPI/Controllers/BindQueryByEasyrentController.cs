﻿using Domain.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    public class BindQueryByEasyrentController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoBindQueryByEasyrent(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BindQueryByEasyrentController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BindListQueryByEasyrent apiInput = null;
            OAPI_GetCardBindListByEasyrent apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion

            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, true);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }

            #endregion

            #region 參數解碼
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_BindListQueryByEasyrent>(Contentjson);

                string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
                string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();
                string ReqParam = AESEncrypt.DecryptAES128(apiInput.P, KEY, IV);
                IDNO = ReqParam;
            }
            #endregion

            #region 送台新查詢
            if (flag)
            {
                TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                {
                    ApiVer = ApiVer,
                    ApposId = TaishinAPPOS,
                    RequestParams = new GetCreditCardListRequestParamasData()
                    {
                        MemberId = IDNO,
                    },
                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                };

                //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);
                if (ds.Tables.Count > 0)
                {
                    apiOutput = new OAPI_GetCardBindListByEasyrent()
                    {
                        HasBind = (ds.Tables[0].Rows.Count == 0) ? 0 : 1,
                        BindListObj = new List<Models.Param.Output.PartOfParam.CreditCardBindListByEasyrent>()
                    };
                    try
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            Models.Param.Output.PartOfParam.CreditCardBindListByEasyrent obj = new Models.Param.Output.PartOfParam.CreditCardBindListByEasyrent()
                            {
                                //AvailableAmount = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["AvailableAmount"].ToString()),
                                BankNo = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["BankNo"].ToString()),
                                CardName = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardName"].ToString()),
                                CardNumber = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardNumber"].ToString()),
                                CardToken = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardToken"].ToString()),
                                CardHash = "",
                                ExpDate = "",
                                CardType = "",
                                IDX = i + 1
                            };
                            apiOutput.BindListObj.Add(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        apiOutput.HasBind = 0;
                    }
                }
                ds.Dispose();

                //WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                //flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                //if (flag)
                //{
                //    int Len = wsOutput.ResponseParams.ResultData.Count;
                //    apiOutput = new OAPI_GetCardBindList()
                //    {
                //        HasBind = (Len == 0) ? 0 : 1,
                //        BindListObj = new List<Models.Param.Output.PartOfParam.CreditCardBindList>()
                //    };
                //    for (int i = 0; i < Len; i++)
                //    {
                //        Models.Param.Output.PartOfParam.CreditCardBindList obj = new Models.Param.Output.PartOfParam.CreditCardBindList()
                //        {
                //            AvailableAmount = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].AvailableAmount),
                //            BankNo = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].BankNo),
                //            CardName = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardName),
                //            CardNumber = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardNumber),

                //            CardToken = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardToken)

                //        };
                //        apiOutput.BindListObj.Add(obj);
                //    }
                //}
                //else
                //{
                //    errCode = wsOutput.RtnCode;
                //    errMsg = wsOutput.RtnMessage;

                //}

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