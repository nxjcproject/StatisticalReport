using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
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
           string queryString1 = @"SELECT X.CompanyName,X.FactoryName,X.ItemName,Y.FirstB,Y.SecondB,Y.ThirdB,Y.PeakB,Y.ValleyB,Y.FlatB,Y.A班,Y.B班,Y.C班,Y.D班,Y.TotalPeakValleyFlatB
                                   FROM 
                                  ((SELECT C.Name as CompanyName,A.Name as FactoryName, B.OrganizationID,B.LevelCode,B.Type,B.LevelType,B.ENABLED,D.ItemName,D.VariableId
                                   FROM system_Organization A
                                left join
                                   system_Organization C on C.LevelCode = substring(A.LevelCode,1,len(A.LevelCode)-2),
                                   system_Organization B,
	                               realtime_KeyIndicatorsMonitorContrast D
                                   where A.OrganizationID = '{2}'
                                       and B.LevelCode like A.LevelCode + '%'
                                       and B.Type = '熟料'
                                       and A.ENABLED = 1
                                       and B.OrganizationID = D.OrganizationID
                                       and D.PageId = 'EnvironmentalMonitor') X
                                  left join
                                  (SELECT F.OrganizationID, F.VariableId, 
                                            SUM(F.FirstB) AS FirstB,SUM(F.SecondB) AS SecondB,SUM(F.ThirdB) AS ThirdB,SUM(F.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,
                                            SUM(F.PeakB) AS PeakB,SUM(F.ValleyB) AS ValleyB,SUM(F.FlatB) AS FlatB,
                                            SUM(CASE WHEN [E].[FirstWorkingTeam] = 'A班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'A班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'A班' THEN [F].[ThirdB] ELSE 0 END) AS A班,
		                                    SUM(CASE WHEN [E].[FirstWorkingTeam] = 'B班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'B班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'B班' THEN [F].[ThirdB] ELSE 0 END) AS B班,
		                                    SUM(CASE WHEN [E].[FirstWorkingTeam] = 'C班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'C班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'C班' THEN [F].[ThirdB] ELSE 0 END) AS C班,
		                                    SUM(CASE WHEN [E].[FirstWorkingTeam] = 'D班' THEN [F].[FirstB] WHEN [E].[SecondWorkingTeam] = 'D班' THEN [F].[SecondB] WHEN [E].[ThirdWorkingTeam] = 'D班' THEN [F].[ThirdB] ELSE 0 END) AS D班	                
					                                    from tz_Balance E, balance_Environmental F 
		                                                where E.TimeStamp >= '{0}'
		                                                and E.TimeStamp <= '{1}'
		                                                and E.StaticsCycle = 'day'
		                                                and E.BalanceId = F.KeyId
		                                                and F.ValueType = 'Environmental'  
		                                                and E.OrganizationID = '{2}'
		                                                group by F.OrganizationID, F.VariableId) Y
					                                    ON X.VariableId=Y.VariableId and X.OrganizationID=Y.OrganizationID) 
                                    ORDER BY X.LevelCode,X.ItemName
                                        ";
           queryString1 = string.Format(queryString1, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), organizationId);

           DataTable result1 = dataFactory.Query(queryString1);
           DataTable result2 = GetModifyEnvironmental(result1, organizationId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
           return result2;   
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
       public static DataTable GetModifyEnvironmental(DataTable environmentalTable,string organizationId, string startDate, string endDate)
       {
           DataTable shiftTable=GetShiftsSchedulingLogMonthly(organizationId, startDate, endDate);
           Dictionary<string, int> shiftDic = new Dictionary<string, int>();
           shiftDic.Add("A班", 0);
           shiftDic.Add("B班", 0);
           shiftDic.Add("C班", 0);
           shiftDic.Add("D班", 0);
           for (int i = 0; i < shiftTable.Rows.Count; i++)
           {
               for (int j = 1; j < shiftTable.Columns.Count; j++)
               {
                   if (shiftTable.Rows[i][j].ToString()=="A班")
                   {
                       shiftDic["A班"] += 1;
                   }
                   else if (shiftTable.Rows[i][j].ToString() == "B班")
                   {
                       shiftDic["B班"] += 1;
                   }
                   else if (shiftTable.Rows[i][j].ToString() == "C班")
                   {
                       shiftDic["C班"] += 1;
                   }
                   else if (shiftTable.Rows[i][j].ToString() == "D班")
                   {
                       shiftDic["D班"] += 1;
                   }
               }
           }
           DateTime m_startDate = Convert.ToDateTime(startDate);
           DateTime m_endDate = Convert.ToDateTime(endDate);
           TimeSpan span=m_endDate - m_startDate;
           int totalDays=span.Days+1;
           for (int i = 0; i < environmentalTable.Rows.Count; i++)
           {
               for (int j = 3; j < environmentalTable.Columns.Count; j++)
               {
                   if (j == 9)
                   {
                       if (shiftDic["A班"]!=0)
                       {
                           environmentalTable.Rows[i][j] = (Convert.ToDouble(environmentalTable.Rows[i][j]) / shiftDic["A班"]);
                       }
                   }
                   else if (j == 10)
                   {
                       if (shiftDic["B班"]!=0)
                       {
                           environmentalTable.Rows[i][j] = (Convert.ToDouble(environmentalTable.Rows[i][j]) / shiftDic["B班"]);
                       }
                   }
                   else if (j == 11)
                   {
                       if (shiftDic["C班"]!=0)
                       {
                           environmentalTable.Rows[i][j] = (Convert.ToDouble(environmentalTable.Rows[i][j]) / shiftDic["C班"]);
                       }
                   }
                   else if (j == 12)
                   {
                       if (shiftDic["D班"]!=0)
                       {
                           environmentalTable.Rows[i][j] = (Convert.ToDouble(environmentalTable.Rows[i][j]) / shiftDic["D班"]);
                       }
                   }
                   else
                   {
                       environmentalTable.Rows[i][j] = (Convert.ToDouble(environmentalTable.Rows[i][j]) / totalDays);
                   }
               }
           }
           return environmentalTable;
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
