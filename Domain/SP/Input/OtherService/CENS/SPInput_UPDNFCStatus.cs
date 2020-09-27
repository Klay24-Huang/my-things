using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.CENS
{
    /// <summary>
    /// 更新興聯讀卡機狀態
    /// </summary>
    public class SPInput_UPDNFCStatus
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// NFC電源控制
        /// <para>0:關閉</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int Mode { set; get; }
        public Int64 LogID { set; get; }
    }
}
