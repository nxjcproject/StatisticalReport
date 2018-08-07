function doPrint(myTableString) {

    var m_Div = $('<div></div');
    var m_TableObj = $(myTableString);
    m_TableObj.appendTo(m_Div);
    //m_Div.append(myTableString);

    //m_PrintDialog.document.write(tableString);
    
    //var m_Options = { mode: 'popup', popClose: false};
    //m_Div.printArea();
    m_Div.jqprint();
}

function PrintHtml(myDataTable) {
    var m_TableString = myDataTable;
    doPrint(m_TableString);
}

/////////////////////获得DataGrid的Html//////////////////////
function GetDataGridTableHtml(myDataGridId, myTitleName, mySelectDatetime) {
    var m_DataGridDataObj = $('#' + myDataGridId).datagrid("getData");
    var test = $('#' + myDataGridId).datagrid("options")["frozenColumns"];
    var asd = (test == []);
    var m_DataGridFrozenColumnsObj = $('#' + myDataGridId).datagrid("options")["frozenColumns"].length != 0 ? $('#' + myDataGridId).datagrid("options")["frozenColumns"][0] : [];
    var m_DataGridColumnsObj = $('#' + myDataGridId).datagrid("options")["columns"] != [] ? $('#' + myDataGridId).datagrid("options")["columns"][0] : [];


    var m_DataGridTableHtml = "";
    var m_frozenColumnsCount = m_DataGridFrozenColumnsObj.length;
    var m_ColumnsCount = m_DataGridColumnsObj.length;
    var m_TitleSpan = m_frozenColumnsCount + m_ColumnsCount;
    var m_TitleHtml = '<tr><td colspan = ' + m_TitleSpan + ' style = "font-size:18pt; text-align:center; font-weight:bold;">' + myTitleName + '</td></tr>';
    m_TitleHtml = m_TitleHtml + '<tr><td colspan = ' + m_TitleSpan + ' style = "text-align:center; "> 统计日期: ' + mySelectDatetime + '</td></tr>';
    var m_ColumnNamesHtml = "<tr>";
    //////////////////////////字段名////////////////////
    for (var j = 0; j < m_frozenColumnsCount; j++) {
        if (m_DataGridFrozenColumnsObj[j]["hidden"] != true) {
            m_ColumnNamesHtml = m_ColumnNamesHtml + '<td style = "border:0.1pt solid black; text-align:center;">' + m_DataGridFrozenColumnsObj[j]["title"] + '</td>';
        }
    }
    for (var j = 0; j < m_ColumnsCount; j++) {
        if (m_DataGridColumnsObj[j]["hidden"] != true) {
            m_ColumnNamesHtml = m_ColumnNamesHtml + '<td style = "border:0.1pt solid black; text-align:center;">' + m_DataGridColumnsObj[j]["title"] + '</td>';
        }
    }
    ////////////////////////行数据//////////////////////
    m_ColumnNamesHtml = m_ColumnNamesHtml + "</tr>";
    for (var i = 0; i < m_DataGridDataObj["rows"].length; i++) {
        m_ColumnNamesHtml = m_ColumnNamesHtml + "<tr>"
        for (var j = 0; j < m_frozenColumnsCount; j++) {
            if (m_DataGridFrozenColumnsObj[j]["hidden"] != true) {
                m_ColumnNamesHtml = m_ColumnNamesHtml + '<td style = "border:0.1pt solid black; text-align:center;">' + m_DataGridDataObj["rows"][i][m_DataGridFrozenColumnsObj[j]["field"]] + '</td>';
            }
        }
        for (var j = 0; j < m_ColumnsCount; j++) {
            if (m_DataGridColumnsObj[j]["hidden"] != true) {
                m_ColumnNamesHtml = m_ColumnNamesHtml + '<td style = "border:0.1pt solid black; text-align:center;">' + m_DataGridDataObj["rows"][i][m_DataGridColumnsObj[j]["field"]] + '</td>';
            }
        }
        m_ColumnNamesHtml = m_ColumnNamesHtml + "</tr>";
    }
    return m_DataGridTableHtml = '<table style = "border:0px;margin:0px;border-collapse:collapse;border-spacing:0px;padding:0px;">' + m_TitleHtml + m_ColumnNamesHtml + '</table>';

}

////////////////////获得树形结构的Html///////////////////////

var MaxDepth = 0;
//TreeGrid的DomId,导出日报标题名称,树形节点名称的字段,选择的组织机构名称,选择的时间
function GetTreeTableHtml(myTreeGridDomId, myTitleName, myTreeTitleColumn, mySelectOrganizationName, mySelectDatetime) {
    var m_ColumnNamesHtml = "";

    var ValueColumns = GetTreeGridColumns(myTreeGridDomId);
    var m_NewTreeDateObj = GetTreeRootNode(myTreeGridDomId);
    var TreeHtml = GetTreeHtml(m_NewTreeDateObj, "", ValueColumns, myTreeTitleColumn);
    ////////////////////以下是标题以及列名///////////////////
    var m_ColumnNamesSpan = MaxDepth + 1;
    var m_TitleSpan = MaxDepth + 1 + ValueColumns.length;
    var m_TitleHtml = '<tr><td colspan = ' + m_TitleSpan + ' style = "font-size:18pt; text-align:center; font-weight:bold;">' + mySelectOrganizationName + myTitleName + '</td></tr>';
    m_TitleHtml = m_TitleHtml + '<tr><td colspan = ' + m_TitleSpan + ' style = "text-align:center; "> 统计日期: ' + mySelectDatetime + '</td></tr>';
    for (var i = 0; i < ValueColumns.length + 1; i++) {
        if (i == 0) {
            m_ColumnNamesHtml = '<tr><td colspan = ' + m_ColumnNamesSpan + ' style = "border:0.1pt solid black; text-align:center;">区域</td>';
        }
        else {
            m_ColumnNamesHtml = m_ColumnNamesHtml + '<td style = "border:0.1pt solid black; text-align:center;">' + ValueColumns[i - 1]["title"] + '</td>';
        }
    }
    TreeHtml = m_ColumnNamesHtml + "</tr>" + TreeHtml;
    TreeHtml = '<table style = "border:0px;margin:0px;border-collapse:collapse;border-spacing:0px;padding:0px;">' + m_TitleHtml + TreeHtml + '</table>';
    return TreeHtml;
}
function GetTreeRootNode(myTreeGridDomId) {
    var m_OldTreeDateObj = $('#' + myTreeGridDomId).treegrid("getData");
    var m_NewTreeDateObj = jQuery.extend(true, {}, m_OldTreeDateObj);   //深度克隆对象
    if (m_NewTreeDateObj != null && m_NewTreeDateObj != undefined && m_NewTreeDateObj.length != 0) {
        $.each(m_NewTreeDateObj, function (i, value) {
            //this;      //this指向当前元素
            //i;         //i表示Array当前下标
            //value;     //value表示Array当前元素
            value["Depth"] = 0;         //深度
            value["AllChildrenCount"] = GetSubTreeNode(0, value);     //所有的孩子总数，包括所有子节点以及子节点的孩子节点
        });
    }
    return m_NewTreeDateObj;
}
function GetSubTreeNode(myDepth, myParentTreeNode) {
    var m_Nodes = myParentTreeNode["children"];
    var m_NodesCount = 0;
    if (m_Nodes != null && m_Nodes != undefined && m_Nodes.length != 0) {
        myParentTreeNode["IsLeaf"] = false;
        m_NodesCount = m_Nodes.length;
        $.each(m_Nodes, function (i, value) {
            value["Depth"] = myDepth + 1;         //深度
            value["AllChildrenCount"] = GetSubTreeNode(myDepth + 1, value);     //所有的孩子总数，包括所有子节点以及子节点的孩子节点
            m_NodesCount = m_NodesCount + value["AllChildrenCount"];
        });
    }
    else {
        myParentTreeNode["IsLeaf"] = true;
        if (myParentTreeNode["Depth"] > MaxDepth) {
            MaxDepth = myParentTreeNode["Depth"];
        }
    }
    return m_NodesCount;
}

function GetTreeHtml(myTreeDateObj, myHtmlString, myValueColumns, myTreeTitleColumns) {
    var m_HtmlString = "";
    if (myTreeDateObj != null && myTreeDateObj != undefined && myTreeDateObj.length != 0) {
        $.each(myTreeDateObj, function (i, value) {
            //this;      //this指向当前元素
            //i;         //i表示Array当前下标
            //value;     //value表示Array当前元素
            if (value["IsLeaf"] != true) {
                m_HtmlString = m_HtmlString + "<tr>" + GetNodeTreeData(value, myTreeTitleColumns, myValueColumns) + "</tr>";
                m_HtmlString = m_HtmlString + GetTreeHtml(value["children"], m_HtmlString, myValueColumns, myTreeTitleColumns);
            }
            else {
                m_HtmlString = m_HtmlString + "<tr>" + GetLeafTreeData(value, myTreeTitleColumns, myValueColumns) + "</tr>";
            }
        });
    }
    return m_HtmlString;
}
function GetNodeTreeData(myNode, myTitleColumnName, myValueColumns) {
    var m_ColumnString = "";
    var m_AllChildrenCount = myNode["AllChildrenCount"] + 1;
    for (var i = myNode["Depth"]; i <= MaxDepth; i++) {
        if (i == myNode["Depth"]) {
            m_ColumnString = m_ColumnString + '<td rowspan = ' + m_AllChildrenCount + ' style = "border:0.1pt solid black;">' + myNode[myTitleColumnName] + '</td>';
        }
        else {
            m_ColumnString = m_ColumnString + '<td style = "border:0.1pt solid black;"></td>';
        }
    }
    for (var i = 0; i < myValueColumns.length; i++) {
        m_ColumnString = m_ColumnString + '<td style = "border:0.1pt solid black;">' + myNode[myValueColumns[i]["field"]] + '</td>';
    }
    return m_ColumnString;
}
function GetLeafTreeData(myNode, myTitleColumnName, myValueColumns) {
    var m_ColumnString = "";
    for (var i = myNode["Depth"]; i <= MaxDepth; i++) {
        if (i == myNode["Depth"]) {
            m_ColumnString = m_ColumnString + '<td style = "border:0.1pt solid black;">' + myNode[myTitleColumnName] + '</td>';
        }
        else {
            m_ColumnString = m_ColumnString + '<td style = "border:0.1pt solid black;"></td>';
        }
    }
    for (var i = 0; i < myValueColumns.length; i++) {
        m_ColumnString = m_ColumnString + '<td style = "border:0.1pt solid black;">' + myNode[myValueColumns[i]["field"]] + '</td>';
    }
    return m_ColumnString;
}
function GetTreeGridColumns(myTreeGridDomId) {
    var m_OldTreeDateObj = $('#' + myTreeGridDomId).treegrid("options")["columns"];
    var m_ValueCloumns = [];
    $.each(m_OldTreeDateObj[0], function (i, value) {
        //this;      //this指向当前元素
        //i;         //i表示Array当前下标
        //value;     //value表示Array当前元素
        if (value["hidden"] != true) {
            m_ValueCloumns.push({ "field": value["field"], "title": value["title"] });
        }
    });
    return m_ValueCloumns;
}
