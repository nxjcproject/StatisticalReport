var SelectOrganizationId = "";
var SelectOrganizationName = "";
var SelectDatetime = "";
$(document).ready(function () {
    InitialDate()
});
function InitialDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 30);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    $('#StartTime').datetimebox('setValue', beforeString);
    $('#EndTime').datetimebox('setValue', nowString);
}

function onOrganisationTreeClick(node) {
    if (node.OrganizationType == '分公司') {
        $.messager.alert('警告', '请选择生产区域节点');
        return;
    }
    $('#productLineName').textbox('setText', node.text);
    SelectOrganizationId = node.OrganizationId;
    loadVariableComboboxData();
}
function loadVariableComboboxData() {
    var queryUrl = 'LightLoadOperationDaily.aspx/GetVariableInfo';
    var dataToSend = '{myOrganizationId: "' + SelectOrganizationId + '"}';
    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d)['rows'];
            var m_ResultData = [];
            m_ResultData.push({ "id": "All", "text": "全部" });
            if (m_MsgData != undefined && m_MsgData != null && m_MsgData.length > 0)
            {
                for (var i = 0; i < m_MsgData.length; i++) {
                    m_ResultData.push(m_MsgData[i]);
                }
            }
            $('#Commbobox_VariableId').combobox('loadData', m_ResultData);
            $('#Commbobox_VariableId').combobox("setValue", m_ResultData[0].id);
        }
    });
}
function QueryReportFun() {
    var m_VariableId =  $('#Commbobox_VariableId').combobox('getValue');
    var m_StartTime = $('#StartTime').combobox('getValue');
    var m_EndTime = $('#EndTime').combobox('getValue');

    SelectOrganizationName = $('#productLineName').textbox('getText');
    SelectDatetime = m_StartTime + "至" + m_EndTime;

    var queryUrl = 'LightLoadOperationDaily.aspx/GetLightLoadData';
    var dataToSend = "{myOrganizationId:'" + SelectOrganizationId + "',myVariableId:'" + m_VariableId + "',myStartTime:'" + m_StartTime + "',myEndTime:'" + m_EndTime + "'}";
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
            var m_MsgData = jQuery.parseJSON(msg.d);
            try
            {
                $('#grid_LightLoadOperation').treegrid("loadData", m_MsgData);
            }
            catch (err) {

            }
            finally {
                $.messager.progress('close');
            }
        },
        error: function () {
            $.messager.progress('close');
            $('#grid_LightLoadOperation').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}

function ExportFileFun() {

    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetTreeTableHtml("grid_LightLoadOperation", "设备低负荷运转统计报表", "VariableDescription", SelectOrganizationName, SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('accept-charset', 'UTF-8');
    form.attr('action', "LightLoadOperationDaily.aspx");

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