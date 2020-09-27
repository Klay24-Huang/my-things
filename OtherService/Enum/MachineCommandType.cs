using System;
namespace OtherService.Enum
{
    public class MachineCommandType
    {
        /// <summary>
        /// 遠傳車機COMMAND
        /// </summary>
        public MachineCommandType()
        {

        }
        public enum CommandType
        {
            #region 汽車及共用開始
            /// <summary>
            /// 汽車專用尋車
            /// </summary>
            SearchVehicle,
            /// <summary>
            /// 查詢萬用卡的卡號
            /// </summary>
            QueryUnivCardNo,
            /// <summary>
            /// 設定萬用卡的卡號
            /// </summary>
            SetUnivCardNo,
            /// <summary>
            /// 中控解鎖，防盜關閉
            /// </summary>
            Unlock_AlertOff,
            /// <summary>
            /// 中控上鎖，防盜開啟
            /// </summary>
            Lock_AlertOn,
            /// <summary>
            /// 要求車機立即回傳資料（both)
            /// </summary>
            ReportNow,
            /// <summary>
            /// 設定租約狀態（汽車)
            /// </summary>
            SetVehicleRent,
            /// <summary>
            /// 設定無租約(both)
            /// </summary>
            SetNoRent,
            /// <summary>
            /// 中控解鎖
            /// </summary>
            Unlock,
            /// <summary>
            /// 中控上鎖
            /// </summary>
            Lock,
            /// <summary>
            /// 防盜關閉
            /// </summary>
            AlertOff,
            /// <summary>
            /// 防盜開啟
            /// </summary>
            AlertOn,
            /// <summary>
            /// 查詢顧客卡
            /// </summary>
            QueryClientCardNo,
            /// <summary>
            /// 設定顧客卡
            /// </summary>
            SetClientCardNo,
            /// <summary>
            /// 清空顧客卡
            /// </summary>
            ClearAllClientCard,
            /// <summary>
            /// 清空萬用卡
            /// </summary>
            ClearAllUnivCard,
            #endregion
            #region 機車開始
            /// <summary>
            /// 設定租約狀態(機車)，需提供ble_code供驗證
            /// </summary>
            SetMotorcycleRent,
            /// <summary>
            /// 開啟電源
            /// </summary>
            SwitchPowerOn,
            /// <summary>
            /// 關閉電源
            /// </summary>
            SwitchPowerOff,
            /// <summary>
            /// 啟動喇叭尋車功能
            /// </summary>
            SetHornOn,
            /// <summary>
            /// 啟動閃燈尋車功能
            /// </summary>
            SetLightFlash,
            /// <summary>
            /// 開啟坐墊
            /// </summary>
            OpenSet,
            /// <summary>
            /// 開啟/關閉電池蓋
            /// </summary>
            SetBatteryCap,
            
            #endregion
        }

        public string GetCommandName(MachineCommandType.CommandType type)
        {
            string CommandName = type.ToString();
        /*    switch (type)
            {
                case CommandType.SearchVehicle:  //汽車尋車
                    CommandName = "SearchVehicle";
                    break;
                case CommandType.QueryUnivCardNo:
                    CommandName = "QueryUnivCardNo";
                    break;
                   

            }*/
            return CommandName;
        }
    }
}
