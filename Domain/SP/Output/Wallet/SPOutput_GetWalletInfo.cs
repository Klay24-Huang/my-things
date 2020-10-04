using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOutput_GetWalletInfo:SPOutput_Base
    {
        /// <summary>
        /// 會員id
        /// </summary>
        public string WalletMemberID { set; get; }
        /// <summary>
        /// 虛擬帳號id
        /// </summary>
        public string WalletAccountID { set; get; }
        /// <summary>
        /// 開戶email
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 開戶電話
        /// </summary>
        public string PhoneNo { set; get; }
        public string Name { set; get; }
    }
}
