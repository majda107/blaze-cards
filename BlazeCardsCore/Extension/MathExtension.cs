using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Extension
{
    public class MathExtension
    {
        public static double Hypot(params double[] values)
        {
            double sum = 0;
            foreach (var value in values)
                sum += value * value;

            return Math.Sqrt(Math.Abs(sum));
        }
    }
}
