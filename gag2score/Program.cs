using System;

namespace gag2score
{
    class Program
    {
        static void Main(string[] args)
        {
            HumorCalculator hc = new HumorCalculator();
            hc.humorScore("ふとんがふっとんだ");
        }
    }
}
