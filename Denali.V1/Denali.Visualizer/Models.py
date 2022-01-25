class AggregateData(object):
    def __init__(self, jsonDict):
        self.open = jsonDict['o']
        self.close = jsonDict['c']
        self.high = jsonDict['h']
        self.low = jsonDict['l']
        self.time = jsonDict['t']

class MovingAverage(object):
    def __init__(self, value: int, time: str):
        self.value = value
        self.time = time