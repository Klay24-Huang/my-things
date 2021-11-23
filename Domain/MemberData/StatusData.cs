namespace Domain.MemberData
{
    public class StatusData
    {
        /// <summary>
        /// 身分證號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMNAME { set; get; }
        /// <summary>
        /// 登入狀態 Y/N
        /// </summary>
        public string Login { set; get; }
        /// <summary>
        /// 註冊是否完成 0:未完成 1:已完成
        /// </summary>
        public int Register { set; get; }
        /// <summary>
        /// 審核結果 是否通過審核(0:未審;1:已審;2:審核不通過)
        /// </summary>
        public int Audit { set; get; }
        /// <summary>
        /// 審核身分證 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_ID { set; get; }
        /// <summary>
        /// 審核汽車駕照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_Car { set; get; }
        /// <summary>
        /// 審核機車駕照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_Motor { set; get; }
        /// <summary>
        /// 審核自拍照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_Selfie { set; get; }
        /// <summary>
        /// 審核法定代理人 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_F01 { set; get; }
        /// <summary>
        /// 審核簽名檔 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)
        /// </summary>
        public int Audit_Signture { set; get; }
        /// <summary>
        /// 黑名單 Y/N
        /// </summary>
        public string BlackList { set; get; }
        /// <summary>
        /// 會員頁9.0卡狀態 (0:PASS 1:未完成註冊 2:完成註冊未上傳照片 3:身分審核中 4:審核不通過 5:身分變更審核中 6:身分變更審核失敗)
        /// </summary>
        public int MenuCTRL { set; get; }
        /// <summary>
        /// 會員頁9.0狀態顯示 (這邊要通過審核才會有文字 MenuCTRL5 6才會有文字提示)
        /// </summary>
        public string MenuStatusText { set; get; }
        /// <summary>
        /// 狀態文字說明
        /// </summary>
        public string StatusTextCar { set; get; }
        /// <summary>
        /// 機車狀態文字說明
        /// </summary>
        public string StatusTextMotor { set; get; }
        /// <summary>
        /// 目前汽車出租數
        /// </summary>
        public int NormalRentCount { set; get; }
        /// <summary>
        /// 目前路邊出租數
        /// </summary>
        public int AnyRentCount { set; get; }
        /// <summary>
        /// 目前機車出租數
        /// </summary>
        public int MotorRentCount { set; get; }
        /// <summary>
        /// 目前全部出租數
        /// </summary>
        public int TotalRentCount { set; get; }

        /// <summary>
        /// 會員積分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 停權等級 (0:無 1:暫時停權 2:永久停權)
        /// </summary>
        public int BlockFlag { get; set; }

        /// <summary>
        /// 停權截止日
        /// </summary>
        public string BLOCK_EDATE { get; set; }

        /// <summary>
        /// 會員條款狀態
        /// <para>Y：重新確認</para>
        /// <para>N：不需重新確認</para>
        /// </summary>
        public string CMKStatus { get; set; }

        /// <summary>
        /// 是否顯示購買牌卡
        /// <para>Y:是</para>
        /// <para>N:否</para>
        /// </summary>
        public string IsShowBuy { get; set; }
        /// <summary>
        /// 是否有推播訊息 20210917 ADD BY ADAM
        /// </summary>
        public string HasNoticeMsg { get; set; }

        /// <summary>
        /// 預授權條款狀態
        /// <para>Y：重新確認</para>
        /// <para>N：不需重新確認</para>
        /// </summary>
        public string AuthStatus { get; set; }

        /// <summary>
        /// 和泰OneID綁定狀態
        /// <para>Y：綁定</para>
        /// <para>N：未綁</para>
        /// </summary>
        public string BindHotai { get; set; }
    }
}