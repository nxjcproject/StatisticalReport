var editIndex = undefined;

$(document).ready(function () {
    $('#lbQuery').linkbutton('disable');
    $('#lbCalc').linkbutton('disable');
    $('#lbSave').linkbutton('disable');
    LoadCementYearlyPerUnitDistributionPowerConsumptionData('first');
});

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);

    $('#gridMain_ReportTemplate').datagrid('loadData', []);

    $('#lbQuery').linkbutton('enable');
    $('#lbCalc').linkbutton('disable');
    $('#lbSave').linkbutton('disable');
}


function QueryCementYearlyPerUnitDistributionPowerConsumptionFun() {
    endEditing();           //关闭正在编辑

    var organizationID = $('#organizationId').val();
    var datetime = $('#datetime').datetimespinner('getValue');
    if (organizationID == "" || datetime == "") {
        $.messager.alert('提示', '请选择生产线和时间');
        return;
    }

    LoadCementYearlyPerUnitDistributionPowerConsumptionData('last');
}

function LoadCementYearlyPerUnitDistributionPowerConsumptionData(myLoadType) {
    var organizationID = $('#organizationId').val();
    var clinkerOrganizationId = 'zc_nxjc_qtx_efc_clinker02';
    var datetime = $('#datetime').datetimespinner('getValue');

    $.messager.progress();

    $.ajax({
        type: "POST",
        url: "report_CementYearlyPerUnitDistributionPowerConsumption.aspx/GetCementYearlyPerUnitDistributionPowerConsumptionInfo",
        data: "{organizationId:'" + organizationID + "', clinkerOrganizationId:'" + clinkerOrganizationId + "', year:'" + datetime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeCementYearlyPerUnitDistributionPowerConsumptionGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData);

                $('#lbQuery').linkbutton('disable');
                $('#lbCalc').linkbutton('enable');
            }

            $.messager.progress('close');
        },
        error: function (msg) {
            $.messager.progress('close');
        }
    });
}
function RefreshCementYearlyPerUnitDistributionPowerConsumptionPlanFun() {
    QueryCementYearlyPerUnitDistributionPowerConsumptionPlanInfoFun();
}

// 计算
function CalculateCementYearlyPerUnitDistributionPowerConsumptionFun() {
    endEditing();           //关闭正在编辑

    var organizationID = $('#organizationId').val();
    var clinkerOrganizationId = 'zc_nxjc_qtx_efc_clinker02';
    var datetime = $('#datetime').datetimespinner('getValue');

    $.messager.progress();

    var m_DataGridData = $('#gridMain_ReportTemplate').datagrid('getData');
    if (m_DataGridData['rows'].length > 0) {
        var m_DataGridDataJson = '{"rows":' + JSON.stringify(m_DataGridData['rows']) + '}';
        $.ajax({
            type: "POST",
            url: "report_CementYearlyPerUnitDistributionPowerConsumption.aspx/CalculateCementYearlyPerUnitDistributionPowerConsumption",
            data: "{organizationId:'" + organizationID + "', clinkerOrganizationId:'" + clinkerOrganizationId + "',year:'" + datetime + "',gridData:'" + m_DataGridDataJson + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData);

                $('#lbSave').linkbutton('enable');

                $.messager.progress('close');
            },
            error: function (msg) {
                $.messager.progress('close');
            }
        });
    }
    else {
        alert("请先读入能耗原始数据！");
    }
}

// 保存
function SaveCementYearlyPerUnitDistributionPowerConsumptionFun() {
    endEditing();           //关闭正在编辑

    var organizationID = $('#organizationId').val();
    var clinkerOrganizationId = 'zc_nxjc_qtx_efc_clinker02';
    var datetime = $('#datetime').datetimespinner('getValue');

    $.messager.progress();

    var m_DataGridData = $('#gridMain_ReportTemplate').datagrid('getData');
    if (m_DataGridData['rows'].length > 0) {
        var m_DataGridDataJson = '{"rows":' + JSON.stringify(m_DataGridData['rows']) + '}';
        $.ajax({
            type: "POST",
            url: "report_CementYearlyPerUnitDistributionPowerConsumption.aspx/SaveCementYearlyPerUnitDistributionPowerConsumption",
            data: "{organizationId:'" + organizationID + "', clinkerOrganizationId:'" + clinkerOrganizationId + "',year:'" + datetime + "',gridData:'" + m_DataGridDataJson + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);
                $.messager.progress('close');
                $.messager.alert("提示", m_MsgData.result);
            },
            error: function (msg) {
                $.messager.progress('close');
            }
        });
    }
    else {
        alert("无可以保存的数据！");
    }
}

//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializeCementYearlyPerUnitDistributionPowerConsumptionGrid(myData) {

    var m_IdColumn = myData['columns'].splice(0, 1);

    $('#gridMain_ReportTemplate').datagrid({
        title: '',
        //data: myData,
        dataType: "json",
        striped: true,
        idField: m_IdColumn[0].field,
        columns: [myData['columns']],
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        singleSelect: true,
        onClickCell: onClickCell,

        toolbar: '#toolbar_ReportTemplate'
    });
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#gridMain_ReportTemplate').datagrid('validateRow', editIndex)) {
        $('#gridMain_ReportTemplate').datagrid('endEdit', editIndex);

        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onClickCell(index, field) {
    if (endEditing()) {
        $('#gridMain_ReportTemplate').datagrid('selectRow', index).datagrid('editCell', { index: index, field: field });
        editIndex = index;
    }
}


// datetime spinner
function formatter2(date) {
    if (!date) { return ''; }
    var y = date.getFullYear();
    return y + '';
}
function parser2(s) {
    if (!s) { return null; }
    if (!isNaN(s)) {
        return new Date(s, 0, 1);
    } else {
        return new Date();
    }
}