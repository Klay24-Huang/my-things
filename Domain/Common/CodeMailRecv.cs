using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    /// <summary>
    /// MAIL收件人
    /// </summary>
    public class CodeMailRecv
    {
        /// <summary>
        /// EMail
        /// </summary>
        public string Mail { get; set;}
        
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecvName { get; set; }
    }
}

