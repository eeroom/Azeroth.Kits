using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDemo {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("主线程开始");
            var lst = System.Linq.Enumerable.Range(1, 1000);
            foreach (var item in lst) {
                var rt2=  Method1(item);
                //Method2(item);
                //Method3(item);
            }
            Console.WriteLine("主线程跑完");
            Console.ReadLine();
        }

        /// <summary>
        /// 效果：1000个任务差不多同时执行完，每个任务中途都等待了5s
        ///不会阻塞当前的调用线程，会让其回到线程池执行其他任务，
        ///代码本身这里任然阻塞，等待后续Task体系安排线程执行await下一行的代码
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static async Task<int> Method1(int x ) {
            int fid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var fss = DateTime.Now.Second;
            //不会阻塞当前的调用线程，会让其回到线程池执行其他任务，
            //代码本身这里任然阻塞，等待后续Task体系安排线程执行await下一行的代码
            await Task.Delay(5 * 1000);
            Console.WriteLine($"value={x},fId={fid},sId={System.Threading.Thread.CurrentThread.ManagedThreadId},fss={fss},sss={DateTime.Now.Second}");
            return await Task.FromResult(x);
        }

        /// <summary>
        /// 效果：1000个任务挨个执行完，每个任务中途没有等待
        ///Task.Delay()，调用后只是一个task对象，因为async方法被编译器改造后，遇到await会return(异步方法的状态机原理),不会阻塞当前线程
        ///所以fss和sss的值是一样的
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static void Method2(int x) {
            int fid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var fss = DateTime.Now.Second;
            //Task.Delay()，调用后只是一个task对象，
            //因为async方法被编译器改造后，遇到await会return(异步方法的状态机原理),不会阻塞当前线程
            //所以fss和sss的值是一样的
            Task.Delay(5 * 1000);
            //var rt = Task.Delay(5 * 1000);
            Console.WriteLine($"value={x},fId={fid},sId={System.Threading.Thread.CurrentThread.ManagedThreadId},fss={fss},sss={DateTime.Now.Second}");
        }

        /// <summary>
        /// 效果：1000个任务挨个执行完，每个任务中途都等待了5s
        /// 1000个任务分批次（批次大小受线程池大小影响）执行完，每个任务中途都等待了5s
        ///阻塞当前的调用线程
        ///代码本身这里任然阻塞
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static void Method3(int x) {
            int fid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var fss = DateTime.Now.Second;
            var rt = Task.Delay(5 * 1000);
            //阻塞当前的调用线程
            //代码本身这里任然阻塞
            rt.Wait();
            Console.WriteLine($"value={x},fId={fid},sId={System.Threading.Thread.CurrentThread.ManagedThreadId},fss={fss},sss={DateTime.Now.Second}");
        }

        /// <summary>
        /// 效果：1000个任务分批次（批次大小受线程池大小影响）执行完，每个任务中途都等待了5s
        ///阻塞当前的调用线程
        ///代码本身这里任然阻塞
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static void Method4(int x) {
            int fid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var fss = DateTime.Now.Second;
            var rt = Task.Delay(5 * 1000);
            //阻塞当前的调用线程
            //代码本身这里任然阻塞
            rt.Wait();
            Console.WriteLine($"value={x},fId={fid},sId={System.Threading.Thread.CurrentThread.ManagedThreadId},fss={fss},sss={DateTime.Now.Second}");
        }
    }
}
