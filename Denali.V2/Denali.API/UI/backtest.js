let lineSeriesOne;
let baselineLineSeries;

document.addEventListener("DOMContentLoaded", () => {
    BuildChart();

    document.getElementById('backTestData').addEventListener('change', event => {
      const file = event.target.files[0];
      const reader = new FileReader();

      reader.onload = e => {
        const data = JSON.parse(e.target.result);
        lineSeriesOne.setData(getData(data));
      };

      reader.readAsText(file);
    });

    document.getElementById('baselineData').addEventListener('change', event => {
      const file = event.target.files[0];
      const reader = new FileReader();

      reader.onload = e => {
        const data = JSON.parse(e.target.result);
        baselineLineSeries.setData(getData(data));
      };

      reader.readAsText(file);
    });
});



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

    lineSeriesOne = chart.addSeries(LightweightCharts.LineSeries, { color: '#2962FF' });
    baselineLineSeries= chart.addSeries(LightweightCharts.LineSeries, { color: '#960012' });
}


function getGraphTime(date) {
    return Math.floor(new Date(date).getTime() / 1000)
}

function getData(data) {
    let points = []
    for (const datum of data) {
        points.push(
            { 
                time: getGraphTime(datum.Day),
                value: datum.RunningCapital
            }
        )
    }
    return points;
}