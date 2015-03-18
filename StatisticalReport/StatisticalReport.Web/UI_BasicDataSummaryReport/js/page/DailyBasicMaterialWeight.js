
var _data = "";
function loadGridData(myLoadType, organizationId, datetime) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "DailyBasicMaterialWeight.aspx/GetMaterialWeightDailyReport",
        data: '{organizationId: "' + organizationId + '", datetime: "' + datetime + '"}',
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
    var datetime = $('#datetime').datetimespinner('getValue');
    if (organizationID == "" || datetime == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }

    loadGridData('first', organizationID, datetime);
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