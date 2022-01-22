using System;
using System.IO;

namespace AtCoderTemplate
{
    class LocalEntryPoint
    {

        static void Main()
        {
            LocalMain();
        }
        static void LocalMain()
        {
            // ソリューションが保管されているフォルダをフルパスで設定
            string projectName = Path.GetFileName(AppDomain.CurrentDomain.FriendlyName);
            string projectPath = "D:\\csharp\\CSharp-CompetitiveProgramming\\" + projectName + "\\";

            string inputDir = projectPath + "input\\";
            string outputDir = projectPath + "output\\";

            // 実行
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string[] inputFilePaths = Directory.GetFiles(inputDir, "*.in");

            foreach (string inputFilePath in inputFilePaths)
            {
                // 標準出力を退避
                var stdout = Console.Out;

                // 入出力フォルダの設定
                // input/*.in -> 処理 -> output/*.out
                string inputFileName = Path.GetFileName(inputFilePath);
                string outputFileName = inputFileName.Substring(0, inputFileName.Length - 3) + ".out";
                string outputFilePath = outputDir + outputFileName;
                using (StreamReader reader = new StreamReader(inputFilePath))
                {
                    Console.SetIn(new StreamReader(inputFilePath));
                    using (StreamWriter writer = new StreamWriter(outputFilePath, false))
                    {
                        Console.SetOut(writer);

                        sw.Restart();
                        var solver = new Solver();
                        solver.solve();

                        // 標準出力に戻す
                        Console.SetOut(stdout);
                    }
                }

                // 標準出力に戻す
                Console.SetOut(stdout);

                Console.WriteLine($"Done {Path.GetFileName(inputFileName)} -> {Path.GetFileName(outputFileName)} in {sw.ElapsedMilliseconds} ms.");
            }
        }
    }
}
