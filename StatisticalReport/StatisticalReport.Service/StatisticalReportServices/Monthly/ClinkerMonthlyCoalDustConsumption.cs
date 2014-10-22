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

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_Clinker_Output"] = (decimal)dr["ClinkerProductionFirstShift"];
                newRow["Second_Clinker_Output"] = (decimal)dr["ClinkerProductionSecondShift"];
                newRow["Third_Clinker_Output"] = (decimal)dr["ClinkerProductionThirdShift"];
                newRow["First_KilnHead_CoalDust"] = (decimal)dr["KilnHeadCoalDustConsumptionFirstShift"];
                newRow["Second_KilnHead_CoalDust"] = (decimal)dr["KilnHeadCoalDustConsumptionSecondShift"];
                newRow["Third_KilnHead_CoalDust"] = (decimal)dr["KilnHeadCoalDustConsumptionThirdShift"];
                newRow["First_KilnTail_CoalDust"] = (decimal)dr["KilnTailCoalDustConsumptionFirstShift"];
                newRow["Second_KilnTail_CoalDust"] = (decimal)dr["KilnTailCoalDustConsumptionSecondShift"];
                newRow["Third_KilnTail_CoalDust"] = (decimal)dr["KilnTailCoalDustConsumptionThirdShift"];
                newRow["First_sum_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionFirstShift"];
                newRow["Second_sum_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionSecondShift"];
                newRow["Third_sum_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionThirdShift"];
                temp1.Rows.Add(newRow);
            }

            Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);

            foreach (DataRow dr in temp1.Rows)
            {
                if ((decimal)dr["First_Clinker_Output"] != 0)
                {
                    dr["First_CoalDustConsumption"] = (decimal)dr["First_sum_CoalDust"] / (decimal)dr["First_Clinker_Output"];
                }
                if ((decimal)dr["Second_Clinker_Output"] != 0)
                {
                    dr["Second_CoalDustConsumption"] = (decimal)dr["Second_sum_CoalDust"] / (decimal)dr["Second_Clinker_Output"];
                }
                if ((decimal)dr["Third_Clinker_Output"] != 0)
                {
                    dr["Third_CoalDustConsumption"] = (decimal)dr["Third_sum_CoalDust"] / (decimal)dr["Third_Clinker_Output"];
                }

                dr["Amountto_Clinker_Output"] = (decimal)dr["First_Clinker_Output"] + (decimal)dr["Second_Clinker_Output"] + (decimal)dr["Third_Clinker_Output"];
                dr["Amountto_KilnHead_CoalDust"] = (decimal)dr["First_KilnHead_CoalDust"] + (decimal)dr["Second_KilnHead_CoalDust"] + (decimal)dr["Third_KilnHead_CoalDust"];
                dr["Amountto_KilnTail_CoalDust"] = (decimal)dr["First_KilnTail_CoalDust"] + (decimal)dr["Second_KilnTail_CoalDust"] + (decimal)dr["Third_KilnTail_CoalDust"];
                dr["Amountto_sum_CoalDust"] = (decimal)dr["First_sum_CoalDust"] + (decimal)dr["Second_sum_CoalDust"] + (decimal)dr["Third_sum_CoalDust"];
                if ((decimal)dr["Amountto_Clinker_Output"] != 0)
                {
                    dr["Amountto_CoalDustConsumption"] = (decimal)dr["Amountto_sum_CoalDust"] / (decimal)dr["Amountto_Clinker_Output"];
                }
            }

            return temp1;
        }
    }
}
