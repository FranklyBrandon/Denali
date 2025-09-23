let chart;
let candleSeries;
let slowEMASeries;
let fastEMASeries;

let candleDataLabel;
let slowEMADataLabel;
let fastEMADataLabel;

document.addEventListener("DOMContentLoaded", () => {
    BuildChart();
    candleDataLabel = document.getElementById('price-value');
    slowEMADataLabel = document.getElementById('slow-value');
    fastEMADataLabel = document.getElementById('fast-value');
});

function OnSubmit() {
    const symbol = document.getElementById('symbolInput').value;
    const date = document.getElementById('dateInput').value;
    fetch(`https://localhost:7166/api/stockdata/${symbol}?date=${date}`)
        .then(resp => resp.json())
        .then(data => {
            SetData(symbol, data);
        });
}

function BuildChart() {
    // Main chart
    chart = LightweightCharts.createChart(document.getElementById('chartContainer'),
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

    // Candlestick series
    candleSeries = chart.addSeries(LightweightCharts.CandlestickSeries, 
    { 
        priceLineVisible: false, // historic chart
        upColor: '#26a69a', 
        downColor: '#ef5350', 
        borderVisible: false, 
        wickUpColor: '#26a69a', 
        wickDownColor: '#ef5350' 
    });

    // Slow EMA series
    slowEMASeries = chart.addSeries(LightweightCharts.LineSeries, 
    { 
        color: '#ff8c00',
        lineWidth: 1,
        priceLineVisible: false, // historic chart,
        crosshairMarkerVisible: false // remove dot on current data point
    });

    // Fast EMA series
    fastEMASeries = chart.addSeries(LightweightCharts.LineSeries, 
    { 
        color: '#6fff00',
        lineWidth: 1,
        priceLineVisible: false, // historic chart
        crosshairMarkerVisible: false // remove dot on current data point
    });

    chart.subscribeCrosshairMove(myCrosshairMoveHandler);
}

function SetData(symbol, data) {
    chart.priceScale('right').applyOptions({
        autoScale: true,
    });
    
    // Candle data
    let bars = data.stockData[symbol];
    let candles = []
    for (const bar of bars) {
        candles.push(
            { 
                time: getGraphTime(bar.timeUtc),
                open: bar.open,
                high: bar.high,
                low: bar.low,
                close: bar.close
            }
        )
    }
    candleSeries.setData(candles);

    // Slow EMA data
    let slowEmas = data.slowEmas[symbol];
    let slowEmaValues = []
    for (const slowEma of slowEmas) {
        slowEmaValues.push(
            {
               time: getGraphTime(slowEma.timeUtc), 
               value: slowEma.value
            }
        )
    }
    slowEMASeries.setData(slowEmaValues);

    // Fast EMA data
    let fastEmas = data.fastEmas[symbol];
    let fastEmaValues = []
    for (const fastEma of fastEmas) {
        fastEmaValues.push(
            {
                time: getGraphTime(fastEma.timeUtc), 
                value: fastEma.value
            }
        )
    }
    fastEMASeries.setData(fastEmaValues);

    // Signals
    let entrySignals = data.entrySignals;
    const markers = [];
    for (const signal of entrySignals) {
        console.log(signal.signalBar.timeUtc);
        console.log(signal.openingRangeBreakoutTime);
        console.log(signal.firstPullbackTime);
        console.log(signal.confirmationPullbackTime);

        markers.push(
            {
                time: getGraphTime(signal.signalBar.timeUtc),
                position: 'aboveBar',
                color: '#2cc900',
                shape: 'arrowUp',
                text: 'Entry',
            },
            {
                time: getGraphTime(signal.openingRangeBreakoutTime),
                position: 'aboveBar',
                color: '#343aeb',
                shape: 'arrowUp',
                text: 'ORB',
            },
            {
                time: getGraphTime(signal.firstPullbackTime),
                position: 'belowBar',
                color: '#eb4034',
                shape: 'arrowDown',
                text: '1st Pullback',
            },
            {
                time: getGraphTime(signal.confirmationPullbackTime),
                position: 'belowBar',
                color: '#eb4034',
                shape: 'arrowDown',
                text: '2nd Pullback',
            }
        )
    }

    LightweightCharts.createSeriesMarkers(candleSeries, markers)

    chart.timeScale().fitContent();
}

function myCrosshairMoveHandler(param) {
    if (!param.point) {
        return;
    }

    //const yPrice = candleSeries.coordinateToPrice(param.point.y);
    //console.log(`The cursor position in price is ${yPrice}.`);
    const candleData = param.seriesData.get(candleSeries);
    candleDataLabel.innerHTML = `Open: ${candleData.open}, High: ${candleData.high}, Low: ${candleData.low}, Close: ${candleData.close}`;

    const slowEmaData = param.seriesData.get(slowEMASeries);
    slowEMADataLabel.innerHTML = `Slow EMA: ${slowEmaData.value}`

    const fastEmaData = param.seriesData.get(fastEMASeries);
    fastEMADataLabel.innerHTML = `Fast EMA: ${fastEmaData.value}`

    console.log(`The price for the datapoint is ${dataPoint.close}.`);
}

function getGraphTime(date) {
    return Math.floor(new Date(date).getTime() / 1000)
}