import json, os
from typing import List

class AggregateData(object):
    def __init__(self, jsonDict):
        self.open = jsonDict['o']
        self.close = jsonDict['c']
        self.high = jsonDict['h']
        self.low = jsonDict['l']
        self.time = jsonDict['t']

def get_alpaca_candle_data() -> List[AggregateData]:
    
    dir = "C:\\Denali\\Denali.Visualizer"
    fileName = "11_1_2021_AAPL.txt"
    path = os.path.join(dir, fileName)
    file = open(path, "r")

    rawData = json.load(file)
    candleData = list(rawData['bars'])
    aggregateData = list()

    for candle in candleData:
        aggregateData.append(AggregateData(candle))

    return aggregateData

