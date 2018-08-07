using StatisticalReport.Service.BasicDataSummaryReport;
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
    public partial class EnvironmentalStatisticalReport : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\ReportHeaderTemplate\\EnvironmentalStatisticalReport.xml";
        private static DataTable myDataTable;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx_efcf", "zc_nxjc_qtx_tys", "zc_nxjc_ychc_lsf", "zc_nxjc_szsc_szsf", "zc_nxjc_ychc_ndf", "zc_nxjc_ychc_yfcf", "zc_nxjc_qtx_efc", "zc_nxjc_tsc_tsf", "zc_nxjc_klqc_klqf", "zc_nxjc_znc_znf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EnvironmentalStatisticalReport.aspx";                                     //向web用户控件传递当前调用的页面名称
                //this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");
                //this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
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
                m_ExportTable = m_ExportTable.Replace("&nbsp", "  ");
                EnvironmentalStatisticalReportService.ExportExcelFile("xls", m_Parameter2 + "环境统计报表.xls", m_ExportTable);
            }
        }
        [WebMethod]
        public static string GetShiftsSchedulingLog(string organizationId, string startDate, string endDate)
        {
            DataTable table = EnvironmentalStatisticalReportService.GetShiftsSchedulingLogMonthly(organizationId, startDate, endDate);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }
        [WebMethod]
        public static string GetEnvironmentalStatisticalReport(string organizationId, DateTime startDate, DateTime endDate)
        {
            DataTable dt = EnvironmentalStatisticalReportService.GetEnvironmentalByOrganiztionIds(organizationId, startDate, endDate);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
        }
    }
}