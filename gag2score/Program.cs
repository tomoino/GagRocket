using System;
using System.Collections.Generic;
using System.IO;

namespace gag2score
{
    class Program
    {
        static void Main(string[] args)
        {
            string[][] data = loadCSV("../src/dajare_data_10.csv");

            foreach (string[] row in data) {
                foreach (string elm in row) {
                    System.Console.Write("{0} ", elm);
                }
                    System.Console.Write("\n");
            }

            HumorCalculator hc = new HumorCalculator();
            hc.humorScore("ふとんがふっとんだ");
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
