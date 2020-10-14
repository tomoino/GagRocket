using System;

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
            Console.WriteLine(str);
            if (!isDajare(str))
                return 0.0;

            return 1.0;
        }
    }
}
