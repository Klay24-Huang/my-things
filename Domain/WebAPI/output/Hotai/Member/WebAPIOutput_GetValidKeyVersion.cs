using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 取得後台金鑰有效版本號
    /// </summary>
    public class WebAPIOutput_GetValidKeyVersion
    {
       public List<Data> Data { get; set; } 
    }
    
    public class Data 
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string versions { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int seq { get; set; }

        /// <summary>
        /// 到期時間
        /// </summary>
        public DateTime? expiredate { get; set; }
    }

    
}
