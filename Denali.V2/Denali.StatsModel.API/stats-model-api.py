import math
import numpy as np
from flask import Flask, jsonify, request
import statsmodels.tsa.stattools as sm
import pandas as pd

app = Flask(__name__)

# Accepts JSON data that contains an 'x' and 'y' parameter 
# that represent two series of prices for two correlated stocks.
@app.route('/ols', methods=['GET'])
def ols_get():
    data = request.get_json(force=True)
    x = data['x']
    y = data['y']
    backlog = data['backlog']

    results = []
    for i in range(backlog - 1, len(x)):
        frameStart = i - (backlog - 1)
        frameEnd = i
        #Lists can be sliced by their indices using myList[start:stop:skip].
        xDataInFrame = x[frameStart:frameEnd]
        yDataInFrame = y[frameStart:frameEnd]
        pdx = pd.DataFrame(xDataInFrame, columns=['value'])
        pdy = pd.DataFrame(yDataInFrame, columns=['value'])

        beta = calculateBeta(pdx, pdy)

        spreads = []
        rawSpreads = []
        for i in range(backlog - 1):
            xData = xDataInFrame[i]
            yData = yDataInFrame[i]

            spread = math.log(xData['value']) - beta * math.log(yData['value'])

            rawSpreads.append(spread)
            spreads.append(dict(spread = spread, timeUTC = xData['timeUTC']))

        spreadMean = np.mean(rawSpreads)
        spreadStd = np.std(rawSpreads)
        lastSpread = spreads[-1]
        zscore = (lastSpread - spreadMean) / spreadStd

        results.append(dict(lastSpread['spread'], zscore = zscore, beta = beta, timeUTC = lastSpread['timeUTC']))

    response = dict(results = results, beta = beta)
    return jsonify(response)

def calculateBeta(pdx, pdy):
    constY = sm.add_constant(pdy['value'])
    result = sm.OLS(pdx['value'], constY).fit()
    return result.params['value']

if __name__ == '__main__':
    app.run()