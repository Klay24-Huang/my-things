using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 集團服務
    /// </summary>
    public class WebAPIOutput_GroupApps:WebAPIOutput_Base
    {
       public List<GroupApps> groupApps { get; set; }
    }

    public class GroupApps 
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 類型 (App;web)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 網址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 圖片網址
        /// </summary>
        public string PicUrl { get; set; }
    }

}
