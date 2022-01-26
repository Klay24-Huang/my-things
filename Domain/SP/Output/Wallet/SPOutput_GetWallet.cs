using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOutput_GetWallet : SPOutput_Base
    {
        /// <summary>
        /// 錢包會員ID
        /// </summary>
        public string WalletMemberID { set; get; }

        /// <summary>
        /// 錢包虛擬ID
        /// </summary>
        public string WalletAccountID { set; get; }

        /// <summary>
        /// 開戶時申請的mail
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 開戶時申請的電話
        /// </summary>
        public string PhoneNo { set; get; }

        /// <summary>
        /// 開戶時申請的姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 錢包餘額
        /// </summary>
        public int WalletBalance { set; get; }

        /// <summary>
        /// 當月交易金流
        /// </summary>
        public int MonthlyTransAmount { set; get; }
    }
}
