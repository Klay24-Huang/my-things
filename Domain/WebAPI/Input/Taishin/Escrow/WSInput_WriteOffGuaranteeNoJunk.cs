using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Escrow
{
    public class WSInput_WriteOffGuaranteeNoJunk: WSInput_EscrowBase
    {
        /// <summary>
        /// 交易類別
        /// <para>T016 : 禮券序號核銷</para>
        /// <para>T025 : 禮券序號報廢</para>
        /// </summary>
        public string TransType { get; set; }
        /// <summary>
        /// 核銷資料 (禮券序號+核銷碼，傳入張數以1000為限
        /// </summary>
        public string[] WriteOffData { get; set; }
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 交易來源
        /// </summary>
        public string SourceFrom { get; set; }
    }
}
