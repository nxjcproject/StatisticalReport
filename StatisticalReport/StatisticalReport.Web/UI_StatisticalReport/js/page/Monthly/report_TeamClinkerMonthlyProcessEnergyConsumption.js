﻿$(function () {
    //var m_UserName = $('#HiddenField_UserName').val();
    //loadGridData('first');
    InitializeGrid('');
});

function loadGridData(myLoadType) {

    var organizationId = "df863854-89ae-46e6-80e8-96f6db6471b4";
    var datetime = "2014-10";
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "report_TeamClinkerMonthlyProcessEnergyConsumption.aspx/GetReportData",
        data: '{organizationId: "' + organizationId + '", datetime: "' + datetime + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (myLoadType == 'first') {
                m_MsgData = jQuery.parseJSON(msg.d);
                InitializeGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData['rows']);
            }
        }
    });
}
function InitializeGrid(myData) {

    $('#gridMain_ReportTemplate').datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: "field",
        //frozenColumns: [[m_IdAndNameColumn[1]]],
        columns: myData['columns'],
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        //pagination: true,
        singleSelect: true,
        //onClickCell: onClickCell,
        //idField: m_IdAndNameColumn[0].field,
        //pageSize: 20,
        //pageList: [20, 50, 100, 500],

        toolbar: '#toolbar_ReportTemplate'
    });

    //for(
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "report_TeamClinkerMonthlyProcessEnergyConsumption.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();

    /*
    var m_Parmaters = { "myFunctionName": m_FunctionName, "myParameter1": m_Parameter1, "myParameter2": m_Parameter2 };
    $.ajax({
        type: "POST",
        url: "Report_Example.aspx",
        data: m_Parmater,                       //'myFunctionName=' + m_FunctionName + '&myParameter1=' + m_Parameter1 + '&myParameter2=' + m_Parameter2,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "1") {
                alert("导出成功!");
            }
            else{
                alert(msg.d);
            }
        }
    });
    */
}
function RefreshFun() {
    loadGridData('last');
}
function PrintFileFun() {
    $.ajax({
        type: "POST",
        url: "report_TeamClinkerMonthlyProcessEnergyConsumption.aspx/PrintFile",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            PrintHtml(msg.d);
        }
    });


}

function QueryReportFun() {
    var organizationID = $('#organizationId').val();
    var datetime = $('#datetime').datetimebox('getValue').substr(0, 7);
    if (organizationID == "" || datetime == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }

    loadGridData('first');
}

// datetime spinner
function onOrganisationTreeClick(node) {
    $('#productLineName').val(node.text);
    $('#organizationId').val(node.OrganizationID);
}

$(function () {
    $('.combo-arrow').click(function () {
        $('.calendar-title > span').click();
        $('.calendar-menu-month').click(function () {
            $("tr.calendar-first > .calendar-last").click();
        });
    });
});