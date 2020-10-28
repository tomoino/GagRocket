import numpy as np
from keras.models import load_model
import MeCab
import re

def calc_humor_score(text, model, word_index):
    (words, reading) = morph(text)
    if not is_dajare(reading):
        return 0
    
    return predict(words, model, word_index)
    
def morph(text):
    words = [] # 単語の原形
    reading = "" # 単語の読み（発音）

    # 発音に関係のない不要な文字を削除
    char_to_trim = [" "," ", "、", "。", "!", "?", "！", "？", "「", "」", "『", "』", "(", ")", "（", "）", "・", "～", "\"", "\'"];
    for c in char_to_trim:
        text = text.replace(c, "")

    res = MeCab.Tagger().parse(text)

    lines = res.split('\n')
    items = (re.split('[\t]',line) for line in lines)
    for item in items:
        if len(item) == 1:
            continue

        info = re.split('[,]', item[1])
        words.append(info[6])

        if len(info) == 9:
            reading += info[8] if info[8] != "ヲ" else "オ"
        else:
            reading += item[0] # 登録されていない語やカタカナ語への対応

    return (words, reading)

def is_dajare(reading):
    reading = reading.replace("ッ", "").replace("ー", "") # 音韻変化を考慮してトリム
    reading_len = len(reading)

    for i in range(2, int(reading_len / 2) + 1): # i文字の繰り返しを検出する
        parts = [reading[j:j + i] for j in range(0, reading_len - i + 1)] # 1文字ずつずらしながら検出
        if len(parts) != len(set(parts)):
            return True

    return False

def predict(words, model, word_index):
    max_length = 32 # 含まれる単語の最大の数
    words = [word_index[word] for word in words if word in word_index]
    words = words + [0]*(max_length - len(words))
    ret = model.predict(np.array([words]))
    
    return ret[0][0]