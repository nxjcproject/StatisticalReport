
//$(document).ready(function () {
//    var m_MsgData;
//    $.ajax({
//        type: "POST",
//        url: "CoalUsageDailyReport.aspx/GetCoalUsageDailyReport",
//        data: '',
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            m_MsgData = jQuery.parseJSON(msg.d);
//            $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData['rows']);
//        },
//        error: handleError
//    });
//});


//function InitializeGrid(myData) {

//    $('#gridMain_ReportTemplate').datagrid({
//        title: '',
//        data: myData,
//        dataType: "json",
//        striped: true,
//        rownumbers: true,
//        singleSelect: true,

//        toolbar: '#toolbar_ReportTemplate'
//    });
//}

//function handleError() {
//    $('#gridMain_ReportTemplate').datagrid('loadData', []);
//    $.messager.alert('失败', '获取数据失败');
//}


//********************************************
var timeType = '';
var startDate = '';//开始日期
$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    startDate = nowDate.getFullYear() + '-' +( nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#startDate").datebox('setValue', startDate);
    loadGridData("first",startDate,startDate);
});

function loadGridData(myLoadType,startDate, endDate) {
    var m_MsgData;
    var data = '{startDate: "' + startDate + '", endDate: "' + endDate + '"}';
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CoalUsageDailyReport.aspx/GetCoalUsageDailyReport",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == "first") {
                InitializeGrid(m_MsgData);
            }
            if (myLoadType == "last") {
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            handleError
        }
    });
}

function InitializeGrid(myData) {

    $('#gridMain_ReportTemplate').datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        rownumbers: true,
        singleSelect: true,

        toolbar: '#toolbar_ReportTemplate'
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

//显示结束时间日期框
function DataBoxShow() {
    $('.mDisplay').css('display', '');
}
//隐藏结束时间日期框
function DataBoxHide() {
    $('.mDisplay').css('display', 'none');
}

function QueryReportFun() {
    timeType = $('input[name="timeType"]:checked').val();
    //var starDate = '';//开始日期
    var endDate = '';//结束日期
    var data = '';//参数
    if ('day' == timeType) {
        startDate = $('#startDate').datetimespinner('getValue');//开始时间
        //var endDate = $('#endDate').datetimespinner('getValue');//结束时间
        //data = '{startDate: "' + startDate + '", endDate: "' + startDate + '"}'
        endDate = startDate;
        if (startDate == "") {
            $.messager.alert('警告', '请选择时间');
            return;
        }
    }
    else {
        startDate = $('#startDate').datebox('getValue');//开始时间
        endDate = $('#endDate').datebox('getValue');//结束时间
        //data = '{startDate: "' + startDate + '", endDate: "' + endDate + '"}'
        if (startDate == "" || endDate == "") {
            $.messager.alert('警告', '请选择起止时间');
            return;
        }
        if (startDate > endDate) {
            $.messager.alert('警告', '结束时间不能大于开始时间！');
            return;
        }
    }
    loadGridData("last",startDate, endDate);
}