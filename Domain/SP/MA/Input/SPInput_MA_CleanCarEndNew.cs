using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.MA.Input
{
    public class SPInput_MA_CleanCarEndNew:SPInput_Base
    {
        public string UserID { set; get; }
        public string CarNo { set; get; }
        public Int64 OrderNum { set; get; }
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
        public string remark { set; get; }
        /// <summary>
        /// 車內照
        /// </summary>
        public string incarPic { set; get; }
        /// <summary>
        /// 車外照
        /// </summary>
        public string outcarPic { set; get; }
        /// <summary>
        /// 車內照，base64圖片類型
        /// </summary>
        public string incarPicType { set; get; }
        /// <summary>
        /// 車外照，base64圖片類型
        /// </summary>
        public string outcarPicType { set; get; }
        public int isCar { set; get; }
    }
}
