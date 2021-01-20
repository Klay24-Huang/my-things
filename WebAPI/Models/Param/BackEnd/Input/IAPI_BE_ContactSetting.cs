using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_ContactSetting
    {
        public string OrderNo{ set; get; }
        /// <summary>
        /// 動作類型
        /// <para>0:強制取車</para>
        /// <para>1:強制還車</para>
        /// <para>2:強制取消</para>
        /// </summary>
        public Int16 type { set; get; }
        /// <summary>
        /// 動作用途
        /// <para>0:會員</para>
        /// <para>1:清潔取還車</para>
        /// <para>2:保修取還車</para>
        /// </summary>
        public Int16 Mode { set; get; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public string returnDate { set; get; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string bill_option { set; get; }
        /// <summary>
        /// 手機條碼載具,自然人憑證載具
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string unified_business_no { set; get; }
        /// <summary>
        /// 停車格
        /// </summary>
        public string parkingSpace { set; get; }
        /// <summary>
        /// 車機出錯是否bypass
        /// <para>1:是</para>
        /// <para>0:否</para>
        /// </summary>
        public int ByPass { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}