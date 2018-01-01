<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CoalUsageDailyReport.aspx.cs" Inherits="StatisticalReport.Web.UI_ComprehensiveDailyReport.CoalUsageDailyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>用煤量日报</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/UI_ComprehensiveDailyReport/js/page/CoalUsageDailyReport.js"></script>
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
                                <td><input type="radio" name="timeType" value="day" checked="checked" onclick="DataBoxHide()"/>日报</td>
                                <td><input type="radio" name="timeType" value=" custom" onclick="DataBoxShow()"/>自定义时间</td>
                                <td><div class="datagrid-btn-separator"></div></td>
                                <td style="width: 60px; text-align: right;">开始时间</td>
                                <td style="padding-top:5px"><input id="startDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/></td>
                                <td class="mDisplay" style="display:none; width:60px; text-align:right;">结束时间</td>
                                <td class="mDisplay" style="display:none;"><input id="endDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/></td>
                                <td style="padding-top:5px;"><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" 
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
            <table id="gridMain_ReportTemplate" class="easyui-datagrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true,fit:true" title="">
		        <thead>
			        <tr>                        
                        <th data-options="field:'CompanyName',width:100">公司名称</th>
                        <th data-options="field:'FactoryName',width:100">分厂名称</th>
                        <th data-options="field:'VariableName',width:150">项目</th>
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
