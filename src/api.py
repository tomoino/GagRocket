# python

from flask import Flask, Blueprint, request, abort, jsonify
import numpy as np
from keras.models import load_model

app = Flask(__name__)

def load_word_index(filepath):
    word_index = {}

    with open(filepath,'r') as f:
        for l in f:
            row = l.split(",")
            word_index[row[0]] = row[1]

    return word_index

# model and backend graph must be created on global
global model, word_index
model = load_model("rnn_model.h5")
word_index = load_word_index("word_index.csv")

@app.route('/', methods=['POST'])
def index():
    payload = request.json
    gag = payload.get('text')

    max_length = 32
    gag = [word_index[word] for word in gag.split(' ') if word in word_index]
    gag = gag + [0]*(max_length - len(gag))
    ret = predict(np.array([gag]))
    predict_result = ret[0][0]

    return str(predict_result), 201

def predict(gags):
    ret = model.predict(gags)
    return ret

if __name__ == "__main__":
	app.debug = True
	app.run()