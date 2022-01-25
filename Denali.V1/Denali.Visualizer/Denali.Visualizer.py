from re import X
import plotly.graph_objects as go
import json, os
from MovingAverages import get_smas, get_emas
from AlpacaDataLoader import get_alpaca_candle_data, AggregateData

def main():
    aggregateData = get_alpaca_candle_data()

    df = dict(Date=[], Open=[], Close=[], High=[], Low=[])
    for aggregate in aggregateData:
        df['Date'].append(aggregate.time)
        df['Open'].append(aggregate.open)
        df['Close'].append(aggregate.close)
        df['High'].append(aggregate.high)
        df['Low'].append(aggregate.low)

    fig = go.Figure(data=[
        go.Candlestick(
            x=df['Date'],
            open=df['Open'],
            high=df['High'],
            low=df['Low'],
            close=df['Close'])])

    '''
    fig.add_scatter(
        x=smas['Date'],
        y=smas['Value']
    )
    '''
    fig.write_html('Denali_Results.html', auto_open=True)

if __name__ == "__main__":
    main()




