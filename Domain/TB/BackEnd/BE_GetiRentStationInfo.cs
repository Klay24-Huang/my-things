using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 據點照片及描述
    /// </summary>
    public class BE_GetiRentStationInfo
    {
        /// <summary>
        /// PK
        /// </summary>
        public int iRentStationInfoID{set;get;}
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID         {set;get;}
        /// <summary>
        /// 據點照片
        /// </summary>
        public string StationPic        {set;get;}
        /// <summary>
        /// 排序（1~4)
        /// </summary>
        public Int16 Sort              {set;get;}
        /// <summary>
        /// 照片描述
        /// </summary>
        public string PicDescription { set; get; }
    }
}
