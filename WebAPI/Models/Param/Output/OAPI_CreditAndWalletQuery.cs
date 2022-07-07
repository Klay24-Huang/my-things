using Domain.TB.Hotai;
using System.Collections.Generic;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 查詢綁卡及錢包
    /// </summary>
    public class OAPI_CreditAndWalletQuery
    {
        /// <summary>
        /// 付費方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// </summary>
        public int PayMode { get; set; }

        /// <summary>
        /// 是否有綁定
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int HasBind { set; get; } = 0;

        /// <summary>
        /// 是否有錢包
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int HasWallet { set; get; } = 0;

        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int TotalAmount { set; get; }

        /// <summary>
        /// 信用卡列表
        /// </summary>
        public List<CreditCardBindList> BindListObj { set; get; }

        /// <summary>
        /// 發票寄送方式
        /// <para>1:捐贈</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int MEMSENDCD { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { get; set; }

        /// <summary>
        /// 手機條碼
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 是否同意自動儲值
        /// <para>0:不同意</para>
        /// <para>1:同意</para>
        /// </summary>
        public int AutoStored { get; set; }

        /// <summary>
        /// 是否有和泰PAY
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int HasHotaiPay { get; set; }

        /// <summary>
        /// 和泰PAY卡清單
        /// </summary>
        public List<HotaiCardInfo> HotaiListObj { get; set; }

        /// <summary>
        /// 機車預扣款金額
        /// </summary>
        public int MotorPreAmt { get; set; }

        /// <summary>
        /// 企業會員統一編號
        /// </summary>
        public string TaxID { get; set; }
        /// <summary>
        /// 機車預扣款金額
        /// </summary>
        public int EnterpriseStatus { get; set; }
        /// <summary>
        /// 企業會員部門名稱
        /// </summary>
        public string EnterpriseDeptCN { get; set; }
        /// <summary>
        /// 企業會員公司名稱
        /// </summary>
        public string EnterpriseCmpCN { get; set; }
        /// <summary>
        /// 企業會員員工編號
        /// </summary>
        public string EmployeeID { get; set; }
    }
}