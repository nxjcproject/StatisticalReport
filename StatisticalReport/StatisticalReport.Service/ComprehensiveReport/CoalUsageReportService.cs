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
        public static DataTable GetCoalUsageDailyByOrganiztionIds(string[] organizationIds)
        {
            return GetCoalUsageDailyByOrganiztionIds(organizationIds, DateTime.Now.AddDays(-1));
        }

        /// <summary>
        /// 查询所有产线的用煤量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetCoalUsageDailyByOrganiztionIds(string[] organizationIds, DateTime time)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @" SELECT [C].*, [A].[TimeStamp]
                                      FROM [balance_Energy] AS [C] INNER JOIN
                                           [tz_Balance] AS [A] ON [C].[KeyId] = [A].[BalanceId]
                                     WHERE ([A].[StaticsCycle] = 'day') AND 
                                           ([A].[TimeStamp] = '{1}') AND
                                   	       ([C].[VariableId] IN ('clinker_PulverizedCoalInput')) AND
                                   	       ([C].[OrganizationID] IN 
                                   	       (SELECT [OA].[OrganizationID]
                                              FROM [system_Organization] AS [OA] INNER JOIN
                                                   [system_Organization] AS [OB] ON [OA].[LevelCode] LIKE ([OB].[LevelCode] + '%')
                                             WHERE [OB].[OrganizationID] IN ({0})))
                                ";

            StringBuilder organiztionIdsParameter = new StringBuilder();
            foreach (var organizationId in organizationIds)
            {
                organiztionIdsParameter.Append("'");
                organiztionIdsParameter.Append(organizationId);
                organiztionIdsParameter.Append("',");
            }
            organiztionIdsParameter.Remove(organiztionIdsParameter.Length - 1, 1);

#if DEBUG
            return dataFactory.Query(string.Format(queryString, organiztionIdsParameter.ToString(), "2015-01-10"));
#else
            return dataFactory.Query(string.Format(queryString, organiztionIdsParameter.ToString(), time.ToString("yyyy-MM-dd")));
#endif
        }
    }
}