using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.ComprehensiveReport
{
    public static class CoalUsageReportService
    {
        /// <summary>
        /// 查询所有产线的用煤量（前一天）
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <returns></returns>
        public static DataTable GetCoalUsageDailyByOrganiztionIds(string[] levelCodes)
        {
            return GetCoalUsageDailyByOrganiztionIds(levelCodes, DateTime.Now.AddDays(-1));
        }

        /// <summary>
        /// 查询所有产线的用煤量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetCoalUsageDailyByOrganiztionIds(string[] levelCodes, DateTime time)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

//            string queryString = @" SELECT [C].*, [A].[TimeStamp]
//                                      FROM [balance_Energy] AS [C] INNER JOIN
//                                           [tz_Balance] AS [A] ON [C].[KeyId] = [A].[BalanceId]
//                                     WHERE ([A].[StaticsCycle] = 'day') AND 
//                                           ([A].[TimeStamp] = '{1}') AND
//                                   	       ([C].[VariableId] IN ('clinker_PulverizedCoalInput')) AND
//                                   	       ([C].[OrganizationID] IN 
//                                   	       (SELECT [OA].[OrganizationID]
//                                              FROM [system_Organization] AS [OA] INNER JOIN
//                                                   [system_Organization] AS [OB] ON [OA].[LevelCode] LIKE ([OB].[LevelCode] + '%')
//                                             WHERE [OB].[OrganizationID] IN ({0})))
//                                ";
            string queryString = @" SELECT 
                                    W.CompanyName,
                                    W.FactoryName,
                                    TB.OrganizationID AS FactoryOrgID,
                                    BE.VariableName AS VariableName,
                                    BE.FirstB AS FirstB,
                                    BE.SecondB AS SecondB,
                                    BE.ThirdB AS ThirdB,
                                    BE.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                    FROM tz_Balance AS TB,
                                    balance_Energy AS BE,
                                      (SELECT C.Name AS CompanyName,
	                                    F.Name AS FactoryName,
	                                    F.OrganizationID AS FactoryOrganizationID
	                                    FROM system_Organization AS C,
	                                    system_Organization AS F
	                                    WHERE LEN(C.LevelCode)=3 AND 
	                                    LEN(F.LevelCode)=5 AND
	                                    left(F.LevelCode,3)=C.LevelCode
	                                    ) AS W,
                                       (
										SELECT A.OrganizationID FROM system_Organization AS A WHERE {0}
										) AS O
                                    WHERE BE.KeyId=TB.BalanceId AND
                                    BE.VariableId='clinker_PulverizedCoalInput' AND
                                    TB.TimeStamp='{1}' AND
                                    W.FactoryOrganizationID=TB.OrganizationID AND
                                    TB.[OrganizationID]=O.OrganizationID
                                ";

            //StringBuilder organiztionIdsParameter = new StringBuilder();
            //foreach (var organizationId in organizationIds)
            //{
            //    organiztionIdsParameter.Append("'");
            //    organiztionIdsParameter.Append(organizationId);
            //    organiztionIdsParameter.Append("',");
            //}
            //organiztionIdsParameter.Remove(organiztionIdsParameter.Length - 1, 1);
            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in levelCodes)
            {
                levelCodesParameter.Append("A.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode+"%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" AND ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 5, 5);
#if DEBUG
            return dataFactory.Query(string.Format(queryString, levelCodesParameter.ToString(), "2015-02-09"));
#else
            return dataFactory.Query(string.Format(queryString, levelCodesParameter.ToString(), time.ToString("yyyy-MM-dd")));
            //return dataFactory.Query(string.Format(queryString, organiztionIdsParameter.ToString(), time.ToString("yyyy-MM-dd")));
#endif
        }
    }
}