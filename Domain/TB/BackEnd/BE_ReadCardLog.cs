using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 讀卡紀錄
    /// </summary>
    public class BE_ReadCardLog
    {
        public Int64 ReadCardID { set; get; }
        public DateTime ReadTime { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        public string Status { set; get; }
    }
}
