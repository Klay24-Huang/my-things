using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 行政區列表
    /// </summary>
    public class WebAPIOutput_Townships
    {
        public List<counties> counties { get; set; }
        public List<townships> townships { get; set; }
    }

    /// <summary>
    /// 城市
    /// </summary>
    public class counties
    {
        /// <summary>
        /// 文字
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 編號
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 城市代號
        /// </summary>
        public string groupId { get; set; }
    }

    /// <summary>
    /// 鄉鎮市區
    /// </summary>
    public class townships
    {
        /// <summary>
        /// 文字
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 編號
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 城市代號
        /// </summary>
        public string groupId { get; set; }
    }
}
