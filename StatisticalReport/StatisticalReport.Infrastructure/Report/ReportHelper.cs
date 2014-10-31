using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Infrastructure.Report
{
    public class ReportHelper
    {
        #region  添加合计 分层次 cdy
        /// <summary>
        /// 按照sortStr在totalStr上合计。添加行合计
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortStr"></param>
        /// <param name="totalStr"></param>
        /// <returns></returns>
        public static DataTable MyTotalOn(DataTable table, string sortStr, string totalStr)//字符串sortStr,totalStr应用英文逗号分隔
        {
            if (table.Rows.Count == 0)
                return table;

            string[] SortArray = sortStr.Split(',', '，');
            string[] TotalArray = totalStr.Split(',', '，');
            DataTable Table = SortTable(table, SortArray);
            int n = SortArray.Length;//需要排序列的个数
            int m = TotalArray.Length;//需要合计列的个数
            int RowsNum = Table.Rows.Count;
            string Compare = "shshjs";
            DataTable[] T = new DataTable[n];
            for (int a = 0; a < n; a++)
            {
                T[a] = Table.Clone();
            }
            for (int t = 0; t < n; t++)
            {
                string str;
                int j = 0;
                int k = 0;
                int p = 0;
                foreach (DataRow dr1 in Table.Rows)
                {
                    p++;
                    str = dr1[SortArray[n - 1 - t]].ToString().Trim();//去掉前后的空格
                    //string str44 = dr1[1].ToString();
                    if (str != Compare)//不同的情况下
                    {
                        Compare = str;
                        DataRow row = dr1;
                        k++;//新生产表的行数
                        j = k - 1;//新表最后一行的索引号
                        for (int i = 0; i < t; i++)
                        {
                            //row[n-i-1] = null; 
                            row[SortArray[n - 1 - i]] = null;
                        }
                        T[t].Rows.Add(row.ItemArray);
                    }
                    else//相同的情况下
                    {

                        DataRow row = T[t].NewRow();
                        for (int i = 0; i < m; i++)
                        {
                            row = T[t].Rows[k - 1];//T1的最后一行
                            if (T[t].Rows[k - 1][TotalArray[i]] is DBNull)//数据库中的空在程序中为DBNull
                            {
                                T[t].Rows[k - 1][TotalArray[i]] = 0;
                            }
                            if (dr1[TotalArray[i]] is DBNull)
                            {
                                dr1[TotalArray[i]] = 0;
                            }
                            T[t].Rows[k - 1][TotalArray[i]] = Convert.ToDouble(T[t].Rows[k - 1][TotalArray[i]]) + Convert.ToDouble(dr1[TotalArray[i]]);
                        }
                    }
                }

            }
            for (int i = 0; i < n - 1; i++)
            {
                //T[n-1].Merge(T[n-i-2]);
                T[0].Merge(T[i + 1]);
            }
            //return T[0];
            //MyTableSort mts = new MyTableSort(T[0]);
            return SortTable(T[0], SortArray);
        }

        /// <summary>
        /// 除去specialField字段外的所有的字段都要做合计, 添加最后一列合计列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortStr"></param>
        /// <returns></returns>
        public static DataTable MyTotalOnAll(DataTable table, string sortStr, params string[] specialField)
        {
            if (table.Rows.Count == 0)
                return table;

            string[] SortArray = sortStr.Split(',', '，');
            List<string> list = new List<string>();
            foreach (DataColumn dc in table.Columns)//table中的所有的字段加入到list中
            {
                list.Add(dc.ToString());
            }
            foreach (string str in specialField)//从list中除去specialField字段，从而得到需要total的字段
            {
                list.Remove(str);
            }
            int m = list.Count;//需要合计列的个数
            string[] TotalArray = new string[m];
            for (int i = 0; i < m; i++)
            {
                TotalArray[i] = list.ElementAt(i);
            }
            DataTable Table = SortTable(table, SortArray);
            int n = SortArray.Length;//需要排序列的个数
            //int m = TotalArray.Length;//需要合计列的个数
            int RowsNum = Table.Rows.Count;
            string Compare = "shshjs";
            DataTable[] T = new DataTable[n];
            for (int a = 0; a < n; a++)
            {
                T[a] = Table.Clone();
            }
            for (int t = 0; t < n; t++)
            {
                string str;
                int j = 0;
                int k = 0;
                int p = 0;
                foreach (DataRow dr1 in Table.Rows)
                {
                    p++;
                    str = dr1[SortArray[n - 1 - t]].ToString().Trim();//去掉前后的空格
                    //string str44 = dr1[1].ToString();
                    if (str != Compare)//不同的情况下
                    {
                        Compare = str;
                        DataRow row = dr1;
                        k++;//新生产表的行数
                        j = k - 1;//新表最后一行的索引号
                        for (int i = 0; i < t; i++)
                        {
                            //row[n-i-1] = null; 
                            row[SortArray[n - 1 - i]] = null;
                        }
                        T[t].Rows.Add(row.ItemArray);
                    }
                    else//相同的情况下
                    {

                        DataRow row = T[t].NewRow();
                        for (int i = 0; i < m; i++)
                        {
                            row = T[t].Rows[k - 1];//T1的最后一行
                            if (T[t].Rows[k - 1][TotalArray[i]] is DBNull)//数据库中的空在程序中为DBNull
                            {
                                T[t].Rows[k - 1][TotalArray[i]] = 0;
                            }
                            if (dr1[TotalArray[i]] is DBNull)
                            {
                                dr1[TotalArray[i]] = 0;
                            }
                            T[t].Rows[k - 1][TotalArray[i]] = Convert.ToDouble(T[t].Rows[k - 1][TotalArray[i]]) + Convert.ToDouble(dr1[TotalArray[i]]);
                        }
                    }
                }

            }
            for (int i = 0; i < n - 1; i++)
            {
                //T[n-1].Merge(T[n-i-2]);
                T[0].Merge(T[i + 1]);
            }
            //return T[0];
            //MyTableSort mts = new MyTableSort(T[0]);
            return SortTable(T[0], SortArray);
        }
        #endregion

        #region 添加合计行   zcs
        /// <summary>
        /// 添加合计行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="referenceField"></param>
        /// <param name="totalField"></param>
        public static void GetTotal(DataTable table, string referenceField, string totalField)     //zcs
        {
            string[] TotalArray = totalField.Split(',', '，');

            DataRow totalRow = table.NewRow();
            totalRow[referenceField] = "合计";

            foreach (string item in TotalArray)
            {
                totalRow[item] = 0;
            }

            foreach (DataRow dr in table.Rows)
            {
                foreach (string item in TotalArray)
                {
                    totalRow[item] = Convert.ToDouble(totalRow[item]) + Convert.ToDouble(dr[item]);
                }
            }
            table.Rows.Add(totalRow);
        }
        #endregion

        #region 修改DataTable列的数据类型  zcs
        /// <summary>
        /// 将DataTable中的decimal列修改为Int64
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable ReportDataToInteger(DataTable source)
        {
            DataTable result = ConvertColumnType(source);

            foreach (DataRow item in source.Rows)
            {
                DataRow dr = result.NewRow();
                CopyDatas(dr, item);
                result.Rows.Add(dr);
            }

            return result;
        }
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="targetRow"></param>
        /// <param name="sourceRow"></param>
        private static void CopyDatas(DataRow targetRow, DataRow sourceRow)
        {
            for (int i = 0; i < sourceRow.ItemArray.Count(); i++)
            {
                targetRow[i] = sourceRow[i];
            }
        }
        /// <summary>
        /// 转换列的数据类型
        /// </summary>
        /// <param name="source">数据源表</param>
        /// <returns></returns>
        private static DataTable ConvertColumnType(DataTable source)
        {
            DataTable result = new DataTable();

            foreach (DataColumn item in source.Columns)
            {
                if (item.DataType != typeof(decimal))
                {
                    DataColumn dc = new DataColumn(item.ColumnName, item.DataType);
                    result.Columns.Add(dc);
                }
                else
                {
                    DataColumn dc = new DataColumn(item.ColumnName, typeof(Int64));
                    result.Columns.Add(dc);
                }
            }

            return result;
        }
        #endregion

        #region  分组合计，不分层次 cdy
        /// <summary>
        /// 给目标表添加合计
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable ReportAddToSumRow(DataTable table)
        {
            if (table.Rows.Count == 0)
                return table;

            table.Columns.Remove("ID");
            table.Columns.Remove("KeyID");
            DataTable tableOfSum = ReportHelper.MyTotalOnAll(table, "CementTypes", "vDate", "CementTypes");//最后的合计部分
            foreach (DataRow dr in tableOfSum.Rows)
            {
                dr["vDate"] = "总总合计";
            }
            DataTable temp = table.Copy();
            temp.Merge(tableOfSum);
            DataTable dayOfSum = ReportHelper.GroupByTotalAll(temp, "vDate", "vDate", "CementTypes");//每日后面的合计和总合计后面的合计
            foreach (DataRow dr in dayOfSum.Rows)
            {
                dr["CementTypes"] = "总总合计";
            }
            DataTable resultTableNoSort = new DataTable();
            resultTableNoSort.Merge(temp);
            resultTableNoSort.Merge(dayOfSum);
            DataTable resulttable = ReportHelper.SortTable(resultTableNoSort, "vDate,CementTypes");

            foreach (DataRow dr in resulttable.Rows)
            {
                if (dr["vDate"].ToString() == "总总合计")
                {
                    dr["vDate"] = "合计";
                }
                if (dr["CementTypes"].ToString() == "总总合计")
                {
                    dr["CementTypes"] = "合计";
                }
            }
            return resulttable;
        }
        /// <summary>
        /// 分组合计，没有层次
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortStr"></param>
        /// <param name="totalStr"></param>
        /// <returns></returns>
        public static DataTable GroupByTotal(DataTable table, string sortStr, string totalStr)//分组合计
        {
            if (table.Rows.Count == 0)
                return table;

            string[] SortArray = sortStr.Split(',', '，');
            string[] TotalArray = totalStr.Split(',', '，');
            DataTable Table = SortTable(table, SortArray);
            DataTable temp = Table.Clone();
            int n = SortArray.Length;//需要排序列的个数
            int m = TotalArray.Length;//需要合计列的个数
            int RowsNum = Table.Rows.Count;
            string[] Compare = new string[n];
            string[] str = new string[n];
            for (int i = 0; i < n; i++)
            {
                str[i] = "_sdffas";
            }

            int k = 0;
            foreach (DataRow dr in Table.Rows)
            {
                for (int i = 0; i < n; i++)
                {
                    str[i] = dr[SortArray[i]].ToString().Trim();//去掉前后的空格
                    //str2 = dr[SortArray[1]].ToString().Trim();//去掉前后的空格
                }
                if (CompareArray(str, Compare))//相同的情况下
                {
                    DataRow row = temp.NewRow();
                    for (int i = 0; i < m; i++)
                    {
                        row = temp.Rows[k - 1];//T1的最后一行

                        if (Table.Rows[k - 1][TotalArray[i]] is DBNull)//数据库中的空在程序中为DBNull
                        {
                            Table.Rows[k - 1][TotalArray[i]] = 0;
                        }
                        if (dr[TotalArray[i]] is DBNull)
                        {
                            dr[TotalArray[i]] = 0;
                        }
                        if (temp.Rows[k - 1][TotalArray[i]] is DBNull)
                        { temp.Rows[k - 1][TotalArray[i]] = 0; }
                        if (dr[TotalArray[i]] is DBNull)
                        { dr[TotalArray[i]] = 0; }
                        temp.Rows[k - 1][TotalArray[i]] = Convert.ToDouble(temp.Rows[k - 1][TotalArray[i]]) + Convert.ToDouble(dr[TotalArray[i]]);
                    }
                }
                else//不同的情况下
                {
                    for (int i = 0; i < n; i++)
                    {
                        Compare[i] = str[i];
                    }
                    //    Compare1 = str1;
                    //Compare2 = str2;
                    DataRow row = dr;
                    k++;//新生产表的行数
                    temp.Rows.Add(row.ItemArray);
                }
            }
            return temp;
        }

        /// <summary>
        /// 分组合计全部字段，没有层次
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortStr"></param>
        /// <param name="totalStr"></param>
        /// <returns></returns>
        public static DataTable GroupByTotalAll(DataTable table, string sortStr, params string[] specialField)//分组合计
        {
            if (table.Rows.Count == 0)
                return table;

            string[] SortArray = sortStr.Split(',', '，');
            List<string> list = new List<string>();
            foreach (DataColumn dc in table.Columns)//table中的所有的字段加入到list中
            {
                list.Add(dc.ToString());
            }
            foreach (string str in specialField)//从list中除去specialField字段，从而得到需要total的字段
            {
                list.Remove(str);
            }
            int m = list.Count;//需要合计列的个数
            string[] TotalArray = new string[m];
            for (int i = 0; i < m; i++)
            {
                TotalArray[i] = list.ElementAt(i);
            }
            DataTable Table = SortTable(table, SortArray);
            DataTable temp = Table.Clone();
            int n = SortArray.Length;//需要排序列的个数
            //int m = TotalArray.Length;//需要合计列的个数
            int RowsNum = Table.Rows.Count;
            string[] Compare = new string[n];
            string[] strs = new string[n];
            for (int i = 0; i < n; i++)
            {
                strs[i] = "_sdffas";
            }

            int k = 0;
            foreach (DataRow dr in Table.Rows)
            {
                for (int i = 0; i < n; i++)
                {
                    strs[i] = dr[SortArray[i]].ToString().Trim();//去掉前后的空格
                    //str2 = dr[SortArray[1]].ToString().Trim();//去掉前后的空格
                }
                if (CompareArray(strs, Compare))//相同的情况下
                {
                    DataRow row = temp.NewRow();
                    for (int i = 0; i < m; i++)
                    {
                        row = temp.Rows[k - 1];//T1的最后一行

                        if (Table.Rows[k - 1][TotalArray[i]] is DBNull)//数据库中的空在程序中为DBNull
                        {
                            Table.Rows[k - 1][TotalArray[i]] = 0;
                        }
                        if (dr[TotalArray[i]] is DBNull)
                        {
                            dr[TotalArray[i]] = 0;
                        }
                        if (temp.Rows[k - 1][TotalArray[i]] is DBNull)
                        { temp.Rows[k - 1][TotalArray[i]] = 0; }
                        if (dr[TotalArray[i]] is DBNull)
                        { dr[TotalArray[i]] = 0; }
                        temp.Rows[k - 1][TotalArray[i]] = Convert.ToDouble(temp.Rows[k - 1][TotalArray[i]]) + Convert.ToDouble(dr[TotalArray[i]]);
                    }
                }
                else//不同的情况下
                {
                    for (int i = 0; i < n; i++)
                    {
                        Compare[i] = strs[i];
                    }
                    //    Compare1 = str1;
                    //Compare2 = str2;
                    DataRow row = dr;
                    k++;//新生产表的行数
                    temp.Rows.Add(row.ItemArray);
                }
            }
            return temp;
        }
        #endregion

        /// <summary>
        /// 比较数组
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        private static bool CompareArray(string[] array1, string[] array2)
        {
            int n = array1.Length;
            int m = array2.Length;
            if (n == m)
            {
                for (int i = 0; i < n; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #region DataTable排序 cdy
        /// <summary>
        /// 以默认升序方式排列
        /// </summary>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable _oldTable, params string[] OrderBy)
        {

            DataView view = new DataView(_oldTable);
            StringBuilder orderByStr = new StringBuilder();
            int i = 0;
            int n = OrderBy.Length;
            foreach (string str in OrderBy)
            {
                i++;
                orderByStr.Append(str);
                if (i < n)
                {
                    orderByStr.Append(",");
                }
            }
            string strn = orderByStr.ToString();
            view.Sort = strn;
            DataTable NewTable = view.ToTable();
            return NewTable;

        }
        /// <summary>
        /// 以可选的升序或降序方式排列
        /// </summary>
        /// <param name="px"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable _oldTable, string px, params string[] OrderBy)
        {
            DataView view = new DataView(_oldTable);
            StringBuilder orderByStr = new StringBuilder();
            int i = 0;
            int n = OrderBy.Length;
            // orderByStr.Append("State "+px+",");
            foreach (string str in OrderBy)
            {
                i++;
                orderByStr.Append(str);
                if (i < n)
                {
                    orderByStr.Append(",");
                }
            }
            orderByStr.Append(" " + px);
            string strn = orderByStr.ToString();
            view.Sort = strn;
            DataTable NewTable = view.ToTable();
            return NewTable;

        }
        #endregion

        /// <summary>
        /// 格式化数据类型为字符串类型，补全小数位数和整数位数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="_long"></param>
        /// <param name="_decimalNum"></param>
        /// <returns></returns>
        public static string MyToString(double num, int _long, int _decimalNum)
        {
            string result = "";//最终结果
            int n = num.ToString().IndexOf(".");
            if (n > 0)//如果有小数部分
            {
                string[] oldStr = num.ToString().Split('.');//将整数部分和小数部分拆开
                string resultInt = oldStr[0];//整数部分
                string resultDcm = oldStr[1];//小数部分
                int intNum = _long - _decimalNum;//要求的整数部分的长度
                int oldIntNum = oldStr[0].Length;//原先整数部分的长度
                int oldDecimalNum = oldStr[1].Length;//原先小数部分的长度
                //先处理整数部分
                if (oldIntNum >= intNum)//如果原来的整数部分长度大于要求的整数部分长度，整数部分不变
                {
                    resultInt = oldStr[0];
                }
                else
                {
                    for (int i = 0; i < intNum - oldIntNum; i++)
                    {
                        resultInt = "0" + resultInt;
                    }
                }
                //在处理小数部分
                if (oldDecimalNum >= _decimalNum)
                {
                    resultDcm = oldStr[1].Substring(0, _decimalNum);
                }
                else
                {
                    for (int i = 0; i < _decimalNum - oldDecimalNum; i++)
                    {
                        resultDcm = resultDcm + "0";
                    }
                }
                //将整数部分和小数部分合并在一起
                result = resultInt + "." + resultDcm;
                return result;
            }
            else//如果没有小数部分
            {
                string resultInt = num.ToString();
                int m = resultInt.Length;
                if (_long > m)
                {
                    for (int i = 0; i < _long - m; i++)
                    {
                        resultInt = "0" + resultInt;
                    }
                }
                return resultInt;
            }
        }
    }
}
