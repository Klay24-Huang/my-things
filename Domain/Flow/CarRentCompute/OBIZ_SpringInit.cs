using Domain.TB;
using System.Collections.Generic;

namespace Domain.Flow.CarRentCompute
{
    public class OBIZ_SpringInit : IBIZ_SpringInit
    {
        /// <summary>
        /// 虛擬月租
        /// </summary>
        public List<MonthlyRentData> VisMons { get; set; } = new List<MonthlyRentData>();
    }
}