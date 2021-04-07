namespace Domain.SP.Output.Common
{
    public class SPOutput_CheckAppVersion : SPOutput_Base
    {
        /// <summary>
        /// 強制更新 1=強更，0=否
        /// </summary>
        public int MandatoryUPD { get; set; }
    }
}