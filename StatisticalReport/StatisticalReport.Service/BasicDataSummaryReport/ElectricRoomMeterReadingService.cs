using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public static class ElectricRoomMeterReadingService
    {
        private static string meterDNName;
        /// <summary>
        /// 获得所有的电气室
        /// </summary>
        /// <param name="factoryOrganizationId">分厂级别的组织机构ID</param>
        /// <returns></returns>
        public static DataTable GetElectricRoomByOrganizationId(string factoryOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string sqlStringER = @"SELECT B.MeterDatabase
                                    FROM system_Organization AS A,system_Database AS B
                                    WHERE A.DatabaseID=B.DatabaseID AND
                                    A.OrganizationID=@OrganizationID";
            SqlParameter parameterER = new SqlParameter("OrganizationID", factoryOrganizationId);
            DataTable factoryInfoTable = dataFactory.Query(sqlStringER, parameterER);
            if (factoryInfoTable.Rows.Count != 1)
                throw new Exception("查找分厂数据库失败！");
            meterDNName = factoryInfoTable.Rows[0]["MeterDatabase"].ToString().Trim();
            string sqlString = @"SELECT DISTINCT A.ElectricRoom AS ElectricRoom
                                    FROM [{0}].[dbo].[AmmeterContrast] AS A
                                    WHERE A.EnabledFlag='true'";
            DataTable electricRoomTable = dataFactory.Query(string.Format(sqlString, meterDNName));
            return electricRoomTable;
        }
        /// <summary>
        /// 获得电表值
        /// </summary>
        /// <param name="electricRoom">电气室名</param>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">结束日期</param>
        /// <returns></returns>
        public static DataTable GetAmmeterValue(string electricRoom,string startTime,string endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string sqlStringAmmeter = @"SELECT AmmeterName,AmmeterNumber,ElectricRoom 
                                            FROM [{0}].[dbo].[AmmeterContrast] 
                                            WHERE ElectricRoom=@electricRoom";
            SqlParameter parameter = new SqlParameter("electricRoom", electricRoom);
            //电表信息
            DataTable ammeterInfoTable = dataFactory.Query(string.Format(sqlStringAmmeter, meterDNName), parameter);
            //根据差值计算电表电量
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT TOP (1) "); 
            foreach (DataRow dr in ammeterInfoTable.Rows)
            {
                stringBuilder.Append(dr["AmmeterNumber"].ToString().Trim());
                stringBuilder.Append("energy AS '");
                stringBuilder.Append(dr["AmmeterName"].ToString().Trim() + "(" + dr["AmmeterNumber"].ToString().Trim()+")");
                stringBuilder.Append("',");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("FROM [{0}].[dbo].[HistoryAmmeter] WHERE vDate{2}@date order by vDate {1}");
            SqlParameter parameterStartDate=new SqlParameter("date",startTime);
            SqlParameter parameterEndDate=new SqlParameter("date",endTime);
            DataTable startTable = dataFactory.Query(string.Format(stringBuilder.ToString(), meterDNName, "ASC",">="), parameterStartDate);
            DataTable endTable = dataFactory.Query(string.Format(stringBuilder.ToString(), meterDNName, "DESC","<="), parameterEndDate);
            startTable.Merge(endTable);
            if (startTable.Rows.Count != 2)
                throw new Exception("数据不完整！");

            DataTable resultTable = startTable.Clone();
            DataRow row = resultTable.NewRow();
            foreach (DataColumn dc in startTable.Columns)//遍历所有的列 
            {
                string columnName = dc.ColumnName;
                row[columnName] =ReportHelper.MyToDecimal(startTable.Rows[1][columnName]) - ReportHelper.MyToDecimal(startTable.Rows[0][columnName]);
            }
            //table.Clear();
            resultTable.Rows.Add(row);

            //根据增量值计算电量电量
            stringBuilder.Clear();
            stringBuilder.Append("select ");
            foreach (DataRow dr in ammeterInfoTable.Rows)
            {
                stringBuilder.Append("sum(");
                stringBuilder.Append(dr["AmmeterNumber"].ToString().Trim());
                stringBuilder.Append("energy) AS '");
                stringBuilder.Append(dr["AmmeterName"].ToString().Trim() + "(" + dr["AmmeterNumber"].ToString().Trim() + ")");
                stringBuilder.Append("',");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("FROM [{0}].[dbo].[HistoryAmmeterIncrement] WHERE vDate>=@startTime and vDate<=@endTime");
            SqlParameter[] parameters = { new SqlParameter("startTime", startTime), new SqlParameter("endTime", endTime) };
            DataTable incrementTable = dataFactory.Query(string.Format(stringBuilder.ToString(), meterDNName), parameters);
            if (incrementTable.Rows.Count == 1)
            {
                DataRow iRow = resultTable.NewRow();
                iRow.ItemArray= incrementTable.Rows[0].ItemArray;
                resultTable.Rows.Add(iRow);
            }
            return HorizontalToVertical(resultTable);
        }
        /// <summary>
        /// 横表转纵表
        /// </summary>
        /// <param name="sourceTable">原始表</param>
        /// <returns></returns>
        private static DataTable HorizontalToVertical(DataTable sourceTable)
        {
            DataTable result = new DataTable();
            DataColumn AmmertNameColumn = new DataColumn("AmmeterName", typeof(string));
            DataColumn AmmertValueColumn = new DataColumn("Value", typeof(decimal));
            DataColumn IncrementVallueColumn = new DataColumn("IncrementValue",typeof(decimal));
            //用表差值
            DataColumn DvalueColumn = new DataColumn("DvalueColumn",typeof(decimal));
            result.Columns.Add(AmmertNameColumn);
            result.Columns.Add(AmmertValueColumn);
            result.Columns.Add(IncrementVallueColumn);
            result.Columns.Add(DvalueColumn);
            if(sourceTable.Rows.Count!=2)
                throw new Exception("sourceTable数据不完整！");
            foreach (DataColumn dc in sourceTable.Columns)
            {
                DataRow row = result.NewRow();
                row["AmmeterName"] = dc.ColumnName.Trim();
                row["Value"] =Convert.ToDecimal(sourceTable.Rows[0][dc.ColumnName])<0?0: sourceTable.Rows[0][dc.ColumnName];
                row["IncrementValue"]=sourceTable.Rows[1][dc.ColumnName];
                row["DvalueColumn"] = Convert.ToDecimal(row["Value"]) -Convert.ToDecimal(row["IncrementValue"]);
                result.Rows.Add(row);
            }
            //int count = result.Rows.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    if (Convert.ToDecimal(result.Rows[i]["Value"]) < 0)
            //        result.Rows[i]["Value"] = 0;
            //}
            return result;
        }
    }
}
