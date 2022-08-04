using System.Collections.Generic;

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
        public string RtnCode { set; get; }
        public WebAPIOutput_EnterpriseListData Data { set; get; }
    }
    public class WebAPIOutput_EnterpriseListData
    {
        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CUSTNM { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
        public List<WebAPIOutput_EnterpriseDept> depList { set; get; }

    }

    public class WebAPIOutput_EnterpriseDept
    {
        /// <summary>
        /// 部門代碼
        /// </summary>
        public int DeptNo { set; get; }
        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { set; get; }
    }

    public class WebAPIOutput_CheckoutOption
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        public string Message { set; get; }
        public WebAPIOutput_CheckoutOptionData[] Data { set; get; }
    }

    public class WebAPIOutput_CheckoutOptionData
    {
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; } = "";
        /// <summary>
        /// Etag
        /// </summary>
        public string Etag { set; get; } = "N";
        /// <summary>
        /// 安心服務
        /// </summary>
        public string SafeServ { set; get; } = "N";
        /// <summary>
        /// 停車費
        /// </summary>
        public string Parking { set; get; } = "N";
        /// <summary>
        /// 啟用日期
        /// </summary>
        public string EnableDate { set; get; } = "N";
    }

    public class WebAPIOutput_EnterpriseDelete
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 回傳代碼
        /// </summary>
        public string RtnCode { get; set; }

        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { get; set; }
    }
}