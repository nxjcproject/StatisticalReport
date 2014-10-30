using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.VBReport.Yearly
{
    public class CementMillYearlyOutput                //cdy
    {
        private static string connectionString;
        private static TZHelper _tzHelper;

        static CementMillYearlyOutput()//静态构造函数
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connectionString);
        }
        public static DataTable TableQuery(string organizeID, string date)
        {
            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizeID, date, "table_CementMillYearlyOutput");
            DataTable temp1 = ReportHelper.ReportAddToSumRow(temp2);
            DataTable temp = ReportHelper.ReportDataToInteger(temp1);
            return temp;
        }
    }
}