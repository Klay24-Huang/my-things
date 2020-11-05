using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台電子柵欄資料
    /// </summary>
    public class BE_BE_GetPolygonData
    {
        /// <summary>
        /// PK
        /// </summary>
        public Int64 BLOCK_ID    {set;get;}
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID   {set;get;}
        /// <summary>
        /// 電子柵欄名稱
        /// </summary>
        public string BlockName   {set;get;}
        /// <summary>
        /// 類型
        /// <para>0:可還車</para>
        /// <para>1:可還車裡的不可還車</para>
        /// <para>2:優惠</para>
        /// </summary>
        public string BlockType   {set;get;}
        /// <summary>
        /// 模式
        /// <para>0:優惠的出車</para>
        /// <para>1:優惠的還車</para>
        /// </summary>
        public string PolygonMode {set;get;}
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude   {set;get;}
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude    {set;get;}
        /// <summary>
        /// 顏
        /// </summary>
        public string MAPColor    {set;get;}
        /// <summary>
        /// 起日
        /// </summary>
        public string StartDate   {set;get;}
        /// <summary>
        /// 迄日
        /// </summary>
        public string EndDate { set; get; }
    }
}
