﻿using StatisticalReport.Service.BasicDataSummaryReport;
using StatisticalReport.Service.ComprehensiveReport;
using StatisticalReport.Service.StatisticalReportServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web.UI_BasicDataSummaryReport
{
    public partial class DailyBasicElectricityUsage : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "DailyBasicElectricityUsage.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");  
            }
            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "电量报表.xls", m_ExportTable);
            }
        }

        [WebMethod]
        public static string GetElectricityUsageDailyReport(string organizationId, DateTime startDate, DateTime endDate)
        {
            //List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            //IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable dt = DailyBasicElectricityUsageService.GetDailyBasicElectricityUsageByOrganiztionIds(organizationId, startDate,endDate);

            //DataTable dt = ElectricityUsageReportService.GetElectricityUsageDailyByOrganiztionIds();
            //return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            //string test = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode");
            return EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "FormulaLevelCode");
        }
    }
}