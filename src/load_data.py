def load_dajare_data(filepath):
    dajare_data = []  # [[dajare_text, isDajare, score]]
    
    with open(filepath,'r', encoding="utf8", errors='ignore') as f:
        for l in f:
            row = l.replace("\n", "").split(",")
            try:
                dajare_data.append([row[0], 1, float(row[1])])
            except:
                pass

    return dajare_data

def load_not_dajare_data(filepath):
    not_dajare_data = []  # [[dajare_text, isDajare]]
    
    with open(filepath,'r', encoding="utf8", errors='ignore') as f:
        for l in f:
            row = l.replace("\n", "").split(",")
            not_dajare_data.append([row[0], 0])

    return not_dajare_data