using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_UserWalletInfo
    {
        public string IDNO { get; set; }
        public string Status { get; set; }
        public int StoreAmount { get; set; }
        public int ReceiveAmount { get; set; }
        public int WalletBalance { get; set; }
    }
}
