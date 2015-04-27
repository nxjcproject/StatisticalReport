
var _data = "";

$(function () {
    InitDate();
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
function loadGridData(myLoadType, organizationId, startDate,endDate) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "DailyBasicMaterialWeight.aspx/GetMaterialWeightDailyReport",
        data: '{organizationId: "' + organizationId + '", startDate: "' + startDate + '", endDate: "' + endDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            _data = m_MsgData;
            InitializeGrid(m_MsgData);

        },
        error: handleError
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    var organizationID = $('#organizationId').val();
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间
    if (organizationID == "" || startDate == "" || endDate=="") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }
    loadGridData('first', organizationID, startDate, endDate);
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
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

function RefreshFun() {
    QueryReportFun();
}