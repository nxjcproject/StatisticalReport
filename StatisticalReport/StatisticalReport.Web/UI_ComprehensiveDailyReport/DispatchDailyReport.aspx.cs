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
    public partial class DispatchDailyReport : WebStyleBaseForEnergy.webStyleBase
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
            }
        }
        /// <summary>
        /// 累计值（本月1号到昨天的累计）
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetComplete(DateTime date)
        {
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            //DataTable table = DispatchDailyReportService.GetCompanyTargetCompletion(levelCodes.ToArray());
            //string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            DataTable table = DispatchDailyReportService.GetTreeTargetComletion(levelCodes.ToArray(), date);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }
        [WebMethod]
        public static string GetPlanAndCompelete(DateTime date,string companyName)
        {
            DataTable table = DispatchDailyReportService.GetPlanAndTargetCompletionByCompanyName(date,companyName,true);
            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in table.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }
            string[] columnsNames = { "熟料产量(千吨)", "发电量(10WKwh)", "吨熟料发电量(KWH/吨)", "熟料电耗(Kwh/t)", 
                                        "熟料煤耗(10Kg/t)", "生料磨电耗(Kwh/t)", "煤磨电耗(Kwh/t)", "水泥产量(千吨)", "水泥电耗(Kwh/t)", "水泥磨电耗(Kwh/t)" };
            //string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table,colNames.ToArray(), new string[] { "计划","完成情况" }, "项目指标", "", 1);
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, columnsNames, new string[] { "计划", "完成情况" }, "项目指标", "", 1);
            return json;
        }
        [WebMethod]
        public static string GetGapPlanAndComplete(DateTime date,string companyName)
        {
            DataTable table = DispatchDailyReportService.GetDailyGapPlanAndTargetCompletion(companyName,date);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetEnergyAlarm(DateTime date)
        {
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable table = DispatchDailyReportService.GetEnergyAlarmTable(levelCodes.ToArray(),date);
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
        public static string GetMachineHaltAlarm(DateTime date)
        {
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable table = DispatchDailyReportService.GetMachineHaltAlarmTable(levelCodes.ToArray(),date);
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