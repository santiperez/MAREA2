using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public static class DigitsSumExtension
    {
        public static int SumDigits(this int num)
        {
            int acc;
            do
            {
                acc = 0;
                while (num > 0)
                {
                    acc += num % 10;
                    num /= 10;
                }
                num = acc;
            } while (acc >= 10);
            return acc;
        }

    }
}
