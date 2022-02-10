namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetDefPayMode
    {
        /// <summary>
        /// 支付方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// </summary>
        public int PayMode { get; set; }
    }
}