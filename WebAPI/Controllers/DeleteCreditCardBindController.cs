using Domain.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.ResultData;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
namespace WebAPI.Controllers
{
    public class DeleteCreditCardBindController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoDeleteCreditCardBind(Dictionary<string, object> value)
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
            string funName = "DeleteCreditCardBindController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_DeleteCreditCardAuth apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            bool hasFind = false;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);


            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_DeleteCreditCardAuth>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    string[] checkList = { apiInput.CardToken };
                    string[] errList = { "ERR900" };
                    //1.判斷必填
                    flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                }
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            else
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion
            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);

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
                WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                if (flag)
                {
                    int Len = wsOutput.ResponseParams.ResultData.Count;
                    if (Len > 0)
                    {
                        int index = wsOutput.ResponseParams.ResultData.FindIndex(delegate (GetCreditCardResultData obj)
                        {
                            return obj.CardToken == apiInput.CardToken;
                        });
                        if (index > -1)
                        {
                            hasFind = true;
                        }
                    }
                    if (hasFind)//有找到，可以做刪除
                    {
                        Thread.Sleep(1000);
                        PartOfDeleteCreditCardAuth WSDeleteInput = new PartOfDeleteCreditCardAuth()
                        {
                            ApiVer = "1.0.0",
                            ApposId = TaishinAPPOS,
                            RequestParams = new DeleteCreditCardAuthRequestParamasData()
                            {
                                MemberId = IDNO,
                                CardToken = apiInput.CardToken
                            },
                            Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                            TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))
                        };

                        WebAPIOutput_DeleteCreditCardAuth WSDeleteOutput = new WebAPIOutput_DeleteCreditCardAuth();
                        flag = WebAPI.DoDeleteCreditCardAuth(WSDeleteInput, ref errCode, ref WSDeleteOutput);
                        if (WSDeleteOutput.ResponseParams.ResultData.IsSuccess==false) {
                            flag = false;
                            errCode = "ERR196";
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR195";
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
