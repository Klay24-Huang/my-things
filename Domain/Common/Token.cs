using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class Token
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Access_token { set; get; }
        /// <summary>
        /// Refrash Token
        /// </summary>
        public string Refrash_token { set; get; }
        /// <summary>
        /// 有效期限(單位秒)
        /// </summary>
        public int Rxpires_in { set; get; }
        /// <summary>
        /// Refrash 有效期限(單位秒)
        /// </summary>
        public int Refrash_Rxpires_in { set; get; }
    }
}
