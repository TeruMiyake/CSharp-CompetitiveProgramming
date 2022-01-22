using System;
using System.IO; // input.txt/output.txt IO 用
using System.Runtime.CompilerServices;

using System.Collections.Generic;
using System.Linq;

using LibraryForDotnetCore.Basics;

namespace AtCoderTemplate
{
    internal class Program
    {
        static void Main()
        {
            Solver sol = new Solver();
            sol.solve();
        }
    }

    public class Solver
    {
        bool isOnRealStage = false;
        public void solve([CallerMemberName] string memberName = "")
        {
            if (memberName != "LocalMain") isOnRealStage = true;
            // 本番環境でのみ行う処理
            if (isOnRealStage)
            {
                var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
                Console.SetOut(writer);
            }


            //int [] intArr = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            //int L = intArr[0]; int R = intArr[1];
            //string S = Console.ReadLine();
            int t = int.Parse(Console.ReadLine());

            int maxcnt = 0;
            string ans = "";
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < t; i++)
            {
                string s = Console.ReadLine();
                if (dict.ContainsKey(s))
                {
                    dict[s]++;
                    if (dict[s] > maxcnt)
                    {
                        maxcnt = dict[s];
                        ans = s;
                    }
                }
                else
                {
                    dict[s] = 1;
                    if (dict[s] > maxcnt)
                    {
                        maxcnt = dict[s];
                        ans = s;
                    }
                }
            }
            Console.WriteLine(ans);

            // ここから終了時に必ず行う処理を記述
            Console.Out.Flush();
        }
    }
}

namespace LibraryForDotnetCore.Basics
{
    public static class Basics
    {
        /// <summary>
        /// 拡張メソッド
        /// オブジェクトの型名をコンソールに出力する
        /// </summary>
        /// <param name="obj"></param>
        public static void WriteType(this Object obj)
        {
            /// 受け取った変数の型の正式名称をコンソールに表示
            Console.WriteLine(obj.GetType().FullName);
        }

        /// <summary>
        /// Ceil(a / b) を返す
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long CeilDivide(long a, long b)
        {
            return (a + b - 1) / b;
        }
    }

}
