using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wrapper7z {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("请输入lib7z的文件路径");
            string lib7zpath = Console.ReadLine();
            while (true)
            {
                Console.WriteLine("请输入被解压的文件");
                string zipfilepath = Console.ReadLine();
                Console.WriteLine("请输入输出的目录");
                string outputPath = Console.ReadLine();
                var w7 = new Wrapper7z(lib7zpath, zipfilepath);
                ulong total = 0;
                w7.BeginDecompress += x => total = x;
                w7.OnDecompress += x => Console.WriteLine($"解压进度{Math.Round(100.0d * x / total, MidpointRounding.ToEven)}");
                w7.Decompress(outputPath);
                Console.WriteLine("完成解压");
            }
        }
    }
}
