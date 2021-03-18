using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.rootAPI
{
   public class GetProjectObj
    {
        public string StationID { set; get; }
        public string StationName { set; get; }
        /// <summary>
        /// 縣市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 行政區
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 其他說明
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 據點描述（app顯示）
        /// </summary>
        public string ContentForAPP { get; set; }
        /// <summary>
        /// 最低價
        /// </summary>
        public int Minimum { set; get; }

        /// <summary>
        /// 是否有車可租(BY據點)
        /// </summary>
        public string IsRent { set; get; }

        /// <summary>
        /// 是否是常用據點 20210315 ADD BY ADAM
        /// </summary>
        public int IsFavStation { set; get; }

        /// <summary>
        /// 專案資料
        /// </summary>
        public List<ProjectObj> ProjectObj { set; get; }

        /// <summary>
        /// 站點照片
        /// </summary>
        public List<StationInfoObj> StationInfoObj { get; set; }
    }
}
