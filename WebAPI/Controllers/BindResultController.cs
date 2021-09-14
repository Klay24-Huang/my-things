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
using Newtonsoft.Json;
using NLog;
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
using Prometheus; //20210707唐加prometheus
using StackExchange.Redis;//20210913唐加redis

namespace WebAPI.Controllers
{
    /// <summary>
    /// 非同步接收台新回傳結果
    /// </summary>
    public class BindResultController : ApiController
    {
        //唐加prometheus
        private static readonly Gauge ProcessedJobCount1 = Metrics.CreateGauge("BindResult_CallTimes", "Num_BindResult_CallTimes");
        private static readonly Gauge ProcessedJobCount2 = Metrics.CreateGauge("BindResult_CallTaishin", "Num_BindResult_CallTaishin");
        private static readonly Gauge ProcessedJobCount3 = Metrics.CreateGauge("BindResult_DoGetCreditCardList", "Num_BindResult_DoGetCreditCardList");
        private static readonly Gauge ProcessedJobCount4 = Metrics.CreateGauge("BindResult_DoDeleteCreditCardAuth", "Num_BindResult_DoDeleteCreditCardAuth");
        private static readonly Gauge ProcessedJobCount5 = Metrics.CreateGauge("BindResult_Error", "Num_BindResult_Error");
        private static readonly Gauge ProcessedJobCount6 = Metrics.CreateGauge("BindResult_OrderNoNull", "Num_BindResult_OrderNoNull");
        private static readonly Gauge ProcessedJobCount7 = Metrics.CreateGauge("BindResult_hasFind", "Num_BindResult_hasFind");

        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        //20210913唐加redis，解決azure多執行個體造成prometheus的數值亂跳問題
        private string RedisConnet = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public BindResultController()
        {
            if (lazyConnection == null)
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(RedisConnet));
            }
        }

        public Dictionary<string, object> DoBindResult(Dictionary<string, object> value)
        {
            //ProcessedJobCount1.Inc();//唐加prometheus
            SetCount("Num_BindResult_CallTimes");

            #region 初始宣告
            logger.Trace("Init:" + JsonConvert.SerializeObject(value));
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
            try
            {

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
                        if (OrderNo == null)
                        {
                            flag = false;
                            errCode = "ERR197";
                            //ProcessedJobCount6.Inc();
                            SetCount("Num_BindResult_OrderNoNull");
                        }
                    }
                    logger.Trace("Call:" + JsonConvert.SerializeObject(apiInput) + ",Error:" + errCode);
                }

                #region 送台新查詢
                bool hasFind = false;
                object[] objparms = new object[1];
                try
                {
                    if (apiInput.RequestParams.CardToken != null && apiInput.RequestParams.CardStatus != null)
                    {
                        if (apiInput.RequestParams.CardToken != "" && apiInput.RequestParams.CardStatus == "1")
                        {
                            objparms[0] = new
                            {
                                BankNo = apiInput.RequestParams.BankNo,
                                CardNumber = apiInput.RequestParams.CardNumber,
                                CardName = apiInput.RequestParams.CardName,
                                AvailableAmount = "",
                                CardToken = apiInput.RequestParams.CardToken
                            };
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ProcessedJobCount2.Inc();//唐加prometheus
                    SetCount("Num_BindResult_CallTaishin");
                    logger.Trace("setRequestParams Error:" + ex.Message);
                }
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
                    int Len = 0;
                    WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                    try
                    {
                        logger.Trace("GetCreditCardList_Start:" + JsonConvert.SerializeObject(wsInput));
                        flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                        Len = wsOutput.ResponseParams.ResultData.Count;
                        logger.Trace("GetCreditCardList_End_Success:" + JsonConvert.SerializeObject(wsOutput));
                    }
                    catch (Exception ex)
                    {
                        //ProcessedJobCount3.Inc();//唐加prometheus
                        SetCount("Num_BindResult_DoGetCreditCardList");
                        flag = false;
                        logger.Trace("GetCreditCardList_End:" + JsonConvert.SerializeObject(wsOutput) + ",Error:" + ex.Message);
                    }
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
      
                    if (hasFind)//有找到，可以做刪除
                    {
                        Thread.Sleep(1000);
                        try
                        {

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
                        catch (Exception ex)
                        {
                            //ProcessedJobCount4.Inc();//唐加prometheus
                            SetCount("Num_BindResult_DoDeleteCreditCardAuth");
                            logger.Trace("DoDeleteCreditCardAuth:" + ",Error:" + ex.Message);
                            flag = false;
                            errCode = "ERR195";
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR195";
                        //ProcessedJobCount7.Inc();//唐加prometheus
                        SetCount("Num_BindResult_hasFind");
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
                        logger.Trace("SaveBindCard:" + ",Error:" + returnMessage);
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
            }
            catch (Exception ex)
            {
                //ProcessedJobCount5.Inc();//唐加prometheus
                SetCount("Num_BindResult_Error");
                logger.Trace("OUTTER_ERROR:" + ",Error:" + ex.Message);
            }
            #region 輸出
            Dictionary<string, object> output = new Dictionary<string, object>();
            output.Add("OrderNo", apiInput.RequestParams.OrderNo);
            output.Add("IsSuccess", true);
            output.Add("TimeStamp", apiInput.TimeStamp);
            output.Add("Random", apiInput.Random);
            output.Add("CheckSum", apiInput.CheckSum);

            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            logger.Trace("Output:" + JsonConvert.SerializeObject(output));
            return output;
            #endregion
        }

        private void SetCount(string memo)
        {
            var value = 1;
            try
            {
                ConnectionMultiplexer connection = lazyConnection.Value;
                IDatabase cache = connection.GetDatabase();

                var key = memo;
                var cacheString = cache.StringGet(key);
                if (cacheString.HasValue)
                {
                    int.TryParse(cacheString.ToString(), out value);
                    value++;
                }
                cache.StringSet(key, value);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            switch (memo)
            {
                case "Num_BindResult_CallTimes":
                    ProcessedJobCount1.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_CallTaishin":
                    ProcessedJobCount2.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_DoGetCreditCardList":
                    ProcessedJobCount3.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_DoDeleteCreditCardAuth":
                    ProcessedJobCount4.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_Error":
                    ProcessedJobCount5.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_OrderNoNull":
                    ProcessedJobCount6.Set(value); //宣告Guage才能用set
                    break;
                case "Num_BindResult_hasFind":
                    ProcessedJobCount7.Set(value); //宣告Guage才能用set
                    break;
            }
        }
    }
}
