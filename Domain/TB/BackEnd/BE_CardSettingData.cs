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
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 模式：0:清空;1:寫入
        /// </summary>
        public int Mode { set; get; }
        /// <summary>
        /// 卡別：0:一般;2:萬用卡
        /// </summary>
        public int CardType { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// 是否成功：0:失敗;1:成功
        /// </summary>
        public int IsSuccess { set; get; }
        /// <summary>
        /// 執行時間
        /// </summary>
        public DateTime MKTime { set; get; }
    }
}
