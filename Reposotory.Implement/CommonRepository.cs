using Domain;
using Domain.TB;
using Domain.TB.BackEnd;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using WebCommon;

namespace Reposotory.Implement
{
    public class CommonRepository : ICommonRepository
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public CommonRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        private List<T> GetObjList<T>(ref bool flag, ref List<ErrorInfo> lstError, string SQL, SqlParameter[] para, string term)
            where T : class
        {
            List<T> lstObj;
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            using (SqlCommand command = new SqlCommand(SQL, conn))
            {
                if ("" != term)
                {
                    for (int i = 0; i < para.Length; i++)
                    {
                        if (null != para[i])
                        {
                            command.Parameters.Add(para[i]);
                        }

                    }
                }
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 180;
                lstObj = new List<T>();
                if (conn.State != ConnectionState.Open) conn.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //  MachineInfo item = new MachineInfo();
                        T obj = (T)Activator.CreateInstance(typeof(T));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = obj.GetType().GetProperty(reader.GetName(i));

                            if (property != null && !reader.GetValue(i).Equals(DBNull.Value))
                            {
                                flag = new ReflectionHelper().SetPropertyValue(property.Name, reader.GetValue(i).ToString(), ref obj, ref lstError);
                            }
                        }

                        lstObj.Add(obj);
                    }
                }
            }

            return lstObj;
        }
        /// <summary>
        /// 取出錯誤列表
        /// </summary>
        /// <param name="ErrCode">錯誤代碼</param>
        /// <returns></returns>
        public List<ErrorMessageList> GetErrorList(string ErrCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageList ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WITH(NOLOCK)  WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            return lstErr;
        }

        /// <summary>
        /// 取得單一筆錯誤訊息
        /// </summary>
        /// <param name="ErrCode">錯誤代碼</param>
        /// <returns>ErrorMessageList</returns>
        public ErrorMessageList GetErrorMessage(string ErrCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageList ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WITH(NOLOCK)  WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            ErrorMessageList obj = null;
            if (lstErr != null)
            {
                if (lstErr.Count > 0)
                {
                    obj = new ErrorMessageList()
                    {
                        ErrCode = lstErr[0].ErrCode,
                        ErrMsg = lstErr[0].ErrMsg
                    };
                }
            }
            return obj;
        }
        public ErrorMessageList GetErrorMessage(string ErrCode, int Language)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageMutiLanguage ";
            SqlParameter[] para = new SqlParameter[3];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (term != "")
            {
                term += " AND ";
            }
            term += " LanguageId=@LanguageId";
            para[nowCount] = new SqlParameter("@LanguageId", SqlDbType.Int);
            para[nowCount].Value = Language;
            para[nowCount].Direction = ParameterDirection.Input;
            nowCount++;
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            ErrorMessageList obj = null;
            if (lstErr != null)
            {
                if (lstErr.Count > 0)
                {
                    obj = new ErrorMessageList()
                    {
                        ErrCode = lstErr[0].ErrCode,
                        ErrMsg = lstErr[0].ErrMsg
                    };
                }
            }
            return obj;
        }
        public List<CityData> GetAllCity()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CityData> lstZip = null;
            int nowCount = 0;
            string SQL = "SELECT * FROM TB_City ORDER BY CityID ASC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
         
            lstZip = GetObjList<CityData>(ref flag, ref lstError, SQL, para, term);
            return lstZip;
        }
        /// <summary>
        ///  取得行政區列表
        /// </summary>
        /// <param name="CityID">縣市代碼</param>
        /// <returns></returns>
        public List<ZipCodeData> GetAllZip(int CityID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ZipCodeData> lstZip = null;
            int nowCount = 0;
            string SQL = "SELECT * FROM VW_GetZipCode ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (CityID > 0)
            {
                term += " CityID=@CityID";
                para[nowCount] = new SqlParameter("@CityID", SqlDbType.Int);
                para[nowCount].Value = CityID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY CityID,AreaID ASC";
            lstZip = GetObjList<ZipCodeData>(ref flag, ref lstError, SQL, para, term);
            return lstZip;
        }
        public List<iRentManagerStation> GetAllManageStation()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentManagerStation> lstManagerStation = null;
            int nowCount = 0;
            string SQL = "SELECT [StationID],[StationName] FROM [TB_ManagerStation] ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
         
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY StationID ASC";
            lstManagerStation = GetObjList<iRentManagerStation>(ref flag, ref lstError, SQL, para, term);
            return lstManagerStation;
        }
        public UPDList GetUpdList()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<UPDList> lstUPDList = null;
            UPDList ObjUPDList = null;
            int nowCount = 0;
            string SQL = "SELECT TOP 1 [AreaList],[LoveCode],[NormalRent],[Polygon],[Parking]  FROM [dbo].[TB_UPDDataWatchTable] ORDER BY Id DESC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstUPDList = GetObjList<UPDList>(ref flag, ref lstError, SQL, para, term);
            if (lstUPDList != null)
            {
                if (lstUPDList.Count > 0)
                {
                    ObjUPDList = lstUPDList[0];
                }
            }
            return ObjUPDList;
        }
        /// <summary>
        /// 取得愛心捐贈碼
        /// </summary>
        /// <returns></returns>
        public List<LoveCodeListData> GetLoveCode()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<LoveCodeListData> lstLoveCode = null;
            int nowCount = 0;
            string SQL = "SELECT [LoveName],[LoveCode],[LoveShortName],[UNICode] FROM TB_LoveCode ORDER BY Id ASC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstLoveCode = GetObjList<LoveCodeListData>(ref flag, ref lstError, SQL, para, term);
            return lstLoveCode;
        }
        /// <summary>
        /// 取得愛心捐贈碼,指定筆數
        /// </summary>
        /// <param name="TakeCount">指定筆數</param>
        /// <returns></returns>
        public List<LoveCodeListData> GetLoveCode(int TakeCount)
        {
            bool flag = false;

            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<LoveCodeListData> lstLoveCode = null;
            string SQL = "SELECT TOP {0} [LoveName],[LoveCode],[LoveShortName],[UNICode] FROM TB_LoveCode ORDER BY Id ASC";

            string repWorld = "";
            if (TakeCount > 0) 
                repWorld = TakeCount.ToString();
            SQL = String.Format(SQL, repWorld);

            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstLoveCode = GetObjList<LoveCodeListData>(ref flag, ref lstError, SQL, para, term);
            return lstLoveCode;
        }
        /// <summary>
        /// 取出日期區間內的所有假日
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <returns></returns>
        public List<Holiday> GetHolidays(string SD, string ED)
        {
            bool flag = false;
            List<Holiday> lstHoliday = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT HolidayDate FROM TB_Holiday  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            flag = (false == string.IsNullOrEmpty(SD));
            if (flag)
            {
                flag = (false == string.IsNullOrEmpty(ED));
            }

            if (flag)
            {
                para[0] = new SqlParameter("@SD", SqlDbType.VarChar, 8);
                para[0].Value = SD;
                para[0].Direction = ParameterDirection.Input;
                para[1] = new SqlParameter("@ED", SqlDbType.VarChar, 8);
                para[1].Value = ED;
                para[1].Direction = ParameterDirection.Input;
                term = " HolidayDate>=@SD AND HolidayDate<=@ED";
                if ("" != term)
                {
                    SQL += " WHERE use_flag=1 AND " + term;
                }
                else
                {
                    SQL += " WHERE user_flag=1 ";
                }
                SQL += "  ORDER BY HolidayDate ASC";

                lstHoliday = GetObjList<Holiday>(ref flag, ref lstError, SQL, para, term);
            }

            return lstHoliday;

        }
        /// <summary>
        /// 取出選單列表
        /// </summary>
        /// <returns></returns>
        public List<BE_MenuCombind> GetMenuList()
        {
            bool flag = false;
            List<BE_MenuList> lstMenu = null;
            List<BE_MenuCombind> lstData = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT * FROM VW_GetMenuList WHERE OperationPowerGroupId>0  ORDER BY [Sort] ASC,SubMenuSort asc  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstMenu = GetObjList<BE_MenuList>(ref flag, ref lstError, SQL, para, term);
            if (lstMenu != null)
            {
                lstData = new List<BE_MenuCombind>();
                int len = lstMenu.Count;
                if (len > 0)
                {
                    BE_MenuCombind first = new BE_MenuCombind()
                    {
                        MenuId = lstMenu[0].MenuId,
                        MenuCode = lstMenu[0].SubMenuCode.Substring(0, 1),
                        MenuName = lstMenu[0].MenuName,
                        Sort = lstMenu[0].Sort,
                        lstSubMenu = new List<BE_SubMenu>()
                             
                    };
                    first.lstSubMenu.Add(
                        new BE_SubMenu()
                        {
                            isNewWindow = lstMenu[0].isNewWindow,
                            SubMenuCode = lstMenu[0].SubMenuCode,
                            MenuAction = lstMenu[0].MenuAction,
                            MenuController = lstMenu[0].MenuController,
                            OperationPowerGroupId = lstMenu[0].OperationPowerGroupId,
                            SubMenuName = lstMenu[0].SubMenuName,
                            SubMenuSort = lstMenu[0].SubMenuSort
                        }
                    );
                    lstData.Add(first);
                    for (int i = 1; i < len; i++)
                    {
                        int index = lstData.FindIndex(delegate (BE_MenuCombind t)
                        {
                            return t.MenuId == lstMenu[i].MenuId;
                        });
                        if (index > -1)
                        {
                            lstData[index].lstSubMenu.Add(
                             new BE_SubMenu()
                             {
                                 isNewWindow = lstMenu[i].isNewWindow,
                                 SubMenuCode = lstMenu[i].SubMenuCode,
                                 MenuAction = lstMenu[i].MenuAction,
                                 MenuController = lstMenu[i].MenuController,
                                 OperationPowerGroupId = lstMenu[i].OperationPowerGroupId,
                                 SubMenuName = lstMenu[i].SubMenuName,
                                 SubMenuSort = lstMenu[i].SubMenuSort
                             }
                         );
                        }
                        else
                        {
                            BE_MenuCombind tmp = new BE_MenuCombind()
                            {
                                MenuId = lstMenu[i].MenuId,
                                MenuCode = lstMenu[i].SubMenuCode.Substring(0, 1),
                                MenuName = lstMenu[i].MenuName,
                                Sort = lstMenu[i].Sort,
                                lstSubMenu = new List<BE_SubMenu>()

                            };
                            tmp.lstSubMenu.Add(
                                new BE_SubMenu()
                                {
                                    isNewWindow = lstMenu[i].isNewWindow,
                                    SubMenuCode = lstMenu[i].SubMenuCode,
                                    MenuAction = lstMenu[i].MenuAction,
                                    MenuController = lstMenu[i].MenuController,
                                    OperationPowerGroupId = lstMenu[i].OperationPowerGroupId,
                                    SubMenuName = lstMenu[i].SubMenuName,
                                    SubMenuSort = lstMenu[i].SubMenuSort
                                }
                            );
                            lstData.Add(tmp);
                        }
                    }
                }
                
            }

            return lstData;
        }
        public List<BE_MenuCombindConsistPower> GetMenuListConsistPower()
        {
            bool flag = false;
            List<BE_MenuList> lstMenu = null;
            List<BE_PowerListCombind> lstPower = GetMenuPowerList();
            List<BE_MenuCombindConsistPower> lstData = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT * FROM VW_GetMenuList WHERE OperationPowerGroupId>0  ORDER BY [Sort] ASC,SubMenuSort asc  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstMenu = GetObjList<BE_MenuList>(ref flag, ref lstError, SQL, para, term);
            if (lstMenu != null)
            {
                lstData = new List<BE_MenuCombindConsistPower>();
                int len = lstMenu.Count;
                if (len > 0)
                {
                    BE_MenuCombindConsistPower first = new BE_MenuCombindConsistPower()
                    {
                        MenuId = lstMenu[0].MenuId,
                        MenuCode = lstMenu[0].SubMenuCode.Substring(0, 1),
                        MenuName = lstMenu[0].MenuName,
                        Sort = lstMenu[0].Sort,
                        lstSubMenu = new List<BE_SubMenuConsistPower>()

                    };
                    first.lstSubMenu.Add(
                        new BE_SubMenuConsistPower()
                        {
                            isNewWindow = lstMenu[0].isNewWindow,
                            SubMenuCode = lstMenu[0].SubMenuCode,
                            MenuAction = lstMenu[0].MenuAction,
                            MenuController = lstMenu[0].MenuController,
                            OperationPowerGroupId = lstMenu[0].OperationPowerGroupId,
                            SubMenuName = lstMenu[0].SubMenuName,
                            SubMenuSort = lstMenu[0].SubMenuSort,
                            lstPowerFunc = lstPower.FindAll(delegate (BE_PowerListCombind sp) { return sp.OperationPowerGroupId == lstMenu[0].OperationPowerGroupId; })
                        }
                    );
                    lstData.Add(first);
                    for (int i = 1; i < len; i++)
                    {
                        int index = lstData.FindIndex(delegate (BE_MenuCombindConsistPower t)
                        {
                            return t.MenuId == lstMenu[i].MenuId;
                        });
                        if (index > -1)
                        {
                            lstData[index].lstSubMenu.Add(
                             new BE_SubMenuConsistPower()
                             {
                                 isNewWindow = lstMenu[i].isNewWindow,
                                 SubMenuCode = lstMenu[i].SubMenuCode,
                                 MenuAction = lstMenu[i].MenuAction,
                                 MenuController = lstMenu[i].MenuController,
                                 OperationPowerGroupId = lstMenu[i].OperationPowerGroupId,
                                 SubMenuName = lstMenu[i].SubMenuName,
                                 SubMenuSort = lstMenu[i].SubMenuSort,
                                 lstPowerFunc = lstPower.FindAll(delegate (BE_PowerListCombind sp) { return sp.OperationPowerGroupId == lstMenu[0].OperationPowerGroupId; })
                             }
                         );
                        }
                        else
                        {
                            BE_MenuCombindConsistPower tmp = new BE_MenuCombindConsistPower()
                            {
                                MenuId = lstMenu[i].MenuId,
                                MenuCode = lstMenu[i].SubMenuCode.Substring(0, 1),
                                MenuName = lstMenu[i].MenuName,
                                Sort = lstMenu[i].Sort,
                                lstSubMenu = new List<BE_SubMenuConsistPower>()

                            };
                            tmp.lstSubMenu.Add(
                                new BE_SubMenuConsistPower()
                                {
                                    isNewWindow = lstMenu[i].isNewWindow,
                                    SubMenuCode = lstMenu[i].SubMenuCode,
                                    MenuAction = lstMenu[i].MenuAction,
                                    MenuController = lstMenu[i].MenuController,
                                    OperationPowerGroupId = lstMenu[i].OperationPowerGroupId,
                                    SubMenuName = lstMenu[i].SubMenuName,
                                    SubMenuSort = lstMenu[i].SubMenuSort,
                                    lstPowerFunc = lstPower.FindAll(delegate (BE_PowerListCombind sp) { return sp.OperationPowerGroupId == lstMenu[0].OperationPowerGroupId; })
                                }
                            );
                            lstData.Add(tmp);
                        }
                    }
                }

            }

            return lstData;
        }
        /// <summary>
        /// 選單功能權限
        /// </summary>
        /// <returns></returns>
        public List<BE_PowerListCombind> GetMenuPowerList()
        {
            bool flag = false;
            List<BE_PowerList> lstPower = null;
            List<BE_PowerListCombind> lstData = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT * FROM VW_GetDefPowerList WHERE OperationPowerGroupId>0  ORDER BY [OperationPowerGroupId] ASC,OperationPowerID asc  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstPower = GetObjList<BE_PowerList>(ref flag, ref lstError, SQL, para, term);
            if (lstPower != null)
            {
                lstData = new List<BE_PowerListCombind>();
                int len = lstPower.Count;
                if (len > 0)
                {
                    BE_PowerListCombind first = new BE_PowerListCombind()
                    {
                        OperationPowerGroupId = lstPower[0].OperationPowerGroupId,
                        lstPowerFunc = new List<BE_PowerListCombindSubData>()

                    };
                    first.lstPowerFunc.Add(
                        new BE_PowerListCombindSubData()
                        {
                             Code = lstPower[0].Code,
                            OPName = lstPower[0].OPName
                        }
                    );
                    lstData.Add(first);
                    for (int i = 1; i < len; i++)
                    {
                        int index = lstData.FindIndex(delegate (BE_PowerListCombind t)
                        {
                            return t.OperationPowerGroupId == lstPower[i].OperationPowerGroupId;
                        });
                        if (index > -1)
                        {
                            lstData[index].lstPowerFunc.Add(
                             new BE_PowerListCombindSubData()
                             {
                                 Code = lstPower[i].Code,
                                 OPName = lstPower[i].OPName
                             }
                         );
                        }
                        else
                        {
                            BE_PowerListCombind tmp = new BE_PowerListCombind()
                            {
                                OperationPowerGroupId = lstPower[i].OperationPowerGroupId,
                                lstPowerFunc = new List<BE_PowerListCombindSubData>()

                            };
                            tmp.lstPowerFunc.Add(
                                new BE_PowerListCombindSubData()
                                {
                                    Code = lstPower[i].Code,
                                    OPName = lstPower[i].OPName
                                }
                            );
                            lstData.Add(tmp);
                        }
                    }
                }

            }

            return lstData;
        }

    }
}

