document.addEventListener('DOMContentLoaded', function () {
    const chartOptions = { width: window.innerWidth - 50, height: window.innerHeight - 50,layout: { textColor: 'black', background: { type: 'solid', color: 'white' } } };
    const chart = LightweightCharts.createChart(document.getElementById('chartContainer'), chartOptions);
    const histogramSeries = chart.addHistogramSeries({ color: '#26a69a' });
    
    histogramSeries.setData(dataChartData);
    chart.timeScale().fitContent();
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