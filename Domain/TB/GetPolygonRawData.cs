using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class GetPolygonRawData
    {
        /// <summary>
        /// 電子柵欄類型
        /// <para>0:可還車區域</para>
        /// <para>1:可還車區域中的不可還車區域</para>
        /// <para>2:高風險區域</para>
        /// <para>3:優惠區域</para>
        /// </summary>
        public int  PolygonMode{set;get;}
        /// <summary>
        /// 經度
        /// </summary>
        public string  Longitude { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public string Latitude { set; get; }
    }
}
