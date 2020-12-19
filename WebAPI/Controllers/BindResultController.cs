using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.TB;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.ResultData;
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
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 非同步接收台新回傳結果
    /// </summary>
    public class BindResultController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        public Dictionary<string, object> DoBindResult(Dictionary<string, object> value)
        {
            #region 初始宣告

            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BindResultController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            BindOriRequestParams apiInput = null;
            OAPI_Base apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string Access_Token_string = "", Access_Token = "";
            #endregion
            #region 防呆

           flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BindResult>(Contentjson);
            apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<BindOriRequestParams>(Contentjson);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);


            }

            string IDNO = apiInput.RequestParams.MemberId;
            string OrderNo = apiInput.RequestParams.OrderNo;
            string CardToken = "";
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.GetUnBindLog);
                SPInput_GetUnBindLog spUnBindLogInput = new SPInput_GetUnBindLog()
                {
                    IDNO = IDNO,
                    OrderNo = OrderNo,
                    LogID = LogID
                };
                SPOutput_Base SPOutputBase = new SPOutput_Base();
                SQLHelper<SPInput_GetUnBindLog, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetUnBindLog, SPOutput_Base>(connetStr);
                List<UnBindLog> lstOut = new List<UnBindLog>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spUnBindLogInput, ref SPOutputBase, ref lstOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                if (lstOut.Count > 0)
                {
                    CardToken = lstOut[0].CardToken;
                }
                else
                {
                    flag = false;
                    errCode = "ERR197";
                }
            }

            #region 送台新查詢
            bool hasFind = false;
            object[] objparms = new object[1];
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
                int Len = wsOutput.ResponseParams.ResultData.Count;
                if (Len > 0)
                {
                    int index = wsOutput.ResponseParams.ResultData.FindIndex(delegate (GetCreditCardResultData obj)
                    {
                        return obj.CardToken == CardToken;
                    });
                    if (index > -1)
                    {
                        hasFind = true;
                    }

                    objparms = new object[Len == 0 ? 1 : Len];
                    if (Len > 0)
                    {
                        for (int i = 0; i < Len; i++)
                        {
                            objparms[i] = new
                            {
                                BankNo = wsOutput.ResponseParams.ResultData[i].BankNo == null ? "" : wsOutput.ResponseParams.ResultData[i].BankNo,
                                CardNumber = wsOutput.ResponseParams.ResultData[i].CardNumber,
                                CardName = wsOutput.ResponseParams.ResultData[i].CardName,
                                AvailableAmount = wsOutput.ResponseParams.ResultData[i].AvailableAmount == null ? "" : wsOutput.ResponseParams.ResultData[i].AvailableAmount,
                                CardToken = wsOutput.ResponseParams.ResultData[i].CardToken
                            };
                        }
                    }
                    else
                    {
                        objparms[0] = new
                        {
                            BankNo = "",
                            CardNumber = "",
                            CardName = "",
                            AvailableAmount = "",
                            CardToken = ""
                        };
                    }
                }
                else
                {
                    objparms[0] = new
                    {
                        BankNo = "",
                        CardNumber = "",
                        CardName = "",
                        AvailableAmount = "",
                        CardToken = ""
                    };

                }
                if (hasFind)//有找到，可以做刪除
                {
                    Thread.Sleep(1000);
                    PartOfDeleteCreditCardAuth WSDeleteInput = new PartOfDeleteCreditCardAuth()
                    {
                        ApiVer = ApiVerOther,
                        ApposId = TaishinAPPOS,
                        RequestParams = new DeleteCreditCardAuthRequestParamasData()
                        {
                            MemberId = IDNO,
                            CardToken = CardToken
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

                object[][] parms1 = {
                        new object[] {
                            IDNO,
                            CardToken,
                            LogID
                    },
                        objparms
                    };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";
                string SPName = new ObjType().GetSPName(ObjType.SPType.UnBindCard);

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                //logger.Trace(JsonConvert.SerializeObject(ds1));
                if (ds1.Tables.Count == 0)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = returnMessage;
                }
                ds1.Dispose();
            }
            #endregion

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
