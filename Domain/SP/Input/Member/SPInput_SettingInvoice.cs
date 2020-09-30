using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Member
{
    public class SPInput_SettingInvoice : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 發票方式
        /// <para>1:愛心碼</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int InvoiceType {set;get;}
        /// <summary>
        /// 設定模式
        /// <para>0:會員</para>
        /// <para>1:還車</para>
        /// </summary>
        public Int16 SettingMode {set;get;}
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN      {set;get;}
        /// <summary>
        /// 統編
        /// </summary>
        public string UniCode     {set;get;}
        /// <summary>
        /// 手機條碼
        /// </summary>
        public string CARRIERID   {set;get;}


        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
    }
}
