import plotly.graph_objects as go
import sys, os

print('Hello World!')

df = dict(Date=[1,2], Open=[1,2], Close=[1,2], High=[1,2], Low=[1,2])
dir = "C:\\Denali\\Denali.Worker\\bin\\Debug\\net5.0"
path = os.path.join(dir, "filename")


fig = go.Figure(data=[
    go.Candlestick(
        x=df['Date'],
        open=df['Open'],
        high=df['High'],
        low=df['Low'],
        close=df['Close'])])

fig.write_html('Denali_Results.html', auto_open=True)