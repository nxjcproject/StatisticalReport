using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class CementMilYearlyEnergyConsumption                       //cdy
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        static CementMilYearlyEnergyConsumption()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        /// <summary>
        /// 水泥粉磨能耗年统计分析
        /// </summary>
        /// <param name="_organizeID"></param>
        /// <param name="_year"></param>
        /// <returns></returns>
        public static DataTable TableQuery(string _organizeID, string _year)
        {
            DataTable temp1;
            temp1 = tzHelper.CreateTableStructure("report_CementMilYearlyEnergyConsumption");//目标数据表
            int MonthEnd;
            if (DateTime.Now.Year.ToString() != _year)
            { MonthEnd = 12; }
            else
            { MonthEnd = Convert.ToInt32(DateTime.Now.Month); }

            //形成产量数据----------------start----------------------
            DataTable temp_cl;
            temp_cl = tzHelper.CreateTableStructure("table_CementMillMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "table_CementMillMonthlyOutput");
                foreach (DataRow dr in temp2.Rows)
                {
                    dr["vDate"] = "合计";
                }
                temp2 = ReportHelper.MyTotalOn(temp2, "vDate", "CementProductionSum");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Output_Cement_Monthly"] = dr["CementProductionSum"];//水泥制备
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Output_Cement_Accumulative"] = dr["CementProductionSum"];//水泥制备
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = tzHelper.CreateTableStructure("table_CementMillMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "table_CementMillMonthlyElectricity_sum");
                foreach (DataRow dr in temp2.Rows)
                {
                    dr["vDate"] = "合计";
                }
                temp2 = ReportHelper.MyTotalOn(temp2, "vDate", "AmounttoCementPreparationSum,CementGrindingSum,AdmixturePreparationSum,AmounttoCementPackagingSum");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_Cement_Monthly"] = Convert.ToInt64(dr["AmounttoCementPreparationSum"]);
                    row["Electricity_CementGrinding_Monthly"] = Convert.ToInt64(dr["CementGrindingSum"]);
                    row["Electricity_AdmixturePreparation_Monthly"] = Convert.ToInt64(dr["AdmixturePreparationSum"]);
                    row["Electricity_BagsBulk_Monthly"] = Convert.ToInt64(dr["AmounttoCementPackagingSum"]);
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_Cement_Accumulative"] = Convert.ToInt64(dr["AmounttoCementPreparationSum"]);
                    row["Electricity_CementGrinding_Accumulative"] = Convert.ToInt64(dr["CementGrindingSum"]);
                    row["Electricity_AdmixturePreparation_Accumulative"] = Convert.ToInt64(dr["AdmixturePreparationSum"]);
                    row["Electricity_BagsBulk_Accumulative"] = Convert.ToInt64(dr["AmounttoCementPackagingSum"]);
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------
            string sumString = "Electricity_Cement_Monthly,Electricity_Cement_Accumulative,Electricity_CementGrinding_Monthly,"
                + "Electricity_CementGrinding_Accumulative,Electricity_AdmixturePreparation_Monthly,"
                + "Electricity_AdmixturePreparation_Accumulative,Electricity_BagsBulk_Monthly,Electricity_BagsBulk_Accumulative,"
                + "Output_Cement_Monthly,Output_Cement_Accumulative";
            DataTable temp = ReportHelper.MyTotalOn(temp1, "vDate", sumString);    // temp每月明细
            foreach (DataRow dr in temp.Rows)
            {
                //电耗—水泥制备-本月
                if (MyToDecimal(dr["Output_Cement_Monthly"]) != 0)
                {
                    dr["ElectricityConsumption_Cement_Monthly"] = MyToDecimal(dr["Electricity_Cement_Monthly"]) / MyToDecimal(dr["Output_Cement_Monthly"]);
                }
                //电耗-水泥制备-累计
               if(MyToDecimal(dr["Output_Cement_Accumulative"])!=0)
               {
                   dr["ElectricityConsumption_Cement_Accumulative"] = MyToDecimal(dr["Electricity_Cement_Accumulative"]) / MyToDecimal(dr["Output_Cement_Accumulative"]);
                }
                //电耗-水泥磨-本月  //用 水泥磨/水泥制备 不知道对不对 
                if(MyToDecimal(dr["Output_Cement_Monthly"])!=0)
                {
                    dr["ElectricityConsumption_CementGrinding_Monthly"] = MyToDecimal(dr["Electricity_CementGrinding_Monthly"]) / MyToDecimal(dr["Output_Cement_Monthly"]);
                }
                //电耗-水泥磨-累计
                if (MyToDecimal(dr["Output_Cement_Accumulative"])!=0)
                {
                    dr["ElectricityConsumption_CementGrinding_Accumulative"] = MyToDecimal(dr["Electricity_CementGrinding_Accumulative"]) / MyToDecimal(dr["Output_Cement_Accumulative"]);
                }


                //吨水泥综合电耗-本月
                if (MyToDecimal(dr["Output_Cement_Monthly"])!=0)
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"]
                        = (MyToDecimal(dr["Electricity_Cement_Monthly"])
                        + MyToDecimal(dr["Electricity_AdmixturePreparation_Monthly"])
                        + MyToDecimal(dr["Electricity_BagsBulk_Monthly"]))
                        / MyToDecimal(dr["Output_Cement_Monthly"]);
                }
                //吨水泥综合电耗-累计
                if (MyToDecimal(dr["Output_Cement_Accumulative"]) != 0)
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"]
                        = (MyToDecimal(dr["Electricity_Cement_Accumulative"])
                        + MyToDecimal(dr["Electricity_AdmixturePreparation_Accumulative"])
                        + MyToDecimal(dr["Electricity_BagsBulk_Accumulative"]))
                        / MyToDecimal(dr["Output_Cement_Accumulative"]);
                }

            }
            return temp;
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
