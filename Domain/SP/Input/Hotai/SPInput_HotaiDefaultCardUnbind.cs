
namespace Domain.SP.Input.Hotai
{
    public class SPInput_HotaiDefaultCardUnbind : SPInput_Base
    {
        public string IDNO { get; set; }
        
        public int HotaiCardID { get; set; }
        /// <summary>
        /// 更新程式
        /// </summary>
        public string U_FuncName { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string U_USERID { get; set; }

    }
}
