using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线程同步
{
    class Program
    {
        /*
        张三生产面包，生产出来就通知李四，自己则休息，生产一个大约20秒
        李四销售面包，销售完以后就通知张三，自己则休息，销售一个大约20秒
        */
        static void Main(string[] args)
        {
            var productionLock = new System.Threading.AutoResetEvent(true);
            var saleLock = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadPool.QueueUserWorkItem(ProductionHandler, Tuple.Create(productionLock,saleLock));
            System.Threading.ThreadPool.QueueUserWorkItem(SaleHandler, Tuple.Create(productionLock, saleLock));
            //productionLock.Set();
            Console.ReadLine();



        }

        private static void SaleHandler(object state)
        {
            var wrapper = (Tuple<System.Threading.AutoResetEvent, System.Threading.AutoResetEvent>)state;
            var productionLock = wrapper.Item1;
            var saleLock = wrapper.Item2;
            while (true)
            {
                if (saleLock.WaitOne())
                {
                    Console.WriteLine($"开始销售:{DateTime.Now.ToString("HH:mm:ss")}");
                    System.Threading.Thread.Sleep(new Random().Next(10, 20) * 1000);
                    Console.WriteLine($"完成销售:{DateTime.Now.ToString("HH:mm:ss")}");
                    productionLock.Set();
                    Console.WriteLine($"通知生产:{DateTime.Now.ToString("HH:mm:ss")}");
                }
            }
        }

        private static void ProductionHandler(object state)
        {
            var wrapper = (Tuple<System.Threading.AutoResetEvent, System.Threading.AutoResetEvent>)state;
            var productionLock = wrapper.Item1;
            var saleLock = wrapper.Item2;
            while (true)
            {
                if (productionLock.WaitOne())
                {
                    Console.WriteLine($"开始生产:{DateTime.Now.ToString("HH:mm:ss")}");
                    System.Threading.Thread.Sleep(new Random().Next(10,20)*1000);
                    Console.WriteLine($"完成生产:{DateTime.Now.ToString("HH:mm:ss")}");
                    saleLock.Set();
                    Console.WriteLine($"通知销售:{DateTime.Now.ToString("HH:mm:ss")}");
                }
            }
        }
    }
}
