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
    public class TeamCementMonthlyEnergyConsumption                      //zcs
    {
        private static TZHelper _tzHelper;

        static TeamCementMonthlyEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = CementMilMonthlyEnergyConsumption.TableQuery(organizationID, date); // 获得“水泥粉磨能耗月统计分析”数据表
            DataTable result = _tzHelper.CreateTableStructure("report_TeamCementMonthlyEnergyConsumption");

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
                        newRow["TeamA_Electricity_Cement"] = temp1.Rows[i - 1]["First_Electricity_Cement"];
                        newRow["TeamA_Electricity_CementGrinding"] = temp1.Rows[i - 1]["First_Electricity_CementGrinding"];
                        newRow["TeamA_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["First_Electricity_AdmixturePreparation"];
                        newRow["TeamA_Electricity_BagsBulk"] = temp1.Rows[i - 1]["First_Electricity_BagsBulk"];
                        newRow["TeamA_Output_Cement"] = temp1.Rows[i - 1]["First_Output_Cement"];
                        newRow["TeamA_Output_BagsBulk"] = temp1.Rows[i - 1]["First_Output_BagsBulk"];
                        newRow["TeamA_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Cement"];
                        newRow["TeamA_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CementGrinding"];
                        newRow["TeamA_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["First_ElectricityConsumption_BagsBulk"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "B组")
                    {
                        newRow["TeamB_Electricity_Cement"] = temp1.Rows[i - 1]["First_Electricity_Cement"];
                        newRow["TeamB_Electricity_CementGrinding"] = temp1.Rows[i - 1]["First_Electricity_CementGrinding"];
                        newRow["TeamB_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["First_Electricity_AdmixturePreparation"];
                        newRow["TeamB_Electricity_BagsBulk"] = temp1.Rows[i - 1]["First_Electricity_BagsBulk"];
                        newRow["TeamB_Output_Cement"] = temp1.Rows[i - 1]["First_Output_Cement"];
                        newRow["TeamB_Output_BagsBulk"] = temp1.Rows[i - 1]["First_Output_BagsBulk"];
                        newRow["TeamB_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Cement"];
                        newRow["TeamB_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CementGrinding"];
                        newRow["TeamB_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["First_ElectricityConsumption_BagsBulk"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "C组")
                    {
                        newRow["TeamC_Electricity_Cement"] = temp1.Rows[i - 1]["First_Electricity_Cement"];
                        newRow["TeamC_Electricity_CementGrinding"] = temp1.Rows[i - 1]["First_Electricity_CementGrinding"];
                        newRow["TeamC_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["First_Electricity_AdmixturePreparation"];
                        newRow["TeamC_Electricity_BagsBulk"] = temp1.Rows[i - 1]["First_Electricity_BagsBulk"];
                        newRow["TeamC_Output_Cement"] = temp1.Rows[i - 1]["First_Output_Cement"];
                        newRow["TeamC_Output_BagsBulk"] = temp1.Rows[i - 1]["First_Output_BagsBulk"];
                        newRow["TeamC_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Cement"];
                        newRow["TeamC_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CementGrinding"];
                        newRow["TeamC_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["First_ElectricityConsumption_BagsBulk"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["甲班"] == "D组")
                    {
                        newRow["TeamD_Electricity_Cement"] = temp1.Rows[i - 1]["First_Electricity_Cement"];
                        newRow["TeamD_Electricity_CementGrinding"] = temp1.Rows[i - 1]["First_Electricity_CementGrinding"];
                        newRow["TeamD_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["First_Electricity_AdmixturePreparation"];
                        newRow["TeamD_Electricity_BagsBulk"] = temp1.Rows[i - 1]["First_Electricity_BagsBulk"];
                        newRow["TeamD_Output_Cement"] = temp1.Rows[i - 1]["First_Output_Cement"];
                        newRow["TeamD_Output_BagsBulk"] = temp1.Rows[i - 1]["First_Output_BagsBulk"];
                        newRow["TeamD_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["First_ElectricityConsumption_Cement"];
                        newRow["TeamD_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["First_ElectricityConsumption_CementGrinding"];
                        newRow["TeamD_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["First_ElectricityConsumption_BagsBulk"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["First_ComprehensiveElectricityConsumption"];
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
                        newRow["TeamA_Electricity_Cement"] = temp1.Rows[i - 1]["Second_Electricity_Cement"];
                        newRow["TeamA_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Second_Electricity_CementGrinding"];
                        newRow["TeamA_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Second_Electricity_AdmixturePreparation"];
                        newRow["TeamA_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Second_Electricity_BagsBulk"];
                        newRow["TeamA_Output_Cement"] = temp1.Rows[i - 1]["Second_Output_Cement"];
                        newRow["TeamA_Output_BagsBulk"] = temp1.Rows[i - 1]["Second_Output_BagsBulk"];
                        newRow["TeamA_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Cement"];
                        newRow["TeamA_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CementGrinding"];
                        newRow["TeamA_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_BagsBulk"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "B组")
                    {
                        newRow["TeamB_Electricity_Cement"] = temp1.Rows[i - 1]["Second_Electricity_Cement"];
                        newRow["TeamB_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Second_Electricity_CementGrinding"];
                        newRow["TeamB_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Second_Electricity_AdmixturePreparation"];
                        newRow["TeamB_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Second_Electricity_BagsBulk"];
                        newRow["TeamB_Output_Cement"] = temp1.Rows[i - 1]["Second_Output_Cement"];
                        newRow["TeamB_Output_BagsBulk"] = temp1.Rows[i - 1]["Second_Output_BagsBulk"];
                        newRow["TeamB_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Cement"];
                        newRow["TeamB_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CementGrinding"];
                        newRow["TeamB_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_BagsBulk"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "C组")
                    {
                        newRow["TeamC_Electricity_Cement"] = temp1.Rows[i - 1]["Second_Electricity_Cement"];
                        newRow["TeamC_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Second_Electricity_CementGrinding"];
                        newRow["TeamC_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Second_Electricity_AdmixturePreparation"];
                        newRow["TeamC_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Second_Electricity_BagsBulk"];
                        newRow["TeamC_Output_Cement"] = temp1.Rows[i - 1]["Second_Output_Cement"];
                        newRow["TeamC_Output_BagsBulk"] = temp1.Rows[i - 1]["Second_Output_BagsBulk"];
                        newRow["TeamC_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Cement"];
                        newRow["TeamC_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CementGrinding"];
                        newRow["TeamC_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_BagsBulk"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["乙班"] == "D组")
                    {
                        newRow["TeamD_Electricity_Cement"] = temp1.Rows[i - 1]["Second_Electricity_Cement"];
                        newRow["TeamD_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Second_Electricity_CementGrinding"];
                        newRow["TeamD_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Second_Electricity_AdmixturePreparation"];
                        newRow["TeamD_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Second_Electricity_BagsBulk"];
                        newRow["TeamD_Output_Cement"] = temp1.Rows[i - 1]["Second_Output_Cement"];
                        newRow["TeamD_Output_BagsBulk"] = temp1.Rows[i - 1]["Second_Output_BagsBulk"];
                        newRow["TeamD_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_Cement"];
                        newRow["TeamD_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_CementGrinding"];
                        newRow["TeamD_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Second_ElectricityConsumption_BagsBulk"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Second_ComprehensiveElectricityConsumption"];
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
                        newRow["TeamA_Electricity_Cement"] = temp1.Rows[i - 1]["Third_Electricity_Cement"];
                        newRow["TeamA_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Third_Electricity_CementGrinding"];
                        newRow["TeamA_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Third_Electricity_AdmixturePreparation"];
                        newRow["TeamA_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Third_Electricity_BagsBulk"];
                        newRow["TeamA_Output_Cement"] = temp1.Rows[i - 1]["Third_Output_Cement"];
                        newRow["TeamA_Output_BagsBulk"] = temp1.Rows[i - 1]["Third_Output_BagsBulk"];
                        newRow["TeamA_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Cement"];
                        newRow["TeamA_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CementGrinding"];
                        newRow["TeamA_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_BagsBulk"];
                        newRow["TeamA_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["丙班"] == "B组")
                    {
                        newRow["TeamB_Electricity_Cement"] = temp1.Rows[i - 1]["Third_Electricity_Cement"];
                        newRow["TeamB_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Third_Electricity_CementGrinding"];
                        newRow["TeamB_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Third_Electricity_AdmixturePreparation"];
                        newRow["TeamB_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Third_Electricity_BagsBulk"];
                        newRow["TeamB_Output_Cement"] = temp1.Rows[i - 1]["Third_Output_Cement"];
                        newRow["TeamB_Output_BagsBulk"] = temp1.Rows[i - 1]["Third_Output_BagsBulk"];
                        newRow["TeamB_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Cement"];
                        newRow["TeamB_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CementGrinding"];
                        newRow["TeamB_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_BagsBulk"];
                        newRow["TeamB_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["丙班"] == "C组")
                    {
                        newRow["TeamC_Electricity_Cement"] = temp1.Rows[i - 1]["Third_Electricity_Cement"];
                        newRow["TeamC_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Third_Electricity_CementGrinding"];
                        newRow["TeamC_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Third_Electricity_AdmixturePreparation"];
                        newRow["TeamC_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Third_Electricity_BagsBulk"];
                        newRow["TeamC_Output_Cement"] = temp1.Rows[i - 1]["Third_Output_Cement"];
                        newRow["TeamC_Output_BagsBulk"] = temp1.Rows[i - 1]["Third_Output_BagsBulk"];
                        newRow["TeamC_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Cement"];
                        newRow["TeamC_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CementGrinding"];
                        newRow["TeamC_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_BagsBulk"];
                        newRow["TeamC_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                    }
                    else if (teamDictionary["丙班"] == "D组")
                    {
                        newRow["TeamD_Electricity_Cement"] = temp1.Rows[i - 1]["Third_Electricity_Cement"];
                        newRow["TeamD_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Third_Electricity_CementGrinding"];
                        newRow["TeamD_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Third_Electricity_AdmixturePreparation"];
                        newRow["TeamD_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Third_Electricity_BagsBulk"];
                        newRow["TeamD_Output_Cement"] = temp1.Rows[i - 1]["Third_Output_Cement"];
                        newRow["TeamD_Output_BagsBulk"] = temp1.Rows[i - 1]["Third_Output_BagsBulk"];
                        newRow["TeamD_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_Cement"];
                        newRow["TeamD_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_CementGrinding"];
                        newRow["TeamD_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Third_ElectricityConsumption_BagsBulk"];
                        newRow["TeamD_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Third_ComprehensiveElectricityConsumption"];
                    }

                    result.Rows.Add(newRow);
                }
                #endregion
                #region 合计
                {
                    DataRow newRow = result.NewRow();
                    newRow["vDate"] = (string)temp1.Rows[i - 1]["vDate"];

                    newRow["Amountto_Electricity_Cement"] = temp1.Rows[i - 1]["Amountto_Electricity_Cement"];
                    newRow["Amountto_Electricity_CementGrinding"] = temp1.Rows[i - 1]["Amountto_Electricity_CementGrinding"];
                    newRow["Amountto_Electricity_AdmixturePreparation"] = temp1.Rows[i - 1]["Amountto_Electricity_AdmixturePreparation"];
                    newRow["Amountto_Electricity_BagsBulk"] = temp1.Rows[i - 1]["Amountto_Electricity_BagsBulk"];
                    newRow["Amountto_Output_Cement"] = temp1.Rows[i - 1]["Amountto_Output_Cement"];
                    newRow["Amountto_Output_BagsBulk"] = temp1.Rows[i - 1]["Amountto_Output_BagsBulk"];
                    newRow["Amountto_ElectricityConsumption_Cement"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_Cement"];
                    newRow["Amountto_ElectricityConsumption_CementGrinding"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_CementGrinding"];
                    newRow["Amountto_ElectricityConsumption_BagsBulk"] = temp1.Rows[i - 1]["Amountto_ElectricityConsumption_BagsBulk"];
                    newRow["Amountto_ComprehensiveElectricityConsumption"] = temp1.Rows[i - 1]["Amountto_ComprehensiveElectricityConsumption"];

                    result.Rows.Add(newRow);
                }
                #endregion
            }

            string column = "TeamA_Electricity_Cement,TeamA_Electricity_CementGrinding,TeamA_Electricity_AdmixturePreparation,TeamA_Electricity_BagsBulk,TeamA_Output_Cement,TeamA_Output_BagsBulk," +
                "TeamA_ElectricityConsumption_Cement,TeamA_ElectricityConsumption_CementGrinding,TeamA_ElectricityConsumption_BagsBulk,TeamA_ComprehensiveElectricityConsumption," +
                "TeamB_Electricity_Cement,TeamB_Electricity_CementGrinding,TeamB_Electricity_AdmixturePreparation,TeamB_Electricity_BagsBulk,TeamB_Output_Cement,TeamB_Output_BagsBulk," +
                "TeamB_ElectricityConsumption_Cement,TeamB_ElectricityConsumption_CementGrinding,TeamB_ElectricityConsumption_BagsBulk,TeamB_ComprehensiveElectricityConsumption," +
                "TeamC_Electricity_Cement,TeamC_Electricity_CementGrinding,TeamC_Electricity_AdmixturePreparation,TeamC_Electricity_BagsBulk,TeamC_Output_Cement,TeamC_Output_BagsBulk," +
                "TeamC_ElectricityConsumption_Cement,TeamC_ElectricityConsumption_CementGrinding,TeamC_ElectricityConsumption_BagsBulk,TeamC_ComprehensiveElectricityConsumption," +
                "TeamD_Electricity_Cement,TeamD_Electricity_CementGrinding,TeamD_Electricity_AdmixturePreparation,TeamD_Electricity_BagsBulk,TeamD_Output_Cement,TeamD_Output_BagsBulk," +
                "TeamD_ElectricityConsumption_Cement,TeamD_ElectricityConsumption_CementGrinding,TeamD_ElectricityConsumption_BagsBulk,TeamD_ComprehensiveElectricityConsumption," +
                "Amountto_Electricity_Cement,Amountto_Electricity_CementGrinding,Amountto_Electricity_AdmixturePreparation,Amountto_Electricity_BagsBulk,Amountto_Output_Cement," +
                "Amountto_Output_BagsBulk,Amountto_ElectricityConsumption_Cement,Amountto_ElectricityConsumption_CementGrinding,Amountto_ElectricityConsumption_BagsBulk,Amountto_ComprehensiveElectricityConsumption";
            result = ReportHelper.MyTotalOn(result, "vDate", column);
            ReportHelper.GetTotal(result, "vDate", column);

            return result;
        }
    }
}
