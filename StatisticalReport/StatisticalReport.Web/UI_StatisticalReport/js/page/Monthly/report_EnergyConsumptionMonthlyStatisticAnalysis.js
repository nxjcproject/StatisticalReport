$(function () {
    //var m_UserName = $('#HiddenField_UserName').val();
    //loadGridData('first');
    var reportType = '';
    InitializeGrid('');
});

function loadGridData(myLoadType, organizationId, datetime) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "report_EnergyConsumptionMonthlyStatisticAnalysis.aspx/GetReportData",
        data: '{organizationId: "' + organizationId + '", datetime: "' + datetime + '",reportType:"' + reportType + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            if (myLoadType == 'first') {
                m_MsgData = jQuery.parseJSON(msg.d);
                InitializeGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#gridMain_ReportTemplate').treegrid('loadData', m_MsgData['rows']);
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

function handleError() {
    $('#gridMain_ReportTemplate').treegrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function InitializeGrid(myData) {
    var length = 60;
    $('#gridMain_ReportTemplate').treegrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,//设置为 true，则把行条纹化。（即奇偶行使用不同背景色）
        idField: "id",//指示哪个字段是标识字段。
        treeField: "Name",
        //frozenColumns: [[m_IdAndNameColumn[1]]],
        columns: [[
                    { field: 'Name', title: '名称', width: 150, rowspan: 2 },
                    { title: '用电量(KWh)', width: 3 * length, colspan: 3 },
                    { field: 'Consumption_CoalDust', title: '用煤量', width: length, rowspan: 2 },
                    { title: '产量', width: 3*length+20, colspan: 4 },
                    { title: '电耗(KWh/t)', width: 3 * length, colspan: 3 },
                    { field: 'ComprehensiveElectricityConsumption', title: '吨熟料综合电耗(KWh/t)', width: 2.2* length, rowspan: 2 },
                    { field: 'ComprehensiveCoalConsumption', title: '吨熟料实物煤耗(kg/t)', width: 2.2 * length, rowspan: 2 },
                    { field: 'ComprehensiveElectricityOutput', title: '吨熟料发电量(KWh/t)', width: 2.2 * length, rowspan: 2 }
                 ],
                [
                    { field: 'Electricity_RawBatch', title: '生料制备', width: length },
                    { field: 'Electricity_Clinker', title: '熟料烧成', width: length },
                    { field: 'Electricity_Cement', title: '水泥制备', width: length },
                    { field: 'Output_RawBatch', title: '生料', width: length },
                    { field: 'Output_Clinker', title: '熟料', width: length },
                    { field: 'Output_Cement', title: '水泥', width: length },
                    { field: 'Output_Cogeneration', title: '发电量(KWh)', width: length+20 },
                    { field: 'ElectricityConsumption_RawBatch', title: '生料制备', width: length },
                    { field: 'ElectricityConsumption_Clinker', title: '熟料烧成', width: length },
                    { field: 'ElectricityConsumption_Cement', title: '水泥制备', width: length },
                ]],
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
    form.attr('action', "report_CementMilMonthlyEnergyConsumption.aspx");

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
        url: "report_CementMilMonthlyEnergyConsumption.aspx/PrintFile",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            PrintHtml(msg.d);
        }
    });


}

function QueryReportFun() {
    reportType = $('input[name="reportType"]:checked').val();
    var organizationID = $('#organizationId').val();
    var datetime = $('#datetime').datetimespinner('getValue');
    var productLevel=$('#productLineType').val();
    if (organizationID == "" || datetime == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (7 == productLevel.length) {
        $.messager.alert("警告","请选择分厂及以上节点！");
        return;
    }
    loadGridData('first', organizationID, datetime);
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    $('#productLineType').val(node.id);
}

// datetime spinner
function formatter2(date) {
    if (!date) { return ''; }
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    return y + '-' + (m < 10 ? ('0' + m) : m);
}
function parser2(s) {
    if (!s) { return null; }
    var ss = s.split('-');
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    if (!isNaN(y) && !isNaN(m)) {
        return new Date(y, m - 1, 1);
    } else {
        return new Date();
    }
}

//$(function () {
//    $('.combo-arrow').click(function () {
//        $('.calendar-title > span').click();
//        $('.calendar-menu-month').click(function () {
//            $("tr.calendar-first > .calendar-last").click();
//        });
//    });
//});
