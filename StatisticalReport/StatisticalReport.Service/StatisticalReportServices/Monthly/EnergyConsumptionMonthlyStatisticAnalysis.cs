using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly
{
    public class EnergyConsumptionMonthlyStatisticAnalysis
    {
        private static TZHelper tzHelper;
        private static string connectionstring;
        static EnergyConsumptionMonthlyStatisticAnalysis()
        {
            connectionstring = ConnectionStringFactory.NXJCConnectionString;// ConfigurationManager.ConnectionStrings["NXJC_TEST"].ToString();//连接字符串
            tzHelper = new TZHelper(connectionstring);
        }
        /// <summary>
        /// 能源消耗月统计分析报表
        /// </summary>
        /// <param name="_temp"></param>
        /// <param name="_organizeID">到分厂层次</param>
        /// <param name="_date">到月份，例：2014-08</param>
        /// <returns></returns>
        public static DataTable TableQuery(string _organizeID, string _date)//_temp表需要从张迪那获得
        {
            // DataTable temp=new DataTable();//从张迪那获得
            int productLevelLength = 0;
            string[] strArray = _organizeID.Split('_');
            switch (strArray.Count())
            {
                case 2://总公司级别
                    productLevelLength = 11;
                    break;
                case 3://分公司级别
                    productLevelLength = 9;
                    break;
                case 4://分厂级别
                    productLevelLength = 7;
                    break;
            }
            DataTable _temp = tzHelper.GetCompanyLevelTable(_organizeID);
            // DataColumn[] columns = new DataColumn[];
            //List<DataColumn> columns=new List<DataColumn>();
            //columns.Add(new DataColumn(
            string[] columnName = { "Electricity_RawBatch", "Electricity_Clinker", "Electricity_Cement", "Consumption_CoalDust",
                                      "Output_RawBatch", "Output_Clinker", "Output_Cement", "Output_Cogeneration", "ElectricityConsumption_RawBatch", 
                                      "ElectricityConsumption_Clinker", "ElectricityConsumption_Cement", "ComprehensiveElectricityConsumption", 
                                      "ComprehensiveCoalConsumption", "ComprehensiveElectricityOutput" };//需要添加的字段
            foreach (string cl_name in columnName)
            {
                _temp.Columns.Add(cl_name, Type.GetType("System.String"));//将字段添加到_temp表中
            }
            foreach (DataRow dr in _temp.Rows)
            {
                string organizationID = dr["OrganizationID"].ToString();//确认ID的数据类型
                string type = dr["Type"].ToString();//确认Type的数据类型
                //处理孰料线
                if ("熟料" == type)
                {
                    DataTable table_cl = tzHelper.GetReportData("tz_Report", organizationID, _date, "table_ClinkerMonthlyOutput", "vDate='合计'");
                    if (table_cl.Rows.Count > 0)//只有在有数据的时候才会进行以下处理
                    {
                        DataRow row_cl = table_cl.Rows[0];
                        dr["Output_RawBatch"] = row_cl["RawBatchProductionSum"];//生料
                        dr["Output_Clinker"] = row_cl["ClinkerProductionSum"];//熟料
                        dr["Output_Cogeneration"] = row_cl["PowerGenerationSum"];//发电量
                        dr["Consumption_CoalDust"] = row_cl["AmounttoCoalDustConsumptionSum"];//用煤量
                    }
                    DataTable table_dl = tzHelper.GetReportData("tz_Report", organizationID, _date, "table_ClinkerMonthlyElectricity_sum", "vDate='合计'");
                    if (table_dl.Rows.Count > 0)//
                    {
                        DataRow row_dl = table_dl.Rows[0];
                        dr["Electricity_RawBatch"] = row_dl["AmounttoRawBatchPreparationSum"];//用电量—生料制备
                        dr["Electricity_Clinker"] = row_dl["AmounttoFiringSystemSum"];//用电量-熟料烧成
                    }
                }
                //处理水泥磨
                if ("水泥磨" == type)
                {
                    //产量-水泥
                    DataTable table_cl = tzHelper.GetReportData("tz_Report", organizationID, _date, "table_CementMillMonthlyOutput");//水泥报表数据库中没有合计行
                    foreach (DataRow _row in table_cl.Rows)
                    {
                        _row["vDate"] = 1;
                    }
                    DataTable table_cl_sum = ReportHelper.MyTotalOn(table_cl, "vDate", "CementProductionSum");
                    if (table_cl_sum.Rows.Count > 0)
                    {
                        DataRow rowCL_Sum = table_cl_sum.Rows[0];
                        dr["Output_Cement"] = rowCL_Sum["CementProductionSum"];//产量-水泥
                    }
                    //用电量-水泥
                    DataTable table_dl = tzHelper.GetReportData("tz_Report", organizationID, _date, "table_CementMillMonthlyElectricity_sum");
                    foreach (DataRow _row in table_dl.Rows)
                    {
                        _row["vDate"] = 1;
                    }
                    DataTable table_dl_sum = ReportHelper.MyTotalOn(table_dl, "vDate", "AmounttoSum");
                    if (table_dl_sum.Rows.Count > 0)
                    {
                        DataRow rowDL_Sum = table_dl_sum.Rows[0];
                        dr["Electricity_Cement"] = rowDL_Sum["AmounttoCementPreparationSum"];
                    }
                }
            }
            foreach (DataRow dr in _temp.Rows)
            {
                string m_levelCode = dr["LevelCode"].ToString();
                if (m_levelCode.Length != productLevelLength)//生产线层次不用再求
                {
                    DataRow[] rows = _temp.Select("LevelCode like '" + m_levelCode + "%'");
                    //对于一些合计行做统计
                    foreach (DataRow _row in rows)
                    {
                        //电量-生料制备  
                        dr["Electricity_RawBatch"] = ReportHelper.MyToDecimal(dr["Electricity_RawBatch"]) + ReportHelper.MyToDecimal(_row["Electricity_RawBatch"]);
                        //电量-熟料烧成
                        dr["Electricity_Clinker"] = ReportHelper.MyToDecimal(dr["Electricity_Clinker"]) + ReportHelper.MyToDecimal(_row["Electricity_Clinker"]);
                        //电量-水泥制备
                        dr["Electricity_Cement"] = ReportHelper.MyToDecimal(dr["Electricity_Cement"]) + ReportHelper.MyToDecimal(_row["Electricity_Cement"]);
                        //消耗量-煤粉
                        dr["Consumption_CoalDust"] = ReportHelper.MyToDecimal(dr["Consumption_CoalDust"]) + ReportHelper.MyToDecimal(_row["Consumption_CoalDust"]);
                        //产量-生料制备
                        dr["Output_RawBatch"] = ReportHelper.MyToDecimal(dr["Output_RawBatch"]) + ReportHelper.MyToDecimal(_row["Output_RawBatch"]);
                        //产量-熟料烧成
                        dr["Output_Clinker"] = ReportHelper.MyToDecimal(dr["Output_Clinker"]) + ReportHelper.MyToDecimal(_row["Output_Clinker"]);
                        //产量-水泥制备
                        dr["Output_Cement"] = ReportHelper.MyToDecimal(dr["Output_Cement"]) + ReportHelper.MyToDecimal(_row["Output_Cement"]);
                        //产量-余热发电发电量
                        dr["Output_Cogeneration"] = ReportHelper.MyToDecimal(dr["Output_Cogeneration"]) + ReportHelper.MyToDecimal(_row["Output_Cogeneration"]);
                    }
                }
                //处理电耗等数据
                //电耗-生料制备
                dr["ElectricityConsumption_RawBatch"] = ReportHelper.MyToDecimal(dr["Output_RawBatch"]) != 0 ? Decimal.Parse((ReportHelper.MyToDecimal(dr["Electricity_RawBatch"]) / ReportHelper.MyToDecimal(dr["Output_RawBatch"])).ToString("#0.0000")) : 0;
                //电耗-熟料烧成
                dr["ElectricityConsumption_Clinker"] = ReportHelper.MyToDecimal(dr["Output_Clinker"]) != 0 ? Decimal.Parse((ReportHelper.MyToDecimal(dr["Electricity_Clinker"]) / ReportHelper.MyToDecimal(dr["Output_Clinker"])).ToString("#0.0000")) : 0;
                //电耗-水泥制备
                dr["ElectricityConsumption_Cement"] = ReportHelper.MyToDecimal(dr["Output_Cement"]) != 0 ? Decimal.Parse((ReportHelper.MyToDecimal(dr["Electricity_Cement"]) / ReportHelper.MyToDecimal(dr["Output_Cement"])).ToString("#0.00000")) : 0;
                //吨熟料综合电耗
                dr["ComprehensiveElectricityConsumption"] = ReportHelper.MyToDecimal(dr["Output_Clinker"]) != 0 ?Decimal.Parse((ReportHelper.MyToDecimal(dr["Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Electricity_Clinker"])
                        / ReportHelper.MyToDecimal(dr["Output_Clinker"])).ToString("#0.0000")) : 0;
                //吨熟料综合实物煤耗
                dr["ComprehensiveCoalConsumption"] = ReportHelper.MyToDecimal(dr["Output_Clinker"]) != 0 ? Decimal.Parse((ReportHelper.MyToDecimal(dr["Consumption_CoalDust"]) * 1000 / ReportHelper.MyToDecimal(dr["Output_Clinker"])).ToString("#0.0000")) : 0;
                //吨熟料发电量
                dr["ComprehensiveElectricityOutput"] = ReportHelper.MyToDecimal(dr["Output_Clinker"]) != 0 ? Decimal.Parse((ReportHelper.MyToDecimal(dr["Output_Cogeneration"]) / ReportHelper.MyToDecimal(dr["Output_Clinker"])).ToString("#0.0000")) : 0;

            }


            return _temp;
        }
        /// <summary>
        /// 将obj转换为decimal，如果为DBNull转为0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //private static decimal ReportHelper.MyToDecimal(object obj)
        //{
        //    if (obj is DBNull)
        //    {
        //        obj = 0;
        //        return Convert.ToDecimal(obj);
        //    }
        //    else
        //    {
        //        return Convert.ToDecimal(obj);
        //    }
        //}
    }
}
