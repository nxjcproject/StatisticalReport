<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CoalConsumptionDailyReport.aspx.cs" Inherits="StatisticalReport.Web.UI_ComprehensiveDailyReport.CoalConsumptionDailyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>煤耗日报</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/UI_ComprehensiveDailyReport/js/page/CoalConsumptionDailyReport.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 图表开始 -->
        <div id="toolbar_ReportTemplate" style="display: none;">
            <table>
	            <tr>
	                <td>
		                <table>
	                        <tr>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshFun();">刷新</a>
                                </td>
                                <!--<td><div class="datagrid-btn-separator"></div>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>-->
                            </tr>
                        </table>
		            </td>
                </tr>
	        </table>
        </div>

        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
            <table id="gridMain_ReportTemplate" class="easyui-datagrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true" title="">
		        <thead>
			        <tr>
                        <th data-options="field:'VariableName',width:150">项目</th>
                        <th data-options="field:'FirstB',width:150">甲班</th>
                        <th data-options="field:'SecondB',width:150">乙班</th>
                        <th data-options="field:'ThirdB',width:150">丙班</th>
                        <th data-options="field:'TotalPeakValleyFlatB',width:150">合计</th>
			        </tr>
		        </thead>
            </table>
        </div>
        <!-- 图表结束 -->
    </div>

    <form id="form_Main" runat="server"></form>

</body>
</html>
