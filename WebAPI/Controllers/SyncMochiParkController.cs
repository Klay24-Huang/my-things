﻿using Domain.Common;
using Domain.SP.Input.Mochi;
using Domain.SP.Output;
using Domain.SP.Output.Mochi;
using Domain.TB.Mochi;
using Domain.WebAPI.output.Mochi;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 同步車麻吉停車場
    /// </summary>
    public class SyncMochiParkController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoSyncMochiPark(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SyncMochiParkController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            NullOutput apiInput = null;
            NullOutput outputApi = new NullOutput();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string MochiToken = ""; //車麻吉token
            #endregion
            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("not input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion
            #region TB
            if (flag)
            {
                List<SyncMachiParkId> lstParkId = null;
                List<SyncMachiParkId> ListParkSend = new List<SyncMachiParkId>();   //要寄信通知的停車場清單
                MochiParkAPI webAPI = new MochiParkAPI();
                DateTime NowDate = DateTime.Now;
                SPInput_GetMachiToken spIGetToken = new SPInput_GetMachiToken()
                {
                    NowTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    LogID = LogID
                };
                SPOutput_GetMachiToken spOGetToken = new SPOutput_GetMachiToken();
                SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken> sqlGetHelp = new SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken>(connetStr);
                string spName = new ObjType().GetSPName(ObjType.SPType.GetMochiToken);

                flag = sqlGetHelp.ExecuteSPNonQuery(spName, spIGetToken, ref spOGetToken, ref lstError);
                if (spOGetToken.Token == "")
                {
                    WebAPIOutput_MochiLogin wsOutLogin = new WebAPIOutput_MochiLogin();
                    flag = webAPI.DoLogin(ref wsOutLogin);
                    if (flag && wsOutLogin.data.access_token != "")
                    {
                        long second = wsOutLogin.data.expires_in;
                        DateTime TokenEnd = NowDate.AddSeconds(second);

                        SPInput_MaintainMachiToken spMaintain = new SPInput_MaintainMachiToken()
                        {
                            Token = wsOutLogin.data.access_token,
                            StartDate = NowDate,
                            EndDate = TokenEnd,
                            LogID = LogID
                        };
                        SPOutput_Base spMainOut = new SPOutput_Base();
                        SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base> sqlMainHelp = new SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base>(connetStr);
                        spName = new ObjType().GetSPName(ObjType.SPType.MaintainMachiToken);
                        flag = sqlMainHelp.ExecuteSPNonQuery(spName, spMaintain, ref spMainOut, ref lstError);
                        if (flag)
                        {
                            MochiToken = wsOutLogin.data.access_token;
                        }
                    }
                }
                else
                {
                    MochiToken = spOGetToken.Token;
                }

                if (flag)
                {
                    //取出目前所有的停車場
                    lstParkId = new ParkingRepository(connetStr).GetMachiParkId();
                }

                if (flag)
                {
                    if (MochiToken != "")
                    {
                        WebAPIOutput_ParkData wsoutPark = new WebAPIOutput_ParkData();
                        flag = webAPI.DoSyncPark(MochiToken, ref wsoutPark);

                        if (flag)
                        {
                            int len = wsoutPark.data.Parkinglots.Count;

                            lstError = new List<ErrorInfo>();
                            for (int i = 0; i < len; i++)
                            {
                                string detailStr = "";
                                for (int j = 0; j < wsoutPark.data.Parkinglots[i].detail.Opening_Time.detail.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        detailStr += wsoutPark.data.Parkinglots[i].detail.Opening_Time.detail[j];
                                    }
                                    else
                                    {
                                        detailStr += "⊙" + wsoutPark.data.Parkinglots[i].detail.Opening_Time.detail[j];
                                    }
                                }
                                //找出是否有存在
                                int index = lstParkId.FindIndex(delegate (SyncMachiParkId ParkId)
                                {
                                    return wsoutPark.data.Parkinglots[i].id.Replace(" ", "") == ParkId.Id.Replace(" ", "");
                                });
                                if (index == -1) //新增
                                {
                                    try
                                    {
                                        SPInput_MochiParkHandle spInput = new SPInput_MochiParkHandle()
                                        {
                                            addr = wsoutPark.data.Parkinglots[i].detail.address,
                                            tel = wsoutPark.data.Parkinglots[i].detail.telephone ?? "",
                                            city = wsoutPark.data.Parkinglots[i].detail.city,
                                            open_status = wsoutPark.data.Parkinglots[i].detail.Opening_Time.status,
                                            t_period = wsoutPark.data.Parkinglots[i].detail.Opening_Time.period,
                                            all_day_open = Convert.ToInt16((wsoutPark.data.Parkinglots[i].detail.Opening_Time.all_day_open) ? 1 : 0),
                                            detail = detailStr,
                                            addUser = "SYSTEM",
                                            lat = Convert.ToDecimal(wsoutPark.data.Parkinglots[i].lat),
                                            lng = Convert.ToDecimal(wsoutPark.data.Parkinglots[i].lng),
                                            charge_mode = wsoutPark.data.Parkinglots[i].current_price.charge_mode,
                                            price = Convert.ToInt32((wsoutPark.data.Parkinglots[i].current_price.price.HasValue) ? wsoutPark.data.Parkinglots[i].current_price.price : 0),
                                            cooperation_state = wsoutPark.data.Parkinglots[i].cooperation_state,
                                            t_Operator = "",
                                            Name = wsoutPark.data.Parkinglots[i].name,
                                            Id = wsoutPark.data.Parkinglots[i].id,
                                            LogID = LogID
                                        };
                                        SPOutput_Base spOut = new SPOutput_Base();
                                        spName = new ObjType().GetSPName(ObjType.SPType.MochiParkHandle);

                                        SQLHelper<SPInput_MochiParkHandle, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_MochiParkHandle, SPOutput_Base>(connetStr);
                                        flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                                        if (!flag)
                                        {
                                            break;
                                        }

                                        // 新增的要通知
                                        ListParkSend.Add(new SyncMachiParkId {
                                            Id = wsoutPark.data.Parkinglots[i].id,
                                            Name = wsoutPark.data.Parkinglots[i].name,
                                            use_flag =1
                                        });
                                    }
                                    catch
                                    {
                                        flag = false;
                                    }
                                }
                                else
                                {
                                    lstParkId[index].use_flag = 1;
                                }
                            }

                            lstParkId = lstParkId.FindAll(delegate (SyncMachiParkId ParkId) { return ParkId.use_flag == 0; });
                            if (lstParkId != null)
                            {
                                if (lstParkId.Count > 0)
                                {
                                    int delCount = lstParkId.Count;
                                    spName = new ObjType().GetSPName(ObjType.SPType.DisabledMachiPark);
                                    for (int k = 0; k < delCount; k++)
                                    {
                                        //usp_disabledMachiPark_202004
                                        SPInput_DisabledMochiPark spInputDisabled = new SPInput_DisabledMochiPark()
                                        {
                                            Id = lstParkId[k].Id,
                                            LogID = LogID
                                        };
                                        SPOutput_Base spOutDisabled = new SPOutput_Base();

                                        SQLHelper<SPInput_DisabledMochiPark, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_DisabledMochiPark, SPOutput_Base>(connetStr);
                                        flag = sqlHelp.ExecuteSPNonQuery(spName, spInputDisabled, ref spOutDisabled, ref lstError);
                                    }
                                    
                                    // 停用的要通知
                                    ListParkSend.AddRange(lstParkId);
                                }
                            }

                            if (flag)
                            {
                                if (ListParkSend != null && ListParkSend.Count > 0)
                                {
                                    //發mail
                                    SendMail send = new SendMail();
                                    string Body = string.Empty;
                                    string Table = string.Empty;

                                    Table += "<table border=1><tr style='background-color:#8DD26F;'><th>名稱</th><th>異動</th></tr>";

                                    foreach (var Park in ListParkSend.OrderByDescending(x => x.use_flag))
                                    {
                                        Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", Park.Name, Park.use_flag == 1 ? "新增" : "停用");
                                    }

                                    Table += "</table>";

                                    Body = string.Format("<p>{0}</p>", Table);

                                    string Title = "車麻吉停車場異動清單";

                                    flag = Task.Run(() => send.DoSendMail(Title, Body, ConfigurationManager.AppSettings["MachiStaionMailList"])).Result;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}