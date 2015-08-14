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
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx" };
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
        public static string GetPlanAndCompelete(DateTime myDate, string myLevelCode)
        {
            string m_Json = "";
            DataTable table = DispatchDailyReportService.GetPlanAndTargetCompletionByLevelCode(myDate, myLevelCode, true);
            if (table != null)
            {
                m_Json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            }
            return m_Json;

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