using StatisticalReport.Service.ComprehensiveReport;
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
    public partial class ElectricityUsageDailyReport : WebStyleBaseForEnergy.webStyleBase
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

        [WebMethod]
        public static string GetElectricityUsageDailyReport(DateTime startDate,DateTime endDate)
        {
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable dt = ElectricityUsageReportService.GetElectricityUsageDailyByOrganiztionIds(levelCodes.ToArray(),startDate,endDate);

            //DataTable dt = ElectricityUsageReportService.GetElectricityUsageDailyByOrganiztionIds();
            //return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            //string test=EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode");
            return EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode");
        }
    }
}