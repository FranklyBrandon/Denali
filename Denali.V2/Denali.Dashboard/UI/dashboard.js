

function OnSubmit() {
    const symbol = document.getElementById('symbolInput').value;
    const date = document.getElementById('dateInput').value;
    fetch(`https://localhost:7166/api/stockdata/${symbol}?date=${date}`)
        .then(resp => resp.json())
        .then(data => {
            console.log(symbol, data);
            BuildChart(symbol, data);
        });
}

function BuildChart(symbol, data) {
    const chart = LightweightCharts.createChart(document.getElementById('chartContainer'),
    { 
        width: window.innerWidth - 100,
        height: window.innerHeight - 150,
        layout: {
            background: { color: '#222' },
            textColor: '#DDD',
        },
        grid: {
            vertLines: { color: '#444' },
            horzLines: { color: '#444' },
        },
        timeScale: {
            timeVisible: true, // show minutes
        },
        crosshair: {
            mode: LightweightCharts.CrosshairMode.Normal,
        }
    });
    
    const candlestickSeries = chart.addSeries(LightweightCharts.CandlestickSeries, 
    { 
        priceLineVisible: false, // historic chart
        upColor: '#26a69a', 
        downColor: '#ef5350', 
        borderVisible: false, 
        wickUpColor: '#26a69a', 
        wickDownColor: '#ef5350' 
    });

    let bars = data.stockData[symbol];
    let candles = []
    for (const bar of bars) {
        candles.push(
            { 
                time: Math.floor(new Date(bar.timeUtc).getTime() / 1000),
                open: bar.open,
                high: bar.high,
                low: bar.low,
                close: bar.close
            }
        )
    }
    candlestickSeries.setData(candles);

    const slowEmaSeries = chart.addSeries(LightweightCharts.LineSeries, 
    { 
        color: '#ff8c00',
        lineWidth: 1,
        priceLineVisible: false, // historic chart,
        crosshairMarkerVisible: false // remove dot on current data point
    });

    let slowEmas = data.slowEmas;
    let slowEmaValues = []
    for (const slowEma of slowEmas) {
        slowEmaValues.push(
            {
               time: Math.floor(new Date(slowEma.timeUtc).getTime() / 1000), 
               value: slowEma.value
            }
        )
    }
    slowEmaSeries.setData(slowEmaValues);

    const fastEmaSeries = chart.addSeries(LightweightCharts.LineSeries, 
    { 
        color: '#6fff00',
        lineWidth: 1,
        priceLineVisible: false, // historic chart
        crosshairMarkerVisible: false // remove dot on current data point
    });
    
    let fastEmas = data.fastEmas;
    let fastEmaValues = []
    for (const fastEma of fastEmas) {
        fastEmaValues.push(
            {
                time: Math.floor(new Date(fastEma.timeUtc).getTime() / 1000), 
                value: fastEma.value
            }
        )
    }
    fastEmaSeries.setData(fastEmaValues);

    chart.timeScale().fitContent();
}