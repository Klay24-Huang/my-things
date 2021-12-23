using Domain.Flow.Hotai;
using Domain.SP.Input.Common;
using Domain.SP.Input.Hotai;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.CTBCPOS;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output;
using Domain.WebAPI.output.CTBCPOS;
using Domain.WebAPI.output.Hotai.Member;
using Domain.WebAPI.output.Hotai.Payment;
using Newtonsoft.Json;
using NLog;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WebCommon;

namespace OtherService
{
    public class HotaipayService
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
       // private string CTBCTestCard = ConfigurationManager.AppSettings["CTBCTestCard"]?.ToString()??"";
        private static ConfigManager configManager = new ConfigManager("hotaipayment");
        private string merID = configManager.GetKey("CTBCMerID");
        private string terMinnalID = configManager.GetKey("CTBCTerminalID");
        private string CTBCTestCard = configManager.GetKey("CTBCTestCard");
        HotaiMemberAPI hotaiMemberAPI = new HotaiMemberAPI();
        /// <summary>
        /// 取得和泰卡片清單
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoQueryCardList(IFN_QueryCardList input, ref OFN_HotaiCreditCardList output, ref string errCode)
        {
            logger.Info($"DoQueryCardList | start | INPUT : {JsonConvert.SerializeObject(input)}");
            bool flag = true;
            HotaiPaymentAPI PaymentAPI = new HotaiPaymentAPI();
            output.CreditCards = new List<HotaiCardInfo>();
            if (CTBCTestCard == "1")
            {
                logger.Info($"DoQueryCardList |Get AccessToken | 進入測試模式");

                var creditCards = GetTestCards();

                output.CreditCards = creditCards;

                return true;
            }

            //1.取得會員Token
            HotaiToken hotaiToken = new HotaiToken();
            flag = DoQueryToken(input.IDNO, input.PRGName, ref hotaiToken, ref errCode);
            logger.Info($"DoQueryCardList |Get AccessToken | Result:{ flag } ; errCode:{errCode} | IDNO :{input.IDNO} ; 會員Token : {JsonConvert.SerializeObject(hotaiToken)}");

            if (hotaiToken.IsCancel == 1)
            {
                flag = false;
                errCode = "ERR953";
            }

            //2.向中信取得卡清單
            WebAPIOutput_GetCreditCards cardsOptput = new WebAPIOutput_GetCreditCards();
            if (flag)
            {
                var objGetCard = new WebAPIInput_GetCreditCards
                {
                    AccessToken = hotaiToken.AccessToken
                };
                flag = PaymentAPI.GetHotaiCardList(objGetCard, ref cardsOptput);

                logger.Info($"DoQueryCardList | GetCTBCCards | Result:{ flag } ; errCode:{errCode} | cardOptput : {JsonConvert.SerializeObject(cardsOptput)}");
            }
            //3.資料庫取得預設卡
            var dbDefaultCard = new SPOutput_HotaiGetDefaultCard();
            if (flag)
            {
                dbDefaultCard = sp_GetDefaultCard(input.IDNO, input.LogID, ref flag, ref errCode);
                logger.Info($"DoQueryCardList | GetDefaultCard | Result:{ flag } ; errCode:{errCode} | dbDefaultCard : {JsonConvert.SerializeObject(dbDefaultCard)}");
            }
            //4.比對預設卡與卡清單
            if (flag)
            {
                var creditCards = new List<HotaiCardInfo>();
                if (cardsOptput.CardCount == 1 && dbDefaultCard.HotaiCardID == 0)
                {
                    var originalCard = cardsOptput.HotaiCards.FirstOrDefault();

                    SPInput_SetDefaultCard sp_setCardInput =
                        new SPInput_SetDefaultCard
                        {
                            IDNO = input.IDNO,
                            OneID = originalCard.MemberOneID,
                            CardNo = originalCard.CardNoMask,
                            CardToken = originalCard.Id.ToString(),
                            CardType = originalCard.CardType,
                            BankDesc = originalCard.BankDesc,
                            PRGName = input.PRGName,
                            BankCode = originalCard.BinInfo?.Code+"" ?? "",
                        };

                    //寫入預設卡片
                    flag = sp_SetDefaultCard(sp_setCardInput, ref errCode);
                    logger.Info($"DoQueryCardList | SetDefaultCard | Result:{ flag } ; errCode:{errCode} | sp_setCardInput:{JsonConvert.SerializeObject(sp_setCardInput)}");
                    creditCards.Add(setHotaiCardInfo(originalCard, originalCard.Id.ToString()));
                }
                else
                {
                    cardsOptput.HotaiCards.ForEach(
                    p => creditCards.Add(setHotaiCardInfo(p, dbDefaultCard.CardToken)));

                }
                var hasDefault = creditCards.FindIndex(p => p.IsDefault == 1) == -1 ? false : true;
                if (!hasDefault && dbDefaultCard.HotaiCardID != 0)
                {
                    var unbindFlag = sp_HotaiDefaultCardUnbind(
                            new SPInput_HotaiDefaultCardUnbind
                            {
                                IDNO = input.IDNO,
                                HotaiCardID = dbDefaultCard.HotaiCardID,
                                LogID = input.LogID,
                                U_FuncName = input.PRGName,
                                U_USERID = "Sys"
                            }, ref errCode);
                }

                output.CreditCards = creditCards;
            }
            //5.整理後回傳
            if (flag && output.CreditCards?.Count() == 0)
            {
                flag = false;
                errCode = "ERR991";
            }
            logger.Info($"DoQueryCardList | Final | Result:{ flag } ; errCode:{errCode} | Output:{JsonConvert.SerializeObject(output)}");
            return flag;
        }
        /// <summary>
        /// 取得和泰Pay預設卡片
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="card"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoQueryDefaultCard(IFN_QueryDefaultCard input, ref HotaiCardInfo card, ref string errCode)
        {
            bool flag = true;
            OFN_HotaiCreditCardList hotaiCards = new OFN_HotaiCreditCardList();

            var objGetCards = new IFN_QueryCardList
            {
                IDNO = input.IDNO,
                LogID = input.LogID,
                PRGName = input.PRGName,
                insUser = input.insUser
            };

            flag = DoQueryCardList(objGetCards, ref hotaiCards, ref errCode);
            if (flag)
            {
                card = hotaiCards.CreditCards.Find(p => p.IsDefault == 1);

                flag = (card == null) ? false : true;
            }

            return flag;
        }
        /// <summary>
        /// 取得指定卡片
        /// </summary>
        /// <param name="input"></param>
        /// <param name="card"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoQueryCard(IFN_HotaiQueryCardForOne input, ref HotaiCardInfo card, ref string errCode)
        {
            bool flag = true;
            OFN_HotaiCreditCardList hotaiCards = new OFN_HotaiCreditCardList();

            //flag = int.TryParse(input.CardToken, out var findCardToken);

            var objGetCards = new IFN_QueryCardList
            {
                IDNO = input.IDNO,
                LogID = input.LogID,
                PRGName = input.PRGName,
                insUser = input.insUser
            };
            if (flag)
            {
                flag = DoQueryCardList(objGetCards, ref hotaiCards, ref errCode);
            }
            if (flag)
            {
                card = hotaiCards.CreditCards.Find(p => p.CardToken == input.CardToken);

                flag = (card == null || card == default) ? false : true;
            }

            return flag;
        }

        /// <summary>
        /// 取得和泰Token
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="LogID"></param>
        /// <param name="PRGName"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoQueryToken(string IDNO, string PRGName, ref HotaiToken output, ref string errCode)
        {
            bool flag = true;
            int HttpStatusCode = 0;
            WebAPIOutput_Token outputToken = new WebAPIOutput_Token();
            //查詢Db Token
            SPOutput_QueryToken SPOut = sp_QueryToken(IDNO, ref flag, ref errCode);
            if (flag)
            {
                //檢查Token
                flag = hotaiMemberAPI.DoCheckToken(SPOut.AccessToken, ref errCode, ref HttpStatusCode);
                logger.Info($"DoQueryToken | DoCheckToken | Result:{ flag } ; errCode:{errCode} ; HttpStatusCode:{HttpStatusCode} ; | AccessToken :{$"{SPOut.AccessToken}"}");

                if (flag)
                {
                    output.OneID = SPOut.OneID;
                    output.AccessToken = SPOut.AccessToken;
                    output.RefreshToken = SPOut.RefreshToken;
                    output.IsCancel = SPOut.IsCancel;
                }
                else if (HttpStatusCode == 401) //Token 過期
                {
                    WebAPIInput_RefreshToken sp_refreshTokenInput = new WebAPIInput_RefreshToken()
                    {
                        access_token = SPOut.AccessToken,
                        refresh_token = SPOut.RefreshToken
                    };
                    flag = hotaiMemberAPI.DoRefreshToken(sp_refreshTokenInput, ref outputToken, ref errCode, ref HttpStatusCode);
                    logger.Info($"DoQueryToken | DoRefreshToken | Result:{ flag } ; errCode:{errCode} ; HttpStatusCode:{HttpStatusCode} ; | sp_refreshTokenInput:{JsonConvert.SerializeObject(sp_refreshTokenInput)}");

                    //更新Db Token
                    if (flag)
                    {
                        SPInput_SetToken sp_setTokenInput = new SPInput_SetToken()
                        {
                            IDNO = IDNO,
                            PRGName = PRGName,
                            AccessToken = outputToken.access_token,
                            RefreshToken = outputToken.refresh_token
                        };
                        flag = sp_SetToken(sp_setTokenInput, ref errCode);

                        if (flag)
                        {
                            output.OneID = SPOut.OneID;
                            output.AccessToken = outputToken.access_token;
                            output.RefreshToken = outputToken.refresh_token;
                            output.IsCancel = SPOut.IsCancel;
                        }
                    }
                }

                if (!flag)
                {
                    errCode = "ERR941";
                }
            }
            return flag;
        }
        /// <summary>
        /// 查詢和泰Token
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="flag"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public SPOutput_QueryToken sp_QueryToken(string IDNO, ref bool flag, ref string errCode)
        {
            SPInput_QueryToken spInput = new SPInput_QueryToken()
            {
                IDNO = IDNO,
            };
            string spName = "usp_HotaiToken_Q01";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPOutput_QueryToken spOutput = new SPOutput_QueryToken();
            SQLHelper<SPInput_QueryToken, SPOutput_QueryToken> sqlHelp = new SQLHelper<SPInput_QueryToken, SPOutput_QueryToken>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);
            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
            return spOutput;
        }
        /// <summary>
        /// 更新和泰Token
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool sp_SetToken(SPInput_SetToken spInput, ref string errCode)
        {
            bool flag = false;
            string spName = "usp_HotaiToken_U01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_SetToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetToken, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }

        /// <summary>
        /// 查詢和泰Pay預設卡
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="flag"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public SPOutput_HotaiGetDefaultCard sp_GetDefaultCard(string IDNO, long LogID, ref bool flag, ref string errCode)
        {
            SPInput_HotaiGetDefaultCard spInput = new SPInput_HotaiGetDefaultCard()
            {
                IDNO = IDNO,
                LogID = LogID
            };
            string spName = "usp_HotaiGetDefaultCard_Q01";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPOutput_HotaiGetDefaultCard spOutput = new SPOutput_HotaiGetDefaultCard();
            SQLHelper<SPInput_HotaiGetDefaultCard, SPOutput_HotaiGetDefaultCard> sqlHelp = new SQLHelper<SPInput_HotaiGetDefaultCard, SPOutput_HotaiGetDefaultCard>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return spOutput;
        }
        /// <summary>
        /// 綁定和泰Pay預設卡
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool sp_SetDefaultCard(SPInput_SetDefaultCard spInput, ref string errCode)
        {
            bool flag = true;
            string spName = "usp_SetHotaiDefaultCard_U01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_SetDefaultCard, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetDefaultCard, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
            return flag;
        }

        /// <summary>
        /// 解綁和泰會員
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool sp_MemberUnBind(SPInput_MemberUnBind spInput, ref string errCode)
        {
            bool flag = true;
            string spName = "usp_HotaiMemberUnBind_U01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_MemberUnBind, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_MemberUnBind, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
            return flag;
        }

        /// <summary>
        /// 和泰預設卡片失效
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool sp_HotaiDefaultCardUnbind(SPInput_HotaiDefaultCardUnbind spInput, ref string errCode)
        {
            string spName = "usp_HotaiDefaultCardUnbind_U01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_HotaiDefaultCardUnbind, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_HotaiDefaultCardUnbind, SPOutput_Base>(connetStr);
            bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }

        /// <summary>
        /// 和泰手動綁卡
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoAddCard(IFN_HotaiAddCard input, ref OFN_HotaiAddCard output, ref string errCode)
        {
            logger.Info($"DoAddCard | start | INPUT : {JsonConvert.SerializeObject(input)}");
            bool flag = true;
            HotaiPaymentAPI PaymentAPI = new HotaiPaymentAPI();
            output.postData = new HotaiResReqJsonPwd();

            //1.取得會員Token
            HotaiToken hotaiToken = new HotaiToken();
            flag = DoQueryToken(input.IDNO, input.PRGName, ref hotaiToken, ref errCode);
            logger.Info($"DoAddCard |Get AccessToken | Result:{ flag } ; errCode:{errCode} | IDNO :{input.IDNO} ; 會員Token : {JsonConvert.SerializeObject(hotaiToken)}");


            //2.向和泰取得新增卡片請求密文
            WebAPIOutput_AddHotaiCards apiOutput = new WebAPIOutput_AddHotaiCards();
            if (flag)
            {
                var apiInput = new WebAPIInput_AddCard
                {
                    AccessToken = hotaiToken.AccessToken,
                    RedirectURL = input.RedirectURL
                    
                };
                flag = PaymentAPI.AddCard(apiInput, ref apiOutput);

                logger.Info($"DoAddCard | GetAddCardPWD | Result:{ flag } ; errCode:{errCode} | apiOutput : {JsonConvert.SerializeObject(apiOutput)}");
            }
           

            if (flag)
            {
                output.postData = apiOutput?.PostData;
                output.gotoUrl = apiOutput.GotoUrl;
            }
            output.succ = flag;

            return flag;
        }


        /// <summary>
        /// 和泰中信卡快速綁定
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoFastAddCard(IFN_HotaiFastAddCard input, ref OFN_HotaiFastAddCard output, ref string errCode)
        {
            logger.Info($"DoFastAddCard | start | INPUT : {JsonConvert.SerializeObject(input)}");
            bool flag = true;
            HotaiPaymentAPI PaymentAPI = new HotaiPaymentAPI();
            output.postData = new HotaiResFastBind();

            //1.取得會員Token
            HotaiToken hotaiToken = new HotaiToken();
            flag = DoQueryToken(input.IDNO, input.PRGName, ref hotaiToken, ref errCode);
            logger.Info($"DoAddCard |Get AccessToken | Result:{ flag } ; errCode:{errCode} | IDNO :{input.IDNO} ; 會員Token : {JsonConvert.SerializeObject(hotaiToken)}");


            //2.向和泰取得新增卡片請求密文
            WebAPIOutput_FastAddHotaiCard apiOutput = new WebAPIOutput_FastAddHotaiCard();
            if (flag)
            {
                var apiInput = new WebAPIInput_FastAddCard
                {
                    AccessToken = hotaiToken.AccessToken,
                    RedirectURL = input.RedirectURL,
                    IDNO = input.CTBCIDNO,
                    Birthday = input.Birthday
                    
                };

                flag = PaymentAPI.FastAddCard(apiInput, ref apiOutput);

                logger.Info($"DoFastAddCard | GetFastAddCardPWD | Result:{ flag } ; errCode:{errCode} | apiOutput : {JsonConvert.SerializeObject(apiOutput)}");
            }

            if (flag)
            {
                output.postData = apiOutput?.PostData;
                output.gotoUrl = apiOutput.GotoUrl;
            }
            output.succ = flag;

            return flag;
        }

        /// <summary>
        /// 請求授權
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoReqPaymentAuth(IFN_HotaiPaymentAuth input, ref OFN_HotaiPaymentAuth output, ref string errCode)
        {
            logger.Info($"DoReqPaymentAuth | start | INPUT : {JsonConvert.SerializeObject(input)}");
            var flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            var spStep4Input = new SP_Input_HotaiTranStep4
            {
                LogID = input.LogID,
                CardType = 0,
                OrderNo = input.OrderNo,
                ChkClose = (input.AutoClose == 1) ? 1 : 0,
                ProName = input.PRGName,
                UserID = (input.insUser == input.PRGName) ? "" : ((input.insUser.Length > 20) ? input.insUser.Substring(0, 20) : input.insUser),
                AuthType = input.AuthType,
                MerchantMemberID = input.IDNO,
                process_date = DateTime.Now,
            };

            //1.取得會員Token
            HotaiToken hotaiToken = new HotaiToken();
            flag = DoQueryToken(input.IDNO, input.PRGName, ref hotaiToken, ref errCode);
            logger.Info($"DoReqPaymentAuth |Get AccessToken | Result:{ flag } ; errCode:{errCode} | IDNO :{input.IDNO} ; 會員Token : {JsonConvert.SerializeObject(hotaiToken)}");

            HotaiPaymentAPI PaymentAPI = new HotaiPaymentAPI();

            if (flag)
            {
                var WebAPI = new PayInfoForCredit();
                var temPayTypeInfo = WebAPI.GetPayTypeInfo(input.PayType);

                var spStep1Input = new SP_Input_HotaiTranStep1
                {
                    LogID = input.LogID,
                    amount = input.Amount,
                    OrderNo = input.OrderNo,
                    MemberID = input.IDNO,
                    AuthType = input.AuthType,
                    AutoClose = input.AutoClose,
                    CardToken = input.CardToken,
                    CreditType = input.PayType,
                    PrgName = input.PRGName,
                    PrgUser = input.insUser,
                    MerchantTradeNoLeft = string.Format("{0}{1}"
                       , temPayTypeInfo.FrontPart.Equals("OrderNo") ? input.OrderNo.ToString() : input.IDNO
                       , temPayTypeInfo.PaySuff
                       ),
                    MerchantID = merID,
                };

                var insStep1 = InsHotaiTranStep1(spStep1Input, ref flag, ref errCode, ref lstError);
                logger.Info($"DoReqPaymentAuth | insStep1 | Result:{ flag } ; errCode:{errCode} ; MerchantTradeNo :{insStep1.MerchantTradeNo} ; lstError : {JsonConvert.SerializeObject(lstError)}");
                var encpyptOutput = new WebAPIOutput_CreditCardPayEncrypt();
                if (flag)
                {
                    input.Transaction_no = insStep1?.MerchantTradeNo ?? "";
                    flag = string.IsNullOrEmpty(input.Transaction_no) ? false : true;
                }

                if (flag)
                {
                    logger.Info("DoReqPaymentAuth | insStep2 | Begin");
                    spStep4Input.MerchantTradeNo = input.Transaction_no;
                    spStep4Input.PretStep = 1;
                    var encpyptInput = new WebAPIInput_CreditCardPayEncpypt()
                    {
                        AccessToken = hotaiToken.AccessToken,
                        CardToken = input.CardToken,
                        MerID = merID,
                        TerMinnalID = terMinnalID,
                        Lidm = input.Transaction_no,
                        PurchAmt = input.Amount,
                        TxType = "0",
                        AutoCap = "0",
                        RedirectUrl = "",
                        PromoCode = input.PromoCode,
                    };

                    flag = PaymentAPI.CreaditCardPayEncpypt(encpyptInput, ref encpyptOutput);
                    logger.Info($"DoReqPaymentAuth | insStep2 | CreaditCardPayEncpypt | Result:{ flag } ; encpyptOutput : {JsonConvert.SerializeObject(encpyptOutput)}");

                    if (flag)
                    {
                        var spStep2Input = new SP_Input_HotaiTranStep2
                        {
                            LogID = input.LogID,
                            MerchantTradeNo = input.Transaction_no,
                            reqjsonpwd = encpyptOutput.PostData.reqjsonpwd,
                            PrgName = input.PRGName,
                            PrgUser = input.insUser,
                        };

                        InsHotaiTranStep2(spStep2Input, ref flag, ref errCode, ref lstError);

                        logger.Info($"DoReqPaymentAuth | insStep1 | InsHotaiTran | Result:{ flag } ; errCode:{errCode} ; lstError : {JsonConvert.SerializeObject(lstError)}");
                    }
                    else
                    {
                        spStep4Input.IsSuccess = -3;
                        //spStep4Input.RetMsg = "授權加密失敗";
                        spStep4Input.RetCode = encpyptOutput.ErrorCode;
                        spStep4Input.RetMsg = encpyptOutput.ErrorMessage ?? "授權加密失敗";
                        errCode = encpyptOutput.ErrorCode;
                    }

                    var apiOutput = new WebAPIOutput_CreditCardPay();

                    
                    if (flag)
                    {
                        logger.Info("DoReqPaymentAuth | insStep3 | Begin");
                        spStep4Input.PretStep = 2;
                        var apiInput = new WebAPIInput_CreditCardPay
                        {
                            PostData = encpyptOutput.PostData
                        };
                        flag = PaymentAPI.CreaditCardPay(apiInput, ref apiOutput);
                        logger.Info($"DoReqPaymentAuth | insStep3 | CreaditCardPay | Result:{ flag } ; encpyptOutput : {JsonConvert.SerializeObject(encpyptOutput)}");

                        if (flag)
                        {
                            var spStep3Input = new SP_Input_HotaiTranStep3
                            {
                                LogID = input.LogID,
                                MerchantTradeNo = input.Transaction_no,
                                PageTitle = apiOutput.PageTitle,
                                PageContent = apiOutput.PageText,
                                PrgName = input.PRGName,
                                PrgUser = input.insUser,
                            };

                            InsHotaiTranStep3(spStep3Input, ref flag, ref errCode, ref lstError);
                            logger.Info($"DoReqPaymentAuth | insStep3 | InsHotaiTran | Result:{ flag } ; errCode:{errCode} ; lstError : {JsonConvert.SerializeObject(lstError)}");

                        }
                        else
                        {
                            //中信授權連線異常
                            spStep4Input.IsSuccess = -2;
                            spStep4Input.RetCode = apiOutput.ErrorCode;
                            spStep4Input.RetMsg = apiOutput.ErrorMessage;
                            errCode = apiOutput.ErrorCode;
                        }
                    }
                    if (flag)
                    {
                        logger.Info("DoReqPaymentAuth | insStep4 | Begin");
                        spStep4Input.PretStep = 3;
                        var decryptInput = new WebAPIInput_DecryptCTBCHtml()
                        {
                            AccessToken = hotaiToken.AccessToken,
                            PageText = apiOutput.PageText

                        };
                        var decryptOut = new WebAPIOutput_DecryptCTBCHtml();
                        flag = PaymentAPI.DecryptCTBCHtml(decryptInput, ref decryptOut);
                        logger.Info($"DoReqPaymentAuth | insStep4 | DecryptCTBCHtml | Result:{ flag } ; encpyptOutput : {JsonConvert.SerializeObject(decryptOut)}");
                        
                       
                        if (!flag && decryptOut.ErrorCode != "000000")
                        {
                            //和泰解析異常
                            spStep4Input.IsSuccess = -3;
                            spStep4Input.RetCode = decryptOut.ErrorCode;
                            spStep4Input.RetMsg = decryptOut.ErrorMessage;
                            errCode = decryptOut.ErrorCode;
                        }
                        else
                        {
                            spStep4Input.AuthErrcode = decryptOut.Errcode;
                            spStep4Input.MemberID = decryptOut.MemberID;
                            spStep4Input.MerchantID = decryptOut.MerchantID;
                            spStep4Input.MerID = decryptOut.MerID;
                            spStep4Input.RequestNo = decryptOut.RequestNo;
                            spStep4Input.AuthStatus = decryptOut.Status;
                            spStep4Input.AuthStatusCode = decryptOut.StatusCode;
                            spStep4Input.AuthStatusDesc = decryptOut.StatusDesc;
                            spStep4Input.RetCode = flag ? "1000" : decryptOut.StatusCode;
                            spStep4Input.RetMsg = flag ? "交易成功" : decryptOut.StatusDesc;

                            //授權交易失敗
                            spStep4Input.IsSuccess = -1;

                            if (flag)
                            {
                                spStep4Input.IsSuccess = 1;
                                spStep4Input.AuthAmt = decryptOut.AuthAmt;
                                spStep4Input.AuthCode = decryptOut.AuthCode;
                                spStep4Input.Authrrpid = decryptOut.Authrrpid;
                                spStep4Input.CardNumber = decryptOut.CardNumber;
                                spStep4Input.Last4digitPAN = decryptOut.Last4digitPAN;
                                spStep4Input.NumberOPay = decryptOut.NumberOPay;
                                spStep4Input.TermSeq = decryptOut.TermSeq;
                                spStep4Input.RetrRef = decryptOut?.RetrRef ?? "";
                                spStep4Input.Xid = decryptOut.Xid;

                            }
                        }

                    }
                    var resultFlag = true;
                    InsHotaiTranStep4(spStep4Input, ref resultFlag, ref errCode, ref lstError);
                }
            }
            output.RtnCode = spStep4Input?.RetCode ?? "0";
            output.AuthCode = spStep4Input?.RetCode?? "";
            output.AuthMessage = spStep4Input?.RetMsg ?? "";
            output.CardNo = spStep4Input?.CardNumber ?? "";
            output.BankTradeNo = spStep4Input?.Xid ?? "";
            output.Transaction_no = input?.Transaction_no ?? "";
            output.PreStep = spStep4Input.PretStep;

            return flag;
        }

        private HotaiCardInfo setHotaiCardInfo(HotaiCardInfoOriginal input, string defaultCardToken)
        {
            return new HotaiCardInfo
            {
                CardToken = input.Id.ToString(),
                CardName = input.AliasName,
                CardType = input.CardType,
                BankDesc = input.BankDesc,
                CardNumber = input.CardNoMask,
                IsDefault = input.Id.ToString().Equals(defaultCardToken) ? 1 : 0,
                MemberOneID = input.MemberOneID,
                BankCode = input.BinInfo?.Code + "" ?? "",
            };
        }

        /// <summary>
        /// 測試卡清單
        /// </summary>
        /// <returns></returns>
        private List<HotaiCardInfo> GetTestCards()
        {
            var creditCards = new List<HotaiCardInfo>();

            creditCards.Add(new HotaiCardInfo
            {
                CardToken = 1385.ToString(),
                CardName = "",
                CardType = "Visa",
                BankDesc = "國外卡",
                CardNumber = "****-****-****-5278",
                IsDefault = 1,
                MemberOneID = "0064fb4f-8250-4690-954b-2ba94862606b",
                BankCode = "822222"
            });
            return creditCards;
        }


        public bool DoQueryCTBCTransaction(WebAPIInput_InquiryByLidm input, out WebAPIOutput_InquiryByLidm output,ref string errCode)
        {
            output = new WebAPIOutput_InquiryByLidm();

            CTBCPosAPI posAPI = new CTBCPosAPI();

            var flag = posAPI.QueryCTBCTransaction(input, out output);

            return flag;
        }
        /// <summary>
        /// 解析AccessToken取得IDNO
        /// </summary>
        /// <param name="Access_Token"></param>
        /// <param name="LogID"></param>
        /// <param name="IDNO"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool GetIDNOFromToken(string Access_Token, Int64 LogID, ref string IDNO, ref List<ErrorInfo> lstError, ref string errCode)
        {
            bool flag = true;
            string CheckTokenName = "usp_CheckTokenReturnID";
            SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
            {
                LogID = LogID,
                Token = Access_Token
            };
            SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
            SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                IDNO = spOut.IDNO;
            }
            return flag;
        }


        public SPOutput_HotaiTranStep1 InsHotaiTranStep1(SP_Input_HotaiTranStep1 input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SP_Input_HotaiTranStep1, SPOutput_HotaiTranStep1> SqlHelper = new SQLHelper<SP_Input_HotaiTranStep1, SPOutput_HotaiTranStep1>(connetStr);
            SPOutput_HotaiTranStep1 spOut = new SPOutput_HotaiTranStep1();
            string SPName = "usp_HotaiTranStep1_I01";//new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);

            if (flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return spOut;
        }

        public void InsHotaiTranStep2(SP_Input_HotaiTranStep2 input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SP_Input_HotaiTranStep2, SPOutput_Base> SqlHelper = new SQLHelper<SP_Input_HotaiTranStep2, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_HotaiTranStep2_U01";//new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if(flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }

        public void InsHotaiTranStep3(SP_Input_HotaiTranStep3 input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SP_Input_HotaiTranStep3, SPOutput_Base> SqlHelper = new SQLHelper<SP_Input_HotaiTranStep3, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_HotaiTranStep3_U01";//new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if(flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }

        public void InsHotaiTranStep4(SP_Input_HotaiTranStep4 input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SP_Input_HotaiTranStep4, SPOutput_Base> SqlHelper = new SQLHelper<SP_Input_HotaiTranStep4, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_HotaiTranStep4_U01";//new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }


        /// <summary>
        /// 驗證SP回傳值
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="Error"></param>
        /// <param name="ErrorCode"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        public void checkSQLResult(ref bool flag, int Error, string ErrorCode, ref List<ErrorInfo> lstError, ref string errCode)
        {
            if (flag)
            {
                if (Error == 1)
                {
                    lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                    errCode = ErrorCode;
                    flag = false;
                }
                else
                {
                    if (ErrorCode != "0000")
                    {
                        lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                        errCode = ErrorCode;
                        flag = false;
                    }
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        public Dictionary<string,string> QueryStringDecryption(string encrypStr)
        {
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(encrypStr))
            {
                //base64解碼
                string KEY = configManager.GetKey("AESKEY").Trim();
                string IV = configManager.GetKey("AESIV").Trim();
                string ReqParam = AESEncrypt.DecryptAES128(encrypStr, KEY, IV);

                if (ReqParam != "")
                { 
                    var parms = ReqParam.Split(new char[] { '&' }).ToList();
                    foreach(var param in parms)
                    {
                        var resultQStr = param.Split(new char[] { '=' }).ToList();
                        if (resultQStr.Count==2)
                            resultDic.Add(resultQStr[0], resultQStr[1]);
                            
                    }
                }

                return resultDic;
            }
            else
                return new Dictionary<string, string>();

            
        }
    }
}