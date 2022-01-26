using LibraryForDotnetCore.Basics;
using System;
using System.Collections.Generic;
using System.Globalization; // CultureInfo
using System.IO; // input.txt/output.txt IO 用
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text; // StringBuilder

namespace Problem_ARC133_B
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
            if (isOnRealStage)
            {
                scanner = new StreamScanner(Console.OpenStandardInput());
                var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
                Console.SetOut(writer);
            }
            else
            {
                scanner = new StreamScanner();
            }
            // ここからメイン処理を記述

            int N = ri();
            List<int> As = new List<int>();
            for (int i = 0; i < N; i++) As.Add(ri());

            int ans = -1;
            int now = 0;
            for (int i = 0; i < N; i++)
            {
                int a = As[i];
                if (now > a)
                {
                    ans = i - 1;
                    break;
                }
                now = a;
            }
            if (ans == -1) ans = N - 1;

            int deletetarget = As[ans];
            As.RemoveAll(a => a == deletetarget);

            string ansstr = string.Join(" ", As);

            Console.WriteLine(ansstr);


            // ここから終了時に必ず行う処理を記述
            Console.Out.Flush();
        }
        /// <summary>
        /// ジェネリック狭義最長増加部分列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="inf"></param>
        /// <param name="minusinf"></param>
        /// <returns></returns>
        [System.Obsolete("Expander を実装したら MyUtil -> Util とする")]
        int LIS<T>(IList<T> arr, T inf, T minusinf) where T : IComparable
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
        // レギュラーメンバー
        const int INF = 1_000_000_007;
        const long LINF = (long)1 << 62;
        // 入出力
        // Space を除く通常文字（U+0021:'!' ~ U+007E:'~'）が続く限り読み込む
        public StreamScanner scanner;
        string rs() { return scanner.Scan(); }
        // Space を除く通常文字（U+0021:'!' ~ U+007E:'~'）以外は無視
        char rc() { return scanner.Char(); }
        int ri() { return scanner.Integer(); }
        long rl() { return scanner.Long(); }

        /// <summary>
        /// 入力高速化 以下のサイトを元にアレンジ
        /// https://qiita.com/Camypaper/items/de6d576fe5513743a50e
        /// ローカル環境では StreamScanner() からインスタンス化
        ///  -> ReadOnLocal() 内の Console.In.Read() を使う
        /// 提出時は StreamScanner(stream) でインスタンス化
        ///  -> ReadOnRealStage() 内の str.Read() を使う
        /// </summary>
        public class StreamScanner
        {
            public delegate int PlatformReader();
            public PlatformReader PlatformRead;
            public StreamScanner()
            {
                PlatformRead = ReadOnLocal;
            }
            public StreamScanner(Stream stream)
            {
                str = stream;
                PlatformRead = ReadOnRealStage;
            }
            private readonly Stream str;
            private readonly byte[] buf = new byte[1024];
            private int len, ptr;
            public bool isEof = false;
            public bool IsEndOfStream { get { return isEof; } }
            private byte read()
            {
                if (isEof) throw new EndOfStreamException();
                if (ptr >= len)
                {
                    ptr = 0;
                    // プラットフォーム毎に対応した Read() を呼び出す
                    if ((len = PlatformRead()) <= 0)
                    {
                        isEof = true;
                        return 0;
                    }
                }
                return buf[ptr++];
            }
            private int ReadOnLocal()
            {
                char[] cbuf = new char[1024];
                for (int i = 0; i < 1024; i++) cbuf[i] = (char)buf[i];
                int ret = Console.In.Read(cbuf, 0, 1024);
                for (int i = 0; i < 1024; i++) buf[i] = (byte)cbuf[i];
                return ret;
            }
            private int ReadOnRealStage()
            {
                return str.Read(buf, 0, 1024);
            }
            public char Char()
            {
                byte b = 0;
                do b = read();
                while (b < 33 || 126 < b);
                return (char)b;
            }
            public string Scan()
            {
                var sb = new StringBuilder();
                // Space を除く通常文字（U+0021:'!' ~ U+007E:'~'）が続く限り読み込む
                for (var b = Char(); b >= 33 && b <= 126; b = (char)read())
                    sb.Append(b);
                return sb.ToString();
            }
            public long Long()
            {
                long ret = 0; byte b = 0; var ng = false;
                do b = read();
                while (b != '-' && (b < '0' || '9' < b));
                if (b == '-') { ng = true; b = read(); }
                for (; true; b = read())
                {
                    if (b < '0' || '9' < b)
                        return ng ? -ret : ret;
                    else ret = ret * 10 + b - '0';
                }
            }
            public int Integer() { return (int)Long(); }
            public double Double() { return double.Parse(Scan(), CultureInfo.InvariantCulture); }
        }
    }
}

namespace LibraryForDotnetCore.Basics
{
    public static class MyUtil
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
    }

}