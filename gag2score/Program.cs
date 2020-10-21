using System;
using System.Collections.Generic;
using System.IO;
 using System.Text;

namespace gag2score
{
    class Program
    {
        static void Main(string[] args)
        {
            string[][] data = loadCSV("../src/dajare_data_59900.csv");
            // testDajareDiscriminator(data);
            makeDataForLearning(data);
        }

        static string[][] loadCSV(string filePath) {
            var list = new List<string[]>();
            StreamReader reader = 
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8"));
            while (reader.Peek() >= 0) list.Add(reader.ReadLine().Split(','));
            reader.Close();
            return list.ToArray();
        }

        // だじゃれと判定されるデータをCSV出力する
        static void makeDataForLearning(string[][] data) {
            HumorCalculator hc = new HumorCalculator();
            var dajareList = new List<string[]>(); // moto, score, reviewnum, wordlist, kana
            foreach (string[] row in data) {
                var (wordList, kana) = hc.morph(row[0]);

                if (hc.isDajare(kana)) {
                    var newRow = new string[] {row[0], row[1], row[2], string.Join( " ", wordList), kana}; 
                    dajareList.Add(newRow);
                }
            }

            // CSVに書き込み
            try
            {
                // ファイルを開く
                StreamWriter file = new StreamWriter(@"dajare_data_for_learning.csv", false, Encoding.UTF8);
                foreach (string[] row in dajareList)
                {
                    file.WriteLine($"{row[0]},{row[1]},{row[2]},{row[3]},{row[4]}");
                }
                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); // 例外検出時にエラーメッセージを表示
            }
        }

        static void testDajareDiscriminator (string[][] data) {
            HumorCalculator hc = new HumorCalculator();
            int n = data.Length;
            int cnt = 0;

            foreach (string[] row in data) {
                var (wordList, kana) = hc.morph(row[0]);
                if (hc.isDajare(kana))
                    cnt++;
            }

            Console.WriteLine("識別率:{0}%", (double)cnt / (double)n*100.0);
        }
    }
}
