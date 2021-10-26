using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_UpdTradeForClose: SPInput_UpdTrade
    {
        public int AutoClose { get; set; }
        /// <summary>
        /// 信用卡類別 0:和泰、1:台新
        /// </summary>
        public int CardType { get; set; }

        public string ProName { get; set; }

        public string UserID { get; set; }

    }
}
