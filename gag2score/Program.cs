using System;
using System.Collections.Generic;
using System.IO;

namespace gag2score
{
    class Program
    {
        static void Main(string[] args)
        {
            HumorCalculator hc = new HumorCalculator();
            // Console.WriteLine(hc.humorScore("私は日本人だ。"));
            string[][] data = loadCSV("../src/dajare_data_4297.csv");
            int n = data.Length;
            int cnt = 0;

            foreach (string[] row in data) {
                // foreach (string elm in row) {
                //     System.Console.Write("{0} ", elm);
                // }
                // System.Console.Write("\n");
                double humorScore = hc.humorScore(row[0]);
                if (humorScore > 0) 
                    cnt++;

                // System.Console.WriteLine(humorScore);
                // Console.WriteLine();
            }

            Console.WriteLine("識別率:{0}%", (double)cnt / (double)n*100.0);
        }

        static string[][] loadCSV(string filePath) {
            var list = new List<string[]>();
            StreamReader reader = 
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8"));
            while (reader.Peek() >= 0) list.Add(reader.ReadLine().Split(','));
            reader.Close();
            return list.ToArray();
        }
    }
}
