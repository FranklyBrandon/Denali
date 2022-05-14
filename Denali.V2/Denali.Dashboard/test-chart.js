document.addEventListener('DOMContentLoaded', function () {
    var chart = buildChart();
    var candleSeries = buildCandleSeries(chart)
    setBarData(candleSeries)
    setElephants(candleSeries)
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

function buildCandleSeries(chart) {
    return chart.addCandlestickSeries({
        upColor: 'rgba(255, 144, 0, 1)',
        downColor: '#000',
        borderDownColor: 'rgba(255, 144, 0, 1)',
        borderUpColor: 'rgba(255, 144, 0, 1)',
        wickDownColor: 'rgba(255, 144, 0, 1)',
        wickUpColor: 'rgba(255, 144, 0, 1)',
      });
}

function setBarData(candleSeries) {       

    let bars = barData.bars
    let candles = []
    for (const bar of bars) {
        candles.push(
            { 
                time: (Date.parse(bar.t) / 1000),
                open: bar.o,
                high: bar.h,
                low: bar.l,
                close: bar.c
            }
        )
    }
    candleSeries.setData(candles)
}

function setElephants(candleSeries) {

    let elephants = []
    elephantData.forEach(elephant => {
        elephants.push(
            {
                time: (Date.parse(elephant) / 1000), 
                position: 'aboveBar', 
                color: '#f68410', 
                shape: 'circle', 
                text: 'E'
            }
        )
    });
    console.log(elephants)
    candleSeries.setMarkers(elephants)
}

