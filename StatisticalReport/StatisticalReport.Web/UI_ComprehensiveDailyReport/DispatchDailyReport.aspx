<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchDailyReport.aspx.cs" Inherits="StatisticalReport.Web.UI_ComprehensiveDailyReport.DispatchDailyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调度日报</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <%--<link type="text/css" rel="stylesheet" href="/UI_ComprehensiveDailyReport/css/page/DispatchDailyReport.css" />--%>

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>
    <!--<script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shCore.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushJScript.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushXml.min.js"></script>-->

    <!-- Additional plugins go here -->
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.barRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pieRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pointLabels.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.cursor.min.js"></script>

    <%--<script type="text/javascript" src="/lib/pllib/themes/jquery.jqplot.js"></script>
    <script type="text/javascript" src="/lib/pllib/themes/jjquery.jqplot.min.js"></script>--%>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>
    <script type="text/javascript" src="/UI_ComprehensiveDailyReport/js/page/DispatchDailyReport.js"></script>
</head>
<body class="easyui-layout" >
   <%-- <div data-options="region:'north',border:true" style="height:30px;padding-top:2px;">
        <span style="padding-left:10px;">时间：</span><span><input id="dd" type="text" class="easyui-datebox" style="width:150px;"/></span>
    </div>--%>
    <div data-options="region:'center',border:true" >
        <div class="easyui-layout" data-options="fit:true,border:true">
            <div id="completeGridContainId" data-options="region:'center',border:true">
                <div id="tools"><input id="dateTime" type="text" class="easyui-datebox" required="required" />
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                    onclick="QueryReportFun();">查询</a>
                </div>
                <table  id="completeGridId" class="easyui-treegrid" data-options="idField:'id',treeField:'Name',rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick" title="">
                    <thead>
                        <tr>
                            <th data-options="field:'Name',width:120">名称</th>
                            <th data-options="field:'clinker_ClinkerOutput',width:70">熟料产量(t)</th>
                            <th data-options="field:'cement_CementOutput',width:70">水泥产量(t)</th>
                            <th data-options="field:'clinker_PulverizedCoalOutput',width:70">煤粉产量(t)</th>
                            <th data-options="field:'clinker_PulverizedCoalInput',width:82">煤粉消耗量(t)</th>
                            <th data-options="field:'clinker_KilnHeadPulverizedCoalInput',width:105">窑头煤粉消耗量(t)</th>
                            <th data-options="field:'clinker_KilnTailPulverizedCoalInput',width:105">窑尾煤粉消耗量(t)</th>                           
                            <th data-options="field:'clinker_MixtureMaterialsOutput',width:70">生料产量(t)</th>
                            <th data-options="field:'clinker_LimestoneInput',width:82">石灰石消耗(t)</th>
                            <th data-options="field:'clinker_ElectricityQuantity',width:80">熟料电量(KWH)</th>
                            <th data-options="field:'cementmill_ElectricityQuantity',width:100">水泥磨电量(KWH)</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div id="PlanAndCompleteChartId" data-options="region:'south',border:true" style="height: 250px;">
                <div id="chartWindow" class="easyui-window" title="Custom Window Tools" data-options="iconCls:'icon-save',minimizable:false,onMaximize:updateWindowChart,onRestore:updateWindowChart,tools:'#tt'">
                </div>
                <div id="imageContainId"></div>
                <div id="tt">
                    <a href="javascript:void(0)" class="ext-icon-picture_save"  onclick="chartToImage();"></a>
                </div>
            </div>
        </div>
    </div>
    <div data-options="region:'east',border:true" style="width: 550px;">
        <div class="easyui-layout" data-options="fit:true,border:true">
            <div data-options="region:'center',border:true">
                <fieldset>
                    <legend id="legentId">完成情况</legend>
                    <table class="table" id="GapTableId" style="width: 100%;">
                        <tr>
                            <th>项目指标</th>
                            <th>日完成</th>
                            <th>月计划</th>
                            <th>月累计情况</th>
                            <th>差值</th>
                        </tr>
                        <tr>
                            <td>熟料产量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>发电量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>吨熟料发电量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>熟料电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>熟料煤耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>生料磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>煤磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥产量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
            </div>
            <div id="AlarmId" data-options="region:'south',border:true" style="height: 250px;">
                <table id="AlarmContainId">
                    <tr>
                        <th class="alarmTitle" style="text-align: center">能源报警</th>
                        <th class="alarmTitle" style="text-align: center">停机报警</th>
                    </tr>
                    <tr>
                        <td>
                            <div id="EnergyAlarmId"></div>
                        </td>
                        <td>
                            <div id="MachineHaltAlarmId"></div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

</body>
</html>
