using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR138Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }
        /// <summary>
        /// 預約編號
        /// </summary>
        public string ORDNO { set; get; }
        /// <summary>
        /// IRENT訂單編號
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string INVKIND { set; get; }
        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string INVTITLE { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { set; get; }
        /// <summary>
        /// 載具條碼
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CARDNO { set; get; }
        /// <summary>
        /// 網刷訂單編號
        /// </summary>
        public string NORDNO { set; get; }

    }
}
