using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Input
{
    /// <summary>
    /// 【整備人員】設定管轄據點
    /// </summary>
    public class IAPI_MA_SettingManageStation
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 管轄據點代碼
        /// </summary>
        public string[] StationID { set; get; }
    }
}