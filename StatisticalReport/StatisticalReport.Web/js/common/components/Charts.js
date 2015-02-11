function CreatChart(myChartObjId, myData, myChartType, myTitle) {
    var m_ChartObj;
    if (myChartType == 'Line') {
        m_ChartObj = GetLineChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'Bar') {
        m_ChartObj = GetBarChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'MultiBar') {
        m_ChartObj = GetMultiBarChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'Pie') {
        m_ChartObj = GetPieChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'meterGauge') {
        m_ChartObj = GetMeterGauge(myChartObjId, myData, myTitle);
    }
    return m_ChartObj;
}

function GetLineChart(myChartObjId, myData, myTitle, temp) {
    if (myData['columns'][1]['field'].split('-').length > 1) {
        GetDateXLineChart(myChartObjId, myData, myTitle);
        return;
    }

    //var line1 = [6.5, 9.2, 14, 19.65, 26.4, 35, 51];
    //var line2 = [3.5, 3.2, 12, 13.65, 41.4, 21, 51]
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_MaxBarValue = 0;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {
        var m_AxisItem = [i, myData['columns'][i]['title']];
        m_AxisX.push(m_AxisItem);
    }
    var m_Lines = new Array();
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LineTemp = new Array();
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ColumnName = myData['columns'][j]['field'];
            for (var m_Name in m_Rows[i]) {
                if (m_ColumnName == m_Name) {
                    m_LineTemp.push(parseFloat(m_Rows[i][m_Name]));
                }
            }
        }
        m_Lines.push(m_LineTemp);
    }
    //////////////////////////////找到最大的bar累加和////////////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        for (var j = 1; j < myData['columns'].length; j++) {
            var m_CurrentValue = parseFloat(m_Rows[i][myData['columns'][j].field]);
            if (m_CurrentValue > m_MaxBarValue) {
                m_MaxBarValue = m_CurrentValue;
            }
        }
    }
    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);

    var ChartJqplot = $.jqplot(myChartObjId, m_Lines, {
        animate: true,
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        animateReplot: true,
        seriesDefaults: {
            lineWidth: 1,
            markerOptions: { size: 0 }
        },
        axesDefaults: {
            tickRenderer: $.jqplot.CanvasAxisTickRenderer
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Comic Sans MS", cursive',
            fontSize: '11pt',
            textColor: '#C7AA4E'
        },
        legend: {
            show: true,
            fontSize: '3pt',
            show: true,
            location: 'e',
            placement: 'outside'
        },
        axes: {
            xaxis: {
                //renderer: $.jqplot.CategoryAxisRenderer,
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    angle: -30
                },
                //ticks: [['1', '一月份'], ['2', '二月份'], ['3', '三月份'], ['4', '四月份'], ['5', '五月份'], ['6', '六月份'], ['7', '七月份']],
                ticks: m_AxisX,
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                renderer: $.jqplot.LogAxisRenderer,

                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                label: myData['Units']['UnitY']
            }
        },
        cursor: {
            show: true,
            zoom: true
        }
    });
    return ChartJqplot;
}


function GetDateXLineChart(myChartObjId, myData, myTitle) {
    //var line1 = [6.5, 9.2, 14, 19.65, 26.4, 35, 51];
    //var line2 = [3.5, 3.2, 12, 13.65, 41.4, 21, 51]
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_MaxBarValue = 0;
    var formatString = "";
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {
        var m_AxisItem = [i, myData['columns'][i]['title']];
        m_AxisX.push(m_AxisItem);
    }
    var m_Lines = new Array();
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LineTemp = new Array();
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ColumnName = myData['columns'][j]['field'];
            for (var m_Name in m_Rows[i]) {
                if (m_ColumnName == m_Name) {
                    var timeArray = myData['columns'][j]['title'].split('-');
                    var date = null;
                    if (timeArray.length == 4) {
                        date = new Date(timeArray[0], timeArray[1], timeArray[2], timeArray[3]);
                        formatString = "%Y-%m-%d-%H";
                    }
                    else if (timeArray.length == 3) {
                        date = new Date(timeArray[0], timeArray[1], timeArray[2]);
                        formatString = "%Y-%m-%d";
                    }
                    else if (timeArray.length == 2 && timeArray[0].length == 4) {
                        date = new Date(timeArray[0], timeArray[1]);
                        formatString = "%Y-%m";
                    }
                    else if (timeArray.length == 2 && timeArray[0].length == 2) {
                        date = new Date((new Date()).getFullYear(), timeArray[0], timeArray[1]);
                        formatString = "%m-%d";
                    }

                    m_LineTemp.push([date.toUTCString(), parseFloat(m_Rows[i][m_Name])]);
                }
            }
        }
        m_Lines.push(m_LineTemp);
    }
    //////////////////////////////找到最大的bar累加和////////////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        for (var j = 1; j < myData['columns'].length; j++) {
            var m_CurrentValue = parseFloat(m_Rows[i][myData['columns'][j].field]);
            if (m_CurrentValue > m_MaxBarValue) {
                m_MaxBarValue = m_CurrentValue;
            }
        }
    }
    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);

    var ChartJqplot = $.jqplot(myChartObjId, m_Lines, {
        animate: true,
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        animateReplot: true,
        seriesDefaults: {
            lineWidth: 1,
            markerOptions: { size: 0 }
        },
        axesDefaults: {
            tickRenderer: $.jqplot.CanvasAxisTickRenderer
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Comic Sans MS", cursive',
            fontSize: '11pt',
            textColor: '#C7AA4E'
        },
        legend: {
            show: true,
            fontSize: '3pt',
            show: true,
            location: 'e',
            placement: 'outside'
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.DateAxisRenderer,
                tickRenderer: $.jqplot.CanvasAxisTickRenderer ,
                tickOptions: {
                    formatString: formatString,
                    angle: -30,
                    fontSize: '10pt'
                },
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                renderer: $.jqplot.LogAxisRenderer,

                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                label: myData['Units']['UnitY']
            }
        },
        cursor: {
            show: true,
            zoom: true
        }
    });

    $('#' + myChartObjId).parent().bind('_resize', function (event, ui) {
        ChartJqplot.replot({ resetAxes: true });
    });


    return ChartJqplot;
}

function GetBarChart(myChartObjId, myData, myTitle) {
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_Bars = new Array();
    var m_MaxBarValue = 0;  //;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {

        m_AxisX.push(myData['columns'][i]['title']);
    }

    if (m_Rows.length > 0) {
        for (var i = 0; i < m_Rows.length; i++) {
            var m_BarTemp = new Array();
            for (var j = 1; j < myData['columns'].length; j++) {
                m_BarTemp.push(parseFloat(m_Rows[i][myData['columns'][j].field]));
            }
            m_Bars.push(m_BarTemp);
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var i = 0; i < m_Bars.length; i++) {
            for (var j = 0; j < m_Bars[i].length; j++) {
                if (m_Bars[i][j] > m_MaxBarValue) {
                    m_MaxBarValue = m_Bars[i][j];
                }
            }
        }
    }

    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    var BarJqplot = $.jqplot(myChartObjId, m_Bars, {
            animate: !$.jqplot.use_excanvas,
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                pointLabels: { show: true, location: 'e', edgeTolerance: -15 },
                shadowAngle: 135,
                rendererOptions: {
                    barDirection: 'vertical'
                }
            },
            title: {
                text: myTitle,
                fontFamily: '"Comic Sans MS", cursive',
                fontSize: '11pt',
                textColor: '#C7AA4E'
            },
            series: m_Labels,
            legend: {
                show: true,
                location: 'e',
                placement: 'outside'
            },
            axes: {        
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_AxisX,
                    label: myData['Units']['UnitX'],
                    labelOptions: {
                        fontFamily: 'Helvetica',
                        fontSize: '8pt'
                    },
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer
                },
                //yaxis: {
                //    renderer: $.jqplot.CategoryAxisRenderer
                // }
                yaxis: {
                    //renderer: $.jqplot.CategoryAxisRenderer,

                    //tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    //labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    //labelOptions: {
                    //    fontFamily: 'Helvetica',
                    //    fontSize: '8pt'
                    //},
                    tickInterval: m_MaxBarValue / 10,
                    min: 0,
                    max: m_MaxBarValue,
                    pad: 0,
                    label: myData['Units']['UnitY'],
                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    labelOptions: {
                        fontFamily: 'Helvetica',
                        fontSize: '8pt'
                    }
                }
            },
            cursor: {
                show: true,
                zoom: true
            }
        });
    
        $('#chart2b').bind('jqplotDataHighlight',
            function (ev, seriesIndex, pointIndex, data) {
                $('#info2b').html('series: ' + seriesIndex + ', point: ' + pointIndex + ', data: ' + data + ', pageX: ' + ev.pageX + ', pageY: ' + ev.pageY);
            }
        );
        $('#chart2b').bind('jqplotDataClick',
            function (ev, seriesIndex, pointIndex, data) {
                $('#info2c').html('series: ' + seriesIndex + ', point: ' + pointIndex + ', data: ' + data + ', pageX: ' + ev.pageX + ', pageY: ' + ev.pageY);
            }
        );

        $('#chart2b').bind('jqplotDataUnhighlight',
            function (ev) {
                $('#info2b').html('Nothing');
            }
        );
        
        return BarJqplot;
}
function GetMultiBarChart(myChartObjId, myData, myTitle) {
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_Bars = new Array();
    var m_MaxBarValue = 0;  //;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {

        m_AxisX.push(myData['columns'][i]['title']);
    }
    ///////////////////////////////填充数据////////////////////////////////
    if (m_Rows.length == 1) {
        for (var j = 1; j < myData['columns'].length; j++) {
            m_Bars.push(parseFloat(m_Rows[0][myData['columns'][j].field]));
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var j = 0; j < m_Bars.length; j++) {
            m_MaxBarValue = m_MaxBarValue + m_Bars[j];
        }
        m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    }
    else if (m_Rows.length > 1) {
        for (var i = 0; i < m_Rows.length; i++) {
            var m_BarTemp = new Array();
            for (var j = 1; j < myData['columns'].length; j++) {
                m_BarTemp.push(parseFloat(m_Rows[i][myData['columns'][j].field]));
            }
            m_Bars.push(m_BarTemp);
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var j = 0; j < m_Bars[0].length; j++) {
            var m_SumBarValue = 0;
            for (var i = 0; i < m_Bars.length; i++) {
                m_SumBarValue = m_SumBarValue + m_Bars[i][j];
            }
            if (m_SumBarValue > m_MaxBarValue) {
                m_MaxBarValue = m_SumBarValue;
            }
        }
        m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    }

    //var bb = JSON.stringify(m_Bars)
    var MultiBarJqplot = $.jqplot(myChartObjId, m_Bars, {
        // Tell the plot to stack the bars.
        stackSeries: true,
        captureRightClick: true,
        seriesDefaults:{
            renderer:$.jqplot.BarRenderer,
            rendererOptions: {
                // Put a 30 pixel margin between bars.
                barMargin: 15,
                // Highlight bars when mouse button pressed.
                // Disables default highlighting on mouse over.
                highlightMouseDown: true   
            },
            pointLabels: {show: true}
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                ticks: m_AxisX,
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                //renderer: $.jqplot.CategoryAxisRenderer,

                //tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                //labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                //labelOptions: {
                //    fontFamily: 'Helvetica',
                //    fontSize: '8pt'
                //},
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                // Don't pad out the bottom of the data range.  By default,
                // axes scaled as if data extended 10% above and below the
                // actual range to prevent data points right on grid boundaries.
                // Don't want to do that here.
                padMin: 0,
                label: myData['Units']['UnitY'],
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica',
                    fontSize: '8pt'
                }
            }
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Comic Sans MS", cursive',
            fontSize: '11pt',
            textColor: '#C7AA4E'
        },
        legend: {
            show: true,
            location: 'e',
            placement: 'outside'
        },
        cursor: {
            show: true,
            zoom: true
        }
    });
    // Bind a listener to the "jqplotDataClick" event.  Here, simply change
    // the text of the info3 element to show what series and ponit were
    // clicked along with the data for that point.
    //$('#chart3').bind('jqplotDataClick', 
    //  function (ev, seriesIndex, pointIndex, data) {
    //      $('#info3').html('series: '+seriesIndex+', point: '+pointIndex+', data: '+data);
    //  }
    //); 

    return MultiBarJqplot;
}
function GetPieChart(myChartObjId, myData, myTitle) {

    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_Pie = new Array();
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_TitleTemp = m_Rows[i]['RowName'];
        var m_ValueTemp = 0;
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ValueTemp = m_ValueTemp + parseFloat(m_Rows[i][myData['columns'][j].field]);
        }
        m_Labels.push(m_TitleTemp);
        m_Pie.push([m_TitleTemp, m_ValueTemp]);
    }
    var PieJqplot = jQuery.jqplot(myChartObjId, [m_Pie],
    {
        animate: !$.jqplot.use_excanvas,
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        animateReplot: true,
        seriesDefaults: {
            shadow: true,
            //color: '#FF0000',
            renderer: jQuery.jqplot.PieRenderer,
            rendererOptions: {
                startAngle: 180,
                showDataLabels: true,

                //diameter: undefined, // 设置饼的直径
                padding: 20,        // 饼距离其分类名称框或者图表边框的距离，变相该表饼的直径
                sliceMargin: 5,     // 饼的每个部分之间的距离
                fill: true,         // 设置饼的每部分被填充的状态
                shadow: true,       //为饼的每个部分的边框设置阴影，以突出其立体效果
                shadowOffset: 2,    //设置阴影区域偏移出饼的每部分边框的距离
                shadowDepth: 5,     // 设置阴影区域的深度
                shadowAlpha: 0.07   // 设置阴影区域的透明度
            }
        },

        title: {
            text: myTitle,
            fontFamily: '"Comic Sans MS", cursive',
            fontSize: '11pt',
            textColor: '#C7AA4E'
        },
        legend: { show: true, location: 'e', placement: 'outside', fontSize: '8pt', textColor:'#aaa'}
    });
    $('.jqplot-target').css('color', '#444');

    return PieJqplot;
}

// 获取仪表盘
function GetMeterGauge(myChartObjId, myData, myTitle) {
    
    var m_Rows = myData['rows'];    // 数据行
    var m_ActualValue = 0;
    var m_AlarmValue = 0;
    var m_MinimunVlaue = 0;
    var m_MaximunValue = 0;

    // 数据验证
    if(myData['rows'].length == 0)  // 无数据行
        return null;

    // 遍历数据表
    // 数据表为一个n行2列的表，列名为RowName是数据的类型，列名为Value是数据的值
    // 其中数据类型包括：实际值，报警值，表盘下限值（可选，为空时下限值为0）， 表盘上限值（可选，为空时，程序自动以报警值为表盘的70%的方式计算表盘上限）
    for (var i = 0; i < m_Rows.length; i++) {
        var m_ValueType = m_Rows[i]['RowName'];
        var m_Value = parseFloat(m_Rows[i]['value']);

        switch (m_ValueType) {
            case '表盘实际值':     // 当前的实际值
                m_ActualValue = m_Value;
                break;
            case '报警设定值':      // 设定的报警值
                m_AlarmValue = m_Value;
                break;
            case '表盘下限值':    // 表盘的下限值
                m_MinimunVlaue = m_Value;
                break;
            case '表盘上限值':    // 表盘的上限值
                m_MaximunValue = m_Value;
                break;
            default:                // 其他值忽略
                break;
        }
    }

    // 计算表盘的最大值
    if (m_MaximunValue == 0 && m_AlarmValue > 0) {  // 仅当表盘上限值为0，且设定报警值大于0时，才计算表盘的上限值
        m_MaximunValue = m_ActualValue / 0.7;
    }

    // 获取仪表盘
    var MeterGaugeJqplot = $.jqplot(myChartObjId, [[m_ActualValue]], {
        title: myTitle,
        seriesDefaults: {
            renderer: $.jqplot.MeterGaugeRenderer,
            rendererOptions: {
                min: m_MinimunVlaue,                            // 表盘的下限值
                max: m_MaximunValue,                            // 表盘的上限值
                ringColor: '#4B4B4B',                           // 表盘的颜色（包括指针）
                intervals: [m_AlarmValue, m_MaximunValue],      // 表盘的区间（现仅有正常区间和报警区间）
                intervalColors: ['#99FF66', '#FF0033'],         // 表盘的区间颜色（正常区间为绿色，报警区间为红色）
                label: myData['Units']['UnitX'],                // 表盘的单位（仅从UnitX中取）
                hubRadius: 10                                   // 表盘指针下方圆点的大小
            }
        }
    });

    return MeterGaugeJqplot;
}

function ReleasePlotChart(containerId, plot) {
    if (plot) {
        plot.destroy();

        var elementId = '#' + containerId;
        $(elementId).unbind(); // for iexplorer  
        $(elementId).empty();

        plot = null;
    }
}
function GetYaxisMax(myMaxValue) {
    var m_MaxValue = Math.abs(myMaxValue);
    if (m_MaxValue >= 1) {
        var m_MaxDigits = 1;
        while ((m_MaxValue / 10) > 1 && m_MaxDigits < 10000000000000000000000000000) {
            m_MaxValue = m_MaxValue / 10;
            m_MaxDigits = m_MaxDigits * 10;
        }
        return Math.ceil((myMaxValue / m_MaxDigits)) * m_MaxDigits;
    }
    else if (m_MaxValue > 0 && m_MaxValue < 1) {
        var m_MaxDigits = 1;
        while ((m_MaxValue * 10) < 1 && m_MaxDigits < 10000000000000000000000000000) {
            m_MaxValue = m_MaxValue * 10;
            m_MaxDigits = m_MaxDigits * 10;
        }
        return Math.ceil((myMaxValue * m_MaxDigits)) / m_MaxDigits;
    }
    else {
        return 1;
    }

}