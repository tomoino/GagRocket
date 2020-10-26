#coding: utf-8
import numpy as np
import keras
from keras.optimizers import *
from keras.layers import *
from keras.callbacks import *
from keras.models import *
from numpy import *
import codecs
import pandas as pd
import matplotlib.pyplot as plt

def create_model(embed_size=128, max_length=50, filter_sizes=(2, 3, 4, 5), filter_num=64):
    inp = Input(shape=(max_length,))
    emb = Embedding(0xffff, embed_size)(inp)
    emb_ex = Reshape((max_length, embed_size, 1))(emb)
    convs = []
    # Convolution2Dを複数通りかける
    for filter_size in filter_sizes:
        conv = Convolution2D(filter_num, filter_size, embed_size, activation="relu")(emb_ex)
        pool = MaxPooling2D(pool_size=(max_length - filter_size + 1, 1))(conv)
        convs.append(pool)
    convs_merged = Concatenate()(convs)
    reshape = Reshape((filter_num * len(filter_sizes),))(convs_merged)
    fc1 = Dense(64, activation="relu")(reshape)
    bn1 = BatchNormalization()(fc1)
    do1 = Dropout(0.5)(bn1)
    fc2 = Dense(1, activation='sigmoid')(do1)
    model = Model(input=inp, output=fc2)
    return model

def load_data(filepath, max_length=50, min_length=1):
    gags = []
    tmp_gags = []
    with open(filepath,'r',encoding="utf-8_sig") as f:
        for l in f:
            row = l.split(",")
            
            score = row[1]
            kana = row[4]

            # 文字毎にUNICODEに変換
            kana = [ord(c) for c in kana.strip()]
            # 長い部分は打ち切り
            kana = kana[:max_length]
            kana_len = len(kana)
            if kana_len < min_length:
                continue
            if kana_len < max_length:
                # 固定長にするために足りない部分を0で埋める
                kana += ([0] * (max_length - kana_len))
            if float(score) < 3: # 評価3未満のだじゃれは面白くないと判定
                tmp_gags.append((0, kana))
            else:
                gags.append((1, kana))

    # 学習のためには面白いだじゃれとそれ以外が同数であった方がいい
    random.shuffle(tmp_gags)
    gags.extend(tmp_gags[:len(gags)])
    random.shuffle(gags)
    return gags

def train(inputs, targets, batch_size=100, epoch_count=100, max_length=50, model_filepath="model.h5", learning_rate=0.001):
    # 学習率を少しずつ下げるようにする
    start = learning_rate
    stop = learning_rate * 0.01
    learning_rates = np.linspace(start, stop, epoch_count)

    # モデル作成
    model = create_model(max_length=max_length)
    optimizer = Adam(lr=learning_rate)
    model.compile(loss='binary_crossentropy',
                  optimizer=optimizer,
                  metrics=['accuracy'])

    # 学習
    history = model.fit(inputs, targets,
              nb_epoch=epoch_count,
              batch_size=batch_size,
              verbose=1,
              validation_split=0.1,
              shuffle=True,
              callbacks=[
                  LearningRateScheduler(lambda epoch: learning_rates[epoch]),
              ])

    # モデルの保存
    model.save(model_filepath)
    return history

if __name__ == "__main__":
    gags = load_data("data_for_train.csv")

    input_values = []
    target_values = []
    for target_value, input_value in gags:
        input_values.append(input_value)
        target_values.append(target_value)
    input_values = np.array(input_values)
    target_values = np.array(target_values)
    history = train(input_values, target_values, epoch_count=500)
    history_df = pd.DataFrame(history.history)
    history_df.plot()
    plt.show()