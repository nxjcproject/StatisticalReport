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
    public partial class CoalConsumptionDailyReport : WebStyleBaseForEnergy.webStyleBase
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

        [WebMethod]
        public static string GetCoalConsumptionDailyReport(DateTime dateTime)
        {
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable dt = CoalConsumptionReportService.GetCoalConsumptionDailyByOrganiztionIds(levelCodes.ToArray(),dateTime);

            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
        }
    }
}