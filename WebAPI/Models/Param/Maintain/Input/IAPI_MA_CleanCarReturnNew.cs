using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Input
{
    public class IAPI_MA_CleanCarReturnNew
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNum { set; get; }
        /// <summary>
        /// 是否為汽車
        /// <para>0:機車</para>
        /// <para>1:汽車</para>
        /// </summary>
        public int IsCar { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string MachineNo { set; get; }
        /// <summary>
        /// 使用者id
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車外清潔(0:否;1:是)
        /// </summary>
        public Int16 outsideClean { set; get; }
        /// <summary>
        /// 內裝清潔0:否;1:是)
        /// </summary>
        public Int16 insideClean { set; get; }
        /// <summary>
        /// 車輛救援0:否;1:是)
        /// </summary>
        public Int16 rescue { set; get; }
        /// <summary>
        /// 非路邊租還調度0:否;1:是)
        /// </summary>
        public Int16 dispatch { set; get; }
        /// <summary>
        /// 路邊租還調度0:否;1:是)
        /// </summary>
        public Int16 Anydispatch { set; get; }
        /// <summary>
        /// 保養0:否;1:是
        /// </summary>
        public Int16 Maintenance { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string remark { set; get; }
        /// <summary>
        /// 車內照，記得要把⊙ replace
        /// </summary>
        public string incarPic { set; get; }
        /// <summary>
        /// 車內照，base64圖片類型
        /// </summary>
        public string incarPicType { set; get; }
        /// <summary>
        /// 車外照，記得要把⊙ replace
        /// </summary>
        public string outcarPic { set; get; }
        /// <summary>
        /// 車外照，base64圖片類型
        /// </summary>
        public string outcarPicType { set; get; }
    }
}