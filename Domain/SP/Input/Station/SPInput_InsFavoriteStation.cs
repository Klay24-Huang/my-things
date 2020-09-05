using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Station
{
    /// <summary>
    /// 設定/移除常用站點
    /// </summary>
   public class SPInput_InsFavoriteStation
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:移除</para>
        /// <para>1:設定</para>
        /// </summary>
        public Int16 Mode { set; get; }
 
        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
