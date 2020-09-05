using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetParkingData
    {
        /// <summary>
        /// 是否顯示全部
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int? ShowALL { set; get; }
        /// <summary>
        /// 停車場類型
        /// <para>0:一般（調度）</para>
        /// <para>1:特約（車麻吉及其他）</para>
        /// <para>2:全部顯示</para>
        /// </summary>
        public int? Mode { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double? Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public double? Longitude { set; get; }
        /// <summary>
        /// 半徑
        /// </summary>
        public double? Radius { set; get; }
    }
}