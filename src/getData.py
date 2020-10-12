import requests
from bs4 import BeautifulSoup
import re
import csv

data = []

for i in range(10):
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

print(data)
with open('dajare_data.csv', 'w', newline="") as f:
    writer = csv.writer(f)
    writer.writerows(data)
# for elem in elems: 
#   try:
#     string = elem.get("class").pop(0)
#     if string in "category":
#       print(elem.string)
#       title = elem.find_next_sibling("h3")
#       print(title.text.replace('\n',''))
#       r = elem.find_previous('a')
#       print(urlName + r.get('href'), '\n')
#   except:
#     pass