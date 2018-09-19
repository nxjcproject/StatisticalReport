using EnergyConsumption;
using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class EnergyConsumptionDetailReportService
    {
        private static string _connString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connString);

        public static DataTable GetEnergyConsumptionDetailReportDataTable(string mOrganizationId, string mStartDate, string mEndDate, string mChecked)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.OrganizationID,B.Name,B.LevelCode,B.VariableID,B.LevelType,C.ValueFormula,'' as FormulaValue,'' as OutPut , '' as FormulaConsumption
                                 from (SELECT LevelCode,Type FROM system_Organization WHERE OrganizationID=@organizationId) M inner join system_Organization N
 	                                on N.LevelCode Like M.LevelCode+'%' inner join tz_Formula A 
 	                                on A.OrganizationID=N.OrganizationID inner join formula_FormulaDetail B
 	                                on A.KeyID = (SELECT TOP 1 KeyID FROM tz_Formula WHERE OrganizationID= @organizationId AND ENABLE = 1 AND State = 0 AND CreatedDate <= @endTime
						                          ORDER BY CreatedDate DESC) 
                                       and A.KeyID=B.KeyID and A.ENABLE=1 and A.State=0 {0} left join balance_Energy_Template C 
 	                                on B.VariableId+'_ElectricityConsumption'=C.VariableId and C.ProductionLineType=M.Type 
                                 order by OrganizationID,LevelCode";
            SqlParameter[] parameter ={  new SqlParameter("@organizationId", mOrganizationId),                                        
                                         new SqlParameter("@endTime", mEndDate)};
            string condition = null;
            //判断设备是否选中
            if (mChecked == "false")
            {
                condition = " and B.LevelType != 'MainMachine' ";
            }

            DataTable frameTable = _dataFactory.Query(string.Format(mySql, condition), parameter);
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

            DataTable mFrameTable = GetFrameDataTable(mOrganizationId, mStartDate, mEndDate, frameTable);

            DataColumn stateColumn = new DataColumn("state", typeof(string));
            mFrameTable.Columns.Add(stateColumn);
            foreach (DataRow dr in mFrameTable.Rows)
            {
                bool haveChidren = HaveChildren(dr["LevelCode"].ToString().Trim(), mFrameTable);
                if (dr["LevelCode"].ToString().Trim().Length == 7 && haveChidren)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return mFrameTable;
        }

        private static DataTable GetFrameDataTable(string mOrganizationId, string mStartDate, string mEndDate, DataTable mFrameTable)
        {
            DataTable table = new DataTable();

            //判断开始时间和结束时间是不是整天，整天的取[balance_Energy]数据，其余的另行计算
            DateTime mStartTime = DateTime.Parse(mStartDate);
            DateTime mEndTime = DateTime.Parse(mEndDate);


            //判断开始、结束时间为同一天和只相差一天的，只能从HistoryFormulaValue单独计算
            if (mStartTime.Date == mEndTime.Date || mStartTime.AddDays(1).Date == mEndTime.Date)
            {
                DataTable HistoryFormulaValueTable = GetHistoryFormulaValueTable(mOrganizationId, mStartTime.ToString(), mEndTime.ToString(), mStartTime.ToString(), mEndTime.ToString());

                for (int i = 0; i < mFrameTable.Rows.Count; i++)
                {
                    string frameVariableID = mFrameTable.Rows[i]["VariableID"].ToString().Trim();
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        string sourceVariableID = HistoryFormulaValueTable.Rows[j]["VariableID"].ToString().Trim();
                        if (sourceVariableID == frameVariableID)
                        {
                            mFrameTable.Rows[i]["FormulaValue"] = Convert.ToDouble(HistoryFormulaValueTable.Rows[j]["FormulaValue"]).ToString("0.0");
                            mFrameTable.Rows[i]["OutPut"] = Convert.ToDouble(HistoryFormulaValueTable.Rows[j]["DenominatorValue"]).ToString("0.0");
                            if (Convert.ToDouble(mFrameTable.Rows[i]["OutPut"]) >= 1.0)
                            {
                                mFrameTable.Rows[i]["FormulaConsumption"] = (Convert.ToDouble(mFrameTable.Rows[i]["FormulaValue"]) / Convert.ToDouble(mFrameTable.Rows[i]["OutPut"])).ToString("0.00");
                            }
                            else
                            {
                                mFrameTable.Rows[i]["FormulaConsumption"] = "0.00";
                            }
                            //找到对应的变量即跳出此次内循环
                            break;
                        }
                    }
                }
                table = mFrameTable;
            }
            else
            {
                DateTime mStartTime1 = mStartTime;
                DateTime mStartTime2 = mStartTime.AddDays(1).Date;
                DateTime mEndTime1 = mEndTime.Date;
                DateTime mEndTime2 = mEndTime;
                DataTable mBalanceTable = GetBalanceTable(mOrganizationId, mStartTime2.ToString("yyyy-MM-dd"), mEndTime1.ToString("yyyy-MM-dd"));
                DataTable mHistoryFormulaValueTable = GetHistoryFormulaValueTable(mOrganizationId, mStartTime1.ToString(), mStartTime2.ToString(), mEndTime1.ToString(), mEndTime2.ToString());

                //先将balance数据的电量、产量插入mFrameTable
                for (int i = 0; i < mFrameTable.Rows.Count; i++)
                {
                    string mValueFormula = mFrameTable.Rows[i]["ValueFormula"].ToString().Trim();
                    string[] ValueFormula = mValueFormula.Split('/');
                    string ElectricityQuantityVariableID = ValueFormula[0].Substring(ValueFormula[0].IndexOf('[') + 1, ValueFormula[0].IndexOf(']') - ValueFormula[0].IndexOf('[') - 1);
                    string OutputVariableID = ValueFormula[1].Substring(ValueFormula[1].IndexOf('[') + 1, ValueFormula[1].IndexOf(']') - ValueFormula[0].IndexOf('[') - 1);
                    for (int j = 0; j < mBalanceTable.Rows.Count; j++)
                    {
                        string VariableID = mBalanceTable.Rows[j]["VariableID"].ToString().Trim();
                        if (VariableID == ElectricityQuantityVariableID)
                        {
                            mFrameTable.Rows[i]["FormulaValue"] = mBalanceTable.Rows[j]["TotalPeakValleyFlatB"];
                        }
                        if (VariableID == OutputVariableID)
                        {
                            mFrameTable.Rows[i]["OutPut"] = mBalanceTable.Rows[j]["TotalPeakValleyFlatB"];
                        }
                    }

                    string mFrameVariableID = mFrameTable.Rows[i]["VariableID"].ToString().Trim();
                    for (int k = 0; k < mHistoryFormulaValueTable.Rows.Count; k++)
                    {
                        string mHistoryVariableID = mHistoryFormulaValueTable.Rows[k]["VariableID"].ToString().Trim();
                        if (mHistoryVariableID == mFrameVariableID)
                        {
                            mFrameTable.Rows[i]["FormulaValue"] = double.Parse(mFrameTable.Rows[i]["FormulaValue"].ToString()) + double.Parse(mHistoryFormulaValueTable.Rows[k]["FormulaValue"].ToString());
                            mFrameTable.Rows[i]["OutPut"] = double.Parse(mFrameTable.Rows[i]["OutPut"].ToString()) + double.Parse(mHistoryFormulaValueTable.Rows[k]["DenominatorValue"].ToString());
                        }
                    }

                    if (Convert.ToDouble(mFrameTable.Rows[i]["OutPut"]) >= 1.0)
                    {
                        mFrameTable.Rows[i]["FormulaConsumption"] = (Convert.ToDouble(mFrameTable.Rows[i]["FormulaValue"]) / Convert.ToDouble(mFrameTable.Rows[i]["OutPut"])).ToString("0.00");
                    }
                    else
                    {
                        mFrameTable.Rows[i]["FormulaConsumption"] = "0.00";
                    }
                }               

                table = mFrameTable;
            }

            return table;
        }

        /// <summary>
        /// 整天数据从balance_Energy中取
        /// </summary>
        /// <param name="mOrganizationId"></param>
        /// <param name="mStartDate"></param>
        /// <param name="mEndDate"></param>
        /// <returns></returns>
        private static DataTable GetBalanceTable(string mOrganizationId, string mStartDate, string mEndDate)
        {
            string mSql = @"select B.OrganizationID,B.VariableId,SUM(B.TotalPeakValleyFlatB) as TotalPeakValleyFlatB
                                from tz_Balance A,balance_Energy B
                                where A.BalanceId = B.KeyId
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp >= @startTime 
                                and A.TimeStamp <= @endTime
                                and B.OrganizationID = @organizationId                                   
                                group by B.OrganizationID,B.VariableId";
            SqlParameter[] parameters = { new SqlParameter("@organizationId", mOrganizationId), 
                                          new SqlParameter("@startTime", mStartDate), 
                                          new SqlParameter("@endTime", mEndDate) 
                                        };
            DataTable table = _dataFactory.Query(mSql, parameters);
            return table;
        }

        /// <summary>
        /// 不是整天的数据，在HistoryMainMachineFormulaValue,HistoryFormulaValue中单独计算
        /// 然后与balance_Energy的数据整合
        /// </summary>
        /// <param name="mOrganizationId"></param>
        /// <param name="mStartTime1"></param>
        /// <param name="mStartTime2"></param>
        /// <param name="mEndTime1"></param>
        /// <param name="mEndTime2"></param>
        /// <returns></returns>
        private static DataTable GetHistoryFormulaValueTable(string mOrganizationId, string mStartTime1, string mStartTime2, string mEndTime1, string mEndTime2)
        {
            string sql_MeterDatabase = @"SELECT A.DatabaseID
                                               ,A.ManagementDatabase
                                               ,A.DCSProcessDatabase
                                               ,A.MeterDatabase
                                           FROM system_Database A,
                                                system_Organization B
                                          WHERE B.OrganizationID = '{0}' 
                                           AND A.DatabaseID = B.DatabaseID";
            DataTable MeterDatabaseTable = _dataFactory.Query(string.Format(sql_MeterDatabase, mOrganizationId));
            string mMeterDatabase = MeterDatabaseTable.Rows[0]["MeterDatabase"].ToString().Trim();

            string sql_HistoryFormulaValue = @"SELECT * FROM
                                               ((SELECT 
                                                   OrganizationID
                                                  ,VariableID
                                                  ,LevelCode
	                                              ,SUM(FormulaValue) AS FormulaValue
                                                  ,SUM(DenominatorValue) AS DenominatorValue
                                              FROM [{0}].[dbo].[HistoryMainMachineFormulaValue]
                                             WHERE (vDate >= @mStartTime1 AND vDate <= @mStartTime2)
                                                OR (vDate >= @mEndTime1 AND vDate <= @mEndTime2)
                                             GROUP BY OrganizationID,VariableID,LevelCode)
                                             UNION ALL
                                             (SELECT 
                                                   OrganizationID
                                                  ,VariableID
                                                  ,LevelCode
                                                 ,SUM(FormulaValue) AS FormulaValue
                                                  ,SUM(DenominatorValue) AS DenominatorValue
                                              FROM [{0}].[dbo].[HistoryFormulaValue]
                                             WHERE (vDate >= @mStartTime1 AND vDate <= @mStartTime2)
                                                OR (vDate >= @mEndTime1 AND vDate <= @mEndTime2)
                                             GROUP BY OrganizationID,VariableID,LevelCode)) A
                                             WHERE A.OrganizationID = '{1}'
                                             ORDER BY A.LevelCode";
            SqlParameter[] paras ={ new SqlParameter("@mStartTime1",mStartTime1),
                                    new SqlParameter("@mStartTime2",mStartTime2),
                                    new SqlParameter("@mEndTime1",mEndTime1),
                                    new SqlParameter("@mEndTime2",mEndTime2)};
            DataTable table = _dataFactory.Query(string.Format(sql_HistoryFormulaValue, mMeterDatabase, mOrganizationId), paras);
            return table;
        }

        /// <summary>
        /// 处理公式
        /// </summary>
        /// <param name="preFormula"></param>
        /// <param name="variableId"></param>
        /// <returns></returns>
        private static string DealWithFormula(string preFormula, string variableId)
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
    }
}
