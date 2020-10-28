import requests
from bs4 import BeautifulSoup
import re
import csv

data = []

for i in range(100000):
    print(i + 1)
    try:
        urlName = "https://dajare.jp/works/"+str(i+1)+"/"
        url = requests.get(urlName)
        soup = BeautifulSoup(url.content, "html.parser")
        [gag] = soup.select("#PanelWorkDetailMainTitle h2")
        [score] = soup.select("#PanelWorkDetailEvaluationAverage")

        gagText = gag.getText() 
        gagScore = re.search(r'(?<=：\n).+?(?=\n)', score.getText()).group()
        gagReviewNum = re.search(r'(?<=（).+?(?=）)', score.getText()).group()
        data.append([gagText, gagScore, gagReviewNum])
    except:
        continue

with open('data/dajare_data.csv', 'w', newline="") as f:
    writer = csv.writer(f)
    for row in data:
        try:
            writer.writerow(row)
        except:
            continue