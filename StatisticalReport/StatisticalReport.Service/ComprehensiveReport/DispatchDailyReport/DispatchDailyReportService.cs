using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.ComprehensiveReport.DispatchDailyReport
{
    public static class DispatchDailyReportService
    {
        private static ISqlServerDataFactory _dataFactory;
        private const int Rate = 1;
        static DispatchDailyReportService()
        {
            _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        }

        public static DataTable GetTreeTargetComletion(string[] levelCodes, DateTime date)
        {
            /*
             *获得公司分厂的tree结构
             */
            string m_Sql = @"SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and (C.VariableId='clinker_ElectricityQuantity'
                                            or C.VariableId='cementmill_ElectricityQuantity'
                                            or C.VariableId='clinker_ClinkerOutput'
                                            or C.VariableId='cement_CementOutput' 
                                            or C.VariableId='clinker_PulverizedCoalOutput' 
                                            or C.VariableId='clinker_MixtureMaterialsOutput'
                                            or C.VariableId='clinker_PulverizedCoalInput'
                                            or C.VariableId='clinker_KilnHeadPulverizedCoalInput'
                                            or C.VariableId='clinker_KilnTailPulverizedCoalInput'
                                            or C.VariableId='clinker_LimestoneInput'
                                            or C.VariableId='clinker_ClinkerOutsourcingInput'
                                            or C.VariableId='clinker_ClinkerInput')
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.Type in ('熟料','水泥磨') or A.Type is null or A.Type = '')
                                {0}
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId
                                order by A.LevelCode";

            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in levelCodes)
            {
                levelCodesParameter.Append("A.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode + "%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
                levelCodesParameter.Append(string.Format("CHARINDEX(A.LevelCode,'{0}')>0", levelCode));
                levelCodesParameter.Append(" OR ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 4, 4);
            string m_levelCodesParameter = levelCodesParameter.ToString();
            if (m_levelCodesParameter != "")
            {
                m_levelCodesParameter = " and (" + m_levelCodesParameter + ")";
            }
            else
            {
                m_levelCodesParameter = " and A.OrganizationID <> A.OrganizationID";
            }
            m_Sql = string.Format(m_Sql, m_levelCodesParameter, date.ToString("yyyy-MM-dd"));
            DataTable result = _dataFactory.Query(m_Sql);

            //将各项目指标添加到result
            string[] m_StringItems = { "OrganizationId", "LevelCode", "Name" };
            string[] m_ValueItems ={"ElectricityQuantity","clinker_ClinkerOutput","cement_CementOutput","clinker_MixtureMaterialsOutput",
                               "clinker_PulverizedCoalOutput","clinker_PulverizedCoalInput","clinker_KilnHeadPulverizedCoalInput","clinker_KilnTailPulverizedCoalInput",
                               "clinker_LimestoneInput","clinker_ClinkerOutsourcingInput", "clinker_ClinkerInput"};

            DataTable m_ResultTree = new DataTable();
            foreach (string item in m_StringItems)
            {
                DataColumn dc = new DataColumn(item, typeof(string));
                m_ResultTree.Columns.Add(dc);
            }
            foreach (string item in m_ValueItems)
            {
                DataColumn dc = new DataColumn(item, typeof(decimal));
                m_ResultTree.Columns.Add(dc);
            }
            if (result != null)
            {
                string m_LevelCode = "";
                DataRow m_DataRow = m_ResultTree.NewRow();
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    if (m_LevelCode != result.Rows[i]["LevelCode"].ToString())
                    {
                        m_LevelCode = result.Rows[i]["LevelCode"].ToString();
                        if (i != 0)             //当出现不同的levelcode表示增加到表中
                        {
                            m_ResultTree.Rows.Add(m_DataRow.ItemArray);
                        }

                        for (int j = 0; j < m_ResultTree.Columns.Count; j++)
                        {
                            m_DataRow[j] = DBNull.Value;
                        }
                        m_DataRow["OrganizationId"] = result.Rows[i]["OrganizationId"];
                        m_DataRow["LevelCode"] = result.Rows[i]["LevelCode"];
                        m_DataRow["Name"] = result.Rows[i]["Name"];
                        string m_ColumnName = result.Rows[i]["VariableId"].ToString();
                        if (m_ColumnName == "clinker_ElectricityQuantity" || m_ColumnName == "cementmill_ElectricityQuantity")
                        {
                            if (m_DataRow["ElectricityQuantity"] == DBNull.Value)
                            {
                                m_DataRow["ElectricityQuantity"] = result.Rows[i]["Value"];
                            }
                            else
                            {
                                m_DataRow["ElectricityQuantity"] = (decimal)m_DataRow["ElectricityQuantity"] + (decimal)result.Rows[i]["Value"];
                            }
                        }
                        else if (m_ColumnName != "")
                        {
                            m_DataRow[m_ColumnName] = result.Rows[i]["Value"];
                        }
                    }
                    else
                    {
                        string m_ColumnName = result.Rows[i]["VariableId"].ToString();
                        if (m_ColumnName == "clinker_ElectricityQuantity" || m_ColumnName == "cementmill_ElectricityQuantity")
                        {
                            if (m_DataRow["ElectricityQuantity"] == DBNull.Value)
                            {
                                m_DataRow["ElectricityQuantity"] = result.Rows[i]["Value"];
                            }
                            else
                            {
                                m_DataRow["ElectricityQuantity"] = (decimal)m_DataRow["ElectricityQuantity"] + (decimal)result.Rows[i]["Value"];
                            }
                        }
                        else if (m_ColumnName != "")
                        {
                            m_DataRow[m_ColumnName] = result.Rows[i]["Value"];
                        }
                    }
                }
                if (result.Rows.Count > 0)
                {
                    m_ResultTree.Rows.Add(m_DataRow.ItemArray);
                }
            }

            return m_ResultTree;
        }
        public static DataTable GetPlanAndTargetCompletionByLevelCode(DateTime date, string LevelCode, bool isShift)
        {
            string m_Sql_PlanResult = @"select C.VariableId, C.ValueType, C.CaculateType, C.Denominator, C.QuotasName, sum(B.{2}) as PlanValue
                                from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D
                                where A.Date = '{0}'
	                            and A.Statue =1
	                            and A.KeyID = B.KeyID
	                            and B.QuotasID = C.QuotasID
	                            and A.OrganizationID = D.OrganizationID
	                            and D.LevelCode like '{1}%'
	                            and (C.ValueType = 'ElectricityQuantity' or C.ValueType = 'MaterialWeight')
	                            group by C.VariableId, C.ValueType, C.CaculateType, C.Denominator, C.QuotasName
                            union all
                            select U.VariableId, U.ValueType, U.CaculateType, U.Denominator, U.QuotasName, sum(U.Elc_{2}) / sum(U.Den_{2}) as PlanValue from
                                (select M.LevelCode, M.VariableId, M.ValueType, M.CaculateType, M.Denominator, M.QuotasName, 
		                            M.January * N.January as Elc_January, N.January as Den_January,
		                            M.February * N.February as Elc_February, N.February as Den_February,
		                            M.March * N.March as Elc_March, N.March as Den_March,
		                            M.April * N.April as Elc_April, N.April as Den_April,
		                            M.May * N.May as Elc_May, N.May as Den_May,
		                            M.June * N.June as Elc_June, N.June as Den_June,
		                            M.July * N.July as Elc_July, N.July as Den_July,
		                            M.August * N.August as Elc_August, N.August as Den_August,
		                            M.September * N.September as Elc_September, N.September as Den_September,
		                            M.October * N.October as Elc_October, N.October as Den_October,
		                            M.November * N.November as Elc_November, N.November as Den_November,
		                            M.December * N.December as Elc_December, N.December as Den_December
	                            from(select D.LevelCode, C.VariableId, C.ValueType, C.CaculateType, C.Denominator, C.QuotasName, 
			                            B.January,B.February,B.March,B.April,B.May,B.June,B.July,B.August,B.September,B.October,B.November,B.December
				                            from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D
				                            where A.Date = '{0}'
				                            and A.Statue =1
				                            and A.KeyID = B.KeyID
				                            and B.QuotasID  = C.QuotasID
				                            and A.OrganizationID = D.OrganizationID
				                            and D.LevelCode like '{1}%'
				                            and (C.ValueType = 'CoalConsumption' or C.ValueType = 'ElectricityConsumption')
				                            ) M left join (
                                                 select D.LevelCode, C.VariableId, C.ValueType, C.CaculateType, C.Denominator, 
						                            B.January,B.February,B.March,B.April,B.May,B.June,B.July,B.August,B.September,B.October,B.November,B.December
						                            from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D
						                            where A.Date = '{0}'
						                            and A.Statue =1
						                            and A.KeyID = B.KeyID
						                            and B.QuotasID = C.QuotasID
						                            and A.OrganizationID = D.OrganizationID
						                            and D.LevelCode like '{1}%'
						                            and (C.ValueType = 'ElectricityQuantity' or C.ValueType = 'MaterialWeight')
						                            ) N on M.Denominator = N.VariableId and M.LevelCode = N.LevelCode) U
                                group by U.VariableId, U.ValueType, U.CaculateType, U.Denominator, U.QuotasName";

            
            string m_Sql_ConsumptionTemplate = @"select A.VariableId, A.ValueType, A.ValueFormula from balance_Energy_Template A where A.Enabled = 1";
            string m_Month = InitMonthDictionary()[date.Month];
            m_Sql_PlanResult = string.Format(m_Sql_PlanResult, date.ToString("yyyy"), LevelCode, m_Month);
            DataTable PlanResultTable = null;
            DataTable ConsumptionTempalte = null;

            try
            {
                PlanResultTable = _dataFactory.Query(m_Sql_PlanResult);
                ConsumptionTempalte = _dataFactory.Query(m_Sql_ConsumptionTemplate);
            }
            catch
            {

            }
            if (PlanResultTable != null)
            {
                PlanResultTable.Columns.Add("ActualDay", typeof(decimal));
                PlanResultTable.Columns.Add("ActualMonth", typeof(decimal));
                PlanResultTable.Columns.Add("MonthCompare", typeof(decimal));

                if (ConsumptionTempalte != null)                      //填入月统计
                {
                    DataTable m_ConsumptionTempalteTempTable = ConsumptionTempalte.Copy();
                    ////计算电耗
                    DataTable m_ClinkerMonthData = GetSumMonthData(date, LevelCode, "熟料");      //熟料
                    DataTable m_ClinkerNormalConsmptionResult = EnergyConsumption.EnergyConsumptionCalculate.Calculate(m_ClinkerMonthData, m_ConsumptionTempalteTempTable, "ValueFormula", new string[] { "Value" });
                    if (m_ClinkerNormalConsmptionResult != null && m_ClinkerNormalConsmptionResult.Rows.Count > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_ClinkerNormalConsmptionResult, "ActualMonth", "Value", "Normal");
                    }
                    DataTable m_CementMonthData = GetSumMonthData(date, LevelCode, "水泥磨");      //水泥磨
                    DataTable m_CementNormalConsmptionResult = EnergyConsumption.EnergyConsumptionCalculate.Calculate(m_CementMonthData, m_ConsumptionTempalteTempTable, "ValueFormula", new string[] { "Value" });
                    if (m_CementNormalConsmptionResult != null && m_CementNormalConsmptionResult.Rows.Count > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_CementNormalConsmptionResult, "ActualMonth", "Value", "Normal");
                    }
                    /////计算产量

                    DataRow[] m_ClinkerQuantityResultRows = m_ClinkerMonthData.Select("ValueType = 'ElectricityQuantity' or ValueType = 'MaterialWeight'");     //计算熟料产量和电量
                    if (m_ClinkerQuantityResultRows.Length > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_ClinkerQuantityResultRows.CopyToDataTable(), "ActualMonth", "Value", "Normal");
                    }
                    DataRow[] m_CementQuantityResultRows = m_CementMonthData.Select("ValueType = 'ElectricityQuantity' or ValueType = 'MaterialWeight'");     //计算水泥磨产量和电量
                    if (m_CementQuantityResultRows.Length > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_CementQuantityResultRows.CopyToDataTable(), "ActualMonth", "Value", "Normal");
                    }

                    ////计算综合电耗、煤耗
                    DataTable m_ComprehensiveDataTable = m_ClinkerMonthData.Clone();
                    string m_FactoryLevelCode = GetFactryLevelCode(LevelCode);
                    
                    DataTable m_ClinkerActualMonthResultTable = GetClinkerSumMonthConsumption(date, LevelCode);
                    Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = AutoSetParameters.AutoSetParameters_V1.SetComprehensiveParametersFromSql("day",
                               date.ToString("yyyy-MM-01"), DateTime.Parse(date.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"), m_FactoryLevelCode, _dataFactory);    //最少分厂级层次码
                    Standard_GB16780_2012.Function_EnergyConsumption_V1 m_Function_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
                    m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerActualMonthResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

                    
                    decimal m_ClinkerPowerConsumption = m_Function_EnergyConsumption_V1.GetClinkerPowerConsumption();
                    decimal m_ClinkerCoalConsumption = m_Function_EnergyConsumption_V1.GetClinkerCoalConsumption();
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "clinker_ElectricityConsumption", "ElectricityConsumption", m_ClinkerPowerConsumption });
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "clinker_CoalConsumption", "CoalConsumption", m_ClinkerCoalConsumption });

                    if (LevelCode != m_FactoryLevelCode)     //当当前层次码比分厂级层次码低,则熟料综合电耗必须得计算分厂平均熟料综合电耗或者煤耗
                    {
                        DataTable m_FactoryClinkerActualMonthResultTable = GetClinkerSumMonthConsumption(date, m_FactoryLevelCode);
                        m_Function_EnergyConsumption_V1.ClearPropertiesList();
                        m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_FactoryClinkerActualMonthResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");
                        m_ClinkerPowerConsumption = m_Function_EnergyConsumption_V1.GetClinkerPowerConsumption();
                        m_ClinkerCoalConsumption = m_Function_EnergyConsumption_V1.GetClinkerCoalConsumption();
                    }
                    m_Function_EnergyConsumption_V1.ClearPropertiesList();           //计算水泥磨的
                    DataTable m_CementActualMonthResultTable = GetCementSumMonthConsumption(date, LevelCode);
                    m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_CementActualMonthResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "cementmill_ElectricityConsumption", "ElectricityConsumption", m_Function_EnergyConsumption_V1.GetCementPowerConsumption(m_ClinkerPowerConsumption) });
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "cementmill_CoalConsumption", "CoalConsumption", m_Function_EnergyConsumption_V1.GetCementCoalConsumption(m_ClinkerCoalConsumption) });
                    SetCaculateValue(PlanResultTable, m_ComprehensiveDataTable, "ActualMonth", "Value", "Comprehensive");
                }
                if (ConsumptionTempalte != null)                        //填入日统计
                {
                    DataTable m_ConsumptionTempalteTempTable = ConsumptionTempalte.Copy();
                    ////计算电耗
                    DataTable m_ClinkerDayData = GetSumDayData(date, LevelCode, "熟料");      //熟料
                    DataTable m_ClinkerNormalConsmptionResult = EnergyConsumption.EnergyConsumptionCalculate.Calculate(m_ClinkerDayData, m_ConsumptionTempalteTempTable, "ValueFormula", new string[] { "Value" });
                    if (m_ClinkerNormalConsmptionResult != null && m_ClinkerNormalConsmptionResult.Rows.Count > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_ClinkerNormalConsmptionResult, "ActualDay", "Value", "Normal");
                    }
                    DataTable m_CementDayData = GetSumDayData(date, LevelCode, "水泥磨");      //水泥磨
                    DataTable m_CementNormalConsmptionResult = EnergyConsumption.EnergyConsumptionCalculate.Calculate(m_CementDayData, m_ConsumptionTempalteTempTable, "ValueFormula", new string[] { "Value" });
                    if (m_CementNormalConsmptionResult != null && m_CementNormalConsmptionResult.Rows.Count > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_CementNormalConsmptionResult, "ActualDay", "Value", "Normal");
                    }
                    /////计算产量

                    DataRow[] m_ClinkerQuantityResultRows = m_ClinkerDayData.Select("ValueType = 'ElectricityQuantity' or ValueType = 'MaterialWeight'");     //计算熟料产量和电量
                    if (m_ClinkerQuantityResultRows.Length > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_ClinkerQuantityResultRows.CopyToDataTable(), "ActualDay", "Value", "Normal");
                    }

                    DataRow[] m_CementQuantityResultRows = m_CementDayData.Select("ValueType = 'ElectricityQuantity' or ValueType = 'MaterialWeight'");     //计算水泥磨产量和电量
                    if (m_CementQuantityResultRows.Length > 0)
                    {
                        SetCaculateValue(PlanResultTable, m_CementQuantityResultRows.CopyToDataTable(), "ActualDay", "Value", "Normal");
                    }
                    ////计算综合电耗、煤耗

                    DataTable m_ComprehensiveDataTable = m_ClinkerDayData.Clone();
                    string m_FactoryLevelCode = GetFactryLevelCode(LevelCode);

                    DataTable m_ClinkerActualDayResultTable = GetClinkerSumDayConsumption(date, LevelCode);
                    Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = AutoSetParameters.AutoSetParameters_V1.SetComprehensiveParametersFromSql("day",
                        date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd"), m_FactoryLevelCode, _dataFactory);

                    Standard_GB16780_2012.Function_EnergyConsumption_V1 m_Function_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
                    m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerActualDayResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

                    decimal m_ClinkerPowerConsumption = m_Function_EnergyConsumption_V1.GetClinkerPowerConsumption();
                    decimal m_ClinkerCoalConsumption = m_Function_EnergyConsumption_V1.GetClinkerCoalConsumption();
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "clinker_ElectricityConsumption", "ElectricityConsumption", m_ClinkerPowerConsumption });
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "clinker_CoalConsumption", "CoalConsumption", m_ClinkerCoalConsumption });

                    if (LevelCode != m_FactoryLevelCode)     //当当前层次码比分厂级层次码低,则熟料综合电耗必须得计算分厂平均熟料综合电耗或者煤耗
                    {
                        DataTable m_FactoryClinkerActualDayResultTable = GetClinkerSumDayConsumption(date, m_FactoryLevelCode);
                        m_Function_EnergyConsumption_V1.ClearPropertiesList();
                        m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_FactoryClinkerActualDayResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");
                        m_ClinkerPowerConsumption = m_Function_EnergyConsumption_V1.GetClinkerPowerConsumption();
                        m_ClinkerCoalConsumption = m_Function_EnergyConsumption_V1.GetClinkerCoalConsumption();
                    }
                    m_Function_EnergyConsumption_V1.ClearPropertiesList();           //计算水泥磨的
                    DataTable m_CementActualDayResultTable = GetCementSumDayConsumption(date, LevelCode);
                    m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_CementActualDayResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

                    m_ComprehensiveDataTable.Rows.Add(new object[] { "cementmill_ElectricityConsumption", "ElectricityConsumption", m_Function_EnergyConsumption_V1.GetCementPowerConsumption(m_ClinkerPowerConsumption) });
                    m_ComprehensiveDataTable.Rows.Add(new object[] { "cementmill_CoalConsumption", "CoalConsumption", m_Function_EnergyConsumption_V1.GetCementCoalConsumption(m_ClinkerCoalConsumption) });
                    SetCaculateValue(PlanResultTable, m_ComprehensiveDataTable, "ActualDay", "Value", "Comprehensive");
                }
                foreach (DataRow RowItem in PlanResultTable.Rows)         //计算差值
                {
                    if (RowItem["ActualMonth"] != DBNull.Value && RowItem["PlanValue"] != DBNull.Value)
                    {
                        RowItem["MonthCompare"] = (decimal)RowItem["ActualMonth"] - (decimal)RowItem["PlanValue"];
                    }
                }
                ///////////适当的排序
                DataView PlanResultView = PlanResultTable.DefaultView;
                PlanResultView.Sort = "CaculateType desc, ValueType desc";
                DataTable PlanResultTableSorted = PlanResultView.ToTable();   //U.ValueType, U.CaculateType
            }
            return PlanResultTable;
        }
        /// <summary>
        /// 根据源表，查找值表myResultTable中相应变量的值
        /// </summary>
        /// <param name="mySourceTable">源表</param>
        /// <param name="myResultTable">值表</param>
        /// <param name="mySourceColumnName">想要把值添加到的源字段名称</param>
        /// <param name="myValueColumnName">值字段名称</param>
        /// <param name="myCaculateType">计算类型</param>
        private static void SetCaculateValue(DataTable mySourceTable, DataTable myResultTable, string mySourceColumnName, string myValueColumnName, string myCaculateType)
        {
            foreach (DataRow m_RowItem in mySourceTable.Rows)
            {
                string m_SourceVariableId = "";
                if (m_RowItem["ValueType"].ToString() == "MaterialWeight")
                {
                    m_SourceVariableId = m_RowItem["VariableId"].ToString();
                }
                else
                {
                    m_SourceVariableId = m_RowItem["VariableId"].ToString() + "_" + m_RowItem["ValueType"].ToString();
                }
                for (int i = 0; i < myResultTable.Rows.Count; i++)
                {

                    if (m_SourceVariableId == myResultTable.Rows[i]["VariableId"].ToString() && m_RowItem["CaculateType"].ToString() == myCaculateType)
                    {
                        if (m_RowItem[mySourceColumnName] == DBNull.Value)    //第一次赋值
                        {
                            m_RowItem[mySourceColumnName] = myResultTable.Rows[i][myValueColumnName]; //(decimal)myResultTable.Rows[i][myValueColumnName] != 0? myResultTable.Rows[i][myValueColumnName];
                        }
                        else
                        {
                            if ((decimal)m_RowItem[mySourceColumnName] == 0 && myResultTable.Rows[i][myValueColumnName] != DBNull.Value)
                            {
                                m_RowItem[mySourceColumnName] = myResultTable.Rows[i][myValueColumnName];
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取能源报警次数表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetEnergyAlarmTable(string[] levelCodes, DateTime date)
        {
            DataTable resultTable = new DataTable();
            DataColumn drCompanyName = new DataColumn("公司名称", typeof(string));
            DataColumn drAlarmNum = new DataColumn("报警次数", typeof(Int16));
            drAlarmNum.DefaultValue = 0;
            resultTable.Columns.Add(drCompanyName);
            resultTable.Columns.Add(drAlarmNum);
            DataTable companyInfo = GetCompanyByLevelCodes(levelCodes);
            foreach (DataRow dr in companyInfo.Rows)
            {
                string organizationId = dr["OrganizationID"].ToString();
                string companyName = dr["CompanyName"].ToString();
                string sql = @"SELECT COUNT(*)
                                       FROM [dbo].[system_Organization] AS A INNER JOIN 
	                                   [dbo].[shift_EnergyConsumptionAlarmLog] AS B 
	                                   ON A.OrganizationID=B.OrganizationID 
	                                   WHERE 
                                        B.StartTime>='{0} 00:00:00.000' AND
                                        B.StartTime<='{1} 23:59:59.000' AND
                                        A.LevelCode LIKE  (SELECT LevelCode FROM [dbo].[system_Organization] WHERE OrganizationID=@organizationId)+'%'                           
                         ";

                SqlParameter parameter = new SqlParameter("organizationId", organizationId);
                DataTable alarmTable = _dataFactory.Query(string.Format(sql, date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd")), parameter);

                DataRow row = resultTable.NewRow();
                row["公司名称"] = companyName;
                row["报警次数"] = alarmTable.Rows[0][0];
                resultTable.Rows.Add(row);
            }
            return resultTable;
        }
        /// <summary>
        /// 获取停机报警次数表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMachineHaltAlarmTable(string[] levelCodes, DateTime date)
        {
            DataTable resultTable = new DataTable();
            DataColumn drCompanyName = new DataColumn("公司名称", typeof(string));
            DataColumn drAlarmNum = new DataColumn("报警次数", typeof(Int16));
            drAlarmNum.DefaultValue = 0;
            resultTable.Columns.Add(drCompanyName);
            resultTable.Columns.Add(drAlarmNum);
            DataTable companyInfo = GetCompanyByLevelCodes(levelCodes);
            foreach (DataRow dr in companyInfo.Rows)
            {
                string organizationId = dr["OrganizationID"].ToString();
                string companyName = dr["CompanyName"].ToString();
                string sql = @"SELECT COUNT(*)
                                       FROM [dbo].[system_Organization] AS A INNER JOIN 
	                                   [dbo].[shift_MachineHaltLog] AS B 
	                                   ON A.OrganizationID=B.OrganizationID 
	                                   WHERE 
                                        B.HaltTime>='{0} 00:00:00.000' AND
                                        B.HaltTime<='{1} 23:59:59.000' AND
                                        A.LevelCode LIKE  (SELECT LevelCode FROM [dbo].[system_Organization] WHERE OrganizationID=@organizationId)+'%'";
                SqlParameter parameter = new SqlParameter("organizationId", organizationId);
                DataTable alarmTable = _dataFactory.Query(string.Format(sql, date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd")), parameter);

                DataRow row = resultTable.NewRow();
                row["公司名称"] = companyName;
                row["报警次数"] = alarmTable.Rows[0][0];
                resultTable.Rows.Add(row);
            }
            return resultTable;
        }

        private static DataTable GetCompanyByLevelCodes(string[] levelCodes)
        {
            string sql = "SELECT [A].[Name] AS CompanyName,[A].[OrganizationID] FROM [dbo].[system_Organization] AS A WHERE {0}";
            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in levelCodes)
            {
                levelCodesParameter.Append("A.LevelCode = ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode);
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 4, 4);
            return _dataFactory.Query(string.Format(sql, levelCodesParameter.ToString()));
        }
        /// <summary>
        /// 返回月份对照字典
        /// </summary>
        /// <returns></returns>
        private static IDictionary<int, string> InitMonthDictionary()
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            result.Add(1, "January");
            result.Add(2, "February");
            result.Add(3, "March");
            result.Add(4, "April");
            result.Add(5, "May");
            result.Add(6, "June");
            result.Add(7, "July");
            result.Add(8, "August");
            result.Add(9, "September");
            result.Add(10, "October");
            result.Add(11, "November");
            result.Add(12, "December");
            return result;
        }
        private static DataTable GetSumMonthData(DateTime date, string LevelCode, string type)
        {
            string m_Sql_MonthData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp like '{0}%'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '{2}'
                                group by B.VariableId, B.ValueType";
            m_Sql_MonthData = string.Format(m_Sql_MonthData, date.ToString("yyyy-MM-"), LevelCode,type);
            DataTable m_MonthDataResultTable = _dataFactory.Query(m_Sql_MonthData);
            return m_MonthDataResultTable;
        }
        private static DataTable GetSumDayData(DateTime date, string LevelCode, string type)
        {
            string m_Sql_DayData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp = '{0}'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '{2}'
                                group by B.VariableId, B.ValueType";
            m_Sql_DayData = string.Format(m_Sql_DayData, date.ToString("yyyy-MM-dd"), LevelCode, type);
            DataTable m_DayDataResultTable = _dataFactory.Query(m_Sql_DayData);
            return m_DayDataResultTable;
        }
        private static DataTable GetClinkerSumMonthConsumption(DateTime date, string LevelCode)
        {
            string m_Sql_MonthData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp like '{0}%'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '熟料'
                                group by B.VariableId, B.ValueType";
            m_Sql_MonthData = string.Format(m_Sql_MonthData, date.ToString("yyyy-MM-"), LevelCode); 
            DataTable m_ClinkerActualMonthResultTable = _dataFactory.Query(m_Sql_MonthData);

            //Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = AutoSetParameters.AutoSetParameters_V1.SetComprehensiveParametersFromSql("day",
            //           date.ToString("yyyy-MM-01"), DateTime.Parse(date.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"), LevelCode, _dataFactory);
            //Standard_GB16780_2012.Function_EnergyConsumption_V1 m_Function_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
            //m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerActualMonthResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

            return m_ClinkerActualMonthResultTable;
        }
        private static DataTable GetClinkerSumDayConsumption(DateTime date, string LevelCode)
        {
            string m_Sql_DayData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp = '{0}'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '熟料'
                                group by B.VariableId, B.ValueType";
            m_Sql_DayData = string.Format(m_Sql_DayData, date.ToString("yyyy-MM-dd"), LevelCode);
            DataTable m_ClinkerActualDayResultTable = _dataFactory.Query(m_Sql_DayData);
            return m_ClinkerActualDayResultTable;
        }
        private static DataTable GetCementSumMonthConsumption(DateTime date, string LevelCode)
        {
            string m_Sql_MonthData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp like '{0}%'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '水泥磨'
                                group by B.VariableId, B.ValueType";
            m_Sql_MonthData = string.Format(m_Sql_MonthData, date.ToString("yyyy-MM-"), LevelCode);
            DataTable m_CementActualMonthResultTable = _dataFactory.Query(m_Sql_MonthData);
            return m_CementActualMonthResultTable;
        }
        private static DataTable GetCementSumDayConsumption(DateTime date, string LevelCode)
        {
            string m_Sql_DayData = @"Select B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A , balance_Energy B, system_Organization C
                                where A.TimeStamp = '{0}'
                                and A.StaticsCycle = 'day'
                                and A.BalanceId = B.KeyID
                                and B.OrganizationID = C.OrganizationID
                                and C.LevelCode like '{1}%'
                                and C.Type = '水泥磨'
                                group by B.VariableId, B.ValueType";
            m_Sql_DayData = string.Format(m_Sql_DayData, date.ToString("yyyy-MM-dd"), LevelCode);
            DataTable m_CementActualDayResultTable = _dataFactory.Query(m_Sql_DayData);
            return m_CementActualDayResultTable;
        }
        private static string GetFactryLevelCode(string LevelCode)
        {
            string m_LevelCode = "";
            string m_Sql = @"Select 
                    A.OrganizationID as OrganizationId, 
                    A.Name as Name,
                    A.LevelCode as LevelCode, 
                    A.Type as OrganizationType  
                    from system_Organization A 
					where A.Enabled = {1} 
                    and A.LevelType = 'Factory'
                    and CHARINDEX(A.LevelCode, '{0}') > 0";
            m_Sql = string.Format(m_Sql, LevelCode, "1");
            DataTable m_OrganizationInfo = _dataFactory.Query(m_Sql);
            if (m_OrganizationInfo != null && m_OrganizationInfo.Rows.Count > 0)
            {
                m_LevelCode = m_OrganizationInfo.Rows[0]["LevelCode"].ToString();    //当前层次码低于分厂级，则找出分厂级
            }
            else
            {
                m_LevelCode = LevelCode;             //如果当前层次码高于分厂级，则用当前层次码
            }
            return m_LevelCode;
        }
    }
}
