using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using StatisticalReport.Service.StatisticalReportServices.Monthly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class TeamCementYearlyElectricityConsumption
    {
        private static TZHelper _tzHelper;

        static TeamCementYearlyElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable result = _tzHelper.CreateTableStructure("report_TeamCementYearlyElectricityConsumption");

            int year = 0;
            int month = 0;
            int.TryParse(date, out year);
            if (year == System.DateTime.Now.Year)
            {
                month = System.DateTime.Now.Month - 1;
            }
            else
            {
                month = 12;
            }
            for (int i = 1; i <= month; i++)
            {
                string dateTime;
                if (i < 10)
                {
                    dateTime = date + "0" + i.ToString();
                }
                else
                {
                    dateTime = date + i.ToString();
                }

                DataTable temp = TeamCementMonthlyElectricityConsumption.TableQuery(organizationID, dateTime);
                result.ImportRow(temp.Rows[temp.Rows.Count - 1]);
            }

            return result;
        }
    }
}
