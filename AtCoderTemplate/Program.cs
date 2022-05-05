using System;
using System.Collections.Generic;
using System.Globalization; // CultureInfo
using System.IO; // input.txt/output.txt IO 用
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text; // StringBuilder
using LibraryForDotnetCore.Basics; // MyUtil
using LibraryForDotnetCore.MySTL;
namespace AtCoderTemplate
{
    internal class Program
    {
        static void Main()
        {
            Solver sol = new Solver();
            // 入出力の切り替え、テスト/本番切り替え、SourceExpander 実行など
            sol.Prepare();
            sol.Solve();
            // Prepare() で設定した内容のリセット
            sol.CleanUp();
        }
    }

    public class Solver
    {
        public void Solve()
        {
            // write your code here
        }
        #region TEMPLATE
        /// <summary>
        /// 標準入力・標準出力の設定など、Solve() の事前準備を行う
        /// </summary>
        /// <param name="memberName"></param>
        public void Prepare([CallerMemberName] string memberName = "")
        {
            // ローカル環境では LocalMain() から呼び出されることを利用して場合分け
            if (memberName != "LocalMain")
            {
                // 本番環境
                scanner = new StreamScanner(Console.OpenStandardInput());
                var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
                Console.SetOut(writer);
            }
            else
            {
                // ローカル環境
                scanner = new StreamScanner();
                // https://github.com/kzrnm/SourceExpander
                SourceExpander.Expander.Expand();
            }
        }
        /// <summary>
        /// 書き換えた標準出力の設定など、事後処理を行う
        /// </summary>
        public void CleanUp()
        {
            Console.Out.Flush();
        }
        // レギュラーメンバー
        const int INF = 1_000_000_007;
        const long LINF = (long)1 << 62;
        // 出力
        void wl(string str) { Console.WriteLine(str); }
        // 入力
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
        #endregion
    }
}