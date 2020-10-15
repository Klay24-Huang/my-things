using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Params.Search.Input
{
    public class Input_CarDashBoard
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { get; set; }
        /// <summary>
        /// 顯示類型
        /// <para>2:全部</para>
        /// <para>1:僅顯示有回應</para>
        /// <para>0:僅顯示無回應</para>
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 篩選條件
        /// <para>0:低電量機車(3TBA)</para>
        /// <para>1:低電量機車(2TBA)</para>
        /// <para>2:發動</para>
        /// <para>3:電池蓋開啟</para>
        /// <para>4:一小時無回應</para>
        /// <para>5:無回應</para>
        /// </summary>
        public List<string> Terms { get; set; }
    }
}