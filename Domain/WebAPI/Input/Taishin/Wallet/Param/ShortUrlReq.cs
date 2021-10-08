using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet.Param
{

    /// <summary>
    /// 短網址查詢請求物件
    /// </summary>
    public class ShortUrlReq
    {

        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string paymentId { get; set; }

    }
}