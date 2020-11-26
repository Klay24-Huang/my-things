$(document).ready(function () {
    $("#btnSend").hide();
    var projectData;
    var areaData;
    $("#City").hide();
    $("#ZipCode").hide();
    $("#start").datepicker();
    $("#end").datepicker();
    GetAreaSync();
    $("#btnCheck").on("click", function () {
        var IDNO = $("#IDNO").val();
        var flag = true;
        var errMsg = "";
        if (IDNO === "") {
            flag = false;
            errMsg = "身份證未輸入";
        } else {
            if (/^[A-Z]{1}[1-2]{1}[0-9]{8}$/.test(IDNO) == false) {
                flag = false;
                errMsg = "身份證格式不正確";
            }
        }
        if (flag) {
            blockUI();
            var SendObj = new Object();
            SendObj.IDNO = IDNO;
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, MemberQueryComplete);

        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
    $("#btnCancel").on("click", function () {
        forminsertorder.submit();
    });
    $("#btnSend").on("click", function () {
        var flag = true;
        var errMsg = "";
        var IDNO = $("#IDNO").val();
        var UName = $("#UName").val();
        var ceil = $("#ceil").val();
        var email = $("#email").val();
        var ProjID = $("#Proj").val();
        var StationID = $("#StationID").val();
        var CarType = $("#CarType").val();
        var invoiceKind = $('input:radio:checked[name="invoiceKind"]').val();
        var invoiceAddr = $("#invoiceAddr").val();
        var City = $("#City").val();
        var ZipCode = $("#ZipCode").val();
        var ED = $("#end").val();
        var EH = $("#EH").val();
        var EM = $("#EM").val();
        var SD = $("#start").val();
        var SH = $("#SH").val();
        var SM = $("#SM").val();
        var checkList = [IDNO, UName, ceil, email,SD,ED, ProjID, StationID, CarType,SH,SM,EH,EM];
        var msgList = ["請輸入身份證", "請輸入姓名", "請輸入聯絡電話", "請輸入E-Mail", "未選擇取車日期", "未選擇還車日期"
                     , "請選擇優惠專案", "請選擇出還車據點", "請選擇車型"
                     , "未選擇取車時間【時】 ", "未選擇取車時間【分】 ", "未選擇還車時間【時】 ", "未選擇還車時間【分】 "];
        var UNIMNO = $("#UNIMNO").val();
        for (var i = 0; i < 6; i++) {
            if (checkList[i] === "") {
                flag = false;
                errMsg += msgList[i] + "<br\>";
            }
            if (checkList[i+6] === "-1") {
                flag = false;
                errMsg += msgList[i+6] + "<br\>";
            }

        }
   
        //console.log("invoiceKind=" + invoiceKind);
        //for (var i = 6; i < 13; i++) {
        //    if (checkList[i] === "-1") {
        //        flag = false;
        //        errMsg += msgList[i] + "<br\>";
        //    }

        //}
     if (flag) {
     
        //if (invoiceKind === "4" || invoiceKind === "3") {
            if (City === "-1" || ZipCode === "-1" || invoiceAddr === "") {
                flag = false;
                errMsg = "填入發票地址為必填";
            }
        //}
     }
     if (flag) {
         if (/^[A-Z]{1}[1-2]{1}[0-9]{8}$/.test(IDNO) == false) {
             flag = false;
             errMsg = "身份證格式不正確";
         }
     }
     if (flag) {
         var endTime = new Date(ED + ' ' + EH + ":" + EM + ":00");
         var startTime = new Date(SD + ' ' + SH + ":" + SM + ":00");
         if (startTime >= endTime) {
             flag = false;
             errMsg = "取車時間不能大於或等於還車時間";
         }
         if (flag) {
             if (new Date() >= startTime) {
                 flag = false;
                 errMsg = "取車時間必需大於現在時間";
             }
         }
     }
        if (flag) {
            blockUI();
            var SendObj = new Object();
            SendObj.IDNO = IDNO;
            SendObj.ProjID = ProjID;
            SendObj.StationID = StationID;
            SendObj.CarType = CarType;
            SendObj.RStationID = StationID;
            SendObj.SD = SD.replace(/\//g, "") + SH + SM + "00";
            SendObj.ED = ED.replace(/\//g, "") + EH + EM + "00";
            SendObj.InvoiceBNO = UNIMNO;
            SendObj.InvoiceType = invoiceKind;
            SendObj.InvoiceAddress = invoiceAddr;
            SendObj.email = email;
            SendObj.City = City;
            SendObj.ZipCode = ZipCode;
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, BookingComplete);

        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
});
function GetArea(jsonData) {
    areaData = jsonData;
    console.log(areaData);
}
function InitCity() {
    var cityArray = [];
    $('#City').show();
    $('#ZipCode').show();
    $.each(areaData.data, function (i, item) {
        if ((jQuery.inArray(item.CityID, cityArray)) == -1) {

            cityArray.push(item.CityID);
            $('#City').append($('<option></option>').val(item.CityID).text(item.CityName));
        }
    });
    $('#City').on("change", function () {
      //  console.log("我有呼叫！！");
        var City = $('#City').val();
        $("#ZipCode").empty().append($('<option></option>').val('-1').text('請選擇鄉、鎮、市、區'));
        $("#invoiceAddr").val('');
        console.log("City=" + City);
        if (City !== "-1") {
            $.each(areaData.data, function (i, item) {
             //   console.log(item.CityID + ";" + City + ";" + (item.CityID === City));
                if (item.CityID === City) {
                    $('#ZipCode').append($('<option></option>').val(item.ZIPCode).text(item.AreaName));
                }
            });
        }
       
    });
}
function SetZipCode(City) {
    $("#ZipCode").empty().append($('<option></option>').val('-1').text('請選擇鄉、鎮、市、區'));
  //  console.log("City=" + City);
    if (City !== "-1") {
        $.each(areaData.data, function (i, item) {
           // console.log(item.CityID + ";" + City + ";" + (item.CityID === City));
            if (item.CityID === City) {
                $('#ZipCode').append($('<option></option>').val(item.ZIPCode).text(item.AreaName));
            }
        });
    }
}
function GetProj(jsonData) {
    $("#btnSend").show();
    $("#btnCheck").hide();
    $("#IDNO").attr('readonly', true);
    projectData = jsonData;
    console.log(projectData);
    var data = projectData.data;
    $("#UName").val(data.UName);
    $("#ceil").val(data.Mobile);
    $("#invoiceAddr").val(data.invoiceAddress);
    $("#email").val(data.email);
    $("#UNIMNO").val(data.UNIMNO);
    var type = parseInt(data.invoiceKind, 10) - 1
    $('#City').empty().append($('<option></option>').val('-1').text('請選擇縣市'));
    InitCity();
    $('#City').val(data.CityID);
    SetZipCode(data.CityID);
    $('#ZipCode').val(data.ZipCode);
    console.log(data.ZipCode);

    $('input[name="invoiceKind"]')[type].checked = true;
 
        $('#Proj').empty().append($('<option></option>').val('-1').text('請選擇方案'));

        $.each(data.projectInfo, function (i, item) {
            $('#Proj').append($('<option></option>').val(item.ProjID).text(item.ProName));
        });
        $('#Proj').on("change", function () {
            var ProjID = $("#Proj").val();
            $('#CityID').empty().append($('<option></option>').val('-1').text('請選擇縣市'));
            $('#StationID').empty().append($('<option></option>').val('-1').text('請選擇據點'));
            $('#StationAddr').html('<span class="span3" ></span>');
            $('#StationDescript').html('<span class="span3" ></span>');
            $('#CarType').empty().append($('<option></option>').val('-1').text('請選擇車型'));
            if (ProjID !== "-1") {
                var ProjIndex = -1;
                for (var i = 0; i < data.projectInfo.length; i++)
                {
                   // console.log(i+":"+data.projectInfo[i].ProjID + ";" + ProjID + ";" + (data.projectInfo[i].ProjID === ProjID));
                    if (data.projectInfo[i].ProjID ===ProjID)
                    {
                        ProjIndex = i;
                        break;
                    }
                }
                if (ProjIndex > -1) {
                    var cityArray = [];
                    $.each(data.projectInfo[ProjIndex].Station, function (i, item) {
                        if ((jQuery.inArray(item.Area, cityArray)) == -1) {

                            cityArray.push(item.Area);
                            $('#CityID').append($('<option></option>').val(item.Area).text(item.Area));
                        }
                        
                    });
                    $('#CityID').on("change", function () {
                        var ProjIndex = -1;
                        $('#StationAddr').html('<span class="span3" ></span>');
                        $('#StationDescript').html('<span class="span3" ></span>');
                        $('#StationID').empty().append($('<option></option>').val('-1').text('請選擇據點'));
                        $('#CarType').empty().append($('<option></option>').val('-1').text('請選擇車型'));
                        for (var i = 0; i < data.projectInfo.length; i++) {
                            // console.log(i+":"+data.projectInfo[i].ProjID + ";" + ProjID + ";" + (data.projectInfo[i].ProjID === ProjID));
                            if (data.projectInfo[i].ProjID === ProjID) {
                                ProjIndex = i;
                                break;
                            }
                        }
                        if ($('#CityID').val() !== "-1" && ProjIndex>-1) {

                            var SiteArray = [];
                            $.each(data.projectInfo[ProjIndex].Station, function (i, item) {
                                if (item.Area === $('#CityID').val()) {
                                    if ((jQuery.inArray(item.Site_ID, SiteArray)) == -1) {

                                        SiteArray.push(item.Site_ID);
                                        $('#StationID').append($('<option></option>').val(item.Site_ID).text(item.SiteName));
                                    }
                                }
                            });

                            $('#StationID').on("change", function () {
                                $('#CarType').empty().append($('<option></option>').val('-1').text('請選擇車型'));
                                $('#StationAddr').html('<span class="span3" ></span>');
                                $('#StationDescript').html('<span class="span3" ></span>');
                                if ($('#StationID').val() !== "-1") {
                                    var ProjIndex = -1;
                                    for (var i = 0; i < data.projectInfo.length; i++) {
                                        // console.log(i+":"+data.projectInfo[i].ProjID + ";" + ProjID + ";" + (data.projectInfo[i].ProjID === ProjID));
                                        if (data.projectInfo[i].ProjID === ProjID) {
                                            ProjIndex = i;
                                            break;
                                        }
                                    }
                                    if (ProjIndex > -1) {

                                        $.each(data.projectInfo[ProjIndex].Station, function (i, item) {
                                            if (item.Site_ID === $('#StationID').val()) {
                                                $('#StationAddr').html('<span class="span3" >'+item.ADDR+'</span>');
                                                $('#StationDescript').html('<span class="span3" >' + item.ADDR + '</span>');
                                                $.each(data.projectInfo[ProjIndex].Station[i].CarType, function (j, car) {
                                                    $('#CarType').append($('<option></option>').val(car.TypeID).text(car.TypeName));
                                                });
                                                
                                            }
                                        });
                                    }
                                    
                                }
                            });
                        }
                    });
                }
            }
        });

}