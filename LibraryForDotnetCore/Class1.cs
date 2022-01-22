using System;

namespace LibraryForDotnetCore
{
    public class Class1
    {
    }

    public static class MyUtil
    {
        /// <summary>
        ///  競プロ用自作便利機能集
        /// </summary>
        /// <param name="obj"></param>
        public static void WriteType(this Object obj)
        {
            /// 受け取った変数の型の正式名称をコンソールに表示
            Console.WriteLine(obj.GetType().FullName);
        }

        public static long CeilDivide(long a, long b)
        {
            return (a + b - 1) / b;
        }
    }
}
