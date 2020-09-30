﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetProject
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        public string CarType { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SDate { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string EDate { set; get; }
        /// <summary>
        /// 顯示方式
        /// <para>0:依據點代碼(此時據點代碼為必填)</para>
        /// <para>1:依經緯度（此時經緯度與半徑為必填)</para>
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