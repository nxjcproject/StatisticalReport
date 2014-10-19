<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_ClinkerMonthlyPeakerValleyFlatElectricityConsumption.aspx.cs" Inherits="StatisticalReport.Web.UI_StatisticalReport.Monthly.report_ClinkerMonthlyPeakerValleyFlatElectricityConsumption" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>孰料生产(峰谷平)用电月统计分析</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery-1.8.3.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint-0.3.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="../js/page/Monthly/report_ClinkerMonthlyPeakerValleyFlatElectricityConsumption.js" charset="utf-8"></script>
</head>
<body>
    <div  class="easyui-layout" data-options="fit:true,border:false">
        <div id="toolbar_ReportTemplate" style="display: none;">
            <table>
                <tr>
	                <td>
		                <table>
			                <tr>
				                <td>查询条件1</td>
		                        <td><input id="userName" style="width: 100px;" /></td>
				                <td>查询条件2</td>
				                <td><input id="roleName" style="width: 100px;" /></td>
				                <td>查询条件3</td>
				                <td><input id="deptName" style="width: 100px;"/></td>
				                <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" 
                                        onclick="QueryReportFun();">查询</a>
                                </td>
			                </tr>
			            </table>
		            </td>
	            </tr>
	            <tr>
	                <td>
		                <table>
	                        <tr>
			                    <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-table_add',plain:true" onclick="addRowFun();">添加</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-table_row_insert',plain:true" onclick="InsertRowFun();">插入</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="saveRowsFun();">保存</a>
                                </td>
			                    <td><div class="datagrid-btn-separator"></div>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" iconCls="icon-reload" plain="true" onclick="RefreshFun();">刷新</a>
                                </td>
                                <td><div class="datagrid-btn-separator"></div>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>
                            </tr>
                        </table>
		            </td>
                </tr>
	        </table>
        </div>
        <div data-options="region:'center', fit:true,border:false">
            <table id="gridMain_ReportTemplate" data-options="fit:true,border:false"></table>
        </div>
    </div>
    <form id="formMain" runat="server" target ="_blank" method ="post">
        <div>
            <asp:HiddenField ID="HiddenField_UserName" runat="server" />
        </div>
    </form>
</body>
</html>
