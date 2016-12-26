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
                                <td style="padding-top:5px;padding-left:10px">选择时间：</td>
                                <td style="padding-top:5px"><input id="dateTime" type="text" class="easyui-datebox" required="required" style="width:100px;"/></td>
                                <td style="padding-top:5px;"><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" 
                                        onclick="QueryReportFun();">查询</a></td>
                            </tr>

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
