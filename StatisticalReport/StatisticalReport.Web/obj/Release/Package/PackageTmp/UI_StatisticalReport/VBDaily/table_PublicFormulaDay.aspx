<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="table_PublicFormulaDay.aspx.cs" Inherits="StatisticalReport.Web.UI_StatisticalReport.VBDaily.table_PublicFormulaDay" %>
<%@ Register Src="../../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>公用工序峰谷平用电统计表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="../js/page/VBDaily/table_PublicFormulaDay.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width:150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div id="toolbar_ReportTemplate" style="display: none;">
            <table>
                <tr>
	                <td>
		                <table>
			                <tr>
				                <td>生产线：</td>
		                        <td><input id="productLineName" class="easyui-textbox" style="width:120px;" readonly="true" /><input id="organizationId" readonly="true" style="display:none;"/></td>
				                <td>时间：</td>
				                <td><input id="datetime" type="text" class="easyui-datebox" style="width:120px;" required="required" /></td>
                                <td><input id="Radio1" type="radio" name="reportType" value="日报" checked="checked" />日报</td>
                                <td><input id="Radio2" type="radio" name="reportType" value="月报" />月报</td>
                                <td><input id="Radio3" type="radio" name="reportType" value="年报" />年报</td>
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
        <div data-options="region:'center',border:false">
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
