using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR010Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string MEMCNAME { get; set; } = "";
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string MEMIDNO { get; set; } = "";
        public string MEMSEX { get; set; } = "";
        public string MEMPWD { get; set; } = "";
        public string MEMEMAIL { get; set; } = "";
        public string MEMBIRTH { get; set; } = "";
        public string MEMTEL { get; set; } = "";
        public string MEMTEL_EXT { get; set; } = "";
        public string MEMCEIL { get; set; } = "";
        public string MEMCOMTEL { get; set; } = "";
        public string MEMCOMTEL_EXT { get; set; } = "";
        public string MEMCONTRACT { get; set; } = "";
        public string MEMSELKEY_K { get; set; } = "0";
        public string MEMCONTEL { get; set; } = "";
        public string MEMEPAPER { get; set; } = "";
        public string MEMMSG { get; set; } = "";
        public string MEMTEL_ZIP { get; set; } = "";
        public string MEMCOMTEL_ZIP { get; set; } = "";
        public string MEMCITY { get; set; } = "";
        public string MEMPOST { get; set; } = "";
        public string MEMADDR { get; set; } = "";
        public string MEMCD { get; set; } = "";
        public string MEMPOST2 { get; set; } = "";
        public string MEMCITY2 { get; set; } = "";
        public string MEMCOUNTRY2 { get; set; } = "";
        public string MEMADDR2 { get; set; } = "";
        public string MEMVERCD { get; set; } = "";
        public string MEMDRIDNO { get; set; } = "";
        public string MEMCARDRCV { get; set; } = "";
        public string MEMCARDNO1 { get; set; } = "";
        public string MEMCARDNO2 { get; set; } = "";
        public string CALLCD { get; set; } = "2";
        public string MEMCARDCD { get; set; } = "";
        public string UNIMNO { get; set; } = "";
        public string MEMSENDCD { get; set; } = "";
        public string IRENTBRNHCD { get; set; } = "";
        public string SPCSTATUS { get; set; } = "";
        public string MARKETING { get; set; } = "";
        public string CARRIERID { get; set; } = "";
        public string NPOBAN { get; set; } = "";
        public string IRENTFLG { get; set; } = "";
        //20210219唐:寫死U是因為MEMBER_API不用寫入，只要寫入MEMBER_NEW即可 
        //20210730唐:搞錯了啦，是因為npr010不只我call，前端api也call，我後端只做更新所以傳u
        public string PROCD { get; set; } = "U"; 
        public string SIGNATURE { get; set; } = "";
        public string NOCFLG { get; set; } = "";
        public List<ExtSigninList> tbExtSigninList { get; set; }
    }

    public class ExtSigninList
    {
        public string SigninType { get; set; } = "";
        public string EXTID { get; set; } = "";
    }
}