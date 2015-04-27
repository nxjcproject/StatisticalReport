using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class DailyBasicElectricityUsageService
    {
        /// <summary>
        /// 查询所有产线的用电量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetDailyBasicElectricityUsageByOrganiztionIds(string organizationId, DateTime startDate,DateTime endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string queryString = @"SELECT  SO.Name,SO.LevelCode,FIN.VariableName,FIN.FormulaLevelCode,SUM(FIN.FirstB) AS FirstB,SUM(FIN.SecondB) AS SecondB,SUM(FIN.ThirdB) AS ThirdB,SUM(FIN.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                    FROM system_Organization AS SO LEFT JOIN 
                                        (SELECT TB.TimeStamp,TB.StaticsCycle,BE.*,RST.VariableId AS FirstVariable,RST.FormulaLevelCode  FROM 
	                                                 (SELECT TF.OrganizationID AS TFOrgID ,FFD.VariableId,FFD.LevelCode AS FormulaLevelCode
	                                                 FROM tz_Formula AS TF,formula_FormulaDetail AS FFD 
	                                                 WHERE TF.KeyID=FFD.KeyID AND 
	                                                 -- FFD.LevelType='ProductionLine' AND
	                                                 TF.Type='2' AND TF.ENABLE='true') AS RST,
	                                    tz_Balance AS TB,balance_Energy AS BE
	                                    WHERE TB.BalanceId=BE.KeyId AND RST.TFOrgID=BE.OrganizationID AND
                                        RST.VariableId+'_ElectricityQuantity'=BE.VariableId AND
	                                    TB.TimeStamp>='{0}' AND TB.TimeStamp<='{1}' AND
										TB.StaticsCycle='day'
                                         ) AS FIN
                                    ON SO.OrganizationID=FIN.OrganizationID 
                                    INNER JOIN 
										 (
										 SELECT A.OrganizationID 
                                            FROM system_Organization AS A 
                                            WHERE A.LevelCode like (
                                                                    SELECT T.LevelCode FROM system_Organization AS T
						                                            WHERE T.OrganizationID='{2}'
						                                            )+'%'
										 ) AS O
									ON
									O.OrganizationID=SO.OrganizationID
                                    WHERE ISNULL(SO.Type,'')<>'余热发电' 
									GROUP BY VariableName,
									SO.Name,SO.LevelCode,
									FIN.VariableName,
                                    FIN.FormulaLevelCode
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
//            DataTable resultTable = dataFactory.Query(string.Format(queryString, organizationId, "2015-02-09")); 
//#else
            DataTable resultTable = dataFactory.Query(string.Format(queryString, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), organizationId));
//#endif
            DataColumn stateColumn = new DataColumn("state", typeof(string));
            resultTable.Columns.Add(stateColumn);

            foreach (DataRow dr in resultTable.Rows)
            {
                if (dr["VariableName"] is DBNull || dr["VariableName"].ToString().Trim() == "")
                {
                    dr["VariableName"] = dr["Name"].ToString().Trim();
                }
                if (dr["FormulaLevelCode"].ToString().Length > 3)
                {
                    dr["LevelCode"] = dr["LevelCode"] + dr["FormulaLevelCode"].ToString().Substring(3);
                }
                if (dr["LevelCode"].ToString().Trim().Length == 7)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return resultTable;
        }
    }
}
