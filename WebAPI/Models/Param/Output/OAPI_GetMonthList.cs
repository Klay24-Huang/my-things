using System.Collections.Generic;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMonthList
    {
        /// <summary>
        /// 模式(1:月租，2我的所有方案)
        /// </summary>
        public int ReMode { get; set; } = 0;
    }

    public class OAPI_AllMonthList_Car: OAPI_GetMonthList
    {
        /// <summary>
        /// 是否為機車
        /// </summary>
        public int IsMotor { get; set; }
        /// <summary>
        /// 汽車牌卡
        /// </summary>
        public List<MonCardParam> NorMonCards { get; set; }
        /// <summary>
        /// 城市車手牌卡
        /// </summary>
        public List<MonCardParam> MixMonCards { get; set; }
    }

    public class OAPI_AllMonthList_Moto : OAPI_GetMonthList
    {
        /// <summary>
        /// 是否為機車
        /// </summary>
        public int IsMotor { get; set; }
        /// <summary>
        /// 機車牌卡
        /// </summary>
        public List<MonCardParam> NorMonCards { get; set; }
    }

    public class OAPI_MyMonthList: OAPI_GetMonthList
    {
        /// <summary>
        /// 汽車牌卡
        /// </summary>
        public MonCardParam_My MyCar { get; set; }
        /// <summary>
        /// 機車牌卡
        /// </summary>
        public MonCardParam_My MyMoto { get; set; }
    } 
}