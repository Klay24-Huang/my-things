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
        public string returnDate { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}