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
//            string queryString = @" SELECT 
//                                    W.CompanyName,
//                                    W.FactoryName,
//                                    TB.OrganizationID AS FactoryOrgID,
//                                    BE.VariableName AS VariableName,
//                                    SUM(BE.FirstB) AS FirstB,
//                                    SUM(BE.SecondB) AS SecondB,
//                                    SUM(BE.ThirdB) AS ThirdB,
//                                    SUM(BE.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
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
//                                        (
//										SELECT A.OrganizationID 
//                                            FROM system_Organization AS A 
//                                            WHERE A.LevelCode like (
//                                                                    SELECT T.LevelCode FROM system_Organization AS T
//						                                            WHERE T.OrganizationID='{2}'
//						                                            )+'%'
//										) AS O
//                                    WHERE BE.KeyId=TB.BalanceId AND
//                                    BE.ValueType='MaterialWeight' AND
//                                    TB.TimeStamp>='{0}' AND TB.TimeStamp<='{1}' AND
//                                    W.FactoryOrganizationID=TB.OrganizationID AND
//                                    TB.[OrganizationID]=O.OrganizationID AND
//									BE.VariableName<>'水泥分品种' 
//									group by W.CompanyName,
//                                    W.FactoryName,
//                                    TB.OrganizationID,
//                                    VariableName									
//                                ";
            //                                    W.CompanyName,
            //                                    W.FactoryName,
            //                                    TB.OrganizationID AS FactoryOrgID,
            //                                    BE.VariableName AS VariableName,
            string queryString = @"Select C.Name AS CompanyName, M.Name as FactoryName, F.OrganizationID as FactoryOrgID, N.Name + B.Name AS VariableName, F.FirstB AS FirstB, F.SecondB AS SecondB, F.ThirdB AS ThirdB, F.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                        from tz_Material A, material_MaterialDetail B, system_Organization M
                                        left join system_Organization C on substring(M.LevelCode,1,Len(M.LevelCode) - 2) = C.LevelCode, system_Organization N, 
                                            (Select E.OrganizationID, E.VariableId, SUM(E.FirstB) AS FirstB,SUM(E.SecondB) AS SecondB,SUM(E.ThirdB) AS ThirdB,SUM(E.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
	                                            from tz_Balance D, balance_Energy E 
		                                        where D.TimeStamp >= '{0}'
		                                        and D.TimeStamp <= '{1}'
		                                        and D.StaticsCycle = 'day'
		                                        and D.BalanceId = E.KeyId
		                                        and E.ValueType = 'MaterialWeight'
		                                        and D.OrganizationID = '{2}'
		                                        group by E.OrganizationID, E.VariableId) F
                                        where M.OrganizationID = '{2}' 
                                        and N.LevelCode like M.LevelCode + '%'
                                        and A.OrganizationID in (N.OrganizationID)
                                        and A.Enable = 1
                                        and A.State = 0
                                        and A.KeyID = B.KeyID
                                        and B.Visible = 1
                                        and A.OrganizationID = F.OrganizationID
                                        and B.VariableId = F.VariableId
                                        order by N.Type, N.Name, B.Name";
//#if DEBUG
//            return dataFactory.Query(string.Format(queryString, levelCodesParameter.ToString(), "2015-02-09"));
//#else
            queryString = string.Format(queryString, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), organizationId);
            return dataFactory.Query(queryString);
//#endif
        }
    }
}