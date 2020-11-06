# <img src="https://user-images.githubusercontent.com/55827264/98373590-d10c1780-2082-11eb-8494-c0b95e8a6e17.png" width=30%>
だじゃれの面白さをエネルギーにして飛ぶロケットのおもちゃ

<img src="https://user-images.githubusercontent.com/55827264/98374512-1c72f580-2084-11eb-9807-7b901088cafd.png" height=300>   <img src="https://user-images.githubusercontent.com/55827264/98374525-239a0380-2084-11eb-8072-6303f8beea6a.png" height=300>

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
