document.addEventListener("DOMContentLoaded", () => {
    BuildChart();
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

    const lineSeriesOne = chart.addSeries(LightweightCharts.LineSeries, { color: '#2962FF' });
    lineSeriesOne.setData(getData());
}


function getGraphTime(date) {
    return Math.floor(new Date(date).getTime() / 1000)
}

function getData() {
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

let data = [
  {
    "Day": "2024-10-29T13:30:00Z",
    "CapitalTraded": 24763.04,
    "TotalCommision": 5.95,
    "GrossProfit": -53.22,
    "RunningCapital": 24946.78
  },
  {
    "Day": "2024-10-30T13:30:00Z",
    "CapitalTraded": 24612.005,
    "TotalCommision": 6.2848,
    "GrossProfit": 31.6818,
    "RunningCapital": 24978.46
  },
  {
    "Day": "2024-10-31T13:30:00Z",
    "CapitalTraded": 24944.581,
    "TotalCommision": 13.8096,
    "GrossProfit": 417.5304,
    "RunningCapital": 25395.99
  },
  {
    "Day": "2024-11-01T13:30:00Z",
    "CapitalTraded": 25289.866,
    "TotalCommision": 16.67872,
    "GrossProfit": 257.32928,
    "RunningCapital": 25653.32
  },
  {
    "Day": "2024-11-04T14:30:00Z",
    "CapitalTraded": 25747.975,
    "TotalCommision": 8.75,
    "GrossProfit": -132.74,
    "RunningCapital": 25520.58
  },
  {
    "Day": "2024-11-05T14:30:00Z",
    "CapitalTraded": 25147.06,
    "TotalCommision": 5.6,
    "GrossProfit": 1019.06,
    "RunningCapital": 26539.64
  },
  {
    "Day": "2024-11-06T14:30:00Z",
    "CapitalTraded": 26901.0476,
    "TotalCommision": 10.5,
    "GrossProfit": -166.594,
    "RunningCapital": 26373.05
  },
  {
    "Day": "2024-11-07T14:30:00Z",
    "CapitalTraded": 26207.895,
    "TotalCommision": 31.44395,
    "GrossProfit": 612.13105,
    "RunningCapital": 26985.18
  },
  {
    "Day": "2024-11-08T14:30:00Z",
    "CapitalTraded": 26806.678,
    "TotalCommision": 16.4366,
    "GrossProfit": -407.6676,
    "RunningCapital": 26577.51
  },
  {
    "Day": "2024-11-11T14:30:00Z",
    "CapitalTraded": 26624.57,
    "TotalCommision": 11.8734,
    "GrossProfit": -392.0734,
    "RunningCapital": 26185.44
  },
  {
    "Day": "2024-11-12T14:30:00Z",
    "CapitalTraded": 32333.89,
    "TotalCommision": 9.1,
    "GrossProfit": -1997.102,
    "RunningCapital": 24188.34
  },
  {
    "Day": "2024-11-13T14:30:00Z",
    "CapitalTraded": 24788.768,
    "TotalCommision": 5.95,
    "GrossProfit": -613.188,
    "RunningCapital": 23575.15
  },
  {
    "Day": "2024-11-14T14:30:00Z",
    "CapitalTraded": 23419.92,
    "TotalCommision": 9.3139,
    "GrossProfit": 486.9675,
    "RunningCapital": 24062.12
  },
  {
    "Day": "2024-11-15T14:30:00Z",
    "CapitalTraded": 23961.95,
    "TotalCommision": 5.1485,
    "GrossProfit": 743.5065,
    "RunningCapital": 24805.63
  },
  {
    "Day": "2024-11-18T14:30:00Z",
    "CapitalTraded": 24894.125,
    "TotalCommision": 6.86,
    "GrossProfit": -1105.6996,
    "RunningCapital": 23699.93
  },
  {
    "Day": "2024-11-19T14:30:00Z",
    "CapitalTraded": 24656.71,
    "TotalCommision": 6.86,
    "GrossProfit": 3008.3715,
    "RunningCapital": 26708.3
  },
  {
    "Day": "2024-11-20T14:30:00Z",
    "CapitalTraded": 26728.025,
    "TotalCommision": 2.8,
    "GrossProfit": 456.11,
    "RunningCapital": 27164.41
  },
  {
    "Day": "2024-11-21T14:30:00Z",
    "CapitalTraded": 26793.7479,
    "TotalCommision": 4.55,
    "GrossProfit": -573.0751,
    "RunningCapital": 26591.33
  },
  {
    "Day": "2024-11-22T14:30:00Z",
    "CapitalTraded": 26861.69,
    "TotalCommision": 3.15,
    "GrossProfit": 533.03,
    "RunningCapital": 27124.36
  },
  {
    "Day": "2024-11-25T14:30:00Z",
    "CapitalTraded": 27679.85,
    "TotalCommision": 3.031,
    "GrossProfit": -499.431,
    "RunningCapital": 26624.93
  },
  {
    "Day": "2024-11-26T14:30:00Z",
    "CapitalTraded": 26424.124,
    "TotalCommision": 4.55,
    "GrossProfit": -410.84,
    "RunningCapital": 26214.09
  },
  {
    "Day": "2024-11-27T14:30:00Z",
    "CapitalTraded": 26782.8682,
    "TotalCommision": 5.524659,
    "GrossProfit": -405.706959,
    "RunningCapital": 25808.38
  },
  {
    "Day": "2024-11-29T14:30:00Z",
    "CapitalTraded": 25639.6432,
    "TotalCommision": 8.043,
    "GrossProfit": -287.5566,
    "RunningCapital": 25520.82
  },
  {
    "Day": "2024-12-02T14:30:00Z",
    "CapitalTraded": 26086.32,
    "TotalCommision": 6.9965,
    "GrossProfit": 610.2335,
    "RunningCapital": 26131.05
  },
  {
    "Day": "2024-12-03T14:30:00Z",
    "CapitalTraded": 25432.0402,
    "TotalCommision": 8.442622,
    "GrossProfit": 1613.998578,
    "RunningCapital": 27745.05
  },
  {
    "Day": "2024-12-04T14:30:00Z",
    "CapitalTraded": 27523.84,
    "TotalCommision": 5.768,
    "GrossProfit": 366.092,
    "RunningCapital": 28111.14
  },
  {
    "Day": "2024-12-05T14:30:00Z",
    "CapitalTraded": 27201.2172,
    "TotalCommision": 4.635092,
    "GrossProfit": 8.936008,
    "RunningCapital": 28120.08
  },
  {
    "Day": "2024-12-06T14:30:00Z",
    "CapitalTraded": 28300.21,
    "TotalCommision": 3.3775,
    "GrossProfit": 776.5525,
    "RunningCapital": 28896.63
  },
  {
    "Day": "2024-12-09T14:30:00Z",
    "CapitalTraded": 26629.4,
    "TotalCommision": 4.9,
    "GrossProfit": -6.16,
    "RunningCapital": 28890.47
  },
  {
    "Day": "2024-12-10T14:30:00Z",
    "CapitalTraded": 27932.993,
    "TotalCommision": 15.4,
    "GrossProfit": 205.143,
    "RunningCapital": 29095.61
  },
  {
    "Day": "2024-12-11T14:30:00Z",
    "CapitalTraded": 29184.55,
    "TotalCommision": 6.5124,
    "GrossProfit": -343.1724,
    "RunningCapital": 28752.44
  },
  {
    "Day": "2024-12-12T14:30:00Z",
    "CapitalTraded": 28986.7639,
    "TotalCommision": 9.772,
    "GrossProfit": 628.4592,
    "RunningCapital": 29380.9
  },
  {
    "Day": "2024-12-13T14:30:00Z",
    "CapitalTraded": 29764.397,
    "TotalCommision": 4.095,
    "GrossProfit": -301.171,
    "RunningCapital": 29079.73
  },
  {
    "Day": "2024-12-16T14:30:00Z",
    "CapitalTraded": 29342.025,
    "TotalCommision": 3.5,
    "GrossProfit": 530.21,
    "RunningCapital": 29609.94
  },
  {
    "Day": "2024-12-17T14:30:00Z",
    "CapitalTraded": 41041.33,
    "TotalCommision": 5.782,
    "GrossProfit": 12235.128,
    "RunningCapital": 41845.07
  },
  {
    "Day": "2024-12-18T14:30:00Z",
    "CapitalTraded": 43239.38,
    "TotalCommision": 9.436,
    "GrossProfit": -362.006,
    "RunningCapital": 41483.06
  },
  {
    "Day": "2024-12-19T14:30:00Z",
    "CapitalTraded": 45475.75,
    "TotalCommision": 11.4415,
    "GrossProfit": 8684.5395,
    "RunningCapital": 50167.6
  },
  {
    "Day": "2024-12-20T14:30:00Z",
    "CapitalTraded": 51465.58,
    "TotalCommision": 5.208,
    "GrossProfit": 594.132,
    "RunningCapital": 50761.73
  },
  {
    "Day": "2024-12-23T14:30:00Z",
    "CapitalTraded": 50955.725,
    "TotalCommision": 16.842,
    "GrossProfit": 2713.762,
    "RunningCapital": 53475.49
  },
  {
    "Day": "2024-12-24T14:30:00Z",
    "CapitalTraded": 52380.02,
    "TotalCommision": 8.3475,
    "GrossProfit": -4425.0175,
    "RunningCapital": 49050.47
  },
  {
    "Day": "2024-12-26T14:30:00Z",
    "CapitalTraded": 42198.36,
    "TotalCommision": 15.6065,
    "GrossProfit": -3175.2765,
    "RunningCapital": 45875.19
  },
  {
    "Day": "2024-12-27T14:30:00Z",
    "CapitalTraded": 48426.9452,
    "TotalCommision": 4.701,
    "GrossProfit": 4183.3686,
    "RunningCapital": 50058.56
  },
  {
    "Day": "2024-12-30T14:30:00Z",
    "CapitalTraded": 49578.936,
    "TotalCommision": 21.3605,
    "GrossProfit": -2690.2705,
    "RunningCapital": 47368.29
  },
  {
    "Day": "2024-12-31T14:30:00Z",
    "CapitalTraded": 40206.42,
    "TotalCommision": 18.655,
    "GrossProfit": -555.935,
    "RunningCapital": 46812.36
  },
  {
    "Day": "2025-01-02T14:30:00Z",
    "CapitalTraded": 46883.772,
    "TotalCommision": 7.119,
    "GrossProfit": 631.185,
    "RunningCapital": 47443.55
  },
  {
    "Day": "2025-01-03T14:30:00Z",
    "CapitalTraded": 47800.889,
    "TotalCommision": 21.315,
    "GrossProfit": 716.5083,
    "RunningCapital": 48160.06
  },
  {
    "Day": "2025-01-06T14:30:00Z",
    "CapitalTraded": 57530.96,
    "TotalCommision": 7.294,
    "GrossProfit": -2804.904,
    "RunningCapital": 45355.16
  },
  {
    "Day": "2025-01-07T14:30:00Z",
    "CapitalTraded": 45630.732,
    "TotalCommision": 8.5488,
    "GrossProfit": 5668.7203,
    "RunningCapital": 51023.88
  },
  {
    "Day": "2025-01-08T14:30:00Z",
    "CapitalTraded": 48698.07,
    "TotalCommision": 16.499,
    "GrossProfit": 2750.951,
    "RunningCapital": 53774.83
  },
  {
    "Day": "2025-01-10T14:30:00Z",
    "CapitalTraded": 52839.258,
    "TotalCommision": 10.451,
    "GrossProfit": 661.957,
    "RunningCapital": 54436.79
  },
  {
    "Day": "2025-01-13T14:30:00Z",
    "CapitalTraded": 53332.386,
    "TotalCommision": 4.9,
    "GrossProfit": 953.88,
    "RunningCapital": 55390.67
  },
  {
    "Day": "2025-01-14T14:30:00Z",
    "CapitalTraded": 55298.718,
    "TotalCommision": 11.725,
    "GrossProfit": 680.745,
    "RunningCapital": 56071.42
  },
  {
    "Day": "2025-01-15T14:30:00Z",
    "CapitalTraded": 55869.2694,
    "TotalCommision": 5.145,
    "GrossProfit": -443.105,
    "RunningCapital": 55628.32
  },
  {
    "Day": "2025-01-16T14:30:00Z",
    "CapitalTraded": 55481.94,
    "TotalCommision": 6.1845,
    "GrossProfit": -153.4245,
    "RunningCapital": 55474.9
  },
  {
    "Day": "2025-01-17T14:30:00Z",
    "CapitalTraded": 56202.764,
    "TotalCommision": 37.2715,
    "GrossProfit": -1556.6535,
    "RunningCapital": 53918.25
  },
  {
    "Day": "2025-01-21T14:30:00Z",
    "CapitalTraded": 55252.43,
    "TotalCommision": 9.562,
    "GrossProfit": 443.246,
    "RunningCapital": 54361.5
  },
  {
    "Day": "2025-01-22T14:30:00Z",
    "CapitalTraded": 53965.804,
    "TotalCommision": 10.5525,
    "GrossProfit": -368.9825,
    "RunningCapital": 53992.52
  },
  {
    "Day": "2025-01-23T14:30:00Z",
    "CapitalTraded": 51894.315,
    "TotalCommision": 5.852,
    "GrossProfit": 862.523,
    "RunningCapital": 54855.04
  },
  {
    "Day": "2025-01-24T14:30:00Z",
    "CapitalTraded": 54835.66,
    "TotalCommision": 3.892,
    "GrossProfit": -540.392,
    "RunningCapital": 54314.65
  },
  {
    "Day": "2025-01-27T14:30:00Z",
    "CapitalTraded": 53205.28,
    "TotalCommision": 5.9535,
    "GrossProfit": -1794.0235,
    "RunningCapital": 52520.63
  },
  {
    "Day": "2025-01-28T14:30:00Z",
    "CapitalTraded": 51389.632,
    "TotalCommision": 8.407,
    "GrossProfit": -1176.053,
    "RunningCapital": 51344.58
  },
  {
    "Day": "2025-01-29T14:30:00Z",
    "CapitalTraded": 49569.38,
    "TotalCommision": 3.15,
    "GrossProfit": -3815.33,
    "RunningCapital": 47529.25
  },
  {
    "Day": "2025-01-30T14:30:00Z",
    "CapitalTraded": 48614.979,
    "TotalCommision": 5.6,
    "GrossProfit": 507.483,
    "RunningCapital": 48036.73
  },
  {
    "Day": "2025-01-31T14:30:00Z",
    "CapitalTraded": 47278.591,
    "TotalCommision": 5.415,
    "GrossProfit": 490.835,
    "RunningCapital": 48527.57
  },
  {
    "Day": "2025-02-03T14:30:00Z",
    "CapitalTraded": 48549.9332,
    "TotalCommision": 3.556,
    "GrossProfit": -1151.2466,
    "RunningCapital": 47376.32
  },
  {
    "Day": "2025-02-04T14:30:00Z",
    "CapitalTraded": 46527.4996,
    "TotalCommision": 1.75,
    "GrossProfit": -121.49,
    "RunningCapital": 47254.83
  },
  {
    "Day": "2025-02-05T14:30:00Z",
    "CapitalTraded": 46728.5225,
    "TotalCommision": 5.25,
    "GrossProfit": -302.2175,
    "RunningCapital": 46952.61
  },
  {
    "Day": "2025-02-06T14:30:00Z",
    "CapitalTraded": 46850.329,
    "TotalCommision": 8.1935,
    "GrossProfit": 334.2175,
    "RunningCapital": 47286.83
  },
  {
    "Day": "2025-02-07T14:30:00Z",
    "CapitalTraded": 46650.11,
    "TotalCommision": 5.6,
    "GrossProfit": 318.5085,
    "RunningCapital": 47605.34
  },
  {
    "Day": "2025-02-10T14:30:00Z",
    "CapitalTraded": 47639.35,
    "TotalCommision": 6.3,
    "GrossProfit": -1254.865,
    "RunningCapital": 46350.48
  },
  {
    "Day": "2025-02-11T14:30:00Z",
    "CapitalTraded": 46459.761,
    "TotalCommision": 5.95,
    "GrossProfit": -437.923,
    "RunningCapital": 45912.56
  },
  {
    "Day": "2025-02-12T14:30:00Z",
    "CapitalTraded": 45381.71,
    "TotalCommision": 4.55,
    "GrossProfit": -731.57,
    "RunningCapital": 45180.99
  },
  {
    "Day": "2025-02-13T14:30:00Z",
    "CapitalTraded": 45349.6596,
    "TotalCommision": 5.6,
    "GrossProfit": -1293.3104,
    "RunningCapital": 43887.68
  },
  {
    "Day": "2025-02-14T14:30:00Z",
    "CapitalTraded": 43524.765,
    "TotalCommision": 7.35,
    "GrossProfit": 126.915,
    "RunningCapital": 44014.6
  },
  {
    "Day": "2025-02-18T14:30:00Z",
    "CapitalTraded": 44211.6,
    "TotalCommision": 6.65,
    "GrossProfit": -40.535,
    "RunningCapital": 43974.07
  },
  {
    "Day": "2025-02-19T14:30:00Z",
    "CapitalTraded": 42948.3637,
    "TotalCommision": 7.7,
    "GrossProfit": 249.2494,
    "RunningCapital": 44223.32
  },
  {
    "Day": "2025-02-20T14:30:00Z",
    "CapitalTraded": 43929.08,
    "TotalCommision": 4.9,
    "GrossProfit": 454.3,
    "RunningCapital": 44677.62
  },
  {
    "Day": "2025-02-21T14:30:00Z",
    "CapitalTraded": 43766.242,
    "TotalCommision": 7.35,
    "GrossProfit": 1980.1964,
    "RunningCapital": 46657.82
  },
  {
    "Day": "2025-02-24T14:30:00Z",
    "CapitalTraded": 46602.638,
    "TotalCommision": 4.9,
    "GrossProfit": 525.26,
    "RunningCapital": 47183.08
  },
  {
    "Day": "2025-02-25T14:30:00Z",
    "CapitalTraded": 47078.309,
    "TotalCommision": 8.9005,
    "GrossProfit": 865.5389,
    "RunningCapital": 48048.62
  },
  {
    "Day": "2025-02-26T14:30:00Z",
    "CapitalTraded": 48094.5587,
    "TotalCommision": 8.4,
    "GrossProfit": 65.8,
    "RunningCapital": 48114.42
  },
  {
    "Day": "2025-02-27T14:30:00Z",
    "CapitalTraded": 48259.23,
    "TotalCommision": 3.15,
    "GrossProfit": 1708.36,
    "RunningCapital": 49822.78
  },
  {
    "Day": "2025-02-28T14:30:00Z",
    "CapitalTraded": 49629.673,
    "TotalCommision": 7,
    "GrossProfit": -335.045,
    "RunningCapital": 49487.74
  },
  {
    "Day": "2025-03-03T14:30:00Z",
    "CapitalTraded": 49259.665,
    "TotalCommision": 7,
    "GrossProfit": -127.715,
    "RunningCapital": 49360.03
  },
  {
    "Day": "2025-03-04T14:30:00Z",
    "CapitalTraded": 47040.22,
    "TotalCommision": 3.5,
    "GrossProfit": 450.4,
    "RunningCapital": 49810.43
  },
  {
    "Day": "2025-03-05T14:30:00Z",
    "CapitalTraded": 49519.085,
    "TotalCommision": 7.861,
    "GrossProfit": -710.431,
    "RunningCapital": 49100
  },
  {
    "Day": "2025-03-06T14:30:00Z",
    "CapitalTraded": 49041.84,
    "TotalCommision": 6.083,
    "GrossProfit": -507.383,
    "RunningCapital": 48592.62
  },
  {
    "Day": "2025-03-07T14:30:00Z",
    "CapitalTraded": 48381.785,
    "TotalCommision": 6.419,
    "GrossProfit": -383.919,
    "RunningCapital": 48208.7
  },
  {
    "Day": "2025-03-10T13:30:00Z",
    "CapitalTraded": 48078.05,
    "TotalCommision": 7.938,
    "GrossProfit": 1459.432,
    "RunningCapital": 49668.13
  },
  {
    "Day": "2025-03-11T13:30:00Z",
    "CapitalTraded": 50747.37,
    "TotalCommision": 8.1305,
    "GrossProfit": -1249.8905,
    "RunningCapital": 48418.24
  },
  {
    "Day": "2025-03-12T13:30:00Z",
    "CapitalTraded": 47647.6,
    "TotalCommision": 15.708,
    "GrossProfit": 3208.172,
    "RunningCapital": 51626.41
  },
  {
    "Day": "2025-03-13T13:30:00Z",
    "CapitalTraded": 52530.415,
    "TotalCommision": 19.873,
    "GrossProfit": -741.3055,
    "RunningCapital": 50885.1
  },
  {
    "Day": "2025-03-14T13:30:00Z",
    "CapitalTraded": 52699.72,
    "TotalCommision": 20.839,
    "GrossProfit": 4210.401,
    "RunningCapital": 55095.5
  },
  {
    "Day": "2025-03-17T13:30:00Z",
    "CapitalTraded": 53830.566,
    "TotalCommision": 3.15,
    "GrossProfit": -1184.25,
    "RunningCapital": 53911.25
  },
  {
    "Day": "2025-03-18T13:30:00Z",
    "CapitalTraded": 52025.98,
    "TotalCommision": 18.494,
    "GrossProfit": -719.7042,
    "RunningCapital": 53191.55
  },
  {
    "Day": "2025-03-19T13:30:00Z",
    "CapitalTraded": 50715.98,
    "TotalCommision": 3.402,
    "GrossProfit": -1605.692,
    "RunningCapital": 51585.86
  },
  {
    "Day": "2025-03-20T13:30:00Z",
    "CapitalTraded": 49140.3365,
    "TotalCommision": 18.536,
    "GrossProfit": 871.938,
    "RunningCapital": 52457.8
  },
  {
    "Day": "2025-03-21T13:30:00Z",
    "CapitalTraded": 52166.397,
    "TotalCommision": 22.9565,
    "GrossProfit": 3550.9735,
    "RunningCapital": 56008.77
  },
  {
    "Day": "2025-03-24T13:30:00Z",
    "CapitalTraded": 42253.0222,
    "TotalCommision": 16.9085,
    "GrossProfit": 2030.2059,
    "RunningCapital": 58038.98
  },
  {
    "Day": "2025-03-25T13:30:00Z",
    "CapitalTraded": 58212.195,
    "TotalCommision": 25.3295,
    "GrossProfit": 1136.6655,
    "RunningCapital": 59175.65
  },
  {
    "Day": "2025-03-26T13:30:00Z",
    "CapitalTraded": 60162.4463,
    "TotalCommision": 34.16,
    "GrossProfit": 850.585,
    "RunningCapital": 60026.24
  },
  {
    "Day": "2025-03-27T13:30:00Z",
    "CapitalTraded": 59543.457,
    "TotalCommision": 5.1485,
    "GrossProfit": -1326.5096,
    "RunningCapital": 58699.73
  },
  {
    "Day": "2025-03-28T13:30:00Z",
    "CapitalTraded": 58728.22,
    "TotalCommision": 7.1925,
    "GrossProfit": 694.0975,
    "RunningCapital": 59393.83
  },
  {
    "Day": "2025-03-31T13:30:00Z",
    "CapitalTraded": 57592.5,
    "TotalCommision": 4.76,
    "GrossProfit": -329.84,
    "RunningCapital": 59063.99
  },
  {
    "Day": "2025-04-01T13:30:00Z",
    "CapitalTraded": 59221.45,
    "TotalCommision": 6.6815,
    "GrossProfit": 6792.4385,
    "RunningCapital": 65856.43
  },
  {
    "Day": "2025-04-02T13:30:00Z",
    "CapitalTraded": 52306.63,
    "TotalCommision": 5.978,
    "GrossProfit": -213.286,
    "RunningCapital": 65643.14
  },
  {
    "Day": "2025-04-03T13:30:00Z",
    "CapitalTraded": 62536.4648,
    "TotalCommision": 37.0125,
    "GrossProfit": -1333.4925,
    "RunningCapital": 64309.65
  },
  {
    "Day": "2025-04-04T13:30:00Z",
    "CapitalTraded": 65051.967,
    "TotalCommision": 12.257,
    "GrossProfit": -261.4104,
    "RunningCapital": 64048.24
  },
  {
    "Day": "2025-04-07T13:30:00Z",
    "CapitalTraded": 56642.6985,
    "TotalCommision": 133.2485,
    "GrossProfit": 4098.9215,
    "RunningCapital": 68147.16
  },
  {
    "Day": "2025-04-08T13:30:00Z",
    "CapitalTraded": 75063.745,
    "TotalCommision": 82.8485,
    "GrossProfit": 13636.3815,
    "RunningCapital": 81783.54
  },
  {
    "Day": "2025-04-09T13:30:00Z",
    "CapitalTraded": 76336.979,
    "TotalCommision": 61.9535,
    "GrossProfit": -6140.5245,
    "RunningCapital": 75643.02
  },
  {
    "Day": "2025-04-10T13:30:00Z",
    "CapitalTraded": 49470.18,
    "TotalCommision": 69.839,
    "GrossProfit": -676.639,
    "RunningCapital": 74966.38
  },
  {
    "Day": "2025-04-11T13:30:00Z",
    "CapitalTraded": 12197.28,
    "TotalCommision": 4.1825,
    "GrossProfit": -4.1825,
    "RunningCapital": 74962.2
  },
  {
    "Day": "2025-04-14T13:30:00Z",
    "CapitalTraded": 69891.37,
    "TotalCommision": 151.025,
    "GrossProfit": 5788.9795,
    "RunningCapital": 80751.18
  },
  {
    "Day": "2025-04-15T13:30:00Z",
    "CapitalTraded": 69034.766,
    "TotalCommision": 15.9075,
    "GrossProfit": -6874.6585,
    "RunningCapital": 73876.52
  },
  {
    "Day": "2025-04-16T13:30:00Z",
    "CapitalTraded": 72501.905,
    "TotalCommision": 46.655,
    "GrossProfit": 10311.97,
    "RunningCapital": 84188.49
  },
  {
    "Day": "2025-04-17T13:30:00Z",
    "CapitalTraded": 97733.29,
    "TotalCommision": 77.693,
    "GrossProfit": -2387.6097,
    "RunningCapital": 81800.88
  },
  {
    "Day": "2025-04-21T13:30:00Z",
    "CapitalTraded": 73172.465,
    "TotalCommision": 34.5135,
    "GrossProfit": -4793.8135,
    "RunningCapital": 77007.07
  },
  {
    "Day": "2025-04-22T13:30:00Z",
    "CapitalTraded": 69032.74,
    "TotalCommision": 68.992,
    "GrossProfit": -3722.492,
    "RunningCapital": 73284.58
  },
  {
    "Day": "2025-04-23T13:30:00Z",
    "CapitalTraded": 73740.0095,
    "TotalCommision": 26.355,
    "GrossProfit": 82.6105,
    "RunningCapital": 73367.19
  },
  {
    "Day": "2025-04-24T13:30:00Z",
    "CapitalTraded": 73344.87,
    "TotalCommision": 18.662,
    "GrossProfit": -232.1087,
    "RunningCapital": 73135.08
  },
  {
    "Day": "2025-04-25T13:30:00Z",
    "CapitalTraded": 73736.2298,
    "TotalCommision": 10.1045,
    "GrossProfit": -176.5845,
    "RunningCapital": 72958.5
  },
  {
    "Day": "2025-04-28T13:30:00Z",
    "CapitalTraded": 73006.354,
    "TotalCommision": 9.0825,
    "GrossProfit": -59.1645,
    "RunningCapital": 72899.34
  },
  {
    "Day": "2025-04-29T13:30:00Z",
    "CapitalTraded": 73150.849,
    "TotalCommision": 34.5275,
    "GrossProfit": -195.1251,
    "RunningCapital": 72704.21
  },
  {
    "Day": "2025-04-30T13:30:00Z",
    "CapitalTraded": 70726.56,
    "TotalCommision": 15.0465,
    "GrossProfit": 976.8035,
    "RunningCapital": 73681.01
  },
  {
    "Day": "2025-05-01T13:30:00Z",
    "CapitalTraded": 74256.03,
    "TotalCommision": 14.2415,
    "GrossProfit": 3284.8795,
    "RunningCapital": 76965.89
  },
  {
    "Day": "2025-05-02T13:30:00Z",
    "CapitalTraded": 78255.199,
    "TotalCommision": 7.1855,
    "GrossProfit": -1333.1055,
    "RunningCapital": 75632.78
  },
  {
    "Day": "2025-05-05T13:30:00Z",
    "CapitalTraded": 77823.3352,
    "TotalCommision": 7,
    "GrossProfit": -3046.385,
    "RunningCapital": 72586.4
  },
  {
    "Day": "2025-05-06T13:30:00Z",
    "CapitalTraded": 38210.234,
    "TotalCommision": 8.7955,
    "GrossProfit": -709.867,
    "RunningCapital": 71876.53
  },
  {
    "Day": "2025-05-07T13:30:00Z",
    "CapitalTraded": 72293.0955,
    "TotalCommision": 20.587,
    "GrossProfit": -1037.651,
    "RunningCapital": 70838.88
  },
  {
    "Day": "2025-05-08T13:30:00Z",
    "CapitalTraded": 71050.802,
    "TotalCommision": 10.85,
    "GrossProfit": 891.064,
    "RunningCapital": 71729.94
  },
  {
    "Day": "2025-05-09T13:30:00Z",
    "CapitalTraded": 71916.132,
    "TotalCommision": 23.8,
    "GrossProfit": -614.986,
    "RunningCapital": 71114.95
  },
  {
    "Day": "2025-05-12T13:30:00Z",
    "CapitalTraded": 72282.374,
    "TotalCommision": 13.65,
    "GrossProfit": 168.733,
    "RunningCapital": 71283.68
  },
  {
    "Day": "2025-05-13T13:30:00Z",
    "CapitalTraded": 71586.612,
    "TotalCommision": 16.025,
    "GrossProfit": 282.9136,
    "RunningCapital": 71566.59
  },
  {
    "Day": "2025-05-14T13:30:00Z",
    "CapitalTraded": 71464.1505,
    "TotalCommision": 9.747555,
    "GrossProfit": -910.778555,
    "RunningCapital": 70655.81
  },
  {
    "Day": "2025-05-15T13:30:00Z",
    "CapitalTraded": 71259.8805,
    "TotalCommision": 13.2265,
    "GrossProfit": -3775.5765,
    "RunningCapital": 66880.23
  },
  {
    "Day": "2025-05-16T13:30:00Z",
    "CapitalTraded": 66740.9527,
    "TotalCommision": 5.476327,
    "GrossProfit": -39.983627,
    "RunningCapital": 66840.25
  },
  {
    "Day": "2025-05-19T13:30:00Z",
    "CapitalTraded": 65606.64,
    "TotalCommision": 14.1365,
    "GrossProfit": -129.6465,
    "RunningCapital": 66710.6
  },
  {
    "Day": "2025-05-20T13:30:00Z",
    "CapitalTraded": 64777.25,
    "TotalCommision": 11.6445,
    "GrossProfit": 1389.9455,
    "RunningCapital": 68100.55
  },
  {
    "Day": "2025-05-21T13:30:00Z",
    "CapitalTraded": 66199.0438,
    "TotalCommision": 13.3875,
    "GrossProfit": 2974.6085,
    "RunningCapital": 71075.16
  },
  {
    "Day": "2025-05-22T13:30:00Z",
    "CapitalTraded": 70963.6942,
    "TotalCommision": 7.063,
    "GrossProfit": 790.563,
    "RunningCapital": 71865.72
  },
  {
    "Day": "2025-05-23T13:30:00Z",
    "CapitalTraded": 70522.025,
    "TotalCommision": 6.125,
    "GrossProfit": -616.1088,
    "RunningCapital": 71249.61
  },
  {
    "Day": "2025-05-27T13:30:00Z",
    "CapitalTraded": 77513.8296,
    "TotalCommision": 8.8865,
    "GrossProfit": -141.8265,
    "RunningCapital": 71107.78
  },
  {
    "Day": "2025-05-28T13:30:00Z",
    "CapitalTraded": 69289.32,
    "TotalCommision": 18.3085,
    "GrossProfit": -248.1035,
    "RunningCapital": 70859.68
  },
  {
    "Day": "2025-05-29T13:30:00Z",
    "CapitalTraded": 69320.4522,
    "TotalCommision": 25.0285,
    "GrossProfit": 3426.8616,
    "RunningCapital": 74286.54
  },
  {
    "Day": "2025-05-30T13:30:00Z",
    "CapitalTraded": 73745.245,
    "TotalCommision": 5.8835,
    "GrossProfit": -809.0585,
    "RunningCapital": 73477.48
  },
  {
    "Day": "2025-06-02T13:30:00Z",
    "CapitalTraded": 72941.178,
    "TotalCommision": 4.9,
    "GrossProfit": -1752.925,
    "RunningCapital": 71724.56
  },
  {
    "Day": "2025-06-03T13:30:00Z",
    "CapitalTraded": 73481.532,
    "TotalCommision": 13.022068,
    "GrossProfit": 125.603132,
    "RunningCapital": 71850.16
  },
  {
    "Day": "2025-06-04T13:30:00Z",
    "CapitalTraded": 72489.615,
    "TotalCommision": 6.9965,
    "GrossProfit": -114.6065,
    "RunningCapital": 71735.55
  },
  {
    "Day": "2025-06-05T13:30:00Z",
    "CapitalTraded": 72429.3106,
    "TotalCommision": 8.7955,
    "GrossProfit": -371.6495,
    "RunningCapital": 71363.9
  },
  {
    "Day": "2025-06-06T13:30:00Z",
    "CapitalTraded": 69446.7052,
    "TotalCommision": 13.951,
    "GrossProfit": -2017.8736,
    "RunningCapital": 69346.03
  },
  {
    "Day": "2025-06-09T13:30:00Z",
    "CapitalTraded": 69675.205,
    "TotalCommision": 22.5295,
    "GrossProfit": -869.7995,
    "RunningCapital": 68476.23
  },
  {
    "Day": "2025-06-10T13:30:00Z",
    "CapitalTraded": 68951.855,
    "TotalCommision": 21.2345,
    "GrossProfit": 749.7655,
    "RunningCapital": 69226
  },
  {
    "Day": "2025-06-11T13:30:00Z",
    "CapitalTraded": 71740.959,
    "TotalCommision": 14.819,
    "GrossProfit": 397.351,
    "RunningCapital": 69623.35
  },
  {
    "Day": "2025-06-12T13:30:00Z",
    "CapitalTraded": 66011.34,
    "TotalCommision": 40.558,
    "GrossProfit": -2073.951,
    "RunningCapital": 67549.4
  },
  {
    "Day": "2025-06-13T13:30:00Z",
    "CapitalTraded": 69001.8766,
    "TotalCommision": 9.471,
    "GrossProfit": -1668.5691,
    "RunningCapital": 65880.83
  },
  {
    "Day": "2025-06-16T13:30:00Z",
    "CapitalTraded": 62250.367,
    "TotalCommision": 34.2615,
    "GrossProfit": 3236.1055,
    "RunningCapital": 69116.94
  },
  {
    "Day": "2025-06-17T13:30:00Z",
    "CapitalTraded": 67344.974,
    "TotalCommision": 15.162,
    "GrossProfit": 668.818,
    "RunningCapital": 69785.76
  },
  {
    "Day": "2025-06-18T13:30:00Z",
    "CapitalTraded": 69429.375,
    "TotalCommision": 5.95,
    "GrossProfit": 385.94,
    "RunningCapital": 70171.7
  },
  {
    "Day": "2025-06-20T13:30:00Z",
    "CapitalTraded": 70586.8056,
    "TotalCommision": 21.077,
    "GrossProfit": 778.311,
    "RunningCapital": 70950.01
  },
  {
    "Day": "2025-06-23T13:30:00Z",
    "CapitalTraded": 70650.27,
    "TotalCommision": 8.6275,
    "GrossProfit": -552.3225,
    "RunningCapital": 70397.69
  },
  {
    "Day": "2025-06-24T13:30:00Z",
    "CapitalTraded": 72276.14,
    "TotalCommision": 11.116,
    "GrossProfit": 756.164,
    "RunningCapital": 71153.85
  },
  {
    "Day": "2025-06-25T13:30:00Z",
    "CapitalTraded": 71338.8808,
    "TotalCommision": 8.729,
    "GrossProfit": -1599.983,
    "RunningCapital": 69553.87
  },
  {
    "Day": "2025-06-26T13:30:00Z",
    "CapitalTraded": 72682.92,
    "TotalCommision": 8.442,
    "GrossProfit": -3484.472,
    "RunningCapital": 66069.4
  },
  {
    "Day": "2025-06-27T13:30:00Z",
    "CapitalTraded": 65696.623,
    "TotalCommision": 16.555,
    "GrossProfit": -929.7021,
    "RunningCapital": 65139.7
  },
  {
    "Day": "2025-06-30T13:30:00Z",
    "CapitalTraded": 63568.041,
    "TotalCommision": 22.547,
    "GrossProfit": 1106.685,
    "RunningCapital": 66246.39
  },
  {
    "Day": "2025-07-01T13:30:00Z",
    "CapitalTraded": 73843.87,
    "TotalCommision": 10.0695,
    "GrossProfit": -2502.6745,
    "RunningCapital": 63743.72
  },
  {
    "Day": "2025-07-02T13:30:00Z",
    "CapitalTraded": 63308.3615,
    "TotalCommision": 6.547515,
    "GrossProfit": -252.974015,
    "RunningCapital": 63490.75
  },
  {
    "Day": "2025-07-03T13:30:00Z",
    "CapitalTraded": 63542.26,
    "TotalCommision": 14.6405,
    "GrossProfit": 192.1495,
    "RunningCapital": 63682.9
  },
  {
    "Day": "2025-07-07T13:30:00Z",
    "CapitalTraded": 63230.798,
    "TotalCommision": 6.419,
    "GrossProfit": 1058.569,
    "RunningCapital": 64741.47
  },
  {
    "Day": "2025-07-08T13:30:00Z",
    "CapitalTraded": 68572.1779,
    "TotalCommision": 44.716,
    "GrossProfit": 1035.0914,
    "RunningCapital": 65776.56
  },
  {
    "Day": "2025-07-09T13:30:00Z",
    "CapitalTraded": 67692.7505,
    "TotalCommision": 33.439,
    "GrossProfit": 476.2238,
    "RunningCapital": 66252.78
  },
  {
    "Day": "2025-07-10T13:30:00Z",
    "CapitalTraded": 65101.218,
    "TotalCommision": 10.02568,
    "GrossProfit": 997.31632,
    "RunningCapital": 67250.1
  },
  {
    "Day": "2025-07-11T13:30:00Z",
    "CapitalTraded": 66446.9991,
    "TotalCommision": 13.1075,
    "GrossProfit": -105.4295,
    "RunningCapital": 67144.67
  },
  {
    "Day": "2025-07-14T13:30:00Z",
    "CapitalTraded": 63044.55,
    "TotalCommision": 15.1305,
    "GrossProfit": -1310.5655,
    "RunningCapital": 65834.1
  },
  {
    "Day": "2025-07-15T13:30:00Z",
    "CapitalTraded": 64395.1094,
    "TotalCommision": 9.0265,
    "GrossProfit": 1393.9746,
    "RunningCapital": 67228.07
  },
  {
    "Day": "2025-07-16T13:30:00Z",
    "CapitalTraded": 66527.3805,
    "TotalCommision": 13.7725,
    "GrossProfit": 1517.7025,
    "RunningCapital": 68745.77
  },
  {
    "Day": "2025-07-17T13:30:00Z",
    "CapitalTraded": 68005.52,
    "TotalCommision": 11.137,
    "GrossProfit": -151.587,
    "RunningCapital": 68594.18
  },
  {
    "Day": "2025-07-18T13:30:00Z",
    "CapitalTraded": 68123.7435,
    "TotalCommision": 8.05,
    "GrossProfit": 24.5767,
    "RunningCapital": 68618.76
  },
  {
    "Day": "2025-07-21T13:30:00Z",
    "CapitalTraded": 68293.2363,
    "TotalCommision": 7.5145,
    "GrossProfit": 3142.7555,
    "RunningCapital": 71761.52
  },
  {
    "Day": "2025-07-22T13:30:00Z",
    "CapitalTraded": 71614.0979,
    "TotalCommision": 21.9205,
    "GrossProfit": 1391.7548,
    "RunningCapital": 73153.27
  },
  {
    "Day": "2025-07-23T13:30:00Z",
    "CapitalTraded": 73476.861,
    "TotalCommision": 8.961043,
    "GrossProfit": 2001.271657,
    "RunningCapital": 75154.54
  },
  {
    "Day": "2025-07-24T13:30:00Z",
    "CapitalTraded": 75239.4571,
    "TotalCommision": 12.95,
    "GrossProfit": 704.1954,
    "RunningCapital": 75858.74
  },
  {
    "Day": "2025-07-25T13:30:00Z",
    "CapitalTraded": 75812.519,
    "TotalCommision": 8.670894,
    "GrossProfit": 234.337706,
    "RunningCapital": 76093.08
  },
  {
    "Day": "2025-07-28T13:30:00Z",
    "CapitalTraded": 76197.45,
    "TotalCommision": 5.25,
    "GrossProfit": 161.45,
    "RunningCapital": 76254.53
  },
  {
    "Day": "2025-07-29T13:30:00Z",
    "CapitalTraded": 73478.67,
    "TotalCommision": 23.2925,
    "GrossProfit": 7330.8375,
    "RunningCapital": 83585.37
  },
  {
    "Day": "2025-07-30T13:30:00Z",
    "CapitalTraded": 83723.37,
    "TotalCommision": 8.4,
    "GrossProfit": 128.8654,
    "RunningCapital": 83714.24
  },
  {
    "Day": "2025-07-31T13:30:00Z",
    "CapitalTraded": 83600.493,
    "TotalCommision": 8.05,
    "GrossProfit": -772.0919,
    "RunningCapital": 82942.15
  },
  {
    "Day": "2025-08-01T13:30:00Z",
    "CapitalTraded": 81729.68,
    "TotalCommision": 11.869,
    "GrossProfit": 62.571,
    "RunningCapital": 83004.72
  },
  {
    "Day": "2025-08-04T13:30:00Z",
    "CapitalTraded": 83167.36,
    "TotalCommision": 16.9525,
    "GrossProfit": -653.2309,
    "RunningCapital": 82351.49
  },
  {
    "Day": "2025-08-05T13:30:00Z",
    "CapitalTraded": 80928.4044,
    "TotalCommision": 20.293,
    "GrossProfit": -74.9936,
    "RunningCapital": 82276.5
  },
  {
    "Day": "2025-08-06T13:30:00Z",
    "CapitalTraded": 82591.7035,
    "TotalCommision": 13.3,
    "GrossProfit": -76.8265,
    "RunningCapital": 82199.67
  },
  {
    "Day": "2025-08-07T13:30:00Z",
    "CapitalTraded": 82379.1906,
    "TotalCommision": 17.792896,
    "GrossProfit": 1341.259804,
    "RunningCapital": 83540.93
  },
  {
    "Day": "2025-08-08T13:30:00Z",
    "CapitalTraded": 83766.5687,
    "TotalCommision": 17.366,
    "GrossProfit": -1015.392,
    "RunningCapital": 82525.54
  },
  {
    "Day": "2025-08-11T13:30:00Z",
    "CapitalTraded": 82815.909,
    "TotalCommision": 14.7,
    "GrossProfit": -1268.5222,
    "RunningCapital": 81257.02
  },
  {
    "Day": "2025-08-12T13:30:00Z",
    "CapitalTraded": 80322.316,
    "TotalCommision": 33.11,
    "GrossProfit": -914.096,
    "RunningCapital": 80342.92
  },
  {
    "Day": "2025-08-13T13:30:00Z",
    "CapitalTraded": 86373.1279,
    "TotalCommision": 11.5535,
    "GrossProfit": -2651.9217,
    "RunningCapital": 77691
  },
  {
    "Day": "2025-08-14T13:30:00Z",
    "CapitalTraded": 70179.27,
    "TotalCommision": 40.544,
    "GrossProfit": -1977.194,
    "RunningCapital": 75713.81
  },
  {
    "Day": "2025-08-15T13:30:00Z",
    "CapitalTraded": 76256.5572,
    "TotalCommision": 25.0425,
    "GrossProfit": -1887.2297,
    "RunningCapital": 73826.58
  },
  {
    "Day": "2025-08-18T13:30:00Z",
    "CapitalTraded": 76010.04,
    "TotalCommision": 8.2705,
    "GrossProfit": 758.2675,
    "RunningCapital": 74584.85
  },
  {
    "Day": "2025-08-19T13:30:00Z",
    "CapitalTraded": 73211.9938,
    "TotalCommision": 26.313,
    "GrossProfit": 1421.4745,
    "RunningCapital": 76006.32
  },
  {
    "Day": "2025-08-20T13:30:00Z",
    "CapitalTraded": 75937.49,
    "TotalCommision": 8.9285,
    "GrossProfit": 1278.7615,
    "RunningCapital": 77285.08
  },
  {
    "Day": "2025-08-21T13:30:00Z",
    "CapitalTraded": 77497.98,
    "TotalCommision": 22.239,
    "GrossProfit": 2626.037,
    "RunningCapital": 79911.12
  },
  {
    "Day": "2025-08-22T13:30:00Z",
    "CapitalTraded": 75576.038,
    "TotalCommision": 8.2635,
    "GrossProfit": 4260.0105,
    "RunningCapital": 84171.13
  },
  {
    "Day": "2025-08-25T13:30:00Z",
    "CapitalTraded": 83933.0495,
    "TotalCommision": 10.248,
    "GrossProfit": 1203.8073,
    "RunningCapital": 85374.94
  },
  {
    "Day": "2025-08-26T13:30:00Z",
    "CapitalTraded": 87834.0669,
    "TotalCommision": 40.4075,
    "GrossProfit": -47.7875,
    "RunningCapital": 85327.15
  },
  {
    "Day": "2025-08-27T13:30:00Z",
    "CapitalTraded": 85760.3,
    "TotalCommision": 5.9885,
    "GrossProfit": 2237.3504,
    "RunningCapital": 87564.5
  },
  {
    "Day": "2025-08-28T13:30:00Z",
    "CapitalTraded": 88155.3502,
    "TotalCommision": 7.7525,
    "GrossProfit": -2200.5143,
    "RunningCapital": 85363.99
  },
  {
    "Day": "2025-08-29T13:30:00Z",
    "CapitalTraded": 84466.9904,
    "TotalCommision": 5.985,
    "GrossProfit": 636.7,
    "RunningCapital": 86000.69
  },
  {
    "Day": "2025-09-02T13:30:00Z",
    "CapitalTraded": 85466.0587,
    "TotalCommision": 13.307,
    "GrossProfit": -2698.7236,
    "RunningCapital": 83301.97
  },
  {
    "Day": "2025-09-03T13:30:00Z",
    "CapitalTraded": 83169.13,
    "TotalCommision": 16.583,
    "GrossProfit": -2651.413,
    "RunningCapital": 80650.56
  },
  {
    "Day": "2025-09-04T13:30:00Z",
    "CapitalTraded": 80265.0437,
    "TotalCommision": 6.5135,
    "GrossProfit": -740.3228,
    "RunningCapital": 79910.24
  },
  {
    "Day": "2025-09-05T13:30:00Z",
    "CapitalTraded": 80352.9574,
    "TotalCommision": 14.319614,
    "GrossProfit": -1041.559414,
    "RunningCapital": 78868.68
  },
  {
    "Day": "2025-09-08T13:30:00Z",
    "CapitalTraded": 79475.67,
    "TotalCommision": 13.0305,
    "GrossProfit": -683.3505,
    "RunningCapital": 78185.33
  },
  {
    "Day": "2025-09-09T13:30:00Z",
    "CapitalTraded": 76886.6239,
    "TotalCommision": 13.8495,
    "GrossProfit": 981.4826,
    "RunningCapital": 79166.81
  },
  {
    "Day": "2025-09-10T13:30:00Z",
    "CapitalTraded": 78139.0081,
    "TotalCommision": 10.07455,
    "GrossProfit": -174.69775,
    "RunningCapital": 78992.11
  },
  {
    "Day": "2025-09-11T13:30:00Z",
    "CapitalTraded": 79333.4,
    "TotalCommision": 7,
    "GrossProfit": 3810.83,
    "RunningCapital": 82802.94
  },
  {
    "Day": "2025-09-12T13:30:00Z",
    "CapitalTraded": 83355.085,
    "TotalCommision": 18.683,
    "GrossProfit": 1891.2509,
    "RunningCapital": 84694.19
  },
  {
    "Day": "2025-09-15T13:30:00Z",
    "CapitalTraded": 87355.9012,
    "TotalCommision": 14.784,
    "GrossProfit": -2461.9285,
    "RunningCapital": 82232.26
  },
  {
    "Day": "2025-09-16T13:30:00Z",
    "CapitalTraded": 78006.326,
    "TotalCommision": 36.4595,
    "GrossProfit": 3050.3775,
    "RunningCapital": 85282.64
  },
  {
    "Day": "2025-09-17T13:30:00Z",
    "CapitalTraded": 81352.345,
    "TotalCommision": 32.249,
    "GrossProfit": -4482.509,
    "RunningCapital": 80800.13
  },
  {
    "Day": "2025-09-18T13:30:00Z",
    "CapitalTraded": 79772.7462,
    "TotalCommision": 23.492,
    "GrossProfit": 66.9437,
    "RunningCapital": 80867.07
  },
  {
    "Day": "2025-09-19T13:30:00Z",
    "CapitalTraded": 80557.006,
    "TotalCommision": 15.008,
    "GrossProfit": 1572.603,
    "RunningCapital": 82439.67
  },
  {
    "Day": "2025-09-22T13:30:00Z",
    "CapitalTraded": 81730.2314,
    "TotalCommision": 15.777564,
    "GrossProfit": -2053.077664,
    "RunningCapital": 80386.59
  },
  {
    "Day": "2025-09-23T13:30:00Z",
    "CapitalTraded": 80929.6555,
    "TotalCommision": 22.253,
    "GrossProfit": 327.6898,
    "RunningCapital": 80714.28
  },
  {
    "Day": "2025-09-24T13:30:00Z",
    "CapitalTraded": 79787.0797,
    "TotalCommision": 19.4215,
    "GrossProfit": 1536.1472,
    "RunningCapital": 82250.43
  },
  {
    "Day": "2025-09-25T13:30:00Z",
    "CapitalTraded": 83344.5279,
    "TotalCommision": 22.6065,
    "GrossProfit": -1172.9365,
    "RunningCapital": 81077.49
  },
  {
    "Day": "2025-09-26T13:30:00Z",
    "CapitalTraded": 81707.2452,
    "TotalCommision": 24.2585,
    "GrossProfit": 2610.5667,
    "RunningCapital": 83688.06
  },
  {
    "Day": "2025-09-29T13:30:00Z",
    "CapitalTraded": 87117.4124,
    "TotalCommision": 8.022,
    "GrossProfit": 1119.418,
    "RunningCapital": 84807.48
  },
  {
    "Day": "2025-09-30T13:30:00Z",
    "CapitalTraded": 85096.2052,
    "TotalCommision": 18.3995,
    "GrossProfit": 1692.9587,
    "RunningCapital": 86500.44
  },
  {
    "Day": "2025-10-01T13:30:00Z",
    "CapitalTraded": 85676.984,
    "TotalCommision": 10.6715,
    "GrossProfit": -2240.7315,
    "RunningCapital": 84259.71
  },
  {
    "Day": "2025-10-02T13:30:00Z",
    "CapitalTraded": 84005.5073,
    "TotalCommision": 12.586,
    "GrossProfit": 436.2601,
    "RunningCapital": 84695.97
  },
  {
    "Day": "2025-10-03T13:30:00Z",
    "CapitalTraded": 79891.78,
    "TotalCommision": 25.6095,
    "GrossProfit": -2148.1807,
    "RunningCapital": 82547.79
  },
  {
    "Day": "2025-10-06T13:30:00Z",
    "CapitalTraded": 84727.5044,
    "TotalCommision": 71.893956,
    "GrossProfit": 800.217944,
    "RunningCapital": 83348.01
  },
  {
    "Day": "2025-10-07T13:30:00Z",
    "CapitalTraded": 90673.0367,
    "TotalCommision": 11.529,
    "GrossProfit": -1325.1402,
    "RunningCapital": 82022.87
  },
  {
    "Day": "2025-10-08T13:30:00Z",
    "CapitalTraded": 79613.0008,
    "TotalCommision": 63.413,
    "GrossProfit": 626.5686,
    "RunningCapital": 82649.44
  },
  {
    "Day": "2025-10-09T13:30:00Z",
    "CapitalTraded": 81383.8141,
    "TotalCommision": 10.395,
    "GrossProfit": 1250.1315,
    "RunningCapital": 83899.57
  },
  {
    "Day": "2025-10-10T13:30:00Z",
    "CapitalTraded": 81402.94,
    "TotalCommision": 37.226,
    "GrossProfit": 807.0689,
    "RunningCapital": 84706.64
  },
  {
    "Day": "2025-10-13T13:30:00Z",
    "CapitalTraded": 81693.3952,
    "TotalCommision": 19.4145,
    "GrossProfit": 2468.9107,
    "RunningCapital": 87175.55
  },
  {
    "Day": "2025-10-14T13:30:00Z",
    "CapitalTraded": 86815.6102,
    "TotalCommision": 26.7365,
    "GrossProfit": 1531.9082,
    "RunningCapital": 88707.46
  },
  {
    "Day": "2025-10-15T13:30:00Z",
    "CapitalTraded": 84278.1764,
    "TotalCommision": 22.9495,
    "GrossProfit": 4818.2633,
    "RunningCapital": 93525.72
  },
  {
    "Day": "2025-10-16T13:30:00Z",
    "CapitalTraded": 92537.0342,
    "TotalCommision": 23.639,
    "GrossProfit": 555.3938,
    "RunningCapital": 94081.11
  },
  {
    "Day": "2025-10-17T13:30:00Z",
    "CapitalTraded": 97146.23,
    "TotalCommision": 9.2575,
    "GrossProfit": -3018.0475,
    "RunningCapital": 91063.06
  },
  {
    "Day": "2025-10-20T13:30:00Z",
    "CapitalTraded": 88874.376,
    "TotalCommision": 21.2415,
    "GrossProfit": 1449.2055,
    "RunningCapital": 92512.27
  },
  {
    "Day": "2025-10-21T13:30:00Z",
    "CapitalTraded": 94953.39,
    "TotalCommision": 30.086,
    "GrossProfit": 10643.704,
    "RunningCapital": 103155.97
  },
  {
    "Day": "2025-10-22T13:30:00Z",
    "CapitalTraded": 103425.7567,
    "TotalCommision": 17.4895,
    "GrossProfit": -1031.9731,
    "RunningCapital": 102124
  },
  {
    "Day": "2025-10-23T13:30:00Z",
    "CapitalTraded": 102106.7682,
    "TotalCommision": 8.092,
    "GrossProfit": -1543.937,
    "RunningCapital": 100580.06
  },
  {
    "Day": "2025-10-24T13:30:00Z",
    "CapitalTraded": 100619.4449,
    "TotalCommision": 6.727,
    "GrossProfit": -327.1831,
    "RunningCapital": 100252.88
  },
  {
    "Day": "2025-10-27T13:30:00Z",
    "CapitalTraded": 98570.315,
    "TotalCommision": 21.1505,
    "GrossProfit": -1577.1874,
    "RunningCapital": 98675.69
  },
  {
    "Day": "2025-10-28T13:30:00Z",
    "CapitalTraded": 97169.02,
    "TotalCommision": 19.621,
    "GrossProfit": -731.731,
    "RunningCapital": 97943.96
  },
  {
    "Day": "2025-10-29T13:30:00Z",
    "CapitalTraded": 97076.065,
    "TotalCommision": 7.1015,
    "GrossProfit": 1394.9735,
    "RunningCapital": 99338.93
  }
]

