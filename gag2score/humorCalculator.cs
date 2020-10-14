using System;
using System.Collections.Generic;
using NMeCab.Specialized;

namespace gag2score
{
    class HumorCalculator
    {
        public bool isDajare(string str)
        {
            return true;
        }

        public double humorScore(string str)
        {
            string[] strToTrim = { " "," ", "、", "。", "!", "?", "！", "？", "「", "」", "『", "』", "(", ")", "（", "）"};
            foreach (string s in strToTrim) {
                str = str.Replace(s, "");
            }

            var (wordList, kana) = morph(str);

            Console.WriteLine(str);
            dispList(wordList);
            Console.WriteLine(kana);
            Console.WriteLine();

            if (!isDajare(str))
                return 0.0;

            return 1.0;
        }

        static (List<string>, string) morph(string str) {
            var wordList = new List<string>();
            string kana = "";

            using (var tagger = MeCabIpaDicTagger.Create()) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse(str); // 形態素解析を実行
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    // Console.WriteLine($"表層形：{node.Surface}");
                    // Console.WriteLine($"読み　：{node.Reading}");
                    // Console.WriteLine($"品詞　：{node.PartsOfSpeech}");
                    // Console.WriteLine();
                    wordList.Add(node.Surface);
                    kana += node.Reading;
                }
            }

            return (wordList, kana);
        }

        void dispList(List<string> list) {
            foreach (string elm in list) {
                Console.Write($"{elm} ");
            }
            Console.Write("\n");
        }
    }
}
