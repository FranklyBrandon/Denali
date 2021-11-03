from re import X
import plotly.graph_objects as go
import json, os

dir = "C:\\Denali\\Denali.Visualizer"
fileName = "11_1_2021_AAPL.txt"
path = os.path.join(dir, fileName)
file = open(path, "r")

candleData = json.load(file)
df = dict(Date=[], Open=[], Close=[], High=[], Low=[])
ema = dict(X=[], Y=[])

for index, value in enumerate(candleData['bars']):
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
