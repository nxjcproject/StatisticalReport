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
        /// 查询所有产线的用煤量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetCoalUsageDailyByOrganiztionIds(string[] levelCodes, DateTime startDate, DateTime endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);                          
//            string queryString = @" SELECT 
//                                    W.CompanyName,
//                                    W.FactoryName,
//                                    TB.OrganizationID AS FactoryOrgID,
//                                    BE.VariableName AS VariableName,
//                                    BE.FirstB AS FirstB,
//                                    BE.SecondB AS SecondB,
//                                    BE.ThirdB AS ThirdB,
//                                    BE.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
//                                    FROM tz_Balance AS TB,
//                                    balance_Energy AS BE,
//                                      (SELECT C.Name AS CompanyName,
//	                                    F.Name AS FactoryName,
//	                                    F.OrganizationID AS FactoryOrganizationID
//	                                    FROM system_Organization AS C,
//	                                    system_Organization AS F
//	                                    WHERE LEN(C.LevelCode)=3 AND 
//	                                    LEN(F.LevelCode)=5 AND
//	                                    left(F.LevelCode,3)=C.LevelCode
//	                                    ) AS W,
//                                       (
//										SELECT A.OrganizationID FROM system_Organization AS A WHERE {0}
//										) AS O
//                                    WHERE BE.KeyId=TB.BalanceId AND
//                                    BE.VariableId='clinker_PulverizedCoalInput' AND
//                                    TB.TimeStamp='{1}' AND
//                                    W.FactoryOrganizationID=TB.OrganizationID AND
//                                    TB.[OrganizationID]=O.OrganizationID
//                                ";

             string queryString = @" SELECT 
                                    W.CompanyName,
                                    W.FactoryName,
                                    TB.OrganizationID AS FactoryOrgID,
                                    BE.VariableName AS VariableName,
                                    SUM(BE.FirstB) AS FirstB,
                                    SUM(BE.SecondB) AS SecondB,
                                    SUM(BE.ThirdB) AS ThirdB,
                                    SUM(BE.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
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
										SELECT A.OrganizationID,A.LevelCode FROM system_Organization AS A WHERE {2}
										) AS O
                                    WHERE BE.KeyId=TB.BalanceId AND
                                    BE.VariableId='clinker_PulverizedCoalInput' AND
                                    TB.TimeStamp>='{0}' AND TB.TimeStamp<='{1}' AND
                                    W.FactoryOrganizationID=TB.OrganizationID AND
                                    TB.[OrganizationID]=O.OrganizationID
									GROUP BY W.CompanyName,W.FactoryName,TB.OrganizationID,BE.VariableName,O.LevelCode
                                    ORDER BY O.LevelCode";

            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in levelCodes)
            {
                levelCodesParameter.Append("A.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode+"%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 4, 4);
            string mSqlQuery = string.Format(queryString, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), levelCodesParameter.ToString());
            DataTable table = dataFactory.Query(mSqlQuery);
            return table;
        }
    }
}