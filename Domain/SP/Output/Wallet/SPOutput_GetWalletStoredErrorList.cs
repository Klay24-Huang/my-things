﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{

    /// <summary>
    /// 錢包儲值錯誤清單
    /// </summary>
    public class SPOutput_GetWalletStoredErrorList
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SEQNO { get; set; } 

        /// <summary>
        /// 會員身份證
        /// </summary>
        public string ID { get; set; } = "";

        /// <summary>
        /// IDNO是否為舊式居留證(0:否 1:是)
        /// </summary>
        public int IsForeign { get; set; } = 0;

        /// <summary>
        /// 商店會員編號，最長20碼
        /// </summary>
        public string MemberId { get; set; } = "";

        /// <summary>
        /// 會員姓名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 會員電話
        /// </summary>
        public string PhoneNo { get; set; } = "";

        /// <summary>
        /// 會員email
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 帳戶類別，預設帶2
        /// <para>1:個人一類</para>
        /// <para>2:個人二類(預設)</para>
        /// <para>3:法人一類</para>
        /// <para>4:法人二類</para>
        /// <para>5:法人三類</para>
        /// </summary>
        public string AccountType { get; set; } = "2";
        /// <summary>
        /// 會員虛擬帳號建立來源
        ///<para>1=使用商店會員編號</para>
        ///<para>2=由系統產生</para>
        /// </summary>
        public string CreateType { get; set; } = "";

        /// <summary>
        /// 金額類別
        /// <para>1:現金</para>
        /// <para>2:信用卡</para>
        /// <para>3:收款金額</para>
        /// </summary>
        public string AmountType { get; set; } = "";

        /// <summary>
        /// 交易金額
        /// </summary>
        public int Amount { get; set; } = 0;

        /// <summary>
        /// 紅利點數，預設代0
        /// </summary>
        public int Bonus { get; set; } = 0;

        /// <summary>
        /// 紅利到期日，預設帶空值
        /// </summary>
        public string BonusExpiredate { get; set; } = "";

        /// <summary>
        /// 交易來源
        /// <para>1:POS</para>
        /// <para>2:APP</para>
        /// <para>3:WEB</para>
        /// <para>4:其他</para>
        /// <para>5:ATM虛擬帳號</para>
        /// <para>6:銀行帳號存入</para>
        /// <para>7:活動贈送</para>
        /// <para>8:商品預售下架轉存</para>
        /// <para>9:線上刷卡儲值</para>
        /// <para>A:銀行紅利點數轉存</para>
        /// <para>B:實體禮物卡轉存</para>
        /// <para>C:中獎發票轉存</para>
        /// <para>Z:線下退款</para>
        /// </summary>
        public string SourceFrom { get; set; } = "9";


        /// <summary>
        /// 覆保起日，交易來源為實體禮物卡轉存時，此欄位才需帶值
        /// </summary>
        public string StoreValueReleaseDate { get; set; } = "";

        /// <summary>
        /// 禮物卡條碼，預設帶空值
        /// </summary>
        public string GiftCardBarCode { get; set; } = "";


        /// <summary>
        /// 回傳代碼
        /// </summary>
        public string ReturnCode { get; set; } = "";

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 回傳異常錯誤訊息
        /// </summary>
        public string ExceptionData { get; set; } = "";

        /// <summary>
        /// 交易類別名稱 對應(TB_WalletCodeTable:CodeGroup)
        /// </summary>
        public string TradeType { get; set; } = "";

        /// <summary>
        /// 特店訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; } = "";

        /// <summary>
        /// 銀行交易編號
        /// </summary>
        public string BankTradeNo { get; set; } = "";

        /// <summary>
        /// 信用卡號
        /// </summary>
        public string CardNumber { get; set; } = "";
    }
}