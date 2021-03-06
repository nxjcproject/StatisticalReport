﻿using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.VBReport.Monthly
{
    public class AmmeterPeakerValleyFlatMonth           //zcs
    {
        private static string connectionString;
        private static TZHelper _tzHelper;

        static AmmeterPeakerValleyFlatMonth()//静态构造函数
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connectionString);
        }
        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable sourceTable = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_AmmeterPeakerValleyFlatMonth");
            DataTable resultTable = ReportHelper.ReportDataToInteger(sourceTable);

            return resultTable;
        }
    }
}
