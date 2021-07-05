using Domain.SP.Output.Member;
using System.Collections.Generic;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMapMedal
    {
        /// <summary>
        /// 徽章清單
        /// </summary>
        public List<MapMedalList> MedalList { get; set; }
    }
}