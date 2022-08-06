import json
from flask import Flask, jsonify, request
import statsmodels.tsa.stattools as sm

app = Flask(__name__)

@app.route('/ols', methods=['GET'])
def ols_get():
    data = request.get_json(force=True)
    x = data['x']
    y = data['y']
    constY = sm.add_constant(y)
    result = sm.OLS(x, constY).fit()
    return jsonify(result)
    #beta = result.params['close']

if __name__ == '__main__':
    app.run()