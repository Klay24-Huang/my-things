using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_HandlePolygon:IAPI_BE_Base
    {
        public List<BE_Polygon> polygon { get; set; }
        public string StationID { get; set; }
        /// <summary>
        /// PK, 0代表新增
        /// </summary>
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string MAPColor { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 模式
        /// <para>0:可還車</para>
        /// <para>1:不可還車</para>
        /// </summary>
        public int Mode { get; set; }
    }
    public class BE_Polygon
    {
        public List<PolygonRawData> RawData { get; set; }
    }
    public class PolygonRawData
    {
        public string lng { get; set; }
        public string lat { get; set; }
        public string MAPColor { get; set; }
    }
}