using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BonusQuery
    {
        /// <summary>
        /// 點數類型
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int PointType { set; get; }
        /// <summary>
        /// 點數名稱
        /// </summary>
        public string GIFTNAME { set; get; }
        /// <summary>
        /// 點數
        /// </summary>
        public string GIFTPOINT { set; get; }
        /// <summary>
        /// 結束日
        /// </summary>
        public string EDATE { set; get; }
        /// <summary>
        /// 剩餘點數
        /// </summary>
        public string LASTPOINT { set; get; }
        /// <summary>
        /// 是否能贈送
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int AllowSend { set; get; }
    }
}