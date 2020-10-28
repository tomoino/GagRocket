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
from load_data import load_dajare_data

def create_model(embed_size=128, max_length=80, filter_sizes=(2, 3, 4, 5), filter_num=64):
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

def load_data(filepath, max_length=80, min_length=1):
    funny_gags = [] # score 3以上のだじゃれを格納
    not_funny_gags = [] # score 3未満のだじゃれを格納
    with open(filepath,'r',encoding="utf-8_sig") as f:
        for l in f:
            row = l.split(",")
            
            score = row[1]
            reading = row[3]

            # 文字毎にUNICODEに変換
            reading = [ord(c) for c in reading.strip()]
            # 長い部分は打ち切り
            reading = reading[:max_length]
            reading_len = len(reading)
            if reading_len < min_length:
                continue
            if reading_len < max_length:
                # 固定長にするために足りない部分を0で埋める
                reading += ([0] * (max_length - reading_len))
            if float(score) < 3: # 評価3未満のだじゃれは面白くないと判定
                not_funny_gags.append((0, reading))
            else:
                funny_gags.append((1, reading))

    gags = funny_gags + not_funny_gags
    random.shuffle(gags)

    print(str(len(gags))+' examples are available')
    return gags

def train(inputs, targets, batch_size=1024, epoch_count=100, max_length=80, model_filepath="model/clcnn_model.h5", learning_rate=0.001):
    # train:validation:test = 6:2:2
    test_len = int(len(inputs) * 0.2)
    test_inputs = inputs[0:test_len]
    test_targets = targets[0:test_len]
    train_inputs = inputs[test_len:]
    train_targets = targets[test_len:]
    
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

    model.summary()

    # 学習
    history = model.fit(train_inputs, train_targets,
              nb_epoch=epoch_count,
              batch_size=batch_size,
              verbose=1,
              validation_split=0.25,
              shuffle=True,
              callbacks=[
                  LearningRateScheduler(lambda epoch: learning_rates[epoch]),
              ])

    score = model.evaluate(test_inputs, test_targets, verbose=0)
    print('Test on '+str(len(test_inputs))+' examples')
    print('Test loss:', score[0])
    print('Test accuracy:', score[1])

    # モデルの保存
    model.save(model_filepath)
    return history

if __name__ == "__main__":
    gags = load_data("data/data_for_ml.csv")

    input_values = []
    target_values = []
    for target_value, input_value in gags:
        input_values.append(input_value)
        target_values.append(target_value)
    input_values = np.array(input_values)
    target_values = np.array(target_values)
    history = train(input_values, target_values, epoch_count=100)
    history_df = pd.DataFrame(history.history)

    history_df.loc[:,['val_loss','loss']].plot()
    plt.show()
    history_df.loc[:,['val_accuracy','accuracy']].plot()
    plt.show()
