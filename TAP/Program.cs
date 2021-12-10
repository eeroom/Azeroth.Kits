using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAP {
    class Program {
        static void Main(string[] args) {
            Handler1(11);
            Console.WriteLine("hello world");
            Console.ReadLine();

        }

        async static Task Handler1(int vv)
        {
            Console.WriteLine("进入Handler1");
            await Task.Delay(2000);
            Console.WriteLine("执行完Task.Delay(2000)");
            var value = await Handler2(vv);
            Console.WriteLine("执行完Handler2");
        }

        static async Task<string> Handler2(int vv)
        {
            Console.WriteLine("进入Handler2");
            await Task.Delay(1000);
            Console.WriteLine("执行完Task.Delay(1000)");
            var rt = vv.ToString();
            return rt;
        }
    }
}
