
$(function () {
    //var m_UserName = $('#HiddenField_UserName').val();
    //loadGridData('first');
    InitDate();
    InitializeGrid('');
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setHours(nowDate.getHours()-1);
    var beforeDate = new Date();
    beforeDate.setMonth(beforeDate.getMonth() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " "+nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate()+" 0:00:00";
    $('#starttime').datetimebox('setValue', beforeString);
    $('#endtime').datetimebox('setValue', nowString);
}

function loadGridData(myLoadType, startTime, endTime) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    var electricRoom = $('#ElectricRoom').combobox('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "ElectricRoomMeterReading.aspx/GetReportData",
        data: '{electricRoom: "' + electricRoom + '", startTime: "' + startTime+ '", endTime: "' + endTime+'"}',
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
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData['rows']);
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
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function InitializeGrid(myData) {
    $('#gridMain_ReportTemplate').datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: "field",
        columns: myData['columns'],
        rownumbers: true,
        singleSelect: true,
        toolbar: '#toolbar_ReportTemplate'
    });

}
//获得电气室
function InitElectricRoom(organizationId) {
    $.ajax({
        type: "POST",
        url: "ElectricRoomMeterReading.aspx/GetElectricRoomInfo",
        data: '{organizationId: "' + organizationId  + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#ElectricRoom').combobox({
                    data:m_MsgData.rows,
                    valueField:'ElectricRoom',
                    textField: 'ElectricRoomName',
                    onShowPanel: function () {
                        var orgCount = m_MsgData.rows.length;
                        // 动态调整高度  
                        if (orgCount < 16) {
                            $(this).combobox('panel').height("auto");
                        } else {
                            $(this).combobox('panel').height(300);
                        }
                    }
                });                    
        },
        error: handleError
    });
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "ElectricRoomMeterReading.aspx");

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
        url: "ElectricRoomMeterReading.aspx/PrintFile",
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
    var electricRoom = $('#ElectricRoom').combobox('getValue');
    var startTime = $('#starttime').datebox('getValue');
    var endTime = $('#endtime').datebox('getValue');
    if (organizationID == "" || electricRoom == "" || startTime == "" || endTime == "") {
        $.messager.alert('警告', '请选择生产线,电气室和时间');
        return;
    }
    if (startTime > endTime) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }

    loadGridData('first', startTime, endTime);
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    InitElectricRoom(node.OrganizationId);
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
