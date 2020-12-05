using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_FeedBackHandle:SPInput_Base
    {
        public Int64 FeedBackID { set; get; }
        /// 處理訊息
        /// </summary>
        public string HandleDescript { set; get; }
        public string UserID { set; get; }
    }
}
