using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Escrow
{
    public class WSInput_GetGuaranteeNo: WSInput_EscrowBase
    {
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 金額類別
        /// <para>1:以現金購買</para>
        /// <para>2:以信用卡購買</para>
        /// <para>3:以收款餘額購買</para>
        /// <para>0:活動贈送</para>
        /// </summary>
        public string AmountType { get; set; }
        /// <summary>
        /// 禮券面額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 禮券張數 (每次最多10000為限)
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 單張實際銷售金額
        /// </summary>
        public int PayAmount { get; set; }
        /// <summary>
        /// 交易來源
        /// </summary>
        public string SourceFrom { get; set; }
        /// <summary>
        /// 卡片類別
        /// <para>1消費者購買</para>
        /// </summary>
        public string CardType { get; set; }
    }
}
