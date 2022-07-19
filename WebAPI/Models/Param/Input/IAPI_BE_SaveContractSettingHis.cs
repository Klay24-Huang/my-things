using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BE_SaveContractSettingHis
    {
        public string UserID { get; set; }
        public Int64 OrderNo { get; set; }
        public int type { get; set; }
        public string returnDate { get; set; }
        public int mode { get; set; }
        public string mode_input { get; set; }
        public int Discount { get; set; }
        public int timeDiscount_car { get; set; }
        public int timeDiscount_motor { get; set; }
        public int score { get; set; }
        public int score_input { get; set; }
        public int costRelife_cost { get; set; }
        public int costRelife_minute { get; set; }
        public string costRelife_input { get; set; }
        public bool notUseCar { get; set; }
        public bool cancelOvertime { get; set; }
        public string bill_option { get; set; }
        public string unified_business_no { get; set; }
        public string parkingSpace { get; set; }
    }
}