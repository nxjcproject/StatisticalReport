using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class Model_EnvironmentSumCol
    {
        private string _OrganizationID;//熟料产线组织机构ID

        public string OrganizationID
        {
            get { return _OrganizationID; }
            set { _OrganizationID = value; }
        }
        private string _ItemName;//环保数据项目名称

        public string ItemName
        {
            get { return _ItemName; }
            set { _ItemName = value; }
        }
        private string _VariableId;//环保变量ID

        public string VariableId
        {
            get { return _VariableId; }
            set { _VariableId = value; }
        }
        private decimal _SumFirstB;

        public decimal SumFirstB
        {
            get { return _SumFirstB; }
            set { _SumFirstB = value; }
        }
        private decimal _SumSecondB;

        public decimal SumSecondB
        {
            get { return _SumSecondB; }
            set { _SumSecondB = value; }
        }
        private decimal _SumThirdB;

        public decimal SumThirdB
        {
            get { return _SumThirdB; }
            set { _SumThirdB = value; }
        }
        private decimal _SumPeakB;

        public decimal SumPeakB
        {
            get { return _SumPeakB; }
            set { _SumPeakB = value; }
        }
        private decimal _SumValleyB;

        public decimal SumValleyB
        {
            get { return _SumValleyB; }
            set { _SumValleyB = value; }
        }
        private decimal _SumFlatB;

        public decimal SumFlatB
        {
            get { return _SumFlatB; }
            set { _SumFlatB = value; }
        }
        private decimal _SumA班;

        public decimal SumA班
        {
            get { return _SumA班; }
            set { _SumA班 = value; }
        }
        private decimal _SumB班;

        public decimal SumB班
        {
            get { return _SumB班; }
            set { _SumB班 = value; }
        }
        private decimal _SumC班;

        public decimal SumC班
        {
            get { return _SumC班; }
            set { _SumC班 = value; }
        }
        private decimal _SumD班;

        public decimal SumD班
        {
            get { return _SumD班; }
            set { _SumD班 = value; }
        }

        


    }
}
