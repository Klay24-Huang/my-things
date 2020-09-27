using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class MotorPowerInfoBase
    {
        /// <summary>
        /// 剩餘電量
        /// </summary>
        public float Power { set; get; }
        /// <summary>
        /// 剩餘里程
        /// </summary>
        public float RemainingMileage { set; get; }
    }
}
