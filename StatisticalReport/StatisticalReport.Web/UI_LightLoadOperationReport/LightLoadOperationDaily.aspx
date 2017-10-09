<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LightLoadOperationDaily.aspx.cs" Inherits="StatisticalReport.Web.UI_LightLoadOperationReport.LightLoadOperationDaily" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>轻负载运行统计</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/UI_LightLoadOperationReport/js/page/LightLoadOperationDaily.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div id="toolbar_LightLoadOperation" style="display: none">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width: 55px; text-align: right;">生产区域</td>
                                <td style="width: 130px;">
                                    <input id="productLineName" class="easyui-textbox" style="width: 120px;" readonly="true" /><input id="organizationId" readonly="true" style="display: none;" /></td>
                                <td style="width: 55px; text-align: right;">开始时间</td>
                                <td style="width: 160px;">
                                    <input id="StartTime" type="text" class="easyui-datetimebox" style="width: 150px;" />
                                </td>
                                <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="QueryReportFun();">查询</a>
                                </td>
                                <td>
                                    <div class="datagrid-btn-separator"></div>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width: 55px; text-align: right;">设备</td>
                                <td style="width: 130px;">
                                    <select id="Commbobox_VariableId" class="easyui-combobox" name="VariableId" style="width: 120px;" data-options="valueField:'id',textField:'text',panelHeight:'auto',required: false,editable: false"></select>
                                </td>
                                <td style="width: 55px; text-align: right;">结束时间</td>
                                <td style="width: 160px;">
                                    <input id="EndTime" type="text" class="easyui-datetimebox" style="width: 150px;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center',border:false">
            <table id="grid_LightLoadOperation" class="easyui-treegrid" 
                data-options="idField:'ID',treeField:'VariableDescription',rownumbers:true,singleSelect:true,border:false,fit:true,toolbar:'#toolbar_LightLoadOperation'">
               <thead frozen="true">
                    <tr>
                        <!--<th data-options="field:'ID',width:200,hidden: true">设备ID</th>
                        <th data-options="field:'OrganizationID',width:110,hidden: true">组织机构</th>-->
                        <th data-options="field:'VariableDescription',width:200">设备名称</th>
                    </tr>
               </thead>
                <thead>
                    <tr>
                        <th data-options="field:'Name',width:100">组织机构</th>
                        <th data-options="field:'StartTimeRun',width:130">开机时间</th>
                        <th data-options="field:'StartTimeAlarm',width:130">报警开始时间</th>
                        <th data-options="field:'EndTimeAlarm',width:130">报警结束时间</th>
                        <th data-options="field:'EndTimeStop',width:130">停机时间</th>
                        <th data-options="field:'AlarmTimeLong',width:130">报警持续时间</th>
                        <th data-options="field:'RunTimeLong',width:130">运行时间</th>
                        <th data-options="field:'LoadValueAvg',width:70">平均负荷</th>
                        <th data-options="field:'LoadTagType',width:70">负荷类型</th>
                        <th data-options="field:'DelayTime',width:80">设定延时</th>
                        <th data-options="field:'LLoadLimit',width:80">设定值</th>
                        <th data-options="field:'Remark',width:150">备注</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <form id="form_Main" runat="server"></form>
</body>
</html>
