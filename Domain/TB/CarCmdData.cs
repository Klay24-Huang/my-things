using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 取出車輛發送車機指令資料
    /// </summary>
    public class CarCmdData
    {
        /// <summary>
        /// 車牌號碼
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 和雲車輛序號
        /// </summary>
        public int TSEQNO { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 遠傳車機token
        /// </summary>
        public string deviceToken { set; get; }
        /// <summary>
        /// 是否為興聯車機(0:否;1:是)
        /// </summary>
        public int IsCens { set; get; }
        /// <summary>
        /// 是否為機車（0:否;1:是)
        /// </summary>
        public int IsMotor { set; get; }
    }
}
