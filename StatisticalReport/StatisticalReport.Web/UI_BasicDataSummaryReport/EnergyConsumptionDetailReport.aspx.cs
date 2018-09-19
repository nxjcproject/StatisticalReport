﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using StatisticalReport.Service.BasicDataSummaryReport;
using StatisticalReport.Service.StatisticalReportServices;

namespace StatisticalReport.Web.UI_BasicDataSummaryReport
{
    public partial class EnergyConsumptionDetailReport : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx_tys", "zc_nxjc_szsc_szsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EnergyConsumptionDetailReport.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");
            }

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "能耗明细报表.xls", m_ExportTable);
            }
        }

        [WebMethod]
        public static string GetEnergyConsumptionDetailReportData(string mOrganizationId, string mStartDate, string mEndDate, string mChecked)
        {
            DataTable table = StatisticalReport.Service.BasicDataSummaryReport.EnergyConsumptionDetailReportService.GetEnergyConsumptionDetailReportDataTable(mOrganizationId, mStartDate, mEndDate, mChecked);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }
    }
}