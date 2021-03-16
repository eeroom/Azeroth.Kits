using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcpServer {
    class Program {
        static void Main(string[] args) {
            //var ip = System.Net.IPAddress.Parse("127.0.0.1");
            var port = 50001;
            var server = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
            server.Start();
            Console.WriteLine("tcp监听已经启动");
            while (true) {
                var client= server.AcceptTcpClient();
                System.Threading.ThreadPool.QueueUserWorkItem(ReciveMessage, client);
            }
        }

        private static void ReciveMessage(object state) {
            var client = state as System.Net.Sockets.TcpClient;
            using (var reader=new System.IO.StreamReader(client.GetStream())) {
                var msg= reader.ReadToEnd();
                Console.WriteLine(msg);
            }
        }
    }
}
