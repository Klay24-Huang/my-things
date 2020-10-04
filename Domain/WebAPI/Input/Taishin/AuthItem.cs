using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    /// <summary>
    /// 商品明細
    /// </summary>
   public  class AuthItem
    {
        /// <summary>
        /// 總金額包含兩位小數，如100代表1.00元
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 品項
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品是否不可累積點數
        /// </summary>
        public string NonPoint { get; set; } = "N";
        /// <summary>
        /// 商品是否不可折抵
        /// </summary>
        public string NonRedeem { get; set; } = "Y";
        /// <summary>
        /// 單價包含兩位小數，如100代表1.00元
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public string Quantity { get; set; } = "1";
   

     

    }
}
