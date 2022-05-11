import json, os
from typing import List

from Models import AggregateData

def get_alpaca_candle_data() -> List[AggregateData]:

    rawData = get_json_data("bars_AAPL_4_22_2022.json")
    candleData = list(rawData['bars'])
    aggregateData = list()

    for candle in candleData:
        aggregateData.append(AggregateData(candle))

    return aggregateData

def get_elephant_bars() -> List[str]:
    rawData = get_json_data("AAPL_Elephants_4_22_2022.json")
    elephants = list()

    for elephant in rawData:
        elephants.append(elephant)

    return elephants

def get_json_data(fileName: str) -> any:
    dir = "C:\\Denali\\Denali.V2\\Denali.Worker\\Resources"
    path = os.path.join(dir, fileName)
    with open(path, "r") as read_file:
        rawData = json.load(read_file, encoding='utf-8')
        return rawData


