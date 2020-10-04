using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class DetailData
    {
        /// <summary>
        /// 交易日期，格式yyyyMMddHHmmss
        /// </summary>
        public string TransDate { set; get; }
        /// <summary>
        /// 金額
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 交易類型
        ///<para>T001 = 交易扣款</para>
        ///<para>T002=交易退款</para>
        ///<para>T003 = 兩階段儲值待確認</para>
        ///<para>T004=兩階段儲值已確認</para>
        ///<para>T005 = 取消儲值</para>
        ///<para>T006=直接儲值</para>
        ///<para>T007 = 儲值退款</para>
        ///<para>T008=會員轉贈</para>
        ///<para>T011 = 批次儲值</para>
        /// </summary>
        public string TransType { set; get; }
        /// <summary>
        /// 交易類型中文
        /// </summary>
        public string TransTypeName { set; get; }
    }
}