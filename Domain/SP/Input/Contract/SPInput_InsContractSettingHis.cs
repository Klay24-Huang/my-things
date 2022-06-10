using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Contract
{
    public class SPInput_InsContractSettingHis : SPInput_Base
    {
        public string A_USER { get; set; }
        public Int64 OrderNo { get; set; }
        public int Type { get; set; }
        public string ReturnTime { get; set; }
        public int Reason { get; set; }
        public string Memo { get; set; }
        public int DiscountType { get; set; }
        public int Discount_C { get; set; }
        public int Discount_M { get; set; }
        public int MemberScoreType { get; set; }
        public int MemberScore { get; set; }
        public int CostRelief_Cost { get; set; }
        public int CostRelief_Minute { get; set; }
        public string CostRelief_Memo { get; set; }
        public int NotUseCar { get; set; }
        public int CancelOvertime { get; set; }
        public string CarrierType { get; set; }
        public string Business_No { get; set; }
        public string Parking { get; set; }
    }
}
