using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NMeCab.Specialized;

namespace Gag2Score
{
    class HumorCalculator
    {
        public bool isDajare(string str)
        {
            str = str.Replace("ッ", "");

            int len = str.Length;

            for (int i = len - 1; i > 1; i--) {
                List<string> list = new List<string>();
                for (int j = 0; j + i <= len;j++)
                {
                    list.Add(str.Substring(j,i));
                    if (i > 2 && str.Contains("ー")) {
                        list.Add(str.Substring(j,i).Replace("ー","ア"));
                        list.Add(str.Substring(j,i).Replace("ー","イ"));
                        list.Add(str.Substring(j,i).Replace("ー","ウ"));
                        list.Add(str.Substring(j,i).Replace("ー","エ"));
                        list.Add(str.Substring(j,i).Replace("ー","オ"));
                    }
                }
                List<string> duplicates = FindDuplication(list);
                if (duplicates.Count > 0) {
                    return true;
                }
            }
            return false;
        }

        public (bool, string) humorScore(string str)
        {
            var (wordList, kana) = morph(str);
            var words = string.Join(" ", wordList);
            return (isDajare(kana), words);

            // if (!isDajare(kana)) {
            //     return (false, kana);
            // }

            // return (true, kana);
        }

        public (List<string>, string) morph(string str) {
            string[] strToTrim = { " "," ", "、", "。", "!", "?", "！", "？", "「", "」", "『", "』", "(", ")", "（", "）", "・", "～", "\"", "\'"};
            foreach (string s in strToTrim) {
                str = str.Replace(s, "");
            }

            var wordList = new List<string>();
            string kana = "";

            var dicDir = @"Assets/Scripts/IpaDic";

            using (var tagger = MeCabIpaDicTagger.Create(dicDir)) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse(str); // 形態素解析を実行
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    // Console.WriteLine($"表層形：{node.Surface}");
                    // Console.WriteLine($"読み　：{node.Reading}");
                    // Console.WriteLine($"品詞　：{node.PartsOfSpeech}");
                    wordList.Add(node.Surface);
                    string tmp = node.Reading; // node.Readingが書き換え不可なので別の変数に代入
                    if (node.Reading == "") {
                        tmp = node.Surface; // カタカナ語の読み情報が入っていないようなので表層形を代入する
                    }
                    if (node.Reading == "ハ" && node.PartsOfSpeech == "助詞")
                        tmp = tmp.Replace("ハ", "ワ");
                    kana += tmp;
                }
            }
            kana = kana.Replace("ヲ", "オ");
            return (wordList, kana);
        }

        void dispList(List<string> list) {
            foreach (string elm in list) {
                Console.Write($"{elm} ");
            }
            Console.Write("\n");
        }

        static List<string> FindDuplication(List<string> list)
        {
            var duplicates = list.GroupBy(name => name).Where(name => name.Count() > 1)
                .Select(group => group.Key).ToList();

            return duplicates;
        }
    }
}
