using System;
using System.IO; // input.txt/output.txt IO 用

using System.Collections.Generic;
using System.Linq;

namespace AtCoderTemplate
{
    internal class Program
    {
        static void Main()
        {
            Solver sol = new Solver();
            sol.test();
        }
    }

    public class Solver
    {
        private System.Diagnostics.Stopwatch sw;
        public void solve()
        {
            sw.Stop();
            //int [] intArr = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            //int L = intArr[0]; int R = intArr[1];
            //string S = Console.ReadLine();
            int t = int.Parse(Console.ReadLine());
            sw.Start();

        }

        public Solver()
        {
            sw = new System.Diagnostics.Stopwatch();
        }

        public void test()
        {
            while (true)
            {
                sw.Restart();
                solve();
                sw.Stop();
                Console.WriteLine("Runtime " + sw.ElapsedMilliseconds + " ms");
            }
        }
    }
}
