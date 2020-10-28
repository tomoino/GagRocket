import csv
import humorcalc as hc
from load_data import load_dajare_data, load_not_dajare_data

def main():
    is_dajare_test()

def is_dajare_test():
    dajare_data = load_dajare_data("data/dajare_data.csv")
    not_dajare_data = load_not_dajare_data("data/not_dajare_data.csv")

    min_len = min(len(dajare_data), len(not_dajare_data))
    test_data = dajare_data[0:min_len] + not_dajare_data[0:min_len] # 短いほうに長さをそろえて連結

    test_data_len = len(test_data)
    print("Length of test data: " + str(test_data_len))

    tp = 0
    fp = 0
    tn = 0
    fn = 0

    false_data = []  # FP, FNのデータを保存する
    data_for_ml = [] # TPから機械学習用のデータを作る [原文, score, words(半角join), reading]
    cnt = 0

    for data in test_data:
        (words, reading) = hc.morph(data[0])
        prediction = 1 if hc.is_dajare(reading) else 0
        actual = data[1]

        if prediction == 0 and actual == 0:
            tn += 1
        elif prediction == 0 and actual == 1:
            fn += 1
            false_data.append(["False Negative", data[0], reading])
        elif prediction == 1 and actual == 0:
            fp += 1
            false_data.append(["False Positive", data[0], reading])
        elif prediction == 1 and actual == 1:
            tp += 1
            data_for_ml.append([data[0], data[2], ' '.join(words), reading]) 

        if cnt % 1000 == 0:
            print("Data no." + str(cnt) + " is done.")
            
        cnt += 1

    print("True Positive: "+str(tp))
    print("False Positive: "+str(fp))
    print("False Negative: "+str(fn))
    print("True Negative: "+str(tn))

    accuracy = float((tp + tn) / (tp + tn + fp + fn))
    precision = float(tp / (tp + fp))
    recall = float(tp / (tp + fn))
    specifility = float(tn / (fp + tn))
    F = (2 * precision * recall) / (precision + recall)

    print("Accuracy: " + str(accuracy))
    print("Precision: "+str(precision))
    print("Recall: "+str(recall))
    print("Specificity: "+str(specifility))
    print("F-measure: " + str(F))
    
    # FP, FNのデータを出力
    with open('data/false_data.csv', 'w', newline="") as f:
        writer = csv.writer(f)
        for row in false_data:
            try:
                writer.writerow(row)
            except:
                continue

    # 機械学習用のデータを出力
    with open('data/data_for_ml.csv', 'w', newline="") as f:
        writer = csv.writer(f)
        for row in data_for_ml:
            try:
                writer.writerow(row)
            except:
                continue

if __name__ == "__main__":
    main()