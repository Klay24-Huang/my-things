$(function () {
    //身分證字號或外籍人士居留証驗證
    /*
     * 第一個字元代表地區，轉換方式為：A轉換成1,0兩個字元，B轉換成1,1……但是Z、I、O分別轉換為33、34、35
     * 第二個字元代表性別，1代表男性，2代表女性
     * 第三個字元到第九個字元為流水號碼。
     * 第十個字元為檢查號碼。
     * 每個相對應的數字相乘，如A123456789代表1、0、1、2、3、4、5、6、7、8，相對應乘上1987654321，再相加。
     * 相加後的值除以模數，也就是10，取餘數再以模數10減去餘數，若等於檢查碼，則驗證通過
     */
    function checkId() {

        var idNumber = $("#IDNO").val();
        var nationality = 0;
        idNumber = idNumber.toUpperCase();
        if (/^[A-Z]$/.test(idNumber.substr(1, 1))) {
            nationality = 1
        } else {
            nationality = 0
        }

        //本國人
        if (nationality == 0) {

            //驗證填入身分證字號長度及格式
            if (idNumber.length != 10) {
                return false;
            }
            //格式，用正則表示式比對第一個字母是否為英文字母
            if (isNaN(idNumber.substr(1, 9)) ||
                (!/^[A-Z]$/.test(idNumber.substr(0, 1)))) {
                return false;
            }

            var idHeader = "ABCDEFGHJKLMNPQRSTUVXYWZIO"; //按照轉換後權數的大小進行排序
            //這邊把身分證字號轉換成準備要對應的
            idNumber = (idHeader.indexOf(idNumber.substring(0, 1)) + 10) + '' + idNumber.substr(1, 9);
            //開始進行身分證數字的相乘與累加，依照順序乘上1987654321
            s = parseInt(idNumber.substr(0, 1)) +
                parseInt(idNumber.substr(1, 1)) * 9 +
                parseInt(idNumber.substr(2, 1)) * 8 +
                parseInt(idNumber.substr(3, 1)) * 7 +
                parseInt(idNumber.substr(4, 1)) * 6 +
                parseInt(idNumber.substr(5, 1)) * 5 +
                parseInt(idNumber.substr(6, 1)) * 4 +
                parseInt(idNumber.substr(7, 1)) * 3 +
                parseInt(idNumber.substr(8, 1)) * 2 +
                parseInt(idNumber.substr(9, 1));

            checkNum = parseInt(idNumber.substr(10, 1));
            //模數 - 總和/模數(10)之餘數若等於第九碼的檢查碼，則驗證成功
            //若餘數為0，檢查碼就是0
            if ((s % 10) == 0 || (10 - s % 10) == checkNum) {
                return true;
            }
            else {
                return false;
            }

        }
        //外籍生，居留證號規則跟身分證號差不多，只是第二碼也是英文字母代表性別，跟第一碼轉換二位數字規則相同，但只取餘數
        else {

            //驗證填入身分證字號長度及格式
            if (idNumber.length != 10) {
                return false;
            }
            //格式，用正則表示式比對第一個字母是否為英文字母
            if (isNaN(idNumber.substr(2, 8)) ||
                (!/^[A-Z]$/.test(idNumber.substr(0, 1))) ||
                (!/^[A-Z]$/.test(idNumber.substr(1, 1)))) {
                return false;
            }

            var idHeader = "ABCDEFGHJKLMNPQRSTUVXYWZIO"; //按照轉換後權數的大小進行排序
            //這邊把身分證字號轉換成準備要對應的
            idNumber = (idHeader.indexOf(idNumber.substring(0, 1)) + 10) +
                '' + ((idHeader.indexOf(idNumber.substr(1, 1)) + 10) % 10) + '' + idNumber.substr(2, 8);
            //開始進行身分證數字的相乘與累加，依照順序乘上1987654321

            s = parseInt(idNumber.substr(0, 1)) +
                parseInt(idNumber.substr(1, 1)) * 9 +
                parseInt(idNumber.substr(2, 1)) * 8 +
                parseInt(idNumber.substr(3, 1)) * 7 +
                parseInt(idNumber.substr(4, 1)) * 6 +
                parseInt(idNumber.substr(5, 1)) * 5 +
                parseInt(idNumber.substr(6, 1)) * 4 +
                parseInt(idNumber.substr(7, 1)) * 3 +
                parseInt(idNumber.substr(8, 1)) * 2 +
                parseInt(idNumber.substr(9, 1));

            //檢查號碼 = 10 - 相乘後個位數相加總和之尾數。
            checkNum = parseInt(idNumber.substr(10, 1));
            //模數 - 總和/模數(10)之餘數若等於第九碼的檢查碼，則驗證成功
            ///若餘數為0，檢查碼就是0
            if ((s % 10) == 0 || (10 - s % 10) == checkNum) {
                return true;
            }
            else {
                return false;
            }
        }
    }



    $("#delete").on('click', function () {
        var flag = checkId();
        //舊有ID有許多不符格式，暫且移除檢核
        if (true) {
            $("#frmDelete").submit();
        } else {
            var message = "身分證格式錯誤";
            disabledLoadingAndShowAlert(message);
        }
    })

});