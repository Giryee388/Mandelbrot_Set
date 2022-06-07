using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotSetUBoji
{
    public class Mandelbrot
    {
        public int TotalIterations = 300;
        public int Limit = 30000000;


        public double Calculate(double x, double y)
        {
            //f(z)= z^2 +c

            double a = x;
            double b = y;

            double z0 = 0;
            double z1 = 0;
            
            double i = 0;
            while (i < TotalIterations)
            {
                double bb = 2 * a * b;
                a = a * a - b * b + x;
                b = bb + y;

                z1 = z0;
                z0 = Math.Abs(a + b);

                if (z0 == z1)
                {
                    return 1;
                }

                if (z1 > Limit)
                {
                    return i / TotalIterations;
                }
                i++;
            }
            return i / TotalIterations;
        }
    }
}
