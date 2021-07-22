using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Escrow
{
    public class WSInput_ReturnStoreValue: WSInput_EscrowBase
    {
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 直接儲值/兩階段儲值已確認回傳之交易序號
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// 交易來源Ex:1
        /// </summary>
        public string SourceFrom { get; set; }
        /// <summary>
        /// 檢查紅利方式
        /// <para>1=不檢查</para>
        /// <para>2=需要返還當筆交易的紅利</para>
        /// <para>3=可返還其它有效紅利(預設)</para>
        /// </summary>
        public string BonusCheck { get; set; } = "3";
    }
}
