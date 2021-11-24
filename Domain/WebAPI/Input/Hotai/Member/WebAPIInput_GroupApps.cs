using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 集團服務
    /// </summary>
    public class WebAPIInput_GroupApps
    {
        /// <summary>
        /// 行動裝置類別(0:官網;1:ios;2:android)
        /// </summary>
        public int deviceOS { get; set; }

    }
}
