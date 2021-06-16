using Domain.WebAPI.output.rootAPI;
using System.Collections.Generic;

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
        /// 專案列表
        /// </summary>
        public List<GetProjectObj> GetProjectObj { set; get; }
    }
}