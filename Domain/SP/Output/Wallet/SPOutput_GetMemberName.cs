namespace Domain.SP.Output.Wallet
{
    public class SPOutput_GetMemberName : SPOutput_Base
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 電話
        /// </summary>
        public string PhoneNo { get; set; }
    }
}