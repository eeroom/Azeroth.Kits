using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAP
{
    class Program
    {
        static void Main(string[] args)
        {
            /// 传统的代码遇到需要长时间执行的方法的时候，为了不阻塞当前线程通常有以下做法：
            /// 1、另起一个线程B，执行该方法，弊端：线程B仍然被阻塞，造成浪费
            /// 2、该方法有对应APM模式的实现，调用的时候传一个回调方法。弊端：意大利面条模式，后执行的代码要写在前面，如果回调多层，导致代码套娃
            /// 3、该方法有对应EAM模式的实现，调用之前注册一个事件回调方法。弊端：后执行的代码要写在前面
            /// TAP模式的优点，不会阻塞当前线程，场景：winform程序，如果主线程阻塞，导致界面无响应，如果是TAP模式的代码，主线程就不会被阻塞，总是能响应用户的操作
            /// web程序，每个执行线程都不阻塞，iis可以在短时间内接受更多的请求，而不出现503的情况，iis可以接受更高的并发峰值
            /// 但是每个请求的执行时间不会缩短（该多久还是要多久），考虑到TAP模式导致的线程切换，耗时还要增加一点
            /// 另一个优点，代码顺序和普通代码一致，不存在后执行的代码要写在前面的情况
            Handler1(11);
            Console.WriteLine("hello world");
            Console.ReadLine();

        }

        public async static Task Handler1(int vv)
        {
            Console.WriteLine("进入Handler1");
            await Task.Delay(2000);
            Console.WriteLine("执行完Task.Delay(2000)");
            var value = await Handler2(vv);
            Console.WriteLine("执行完Handler2");
        }

        public static async Task<string> Handler2(int vv)
        {
            Console.WriteLine("进入Handler2");
            await Task.Delay(1000);
            Console.WriteLine("执行完Task.Delay(1000)");
            var rt = vv.ToString();
            return rt;
        }

        /// <summary>
        /// Handler1的等价代码
        /// </summary>
        /// <param name="vv"></param>
        /// <returns></returns>
        public static Task Handler1Clone(int vv)
        {
            Handler1StateMachine h1s = new Handler1StateMachine();
            h1s.vv = vv;
            h1s.state = -1;
            h1s.builder = System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create();
            h1s.builder.Start(ref h1s);
            return h1s.builder.Task;
        }
    }

    class Handler1StateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
    {
        /// <summary>
        /// 对应原方法的参数，每个参数建一个对应的字段，原方法里面创建其对应StateMachine的时候，对这些字段一一赋值
        /// </summary>
        public int vv;
        /// <summary>
        /// 这个build的类型和方法的返回值对应，原方法里面创建其对应StateMachine的时候，根据方法返回值类型创建这个值
        /// </summary>
        public System.Runtime.CompilerServices.AsyncTaskMethodBuilder builder;
        /// <summary>
        /// 内部辅助字段，用于每次builder.AwaitUnsafeOnCompleted后，再回到MoveNext方法的时候，执行恰当部分的代码
        /// </summary>
        public int state;
        /// <summary>
        /// 对应原方法内await语句对应的TaskAwaiter
        /// </summary>
        System.Runtime.CompilerServices.TaskAwaiter awaiterDelay2000;
        /// <summary>
        /// 对应原方法内await语句对应的TaskAwaiter
        /// </summary>
        System.Runtime.CompilerServices.TaskAwaiter<string> awaiterHandler2;
        /// <summary>
        /// 对应原方法内定义的变量，（和参数类似，只是参数定义在原方法头部）
        /// </summary>
        public string value;

        /// <summary>
        /// 官方的等价翻译
        /// 翻译等价代码的技巧：
        /// 按照原代码的顺序从上往下写，遇到await语句，就转成xxx.GetAwaiter(),并且定义一个对应的TaskAwaiter类型字段
        /// 然后进入IsCompleted判断,每个await语句对应的判断块都类似，不同点：把state设为不同值，这里为方便阅读代码，按照代码顺序递增state的值
        /// 关键方法：builder.AwaitUnsafeOnCompleted；这个方法不会阻塞，当前方法所在线程直接return，去执行其它任务，类库内部帮我们后续再回调到MoveNext
        /// 然后定义一个标签位置，后续代码写在标签之后
        /// </summary>
        public void MoveNext()
        {
            //这里就是配合下面翻译的代码，做的配合工作
            switch (this.state)
            {
                case 0:
                    goto block0;
                case 1:
                    goto block1;
                default:
                    break;
            }
            //翻译等价代码的技巧：
            //按照原代码的顺序从上往下写，遇到await语句，就转成xxx.GetAwaiter(),并且定义一个对应的TaskAwaiter类型字段
            //然后进入IsCompleted判断,每个await语句对应的判断块都类似，不同点：把state设为不同值，这里为方便阅读代码，按照代码顺序递增state的值
            //然后定义一个标签位置，后续代码写在标签之后
            Console.WriteLine("进入Handler1");
            this.awaiterDelay2000 = Task.Delay(2000).GetAwaiter();
            if (!this.awaiterDelay2000.IsCompleted)
            {
                this.state = 0;
                Handler1StateMachine h1s = this;
                this.builder.AwaitUnsafeOnCompleted(ref this.awaiterDelay2000, ref h1s);
                return;
            }
            block0:
            this.awaiterDelay2000.GetResult();
            Console.WriteLine("执行完Task.Delay(2000)");
            this.awaiterHandler2 = Program.Handler2(this.vv).GetAwaiter();
            if (!this.awaiterHandler2.IsCompleted)
            {
                this.state = 1;
                Handler1StateMachine h1s = this;
                this.builder.AwaitUnsafeOnCompleted(ref this.awaiterHandler2, ref h1s);
                return;
            }
            block1:
            var rt = this.awaiterHandler2.GetResult();
            this.value = rt;
            Console.WriteLine("执行完Handler2");
            this.state = -2;
            this.builder.SetResult();
        }
        /// <summary>
        /// 简单粗暴翻译
        /// 翻译等价代码的技巧：
        /// 按照原代码的顺序从上往下写，遇到await语句，就转成xxx.GetAwaiter(),并且定义一个对应的TaskAwaiter类型字段
        /// 不做IsCompleted，全部利用builder.AwaitUnsafeOnCompleted方法结束当前方法，释放当前线程
        /// 弊端：遇到Task.FromResult这种代码，就会有点浪费，因为没必要builder.AwaitUnsafeOnCompleted
        /// </summary>
        public void MoveNext2()
        {
            //翻译等价代码的技巧：
            //按照原代码的顺序从上往下写，遇到await语句，就转成xxx.GetAwaiter(),并且定义一个对应的TaskAwaiter类型字段
            //不做IsCompleted，全部利用builder.AwaitUnsafeOnCompleted方法结束当前方法，释放当前线程
            //
            Handler1StateMachine h1s = this;
            switch (this.state)
            {
                case 0:
                    Console.WriteLine("进入Handler1");
                    this.awaiterDelay2000 = Task.Delay(2000).GetAwaiter();
                    this.state = 1;
                    this.builder.AwaitUnsafeOnCompleted(ref this.awaiterDelay2000, ref h1s);
                    return;
                case 1:
                    this.awaiterDelay2000.GetResult();
                    Console.WriteLine("执行完Task.Delay(2000)");
                    this.awaiterHandler2 = Program.Handler2(this.vv).GetAwaiter();
                    this.state = 2;
                    this.builder.AwaitUnsafeOnCompleted(ref this.awaiterHandler2, ref h1s);
                    return;
                case 2:
                    var rt = this.awaiterHandler2.GetResult();
                    this.value = rt;
                    Console.WriteLine("执行完Handler2");
                    this.state = 3;
                    this.builder.SetResult();
                    return;
                default:
                    break;
            }
        }

        public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
        {

        }
    }
}
