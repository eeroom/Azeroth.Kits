using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UdpServer {
    class Program {
        static void Main(string[] args) {
            var ip = System.Net.IPAddress.Parse("127.0.0.1");
            var port = 50002;
            var server = new System.Net.Sockets.UdpClient(port,System.Net.Sockets.AddressFamily.InterNetwork);
            while (true) {
                var remote = new System.Net.IPEndPoint(System.Net.IPAddress.None, 0);
                var buffer= server.Receive(ref remote);
                System.Threading.ThreadPool.QueueUserWorkItem(ReciveMessage,Tuple.Create(buffer,remote));
            }
        }

        private static void ReciveMessage(object state) {
            var wrapper = state as Tuple<byte[], System.Net.IPEndPoint>;
            var msg= System.Text.UTF8Encoding.UTF8.GetString(wrapper.Item1);
            Console.WriteLine(msg);
        }
    }
}
