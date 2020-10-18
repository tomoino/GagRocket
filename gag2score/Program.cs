using System;
using System.Collections.Generic;
using System.IO;

namespace gag2score
{
    class Program
    {
        static void Main(string[] args)
        {
            string[][] data = loadCSV("../src/dajare_data_59900.csv");
            testDajareDiscriminator(data);
        }

        static string[][] loadCSV(string filePath) {
            var list = new List<string[]>();
            StreamReader reader = 
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8"));
            while (reader.Peek() >= 0) list.Add(reader.ReadLine().Split(','));
            reader.Close();
            return list.ToArray();
        }

        // // だじゃれと判定されるデータをホールドアウト法でテストデータと学習データに分割して出力する
        // static void makeDataForLearning(string[][] data, double train_data_rate) {
        //     HumorCalculator hc = new HumorCalculator();
        //     int n = data.Length;
        //     var dajareList = new List<string[]>(); // moto, score, reviewnum, wordlist, kana
        //     foreach (string[] row in data) {
        //         var (wordList, kana) = hc.morph(row[0]);

        //         if (hc.isDajare(kana)) {

        //         }
        //     }
        // }

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
