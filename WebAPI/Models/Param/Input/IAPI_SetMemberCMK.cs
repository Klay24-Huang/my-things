namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetMemberCMK
    {
        /// <summary>
        /// 是否同意
        /// <para>Y：同意</para>
        /// <para>N：不同意</para>
        /// </summary>
        public string CHKStatus { get; set; }

        /// <summary>
        /// 流水號 (對應TB_CMKDEF)
        /// </summary>
        public int SeqNo { get; set; } = 0;
    }
}