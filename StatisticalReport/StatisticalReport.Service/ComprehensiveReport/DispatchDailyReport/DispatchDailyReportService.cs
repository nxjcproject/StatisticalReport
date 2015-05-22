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
        //累计值（1号到昨天的累计）
        public static DataTable GetCompanyTargetCompletion(string[] levelCodes)
        {
            //目标table
            DataTable destination = new DataTable();
            DataTable itemTable = GetItemList();
            DataColumn companyRow = new DataColumn("公司", typeof(string));
            destination.Columns.Add(companyRow);
            //构造目标表的列
            foreach (DataRow dr in itemTable.Rows)
            {
                DataColumn dc = new DataColumn(dr["QuotasID"].ToString(), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }
            //获得公司和分厂信息
            DataTable company = GetCompany(levelCodes);
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

        public static DataTable GetTreeTargetComletion(string[] levelCodes,DateTime date)
        {
            /*
             *获得公司分厂的tree结构
             */ 
            string sqlTree = @"SELECT A.OrganizationID,A.LevelCode,A.Name
                                    FROM system_Organization AS A
                                    WHERE (A.LevelType='Company' 
                                    OR A.LevelType='Factory') AND
                                    ({0})
                                    ORDER BY A.LevelCode";
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
            DataTable result = _dataFactory.Query(string.Format(sqlTree, levelCodesParameter.ToString()));
            //将各项目指标添加到result
            string[] items={"cement_CementOutput","clinker_ClinkerOutput","clinker_KilnHeadPulverizedCoalInput","clinker_KilnTailPulverizedCoalInput",
                               "clinker_LimestoneInput","clinker_MixtureMaterialsOutput","clinker_PulverizedCoalInput","clinker_PulverizedCoalOutput",
                               "clinker_ElectricityQuantity","cementmill_ElectricityQuantity"};
            foreach(string item in items){
                DataColumn dc=new DataColumn(item,typeof(decimal));
                result.Columns.Add(dc);
            }
            /*
             * 将各项目指标的值增加到result
             */
            foreach (DataRow dr in result.Rows)
            {
                string sqlValue = @"SELECT B.VariableId AS VariableId,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                        FROM tz_Balance AS A,balance_Energy AS B
                                        WHERE A.BalanceId=B.KeyId AND(
                                        B.VariableId='cement_CementOutput' OR
                                        B.VariableId='clinker_ClinkerOutput' OR
                                        B.VariableId='clinker_KilnHeadPulverizedCoalInput' OR
                                        B.VariableId='clinker_KilnTailPulverizedCoalInput' OR
                                        B.VariableId='clinker_LimestoneInput' OR
                                        B.VariableId='clinker_MixtureMaterialsOutput' OR
                                        B.VariableId='clinker_PulverizedCoalInput' OR
                                        B.VariableId='clinker_PulverizedCoalOutput' OR
                                        B.VariableId='clinker_ElectricityQuantity' OR
                                        B.VariableId='cementmill_ElectricityQuantity' )AND
                                        A.TimeStamp='{0}' AND
                                        B.OrganizationID IN(SELECT OrganizationID FROM system_Organization WHERE LevelCode LIKE '{1}%')
                                        GROUP BY B.VariableId
                                        ORDER BY B.VariableId";
                DataTable valueInfo = _dataFactory.Query(string.Format(sqlValue, date.ToString("yyyy-MM-dd"), dr["LevelCode"].ToString().Trim()));
                foreach (DataRow row in valueInfo.Rows)
                {
                    dr[row["VariableId"].ToString().Trim()] = row["TotalPeakValleyFlatB"];
                }
            }
            return result;
        }
        /// <summary>
        /// 根据公司名称获得公司计划和完成情况
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static DataTable GetPlanAndTargetCompletionByCompanyName(DateTime date,string companyName,bool isShift)
        {
            //目标表（没有进行单位换算）
            DataTable destination = new DataTable();
            //目标表（进行过单位换算的）
            DataTable shiftDestination;
            //去计划和实际的项目指标
            DataTable itemTable = GetItemList();
            //DataColumn companyRow = new DataColumn("公司", typeof(string));
            //destination.Columns.Add(companyRow);
            foreach (DataRow dr in itemTable.Rows)
            {
                DataColumn dc = new DataColumn(dr["QuotasID"].ToString(), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }
            shiftDestination = destination.Clone();
            DataTable company = GetCompanyByCompanyName(companyName);

            //取计划源表
            foreach (DataRow planRow in company.Rows)
            {
                //DataTable productLineNum = GetCompanyProductLineNumTable(planRow["LevelCode"].ToString());
                //int clinkerNum = (int)productLineNum.Select("Type='熟料'")[0]["Count"];
                //int cementNum = (int)productLineNum.Select("Type='水泥磨'")[0]["Count"];
                //未进行过单位转换
                DataRow resultPlanRow = destination.NewRow();
                //进行过单位换算
                DataRow shiftResultPlanRow = shiftDestination.NewRow();
                //DataTable planSourceTable;
                string sqlPlan = @"SELECT  
                                 A.OrganizationID,A.ProductionLineType,B.QuotasID,B.{0} 
                              FROM [dbo].[tz_Plan] AS A 
                              INNER JOIN [dbo].[plan_EnergyConsumptionYearlyPlan] AS B 
                                 ON A.KeyID=B.KeyID 
                              WHERE A.Date=@year AND A.OrganizationID LIKE @organizationId+'%'
                              ORDER BY B.ProductionLineType, B.KeyID, B.DisplayIndex";
                SqlParameter[] paramaterPlan = {new SqlParameter("organizationId", planRow["OrganizationID"].ToString().Trim()),
                                                   new SqlParameter("year",date.ToString("yyyy"))};               
                //获得当前月份的英文名字
                string month = InitMonthDictionary()[Int16.Parse(DateTime.Now.ToString("MM"))];
                DataTable temp = _dataFactory.Query(string.Format(sqlPlan,month), paramaterPlan);

                StringBuilder elcFormula_Clinker = new StringBuilder();//熟料用电量
                StringBuilder coalFormula_Clinker = new StringBuilder();//熟料用煤量
                StringBuilder eleFormula_CoalMill = new StringBuilder();//煤磨用电量
                StringBuilder eleCumpFormula_raw = new StringBuilder();//生料磨用电量
                StringBuilder eleFormula_cement = new StringBuilder();//水泥综合用电量
                StringBuilder eleFormula_cementMill = new StringBuilder();//水泥磨用电量
                decimal cementOutPut = 0;//水泥产量
                decimal clinkerOutPut = 0;//熟料产量
                decimal elcOutPut = 0;//发电量
                foreach (DataRow dr in temp.Rows)
                {
                    //if ("熟料产量" == dr["QuotasID"].ToString().Trim())
                        //resultPlanRow["熟料产量"] = ReportHelper.MyToDecimal(resultPlanRow["熟料产量"]) + ReportHelper.MyToDecimal(dr[month]);
                    if ("发电量" == dr["QuotasID"].ToString().Trim())
                    {
                        //resultPlanRow["发电量"] = ReportHelper.MyToDecimal(resultPlanRow["发电量"]) + ReportHelper.MyToDecimal(dr[month]);
                        elcOutPut += ReportHelper.MyToDecimal(dr[month]);
                    }                       
                    if ("熟料产量" == dr["QuotasID"].ToString().Trim())
                    {
                        //resultPlanRow["熟料产量"] = ReportHelper.MyToDecimal(resultPlanRow["熟料产量"]) + ReportHelper.MyToDecimal(dr[month]);
                        clinkerOutPut += ReportHelper.MyToDecimal(dr[month]);
                        elcFormula_Clinker.Append(dr[month].ToString().Trim() + "*");
                        coalFormula_Clinker.Append(dr[month].ToString().Trim() + "*");
                        eleFormula_CoalMill.Append(dr[month].ToString().Trim() + "*");
                        eleCumpFormula_raw.Append(dr[month].ToString().Trim() + "*");
                        //***********
                    }
                    if ("熟料电耗" == dr["QuotasID"].ToString().Trim())
                        elcFormula_Clinker.Append(dr[month].ToString().Trim() + "+");
                    if ("熟料煤耗" == dr["QuotasID"].ToString().Trim())
                    {
                        coalFormula_Clinker.Append(dr[month].ToString().Trim() + "/1000+");
                        eleFormula_CoalMill.Append(dr[month].ToString().Trim() + "/1000*");
                    }
                    if ("煤磨电耗" == dr["QuotasID"].ToString().Trim())
                        eleFormula_CoalMill.Append(dr[month].ToString().Trim() + "+");
                    if ("生料磨电耗" == dr["QuotasID"].ToString().Trim())
                        eleCumpFormula_raw.Append(dr[month].ToString().Trim() + "+");
                    if ("水泥产量" == dr["QuotasID"].ToString().Trim())
                    {
                        //resultPlanRow["水泥产量"] = ReportHelper.MyToDecimal(resultPlanRow["水泥产量"]) + ReportHelper.MyToDecimal(dr[month]);
                        cementOutPut += ReportHelper.MyToDecimal(dr[month]);
                        eleFormula_cement.Append(dr[month].ToString().Trim() + "*");
                        eleFormula_cementMill.Append(dr[month].ToString().Trim() + "*");
                    }
                    if ("水泥电耗" == dr["QuotasID"].ToString().Trim())
                        eleFormula_cement.Append(dr[month].ToString().Trim() + "+");
                    if ("水泥磨电耗" == dr["QuotasID"].ToString().Trim())
                        eleFormula_cementMill.Append(dr[month].ToString().Trim() + "+");
                }
                
                //**************************

                //熟料总用电量
                decimal clinkerElc =ReportHelper.MyToDecimal(destination.Compute(elcFormula_Clinker.Remove(elcFormula_Clinker.Length - 1, 1).ToString(), "true"));
                //总用煤量
                decimal coal = ReportHelper.MyToDecimal(destination.Compute(coalFormula_Clinker.Remove(coalFormula_Clinker.Length - 1, 1).ToString(), "true"));
                //生料磨总电量
                decimal rawMillElc = ReportHelper.MyToDecimal(destination.Compute(eleCumpFormula_raw.Remove(eleCumpFormula_raw.Length - 1, 1).ToString(), "true"));
                //煤磨用电量
                decimal coalMillElc = ReportHelper.MyToDecimal(destination.Compute(eleFormula_CoalMill.Remove(eleFormula_CoalMill.Length - 1, 1).ToString(), "true"));
                //水泥综合用电量
                decimal cementElc = ReportHelper.MyToDecimal(destination.Compute(eleFormula_cement.Remove(eleFormula_cement.Length - 1, 1).ToString(), "true"));
                //水泥磨用电量
                decimal cementMillElc = ReportHelper.MyToDecimal(destination.Compute(eleFormula_cementMill.Remove(eleFormula_cementMill.Length - 1, 1).ToString(), "true"));
               /*
                 ************
                 * 填写总计划
                 ************
               */
                //*************没转换单位的********************
                //熟料
                resultPlanRow["熟料产量"] = clinkerOutPut;
                resultPlanRow["发电量"] = elcOutPut;
                resultPlanRow["吨熟料发电量"] = elcOutPut / clinkerOutPut;
                resultPlanRow["熟料电耗"] = elcOutPut / clinkerOutPut;
                resultPlanRow["熟料煤耗"] = coal*1000 / clinkerOutPut;
                resultPlanRow["生料磨电耗"] = rawMillElc / clinkerOutPut;
                resultPlanRow["煤磨电耗"] = coalMillElc / coal;
                //水泥
                resultPlanRow["水泥产量"] = cementOutPut;
                resultPlanRow["水泥电耗"] = cementElc / cementOutPut;
                resultPlanRow["水泥磨电耗"] = cementMillElc / cementOutPut;
                destination.Rows.Add(resultPlanRow);
                //*************END没转换单位的******************


                //*************转换过单位的********************
                //熟料
                shiftResultPlanRow["熟料产量"] = clinkerOutPut/1000;
                shiftResultPlanRow["发电量"] = elcOutPut/100000;
                shiftResultPlanRow["吨熟料发电量"] = elcOutPut / clinkerOutPut;
                shiftResultPlanRow["熟料电耗"] = elcOutPut / clinkerOutPut;
                shiftResultPlanRow["熟料煤耗"] = coal * 1000 / clinkerOutPut/10;
                shiftResultPlanRow["生料磨电耗"] = rawMillElc / clinkerOutPut;
                shiftResultPlanRow["煤磨电耗"] = coalMillElc / coal;
                //水泥
                shiftResultPlanRow["水泥产量"] = cementOutPut/1000;
                shiftResultPlanRow["水泥电耗"] = cementElc / cementOutPut;
                shiftResultPlanRow["水泥磨电耗"] = cementMillElc / cementOutPut;
                shiftDestination.Rows.Add(shiftResultPlanRow);
                //*************END转换过单位的******************
            }
            //填写实际完成情况
            foreach (DataRow dr in company.Rows)
            {
                //未单位转换
                DataRow row = destination.NewRow();
                //单位转换过的
                DataRow shiftRow = shiftDestination.NewRow();
                //row["公司"] = dr["CompanyName"].ToString().Trim();
                string organizationId = dr["OrganizationID"].ToString();
                string sql = @"SELECT SUM([B].[TotalPeakValleyFlatB]) AS Value,[VariableId]
                                    FROM [dbo].[balance_Energy] AS B INNER JOIN [dbo].[tz_Balance] AS A
	                                ON [A].[BalanceId]=[B].[KeyId]
	                                WHERE [B].[OrganizationID] LIKE @organizationId + '%' AND      
	                                [A].StaticsCycle='day' AND
	                                [A].[TimeStamp]>=@startDate AND
	                                [A].[TimeStamp]<=@endDate AND 
	                                (
	                                [B].[VariableId]='coalPreparation_ElectricityQuantity' OR [B].[VariableId]='clinker_PulverizedCoalOutput' OR [B].[VariableId]='rawMaterialsPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_MixtureMaterialsOutput' OR [B].[VariableId]='clinkerPreparation_ElectricityQuantity'
	                                OR [B].[VariableId]='clinker_ClinkerOutput' OR [B].[VariableId]='clinker_PulverizedCoalInput' 
                                    OR [B].[VariableId]='cement_CementOutput' OR [B].[VariableId]='cementmill_ElectricityQuantity' 
                                    OR [B].[VariableId]='cementGrind_ElectricityQuantity' 
                                    OR [B].[VariableId]='clinkerElectricityGeneration_ElectricityQuantity' 
	                                )
                                    GROUP BY [B].[VariableId]
                             ";
                SqlParameter[] paramaters = {new SqlParameter("startDate",date.ToString("yyyy-MM")+"-01"),
                                                new SqlParameter("endDate",date.ToString("yyyy-MM-dd")),
                                                new SqlParameter("organizationId", organizationId)};
                DataTable sourceTable = _dataFactory.Query(sql, paramaters);
                foreach (DataRow drow in sourceTable.Rows)
                {
                    switch (drow["VariableId"].ToString().Trim())
                    {
                        case "clinker_ClinkerOutput":
                            row["熟料产量"] = (decimal)drow["Value"] / Rate;
                            shiftRow["熟料产量"] = (decimal)drow["Value"] / 1000;
                            //**********test(为保险)发电量在前或者熟料产量在前都可以
                            row["吨熟料发电量"] = (decimal)row["发电量"] == 0 ? 0 : (decimal)row["发电量"] / ((decimal)drow["Value"]);
                            shiftRow["吨熟料发电量"] = (decimal)row["发电量"] == 0 ? 0 : (decimal)row["发电量"] / ((decimal)drow["Value"]);
                            break;
                        case "clinkerElectricityGeneration_ElectricityQuantity":
                            row["发电量"] = (decimal)drow["Value"];
                            shiftRow["发电量"] = (decimal)drow["Value"] / 100000;
                            //**********
                            row["吨熟料发电量"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["熟料产量"]);
                            shiftRow["吨熟料发电量"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["熟料产量"]);
                            break;
                        case "clinkerPreparation_ElectricityQuantity":
                            row["熟料电耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["熟料产量"] * Rate);
                            shiftRow["熟料电耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["熟料产量"] * Rate);
                            break;
                        case "clinker_PulverizedCoalInput":
                            row["熟料煤耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] * 1000 / ((decimal)row["熟料产量"] * Rate);
                            shiftRow["熟料煤耗"] = (decimal)row["熟料产量"] == 0 ? 0 : (decimal)drow["Value"] * 1000 / ((decimal)row["熟料产量"])/10;
                            break;
                        case "rawMaterialsPreparation_ElectricityQuantity":
                            DataRow[] rawMaterialsoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (rawMaterialsoutputRows.Count() == 1)
                            {
                                row["生料磨电耗"] = (decimal)rawMaterialsoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)rawMaterialsoutputRows[0]["Value"];
                                shiftRow["生料磨电耗"] = (decimal)rawMaterialsoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)rawMaterialsoutputRows[0]["Value"];
                            }
                            break;
                        case "coalPreparation_ElectricityQuantity":
                            DataRow[] coalPreparationoutputRows = sourceTable.Select("VariableId='clinker_MixtureMaterialsOutput'");
                            if (coalPreparationoutputRows.Count() == 1)
                            {
                                row["煤磨电耗"] = (decimal)coalPreparationoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)coalPreparationoutputRows[0]["Value"];
                                shiftRow["煤磨电耗"] = (decimal)coalPreparationoutputRows[0]["Value"] == 0 ? 0 : (decimal)drow["Value"] / (decimal)coalPreparationoutputRows[0]["Value"];
                            }
                            break;
                        case "cement_CementOutput":
                            row["水泥产量"] = (decimal)drow["Value"] / Rate;
                            shiftRow["水泥产量"] = (decimal)drow["Value"] / 1000;
                            break;
                        case "cementmill_ElectricityQuantity":
                            row["水泥电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["水泥产量"] * Rate);
                            shiftRow["水泥电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["水泥产量"] * Rate);
                            break;
                        case "cementGrind_ElectricityQuantity":
                            row["水泥磨电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["水泥产量"] * Rate);
                            shiftRow["水泥磨电耗"] = (decimal)row["水泥产量"] == 0 ? 0 : (decimal)drow["Value"] / ((decimal)row["水泥产量"] * Rate);
                            break;
                    }
                }

                destination.Rows.Add(row);
                shiftDestination.Rows.Add(shiftRow);
            }
            
            if (isShift)
                return shiftDestination;//返回转换过单位的
            else
                return destination;//返回没有转换过单位的
        }
        /// <summary>
        /// 获取日计划和完成情况之差
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static DataTable GetDailyGapPlanAndTargetCompletion(string companyName,DateTime date)
        {
            //构造目标表
            DataTable destination = new DataTable();
            DataColumn dcItemName = new DataColumn("项目指标", typeof(string));
            dcItemName.DefaultValue = 0;
            DataColumn dayCompletion = new DataColumn("日完成", typeof(decimal));
            dcItemName.DefaultValue = 0;
            //DataColumn dcPlan = new DataColumn("日平均计划", typeof(decimal));
            DataColumn dcPlan = new DataColumn("月计划", typeof(decimal));
            dcPlan.DefaultValue = 0;
            //DataColumn dcCompletion = new DataColumn("日平均完成", typeof(decimal));
            DataColumn dcCompletion = new DataColumn("月累计完成", typeof(decimal));
            dcCompletion.DefaultValue = 0;
            DataColumn dcGap = new DataColumn("月差值", typeof(decimal));
            dcGap.DefaultValue = 0;
            destination.Columns.Add(dcItemName);
            destination.Columns.Add(dayCompletion);
            destination.Columns.Add(dcPlan);
            destination.Columns.Add(dcCompletion);
            destination.Columns.Add(dcGap);
            //获取数据源表
            DataTable sourceTable = GetPlanAndTargetCompletionByCompanyName(date,companyName,false);
            //将数据源转化为目标表            
            foreach (DataColumn dc in sourceTable.Columns)
            {
                //本月的天数
                //int denominatorPlan = Int16.Parse(DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day)).ToString("dd"));
                ////到昨天的天数
                //int denominatorComplet = DateTime.Now.Day-1;

                DataRow row = destination.NewRow();
                string itemName = dc.ColumnName;
                row["项目指标"] = itemName;
                //if (itemName == "熟料产量" || itemName == "发电量" || itemName == "水泥产量")
                //{
                //    row["日平均计划"] = decimal.Parse(((decimal)sourceTable.Rows[0][itemName] * Rate / denominatorPlan).ToString("#0.00"));
                //    row["日平均完成"] = decimal.Parse(((decimal)sourceTable.Rows[1][itemName] * Rate / denominatorComplet).ToString("#0.00"));
                //}
                //else
                //{
                //    row["日平均计划"] = decimal.Parse(((decimal)sourceTable.Rows[0][itemName]).ToString("#0.00"));
                //    row["日平均完成"] = decimal.Parse(((decimal)sourceTable.Rows[1][itemName]).ToString("#0.00"));
                //}
                //row["差值"] = (decimal)row["日平均计划"] - (decimal)row["日平均完成"];
                row["月计划"]=(decimal)sourceTable.Rows[0][itemName];
                row["月累计完成"] = (decimal)sourceTable.Rows[1][itemName];
                row["月差值"] = (decimal)row["月计划"] - (decimal)row["月累计完成"];
                destination.Rows.Add(row);
            }           
            DataTable dailyComplete = DailyComplete(companyName, date);
            for (int i = 0; i < destination.Rows.Count; i++)
            {
                if (destination.Rows[i]["项目指标"].ToString().Trim() == dailyComplete.Rows[i]["项目指标"].ToString().Trim() )
                    destination.Rows[i]["日完成"] = dailyComplete.Rows[i]["日完成"];
            }

            return destination;
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
                DataTable alarmTable = _dataFactory.Query(string.Format(sql,date.ToString("yyyy-MM-dd"),date.ToString("yyyy-MM-dd")), parameter);

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
        /// <summary>
        /// 获取公司级别的具体公司
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCompany(string[] levelCodes)
        {
            //ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            string sql = "SELECT [A].[Name] AS CompanyName,[A].[OrganizationID] FROM [dbo].[system_Organization] AS A WHERE A.[LevelType]='Company' OR A.[LevelType]='Factory' AND {0}";
            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in levelCodes)
            {
                levelCodesParameter.Append("A.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode + "%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 4, 4);
            return _dataFactory.Query(string.Format(sql,levelCodesParameter.ToString()));
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
            return _dataFactory.Query(sql, parameter);
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
        private static DataTable GetCompanyProductLineNumTable(string levelcode)
        {
            string sql = "SELECT COUNT(*) AS Count,Type FROM [dbo].[system_Organization] WHERE LevelCode LIKE @levelCode+'%' GROUP BY Type";
            SqlParameter parameter = new SqlParameter("levelCode", levelcode);
            return _dataFactory.Query(sql, parameter);
        }
        /// <summary>
        /// 日完成情况
        /// </summary>
        /// <returns></returns>
        private static DataTable DailyComplete(string companyName, DateTime date)
        {
            DataTable company = GetCompanyByCompanyName(companyName);
            string levelcode = company.Rows[0]["LevelCode"].ToString().Trim();
            DataTable result = new DataTable();
            DataColumn itemColumn = new DataColumn("项目指标", typeof(string));
            DataColumn dailyComplete = new DataColumn("日完成", typeof(decimal));
            result.Columns.Add(itemColumn);
            result.Columns.Add(dailyComplete);
            string sql = @"SELECT B.VariableId,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                            FROM tz_Balance AS A,balance_Energy AS B
                            WHERE A.BalanceId=B.KeyId AND
                            (
                            [B].[VariableId]='coalPreparation_ElectricityQuantity' OR [B].[VariableId]='clinker_PulverizedCoalOutput' OR [B].[VariableId]='rawMaterialsPreparation_ElectricityQuantity'
                            OR [B].[VariableId]='clinker_MixtureMaterialsOutput' OR [B].[VariableId]='clinkerPreparation_ElectricityQuantity'
                            OR [B].[VariableId]='clinker_ClinkerOutput' OR [B].[VariableId]='clinker_PulverizedCoalInput' 
                            OR [B].[VariableId]='cement_CementOutput' OR [B].[VariableId]='cementmill_ElectricityQuantity' 
                            OR [B].[VariableId]='cementGrind_ElectricityQuantity' OR [B].[VariableId]='cementPreparation_ElectricityQuantity'
                            OR [B].[VariableId]='clinkerElectricityGeneration_ElectricityQuantity'
                            OR [B].[VariableId]='clinker_ElectricityQuantity'
                            ) AND
                            [A].[TimeStamp]='{0}' AND                          
                            A.StaticsCycle='day' AND
                            B.OrganizationID IN (select OrganizationID from system_Organization where LevelCode like '{1}%')
                            GROUP BY B.VariableId";
            DataTable source = _dataFactory.Query(string.Format(sql, date.ToString("yyyy-MM-dd"), levelcode));
            string sqlTemplete = @"SELECT VariableId,VariableName,ValueFormula
                                    FROM balance_Energy_Template
                                    WHERE VariableId='clinker_ElectricityConsumption' OR
                                    VariableId='clinker_CoalConsumption' OR
                                    VariableId='rawMaterialsPreparation_ElectricityConsumption' OR
                                    VariableId='coalPreparation_ElectricityConsumption' OR
                                    VariableId='cementPreparation_ElectricityConsumption'OR
                                    VariableId='cementmill_ElectricityConsumption'";
            DataTable template = _dataFactory.Query(sqlTemplete);
            DataTable consumption = EnergyConsumption.EnergyConsumptionCalculate.Calculate(source, template, "ValueFormula", new string[] { "TotalPeakValleyFlatB" });
            DataRow row1 = result.NewRow();
            row1["项目指标"] = "熟料产量";

            decimal ClinkerOutput = 0;
            if (source.Select("VariableId='clinker_ClinkerOutput'").Count() != 0)
            {
                ClinkerOutput = ReportHelper.MyToDecimal(source.Select("VariableId='clinker_ClinkerOutput'")[0]["TotalPeakValleyFlatB"]);
                row1["日完成"] = source.Select("VariableId='clinker_ClinkerOutput'")[0]["TotalPeakValleyFlatB"];
            }
            result.Rows.Add(row1);
            DataRow row2 = result.NewRow();
            row2["项目指标"] = "发电量";

            decimal ElectricityGeneration = 0;
            if (source.Select("VariableId='clinkerElectricityGeneration_ElectricityQuantity'").Count() != 0)
            {
                ElectricityGeneration =ReportHelper.MyToDecimal(source.Select("VariableId='clinkerElectricityGeneration_ElectricityQuantity'")[0]["TotalPeakValleyFlatB"]);
                row2["日完成"] = source.Select("VariableId='clinkerElectricityGeneration_ElectricityQuantity'")[0]["TotalPeakValleyFlatB"];              
            }
            result.Rows.Add(row2);
            DataRow row3 = result.NewRow();
            row3["项目指标"] = "吨熟料发电量";
            row3["日完成"] = ClinkerOutput == 0 ? 0 : ElectricityGeneration / ClinkerOutput;
            result.Rows.Add(row3);
            DataRow row4 = result.NewRow();
            row4["项目指标"] = "熟料电耗";
            if (consumption.Select("VariableId='clinker_ElectricityConsumption'").Count() != 0)
                row4["日完成"] = consumption.Select("VariableId='clinker_ElectricityConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row4);

            DataRow row5 = result.NewRow();
            row5["项目指标"] = "熟料煤耗";
            if (consumption.Select("VariableId='clinker_CoalConsumption'").Count() != 0)
                row5["日完成"] = consumption.Select("VariableId='clinker_CoalConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row5);

            DataRow row6 = result.NewRow();
            row6["项目指标"] = "生料磨电耗";
            if (consumption.Select("VariableId='rawMaterialsPreparation_ElectricityConsumption'").Count() != 0)
                row6["日完成"] = consumption.Select("VariableId='rawMaterialsPreparation_ElectricityConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row6);

            DataRow row7 = result.NewRow();
            row7["项目指标"] = "煤磨电耗";
            if (consumption.Select("VariableId='coalPreparation_ElectricityConsumption'").Count() != 0)
                row7["日完成"] = consumption.Select("VariableId='coalPreparation_ElectricityConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row7);

            DataRow row8 = result.NewRow();
            row8["项目指标"] = "水泥产量";
            if (source.Select("VariableId='cement_CementOutput'").Count() != 0)
                row8["日完成"] = source.Select("VariableId='cement_CementOutput'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row8);

            DataRow row9= result.NewRow();
            row9["项目指标"] = "水泥电耗";
            if (consumption.Select("VariableId='cementmill_ElectricityConsumption'").Count() != 0)
                row9["日完成"] = consumption.Select("VariableId='cementmill_ElectricityConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row9);

            DataRow row10 = result.NewRow();
            row10["项目指标"] = "水泥磨电耗";
            if (consumption.Select("VariableId='cementPreparation_ElectricityConsumption'").Count() != 0)
                row10["日完成"] = consumption.Select("VariableId='cementPreparation_ElectricityConsumption'")[0]["TotalPeakValleyFlatB"];
            result.Rows.Add(row10);
            return result;
        }
    }
}
