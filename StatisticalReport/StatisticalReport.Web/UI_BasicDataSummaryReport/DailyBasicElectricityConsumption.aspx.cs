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
    public partial class DailyBasicElectricityConsumption : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\ReportHeaderTemplate\\DailyBasicElectricityConsumption.aspx.xml";
        private static DataTable myDataTable;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx", "zc_nxjc_byc_byf", "zc_nxjc_ychc", "zc_nxjc_tsc", "zc_nxjc_szsc", "zc_nxjc_klqc", "zc_nxjc_znc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "DailyBasicElectricityConsumption.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");
            this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "电耗报表.xls", m_ExportTable);
            }
        }
        [WebMethod]
        public static string GetData(string organizationId, string startTime, string endTime,string consumptionType )
        {
            DataTable table = DailyBasicElectricityConsumptionService.GetElectricityConsumptionData(organizationId, startTime, endTime, consumptionType);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string PrintFile()
        {
            string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            string m_HtmlData = StatisticalReportHelper.CreatePrintHtmlTable(mFileRootPath +
                REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
            return m_HtmlData;
        }

        [WebMethod]
        public static string GetShiftsSchedulingLog(string organizationId, string startDate, string endDate)
        {
            DataTable table = DailyBasicElectricityConsumptionService.GetShiftsSchedulingLogMonthly(organizationId, startDate, endDate);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }


    }
}