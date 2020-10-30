# GagRocket
だじゃれの面白さをエネルギーにして飛ぶロケットのおもちゃ

## 遊び方
* 100次元の学習済みword2vecをdata/w2v.txtに保存
    * 例：[WikiEntVec](https://github.com/singletongue/WikiEntVec/releases)
* モデルを作成
``` 
python src/rnn_train.py 
```
* サーバーを走らせる
``` 
python src/api.py 
```
* dist/GagRocket.exeを実行

## 開発環境
* Windows 10 Pro / Home 2004
* Unity 2019.4.12f1
* Python 3.7.7
* flask 1.1.2
* keras 2.3.1
* MeCab 0.996
