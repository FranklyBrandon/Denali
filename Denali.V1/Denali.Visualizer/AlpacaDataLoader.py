import json, os
from typing import List

from Models import AggregateData

def get_alpaca_candle_data() -> List[AggregateData]:

    dir = "C:\\Denali\\Denali.V1\\Denali.Visualizer"
    fileName = "4_28_2022_AAPL.txt"
    path = os.path.join(dir, fileName)
    file = open(path, "r")

    rawData = json.load(file)
    candleData = list(rawData['bars'])
    aggregateData = list()

    for candle in candleData:
        aggregateData.append(AggregateData(candle))

    return aggregateData

