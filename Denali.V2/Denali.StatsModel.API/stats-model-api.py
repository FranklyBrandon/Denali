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
        frameEnd = i + 1
        #Lists can be sliced by their indices using myList[start:stop:skip].
        xDataInFrame = x[frameStart:frameEnd]
        yDataInFrame = y[frameStart:frameEnd]
        pdx = pd.DataFrame(xDataInFrame, columns=['value'])
        pdy = pd.DataFrame(yDataInFrame, columns=['value'])

        beta = calculateBeta(pdx, pdy)

        spreads = []
        rawSpreads = []
        for i in range(backlog):
            xData = xDataInFrame[i]
            yData = yDataInFrame[i]

            spread = yData['value'] - beta * xData['value']

            rawSpreads.append(spread)
            spreads.append(dict(spread = spread, timeUTC = xData['timeUTC']))

        spreadMean = np.mean(rawSpreads)
        spreadStd = np.std(rawSpreads)
        lastSpread = spreads[-1]
        zscore = (lastSpread['spread'] - spreadMean) / spreadStd

        results.append(dict(zscore = zscore, beta = beta, timeUTC = lastSpread['timeUTC'], spread = lastSpread['spread']))

    response = dict(results = results)
    return jsonify(response)

def calculateBeta(pdx, pdy):
    #constX = sm.add_constant(pdx['value'])
    result = sm.OLS(pdy['value'], pdx['value']).fit()
    return result.params[0]

if __name__ == '__main__':
    app.run()