
$(document).ready(function () {
    var m_MsgData;
    InitDate();
    loadGridData("first");
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    //var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate();
    $('#startDate').datebox('setValue', nowString);
    $('#endDate').datebox('setValue', nowString);
}

function InitializeGrid(myData) {
    $('#gridMain_ReportTemplate').treegrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        rownumbers: true,
        singleSelect: true,

        idField: "id",
        treeField: "VariableName",

        toolbar: '#toolbar_ReportTemplate'
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间
    if (startDate == "" || endDate == "") {
        $.messager.alert('警告', '请选择起止时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }
    loadGridData("last");
}

function loadGridData(myLoadType) {
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间
    $.ajax({
        type: "POST",
        url: "ElectricityUsageDailyReport.aspx/GetElectricityUsageDailyReport",
        data: '{startDate: "' + startDate + '", endDate: "' + endDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == "first") {
                InitializeGrid(m_MsgData);
            }
            if (myLoadType == "last") {
                $('#gridMain_ReportTemplate').treegrid('loadData', m_MsgData);
            }
        },
        error: handleError
    });
}