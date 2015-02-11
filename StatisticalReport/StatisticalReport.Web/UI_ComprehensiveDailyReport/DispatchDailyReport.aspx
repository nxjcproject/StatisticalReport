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
    <link type="text/css" rel="stylesheet" href="/UI_ComprehensiveDailyReport/css/page/DispatchDailyReport.css" />

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

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>
    <script type="text/javascript" src="/UI_ComprehensiveDailyReport/js/page/DispatchDailyReport.js"></script>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',border:true">
        <div class="easyui-layout" data-options="fit:true,border:true">
            <div  data-options="region:'center',border:true">
                <table id="completeGridId" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick" title="">
                    <thead>
                        <tr>
                            <th data-options="field:'公司',width:70">公司</th>
                            <th data-options="field:'熟料产量',width:70">熟料产量</th>
                            <th data-options="field:'发电量',width:70">发电量</th>
                            <th data-options="field:'吨熟料发电量',width:80">吨熟料发电量</th>
                            <th data-options="field:'熟料电耗',width:70">熟料电耗</th>
                            <th data-options="field:'熟料煤耗',width:70">熟料煤耗</th>
                            <th data-options="field:'生料磨电耗',width:70">生料磨电耗</th>
                            <th data-options="field:'煤磨电耗',width:70">煤磨电耗</th>
                            <th data-options="field:'水泥产量',width:70">水泥产量</th>
                            <th data-options="field:'水泥电耗',width:70">水泥电耗</th>
                            <th data-options="field:'水泥磨电耗',width:70">水泥磨电耗</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div id="PlanAndCompleteChartId" data-options="region:'south',border:true" style="height: 270px;">
            </div>
        </div>
    </div>
    <div data-options="region:'east',border:true" style="width: 550px;">
        <div class="easyui-layout" data-options="fit:true,border:true">
            <div data-options="region:'center',border:true" >
                <fieldset>
                    <legend id="legentId">完成情况</legend>
                    <table class="table" id="GapTableId" style="width: 100%;">
                        <tr>
                            <th>项目指标</th>
                            <th>日平均计划</th>
                            <th>日平均完成情况</th>
                            <th>差值</th>
                        </tr>
                        <tr>
                            <td>熟料产量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>发电量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>吨熟料发电量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>熟料电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>熟料煤耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>生料磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>煤磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥产量</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>水泥磨电耗</td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
            </div>
            <div id="AlarmId" data-options="region:'south',border:true" style="height: 270px;">
                <table id="AlarmContainId" ">
                    <tr>
                        <th class="alarmTitle" style="text-align:center">能源报警</th>
                        <th class="alarmTitle" style="text-align:center">停机报警</th>
                    </tr>
                    <tr>
                        <td  ><div id="EnergyAlarmId"></div></td>
                        <td  ><div id="MachineHaltAlarmId"></div></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</body>
</html>
