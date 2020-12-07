using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_QueryOrderMachiParkData
    {
        /// 停車場名稱
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 所在縣市
        /// </summary>
        public string city { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string addr { set; get; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 入場時間
        /// </summary>
        public DateTime Check_in { set; get; }
        /// <summary>
        /// 出場時間
        /// </summary>
        public DateTime Check_out { set; get; }
    }
}
