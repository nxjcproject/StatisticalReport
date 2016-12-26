var SelectOrganizationName = "";
var SelectDatetime = "";
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
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "DailyBasicMaterialWeight.aspx/GetMaterialWeightDailyReport",
        data: '{organizationId: "' + organizationId + '", startDate: "' + startDate + '", endDate: "' + endDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            _data = m_MsgData;
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
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    SelectOrganizationName = $('#productLineName').textbox('getText');
    var organizationID = $('#organizationId').val();
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间
    SelectDatetime = startDate + ' 至 ' + endDate;
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
    if (node["OrganizationType"] == '分厂') {
        $('#productLineName').textbox('setText', node.text);
        $('#organizationId').val(node.OrganizationId);
    }
    else {
        alert("请选择到分厂!");
    }
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

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "DailyBasicMaterialWeight.aspx");

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
function PrintFileFun() {
    $.ajax({
        type: "POST",
        url: "DailyBasicMaterialWeight.aspx/PrintFile",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            PrintHtml(msg.d);
        }
    });
}