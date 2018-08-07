<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ElectricityConsumptionDailyReport.aspx.cs" Inherits="StatisticalReport.Web.UI_ComprehensiveDailyReport.ElectricityConsumptionDailyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>电耗日报</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/UI_ComprehensiveDailyReport/js/page/ElectricityConsumptionDailyReport.js"></script>
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
                                <td style="width: 60px; text-align: right;">选择时间</td>
                                <td><input id="dateTime" type="text" class="easyui-datebox" required="required" style="width:100px;"/></td>
                                <td style="width: 10px;"></td>
                                <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" 
                                        onclick="QueryReportFun();">查询</a></td>
                            </tr>
	                        <%--<tr>                             
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshFun();">刷新</a>
                                </td>
                                <td><div class="datagrid-btn-separator"></div>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>
                            </tr>--%>
                        </table>
		            </td>
                </tr>
	        </table>
        </div>

        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
            <table id="gridMain_ReportTemplate" class="easyui-treegrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true,fit:true" title="">
		        <thead>
			        <tr>
                        <th data-options="field:'VariableName',width:330">项目</th>
                        <th data-options="field:'FirstB',width:80">甲班</th>
                        <th data-options="field:'SecondB',width:80">乙班</th>
                        <th data-options="field:'ThirdB',width:80">丙班</th>
                        <th data-options="field:'TotalPeakValleyFlatB',width:80">合计</th>
			        </tr>
		        </thead>
            </table>
        </div>
        <!-- 图表结束 -->
    </div>

    <form id="form_Main" runat="server"></form>

</body>
</html>
