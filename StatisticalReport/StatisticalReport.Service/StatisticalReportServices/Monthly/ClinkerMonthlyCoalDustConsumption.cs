using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly
{
    public class ClinkerMonthlyCoalDustConsumption                             //zcs
    {
        private static TZHelper _tzHelper;

        static ClinkerMonthlyCoalDustConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = _tzHelper.CreateTableStructure("report_ClinkerMonthlyCoalDustConsumption");

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_Clinker_Output"] = dr["ClinkerProductionFirstShift"];
                newRow["Second_Clinker_Output"] = dr["ClinkerProductionSecondShift"];
                newRow["Third_Clinker_Output"] = dr["ClinkerProductionThirdShift"];
                newRow["First_KilnHead_CoalDust"] = dr["KilnHeadCoalDustConsumptionFirstShift"];
                newRow["Second_KilnHead_CoalDust"] = dr["KilnHeadCoalDustConsumptionSecondShift"];
                newRow["Third_KilnHead_CoalDust"] = dr["KilnHeadCoalDustConsumptionThirdShift"];
                newRow["First_KilnTail_CoalDust"] = dr["KilnTailCoalDustConsumptionFirstShift"];
                newRow["Second_KilnTail_CoalDust"] = dr["KilnTailCoalDustConsumptionSecondShift"];
                newRow["Third_KilnTail_CoalDust"] = dr["KilnTailCoalDustConsumptionThirdShift"];
                newRow["First_sum_CoalDust"] = dr["AmounttoCoalDustConsumptionFirstShift"];
                newRow["Second_sum_CoalDust"] = dr["AmounttoCoalDustConsumptionSecondShift"];
                newRow["Third_sum_CoalDust"] = dr["AmounttoCoalDustConsumptionThirdShift"];
                temp1.Rows.Add(newRow);
            }

            Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);

            foreach (DataRow dr in temp1.Rows)
            {
                if (Convert.ToInt64(dr["First_Clinker_Output"]) != 0)
                {
                    dr["First_CoalDustConsumption"] = Convert.ToDecimal(dr["First_sum_CoalDust"])*1000 / Convert.ToDecimal(dr["First_Clinker_Output"]);
                }
                if (Convert.ToInt64(dr["Second_Clinker_Output"]) != 0)
                {
                    dr["Second_CoalDustConsumption"] = Convert.ToDecimal(dr["Second_sum_CoalDust"])*1000 / Convert.ToDecimal(dr["Second_Clinker_Output"]);
                }
                if (Convert.ToInt64(dr["Third_Clinker_Output"]) != 0)
                {
                    dr["Third_CoalDustConsumption"] = Convert.ToDecimal(dr["Third_sum_CoalDust"])*1000 / Convert.ToDecimal(dr["Third_Clinker_Output"]);
                }

                dr["Amountto_Clinker_Output"] = Convert.ToInt64(dr["First_Clinker_Output"]) + Convert.ToInt64(dr["Second_Clinker_Output"]) + Convert.ToInt64(dr["Third_Clinker_Output"]);
                dr["Amountto_KilnHead_CoalDust"] = Convert.ToInt64(dr["First_KilnHead_CoalDust"]) + Convert.ToInt64(dr["Second_KilnHead_CoalDust"]) + Convert.ToInt64(dr["Third_KilnHead_CoalDust"]);
                dr["Amountto_KilnTail_CoalDust"] = Convert.ToInt64(dr["First_KilnTail_CoalDust"]) + Convert.ToInt64(dr["Second_KilnTail_CoalDust"]) + Convert.ToInt64(dr["Third_KilnTail_CoalDust"]);
                dr["Amountto_sum_CoalDust"] = Convert.ToInt64(dr["First_sum_CoalDust"]) + Convert.ToInt64(dr["Second_sum_CoalDust"]) + Convert.ToInt64(dr["Third_sum_CoalDust"]);
                if (Convert.ToInt64(dr["Amountto_Clinker_Output"]) != 0)
                {
                    dr["Amountto_CoalDustConsumption"] = Convert.ToDecimal(dr["Amountto_sum_CoalDust"])*1000 / Convert.ToDecimal(dr["Amountto_Clinker_Output"]);
                }
            }

            return temp1;
        }
    }
}
