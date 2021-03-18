using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetFavoriteStation
    {
        /// <summary>
        /// 設定最愛站點
        /// </summary>
        public List<FavoriteStation> FavoriteStations { get; set; }
    }

    public class FavoriteStation
    {
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
    }
}