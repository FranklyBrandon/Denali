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
    x = pd.DataFrame(data['x'], columns=['value'])
    y = pd.DataFrame(data['y'], columns=['value'])

    constY = sm.add_constant(y['value'])
    result = sm.OLS(x['value'], constY).fit()
    beta = result.params['value']

    spreads = np.log(x['value']) - beta * np.log(y['value'])
    zscores = (spreads - spreads.mean()) / np.std(spreads)

    response = dict(beta = beta, spreads = spreads.values.tolist(), zscores = zscores.values.tolist())
    return jsonify(response)

if __name__ == '__main__':
    app.run()