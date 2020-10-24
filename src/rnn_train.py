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
import csv

def load_data(filepath, word_index, max_length=32):
    gags = []
    tmp_gags = []
    with open(filepath,'r',encoding="utf-8_sig") as f:
        for l in f:
            row = l.split(",")
            
            score = row[1]
            words = [word_index[word] for word in row[3].split(' ') if word in word_index] # word_indexに変換
            words = words + [0]*(max_length - len(words)) # 長さをそろえる

            if float(score) < 3: # 評価3未満のだじゃれは面白くないと判定
                tmp_gags.append((0, words))
            else:
                gags.append((1, words))

    # 学習のためには面白いだじゃれとそれ以外が同数であった方がいい
    random.shuffle(tmp_gags)
    gags.extend(tmp_gags[:len(gags)])
    random.shuffle(gags)
    return gags

# word2vecからembedding layer用の重み行列を作成。wordとindexを結びつける辞書word_indexも返す。
# predict用にword_index.csvも生成する
def load_word_vec(filepath):
    word_index = {}
    embedding_matrix = []

    with open(filepath,'r',encoding="utf-8_sig") as f:
        for l in f:
            row = l.replace("\n", "").split(" ")
            if len(row) != 101: # 例外が起きる行は無視する
                continue
            word = row[0]
            vec = [float(val) for val in row[1:]]
            embedding_matrix.append(vec)
            word_index[word] = len(embedding_matrix)

    # padding用(入力の時系列方向の長さを揃えるためにtoken id=0でpaddingする想定)にindexを追加
    word_index['0'] = 0

    with open('word_index.csv', 'w', newline="") as f:
        writer = csv.writer(f)
        for key, val in word_index.items():
            try:
                writer.writerow([key,val])
            except:
                continue

    return np.array(embedding_matrix), word_index

def train(inputs, targets, embedding_matrix, batch_size=100, epoch_count=100, max_length=32, model_filepath="rnn_model.h5", learning_rate=0.001):
    # 学習率を少しずつ下げるようにする
    # start = learning_rate
    # stop = learning_rate * 0.01
    # learning_rates = np.linspace(start, stop, epoch_count)

    # 単語数、embeddingの次元
    num_words, w2v_size = embedding_matrix.shape
    # モデルの構築
    model = Sequential([
        Embedding(num_words, w2v_size,
                weights=[embedding_matrix], 
                input_length=max_length,
                trainable=False, 
                mask_zero=True),
        RNN(SimpleRNNCell(1, activation='sigmoid'), input_shape=(None, num_words, w2v_size), return_sequences=False),
    ])
    model.summary()
    # inp = Input(shape=(max_length,))
    # emb = Embedding(num_words, w2v_size,
    #             weights=[embedding_matrix], 
    #             input_length=max_length,
    #             trainable=False, 
    #             mask_zero=True)(inp)
    model.compile(loss='binary_crossentropy',
              optimizer=tf.keras.optimizers.Adam(1e-4),
              metrics=['accuracy'])

    # 学習
    history = model.fit(inputs, targets,
              epochs=epoch_count,
              batch_size=batch_size,
              verbose=1,
              validation_split=0.1,
              shuffle=True)

    # モデルの保存
    model.save(model_filepath)
    return history

if __name__ == "__main__":
    embedding_matrix, word_index = load_word_vec("w2v.txt")
    gags = load_data("data_for_train.csv", word_index)

    input_values = []
    target_values = []
    for target_value, input_value in gags:
        input_values.append(input_value)
        target_values.append(target_value)
    input_values = np.array(input_values)
    target_values = np.array(target_values)

    history = train(input_values, target_values, embedding_matrix, epoch_count=100)
    history_df = pd.DataFrame(history.history)
    history_df.plot()
    plt.show()