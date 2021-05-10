using Domain.WebAPI.output.rootAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 取得專案及資費(資費試算)輸出
    /// </summary>
    public class OAPI_GetProject
    {
        /// <summary>
        /// 是否有可租的卡片
        /// </summary>
        public bool HasRentCard { get; set; } = false;
        /// <summary>
        /// 查詢最大值
        /// </summary>
        //public int PriceMin { get; set; }
        /// <summary>
        ///查詢最小值 
        /// </summary>
        //public int PriceMax { get; set; }
        /// <summary>
        /// 車款下拉
        /// </summary>
        //public List<GetProject_SeatGroup> SeatGroups { get; set; }

        /// <summary>
        /// 目前可使用訂閱制月租
        /// </summary>
        public List<OAPI_NowSubsCard> NowSubsCards { get; set; }

        public List<GetProjectObj> GetProjectObj { set; get; }
    }
}