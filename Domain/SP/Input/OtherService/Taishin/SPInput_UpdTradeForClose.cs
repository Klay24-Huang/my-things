using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_UpdTradeForClose: SPInput_UpdTrade
    {
        /// <summary>
        /// 可否關帳 0:可關帳、1:不可關帳
        /// </summary>
        public int ChkClose { get; set; }
        /// <summary>
        /// 信用卡類別 
        /// </summary>
        public int CardType { get; set; }

        public string ProName { get; set; }

        public string UserID { get; set; }

    }
}
