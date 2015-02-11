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
        private  const int Rate=100000;
        static DispatchDailyReportService()
        {
            _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        }
        public static DataTable GetCompanyTargetCompletion()
        {
            DataTable destination = new DataTable();
            DataTable itemTable = GetItemList();
            DataColumn companyRow = new DataColumn("公司", typeof(string));
            destination.Columns.Add(companyRow);
            foreach (DataRow dr in itemTable.Rows)
            {
                DataColumn dc = new DataColumn(dr["QuotasID"].ToString(), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }
            
            DataTable company=GetCompany();
            foreach (DataRow dr in company.Rows)
            {
                DataRow row = destination.NewRow();
                row["公司"] = dr["CompanyName"].ToString().Trim();
                string organizationId = dr["OrganizationID"].ToString();
                string sql = @"SELECT SUM([B].[TotalPeakValleyFlatB]) AS Value,[VariableId]
                                    FROM [dbo].[balance_Energy] AS B INNER JOIN [dbo].[tz_Balance] AS A
	                                ON [A].[BalanceId]=[B].[KeyId]
	                                WHERE [B].[OrganizationID] LIKE @organizationId + '%' AND      
	                                [A].StaticsCycle='day' AND
	                                [A].[TimeStamp]>=CONVERT(VARCHAR(7),GETDATE(),120)+'01' AND
	                                [A].[TimeStamp]<=CONVERT(VARCHAR(10),GETDATE(),120) AND 
	                                (
	                                [B].[VariableId]='coalPreparation_ElectricityQuantity' OR [B].[VariableId]='clinker_PulverizedCoalOutput' OR [B].[VariableId]='rawMaterialsPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_MixtureMaterialsOutput' OR [B].[VariableId]='clinkerPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_ClinkerOutput' OR [B].[VariableId]='clinker_PulverizedCoalInput' 
                                    OR [B].[VariableId]='cement_CementOutput' OR [B].[VariableId]='cementmill_ElectricityQuantity' 
                                    OR [B].[VariableId]='cementGrind_ElectricityQuantity'
	                                )
                                    GROUP BY [B].[VariableId]
                             ";
                SqlParameter paramater = new SqlParameter("organizationId", organizationId);
                DataTable sourceTable = _dataFactory.Query(sql, paramater);
                foreach (DataRow drow in sourceTable.Rows)
                {
                    switch (drow["VariableId"].ToString().Trim())
                    {
                        case "clinker_ClinkerOutput":
                            row["熟料产量"] = drow["Value"];
                            break;
                        case "clinkerPreparation_ElectricityQuantity":
                            row["熟料电耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)row["熟料产量"];
                            break;
                        case "clinker_PulverizedCoalInput":
                            row["熟料煤耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] * 1000 / (decimal)row["熟料产量"];
                            break;
                        case "rawMaterialsPreparation_ElectricityQuantity":
                            DataRow[] rawMaterialsoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (rawMaterialsoutputRows.Count() == 1)
                            {
                                row["生料磨电耗"] = (decimal)rawMaterialsoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)rawMaterialsoutputRows[0]["Value"];
                            }
                            break;
                        case "coalPreparation_ElectricityQuantity":
                            DataRow[] coalPreparationoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (coalPreparationoutputRows.Count() == 1)
                            {
                                row["煤磨电耗"] = (decimal)coalPreparationoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)coalPreparationoutputRows[0]["Value"];
                            }
                            break;
                        case "cement_CementOutput":
                            row["水泥产量"] = drow["Value"];
                            break;
                        case "cementmill_ElectricityQuantity":
                            row["水泥电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)row["水泥产量"];
                            break;
                        case "cementGrind_ElectricityQuantity":
                            row["水泥磨电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)row["水泥产量"];
                            break;
                    }
                }
                destination.Rows.Add(row);
                //decimal query = sourceTable.AsEnumerable().Where(x => x.Field<string>("VariableId").ToString() == "clinker_ClinkerOutput").Sum(x => x.Field<decimal>("TotalPeakValleyFlatB"));

            }
            return destination;
        }
        /// <summary>
        /// 根据公司名称获得公司计划和完成情况
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static DataTable GetPlanAndTargetCompletionByCompanyName(string companyName)
        {
            DataTable destination = new DataTable();
            DataTable itemTable = GetItemList();
            //DataColumn companyRow = new DataColumn("公司", typeof(string));
            //destination.Columns.Add(companyRow);
            foreach (DataRow dr in itemTable.Rows)
            {
                DataColumn dc = new DataColumn(dr["QuotasID"].ToString(), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataTable company = GetCompanyByCompanyName(companyName);
            
            //取计划源表
            foreach (DataRow planRow in company.Rows)
            {
                DataTable productLineNum = GetCompanyProductLineNumTable(planRow["LevelCode"].ToString());
                int clinkerNum=(int)productLineNum.Select("Type='熟料'")[0]["Count"];
                int cementNum = (int)productLineNum.Select("Type='水泥磨'")[0]["Count"];
                DataRow resultPlanRow = destination.NewRow();
                DataTable planSourceTable;
                string sqlPlan = @"SELECT  
                                 B.* 
                              FROM [dbo].[tz_Plan] AS A 
                              INNER JOIN [dbo].[plan_EnergyConsumptionYearlyPlan] AS B 
                                 ON A.KeyID=B.KeyID 
                              WHERE A.Date=DATEPART(yyyy,GETDATE()) AND A.OrganizationID LIKE @organizationId+'%'";
                SqlParameter paramaterPlan = new SqlParameter("organizationId", planRow["OrganizationID"].ToString().Trim());
                DataTable temp = _dataFactory.Query(sqlPlan, paramaterPlan);
                planSourceTable = ReportHelper.MyTotalOn(temp, "QuotasID", "January,February,March,April,May,June,July,August,September,October,November,December,Totals");
                string month = InitMonthDictionary()[Int16.Parse(DateTime.Now.ToString("MM"))];
                foreach (DataRow planSourceRow in planSourceTable.Rows)
                {
                    
                    string itemName=planSourceRow["QuotasID"].ToString().Trim();
                    resultPlanRow[itemName] = planSourceRow[month];
                }
                resultPlanRow["熟料产量"] = (decimal)resultPlanRow["熟料产量"] / Rate;
                resultPlanRow["发电量"] = (decimal)resultPlanRow["发电量"] / Rate;
                resultPlanRow["水泥产量"] = (decimal)resultPlanRow["水泥产量"] / Rate;

                resultPlanRow["吨熟料发电量"] = (decimal)resultPlanRow["吨熟料发电量"] / clinkerNum;
                resultPlanRow["煤磨电耗"] = (decimal)resultPlanRow["煤磨电耗"] / clinkerNum;
                resultPlanRow["生料磨电耗"] = (decimal)resultPlanRow["生料磨电耗"] / clinkerNum;
                resultPlanRow["熟料电耗"] = (decimal)resultPlanRow["熟料电耗"] / clinkerNum;
                resultPlanRow["熟料煤耗"] = (decimal)resultPlanRow["熟料煤耗"] / clinkerNum;
                resultPlanRow["水泥电耗"] = (decimal)resultPlanRow["水泥电耗"] / cementNum;
                resultPlanRow["水泥磨电耗"] = (decimal)resultPlanRow["水泥磨电耗"] / cementNum;
                destination.Rows.Add(resultPlanRow);
            }
            //填写实际完成情况
            foreach (DataRow dr in company.Rows)
            {
                DataRow row = destination.NewRow();
                //row["公司"] = dr["CompanyName"].ToString().Trim();
                string organizationId = dr["OrganizationID"].ToString();
                string sql = @"SELECT SUM([B].[TotalPeakValleyFlatB]) AS Value,[VariableId]
                                    FROM [dbo].[balance_Energy] AS B INNER JOIN [dbo].[tz_Balance] AS A
	                                ON [A].[BalanceId]=[B].[KeyId]
	                                WHERE [B].[OrganizationID] LIKE @organizationId + '%' AND      
	                                [A].StaticsCycle='day' AND
	                                [A].[TimeStamp]>=CONVERT(VARCHAR(7),GETDATE(),120)+'01' AND
	                                [A].[TimeStamp]<=CONVERT(VARCHAR(10),GETDATE(),120) AND 
	                                (
	                                [B].[VariableId]='coalPreparation_ElectricityQuantity' OR [B].[VariableId]='clinker_PulverizedCoalOutput' OR [B].[VariableId]='rawMaterialsPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_MixtureMaterialsOutput' OR [B].[VariableId]='clinkerPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_ClinkerOutput' OR [B].[VariableId]='clinker_PulverizedCoalInput' 
                                    OR [B].[VariableId]='cement_CementOutput' OR [B].[VariableId]='cementmill_ElectricityQuantity' 
                                    OR [B].[VariableId]='cementGrind_ElectricityQuantity' 
	                                )
                                    GROUP BY [B].[VariableId]
                             ";
                SqlParameter paramater = new SqlParameter("organizationId", organizationId);
                DataTable sourceTable = _dataFactory.Query(sql, paramater);
                foreach (DataRow drow in sourceTable.Rows)
                {
                    switch (drow["VariableId"].ToString().Trim())
                    {
                        case "clinker_ClinkerOutput":
                            row["熟料产量"] = (decimal)drow["Value"] / Rate;
                            break;
                        case "clinkerPreparation_ElectricityQuantity":
                            row["熟料电耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["熟料产量"] * Rate);
                            break;
                        case "clinker_PulverizedCoalInput":
                            row["熟料煤耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] * 1000 / ((decimal)row["熟料产量"] * Rate);
                            break;
                        case "rawMaterialsPreparation_ElectricityQuantity":
                            DataRow[] rawMaterialsoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (rawMaterialsoutputRows.Count() ==1)
                            {
                                row["生料磨电耗"] =(decimal)rawMaterialsoutputRows[0]["Value"]==0?0: (decimal)drow["Value"] / (decimal)rawMaterialsoutputRows[0]["Value"];
                            }
                            break;
                        case "coalPreparation_ElectricityQuantity":
                            DataRow[] coalPreparationoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (coalPreparationoutputRows.Count() == 1)
                            {
                                row["煤磨电耗"] =(decimal)coalPreparationoutputRows[0]["Value"]==0?0: (decimal)drow["Value"]/(decimal)coalPreparationoutputRows[0]["Value"];
                            }
                            break;
                        case "cement_CementOutput":
                            row["水泥产量"] = (decimal)drow["Value"] / Rate;
                            break;
                        case "cementGrind_ElectricityQuantity":
                            row["水泥磨电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["水泥产量"] * Rate);
                            break;
                    }
                }
                destination.Rows.Add(row);
            }
            return destination;
        }
        /// <summary>
        /// 获取日计划和完成情况之差
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static DataTable GetDailyGapPlanAndTargetCompletion(string companyName)
        {
            //构造目标表
            DataTable destination = new DataTable();
            DataColumn dcItemName = new DataColumn("项目指标", typeof(string));
            dcItemName.DefaultValue = 0;
            DataColumn dcPlan = new DataColumn("日平均计划", typeof(decimal));
            dcPlan.DefaultValue = 0;
            DataColumn dcCompletion = new DataColumn("日平均完成", typeof(decimal));
            dcCompletion.DefaultValue = 0;
            DataColumn dcGap = new DataColumn("差值", typeof(decimal));
            dcGap.DefaultValue = 0;
            destination.Columns.Add(dcItemName);
            destination.Columns.Add(dcPlan);
            destination.Columns.Add(dcCompletion);
            destination.Columns.Add(dcGap);
            //获取数据源表
            DataTable sourceTable = GetPlanAndTargetCompletionByCompanyName(companyName);
            //将数据源转化为目标表            
            foreach (DataColumn dc in sourceTable.Columns)
            {
                int denominatorPlan = Int16.Parse(DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day)).ToString("dd"));
                int denominatorComplet = DateTime.Now.Day;

                DataRow row = destination.NewRow();
                string itemName = dc.ColumnName;
                row["项目指标"] = itemName;
                if (itemName == "熟料产量" || itemName == "发电量" || itemName == "水泥产量")
                {
                    row["日平均计划"] = decimal.Parse(((decimal)sourceTable.Rows[0][itemName] * Rate / denominatorPlan).ToString("#0.00"));
                    row["日平均完成"] = decimal.Parse(((decimal)sourceTable.Rows[1][itemName] * Rate / denominatorComplet).ToString("#0.00"));
                }
                else
                {
                    row["日平均计划"] = decimal.Parse(((decimal)sourceTable.Rows[0][itemName]).ToString("#0.00"));
                    row["日平均完成"] = decimal.Parse(((decimal)sourceTable.Rows[1][itemName]).ToString("#0.00"));
                }
                row["差值"] = (decimal)row["日平均计划"] - (decimal)row["日平均完成"];
                destination.Rows.Add(row);
            }

            return destination;
        }
        /// <summary>
        /// 获取能源报警次数表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetEnergyAlarmTable()
        {
            DataTable resultTable = new DataTable();
            DataColumn drCompanyName = new DataColumn("公司名称", typeof(string));
            DataColumn drAlarmNum = new DataColumn("报警次数", typeof(Int16));
            drAlarmNum.DefaultValue = 0;
            resultTable.Columns.Add(drCompanyName);
            resultTable.Columns.Add(drAlarmNum);
            DataTable companyInfo = GetCompany();
            foreach (DataRow dr in companyInfo.Rows)
            {
                string organizationId = dr["OrganizationID"].ToString();
                string companyName = dr["CompanyName"].ToString();
                string sql = @"SELECT COUNT(*)
                                       FROM [dbo].[system_Organization] AS A INNER JOIN 
	                                   [dbo].[shift_EnergyConsumptionAlarmLog] AS B 
	                                   ON A.OrganizationID=B.OrganizationID 
	                                   WHERE A.LevelCode LIKE  (SELECT LevelCode FROM [dbo].[system_Organization] WHERE OrganizationID=@organizationId)+'%'                           
                         ";
                
                SqlParameter parameter = new SqlParameter("organizationId", organizationId);
                DataTable alarmTable=_dataFactory.Query(sql,parameter);
                 
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
        public static DataTable GetMachineHaltAlarmTable()
        {
            DataTable resultTable = new DataTable();
            DataColumn drCompanyName = new DataColumn("公司名称", typeof(string));
            DataColumn drAlarmNum = new DataColumn("报警次数", typeof(Int16));
            drAlarmNum.DefaultValue = 0;
            resultTable.Columns.Add(drCompanyName);
            resultTable.Columns.Add(drAlarmNum);
            DataTable companyInfo = GetCompany();
            foreach (DataRow dr in companyInfo.Rows)
            {
                string organizationId = dr["OrganizationID"].ToString();
                string companyName = dr["CompanyName"].ToString();
                string sql = @"SELECT COUNT(*)
                                       FROM [dbo].[system_Organization] AS A INNER JOIN 
	                                   [dbo].[shift_MachineHaltLog] AS B 
	                                   ON A.OrganizationID=B.OrganizationID 
	                                   WHERE A.LevelCode LIKE  (SELECT LevelCode FROM [dbo].[system_Organization] WHERE OrganizationID=@organizationId)+'%'                           
                         ";

                SqlParameter parameter = new SqlParameter("organizationId", organizationId);
                DataTable alarmTable = _dataFactory.Query(sql, parameter);

                DataRow row = resultTable.NewRow();
                row["公司名称"] = companyName;
                row["报警次数"] = alarmTable.Rows[0][0];
                resultTable.Rows.Add(row);
            }
            return resultTable;
        }
        /// <summary>
        /// 获取公司级别的具体公司
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCompany()
        {
            //ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            string sql = "SELECT [A].[Name] AS CompanyName,[A].[OrganizationID] FROM [dbo].[system_Organization] AS A WHERE LEN([LevelCode])=3";
            return _dataFactory.Query(sql);
        }
        private static DataTable GetItemList()
        {
            string sql = "SELECT * FROM [dbo].[plan_EnergyConsumptionPlan_Template]  ORDER BY [ProductionLineType],[DisplayIndex]";
            return _dataFactory.Query(sql);
        }
        /// <summary>
        /// 根据公司名称获得公司信息
        /// </summary>
        /// <param name="companyName">公司名</param>
        /// <returns></returns>
        private static DataTable GetCompanyByCompanyName(string companyName)
        {
            string sql = "SELECT [A].[Name] AS CompanyName,[A].[OrganizationID],[A].LevelCode FROM [dbo].[system_Organization] AS A WHERE Name=@companyName";
            SqlParameter parameter = new SqlParameter("companyName", companyName);
            return _dataFactory.Query(sql,parameter);
        }
        /// <summary>
        /// 返回月份对照字典
        /// </summary>
        /// <returns></returns>
        private static IDictionary<int, string> InitMonthDictionary()
        {
            IDictionary<int, string> result=new Dictionary<int,string>();
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
        private static DataTable GetCompanyProductLineNumTable(string levelcode)
        {
            string sql = "SELECT COUNT(*) AS Count,Type FROM [dbo].[system_Organization] WHERE LevelCode LIKE @levelCode+'%' GROUP BY Type";
            SqlParameter parameter = new SqlParameter("levelCode", levelcode);
            return _dataFactory.Query(sql, parameter);
        }
    }
}
