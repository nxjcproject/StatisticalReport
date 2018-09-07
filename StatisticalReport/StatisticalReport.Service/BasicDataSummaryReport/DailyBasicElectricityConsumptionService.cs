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
    public class DailyBasicElectricityConsumptionService
    {

        public static DataTable GetElectricityConsumptionData(string organizationId, string startTime, string endTime, string consumptionType)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.OrganizationID,B.Name,B.LevelCode,B.VariableID,B.LevelType,C.ValueFormula 
                                 from (SELECT LevelCode,Type FROM system_Organization WHERE OrganizationID=@organizationId) M inner join system_Organization N
 	                                on N.LevelCode Like M.LevelCode+'%' inner join tz_Formula A 
 	                                on A.OrganizationID=N.OrganizationID inner join formula_FormulaDetail B
 	                                on A.KeyID = (SELECT TOP 1 KeyID FROM tz_Formula WHERE OrganizationID= @organizationId AND ENABLE = 1 AND State = 0 AND CreatedDate <= @endTime
						                          ORDER BY CreatedDate DESC) 
                                       and A.KeyID=B.KeyID and A.Type=2 left join balance_Energy_Template C 
 	                                on B.VariableId+'_'+@consumptionType=C.VariableId and C.ProductionLineType=M.Type 
                                 order by OrganizationID,LevelCode";
//            string mySql = @"SELECT A.OrganizationID
//                                    ,B.Name
//	                                ,B.LevelCode
//	                                ,B.VariableID
//	                                ,B.LevelType
//	                                ,C.ValueFormula 
//	                                FROM 
//		                                system_Organization N,
//		                                tz_Formula A,
//		                                formula_FormulaDetail B 
//                                left join balance_Energy_Template C 
//		                            on (B.VariableId + '_' + 'ElectricityConsumption' = C.VariableId and C.ProductionLineType = N.Type)
//                                    where N.OrganizationID = @organizationId
//	                                and A.OrganizationID = N.OrganizationID
//	                                and A.KeyID = (SELECT TOP 1 KeyID
//						                             FROM tz_Formula
//						                            WHERE OrganizationID= @organizationId AND ENABLE = 1 AND State = 0 AND CreatedDate <= @endTime
//						                            ORDER BY CreatedDate DESC)
//	                                and A.KeyID = B.KeyID 
//	                                and A.Type = 2
//	                                and A.ENABLE = 1
//	                                and A.State = 0
//	                                --and C.ProductionLineType = N.Type
//                                order by B.LevelCode";
            SqlParameter[] parameter ={  new SqlParameter("@organizationId", organizationId),
                                         new SqlParameter("@consumptionType", consumptionType),
                                         new SqlParameter("@endTime", endTime)};
            DataTable frameTable = dataFactory.Query(mySql, parameter);
            string preFormula = "";
            foreach (DataRow dr in frameTable.Rows)
            {
                if (dr["ValueFormula"] is DBNull)
                {
                    if (dr["VariableId"] is DBNull || dr["VariableId"].ToString().Trim() == "")
                    {
                        continue;
                    }
                    preFormula = DealWithFormula(preFormula, dr["VariableId"].ToString().Trim());
                    dr["ValueFormula"] = preFormula;
                }
                else
                {
                    preFormula = dr["ValueFormula"].ToString().Trim();
                }
            }
            string dataSql = @"select B.OrganizationID,B.VariableId,SUM(B.FirstB) as FirstB,SUM(B.SecondB) as SecondB,SUM(B.ThirdB) as ThirdB,SUM(B.TotalPeakValleyFlatB) as TotalPeakValleyFlatB,SUM(B.PeakB) AS PeakB,SUM(B.ValleyB) AS ValleyB,SUM(B.FlatB) AS FlatB,
                                    SUM(CASE WHEN [A].[FirstWorkingTeam] = 'A班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'A班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'A班' THEN [B].[ThirdB] ELSE 0 END) AS teamA,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'B班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'B班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'B班' THEN [B].[ThirdB] ELSE 0 END) AS teamB,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'C班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'C班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'C班' THEN [B].[ThirdB] ELSE 0 END) AS teamC,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'D班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'D班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'D班' THEN [B].[ThirdB] ELSE 0 END) AS teamD
                                from tz_Balance A,balance_Energy B
                                where A.BalanceId=B.KeyId
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp>=@startTime and A.TimeStamp<=@endTime
                                and B.OrganizationID=@organizationId                                   
                                group by B.OrganizationID,B.VariableId";
            SqlParameter[] parameters = { new SqlParameter("@organizationId", organizationId), 
                                          new SqlParameter("@startTime", startTime), 
                                          new SqlParameter("@endTime", endTime) 
                                        };
            DataTable sourceData = dataFactory.Query(dataSql, parameters);
            string[] calColumns = new string[] { "FirstB", "SecondB", "ThirdB", "TotalPeakValleyFlatB", "PeakB", "ValleyB", "FlatB", "teamA", "teamB", "teamC", "teamD" };

            DataTable result = EnergyConsumption.EnergyConsumptionCalculate.CalculateByOrganizationId(sourceData, frameTable, "ValueFormula", calColumns);
            DataColumn stateColumn = new DataColumn("state", typeof(string));
            result.Columns.Add(stateColumn);
                    
            foreach (DataRow dr in result.Rows)
            {
                bool haveChidren = HaveChildren(dr["LevelCode"].ToString().Trim(), result);
                if (dr["LevelCode"].ToString().Trim().Length == 7 && haveChidren)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return result;
        }
               
        /// <summary>
        /// 处理公式
        /// </summary>
        /// <param name="preFormula"></param>
        /// <param name="variableId"></param>
        /// <returns></returns>
        private static string DealWithFormula(string preFormula,string variableId)
        {
            if (preFormula.Contains('_'))
            {
                int num = preFormula.IndexOf('_');
                string subStr = preFormula.Substring(1, num - 1);
                return preFormula.Replace(subStr, variableId);
            }
            else
            {
                return preFormula;
            }     
        }

        /// <summary>
        /// 判断是否有孩子结点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="resultTable"></param>
        /// <returns></returns>
        private static bool HaveChildren(string parent, DataTable resultTable)
        {
            int myLength = parent.Trim().Length;
            DataRow[] rows = resultTable.Select("LevelCode Like '" + parent + "%' and Len(LevelCode)>" + myLength);
            if (rows.Count() > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static DataTable GetShiftsSchedulingLogMonthly(string organizationId, string startDate, string endDate)
        {
            string[] arr = organizationId.Split('_');
            string neworganization = arr[0] + '_' + arr[1] + '_' + arr[2] + '_' + arr[3];


            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string sql = @" SELECT [TimeStamp],[FirstWorkingTeam],[SecondWorkingTeam],[ThirdWorkingTeam]
                              FROM [tz_Balance]
                              WHERE TimeStamp>=@startDate AND TimeStamp<=@endDate
		                            and StaticsCycle = 'day' AND
		                            [OrganizationID]= @neworganization
                              ORDER BY [TimeStamp]";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("neworganization", neworganization),
                new SqlParameter("startDate", startDate),
                new SqlParameter("endDate",endDate)
            };

            DataTable result2 = dataFactory.Query(sql, parameters);
            return result2;
        }



    }
}
