<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DailyBasicElectricityConsumption.aspx.cs" Inherits="StatisticalReport.Web.UI_BasicDataSummaryReport.DailyBasicElectricityConsumption" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>能耗日报</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
            <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>


    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 
            <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>
        <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/UI_BasicDataSummaryReport/js/page/DailyBasicElectricityConsumption.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
       
        
        
         <div data-options="region:'west',split:true" style="width:150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>



        <!-- 图表开始 -->
      <div data-options="region:'center',border:false">

    
          <div class="easyui-layout" data-options="fit:true,border:false">

        <div class="easyui-panel queryPanel"   data-options="region:'north', border:true, collapsible:true, split:false" style="height: 90px;">
<%--            id="toolbar_ReportTemplate"--%>
            
            <table>
            <tr><td style="height:5px;"></td></tr>

                <tr>
	                <td>
		                <table>
			                <tr>
				                <td>生产线：</td>
		                        <td><input id="productLineName" class="easyui-textbox" style="width:120px;" readonly="true" /><input id="organizationId" readonly="true" style="display:none;"/></td>
				                

                                <td>考核项目：</td>
                                <td>
                                <select id="cbbConsumptionType" class="easyui-combobox" data-options="panelHeight:'auto'" style="width:80px;">
                                    <option value="ElectricityConsumption">电耗</option>
                                    <option value="CoalConsumption">煤耗</option>
                                </select>
                            </td>
                            <td><div class="datagrid-btn-separator"></div></td>



                                
                                <td>开始时间：</td>
				                <td>
                                    <%--<input id="datetime" class="easyui-datetimespinner" value="6/24/2014" data-options="formatter:formatter2,parser:parser2,selections:[[0,4],[5,7]]" style="width:180px;" />--%>
                                    <input id="startDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/>
				                </td>
                                <td>结束时间：</td>
				                <td>
                                    <%--<input id="datetime" class="easyui-datetimespinner" value="6/24/2014" data-options="formatter:formatter2,parser:parser2,selections:[[0,4],[5,7]]" style="width:180px;" />--%>
                                    <input id="endDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/>
				                </td>
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
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshFun();">刷新</a>
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

        <div   data-options="region:'center'">
<%--            id="reportTable"   class="easyui-panel"   , border:true, collapsible:false, split:false"--%>
            <table id="gridMain_ReportTemplate" class="easyui-treegrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true,fit:true" style="width:100%">
		        <thead  data-options="frozen:true">
<%--                    data-options="frozen:true"--%>
                    <tr>
                       <th data-options="field:'Name',width:150">项目</th>
                    </tr>
                </thead>
                <thead>
			        <tr>
                        <%--<th data-options="field:'Name',width:250">项目</th>--%>
                        <th data-options="field:'FirstB',width:65">甲班</th>
                        <th data-options="field:'SecondB',width:65">乙班</th>
                        <th data-options="field:'ThirdB',width:65">丙班</th>


                        <th data-options="field:'PeakB',width:65">峰期</th>
                        <th data-options="field:'ValleyB',width:65">谷期</th>
                        <th data-options="field:'FlatB',width:65">平期</th>


                        <th data-options="field:'teamA',width:70">A班</th>
                        <th data-options="field:'teamB',width:70">B班</th>
                        <th data-options="field:'teamC',width:70">C班</th>
                        <th data-options="field:'teamD',width:70">D班</th>
                        <th data-options="field:'TotalPeakValleyFlatB',width:70">合计</th>

			        </tr>
		        </thead>
            </table>
        </div>
        <!-- 图表结束 -->
                       
        
        
         <div  data-options="region:'east', split:true" style="width:300px;" title="排班情况">
	                <table id="dgShiftsScheduling" class="easyui-datagrid" data-options="fill: true,singleSelect:true,fit:true">
		                <thead>
			                <tr>
				                <th data-options="field:'TimeStamp',width:75 ,styler:ShiftsSchedulingStyler">日期</th>
                                <th data-options="field:'FirstWorkingTeam',width:75 ,styler:ShiftsSchedulingStyler">夜班</th>
<%--                                ,styler:ShiftsSchedulingStyler--%>
				                <th data-options="field:'SecondWorkingTeam',width:75 ,styler:ShiftsSchedulingStyler">白班</th>
<%--                                ,styler:ShiftsSchedulingStyler--%>
                                <th data-options="field:'ThirdWorkingTeam',width:75 ,styler:ShiftsSchedulingStyler">中班</th>
<%--                                ,styler:ShiftsSchedulingStyler--%>
			                </tr>
		                </thead>
	                </table>
                </div>


    </div>
    
     </div>
     
    </div>

    <form id="form_Main" runat="server"></form>

</body>
</html>
