using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public  class BE_StationDetailCombind
    {
        /// <summary>
        /// 據點明細
        /// </summary>
        public BE_GetStationInfo detail { set; get; }
        /// <summary>
        /// 據點照片
        /// </summary>
        public List<BE_GetiRentStationInfo> StationImage { set; get; }
        /// <summary>
        /// 電子柵欄
        /// </summary>
        public List<BE_BE_GetPolygonData> StationPolygon { set; get; }
    }
}
