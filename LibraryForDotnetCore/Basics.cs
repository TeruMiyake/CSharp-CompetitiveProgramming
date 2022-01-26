using System;

namespace LibraryForDotnetCore.Basics
{
    public static class Util
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
