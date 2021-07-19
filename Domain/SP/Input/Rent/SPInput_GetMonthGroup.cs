using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_GetMonthGroup
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string MonProjID { get; set; }
        public DateTime? SetNow { get; set; } = null;
    }
}
