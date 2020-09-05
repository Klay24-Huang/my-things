using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetFavoriteStation
    {
        /// <summary>
        /// 常用站點列表
        /// </summary>
        public List<iRentStationData> FavoriteObj { set; get; }
    }
}