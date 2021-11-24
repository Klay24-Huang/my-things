using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    /// <summary>
    /// 綁定和泰Pay預設卡
    /// </summary>
    public class SPInput_SetDefaultCard
    {
        public string IDNO { get; set; }

        public string OneID { get; set; }

        /// <summary>
        /// 信用卡密鑰
        /// </summary>
        public string CardToken { get; set; }

        /// <summary>
        /// 隱碼卡號
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 發卡機構
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 發卡銀行
        /// </summary>
        public string BankDesc { get; set; }
        
        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }
    }
}
