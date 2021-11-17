using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Bill;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Service
{
    public class HotaipayService
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

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



    }
}