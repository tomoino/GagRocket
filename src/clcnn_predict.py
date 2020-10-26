# -*- coding:utf-8 -*-

import numpy as np
from keras.models import load_model

def predict(gags, model_filepath="model.h5"):
    model = load_model(model_filepath)
    ret = model.predict(gags)
    return ret

if __name__ == "__main__":
    max_length = 50
    gag = "フトンガフットンダ"
    gag = [ord(x) for x in gag.strip()]
    gag = gag[:max_length]
    if len(gag) < max_length:
        gag += ([0] * (max_length - len(gag)))
    ret = predict(np.array([gag]))
    predict_result = ret[0][0]
    print("gagの面白さ: {}%".format(predict_result * 100))