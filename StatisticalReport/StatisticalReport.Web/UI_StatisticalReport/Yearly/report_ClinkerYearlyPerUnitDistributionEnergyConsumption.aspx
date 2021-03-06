﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_ClinkerYearlyPerUnitDistributionEnergyConsumption.aspx.cs" Inherits="StatisticalReport.Web.UI_StatisticalReport.Yearly.report_ClinkerYearlyPerUnitDistributionEnergyConsumption" %>
<%@ Register Src="../../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>熟料生产线可比单位产品能耗分析报表</title>

    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/editCell.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="../js/page/Yearly/report_ClinkerYearlyPerUnitDistributionEnergyConsumption.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false " style="width: 150px;">
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
				                <td><input id="datetime" class="easyui-datetimespinner" value="6/24/2014" data-options="formatter:formatter2,parser:parser2,selections:[[0,4],[5,7]]" style="width:100px;" /></td>
                                <td><a id="lbRead" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" 
                                        onclick="ReadClinkerYearlyPerUnitDistributionEnergyConsumptionPlanInfoFun();">查询</a>
                                </td>
				                <td><a id="lbQuery" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-reload'" 
                                        onclick="QueryClinkerYearlyPerUnitDistributionEnergyConsumptionPlanInfoFun();">能耗原始数据读入</a>
                                </td>
                                <%--<td><a id="lbCalc" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-sum'" 
                                        onclick="CalculateClinkerYearlyPerUnitDistributionEnergyConsumptionFun();">计算分析</a>
                                </td>
                                <td><a id="lbSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" 
                                        onclick="SaveClinkerYearlyPerUnitDistributionEnergyConsumptionFun();">分析结果存盘</a>
                                </td>--%>
			                </tr>
			            </table>
		            </td>
	            </tr>
	            <tr>
	                <td>
		                <table>
	                        <tr>
                                <td><a id="lbCalc" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-sum'" 
                                        onclick="CalculateClinkerYearlyPerUnitDistributionEnergyConsumptionFun();">计算分析</a>
                                </td>
                                <td><a id="lbSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" 
                                        onclick="SaveClinkerYearlyPerUnitDistributionEnergyConsumptionFun();">分析结果存盘</a>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" iconCls="icon-reload" plain="true" onclick="RefreshClinkerYearlyPerUnitDistributionEnergyConsumptionPlanFun();">刷新</a>
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
