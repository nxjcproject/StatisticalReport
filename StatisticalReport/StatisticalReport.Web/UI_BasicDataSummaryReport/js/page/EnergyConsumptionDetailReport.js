var SelectOrganizationName = "";
var SelectDatetime = "";

$(function () {
    InitDate();
    loadDataGrid();
});

//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 2);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " 00:00:00";
    $('#startDate').datetimebox('setValue', beforeString);
    $('#endDate').datetimebox('setValue', nowString);
}

function onOrganisationTreeClick(node) {
    $('#productionLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
}

function loadDataGrid() {
    $('#grid_EnergyConsumptionDetailReport').treegrid({
        columns: [[
                { field: 'Name', title: '项目', width: 250, align: 'left' },
                { field: 'FormulaValue', title: '电量', width: 100, align: 'right' },
                { field: 'OutPut', title: '产量', width: 90, align: 'right' },
                { field: 'FormulaConsumption', title: '电耗', width: 90, align: 'right' },
        ]],       
        fit: true,
        toolbar: "#toolbar_EnergyConsumptionDetailReport",
        rownumbers: true,
        singleSelect: true,
        striped: true,
        idField: 'id',
        treeField: 'Name',
        data: []
    })
}

function RefreshFun() {
    QueryReportFun();
}

function QueryReportFun() {
    SelectOrganizationName = $('#productionLineName').textbox('getText');
    var mOrganizationId = $('#organizationId').val();
    var mStartDate = $('#startDate').datetimebox('getValue');//开始时间
    var mEndDate = $('#endDate').datetimebox('getValue');//结束时间
    var mChecked = document.getElementById("checked").checked;//是否包括设备
    SelectDatetime = mStartDate + ' 至 ' + mEndDate;
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "EnergyConsumptionDetailReport.aspx/GetEnergyConsumptionDetailReportData",
        data: "{mOrganizationId:'" + mOrganizationId + "', mStartDate:'" + mStartDate + "', mEndDate:'" + mEndDate + "', mChecked:'" + mChecked + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                $('#grid_EnergyConsumptionDetailReport').treegrid("loadData", []);
                $.messager.alert('提示', '没有查询到数据！');
            } else {
                $('#grid_EnergyConsumptionDetailReport').treegrid("loadData", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_EnergyConsumptionDetailReport").treegrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetTreeTableHtml("grid_EnergyConsumptionDetailReport", "能耗明细报表", "Name", SelectOrganizationName, SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "EnergyConsumptionDetailReport.aspx");

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
