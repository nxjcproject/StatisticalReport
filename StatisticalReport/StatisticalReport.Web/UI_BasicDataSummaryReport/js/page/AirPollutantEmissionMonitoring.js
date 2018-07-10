$(function () {
    InitDate();
    loadDataGrid("first");
});

//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setMonth(beforeDate.getMonth() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " 0:00:00";
    $('#startTime').datetimebox('setValue', beforeString);
    $('#endTime').datetimebox('setValue', nowString);
}

function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_AirPollutantEmission').datagrid({
            columns: [
                [
                    { field: 'Name', title: '分厂', width: 100, rowspan: 2 },
                    { field: 'clinkerConsump1tion', title: '颗粒物(均值)', width: 200, align: 'center', colspan: 2 },
                    { field: 'clinkerConsumpt2ion', title: '二氧化碳(均值)', width: 100, align: 'right', rowspan: 2 },
                    { field: 'cementConsumpti3on', title: '氮氧化物(均值)', width: 100, align: 'right', rowspan: 2 },
                    { field: 'clinkerConsumpti4on', title: 'O2含量(均值)', width: 100, align: 'right', rowspan: 2 },
                    { field: 'clinkerConsumpti5on', title: '喷氨量(累加)', width: 100, align: 'right', rowspan: 2 }
                ],
                [
                    { field: 'Name6', title: '窑头', width: 100, align: 'right' },
                    { field: 'clink7erConsumption', title: '窑尾', width: 100, align: 'right' }
                ]
            ],
            fit: true,
            toolbar: "#toolbar_AirPollutantEmission",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        })
    }
    else {
        $('#grid_AirPollutantEmission').datagrid("loadData", myData);
    }
}

function Query() {
    var startTime = $('#startTime').datetimebox('getValue');//开始时间
    var endTime = $('#endTime').datetimebox('getValue');//结束时间
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "AirPollutantEmissionMonitoring.aspx/GetAirPollutantEmissionData",
        data: "{mStartTime:'" + startTime + "',mEndTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                $('#grid_AirPollutantEmission').datagrid("loadData", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_AirPollutantEmission").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}