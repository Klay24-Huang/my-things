using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Wallet;
using Domain.SP.Input.Bill;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Wallet;
using Domain.SP.Output.Bill;
using Domain.TB;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using Domain.SP.Input.Car;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 使用信用卡付款
    /// </summary>
    public class CreditAuthController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string BindSuccessURL = ConfigurationManager.AppSettings["BindSuccessURL"].ToString();
        private string BindFailURL = ConfigurationManager.AppSettings["BindFailURL"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        private CommonFunc baseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoCreditAuth(Dictionary<string, object> value)
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
            string funName = "CreditAuthController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CreditAuth apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;
            bool HasETAG = false;
            List<OrderQueryFullData> OrderDataLists = null;
            int RewardPoint = 0;    //20201201 ADD BY ADAM REASON.換電獎勵
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CreditAuth>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (apiInput.PayType < 0 || apiInput.PayType > 1)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    if (apiInput.PayType == 0)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        else
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
                #region 這邊要再加上查訂單狀態
                SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    OrderNo = tmpOrder,
                    Token = Access_Token,
                    transaction_no = ""
                };
                SPInput_DonePayBack spInput_PayBack = new SPInput_DonePayBack()
                {
                    NPR330Save_ID = apiInput.NPR330Save_ID == null ? 0 : apiInput.NPR330Save_ID.Value,
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };

                if (apiInput.PayType == 0)
                {
                    #region 取出訂單資訊
                    if (flag)
                    {
                        SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            LogID = LogID,
                            Token = Access_Token
                        };
                        string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
                        SPOutput_Base spOutBase = new SPOutput_Base();
                        SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                        OrderDataLists = new List<OrderQueryFullData>();
                        DataSet ds = new DataSet();
                        flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                        //判斷訂單狀態
                        if (flag)
                        {
                            if (OrderDataLists.Count == 0)
                            {
                                flag = false;
                                errCode = "ERR203";
                            }
                        }

                    }
                    if (flag)
                    {
                        if (OrderDataLists[0].car_mgt_status >= 15)
                        {
                            flag = false;
                            errCode = "ERR209";
                        }
                        else if (OrderDataLists[0].car_mgt_status < 11)
                        {
                            flag = false;
                            errCode = "ERR210";
                        }
                        else
                        {
                            Amount = OrderDataLists[0].final_price;
                        }
                    }
                    #endregion
                    #region 檢查車機狀態
                    if (flag)
                    {
                        flag = new CarCommonFunc().CheckReturnCar(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                    }
                    #endregion
                    #region 檢查iButton
                    //if (flag)
                    //{
                    //    SPInput_CheckCariButton spInput = new SPInput_CheckCariButton()
                    //    {
                    //        OrderNo = tmpOrder,
                    //        Token = Access_Token,
                    //        IDNO = IDNO,
                    //        LogID = LogID
                    //    };
                    //    string SPName = new ObjType().GetSPName(ObjType.SPType.CheckCarIButton);
                    //    SPOutput_Base SPOutputBase = new SPOutput_Base();
                    //    SQLHelper<SPInput_CheckCariButton, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckCariButton, SPOutput_Base>(connetStr);
                    //    flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref SPOutputBase, ref lstError);
                    //    baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                    //}
                    #endregion
                    #region 台新信用卡-Mark
                    //if (flag)
                    //{
                    //    //送台新查詢
                    //    TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                    //    PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                    //    {
                    //        ApiVer = ApiVerOther,
                    //        ApposId = TaishinAPPOS,
                    //        RequestParams = new GetCreditCardListRequestParamasData()
                    //        {
                    //            MemberId = IDNO,
                    //        },
                    //        Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    //        TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    //        TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                    //    };
                    //    WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                    //    flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                    //    if (flag)
                    //    {
                    //        int Len = wsOutput.ResponseParams.ResultData.Count;
                    //        bool hasFind = false;
                    //        string CardToken = "";
                    //        if (Len > 0)
                    //        {
                    //            CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;
                    //            hasFind = true;
                    //        }

                    //        #region 直接授權
                    //        if (hasFind)//有找到，可以做扣款
                    //        {
                    //            SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                    //            {
                    //                IDNO = IDNO,
                    //                LogID = LogID,
                    //                OrderNo = tmpOrder,
                    //                Token = Access_Token,
                    //                transaction_no = ""
                    //            };

                    //            Thread.Sleep(1000);
                    //            if (Amount > 0)
                    //            {
                    //                Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                    //                {
                    //                    Amount = Amount.ToString() + "00",
                    //                    Name = string.Format("{0}租金", apiInput.OrderNo),
                    //                    NonPoint = "N",
                    //                    NonRedeem = "N",
                    //                    Price = Amount.ToString() + "00",
                    //                    Quantity = "1"
                    //                };
                    //                PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                    //                {
                    //                    ApiVer = "1.0.2",
                    //                    ApposId = TaishinAPPOS,
                    //                    RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                    //                    {
                    //                        CardToken = CardToken,
                    //                        InstallPeriod = "0",
                    //                        InvoiceMark = "N",
                    //                        Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                    //                        MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                    //                        MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                    //                        MerchantTradeNo = string.Format("{0}F{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                    //                        NonRedeemAmt = Amount.ToString() + "00",
                    //                        NonRedeemdescCode = "",
                    //                        Remark1 = "",
                    //                        Remark2 = "",
                    //                        Remark3 = "",
                    //                        ResultUrl = BindResultURL,
                    //                        TradeAmount = Amount.ToString() + "00",
                    //                        TradeType = "1",
                    //                        UseRedeem = "N"

                    //                    },
                    //                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    //                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                    //                };
                    //                WSAuthInput.RequestParams.Item.Add(item);

                    //                WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                    //                flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                    //                if (WSAuthOutput.RtnCode != "1000" && WSAuthOutput.ResponseParams.ResultCode != "0000")
                    //                {
                    //                    flag = false;
                    //                    errCode = "ERR197";
                    //                }
                    //                if (flag)
                    //                {
                    //                    PayInput.transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                PayInput.transaction_no = "Free";
                    //            }
                    //            if (flag)
                    //            {
                    //                //if (OrderDataLists[0].ProjType == 4)
                    //                //{
                    //                //    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                    //                //    if (Motorflag == false)
                    //                //    {
                    //                //        //寫入車機錯誤
                    //                //    }
                    //                //}
                    //                bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                    //                if (CarFlag == false)
                    //                {
                    //                    //寫入車機錯誤
                    //                }
                    //                string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                    //                SPOutput_Base PayOutput = new SPOutput_Base();
                    //                SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                    //                flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                    //                baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                    //            }

                    //        }
                    //        else
                    //        {
                    //            flag = false;
                    //            errCode = "ERR195";
                    //        }
                    //        #endregion

                    //    }
                    //    else
                    //    {
                    //        errCode = "ERR730";
                    //    }
                    //}
                    #endregion

                    if (flag)
                    {
                        flag = TaishinCardTrade(apiInput, ref PayInput, ref Amount, ref errCode);
                        if (flag)
                        {
                            string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);

                            //20201201 ADD BY ADAM REASON.換電獎勵
                            //SPOutput_Base PayOutput = new SPOutput_Base();
                            //SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                            SPOutput_GetRewardPoint PayOutput = new SPOutput_GetRewardPoint();
                            SQLHelper<SPInput_DonePayRent, SPOutput_GetRewardPoint> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_GetRewardPoint>(connetStr);
                            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                            //baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                            baseVerify.checkSQLResult(ref flag, PayOutput.Error, PayOutput.ErrorCode, ref lstError, ref errCode);
                            if (flag)
                            {
                                RewardPoint = PayOutput.Reward;
                            }
                        }
                    }

                    //20201201 ADD BY ADAM REASON.換電獎勵
                    if (flag && OrderDataLists[0].ProjType == 4 && RewardPoint > 0)
                    {
                        WebAPIOutput_NPR380Save wsOutput = new WebAPIOutput_NPR380Save();
                        HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                        flag = wsAPI.NPR380Save(IDNO, RewardPoint.ToString(), apiInput.OrderNo, ref wsOutput);
                        //存檔
                        string SPName = new ObjType().GetSPName(ObjType.SPType.SaveNPR380Result);
                        SPOutput_Base NPR380Output = new SPOutput_Base();
                        SPInput_SetRewardResult NPR380Input = new SPInput_SetRewardResult()
                        {
                            OrderNo = tmpOrder,
                            Result = flag == true ? 1 : 0,
                            LogID = LogID
                        };
                        SQLHelper<SPInput_SetRewardResult, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_SetRewardResult, SPOutput_Base>(connetStr);
                        flag = SQLPayHelp.ExecuteSPNonQuery(SPName, NPR380Input, ref NPR380Output, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref NPR380Output, ref lstError, ref errCode);
                    }

                    #region 寫還車照片到azure
                    if (flag)
                    {

                        OtherRepository otherRepository = new OtherRepository(connetStr);
                        List<CarPIC> lstCarPIC = otherRepository.GetCarPIC(tmpOrder, 1);
                        int PICLen = lstCarPIC.Count;
                        for (int i = 0; i < PICLen; i++)
                        {
                            try
                            {
                                string FileName = string.Format("{0}_{1}_{2}.png", apiInput.OrderNo, (lstCarPIC[i].ImageType == 5) ? "Sign" : "PIC" + lstCarPIC[i].ImageType.ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"));

                                flag = new AzureStorageHandle().UploadFileToAzureStorage(lstCarPIC[i].Image, FileName, "carpic");
                                if (flag)
                                {
                                    bool DelFlag = otherRepository.HandleTempCarPIC(tmpOrder, 1, lstCarPIC[i].ImageType, FileName); //更新為azure的檔名
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = true; //先bypass，之後補傳再刪
                            }
                        }

                    }
                    #endregion
                }
                else if (apiInput.PayType == 1)
                {
                    if (flag)
                    {
                        if (apiInput.NPR330Save_ID != null && apiInput.NPR330Save_ID > 0)
                        {
                            flag = TaishinCardTrade(apiInput, ref PayInput, ref Amount, ref errCode);
                            if (flag)
                                flag = DonePayBack(spInput_PayBack, ref errCode, ref lstError);//欠款繳交
                        }
                        else
                        {
                            errCode = "ERR111";
                            flag = false;
                        }
                    }
                }
                #endregion
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

        /// <summary>
        /// 台新信用卡交易
        /// </summary>
        private bool TaishinCardTrade(
            IAPI_CreditAuth apiInput,
            ref SPInput_DonePayRent PayInput,
            ref int Amount, ref string errCode)
        {
            bool flag = true;

            string IDNO = PayInput.IDNO;
            Int64 LogID = PayInput.LogID;
            Int64 tmpOrder = PayInput.OrderNo;
            string Access_Token = PayInput.Token;

            #region 台新信用卡
            if (flag)
            {
                //送台新查詢
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
                    bool hasFind = false;
                    string CardToken = "";
                    if (Len > 0)
                    {
                        CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;
                        hasFind = true;
                    }

                    #region 直接授權
                    if (hasFind)//有找到，可以做扣款
                    {
                        //SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                        //{
                        //    IDNO = IDNO,
                        //    LogID = LogID,
                        //    OrderNo = tmpOrder,
                        //    Token = Access_Token,
                        //    transaction_no = ""
                        //};

                        Thread.Sleep(1000);
                        if (Amount > 0)
                        {
                            Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                            {
                                Amount = Amount.ToString() + "00",
                                Name = string.Format("{0}租金", apiInput.OrderNo),
                                NonPoint = "N",
                                NonRedeem = "N",
                                Price = Amount.ToString() + "00",
                                Quantity = "1"
                            };
                            PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                            {
                                ApiVer = "1.0.2",
                                ApposId = TaishinAPPOS,
                                RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                                {
                                    CardToken = CardToken,
                                    InstallPeriod = "0",
                                    InvoiceMark = "N",
                                    Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                                    MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                                    MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                                    //MerchantTradeNo = string.Format("{0}F{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    MerchantTradeNo = string.Format("{0}F_{1}", tmpOrder, DateTime.Now.ToString("yyMMddHHmm")),      //20201130 ADD BY ADAM 因應短租財務長度20進行調整
                                    NonRedeemAmt = Amount.ToString() + "00",
                                    NonRedeemdescCode = "",
                                    Remark1 = "",
                                    Remark2 = "",
                                    Remark3 = "",
                                    ResultUrl = BindResultURL,
                                    TradeAmount = Amount.ToString() + "00",
                                    TradeType = "1",
                                    UseRedeem = "N"

                                },
                                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                            };
                            WSAuthInput.RequestParams.Item.Add(item);

                            WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                            //flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                            flag = WebAPI.DoCreditCardAuthV2(WSAuthInput, IDNO, ref errCode, ref WSAuthOutput);
                            if (WSAuthOutput.RtnCode != "1000" && WSAuthOutput.ResponseParams.ResultCode != "0000")
                            {
                                flag = false;
                                errCode = "ERR197";
                            }
                            if (flag)
                            {
                                PayInput.transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                            }
                        }
                        else
                        {
                            PayInput.transaction_no = "Free";
                        }
                        if (flag)
                        {
                            //if (OrderDataLists[0].ProjType == 4)
                            //{
                            //    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                            //    if (Motorflag == false)
                            //    {
                            //        //寫入車機錯誤
                            //    }
                            //}

                            if (apiInput.PayType == 0)
                            {
                                bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                                if (CarFlag == false)
                                {
                                    //寫入車機錯誤
                                }
                            }

                            //string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                            //SPOutput_Base PayOutput = new SPOutput_Base();
                            //SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                            //flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                            //baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR195";
                    }
                    #endregion
                }
                else
                {
                    flag = false;
                    errCode = "ERR730";
                }
            }
            #endregion

            return flag;
        }

        /// <summary>
        /// 欠費繳交
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        private bool DonePayBack(SPInput_DonePayBack spInput, ref string errCode, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayBack);
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_DonePayBack, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayBack, SPOutput_Base>(connetStr);
            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutput, ref lstError, ref errCode);
            return flag;
        }
    }
}