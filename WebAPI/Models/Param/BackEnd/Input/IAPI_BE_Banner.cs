using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_Banner
    {
        public int StationType { get; set; }
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }
        public string fileName1 { get; set; }
        public string fileData1 { get; set; }
        public string URL { get; set; }
        public string RunHorse { get; set; }
        public string UserID { get; set; }
        public string SEQNO { get; set; }
    }
}