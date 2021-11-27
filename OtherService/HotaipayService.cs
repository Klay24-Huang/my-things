using Domain.Flow.Hotai;
using Domain.SP.Input.Hotai;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output.Hotai.Member;
using Domain.WebAPI.output.Hotai.Payment;
using Newtonsoft.Json;
using NLog;
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
        private string isDebug = ConfigurationManager.AppSettings["isDebug"]?.ToString()??"";
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
            if (isDebug == "1")
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
                        new SPInput_SetDefaultCard {
                            IDNO = input.IDNO,
                            OneID = originalCard.MemberOneID,
                            CardNo = originalCard.CardNoMask,
                            CardToken = originalCard.Id.ToString(),
                            CardType = originalCard.CardType,
                            BankDesc = originalCard.BankDesc,
                            PRGName = input.PRGName
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
                if (!hasDefault)
                {
                    flag = sp_HotaiDefaultCardUnbind(
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
            if (output.CreditCards?.Count() == 0)
                flag = false;

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
            SPOutput_QueryToken SPOut = sp_QueryToken(IDNO, ref flag, ref errCode);
            if (flag && !string.IsNullOrWhiteSpace(SPOut.AccessToken))
            {
                WebAPIInput_RefreshToken inputToken = new WebAPIInput_RefreshToken()
                {
                    access_token = SPOut.AccessToken,
                    refresh_token = SPOut.RefreshToken
                };

                WebAPIOutput_Token outputToken = new WebAPIOutput_Token();
                flag = hotaiMemberAPI.DoRefreshToken(inputToken, ref outputToken, ref errCode);

                #region 更新和泰會員綁定記錄
                if (flag)
                {
                    SPInput_SetToken inputSetToken = new SPInput_SetToken()
                    {
                        IDNO = IDNO,
                        PRGName = PRGName,
                        AccessToken = outputToken.access_token,
                        RefreshToken = outputToken.refresh_token
                    };
                    flag = sp_SetToken(inputSetToken, ref errCode);
                }
                #endregion

                if (flag)
                {
                    output.AccessToken = outputToken.access_token;
                    output.RefreshToken = outputToken.refresh_token;
                    output.OneID = SPOut.OneID;
                }
            }

            if (!flag)
            {
                errCode = "ERR941";
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

        private HotaiCardInfo setHotaiCardInfo(HotaiCardInfoOriginal input,string defaultCardToken)
        {
            return new HotaiCardInfo
            {
                CardToken = input.Id.ToString(),
                CardName = input.AliasName,
                CardType = input.CardType,
                BankDesc = input.BankDesc,
                CardNumber = input.CardNoMask,
                IsDefault = input.Id.ToString().Equals(defaultCardToken) ? 1 : 0,
                MemberOneID = input.MemberOneID

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
                CardToken = "1385",
                CardName = "",
                CardType = "Visa",
                BankDesc = "國外卡",
                CardNumber = "****-****-****-5278",
                IsDefault = 1,
                MemberOneID = "0064fb4f-8250-4690-954b-2ba94862606b"
            });
            return creditCards;
        }

    }
}