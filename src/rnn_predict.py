import numpy as np
from keras.models import load_model

def predict(gags, model_filepath="rnn_model.h5"):
    model = load_model(model_filepath)
    ret = model.predict(gags)
    return ret

def load_word_index(filepath):
    word_index = {}

    with open(filepath,'r') as f:
        for l in f:
            row = l.split(",")
            word_index[row[0]] = row[1]

    return word_index

if __name__ == "__main__":
    max_length = 32
    word_index = load_word_index("word_index.csv")
    gag = "犬 が 居 ぬ"
    gag = [word_index[word] for word in gag.split(' ') if word in word_index] # word_indexに変換
    gag = gag + [0]*(max_length - len(gag)) # 長さをそろえる
    ret = predict(np.array([gag]))
    predict_result = ret[0][0]
    print("gagの面白さ: {}%".format(predict_result * 100))