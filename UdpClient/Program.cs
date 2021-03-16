using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UdpClient {
    class Program {
        static void Main(string[] args) {
            var ip = System.Net.IPAddress.Parse("127.0.0.1");
            var port = 50002;
            Console.WriteLine("请输入要发送的消息");
            var client = new System.Net.Sockets.UdpClient(System.Net.Sockets.AddressFamily.InterNetwork);
            while (true) {
                var msg = Console.ReadLine();
                client.Connect(ip, port);
                var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(msg);
                client.Send(buffer, buffer.Length);
            }
        }
    }
}
