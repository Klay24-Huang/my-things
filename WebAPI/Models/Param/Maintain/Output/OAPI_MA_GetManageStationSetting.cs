using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Output
{
    /// <summary>
    /// 輸出用帳號取得設定的管轄門市
    /// </summary>
    public class OAPI_MA_GetManageStationSetting
    {
        /// <summary>
        /// 管轄據點帳號
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 管轄據點名稱
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 是否有設定管轄
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int isSelected { set; get; }
    }
}