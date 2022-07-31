document.addEventListener('DOMContentLoaded', function () {
    var chart = buildChart();
    addSpreadData(chart);
});

function buildChart() {
    return LightweightCharts.createChart(document.getElementById('chartContainer'), {
        width: window.innerWidth - 50,
        height: window.innerHeight - 50,
        layout: {
            backgroundColor: '#000000',
            textColor: 'rgba(255, 255, 255, 0.9)',
        },
        grid: {
            vertLines: {
                color: 'rgba(197, 203, 206, 0.5)',
            },
            horzLines: {
                color: 'rgba(197, 203, 206, 0.5)',
            },
        },
        crosshair: {
            mode: LightweightCharts.CrosshairMode.Normal,
        },
        rightPriceScale: {
            borderColor: 'rgba(197, 203, 206, 0.8)',
        },
        timeScale: {
            borderColor: 'rgba(197, 203, 206, 0.8)',
            timeVisible: true
        },
    });
}

function addSpreadData(chart) {
    var lineSeries = chart.addLineSeries();

    let series = []
    for (const point of spreadData) {
        series.push(
            { 
                time: (Date.parse(point.Time) / 1000),
                value: point.Value
            }
        )
    }
    lineSeries.setData(series);
}