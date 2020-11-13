using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_Login:SPOutput_Base
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName {set;get;}
        /// <summary>
        /// 群組
        /// </summary>
        public string UserGroup{set;get; }
        /// <summary>
        /// 權限
        /// </summary>
        public string PowerList { set; get; }
    }
}
