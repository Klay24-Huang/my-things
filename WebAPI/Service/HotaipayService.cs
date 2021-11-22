using Domain.SP.Input.Hotai;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Bill;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebCommon;

namespace WebAPI.Service
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
        public bool DoQueryCardList(string IDNO, ref OFN_HotaiCreditCardList cardList, ref string errCode)
        {
            bool flag = true;
            string HotaiToken = "";
            //todo DoQueryCardList

            //1.取得會員Token
            //flag = GetToken();
            //2.向中信取得卡清單
            if (flag)
            {
                //flag = GetList();
            }
            //3.資料庫取得預設卡
            if(flag)
            {
                //flag = GetDafaultCard(); 
            }
            //4.比對預設卡與卡清單
            if(flag)
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
        public bool DoQueryDefaultCard(string IDNO, ref HotaiCardInfo card, ref string errCode)
        {
            bool flag = true;
            OFN_HotaiCreditCardList hotaiCards = new OFN_HotaiCreditCardList();
            
            flag = DoQueryCardList(IDNO, ref hotaiCards, ref errCode);
            if(flag)
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


            flag = DoQueryCardList(input.IDNO, ref hotaiCards, ref errCode);
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
        public bool DoQueryToken(string IDNO, long LogID, string PRGName, ref HotaiToken output, ref string errCode)
        {
            bool flag = true;
            SPOutput_QueryToken SPOut = sp_QueryToken(IDNO, LogID, ref flag, ref errCode);
            if (flag && !string.IsNullOrWhiteSpace(SPOut.AccessToken))
            {
                WebAPIInput_RefreshToken inputToken = new WebAPIInput_RefreshToken()
                {
                    access_token = SPOut.AccessToken,
                    refresh_token = SPOut.RefreshToken
                };

                WebAPIOutput_Token outputToken = new WebAPIOutput_Token();
                flag = hotaiMemberAPI.DoRefreshToken(inputToken, ref outputToken,ref errCode);

                #region 更新和泰會員綁定記錄
                if (flag)
                {
                    SPInput_SetToken inputSetToken = new SPInput_SetToken()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
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
        public SPOutput_QueryToken sp_QueryToken(string IDNO, long LogID, ref bool flag, ref string errCode)
        {
            SPInput_QueryToken spInput = new SPInput_QueryToken()
            {
                IDNO = IDNO,
                LogID = LogID
            };
            string spName = "usp_HotaiToken_Q01";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPOutput_QueryToken spOut = new SPOutput_QueryToken();
            SQLHelper<SPInput_QueryToken, SPOutput_QueryToken> sqlHelp = new SQLHelper<SPInput_QueryToken, SPOutput_QueryToken>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            CommonFunc baseVerify = new CommonFunc();
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return spOut;
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


    }
}