using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly                        //cdy
{
    public class CementMonthlyElectricityConsumption
    {
        private static TZHelper _tzHelper;

        static CementMonthlyElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        /// <summary>
        /// 水泥（分品种）粉磨电耗月统计查询
        /// </summary>
        /// <param name="organizeID"></param>
        /// <param name="date"></param>
        /// <param name="aimTableName"></param>
        /// <returns></returns>
        public static DataTable TableQuery(string organizeID, string date)
        {
            DataTable Table4 = _tzHelper.CreateTableStructure("report_CementMonthlyElectricityConsumption");
            DataTable Temp = Table4.Clone();
            DataTable table_cl = _tzHelper.GetReportData("tz_Report", organizeID, date, "table_CementMillMonthlyOutput");
            DataTable table_dl = _tzHelper.GetReportData("tz_Report", organizeID, date, "table_CementMillMonthlyElectricity_sum");
            for (int i = 1; i <= 31; i++)
            {
                Temp.Clear();
                DataRow[] Rows_cl = table_cl.Select("vDate=" + i);//取出本月i号的产量行
                DataRow[] Rows_dl = table_dl.Select("vDate=" + i);//取出本月i号的电量行
                if (0 != Rows_cl.Length && 0 != Rows_dl.Length)
                {
                    foreach (DataRow dr in Rows_cl)//填充产量
                    {
                        DataRow row = Temp.NewRow();
                        string str = dr["vDate"].ToString();
                        row["vDate"] =  dr["vDate"];
                        row["CementTypes"] = dr["CementTypes"];
                        row["First_Output"] = Convert.ToInt64(dr["CementProductionFirstShift"]);
                        row["Second_Output"] = Convert.ToInt64(dr["CementProductionSecondShift"]);
                        row["Third_Output"] = Convert.ToInt64(dr["CementProductionThirdShift"]);
                        row["Amountto_Output"] = Convert.ToInt64(dr["CementProductionSum"]);
                        Temp.Rows.Add(row);
                    }
                    foreach (DataRow dr in Rows_dl)//填充电量
                    {
                        DataRow row = Temp.NewRow();
                        row["vDate"] = dr["vDate"];
                        row["CementTypes"] = dr["CementTypes"];
                        row["First_Electricity"] = Convert.ToInt64(dr["AmounttoFirstShift"]);
                        row["Second_Electricity"] = Convert.ToInt64(dr["AmounttoSecondShift"]);
                        row["Third_Electricity"] = Convert.ToInt64(dr["AmounttoThirdShift"]);
                        row["Amountto_Electricity"] = Convert.ToInt64(dr["AmounttoSum"]);
                        Temp.Rows.Add(row);
                    }
                    DataTable temp1 = ReportHelper.MyTotalOn(Temp, "CementTypes", "First_Output,First_Electricity,Second_Output,Second_Electricity,Third_Output,Third_Electricity,Amountto_Output,Amountto_Electricity");
                    DataTable temp2 = ReportHelper.MyTotalOn(temp1, "vDate", "First_Output,First_Electricity,Second_Output,Second_Electricity,Third_Output,Third_Electricity,Amountto_Output,Amountto_Electricity");
                    temp2.Rows[0]["CementTypes"] = "合计";
                    Table4.Merge(temp1);
                    Table4.Merge(temp2);
                }
            }//for循环结束
            DataTable Table5 = ReportHelper.MyTotalOn(Table4, "CementTypes", "First_Output,First_Electricity,Second_Output,Second_Electricity,Third_Output,Third_Electricity");
            for (int i = 0; i < Table5.Rows.Count; i++)
            {
                Table5.Rows[i]["vDate"] = "合计";
            }
            DataRow SumRow = Table4.NewRow();
            foreach (DataRow dr in Table5.Rows)
            {
                if (dr["CementTypes"].ToString() == "合计")
                {
                    SumRow = dr;
                }
                else
                {
                    Table4.Rows.Add(dr.ItemArray);
                }
            }
            Table4.Rows.Add(SumRow.ItemArray);
            int num = Table4.Rows.Count;
            for (int i = 0; i < num; i++)
            {
              
                if (0 != MyToDecimal(Table4.Rows[i]["First_Output"]))
                { 
                    Table4.Rows[i]["First_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["First_Electricity"]) / MyToDecimal(Table4.Rows[i]["First_Output"]);
                }
                if (0 != MyToDecimal(Table4.Rows[i]["Second_Output"]))
                {
                    Table4.Rows[i]["Second_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["Second_Electricity"]) / MyToDecimal(Table4.Rows[i]["Second_Output"]);
                }
                if (0 != MyToDecimal(Table4.Rows[i]["Third_Output"]))
                {
                    Table4.Rows[i]["Third_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["Third_Electricity"]) / MyToDecimal(Table4.Rows[i]["Third_Output"]);
                }
                if (0 != MyToDecimal(Table4.Rows[i]["Amountto_Output"]))
                {
                    Table4.Rows[i]["Amountto_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["Amountto_Electricity"]) / MyToDecimal(Table4.Rows[i]["Amountto_Output"]);
                }
                string pz = Table4.Rows[i]["CementTypes"].ToString().Trim();
                if ("合计" != pz)
                {
                    Table4.Rows[i]["ConvertCoefficient"] = _tzHelper.GetConvertCoefficient(pz); //Dictionary_zhxh[pz];
                    if (0 != MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]))
                    {
                        if(Table4.Rows[i]["First_ElectricityConsumption"] is DBNull)
                        {
                            Table4.Rows[i]["First_ElectricityConsumption"]=0;
                        }
                        Table4.Rows[i]["First_Convert_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["First_ElectricityConsumption"]) / MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]);
                    }
                    if (0 != MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]))
                    {
                        if (Table4.Rows[i]["Second_ElectricityConsumption"] is DBNull)
                        {
                            Table4.Rows[i]["Second_ElectricityConsumption"] = 0;
                        }
                        Table4.Rows[i]["Second_Convert_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["Second_ElectricityConsumption"]) / MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]);
                    }
                    if (0 != MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]))
                    {
                        if (Table4.Rows[i]["Third_ElectricityConsumption"] is DBNull)
                        {
                            Table4.Rows[i]["Third_ElectricityConsumption"] = 0;
                        }
                        Table4.Rows[i]["Third_Convert_ElectricityConsumption"] =  MyToDecimal(Table4.Rows[i]["Third_ElectricityConsumption"]) /  MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]);
                    }
                    if (0 != MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]))
                    {
                        if (Table4.Rows[i]["Amountto_ElectricityConsumption"] is DBNull)
                        {
                            Table4.Rows[i]["Amountto_ElectricityConsumption"] = 0;
                        }
                        Table4.Rows[i]["Amountto_Convert_ElectricityConsumption"] = MyToDecimal(Table4.Rows[i]["Amountto_ElectricityConsumption"]) / MyToDecimal(Table4.Rows[i]["ConvertCoefficient"]);
                    }
                }
            }
            return Table4;
        }
        public static decimal MyToDecimal(object obj)
        {
            if (obj is DBNull)
            {
                obj = 0;
                return Convert.ToDecimal(obj);
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }
    }
}
