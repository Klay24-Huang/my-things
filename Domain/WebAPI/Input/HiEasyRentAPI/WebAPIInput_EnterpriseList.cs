using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_EnterpriseList
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
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
    }
    public class WebAPIInput_CheckoutOption
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
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
        /// <summary>
        /// 查詢基準日
        /// </summary>
        public string QryDate { set; get; }
    }

    public class WebAPIInput_EnterpriseDelete
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { get; set; }
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }
    }
}