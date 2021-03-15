using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetFavoriteStation
    {
        /// <summary>
        /// 改為LIST方式  20210315 ADD BY ADAM 
        /// </summary>
        public List<FavoriteStation> FavoriteStations { get; set; }
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