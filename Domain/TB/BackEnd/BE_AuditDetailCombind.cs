using Domain.WebAPI.output.HiEasyRentAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 審核明細組合
    /// </summary>
    public class BE_AuditDetailCombind
    {
        /// <summary>
        /// 審核資料
        /// </summary>
        public BE_AuditDetail detail { set; get; }
        /// <summary>
        /// 證件照
        /// </summary>
        public BE_AuditCrentials Images { set; get; }
        /// <summary>
        /// 黑名單資料
        /// </summary>
        public WebAPIOutput_NPR172QueryData block { set; get; }
        public List<BE_SameMobileData> SameMobile { set; get; }

        public List<BE_AuditHistory> History { set; get; }
        public List<BE_MileStone> MileStone { set; get; }
        public List<BE_MileStoneDetail> MileStoneD { set; get; }

        public List<BE_AuditRecommendHistory> RecommendHistory { set; get; }
        public List<BE_InsuranceData> InsuranceData { set; get; }
        //20210310唐加
        public string mobileBlock { set; get; }
    }
    
}
