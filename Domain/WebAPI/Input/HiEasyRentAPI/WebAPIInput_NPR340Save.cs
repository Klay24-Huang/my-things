using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR340Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }
        public List<NPR340SaveServiceVar> tbNPR340SaveServiceVar { set; get; }
        public List<NPR340PaymentDetail> tbNPR340PaymentDetail { set; get; }

    }
    public partial class NPR340SaveServiceVar
    {
        /// <summary>
        /// 會員id
        /// </summary>
        public string CUSTID { get; set; }
        /// <summary>
        /// 短租訂單編號
        /// </summary>
        public string ORDNO { get; set; }
        /// <summary>
        /// 短租合約號
        /// </summary>
        public string CNTRNO { get; set; }
        /// <summary>
        /// 類別
        /// <para>1:租金</para>
        /// <para>2:罰單</para>
        /// <para>3:停車費</para>
        /// <para>4:ETAG</para>
        /// <para>5:營損/代收停車費</para>
        /// </summary>
        public long PAYMENTTYPE { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { get; set; }
        /// <summary>
        /// 網路刷卡訂單編號
        /// </summary>
        public string NORDNO { set; get; }
        /// <summary>
        /// 刷卡日，格式應該是yyyyMMdd
        /// </summary>
        public string PAYDATE { set; get; }
        /// <summary>
        /// 刷卡授權碼
        /// </summary>
        public string AUTH_CODE { set; get; }
        /// <summary>
        /// 沖銷金額
        /// </summary>
        public string AMOUNT { set; get; }
        /// <summary>
        /// 刷卡人
        /// </summary>
        public string CDTMAN { set; get; }
        /// <summary>
        /// 信用卡號
        /// </summary>
        public string CARDNO { set; get; }
        /// <summary>
        /// 罰單編號
        /// </summary>
        public string POLNO { get; set; }
        /// <summary>
        /// 0: 台新 1:中信 20220206
        /// </summary>
        public int OPERATOR { get; set; }
    }
    public class NPR340PaymentDetail
    {
        /// <summary>
        /// 合約編號
        /// </summary>
        public string CNTRNO { set; get; }
        /// <summary>
        /// 欠費類型 1.租金 2.罰單 3.停車費 4. ETAG、5.營損 / 代收停車費
        /// </summary>
        public string PAYMENTTYPE { set; get; }
        /// <summary>
        /// 支付方式
        /// <para>1:信用卡            </para>
        /// <para>2:電子錢包-信用卡   </para>
        /// <para>3:電子錢包-虛擬帳戶 </para>
        /// <para>4:電子錢包-超商     </para>
        /// </summary>
        public string PAYTCD { set; get; }
        /// <summary>
        /// 支付金額
        /// </summary>
        public string PAYAMT { set; get; }
        /// <summary>
        /// 支付訂單編號
        /// </summary>
        public string PORDNO { set; get; }
        /// <summary>
        /// 支付說明(ex:租金)
        /// </summary>
        public string PAYMEMO { set; get; }
        /// <summary>
        /// 0: 台新 1:中信 20220206
        /// </summary>
        //public int OPERATOR { get; set; }
    }
}
