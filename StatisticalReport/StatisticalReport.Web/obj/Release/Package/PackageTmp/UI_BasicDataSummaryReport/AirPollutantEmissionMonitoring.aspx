<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AirPollutantEmissionMonitoring.aspx.cs" Inherits="StatisticalReport.Web.UI_BasicDataSummaryReport.AirPollutantEmissionMonitoring" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>大气污染物报表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/page/AirPollutantEmissionMonitoring.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:true, collapsible:false, split:false">
            <table id="grid_AirPollutantEmission"></table>
        </div>
        <div id="toolbar_AirPollutantEmission" style="display:none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width:60px; text-align: right;">开始时间</td>
                                <td>
                                    <input id="startTime" type="text" class="easyui-datetimebox" style="width:150px" required="required"/>
                                </td>
                                <td style="width:60px; text-align: right;">结束时间</td>
                                <td>
                                    <input id="endTime" type="text" class="easyui-datetimebox" style="width:150px" required="required"/>
                                </td>
                                <td style="width:70px; text-align: right;">
                                    <a id="mSelectBtn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>

    </div>
    <form id="form1" runat="server"></form>
</body>
</html>
