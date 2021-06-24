namespace Domain.SP.Input.Car
{
    public class SPInput_GetMotorRent : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 是否顯示全部
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int ShowALL { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 半徑
        /// </summary>
        public double Radius { get; set; }
    }
}