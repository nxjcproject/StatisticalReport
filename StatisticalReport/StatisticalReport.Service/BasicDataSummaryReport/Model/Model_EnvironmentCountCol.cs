using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport.Model
{
    public class Model_EnvironmentCountCol
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

        private int _CountFirstBNotZero;

        public int CountFirstBNotZero
        {
            get { return _CountFirstBNotZero; }
            set { _CountFirstBNotZero = value; }
        }
        private int _CountSecondBNotZero;

        public int CountSecondBNotZero
        {
            get { return _CountSecondBNotZero; }
            set { _CountSecondBNotZero = value; }
        }
        private int _CountThirdBNotZero;

        public int CountThirdBNotZero
        {
            get { return _CountThirdBNotZero; }
            set { _CountThirdBNotZero = value; }
        }
        private int _CountPeakBNotZero;

        public int CountPeakBNotZero
        {
            get { return _CountPeakBNotZero; }
            set { _CountPeakBNotZero = value; }
        }
        private int _CountValleyBNotZero;

        public int CountValleyBNotZero
        {
            get { return _CountValleyBNotZero; }
            set { _CountValleyBNotZero = value; }
        }
        private int _CountFlatBNotZero;

        public int CountFlatBNotZero
        {
            get { return _CountFlatBNotZero; }
            set { _CountFlatBNotZero = value; }
        }
        private int _CountA班NotZero;

        public int CountA班NotZero
        {
            get { return _CountA班NotZero; }
            set { _CountA班NotZero = value; }
        }
        private int _CountB班NotZero;

        public int CountB班NotZero
        {
            get { return _CountB班NotZero; }
            set { _CountB班NotZero = value; }
        }
        private int _CountC班NotZero;

        public int CountC班NotZero
        {
            get { return _CountC班NotZero; }
            set { _CountC班NotZero = value; }
        }
        private int _CountD班NotZero;

        public int CountD班NotZero
        {
            get { return _CountD班NotZero; }
            set { _CountD班NotZero = value; }
        }
    }
}
