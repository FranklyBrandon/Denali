from flask import Flask, jsonify, request

app = Flask(__name__)

@app.route('/hello', methods=['GET'])
def hello_get():
    data = request.get_json(force=True)
    return jsonify({"msg": "hi!"})

if __name__ == '__main__':
    app.run()