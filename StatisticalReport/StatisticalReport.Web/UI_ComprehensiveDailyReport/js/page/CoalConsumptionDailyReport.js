
$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);
    loadGridData("first");
});

function loadGridData(myLoadType) {
    var m_MsgData;
    var date = $("#dateTime").datebox('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CoalConsumptionDailyReport.aspx/GetCoalConsumptionDailyReport",
        data: '{dateTime: "' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == "first") {
                InitializeGrid(m_MsgData);
            }
            if(myLoadType=="last"){
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
    loadGridData("last");
}