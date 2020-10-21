using System;

namespace Domain.MemberData
{
    public class RegisterData
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string MEMPWD { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME { set; get; }
        /// <summary>
        /// 電話
        /// </summary>
        public string MEMTEL { set; get; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime MEMBIRTH { set; get; }

        /// <summary>
        /// 城市
        /// </summary>
        public int MEMAREAID { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string MEMADDR { set; get; }
        /// <summary>
        /// 信箱
        /// </summary>
        public string MEMEMAIL { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CARDNO { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { set; get; }
        /// <summary>
        /// 發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯
        /// </summary>
        public Int16 MEMSENDCD { set; get; }
        /// <summary>
        /// 發票載具
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 是否通過手機驗證(0:否;1:是)
        /// </summary>
        public Int16 HasCheckMobile { set; get; }
        /// <summary>
        /// 是否需重新設定密碼(0:否;1:是)
        /// </summary>
        public Int16 NeedChangePWD { set; get; }
        /// <summary>
        /// 是否綁定社群(0:否;1:是)
        /// </summary>
        public Int16 HasBindSocial { set; get; }
        /// <summary>
        /// 是否已驗證EMAIL(0:否;1:是)
        /// </summary>
        public Int16 HasVaildEMail { set; get; }
        /// <summary>
        /// 審核狀態
        /// <para>0:未審核</para>
        /// <para>1:審核通過</para>
        /// <para>2:審核不通過</para>
        /// </summary>
        public int Audit { set; get; }
        /// <summary>
        /// 目前註冊進行至哪個步驟
        /// </summary>
        public int IrFlag { set; get; }
        /// <summary>
        /// 付費方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// <para>2:line pay</para>
        /// <para>3:街口支付</para>
        /// </summary>
        public Int16 PayMode { set; get; }
        /// <summary>
        /// 可租類型
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// <para>2:汽機車</para>
        /// </summary>
        public Int16 RentType { set; get; }

        /// <summary>
        /// 身份證
        /// </summary>
        public int ID_pic { get; set; }

        /// <summary>
        /// 汽車駕照
        /// </summary>
        public int DD_pic { get; set; }

        /// <summary>
        /// 機車駕照
        /// </summary>
        public int MOTOR_pic { get; set; }

        /// <summary>
        /// 自拍照
        /// </summary>
        public int AA_pic { get; set; }

        /// <summary>
        /// 法定代理人
        /// </summary>
        public int F01_pic { get; set; }
        /// <summary>
        /// 電子簽名
        /// </summary>
        public int Signture_pic { get; set; }
        /// <summary>
        /// 電子簽名BASE64編碼
        /// </summary>
        public string SigntureCode { get; set; }
    }
}