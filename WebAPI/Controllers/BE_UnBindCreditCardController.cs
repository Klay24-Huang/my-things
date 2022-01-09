using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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
    /// 【後台】信用卡解除綁卡
    /// </summary>
    public class BE_UnBindCreditCardController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        //解綁鎖定在舊商代
        private string oldTaishinAPPOS = ConfigurationManager.AppSettings["oldTaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string BindSuccessURL = ConfigurationManager.AppSettings["BindSuccessURL"].ToString();
        private string BindFailURL = ConfigurationManager.AppSettings["BindFailURL"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoBE_UnBindCreditCard(Dictionary<string, object> value)
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
            string funName = "BE_UnBindCreditCardController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_UnBindCreditCard apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";       //身分證號
            string CardToken = "";  //信用卡密鑰
            bool isGuest = true;
            string Contentjson = "";
            bool hasFind = false;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_UnBindCreditCard>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.IDNO };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                }

                if (flag)
                {
                    IDNO = apiInput.IDNO;
                }
            }
            #endregion
            #region 送台新查詢
            if (flag)
            {
                TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                {
                    ApiVer = ApiVerOther,
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
                        hasFind = true;
                    }
                    if (hasFind)    //有找到，可以做刪除
                    {
                        CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;

                        Thread.Sleep(1000);
                        PartOfDeleteCreditCardAuth WSDeleteInput = new PartOfDeleteCreditCardAuth()
                        {
                            ApiVer = ApiVerOther,
                            //ApposId = TaishinAPPOS,
                            ApposId = oldTaishinAPPOS,  //解綁鎖定在舊商代
                            RequestParams = new DeleteCreditCardAuthRequestParamasData()
                            {
                                MemberId = IDNO,
                                CardToken = wsOutput.ResponseParams.ResultData[0].CardToken
                            },
                            Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                            TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))
                        };

                        WebAPIOutput_DeleteCreditCardAuth WSDeleteOutput = new WebAPIOutput_DeleteCreditCardAuth();
                        flag = WebAPI.DoDeleteCreditCardAuth(WSDeleteInput, ref errCode, ref WSDeleteOutput);
                        if (WSDeleteOutput.ResponseParams.ResultData.IsSuccess == false)
                        {
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

            #region 異動DB
            if (flag && hasFind)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_UnBindCreditCard);
                SPInput_BE_UnBindCard spInput = new SPInput_BE_UnBindCard()
                {
                    IDNO = IDNO,
                    CardToken = CardToken,
                    LogID = LogID,
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_UnBindCard, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_UnBindCard, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

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