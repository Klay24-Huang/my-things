using Domain.TB;
using System.Collections.Generic;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetBanner
    {
        /// <summary>
        /// 廣告資訊
        /// </summary>
        public List<BannerData> BannerObj { get; set; }
    }
}