# python

from flask import Flask, Blueprint, request, abort, jsonify
from keras.models import load_model
from humorcalc import calc_humor_score

app = Flask(__name__)

def load_word_index(filepath):
    word_index = {}

    with open(filepath,'r') as f:
        for l in f:
            row = l.replace("\n", "").split(",")
            word_index[row[0]] = row[1]

    return word_index

# model and backend graph must be created on global
global model, word_index
model = load_model("model/rnn_model.h5")
word_index = load_word_index("data/word_index.csv")

@app.route('/', methods=['POST'])
def index():
    payload = request.json
    gag = payload.get('text')
    score = calc_humor_score(gag, model, word_index)

    return str(score), 201

if __name__ == "__main__":
	app.debug = True
	app.run()