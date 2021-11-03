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

    smas = get_smas(candleData, 9)

    for value in candleData:
        df['Date'].append(value['t'])
        df['Open'].append(value['o'])
        df['Close'].append(value['c'])
        df['High'].append(value['h'])
        df['Low'].append(value['l'])

    fig = go.Figure(data=[
        go.Candlestick(
            x=df['Date'],
            open=df['Open'],
            high=df['High'],
            low=df['Low'],
            close=df['Close'])])

    fig.add_scatter(
        x=smas['Date'],
        y=smas['Value']
    )

    fig.write_html('Denali_Results.html', auto_open=True)


def get_smas(candleData, emaBacklook):

    smas = dict(Value=[], Date=[])
    for index, value in enumerate(candleData):
        if (index < emaBacklook - 1):
            continue

        sum = 0
        for x in range(index - (emaBacklook), index):
            sum += candleData[x]['c']

        smas['Value'].append(sum / emaBacklook)
        smas['Date'].append(candleData[x]['t'])

    return smas

if __name__ == "__main__":
    main()




