using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Input
{
    public class IAPI_MA_GetCleanCarByLatLng
    {
        /// <summary>
        /// 目前模式
        /// <para>0:同站</para>
        /// <para>1:路邊租還</para>
        /// <para>4:機車</para>
        /// </summary>
        public int NowType { set; get; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public double lng { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double lat { set; get; }
        /// <summary>
        /// 半徑
        /// </summary>
        public double raduis { set; get; }
    }
}