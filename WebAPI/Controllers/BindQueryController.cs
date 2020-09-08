using Domain.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using OtherService;
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
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取回綁定(信用卡、銀行帳號)列表
    /// </summary>
    
    public class BindQueryController : ApiController
    {
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoGetBindList()
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BindQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CheckAccount apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            string IDNO = "C121119150";
            #endregion
            TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
            PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
            {
                ApiVer = "1.0.0",
                ApposId = TaishinAPPOS,
                RequestParams = new GetCreditCardListRequestParamasData()
                {
                    MemberId = IDNO,
                },
                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                 TransNo=string.Format("{0}_{1}", IDNO,DateTime.Now.ToString("yyyyMMddhhmmss"))

            };
            WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
            flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion
        }
    }
}
