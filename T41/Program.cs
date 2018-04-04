using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T41
{
    class Program
    {
        /// <summary>
        /// 演示c#里面的闭包
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            List<Func<int>> handler = GetHandlers();
            handler.ForEach(x=>System.Console.WriteLine(x().ToString()));
            Console.ReadKey();
        }

        private static List<Func<int>> GetHandlers()
        {
            List<Func<int>> lst = new List<Func<int>>();
            for (int i = 0; i < 3; i++)
            {
                lst.Add(()=>i);
            }
            return lst;
        }
    }
}
