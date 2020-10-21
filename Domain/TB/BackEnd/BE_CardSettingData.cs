using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 卡號設定紀錄
    /// </summary>
    public class BE_CardSettingData
    {
        /// <summary>
        /// 發送時間
        /// </summary>
        public DateTime SendDate { set; get; }
        /// <summary>
        /// 發送狀態
        /// </summary>
        public string SendStatus { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
    }
}
