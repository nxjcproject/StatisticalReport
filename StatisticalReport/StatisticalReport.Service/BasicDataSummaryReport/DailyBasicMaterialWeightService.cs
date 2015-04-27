using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class DailyBasicMaterialWeightService
    {
        /// <summary>
        /// 查询所有产线的产量、消耗量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetMaterialWeightByOrganiztionIds(string organizationId, DateTime startDate,DateTime endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
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
										SELECT A.OrganizationID 
                                            FROM system_Organization AS A 
                                            WHERE A.LevelCode like (
                                                                    SELECT T.LevelCode FROM system_Organization AS T
						                                            WHERE T.OrganizationID='{2}'
						                                            )+'%'
										) AS O
                                    WHERE BE.KeyId=TB.BalanceId AND
                                    BE.ValueType='MaterialWeight' AND
                                    TB.TimeStamp>='{0}' AND TB.TimeStamp<='{1}' AND
                                    W.FactoryOrganizationID=TB.OrganizationID AND
                                    TB.[OrganizationID]=O.OrganizationID AND
									BE.VariableName<>'水泥分品种' 
									group by W.CompanyName,
                                    W.FactoryName,
                                    TB.OrganizationID,
                                    VariableName									
                                ";
            //StringBuilder levelCodesParameter = new StringBuilder();
            //foreach (var levelCode in levelCodes)
            //{
            //    levelCodesParameter.Append("A.LevelCode like ");
            //    levelCodesParameter.Append("'");
            //    levelCodesParameter.Append(levelCode + "%");
            //    levelCodesParameter.Append("'");
            //    levelCodesParameter.Append(" AND ");
            //}
            //levelCodesParameter.Remove(levelCodesParameter.Length - 5, 5);
//#if DEBUG
//            return dataFactory.Query(string.Format(queryString, levelCodesParameter.ToString(), "2015-02-09"));
//#else
            return dataFactory.Query(string.Format(queryString, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"),organizationId));
//#endif
        }
    }
}