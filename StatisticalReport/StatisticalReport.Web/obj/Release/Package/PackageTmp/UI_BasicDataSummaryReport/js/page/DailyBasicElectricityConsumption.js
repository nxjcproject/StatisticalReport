﻿var SelectOrganizationName = "";
var SelectDatetime = "";
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
function GetShiftsSchedulingLog(organizationId, startDate, endDate) {
    var queryUrl = 'DailyBasicElectricityConsumption.aspx/GetShiftsSchedulingLog';
    var dataToSend = '{organizationId: "' + organizationId + '", startDate:"' + startDate + '", endDate:"' + endDate + '"}';
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            $('#dgShiftsScheduling').datagrid({
                data: jQuery.parseJSON(msg.d)
            });
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        }
    });
}


function loadGridData(organizationId, startDate, endDate, consumptionType) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "DailyBasicElectricityConsumption.aspx/GetData",
        data: '{organizationId: "' + organizationId + '", startTime: "' + startDate + '", endTime: "' + endDate + '", consumptionType: "' + consumptionType + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            InitializeGrid(m_MsgData);

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

function handleError() {
    $('#gridMain_ReportTemplate').treegrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    SelectOrganizationName = $('#productLineName').textbox('getText');
    var organizationId = $('#organizationId').val();
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间


    var consumptionType = $('#cbbConsumptionType').combobox('getValue');



    SelectDatetime = startDate + ' 至 ' + endDate;
    if (organizationId == "" || startDate == "" || endDate == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '开始时间不能大于结束时间！');
        return;
    }
    loadGridData(organizationId, startDate, endDate, consumptionType);
    GetShiftsSchedulingLog(organizationId, startDate, endDate);

}

function onOrganisationTreeClick(node) {
    if (node.OrganizationType == '分厂' || node.OrganizationType == '分公司') {
        $.messager.alert('警告', '请选择生产线结点');
        return;
    }
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
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
        treeField: "Name",

        toolbar: '#toolbar_ReportTemplate'
    });
}

function RefreshFun() {
    QueryReportFun();
}

function ShiftsSchedulingStyler(value, row, index) {
    if (value == "A班") {
        return 'background-color:#FF0000;';
    } else if (value == "B班") {
        return 'background-color:#00CD00;';
    } else if (value == "C班") {
        return 'background-color:#FFFF00;';
    } else if (value == "D班") {
        return 'background-color:#8470FF;';
    }
}



function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetTreeTableHtml("gridMain_ReportTemplate", "电耗报表", "Name", SelectOrganizationName, SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "DailyBasicElectricityConsumption.aspx");

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
}
function PrintFileFun() {
    var m_ReportTableHtml = GetTreeTableHtml("gridMain_ReportTemplate", "电耗报表", "Name", SelectOrganizationName, SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}