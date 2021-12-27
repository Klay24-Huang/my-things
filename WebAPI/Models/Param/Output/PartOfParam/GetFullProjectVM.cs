namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class GetFullProjectVM
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID { get; set; }
        /// <summary>
        /// 專案類型
        /// </summary>
        public int PROJTYPE { get; set; }
        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarTypeGroupCode { get; set; }
        /// <summary>
        /// 平日價格
        /// </summary>
        public double PRICE { get; set; }
        /// <summary>
        /// 假日價格
        /// </summary>
        public double PRICE_H { get; set; }
    }
}