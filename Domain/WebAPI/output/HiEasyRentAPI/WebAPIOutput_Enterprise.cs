using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 點數查詢
    /// </summary>
    public class WebAPIOutput_EnterpriseList
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        public string Message { set; get; }
        public WebAPIOutput_EnterpriseListData[] Data { set; get; }
    }

    public class WebAPIOutput_EnterpriseListData {
        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CUSTNM { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
        /// <summary>
        /// 部門代碼
        /// </summary>
        public int DeptNo { set; get; }
        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { set; get; }

    }
}
