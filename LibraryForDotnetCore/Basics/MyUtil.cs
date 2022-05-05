using System;
using System.Collections.Generic;

namespace LibraryForDotnetCore.Basics
{
    public static class MyUtil
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
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
        /// <summary>
        /// 汎用的な二分探索（めぐる式二分探索）
        /// isOK() の結果が単調性を満たす必要がある
        /// </summary>
        /// <param name="arr">IList<object></param>
        /// <param name="ng">決して条件が満たされないような arr の index</param>
        /// <param name="ok">常に条件が満たされるような arr の index</param>
        /// <param name="isOK">bool isOK(コレクション, index) index が条件を満たすか</param>
        /// <returns>isOK() が true となるギリギリの long index</returns>
        public static int BinarySearch<T>(IList<T> arr, int ng, int ok, Func<IList<T>, int, bool> isOK)
        {
            // ok と ng のどちらが大きいか分からないことを考慮
            while (Math.Abs(ok - ng) > 1)
            {
                int mid = (ok + ng) / 2;
                if (isOK(arr, mid)) ok = mid;
                else ng = mid;
            }
            return ok;
        }
        /// <summary>
        /// 狭義最長増加部分列（の長さ）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="inf"></param>
        /// <param name="minusinf"></param>
        /// <returns></returns>
        public static int LIS<T>(IList<T> arr, T inf, T minusinf) where T : IComparable
        {
            int N = arr.Count;
            T[] dp = new T[N + 1];
            dp[0] = minusinf;
            for (int i = 0; i < N; i++) dp[i + 1] = inf;

            for (int i = 0; i < N; i++)
            {
                T num = arr[i];
                // dp[j] < num < dp[j+1] となるような j を見つけ、dp[j+1] = num とする
                bool isOK(IList<T> _dp, int key)
                {
                    return (_dp[key].CompareTo(num) < 0);
                }
                int j = MyUtil.BinarySearch(dp, N + 1, 0, isOK);
                dp[j + 1] = num;
            }

            int ret = -1;
            // dp[0] は必ず 0 であり、それ以降は構築可能な LIS 長まで INF 未満となっている
            // よって、INF 未満の数 -1 を返せばいい
            for (int i = 0; i < N + 1; i++)
                if (dp[i].CompareTo(inf) != 0) ret++;
            return ret;
        }
    }

}