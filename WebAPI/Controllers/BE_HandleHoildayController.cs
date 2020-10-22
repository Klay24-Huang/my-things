using Domain;
using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】新增/移除假日
    /// </summary>
    public class BE_HandleHoildayController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】假日維護
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_HandleHoilday(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_HandleHoildayController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleHoilday apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";

            int MonStart = 1;
            int MonMid = 2;
            int MonEnd = 3;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleHoilday>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if (apiInput.QuerySeason < 1 || apiInput.QuerySeason > 4)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {
                MonStart = ((apiInput.QuerySeason - 1) * 3) + 1;

                MonEnd = (apiInput.QuerySeason * 3) + 1;
                DateTime SD = new DateTime(apiInput.QueryYear, MonStart, 1);
                DateTime MD = SD.AddMonths(1);
                if (MonEnd > 12)
                {
                    apiInput.QueryYear += 1;
                    MonEnd = 1;
                }

                DateTime ED = new DateTime(apiInput.QueryYear, MonEnd, 1).AddSeconds(-1);
                if (apiInput.Hoilday.Count == 0)
                {
                    //移除當季所有
                }
                else
                {
                    List<BE_Hoilday> holidays = GetHolidaysOfBE(SD.ToString("yyyyMMdd"), ED.ToString("yyyyMMdd"));
                    //先刪除
                    List<string> deleteList= new List<string>();
                    int OldLen = holidays.Count;
                    for(int i = 0; i < OldLen; i++)
                    {
                        int index = apiInput.Hoilday.FindIndex(delegate (BE_Hoilday t)
                          {
                              return t.HolidayDate == holidays[i].HolidayDate;
  
                          });
                        if (index < 0)
                        {
                            deleteList.Add(string.Format("'{0}'", holidays[i].HolidayDate));
                        }
                    }
                    //處理要新增的
                    int NewLen = apiInput.Hoilday.Count;
                    List<string> newHoildays = new List<string>();
                    List<string> newShortHoildays = new List<string>();
                    for (int i = 0; i < NewLen; i++)
                    {
                        int index = holidays.FindIndex(delegate (BE_Hoilday t)
                        {
                            return t.HolidayDate == apiInput.Hoilday[i].HolidayDate;

                        });
                        if (index < 0)
                        {
                            newHoildays.Add(string.Format("'{0}'",apiInput.Hoilday[i].HolidayDate));
                            newShortHoildays.Add(string.Format("'{0}'", apiInput.Hoilday[i].HolidayYearMonth));
                        }
                    }
                    if(newHoildays.Count>0 || deleteList.Count > 0)
                    {
                        flag = new OtherRepository(connetStr).HandleHoilday(newHoildays.ToArray(), newShortHoildays.ToArray(), deleteList.ToArray());
                    }
                }

               
              


            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
        private  List<BE_Hoilday> GetHolidaysOfBE(string SD, string ED)
        {
            bool flag = false;
            List<BE_Hoilday> lstHoliday = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT HolidayYearMonth,HolidayDate FROM TB_Holiday  ";
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

                lstHoliday = GetObjList<BE_Hoilday>(ref flag, ref lstError, SQL, para, term);
            }

            return lstHoliday;

        }

        private List<T> GetObjList<T>(ref bool flag, ref List<ErrorInfo> lstError, string SQL, SqlParameter[] para, string term)
     where T : class
        {
            List<T> lstObj;
            using (SqlConnection conn = new SqlConnection(connetStr))
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
    }
}
