import math
import numpy as np
from flask import Flask, jsonify, request
import statsmodels.tsa.stattools as sm
import pandas as pd

app = Flask(__name__)

# Accepts JSON data that contains an 'x' and 'y' parameter 
# that represent two series of returns for two correlated stocks.
@app.route('/ols', methods=['GET'])
def ols_get():
    data = request.get_json(force=True)
    x = data['x']
    y = data['y']
    pdx = pd.DataFrame(data['x'], columns=['value'])
    pdy = pd.DataFrame(data['y'], columns=['value'])

    constY = sm.add_constant(pdy['value'])
    result = sm.OLS(pdx['value'], constY).fit()
    beta = result.params['value']

    spreads = []
    rawSpreads = []
    for i in range(len(x)):
        xData = x[i]
        yData = y[i]
        spread = math.log(xData['value']) - beta * math.log(yData['value'])
        rawSpreads.append(spread)
        spreads.append(dict(spread = spread, timeUTC = xData['timeUTC']))

    results = []
    spreadMean = np.mean(rawSpreads)
    spreadStd = np.std(rawSpreads)
    for _, spread in enumerate(spreads):
        zscore = (spread['spread'] - spreadMean) / spreadStd
        results.append(dict(spread = spread['spread'], zscore = zscore, timeUTC = spread['timeUTC']))

    #spreads = np.log(x['value']) - beta * np.log(y['value'])
    #zscores = (spreads - spreads.mean()) / np.std(spreads)



    response = dict(results = results, beta = beta)
    return jsonify(response)

if __name__ == '__main__':
    app.run()