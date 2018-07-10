using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using StatisticalReport.Service.BasicDataSummaryReport;

namespace StatisticalReport.Web.UI_BasicDataSummaryReport
{
    public partial class AirPollutantEmissionMonitoring : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
        }

        [WebMethod]
        public static string GetAirPollutantEmissionData(string mStartTime, string mEndTime)
        {
            DataTable table = StatisticalReport.Service.BasicDataSummaryReport.AirPollutantEmissionMonitoring.GetAirPollutantEmissionDataInfo(mStartTime, mEndTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}