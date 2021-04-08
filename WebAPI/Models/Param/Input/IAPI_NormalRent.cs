using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_NormalRent
    {
        /// <summary>
        /// 是否顯示全部
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int? ShowALL { set; get; }
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
        public List<string> CarTypes { get; set; } = new List<string>();
        public List<int> Seats { get; set; } = new List<int>();
        public DateTime? SD { get; set; }
        public DateTime? ED { get; set; }
    }
}