from typing import List

from Models import AggregateData, MovingAverage


def get_emas(candleData, emaBacklog):
    smas = get_smas(candleData, emaBacklog)
    smoothingConstant = (2 / (emaBacklog + 1))

    emas = dict(Value=[], Date=[])
    for index, sma in enumerate(smas):
        if len(emas) == 0:
            emas['Date'].append(sma['Date'])
            emas['Value'].append(sma['Value'])
        else:
            emas['Date'].append(sma['Date'])

            previousEma = emas['Value'][index - 1]
            currentEma = (candleData[sma['CandleIndex']] - previousEma) * smoothingConstant + previousEma
            emas['Value'].append(currentEma)

    return get_smas(candleData, emaBacklog)

def get_smas1(candleData, smaBacklog):

    smas = dict(Value=[], Date=[], CandleIndex=[])
    for index in range(0, len(candleData) - 1):
        if (index < smaBacklog - 1):
            continue

        sum = 0
        for i in range(index - (smaBacklog), index):
            sum += candleData[i]['c']

        smas['Value'].append(sum / smaBacklog)
        smas['Date'].append(candleData[index]['t'])
        smas['CandleIndex'].append(index)

    return smas


def get_smas(aggregateData: List[AggregateData], backlog: int) -> List[MovingAverage]:
    
    smas = []
    for index in range(0, len(aggregateData) - 1):
        if (index < backlog - 1):
            continue

        sum = 0
        for i in range(index - (backlog), index):
            sum += aggregateData[i].close
        
        smas.append(MovingAverage(sum / backlog, aggregateData[index].time))
    
    return smas