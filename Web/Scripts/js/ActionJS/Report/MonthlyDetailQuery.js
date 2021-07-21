$(document).ready(function () {
    $("#OrderNum").val(OrderNum);
    $("#start").val(SD);
    $("#end").val(ED);
    $("#userID").val(UserID);
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
        $("#btnDownload").on("click", function () {
            $("#formMonthlyDetailQueryDownload").submit();
        });
    }

    $('table').footable();

    SetDefaultDate();

    $("#dpSDate").flatpickr(
        {
            onChange: (selectedDates, dateStr, instance) => {
                flatpickr("#dpEDate",
                    {
                        enable: [
                            {
                                from: dateStr,
                                to: add_months(dateStr, 1)
                            },
                        ],
                        onChange: (selectedDates, dateStr, instance) => {

                            var mindate = $.format.date(add_months(dateStr, -1), 'yyyy-MM-dd');
                            flatpickr("#dpSDate",
                                {
                                    enable: [
                                        {
                                            from: mindate,
                                            to: dateStr
                                        },
                                    ],

                                });

                            if (new Date($("#dpSDate").val()) < mindate) {
                                $("#dpSDate").val(mindate);
                            }
                        }
                    });
            },
        }
    );
    $("#dpEDate").flatpickr(
        {
            onChange: (selectedDates, dateStr, instance) => {
                var mindate = $.format.date(add_months(dateStr, -1), 'yyyy-MM-dd');
                flatpickr("#dpSDate", {
                    enable: [
                        {
                            from: mindate,
                            to: dateStr
                        },
                    ],
                    onChange: (selectedDates, dateStr, instance) => {
                        flatpickr("#dpEDate", {
                            enable: [
                                {
                                    from: dateStr,
                                    to: add_months(dateStr, 1)
                                },
                            ]
                        });
                    },
                });
            },
        }

    );
    function SetDefaultDate() {
        var EndDate = new Date();
        var StartDate = add_months(EndDate, -1);
        if ($('#dpSDate').val() == '') {
            $("#dpSDate").val($.format.date(StartDate, 'yyyy-MM-dd'));
        }
        if ($('#dpEDate').val() == '') {
            $("#dpEDate").val($.format.date(EndDate, 'yyyy-MM-dd'));

        }
    }
    function add_months(dt, n) {
        mydt = new Date(dt);

        var newdt = new Date(mydt.setMonth(mydt.getMonth() + n));

        //console.log(newdt);
        //console.log(newdt.toISOString());

        return newdt;
    }

    $("#formMonthlyMainQuery").on("submit",
        function () {
            var StartDate = new Date($("#dpDate").val());
            var EndDate = new Date($("#dpEDate").val());

            var MaxDate = add_months(StartDate, 1);

            if (StartDate > EndDate) {
                ShowFailMessage("查詢時數使用起日不得大於迄日");
                return false;
            }

            if (EndDate > MaxDate) {
                ShowFailMessage("查詢時數使用起迄日超過範圍");
                return false;
            }
            else {
                return true;
            }
        });

    if (outerOfDateRangeMsg !== '') {
        ShowFailMessage(outerOfDateRangeMsg);
    }
});