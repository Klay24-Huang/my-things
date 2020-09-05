using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetUPDList
    {
       
        /// <summary>
        /// 行政區最近更新時間
        /// <para>格式：yyyyMMddHHmmSS?</para>
        /// </summary>
        public string AreaList { set; get; }
        /// <summary>
        /// 愛心捐贈碼最近更新時間
        ///  <para>格式：yyyyMMddHHmmSS?</para>
        /// </summary>
        public string LoveCode { set; get; }
        /// <summary>
        /// 同站最新更新時間
        ///  <para>格式：yyyyMMddHHmmSS?</para>
        /// </summary>
        public string NormalRent { set; get; }
        /// <summary>
        /// 電子柵欄最近更新時間
        ///  <para>格式：yyyyMMddHHmmSS?</para>
        /// </summary>
        public string Polygon { set; get; }
        /// <summary>
        /// 停車場最近更新時間
        ///  <para>格式：yyyyMMddHHmmSS?</para>
        /// </summary>
        public string Parking { set; get; }
    }
}