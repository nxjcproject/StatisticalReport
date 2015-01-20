using StatisticalReport.Service.ComprehensiveReport.DispatchDailyReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web.UI_ComprehensiveDailyReport
{
    public partial class DispatchDailyReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string GetComplete()
        {
            DataTable table = DispatchDailyReportService.GetCompanyTargetCompletion();
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetPlanAndCompelete(string companyName)
        {
            DataTable table = DispatchDailyReportService.GetPlanAndTargetCompletionByCompanyName(companyName);
            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in table.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table,colNames.ToArray(), new string[] { "计划","完成情况" }, "项目指标", "", 1);
            return json;
        }
        [WebMethod]
        public static string GetGapPlanAndComplete(string companyName)
        {
            DataTable table = DispatchDailyReportService.GetDailyGapPlanAndTargetCompletion(companyName);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetEnergyAlarm()
        {
            DataTable table = DispatchDailyReportService.GetEnergyAlarmTable();
            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in table.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }
            IList<string> rowNames = new List<string>();
            foreach (DataRow dr in table.Rows)
            {
                rowNames.Add(dr["公司名称"].ToString());
            }
            table.Columns.Remove("公司名称");
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, colNames.ToArray(),rowNames.ToArray(), "公司名称", "报警次数", 1);
            return json;
        }
        [WebMethod]
        public static string GetMachineHaltAlarm()
        {
            DataTable table = DispatchDailyReportService.GetMachineHaltAlarmTable();
            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in table.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }
            IList<string> rowNames = new List<string>();
            foreach (DataRow dr in table.Rows)
            {
                rowNames.Add(dr["公司名称"].ToString());
            }
            table.Columns.Remove("公司名称");
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, colNames.ToArray(), rowNames.ToArray(), "公司名称", "报警次数", 1);
            return json;
        }
    }
}