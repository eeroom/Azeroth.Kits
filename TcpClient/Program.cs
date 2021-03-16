using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcpClient {
    class Program {
        static void Main(string[] args) {
            var ip = System.Net.IPAddress.Parse("127.0.0.1");
            var port = 50001;
            Console.WriteLine("请输入要发送的消息");
            while (true) {
                var msg = Console.ReadLine();
                var client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);
                client.Connect(ip, port);
                using (var writer=new System.IO.StreamWriter(client.GetStream())) {
                    writer.Write(msg);
                }
            }
        }
    }
}
