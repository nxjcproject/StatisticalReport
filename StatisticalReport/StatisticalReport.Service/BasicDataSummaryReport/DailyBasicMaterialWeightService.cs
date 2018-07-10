//using EnergyConsumption;
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
            string queryString1 = @"Select C.Name AS CompanyName, M.Name as FactoryName, F.OrganizationID as FactoryOrgID, (case when N.Name like '%号熟料' then substring(N.Name,1,2) else N.Name end) + (case when B.Name='水泥产量' then '产量' else B.Name end) AS VariableName, F.FirstB AS FirstB, F.SecondB AS SecondB, F.ThirdB AS ThirdB,F.PeakB AS PeakB,F.ValleyB AS ValleyB,F.FlatB AS FlatB,F.A班 as A班,F.B班 as B班,F.C班 as C班,F.D班 as D班,F.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                        from tz_Material A, material_MaterialDetail B, system_Organization M
                                        left join system_Organization C on substring(M.LevelCode,1,Len(M.LevelCode) - 2) = C.LevelCode, system_Organization N, 
                                            (Select E.OrganizationID, E.VariableId, SUM(E.FirstB) AS FirstB,SUM(E.SecondB) AS SecondB,SUM(E.ThirdB) AS ThirdB,SUM(E.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,SUM(E.PeakB) AS PeakB,SUM(E.ValleyB) AS ValleyB,SUM(E.FlatB) AS FlatB,
                                    SUM(CASE WHEN [D].[FirstWorkingTeam] = 'A班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'A班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'A班' THEN [E].[ThirdB] ELSE 0 END) AS A班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'B班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'B班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'B班' THEN [E].[ThirdB] ELSE 0 END) AS B班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'C班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'C班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'C班' THEN [E].[ThirdB] ELSE 0 END) AS C班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'D班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'D班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'D班' THEN [E].[ThirdB] ELSE 0 END) AS D班
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
                                        order by  VariableName
                                        ";
            // order by N.Type, N.Name, B.Name

            string queryString2 = @"Select distinct C.Name AS CompanyName, M.Name as FactoryName, F.OrganizationID as FactoryOrgID,N.Name+F.VariableName AS VariableName, F.FirstB AS FirstB, F.SecondB AS SecondB, F.ThirdB AS ThirdB,F.PeakB AS PeakB,F.ValleyB AS ValleyB,F.FlatB AS FlatB,F.A班 as A班,F.B班 as B班,F.C班 as C班,F.D班 as D班,F.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                        from tz_Material A, material_MaterialDetail B, system_Organization M
                                        left join system_Organization C on substring(M.LevelCode,1,Len(M.LevelCode) - 2) = C.LevelCode, system_Organization N, 
                                            (Select E.OrganizationID, E.VariableId,E.VariableName, SUM(E.FirstB) AS FirstB,SUM(E.SecondB) AS SecondB,SUM(E.ThirdB) AS ThirdB,SUM(E.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,SUM(E.PeakB) AS PeakB,SUM(E.ValleyB) AS ValleyB,SUM(E.FlatB) AS FlatB,
                                    SUM(CASE WHEN [D].[FirstWorkingTeam] = 'A班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'A班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'A班' THEN [E].[ThirdB] ELSE 0 END) AS A班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'B班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'B班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'B班' THEN [E].[ThirdB] ELSE 0 END) AS B班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'C班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'C班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'C班' THEN [E].[ThirdB] ELSE 0 END) AS C班,
		                            SUM(CASE WHEN [D].[FirstWorkingTeam] = 'D班' THEN [E].[FirstB] WHEN [D].[SecondWorkingTeam] = 'D班' THEN [E].[SecondB] WHEN [D].[ThirdWorkingTeam] = 'D班' THEN [E].[ThirdB] ELSE 0 END) AS D班
	                                            from tz_Balance D, balance_Energy E 
		                                        where D.TimeStamp >= '{0}'
		                                        and D.TimeStamp <= '{1}'
		                                        and D.StaticsCycle = 'day'
		                                        and D.BalanceId = E.KeyId
		                                        and E.ValueType = 'ChangeLogCement'  
		                                        and D.OrganizationID = '{2}'
		                                        group by E.OrganizationID, E.VariableId, E.VariableName) F
                                        where M.OrganizationID ='{2}' 
                                        and N.LevelCode like M.LevelCode + '%'
                                        and A.OrganizationID in (N.OrganizationID)
                                        and A.Enable = 1
                                        and A.State = 0
                                        and A.KeyID = B.KeyID
                                        and B.Visible = 1
                                        and A.OrganizationID = F.OrganizationID
                                        
                                        order by FactoryOrgID, VariableName    ";
                                                              
            //#if DEBUG  order by N.Type, N.Name, B.Name    order by N.Type, N.Name, B.Name
//            return dataFactory.Query(string.Format(queryString, levelCodesParameter.ToString(), "2015-02-09"));
//#else
            queryString1 = string.Format(queryString1, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), organizationId);
            queryString2 = string.Format(queryString2, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), organizationId);
 
            
            DataTable result1 = dataFactory.Query(queryString1);
            DataTable result2 = dataFactory.Query(queryString2);

            DataTable newDataTable = result1.Clone();
            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < result1.Rows.Count; i++)
            {
                result1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }

            for (int i = 0; i < result2.Rows.Count; i++)
            {
                result2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            newDataTable.DefaultView.Sort = "VariableName asc";
            return newDataTable.DefaultView.ToTable();
//#endif
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
             DataTable result2=dataFactory.Query(sql, parameters);
             return result2;
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

