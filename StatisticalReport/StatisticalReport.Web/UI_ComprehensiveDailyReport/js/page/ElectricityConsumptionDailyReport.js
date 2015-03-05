﻿
$(document).ready(function () {
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "ElectricityConsumptionDailyReport.aspx/GetElectricityConsumptionDailyReport",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            InitializeGrid(m_MsgData);
            //$('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData['rows']);
        },
        error: handleError
    });
});


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