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
    public partial class ElectricityConsumptionDailyReport : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc", "zc_nxjc_qtx", "zc_nxjc_byc", "zc_nxjc_klqc", "zc_nxjc_lpsc", "zc_nxjc_szsc", "zc_nxjc_tsc", "zc_nxjc_whsmc", "zc_nxjc_znc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            }
        }

        [WebMethod]
        public static string GetElectricityConsumptionDailyReport(DateTime dateTime)
        {
            //oganizationIds=zc_nxjc_qtx zc_nxjc_znc zc_nxjc_szsc zc_nxjc_klqc zc_nxjc_tsc zc_nxjc_byc zc_nxjc_ychc
            List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            DataTable dt = ElectricityConsumptionReportService.GetElectricityConsumptionDailyByOrganiztionIds(oganizationIds, dateTime, dateTime);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode");
            return json;
        }
    }
}