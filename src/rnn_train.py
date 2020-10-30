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
    funny_gags = [] # score 3以上のだじゃれを格納
    not_funny_gags = [] # score 3未満のだじゃれを格納
    with open(filepath,'r',encoding="utf-8_sig") as f:
        for l in f:
            row = l.split(",")
            
            score = row[1]
            words = [word_index[word] for word in row[2].split(' ') if word in word_index] # word_indexに変換
            words = words + [0]*(max_length - len(words)) # 長さをそろえる

            if float(score) < 3: # 評価3未満のだじゃれは面白くないと判定
                not_funny_gags.append((0, words))
            else:
                funny_gags.append((1, words))

    gags = funny_gags + not_funny_gags
    random.shuffle(gags)
    print(str(len(gags)) + ' examples are available')
    
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

    with open('data/word_index.csv', 'w', newline="") as f:
        writer = csv.writer(f)
        for key, val in word_index.items():
            try:
                writer.writerow([key,val])
            except:
                continue

    return np.array(embedding_matrix), word_index

def train(inputs, targets, embedding_matrix, batch_size=1024, epoch_count=100, max_length=32, model_filepath="model/rnn_model.h5", learning_rate=0.001):
    # train:validation:test = 6:2:2
    test_len = int(len(inputs) * 0.2)
    test_inputs = inputs[0:test_len]
    test_targets = targets[0:test_len]
    train_inputs = inputs[test_len:]
    train_targets = targets[test_len:]
    
    # 単語数、embeddingの次元
    num_words, word_vec_size = embedding_matrix.shape
    # モデルの構築 RNN
    model = Sequential([
        Embedding(num_words, word_vec_size,
                weights=[embedding_matrix], 
                input_length=max_length,
                trainable=False, 
                mask_zero=True),
        RNN(SimpleRNNCell(1, activation=None), input_shape=(None, num_words, word_vec_size), return_sequences=False),
        Activation("sigmoid")
    ])

    # モデルの構築 RNN 2
    # model = Sequential([
    #     Embedding(num_words, word_vec_size,
    #             weights=[embedding_matrix], 
    #             input_length=max_length,
    #             trainable=False, 
    #             mask_zero=True),
    #     RNN(SimpleRNNCell(50, activation=None), input_shape=(None, num_words, word_vec_size), return_sequences=False),
    #     Dense(25),
    #     Dense(1),
    #     Activation("sigmoid")
    # ])

    # モデルの構築 LSTM
    # model = Sequential([
    #     Embedding(num_words, word_vec_size,
    #             weights=[embedding_matrix], 
    #             input_length=max_length,
    #             trainable=False, 
    #             mask_zero=True),
    #     LSTM(1, activation=None, input_shape=(None, num_words, word_vec_size), return_sequences=False),
    #     Activation("sigmoid")
    # ])

    model.compile(loss='binary_crossentropy',
              optimizer=tf.keras.optimizers.Adam(1e-4),
              metrics=['accuracy'])

    model.summary()

    # 学習
    history = model.fit(train_inputs, train_targets,
              epochs=epoch_count,
              batch_size=batch_size,
              verbose=1,
              validation_split=0.25,
              shuffle=True)

    score = model.evaluate(test_inputs, test_targets, verbose=0)
    print('Test on '+str(len(test_inputs))+' examples')
    print('Test loss:', score[0])
    print('Test accuracy:', score[1])

    # モデルの保存
    model.save(model_filepath)
    return history

if __name__ == "__main__":
    embedding_matrix, word_index = load_word_vec("data/w2v.txt")
    gags = load_data("data/data_for_ml.csv", word_index)

    input_values = []
    target_values = []
    for target_value, input_value in gags:
        input_values.append(input_value)
        target_values.append(target_value)
    input_values = np.array(input_values)
    target_values = np.array(target_values)

    history = train(input_values, target_values, embedding_matrix, epoch_count=200)
    history_df = pd.DataFrame(history.history)

    history_df.loc[:,['val_loss','loss']].plot()
    plt.show()
    history_df.loc[:,['val_accuracy','accuracy']].plot()
    plt.show()