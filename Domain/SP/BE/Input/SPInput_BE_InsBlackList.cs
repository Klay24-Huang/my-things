using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsBlackList
    {
        public int Mode { set; get; }
        public string Mobile { set; get; }
        public string USERID { set; get; }
        public string MEMO { set; get; }
    }
}
