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
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT TOP (1) "); 
            foreach (DataRow dr in ammeterInfoTable.Rows)
            {
                stringBuilder.Append(dr["AmmeterNumber"].ToString().Trim());
                stringBuilder.Append("energy AS '");  
                stringBuilder.Append(dr["AmmeterName"].ToString().Trim());
                stringBuilder.Append("',");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("FROM [{0}].[dbo].[HistoryAmmeter] WHERE CONVERT(varchar(10),vDate,20){2}@date order by vDate {1}");
            SqlParameter parameterStartDate=new SqlParameter("date",startTime);
            SqlParameter parameterEndDate=new SqlParameter("date",endTime);
            DataTable startTable = dataFactory.Query(string.Format(stringBuilder.ToString(), meterDNName, "ASC",">="), parameterStartDate);
            DataTable endTable = dataFactory.Query(string.Format(stringBuilder.ToString(), meterDNName, "DESC","<="), parameterEndDate);
            startTable.Merge(endTable);
            if (startTable.Rows.Count != 2)
                throw new Exception("数据不完整！");
            DataRow row = startTable.NewRow();
            foreach (DataColumn dc in startTable.Columns)//遍历所有的列 
            {
                string columnName = dc.ColumnName;
                row[columnName] =ReportHelper.MyToDecimal(startTable.Rows[1][columnName]) - ReportHelper.MyToDecimal(startTable.Rows[0][columnName]);
            }
            //table.Clear();
            startTable.Rows.Add(row);

            return HorizontalToVertical(startTable);
        }
        /// <summary>
        /// 横表转纵表
        /// </summary>
        /// <param name="sourceTable">原始表</param>
        /// <returns></returns>
        private static DataTable HorizontalToVertical(DataTable sourceTable)
        {
            DataTable result = new DataTable();
            DataColumn AmmertNamecolumn = new DataColumn("AmmeterName", typeof(string));
            DataColumn AmmertValueColumn = new DataColumn("Value", typeof(decimal));
            result.Columns.Add(AmmertNamecolumn);
            result.Columns.Add(AmmertValueColumn);
            if(sourceTable.Rows.Count!=3)
                throw new Exception("sourceTable数据不完整！");
            foreach (DataColumn dc in sourceTable.Columns)
            {
                DataRow row = result.NewRow();
                row["AmmeterName"] = dc.ColumnName.Trim();
                row["Value"] = sourceTable.Rows[2][dc.ColumnName];
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
