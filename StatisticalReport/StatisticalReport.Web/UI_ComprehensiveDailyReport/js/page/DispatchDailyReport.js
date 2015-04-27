var companyName = '';
var firstCompanyName = '';
var chartData = '';
var date = '';
$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);

    var m_width = $('#AlarmId').width() ;
    var m_height = $('#AlarmId').height();
    $('#AlarmContainId').height(m_height).width(m_width);
    $('#EnergyAlarmId').height(m_height - 25).width(m_width / 2 - 15);
    $('#MachineHaltAlarmId').height(m_height - 25).width(m_width / 2 - 15);
    InitChartWindows();
    loadFisrtCompanyData();
});

function loadFisrtCompanyData() {
    date = $('#dateTime').datebox('getValue');//datebox('setValue', startDate);
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "DispatchDailyReport.aspx/GetComplete",
        data: '{date:"'+date+'"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            //取出第一行公司的名称
            firstCompanyName = m_MsgData[0]['Name'];
            $('#completeGridId').treegrid('loadData', m_MsgData);
            $('#completeGridId').treegrid('collapseAll');
            $('#completeGridId').treegrid({
                toolbar: '#tools'
            });//全部折叠
            //根据第一行公司名称加载计划和完成情况模块和差值模块
            InitChart(firstCompanyName);
        },
        error: handleError
    });
    $.ajax({
        type: "POST",
        url: "DispatchDailyReport.aspx/GetEnergyAlarm",
        data: '{date:"'+date+'"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            updateEnergyAlarmChart(m_MsgData);
        },
        error: handleError
    });
    $.ajax({
        type: "POST",
        url: "DispatchDailyReport.aspx/GetMachineHaltAlarm",
        data: '{date:"' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            updateMachineHaltAlarmChart(m_MsgData);
        },
        error: handleError
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}
function updateEnergyAlarmChart(data) {
    CreateGridChart(data, 'EnergyAlarmId', false, 'Pie');
}

function updateMachineHaltAlarmChart(data) {
    CreateGridChart(data, 'MachineHaltAlarmId', false, 'Pie');
}

function onRowDblClick(rowData) {
    date = $('#dateTime').datebox('getValue');
    companyName = rowData["Name"];
    $('#legentId').html(companyName);
    InitChart(companyName);   
}
function InitChart(companyName) {
    $('#legentId').html(companyName);
    $.ajax({
        type: "POST",
        url: "DispatchDailyReport.aspx/GetPlanAndCompelete",
        data: "{date:'"+date+"',companyName:'" + companyName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = JSON.parse(msg.d);
            chartData = data;
            updateChart(data);
        },
        error: handleError
    });
    $.ajax({
        type: "POST",
        url: "DispatchDailyReport.aspx/GetGapPlanAndComplete",
        data: "{date:'" + date + "',companyName:'" + companyName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = JSON.parse(msg.d);
            updateTable(data);
        },
        error: handleError
    });
}


function updateChart(data) {
    CreateGridChart(data, 'chartWindow', false, 'Bar');
}

function updateTable(data) {
    // 详细计数的HTML

    var str = '';
    // 总计的数目

    var total = 0;

    // 遍历data中的元素
    var array = new Array("项目指标", "日完成","月计划", "月累计完成", "月差值");
    str = '<tr><th>项目指标</th><th>日完成</th><th>月计划</th><th>月累计完成</th><th>月差值</th></tr>';
    for (var i = 0; i < data.total; i++) {
        // 生成表格元素
        str += '<tr>'
        for (var j = 0; j <= 4;j++)
        {
            str += '<td>' + data.rows[i][array[j]] + '</td>';
        }
        str += '</tr>';
        //str += '<tr><td>' + data.rows[i]["项目指标"] + '</td><td>' + data.rows[i]["日平均计划"] + data.rows[i]["日平均完成"] + '</td><td>' + data.rows[i]["差值"] +'</td></tr>';
    }

    // 如果返回记录为空，则生成空的提示

    if (data.total == 0) {
        str = '<tr><td>无记录</td></tr>';
        total = 0;
    }

    // 更新占位符
    $('#GapTableId').html(str);
}

///////////////////////获取window初始位置////////////////////////////
function GetWindowPostion(myWindowIndex, myWindowContainerId) {
    var m_ParentObj = $('#' + myWindowContainerId);
    var m_ParentWidth = m_ParentObj.width();
    var m_ParentHeight = m_ParentObj.height();
    var m_ZeroLeft = 0;
    var m_ZeroTop = 0;
    var m_Padding = 5;
    var m_Width = (m_ParentWidth - m_Padding) / 2;
    var m_Height = (m_ParentHeight - m_Padding) / 2;
    var m_Left = 0;
    var m_Top = 0;
    if (myWindowIndex == 0) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 1) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 2) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }
    else if (myWindowIndex == 3) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }

    return [m_Width, m_Height, m_Left, m_Top]
}

///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myData, myContainerId, myIsShowGrid, myChartType, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized) {
    ;
    var m_WindowId = OpenWindows(myContainerId, '数据分析', myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized); //弹出windows
    var m_WindowObj = $('#' + m_WindowId);
    if (myMaximized != true) {
        CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);               //生成图表
    }

    m_WindowObj.window({
        onBeforeClose: function () {
            ///////////////////////释放图形空间///////////////
            //var m_ContainerId = GetWindowIdByObj($(this));
            ReleaseGridChartObj(m_WindowId);
            CloseWindow($(this))
        },
        onMaximize: function () {
            TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);

        },
        onRestore: function () {
            //TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);
        }
    });
}

function InitChartWindows() {
    var m_top = $('#completeGridContainId').height()+3;
    var m_width = $('#PlanAndCompleteChartId').width();
    var m_height = $('#PlanAndCompleteChartId').height();
    $('#chartWindow').window({
        title:'计划与完成对比',
        width: m_width,
        height: m_height,
        left: 0,
        top: m_top,
        collapsible: false,
        minimizable: false,
        resizable: false,
        inline: true,
        draggable: false,
        maximizable: true,
        maximized: false,
        iconCls: 'ext-icon-chart_bar',
        padding: 10,
        closable:false
    });   
}

function updateWindowChart() {
    //alert("");
    $('#chartWindow').empty();
    CreateGridChart(chartData, 'chartWindow', false, 'Bar');
   
}
//将chart转化为图片
function chartToImage() {
    //alert("");
    var j = $('#chartWindow_Chart').jqplotToImageElem();
    $('#chartWindow').empty();
    $('#chartWindow').append(j);
}

function QueryReportFun() {
    loadFisrtCompanyData();
}