using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yield教程
{
    class App
    {
        static void Main(string[] args)
        {
            foreach (var item in getNames())
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();


        }

        static IEnumerable<string> getNames()
        {
            yield return "张三";
            yield return "李四";
            yield return "aaa";
        }

        ////需要C#8.0才支持
        //async static IAsyncEnumerable<string> getNamesV2()
        //{
        //    yield return await Task.FromResult("张三");
        //    yield return await Task.FromResult("张三2");
        //    yield return await Task.FromResult("张三2");
        //}

        ////需要C#8.0才支持
        //async static void ProcessNames()
        //{
        //    await foreach (var item in getNamesV2())
        //    {
        //        Console.WriteLine(item);
        //    }
        //    Console.ReadLine();
        //}
    }


}
