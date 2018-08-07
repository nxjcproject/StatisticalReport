var GridObjContainerIdArray = new Array();
var GridObjArray = new Array();
var ChartObjContainerIdArray = new Array();
var ChartObjArray = new Array();

function GetChart(myGridObjContainerId, myChartObjContainerId, myData, myChartType, myTitle) {
    if (myGridObjContainerId != null && myGridObjContainerId != undefined && myGridObjContainerId != "") {
        ///////////////////////////////显示datagrid/////////////////////////
        GetGridObjById(myGridObjContainerId, myData, myTitle);
    }
    if (myChartObjContainerId != null && myChartObjContainerId != undefined && myChartObjContainerId != "") {
        ///////////////////////////////////显示chart/////////////////////////////
        GetChartObjById(myChartObjContainerId, myData, myChartType, myTitle);
    }
}
/////////////////////////获得grid/////////////////////////
function GetGridObjById(myGridObjContainerId, myData, myTitle) {
    var m_ExistsIndex = FindArrayIndexByContainerId(GridObjContainerIdArray, myGridObjContainerId);
    if (m_ExistsIndex < 0) {
        ////////////////////////////////////////第一次加载/////////////////////////////////////////
        GridObjArray.push(InitializeGrid(myGridObjContainerId, myData));
        GridObjContainerIdArray.push(myGridObjContainerId);
    }
    else {
        /////////////////////////////////////////////已经加载过一次////////////////////////////////
        GridObjArray[m_ExistsIndex].datagrid('loadData', myData);
    }
}
/////////////////////////////获得chart/////////////////////////
function GetChartObjById(myChartObjContainerId, myData, myChartType, myTitle) {
    var m_ExistsIndex = FindArrayIndexByContainerId(ChartObjContainerIdArray, myChartObjContainerId);
    if (m_ExistsIndex < 0) {
        ////////////////////////////////////////第一次加载/////////////////////////////////////////
        var m_ChartObj = CreatChart(myChartObjContainerId, myData, myChartType, myTitle)
        ChartObjArray.push(m_ChartObj);
        ChartObjContainerIdArray.push(myChartObjContainerId);
    }
    else {
        /////////////////////////////////////////////已经加载过一次////////////////////////////////
        ReleaseChartObj(myChartObjContainerId);

        ChartObjArray.push(CreatChart(myChartObjContainerId, myData, myChartType, myTitle));
        ChartObjContainerIdArray.push(myChartObjContainerId);
    }
}
/////////////////在队列中查找是否该对象已经存在，如果存在返回在array中的索引index///////////////
function FindArrayIndexByContainerId(myContainerIdArray, myContainerId)
{
    var m_ExistsIndex = -1;
    for (var i = 0; i < myContainerIdArray.length; i++) {
        if (myContainerIdArray[i] == myContainerId) {
            m_ExistsIndex = i;
            break;
        }
    }
    return m_ExistsIndex;
}
function ReleaseChartObj(myChartContainerId) {
    var m_ChartExistsIndex = FindArrayIndexByContainerId(ChartObjContainerIdArray, myChartContainerId);
    if (m_ChartExistsIndex >= 0) {
        ReleasePlotChart(myChartContainerId, ChartObjArray[m_ChartExistsIndex]);
        ChartObjContainerIdArray.splice(m_ChartExistsIndex, 1);
        ChartObjArray.splice(m_ChartExistsIndex, 1);
    }
}
function ReleaseGridObj(myGridContainerId) {
    var m_GridExistsIndex = FindArrayIndexByContainerId(GridObjContainerIdArray, myGridContainerId);
    if (m_GridExistsIndex >= 0) {
        ReleaseGird(myGridContainerId, GridObjArray[m_GridExistsIndex]);
        GridObjContainerIdArray.splice(m_GridExistsIndex, 1);
        GridObjArray.splice(m_GridExistsIndex, 1);
    }
}
function ReleaseGridChartObj(myChartContainerId) {
    ReleaseChartObj(myChartContainerId + '_Chart');
    ReleaseGridObj(myChartContainerId + '_Grid');
}
function ReleaseAllGridChartObj(myChartContainerId) {
    for (var i = 0; i < ChartObjArray.length; i++) {
        ReleasePlotChart(myChartContainerId, ChartObjArray[i]);
    }
    ChartObjContainerIdArray.splice(0, ChartObjContainerIdArray.length);
    ChartObjArray.splice(0, ChartObjArray.length);

    for (var i = 0; i < GridObjArray.length; i++) {
        ReleaseGird(myChartContainerId, GridObjArray[i]);
    }
    GridObjContainerIdArray.splice(0, GridObjContainerIdArray.length);
    GridObjArray.splice(0, GridObjArray.length);
}
function ChangeSize(myContainerId) {
    var m_ContainerObj = $('#' + myContainerId);
    var m_GridHeight = parseInt(m_ContainerObj.height() * 0.4);
    $('#' + myContainerId + '_GridPanel').panel('resize', { height: m_GridHeight });
}
//////////////////////////////////////////增加Grid与Chart//////////////////////////
function CreateGridChart(myData, myContainerId, myIsShowGrid, myChartType) {
    var m_ContainerObj = $('#' + myContainerId);
    var m_WindowsHtml = ''
    if (myIsShowGrid == true) {
        var m_GridHeight = parseInt(m_ContainerObj.height() * 0.4);
        var m_ChartHeight = parseInt(m_ContainerObj.height() * 0.5);
        m_WindowsHtml = '<div class="easyui-layout" data-options="fit:true,border:false">' +
                '<div id="' + myContainerId + '_GridPanel" class="easyui-panel" data-options="region:\'north\',border:true, collapsible:false, split:false" style="height:' + m_GridHeight + 'px;">' +
		            '<table id="' + myContainerId + '_Grid" data-options="fit:true,border:false"></table>' +
	            '</div>' +
                '<div id = "' + myContainerId + '_Chart" class="easyui-panel" data-options="region:\'center\',border:false" style="margin-left:20px;width:100px; padding-right:110px;height:' + m_ChartHeight + 'px;padding-bottom:10px;">' +
	            '</div>' +
            '</div>';
        /*
        var m_GridOptions = {        
            id: myContainerId + "_Grid",
            region: "north",
            height: m_GridHeight,
            border:true, 
            collapsible:false, 
            split:false
        };

        $('#Windows_Container').layout('add', m_GridOptions);

        var m_ChartOptions = {
            id: myContainerId + "_Chart",
            region: "center",
            height: m_ChartHeight,
            border: true,
            collapsible: false,
            split: false
        };
        m_ContainerObj.layout('add', m_ChartOptions);
        */

        //$('#' + myContainerId + "_GridPanel").append("<table></table>");
    }
    else {
        m_WindowsHtml = '<div class="easyui-layout" data-options="fit:true,border:false">' +
                '<div id = "' + myContainerId + '_Chart" class="easyui-panel" data-options="region:\'center\',border:false,fit:true" style="margin-left:20px;padding-right:100px;padding-bottom:10px;">' +
	            '</div>' +
	        '</div>';

    }

    m_ContainerObj.append(m_WindowsHtml);
    $.parser.parse(m_ContainerObj);
    
    if (myIsShowGrid == true) {
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Line", "asdfasdfafd");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Bar", null);
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "MultiBar", "afsdasdfa");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", myData, "Pie", null);
        GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, null);
    }
    else {
        GetChart(null, myContainerId + "_Chart", myData, myChartType, null);
    }
}
///////////////////////////刷新该容器内GridChart///////////////////////////////
function RefreshGridChart(myData, myContainerId, myIsShowGrid, myChartType) {
    if (myIsShowGrid == true) {
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Line", "asdfasdfafd");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Bar", null);
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "MultiBar", "afsdasdfa");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", myData, "Pie", null);
        GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, null);
    }
    else {
        GetChart(null, myContainerId + "_Chart", myData, myChartType, null);
    }
}
