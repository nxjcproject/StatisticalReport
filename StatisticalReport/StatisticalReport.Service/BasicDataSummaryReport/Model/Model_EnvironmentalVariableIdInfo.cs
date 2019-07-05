using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport.Model
{
    public class Model_EnvironmentalVariableIdInfo
    {
        private string _OrganizationID;//熟料产线组织机构ID
        private string _ItemName;//环保数据项目名称
        private string _VariableId;//环保变量ID
        public string OrganizationID
        {
            get { return _OrganizationID; }
            set { _OrganizationID = value; }
        }
        public string ItemName
        {
            get { return _ItemName; }
            set { _ItemName = value; }
        }
        public string VariableId
        {
            get { return _VariableId; }
            set { _VariableId = value; }
        }
    }
    
}
