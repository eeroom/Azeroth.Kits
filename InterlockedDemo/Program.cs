using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterlockedDemo {
    class Program {
        static int a = 0;
        static void Main(string[] args) {
            
            if(System.Threading.Interlocked.Exchange(ref a, 1) == 0) {
                Console.WriteLine("第一次进入");
            }
            if(System.Threading.Interlocked.Exchange(ref a, 1) == 0) {
                Console.WriteLine("第二次进入");
            }

            Console.WriteLine("hello world");
            Console.ReadKey();
        }
    }
}
