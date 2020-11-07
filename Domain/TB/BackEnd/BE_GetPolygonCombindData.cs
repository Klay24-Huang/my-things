using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public class BE_GetPolygonCombindData
    {
        public int BLOCK_ID        {set;get;}
        public string StationID       {set;get;}
        public decimal Latitude        {set;get;}
        public decimal Longitude       {set;get;}
        public string BlockName       {set;get;}
        /// <summary>
        /// 類型
        /// <para>0:可還車</para>
        /// <para>1:可還車裡的不可還車</para>
        /// <para>2:優惠</para>
        /// </summary>
        public string BlockType { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:優惠的出車</para>
        /// <para>1:優惠的還車</para>
        /// </summary>
        public string PolygonMode { set; get; }
        public string PolygonLongitude {set;get;}
        public string PolygonLatitude {set;get;}
        public string MAPColor        {set;get;}
        public DateTime StartDate       {set;get;}
        public DateTime EndDate { set; get; }
    }
}
