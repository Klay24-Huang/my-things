using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class WalletTransferData
    {
        /// <summary>
        /// 公司別
        /// </summary>
        public string ORGNO { get; set; }

        /// <summary>
        /// 會員錢包ID
        /// </summary>
        public string CONTNO { get; set; }

        /// <summary>
        /// 作業代號
        /// HS:電子錢包收款(加值)
        /// HT:電子錢包轉贈
        /// HU:電子錢包付款(扣錢)
        /// HW:電子錢包付款取消(扣錢取消)
        /// </summary>
        public string FMFLAG { get; set; }

        /// <summary>
        ///電子錢包的收款類別
        ///1.網路刷卡iRent : X0000033
        ///2.虛擬帳號轉帳 : X0000041
        ///3.超商繳款  : X0000042
        ///4.錢包轉贈 : X0000001
        ///5.合約退款:X0000002
        /// </summary>
        public string CUSTID { get; set; }

        /// <summary>
        /// 台新交易編號
        /// </summary>
        public string NORDNO { get; set; }

        /// <summary>
        /// 轉贈者會員編號
        /// HT:電子錢包轉贈 填[轉贈者會員編號]
        /// HU或HV填使用[合約] or[罰單]or[etag] 
        /// </summary>
        public string TRANS_CONTNO { get; set; }

        /// <summary>
        /// 收支代號
        /// 01: 收款
        /// 02: 收款取消 
        /// 03: 付款
        /// 04: 付款取消
        /// 05: 轉贈
        /// 06: 提領
        /// </summary>
        public string CEFLAG { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string ORDER_NO { get; set; }

        /// <summary>
        /// 作業日期
        /// YYYYMMDD:轉進系統的作業日
        /// </summary>
        public string FOMDATE { get; set; } = string.Format("{0:yyyyMMdd}", DateTime.Now);

        /// <summary>
        /// 傳票日期
        /// YYYYMMDD:收款日
        /// </summary>
        public string BOHDATE { get; set; }

        /// <summary>
        /// 總金額
        /// </summary>
        public decimal TOTAMT { get; set; }

        /// <summary>
        /// 是否要開手續費
        /// 提領時是否要開手續費
        /// 預設 N 要開手續費為 Y
        /// </summary>
        public string FEECHK { get; set; }

        /// <summary>
        /// 銷售金額(發票不含稅)
        /// </summary>
        public int SALAMT { get; set; }

        /// <summary>
        /// 營業稅額(發票稅額)
        /// </summary>
        public int TAXAMT { get; set; }

        /// <summary>
        /// 手續費總額(發票總額)
        /// </summary>
        public int FEEAMT { get; set; }

        /// <summary>
        /// 客戶統編
        /// </summary>
        public string INV_CUSTID { get; set; }

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string INV_CUSTNM { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string INV_ADDR { get; set; }

        /// <summary>
        /// 載具號碼
        /// </summary>
        public string INVCARRIER { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 發票隨機碼
        /// </summary>
        public string RNDCODE { get; set; }

        /// <summary>
        /// 匯款銀行代號
        /// </summary>
        public string RVBANK { get; set; }

        /// <summary>
        /// 匯款銀行帳號
        /// </summary>
        public string RVACNT { get; set; }

        /// <summary>
        /// 匯款戶名
        /// </summary>
        public string RV_NAME { get; set; }

        /// <summary>
        /// 傳票備註
        /// </summary>
        public string REMARK { get; set; }

        /// <summary>
        /// 介面批號 
        /// 同一傳票日期(收款日),一個批號
        /// </summary>
        public string INTFNO { get; set; }
    }
}
