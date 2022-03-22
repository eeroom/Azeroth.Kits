using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncyCastleCrypto
{
    class Program
    {
        static void Main(string[] args)
        {
            //Jiami();
            JiamiQianming();
            Console.ReadKey();
        }

        private static void JiamiQianming() {
            var kp = new GpgKeyPair(System.IO.File.Open("d:/eeroom.asc", System.IO.FileMode.Open)
                , System.IO.File.Open("d:/eeroom.gpg", System.IO.FileMode.Open), "123456");
            using (var fs = System.IO.File.Open("e:/wifi密码-加密-签名.txt.gpg", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite)) {
                GpgUtil.EncryptAndSign(new System.IO.FileInfo("d:/wifi密码.txt"), fs
                    , kp
                    , new GpgEncryptSignCfg());
            }
            Console.WriteLine("加密并签名成功");
        }

        private static void Jiami() {
            using (var fs = System.IO.File.Open("e:/wifi密码-加密.txt.gpg", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite)) {
                GpgUtil.Encrypt(new System.IO.FileInfo("d:/wifi密码.txt"), fs
                    , System.IO.File.Open("d:/eeroom.asc", System.IO.FileMode.Open)
                    , new GpgEncryptSignCfg());
            }
            Console.WriteLine("加密成功");
        }
    }
}
