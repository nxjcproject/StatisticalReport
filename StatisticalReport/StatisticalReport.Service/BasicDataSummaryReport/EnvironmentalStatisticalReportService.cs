using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Service.BasicDataSummaryReport.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class EnvironmentalStatisticalReportService
    {
        public static string CompanyName = "";
        public static string FactoryName = "";
        /// <summary>
        /// 查询所有产线的量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetEnvironmentalByOrganiztionIds(string organizationId, DateTime startDate, DateTime endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            //根据组织机构ID查询环保变量名和产线组织机构ID
            DataTable variableIdTable = GetVariableId(organizationId);
            List<Model_EnvironmentalVariableIdInfo> variableIdList = new List<Model_EnvironmentalVariableIdInfo>();
            for (int i = 0; i < variableIdTable.Rows.Count; i++)
            {
                Model_EnvironmentalVariableIdInfo variableIdModel = new Model_EnvironmentalVariableIdInfo();
                variableIdModel.OrganizationID = variableIdTable.Rows[i]["OrganizationID"].ToString();
                variableIdModel.ItemName = variableIdTable.Rows[i]["ItemName"].ToString();
                variableIdModel.VariableId = variableIdTable.Rows[i]["VariableId"].ToString();
                variableIdList.Add(variableIdModel);
            }
            string m_Sql = @"SELECT X.CompanyName,X.FactoryName,X.ItemName,X.variableid,X.OrganizationID,Y.FirstB,Y.SecondB,Y.ThirdB,Y.PeakB,Y.ValleyB,Y.FlatB,Y.A班,Y.B班,Y.C班,Y.D班,Y.TotalPeakValleyFlatB
                                   FROM 
                                  ((SELECT C.Name as CompanyName,A.Name as FactoryName, B.OrganizationID,B.LevelCode,B.Type,B.LevelType,B.ENABLED,D.ItemName,D.VariableId
                                   FROM system_Organization A
                           left join
                                   system_Organization C on C.LevelCode = substring(A.LevelCode,1,len(A.LevelCode)-2),
                                   system_Organization B,
	                               realtime_KeyIndicatorsMonitorContrast D
                                   where A.OrganizationID = @OrganizationID
                                       and B.LevelCode like A.LevelCode + '%'     
                                       and B.Type = '熟料'
                                       and A.ENABLED = 1
                                       and B.OrganizationID = D.OrganizationID
                                       and D.PageId = 'EnvironmentalMonitor'
									   ) X
                                  left join
                                  (SELECT F.OrganizationID, F.VariableId, 
                                            (F.FirstB) AS FirstB,(F.SecondB) AS SecondB,(F.ThirdB) AS ThirdB,(F.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,
                                            (F.PeakB) AS PeakB,(F.ValleyB) AS ValleyB,(F.FlatB) AS FlatB,
                                            (CASE WHEN [E].[FirstWorkingTeam] = 'A班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'A班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'A班' THEN [F].[ThirdB] ELSE 0 END) AS A班,
		                                    (CASE WHEN [E].[FirstWorkingTeam] = 'B班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'B班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'B班' THEN [F].[ThirdB] ELSE 0 END) AS B班,
		                                    (CASE WHEN [E].[FirstWorkingTeam] = 'C班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'C班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'C班' THEN [F].[ThirdB] ELSE 0 END) AS C班,
		                                    (CASE WHEN [E].[FirstWorkingTeam] = 'D班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'D班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'D班' THEN [F].[ThirdB] ELSE 0 END) AS D班	                
					                                    from tz_Balance E, balance_Environmental F 
		                                                where E.TimeStamp >= @startDate
		                                                and E.TimeStamp <= @endDate
		                                                and E.StaticsCycle = 'day'
		                                                and E.BalanceId = F.KeyId
		                                                and F.ValueType = 'Environmental'  
		                                                and E.OrganizationID = @OrganizationID
		                                                ) Y
					                                    ON X.VariableId=Y.VariableId and X.OrganizationID=Y.OrganizationID) 
                                    ORDER BY X.LevelCode,X.ItemName";
            SqlParameter[] paras = { new SqlParameter("OrganizationID", organizationId), new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate) };
            DataTable sourceTable = dataFactory.Query(m_Sql, paras);
            if (sourceTable.Rows.Count > 0)
            {
                CompanyName = sourceTable.Rows[0]["CompanyName"].ToString();
                FactoryName = sourceTable.Rows[0]["FactoryName"].ToString();
            }
            List<Model_EnvironmentSumCol> sumColList = new List<Model_EnvironmentSumCol>();
            List<Model_EnvironmentCountCol> countColList = new List<Model_EnvironmentCountCol>();

            for (int i = 0; i < variableIdList.Count; i++)
            {
                Model_EnvironmentSumCol sumColModel = new Model_EnvironmentSumCol();
                Model_EnvironmentCountCol countModel = new Model_EnvironmentCountCol();
                DataRow[] drArr = sourceTable.Select("OrganizationID='" + variableIdList[i].OrganizationID + "' and variableid='" + variableIdList[i].VariableId + "'");
                for (int j = 0; j < drArr.Length; j++)
                {
                    sumColModel.OrganizationID = drArr[j]["OrganizationID"].ToString();
                    sumColModel.ItemName = drArr[j]["ItemName"].ToString();
                    sumColModel.VariableId = drArr[j]["variableid"].ToString();
                    sumColModel.SumFirstB += Convert.ToDecimal(drArr[j]["FirstB"]);
                    sumColModel.SumSecondB += Convert.ToDecimal(drArr[j]["SecondB"]);
                    sumColModel.SumThirdB += Convert.ToDecimal(drArr[j]["ThirdB"]);
                    sumColModel.SumPeakB += Convert.ToDecimal(drArr[j]["PeakB"]);
                    sumColModel.SumValleyB += Convert.ToDecimal(drArr[j]["ValleyB"]);
                    sumColModel.SumFlatB += Convert.ToDecimal(drArr[j]["FlatB"]);
                    sumColModel.SumA班 += Convert.ToDecimal(drArr[j]["A班"]);
                    sumColModel.SumB班 += Convert.ToDecimal(drArr[j]["B班"]);
                    sumColModel.SumC班 += Convert.ToDecimal(drArr[j]["C班"]);
                    sumColModel.SumD班 += Convert.ToDecimal(drArr[j]["D班"]);

                    countModel.OrganizationID = drArr[j]["OrganizationID"].ToString();
                    countModel.ItemName = drArr[j]["ItemName"].ToString();
                    countModel.VariableId = drArr[j]["variableid"].ToString();
                    if (Convert.ToDouble(drArr[j]["FirstB"]) != 0.0)
                    {
                        countModel.CountFirstBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["SecondB"]) != 0.0)
                    {
                        countModel.CountSecondBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["ThirdB"]) != 0.0)
                    {
                        countModel.CountThirdBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["PeakB"]) != 0.0)
                    {
                        countModel.CountPeakBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["ValleyB"]) != 0.0)
                    {
                        countModel.CountValleyBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["FlatB"]) != 0.0)
                    {
                        countModel.CountFlatBNotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["A班"]) != 0.0)
                    {
                        countModel.CountA班NotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["B班"]) != 0.0)
                    {
                        countModel.CountB班NotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["C班"]) != 0.0)
                    {
                        countModel.CountC班NotZero += 1;
                    }
                    if (Convert.ToDouble(drArr[j]["D班"]) != 0.0)
                    {
                        countModel.CountD班NotZero += 1;
                    }
                }
                countColList.Add(countModel);
                sumColList.Add(sumColModel);
            }

            DataTable resultTable = sourceTable.Clone();
            for (int i = 0; i < sumColList.Count; i++)
            {
                DataRow dr = resultTable.NewRow();
                dr["CompanyName"] = CompanyName;
                dr["FactoryName"] = FactoryName;
                dr["ItemName"] = sumColList[i].ItemName;
                dr["variableid"] = sumColList[i].VariableId;
                dr["OrganizationID"] = sumColList[i].OrganizationID;
                dr["FirstB"] = countColList[i].CountFirstBNotZero == 0 ? 0 : sumColList[i].SumFirstB / countColList[i].CountFirstBNotZero;
                dr["SecondB"] = countColList[i].CountSecondBNotZero == 0 ? 0 : sumColList[i].SumSecondB / countColList[i].CountSecondBNotZero;
                dr["ThirdB"] = countColList[i].CountThirdBNotZero == 0 ? 0 : sumColList[i].SumThirdB / countColList[i].CountThirdBNotZero;
                dr["PeakB"] = countColList[i].CountPeakBNotZero == 0 ? 0 : sumColList[i].SumPeakB / countColList[i].CountPeakBNotZero;
                dr["ValleyB"] = countColList[i].CountValleyBNotZero == 0 ? 0 : sumColList[i].SumValleyB / countColList[i].CountValleyBNotZero;
                dr["FlatB"] = countColList[i].CountFlatBNotZero == 0 ? 0 : sumColList[i].SumFlatB / countColList[i].CountFlatBNotZero;
                dr["A班"] = countColList[i].CountA班NotZero == 0 ? 0 : sumColList[i].SumA班 / countColList[i].CountA班NotZero;
                dr["B班"] = countColList[i].CountB班NotZero == 0 ? 0 : sumColList[i].SumB班 / countColList[i].CountB班NotZero;
                dr["C班"] = countColList[i].CountC班NotZero == 0 ? 0 : sumColList[i].SumC班 / countColList[i].CountC班NotZero;
                dr["D班"] = countColList[i].CountD班NotZero == 0 ? 0 : sumColList[i].SumD班 / countColList[i].CountD班NotZero;
                int num = 0;//为了求最后的合计列，需要用甲乙丙求和，然后除以甲乙丙非零的个数，比如丙是0，那么就除以2。首先获得这个除数
                if (Convert.ToDecimal(dr["FirstB"]) != 0.0m)
                {
                    num += 1;
                }
                if (Convert.ToDecimal(dr["SecondB"]) != 0.0m)
                {
                    num += 1;
                }
                if (Convert.ToDecimal(dr["ThirdB"]) != 0.0m)
                {
                    num += 1;
                }
                dr["TotalPeakValleyFlatB"] = num == 0 ? 0 : (Convert.ToDecimal(dr["FirstB"]) + Convert.ToDecimal(dr["SecondB"]) + Convert.ToDecimal(dr["ThirdB"])) / num;
                resultTable.Rows.Add(dr);
            }
            for (int i = 0; i < resultTable.Rows.Count; i++)
            {
                for (int j = 5; j < resultTable.Columns.Count; j++)
                {
                    if (Convert.ToDecimal(resultTable.Rows[i][j])==0.0m)
                    {
                        resultTable.Rows[i][j] = DBNull.Value;
                    }
                }
            }

            return resultTable;
        }


        public static DataTable GetShiftsSchedulingLogMonthly(string organizationId, string startDate, string endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string sql = @" SELECT [TimeStamp],[FirstWorkingTeam],[SecondWorkingTeam],[ThirdWorkingTeam]
                              FROM [tz_Balance]
                              WHERE TimeStamp>=@startDate AND TimeStamp<=@endDate
		                            and StaticsCycle = 'day' AND
		                            [OrganizationID] = @organizationId
                              ORDER BY [TimeStamp]";
            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startDate", startDate),
                new SqlParameter("endDate",endDate)
            };
            DataTable result2 = dataFactory.Query(sql, parameters);
            return result2;
        }
        

        public static DataTable GetVariableId(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"SELECT B.OrganizationID,D.ItemName,D.VariableId
                                   FROM system_Organization A
                                   left join
                                   system_Organization C on C.LevelCode = substring(A.LevelCode,1,len(A.LevelCode)-2),
                                   system_Organization B,
	                               realtime_KeyIndicatorsMonitorContrast D
                                   where A.OrganizationID = @OrganizationID
                                       and B.LevelCode like A.LevelCode + '%'     
                                       and B.Type = '熟料'
                                       and A.ENABLED = 1
                                       and B.OrganizationID = D.OrganizationID
                                       and D.PageId = 'EnvironmentalMonitor'
									   order by OrganizationID,ItemName";
            SqlParameter[] para = { new SqlParameter("OrganizationID", organizationId) };
            DataTable variableIdTable = dataFactory.Query(m_Sql, para);
            return variableIdTable;
        }


        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {
            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }
    }
}
