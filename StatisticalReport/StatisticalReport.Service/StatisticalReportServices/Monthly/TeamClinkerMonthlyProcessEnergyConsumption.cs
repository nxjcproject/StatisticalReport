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
    public class TeamClinkerMonthlyProcessEnergyConsumption              //zcs
    {
        private static TZHelper _tzHelper;

        static TeamClinkerMonthlyProcessEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = ClinkerMonthlyProcessEnergyConsumption.TableQuery(organizationID, date); // 获得“熟料生产工序能耗月统计报表”数据表
            DataTable result = _tzHelper.CreateTableStructure("report_TeamClinkerMonthlyProcessEnergyConsumption");

            int year = 0, month = 0;
            int.TryParse(date.Split('-')[0], out year);
            int.TryParse(date.Split('-')[1], out month);

            for (int i = 1; i < temp1.Rows.Count; i++)
            {
                int day = i;
                IDictionary<string, string> teamDictionary = _tzHelper.GetTeamDictionary(organizationID, year, month, day);

                #region 甲班
                if (teamDictionary.Keys.Contains("甲班"))
                {
                    DataRow newRow = result.NewRow();
                    newRow["vDate"] = (string)temp1.Rows[i - 1]["vDate"];

                    if (teamDictionary["甲班"] == "A组")
                    {
                        newRow["TeamA_Electricity_RawBatch"] = temp1.Rows[i - 1]["First_Electricity_RawBatch"];
                        newRow["TeamA_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["First_Electricity_RawBatchGrinding"];
                        newRow["TeamA_Electricity_Clinker"] = temp1.Rows[i - 1]["First_Electricity_Clinker"];
                        newRow["TeamA_Electricity_CoalDust"] = temp1.Rows[i - 1]["First_Electricity_CoalDust"];
                        newRow["TeamA_Consumption_CoalDust"] = temp1.Rows[i - 1]["First_Consumption_CoalDust"];
                        newRow["TeamA_Output_RawBatch"] = temp1.Rows[i - 1]["First_Output_RawBatch"];
                        newRow["TeamA_Output_Clinker"] = temp1.Rows[i - 1]["First_Output_Clinker"];
                        newRow["TeamA_Output_CoalDust"] = temp1.Rows[i - 1]["First_Output_CoalDust"];
                        newRow["TeamA_Output_Cogeneration"] = temp1.Rows[i - 1]["First_Output_Cogeneration"];
                        newRow["TeamA_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatch"];
                        newRow["TeamA_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamA_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Clinker"];
                        newRow["TeamA_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CoalDust"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                        newRow["TeamA_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "B组")
                    {
                        newRow["TeamB_Electricity_RawBatch"] = temp1.Rows[i - 1]["First_Electricity_RawBatch"];
                        newRow["TeamB_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["First_Electricity_RawBatchGrinding"];
                        newRow["TeamB_Electricity_Clinker"] = temp1.Rows[i - 1]["First_Electricity_Clinker"];
                        newRow["TeamB_Electricity_CoalDust"] = temp1.Rows[i - 1]["First_Electricity_CoalDust"];
                        newRow["TeamB_Consumption_CoalDust"] = temp1.Rows[i - 1]["First_Consumption_CoalDust"];
                        newRow["TeamB_Output_RawBatch"] = temp1.Rows[i - 1]["First_Output_RawBatch"];
                        newRow["TeamB_Output_Clinker"] = temp1.Rows[i - 1]["First_Output_Clinker"];
                        newRow["TeamB_Output_CoalDust"] = temp1.Rows[i - 1]["First_Output_CoalDust"];
                        newRow["TeamB_Output_Cogeneration"] = temp1.Rows[i - 1]["First_Output_Cogeneration"];
                        newRow["TeamB_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatch"];
                        newRow["TeamB_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamB_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Clinker"];
                        newRow["TeamB_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CoalDust"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                        newRow["TeamB_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "C组")
                    {
                        newRow["TeamC_Electricity_RawBatch"] = temp1.Rows[i - 1]["First_Electricity_RawBatch"];
                        newRow["TeamC_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["First_Electricity_RawBatchGrinding"];
                        newRow["TeamC_Electricity_Clinker"] = temp1.Rows[i - 1]["First_Electricity_Clinker"];
                        newRow["TeamC_Electricity_CoalDust"] = temp1.Rows[i - 1]["First_Electricity_CoalDust"];
                        newRow["TeamC_Consumption_CoalDust"] = temp1.Rows[i - 1]["First_Consumption_CoalDust"];
                        newRow["TeamC_Output_RawBatch"] = temp1.Rows[i - 1]["First_Output_RawBatch"];
                        newRow["TeamC_Output_Clinker"] = temp1.Rows[i - 1]["First_Output_Clinker"];
                        newRow["TeamC_Output_CoalDust"] = temp1.Rows[i - 1]["First_Output_CoalDust"];
                        newRow["TeamC_Output_Cogeneration"] = temp1.Rows[i - 1]["First_Output_Cogeneration"];
                        newRow["TeamC_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatch"];
                        newRow["TeamC_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamC_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Clinker"];
                        newRow["TeamC_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CoalDust"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                        newRow["TeamC_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "D组")
                    {
                        newRow["TeamD_Electricity_RawBatch"] = temp1.Rows[i - 1]["First_Electricity_RawBatch"];
                        newRow["TeamD_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["First_Electricity_RawBatchGrinding"];
                        newRow["TeamD_Electricity_Clinker"] = temp1.Rows[i - 1]["First_Electricity_Clinker"];
                        newRow["TeamD_Electricity_CoalDust"] = temp1.Rows[i - 1]["First_Electricity_CoalDust"];
                        newRow["TeamD_Consumption_CoalDust"] = temp1.Rows[i - 1]["First_Consumption_CoalDust"];
                        newRow["TeamD_Output_RawBatch"] = temp1.Rows[i - 1]["First_Output_RawBatch"];
                        newRow["TeamD_Output_Clinker"] = temp1.Rows[i - 1]["First_Output_Clinker"];
                        newRow["TeamD_Output_CoalDust"] = temp1.Rows[i - 1]["First_Output_CoalDust"];
                        newRow["TeamD_Output_Cogeneration"] = temp1.Rows[i - 1]["First_Output_Cogeneration"];
                        newRow["TeamD_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatch"];
                        newRow["TeamD_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamD_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Clinker"];
                        newRow["TeamD_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CoalDust"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                        newRow["TeamD_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveCoalConsumption"];
                    }

                    result.Rows.Add(newRow);
                }
                #endregion
                #region 乙班
                if (teamDictionary.Keys.Contains("乙班"))
                {
                    DataRow newRow = result.NewRow();
                    newRow["vDate"] = (string)temp1.Rows[i - 1]["vDate"];

                    if (teamDictionary["乙班"] == "A组")
                    {
                        newRow["TeamA_Electricity_RawBatch"] = temp1.Rows[i - 1]["Second_Electricity_RawBatch"];
                        newRow["TeamA_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_Electricity_RawBatchGrinding"];
                        newRow["TeamA_Electricity_Clinker"] = temp1.Rows[i - 1]["Second_Electricity_Clinker"];
                        newRow["TeamA_Electricity_CoalDust"] = temp1.Rows[i - 1]["Second_Electricity_CoalDust"];
                        newRow["TeamA_Consumption_CoalDust"] = temp1.Rows[i - 1]["Second_Consumption_CoalDust"];
                        newRow["TeamA_Output_RawBatch"] = temp1.Rows[i - 1]["Second_Output_RawBatch"];
                        newRow["TeamA_Output_Clinker"] = temp1.Rows[i - 1]["Second_Output_Clinker"];
                        newRow["TeamA_Output_CoalDust"] = temp1.Rows[i - 1]["Second_Output_CoalDust"];
                        newRow["TeamA_Output_Cogeneration"] = temp1.Rows[i - 1]["Second_Output_Cogeneration"];
                        newRow["TeamA_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatch"];
                        newRow["TeamA_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamA_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Clinker"];
                        newRow["TeamA_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CoalDust"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                        newRow["TeamA_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "B组")
                    {
                        newRow["TeamB_Electricity_RawBatch"] = temp1.Rows[i - 1]["Second_Electricity_RawBatch"];
                        newRow["TeamB_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_Electricity_RawBatchGrinding"];
                        newRow["TeamB_Electricity_Clinker"] = temp1.Rows[i - 1]["Second_Electricity_Clinker"];
                        newRow["TeamB_Electricity_CoalDust"] = temp1.Rows[i - 1]["Second_Electricity_CoalDust"];
                        newRow["TeamB_Consumption_CoalDust"] = temp1.Rows[i - 1]["Second_Consumption_CoalDust"];
                        newRow["TeamB_Output_RawBatch"] = temp1.Rows[i - 1]["Second_Output_RawBatch"];
                        newRow["TeamB_Output_Clinker"] = temp1.Rows[i - 1]["Second_Output_Clinker"];
                        newRow["TeamB_Output_CoalDust"] = temp1.Rows[i - 1]["Second_Output_CoalDust"];
                        newRow["TeamB_Output_Cogeneration"] = temp1.Rows[i - 1]["Second_Output_Cogeneration"];
                        newRow["TeamB_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatch"];
                        newRow["TeamB_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamB_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Clinker"];
                        newRow["TeamB_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CoalDust"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                        newRow["TeamB_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "C组")
                    {
                        newRow["TeamC_Electricity_RawBatch"] = temp1.Rows[i - 1]["Second_Electricity_RawBatch"];
                        newRow["TeamC_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_Electricity_RawBatchGrinding"];
                        newRow["TeamC_Electricity_Clinker"] = temp1.Rows[i - 1]["Second_Electricity_Clinker"];
                        newRow["TeamC_Electricity_CoalDust"] = temp1.Rows[i - 1]["Second_Electricity_CoalDust"];
                        newRow["TeamC_Consumption_CoalDust"] = temp1.Rows[i - 1]["Second_Consumption_CoalDust"];
                        newRow["TeamC_Output_RawBatch"] = temp1.Rows[i - 1]["Second_Output_RawBatch"];
                        newRow["TeamC_Output_Clinker"] = temp1.Rows[i - 1]["Second_Output_Clinker"];
                        newRow["TeamC_Output_CoalDust"] = temp1.Rows[i - 1]["Second_Output_CoalDust"];
                        newRow["TeamC_Output_Cogeneration"] = temp1.Rows[i - 1]["Second_Output_Cogeneration"];
                        newRow["TeamC_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatch"];
                        newRow["TeamC_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamC_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Clinker"];
                        newRow["TeamC_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CoalDust"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                        newRow["TeamC_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "D组")
                    {
                        newRow["TeamD_Electricity_RawBatch"] = temp1.Rows[i - 1]["Second_Electricity_RawBatch"];
                        newRow["TeamD_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_Electricity_RawBatchGrinding"];
                        newRow["TeamD_Electricity_Clinker"] = temp1.Rows[i - 1]["Second_Electricity_Clinker"];
                        newRow["TeamD_Electricity_CoalDust"] = temp1.Rows[i - 1]["Second_Electricity_CoalDust"];
                        newRow["TeamD_Consumption_CoalDust"] = temp1.Rows[i - 1]["Second_Consumption_CoalDust"];
                        newRow["TeamD_Output_RawBatch"] = temp1.Rows[i - 1]["Second_Output_RawBatch"];
                        newRow["TeamD_Output_Clinker"] = temp1.Rows[i - 1]["Second_Output_Clinker"];
                        newRow["TeamD_Output_CoalDust"] = temp1.Rows[i - 1]["Second_Output_CoalDust"];
                        newRow["TeamD_Output_Cogeneration"] = temp1.Rows[i - 1]["Second_Output_Cogeneration"];
                        newRow["TeamD_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatch"];
                        newRow["TeamD_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamD_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Clinker"];
                        newRow["TeamD_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CoalDust"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                        newRow["TeamD_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveCoalConsumption"];
                    }

                    result.Rows.Add(newRow);
                }
                #endregion
                #region 丙班
                if (teamDictionary.Keys.Contains("丙班"))
                {
                    DataRow newRow = result.NewRow();
                    newRow["vDate"] = (string)temp1.Rows[i - 1]["vDate"];

                    if (teamDictionary["丙班"] == "A组")
                    {
                        newRow["TeamA_Electricity_RawBatch"] = temp1.Rows[i - 1]["Third_Electricity_RawBatch"];
                        newRow["TeamA_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_Electricity_RawBatchGrinding"];
                        newRow["TeamA_Electricity_Clinker"] = temp1.Rows[i - 1]["Third_Electricity_Clinker"];
                        newRow["TeamA_Electricity_CoalDust"] = temp1.Rows[i - 1]["Third_Electricity_CoalDust"];
                        newRow["TeamA_Consumption_CoalDust"] = temp1.Rows[i - 1]["Third_Consumption_CoalDust"];
                        newRow["TeamA_Output_RawBatch"] = temp1.Rows[i - 1]["Third_Output_RawBatch"];
                        newRow["TeamA_Output_Clinker"] = temp1.Rows[i - 1]["Third_Output_Clinker"];
                        newRow["TeamA_Output_CoalDust"] = temp1.Rows[i - 1]["Third_Output_CoalDust"];
                        newRow["TeamA_Output_Cogeneration"] = temp1.Rows[i - 1]["Third_Output_Cogeneration"];
                        newRow["TeamA_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatch"];
                        newRow["TeamA_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamA_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Clinker"];
                        newRow["TeamA_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CoalDust"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                        newRow["TeamA_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["丙班"] == "B组")
                    {
                        newRow["TeamB_Electricity_RawBatch"] = temp1.Rows[i - 1]["Third_Electricity_RawBatch"];
                        newRow["TeamB_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_Electricity_RawBatchGrinding"];
                        newRow["TeamB_Electricity_Clinker"] = temp1.Rows[i - 1]["Third_Electricity_Clinker"];
                        newRow["TeamB_Electricity_CoalDust"] = temp1.Rows[i - 1]["Third_Electricity_CoalDust"];
                        newRow["TeamB_Consumption_CoalDust"] = temp1.Rows[i - 1]["Third_Consumption_CoalDust"];
                        newRow["TeamB_Output_RawBatch"] = temp1.Rows[i - 1]["Third_Output_RawBatch"];
                        newRow["TeamB_Output_Clinker"] = temp1.Rows[i - 1]["Third_Output_Clinker"];
                        newRow["TeamB_Output_CoalDust"] = temp1.Rows[i - 1]["Third_Output_CoalDust"];
                        newRow["TeamB_Output_Cogeneration"] = temp1.Rows[i - 1]["Third_Output_Cogeneration"];
                        newRow["TeamB_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatch"];
                        newRow["TeamB_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamB_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Clinker"];
                        newRow["TeamB_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CoalDust"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                        newRow["TeamB_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["丙班"] == "C组")
                    {
                        newRow["TeamC_Electricity_RawBatch"] = temp1.Rows[i - 1]["Third_Electricity_RawBatch"];
                        newRow["TeamC_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_Electricity_RawBatchGrinding"];
                        newRow["TeamC_Electricity_Clinker"] = temp1.Rows[i - 1]["Third_Electricity_Clinker"];
                        newRow["TeamC_Electricity_CoalDust"] = temp1.Rows[i - 1]["Third_Electricity_CoalDust"];
                        newRow["TeamC_Consumption_CoalDust"] = temp1.Rows[i - 1]["Third_Consumption_CoalDust"];
                        newRow["TeamC_Output_RawBatch"] = temp1.Rows[i - 1]["Third_Output_RawBatch"];
                        newRow["TeamC_Output_Clinker"] = temp1.Rows[i - 1]["Third_Output_Clinker"];
                        newRow["TeamC_Output_CoalDust"] = temp1.Rows[i - 1]["Third_Output_CoalDust"];
                        newRow["TeamC_Output_Cogeneration"] = temp1.Rows[i - 1]["Third_Output_Cogeneration"];
                        newRow["TeamC_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatch"];
                        newRow["TeamC_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamC_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Clinker"];
                        newRow["TeamC_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CoalDust"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                        newRow["TeamC_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveCoalConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "D组")
                    {
                        newRow["TeamD_Electricity_RawBatch"] = temp1.Rows[i - 1]["Third_Electricity_RawBatch"];
                        newRow["TeamD_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_Electricity_RawBatchGrinding"];
                        newRow["TeamD_Electricity_Clinker"] = temp1.Rows[i - 1]["Third_Electricity_Clinker"];
                        newRow["TeamD_Electricity_CoalDust"] = temp1.Rows[i - 1]["Third_Electricity_CoalDust"];
                        newRow["TeamD_Consumption_CoalDust"] = temp1.Rows[i - 1]["Third_Consumption_CoalDust"];
                        newRow["TeamD_Output_RawBatch"] = temp1.Rows[i - 1]["Third_Output_RawBatch"];
                        newRow["TeamD_Output_Clinker"] = temp1.Rows[i - 1]["Third_Output_Clinker"];
                        newRow["TeamD_Output_CoalDust"] = temp1.Rows[i - 1]["Third_Output_CoalDust"];
                        newRow["TeamD_Output_Cogeneration"] = temp1.Rows[i - 1]["Third_Output_Cogeneration"];
                        newRow["TeamD_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatch"];
                        newRow["TeamD_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_RawBatchGrinding"];
                        newRow["TeamD_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Clinker"];
                        newRow["TeamD_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CoalDust"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                        newRow["TeamD_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveCoalConsumption"];
                    }

                    result.Rows.Add(newRow);
                }
                #endregion
                #region 合计
                {
                    DataRow newRow = result.NewRow();
                    newRow["vDate"] = (string)temp1.Rows[i - 1]["vDate"];

                    newRow["Amountto_Electricity_RawBatch"] = temp1.Rows[i - 1]["Amountto_Electricity_RawBatch"];
                    newRow["Amountto_Electricity_RawBatchGrinding"] = temp1.Rows[i - 1]["Amountto_Electricity_RawBatchGrinding"];
                    newRow["Amountto_Electricity_Clinker"] = temp1.Rows[i - 1]["Amountto_Electricity_Clinker"];
                    newRow["Amountto_Electricity_CoalDust"] = temp1.Rows[i - 1]["Amountto_Electricity_CoalDust"];
                    newRow["Amountto_Consumption_CoalDust"] = temp1.Rows[i - 1]["Amountto_Consumption_CoalDust"];
                    newRow["Amountto_Output_RawBatch"] = temp1.Rows[i - 1]["Amountto_Output_RawBatch"];
                    newRow["Amountto_Output_Clinker"] = temp1.Rows[i - 1]["Amountto_Output_Clinker"];
                    newRow["Amountto_Output_CoalDust"] = temp1.Rows[i - 1]["Amountto_Output_CoalDust"];
                    newRow["Amountto_Output_Cogeneration"] = temp1.Rows[i - 1]["Amountto_Output_Cogeneration"];
                    newRow["Amountto_ElectricityConsumption_RawBatch"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_RawBatch"];
                    newRow["Amountto_ElectricityConsumption_RawBatchGrinding"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_RawBatchGrinding"];
                    newRow["Amountto_ElectricityConsumption_Clinker"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_Clinker"];
                    newRow["Amountto_ElectricityConsumption_CoalDust"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_CoalDust"];
                    newRow["Amountto_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Amountto_ComprehensiveElectricityConsumption"];
                    newRow["Amountto_ComprehensiveCoalConsumption"] = temp1.Rows[i - 1]["Amountto_ComprehensiveCoalConsumption"];

                    result.Rows.Add(newRow);
                }
                #endregion
            }

            string column = "TeamA_Electricity_RawBatch,TeamA_Electricity_RawBatchGrinding,TeamA_Electricity_Clinker,TeamA_Electricity_CoalDust,TeamA_Consumption_CoalDust,TeamA_Output_RawBatch,"
                + "TeamA_Output_Clinker,TeamA_Output_CoalDust,TeamA_Output_Cogeneration,TeamA_ElectricityConsumption_RawBatch,TeamA_ElectricityConsumption_RawBatchGrinding,"
                + "TeamA_ElectricityConsumption_Clinker,TeamA_ElectricityConsumption_CoalDust,TeamA_ComprehensiveElectricityConsumption,TeamA_ComprehensiveCoalConsumption,"
                + "TeamB_Electricity_RawBatch,TeamB_Electricity_RawBatchGrinding,TeamB_Electricity_Clinker,TeamB_Electricity_CoalDust,TeamB_Consumption_CoalDust,TeamB_Output_RawBatch,"
                + "TeamB_Output_Clinker,TeamB_Output_CoalDust,TeamB_Output_Cogeneration,TeamB_ElectricityConsumption_RawBatch,TeamB_ElectricityConsumption_RawBatchGrinding,"
                + "TeamB_ElectricityConsumption_Clinker,TeamB_ElectricityConsumption_CoalDust,TeamB_ComprehensiveElectricityConsumption,TeamB_ComprehensiveCoalConsumption,"
                + "TeamC_Electricity_RawBatch,TeamC_Electricity_RawBatchGrinding,TeamC_Electricity_Clinker,TeamC_Electricity_CoalDust,TeamC_Consumption_CoalDust,TeamC_Output_RawBatch,"
                + "TeamC_Output_Clinker,TeamC_Output_CoalDust,TeamC_Output_Cogeneration,TeamC_ElectricityConsumption_RawBatch,TeamC_ElectricityConsumption_RawBatchGrinding,"
                + "TeamC_ElectricityConsumption_Clinker,TeamC_ElectricityConsumption_CoalDust,TeamC_ComprehensiveElectricityConsumption,TeamC_ComprehensiveCoalConsumption,"
                + "TeamD_Electricity_RawBatch,TeamD_Electricity_RawBatchGrinding,TeamD_Electricity_Clinker,TeamD_Electricity_CoalDust,TeamD_Consumption_CoalDust,TeamD_Output_RawBatch,"
                + "TeamD_Output_Clinker,TeamD_Output_CoalDust,TeamD_Output_Cogeneration,TeamD_ElectricityConsumption_RawBatch,TeamD_ElectricityConsumption_RawBatchGrinding,"
                + "TeamD_ElectricityConsumption_Clinker,TeamD_ElectricityConsumption_CoalDust,TeamD_ComprehensiveElectricityConsumption,TeamD_ComprehensiveCoalConsumption,"
                + "Amountto_Electricity_RawBatch,Amountto_Electricity_RawBatchGrinding,Amountto_Electricity_Clinker,Amountto_Electricity_CoalDust,Amountto_Consumption_CoalDust,Amountto_Output_RawBatch,"
                + "Amountto_Output_Clinker,Amountto_Output_CoalDust,Amountto_Output_Cogeneration,Amountto_ElectricityConsumption_RawBatch,Amountto_ElectricityConsumption_RawBatchGrinding,"
                + "Amountto_ElectricityConsumption_Clinker,Amountto_ElectricityConsumption_CoalDust,Amountto_ComprehensiveElectricityConsumption,Amountto_ComprehensiveCoalConsumption";
            result = ReportHelper.MyTotalOn(result, "vDate", column);
            ReportHelper.GetTotal(result, "vDate", column);

            return result;
        }
    }
}
