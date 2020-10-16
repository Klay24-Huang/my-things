using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    /// <summary>
    /// 匯入機車資料
    /// </summary>
    public class SPInput_BE_ImportMotoData : SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 短租流水號
        /// </summary>
        public int TSEQNO { set; get; }
        /// <summary>
        /// 車型代碼，需左補0到六碼
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 座椅數
        /// </summary>
        public Int16 Seat { set; get; }
        /// <summary>
        /// 出廠年月
        /// </summary>
        public string FactoryYear { set; get; }
        /// <summary>
        /// 車色
        /// </summary>
        public string CarColor { set; get; }
        /// <summary>
        /// 引擎編號
        /// </summary>
        public string EngineNO { set; get; }
        /// <summary>
        /// 車身編號
        /// </summary>
        public string BodyNO { set; get; }
        /// <summary>
        /// CC數
        /// </summary>
        public int CCNum { set; get; }
        /// <summary>
        /// 是否是機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; } = 1;
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
