from re import X
import plotly.graph_objects as go
import json, os

def main():

    dir = "C:\\Denali\\Denali.Visualizer"
    fileName = "11_1_2021_AAPL.txt"
    path = os.path.join(dir, fileName)
    file = open(path, "r")

    rawData = json.load(file)
    candleData = list(rawData['bars'])
    df = dict(Date=[], Open=[], Close=[], High=[], Low=[])
    ema = dict(X=[], Y=[])

    get_emas(candleData, 9)

    for value in candleData:
        df['Date'].append(value['t'])
        df['Open'].append(value['o'])
        df['Close'].append(value['c'])
        df['High'].append(value['h'])
        df['Low'].append(value['l'])

        ema['X'].append(value['t'])
        ema['Y'].append(value['c'])

    fig = go.Figure(data=[
        go.Candlestick(
            x=df['Date'],
            open=df['Open'],
            high=df['High'],
            low=df['Low'],
            close=df['Close'])])

    fig.add_scatter(
        x=ema['X'],
        y=ema['Y']
    )

    fig.write_html('Denali_Results.html', auto_open=True)


def get_emas(candleData, emaBacklook):
    for index, value in enumerate(candleData):
        print(value)

    emas = []
    return emas

if __name__ == "__main__":
    main()




