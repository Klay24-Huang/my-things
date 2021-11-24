using Domain.Flow.Hotai;
using Domain.SP.Input.Hotai;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output.Hotai.Member;
using Domain.WebAPI.output.Hotai.Payment;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebCommon;

namespace OtherService
{
    public class HotaipayService
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        HotaiMemberAPI hotaiMemberAPI = new HotaiMemberAPI();
        /// <summary>
        /// 取得和泰卡片清單
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="cardList"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoQueryCardList(IFN_QueryCardList input, ref OFN_HotaiCreditCardList cardList, ref string errCode)
        {
            HotaiPaymentAPI PaymentAPI = new HotaiPaymentAPI();
            bool flag = true;
            HotaiToken hotaiToken = new HotaiToken();
            //1.取得會員Token
            flag = DoQueryToken(input.IDNO, input.PRGName, ref hotaiToken, ref errCode);
            //2.向中信取得卡清單
            WebAPIOutput_GetCreditCards cardOptput = new WebAPIOutput_GetCreditCards();

            if (flag)
            {
                var objGetCard = new WebAPIInput_GetCreditCards
                {
                    AccessToken = hotaiToken.AccessToken
                };
                flag = PaymentAPI.GetHotaiCardList(objGetCard, ref cardOptput);
            }
            //3.資料庫取得預設卡
            if (flag)
            {
                sp_GetDefaultCard(input.IDNO, input.LogID, ref flag, ref errCode);
            }
            //4.比對預設卡與卡清單
            if (flag)
            {
                //SetDefault
            }
            //5.整理後回傳

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

                flag = (card == null || card == default) ? false : true;
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

    }
}