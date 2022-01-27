using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace BouncyCastleCrypto
{
    public class GpgUtil
    {
        /// <summary>
        /// 加密并签名
        /// 使用接受方的公钥进行加密
        /// 使用发送方的私钥进行签名
        /// 先压缩，再加密，再签名
        /// </summary>
        /// <param name="kp"></param>
        /// <param name="cfg"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputStream">普通的stream,或者Org.BouncyCastle.Bcpg.ArmoredOutputStream（如果使用加密文件使用ASCII）</param>
        public static void EncryptAndSign(System.IO.FileInfo inputFile,System.IO.Stream outputStream, GpgKeyPair kp, GpgEncryptSignCfg cfg)
        {
            var sr = new Org.BouncyCastle.Security.SecureRandom();
            var pgpEncryptedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataGenerator(cfg.SymmetricKeyAlgorithmTag,cfg.IntegrityProtected, sr);
            pgpEncryptedDataGenerator.AddMethod(kp.PublickKey);
            var pgpCompressedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpCompressedDataGenerator(cfg.CompressionAlgorithmTag);
            var pgpLiteralDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralDataGenerator();

            using (var fs = inputFile.OpenRead())
            using (var outputStreamEncrypted = pgpEncryptedDataGenerator.Open(outputStream, new byte[cfg.BufferSize]))
            using (var outputStreamEncryptedCompressed = pgpCompressedDataGenerator.Open(outputStreamEncrypted))
            using (var outputStreamEncryptedCompressedLiteral = pgpLiteralDataGenerator.Open(outputStreamEncryptedCompressed,
                Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralData.Binary, inputFile.Name, inputFile.Length,inputFile.LastWriteTime))
            {
                var pgpSignatureGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureGenerator(kp.PrivateKeySecreted.PublicKey.Algorithm,
                Org.BouncyCastle.Bcpg.HashAlgorithmTag.Sha256);
                pgpSignatureGenerator.InitSign(Org.BouncyCastle.Bcpg.OpenPgp.PgpSignature.BinaryDocument, kp.PrivateKey);
                var userId = kp.PrivateKeySecreted.PublicKey.GetUserIds().Cast<string>().First();
                var pgpSignatureSubpacketGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator();
                pgpSignatureSubpacketGenerator.SetSignerUserId(cfg.IsCritical, userId);
                pgpSignatureGenerator.SetHashedSubpackets(pgpSignatureSubpacketGenerator.Generate());
                pgpSignatureGenerator.GenerateOnePassVersion(cfg.IsNested).Encode(outputStreamEncryptedCompressedLiteral);

                int dataLenght = 0;
                var buffer = new byte[cfg.BufferSize];
                while ((dataLenght=fs.Read(buffer,0,buffer.Length))>0)
                {
                    outputStreamEncryptedCompressedLiteral.Write(buffer, 0, dataLenght);
                    pgpSignatureGenerator.Update(buffer, 0, dataLenght);
                }
                pgpSignatureGenerator.Generate().Encode(outputStreamEncryptedCompressedLiteral);

            }
        }

        /// <summary>
        /// 加密文件
        /// 使用接收方的公钥加密文件
        /// 先压缩，再加密，
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputStream">普通的stream,或者Org.BouncyCastle.Bcpg.ArmoredOutputStream（如果使用加密文件使用ASCII）</param>
        /// <param name="pubKey"></param>
        /// <param name="cfg"></param>
        public static void Encrypt(System.IO.FileInfo inputFile, System.IO.Stream outputStream, System.IO.Stream publickKeyStream, GpgEncryptSignCfg cfg)
        {
            
            var sr = new Org.BouncyCastle.Security.SecureRandom();
            var pgpEncryptedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataGenerator(cfg.SymmetricKeyAlgorithmTag, cfg.IntegrityProtected, sr);
            var pubKey = GpgKeyPair.ReadPublicKey(publickKeyStream);
            pgpEncryptedDataGenerator.AddMethod(pubKey);
            var pgpCompressedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpCompressedDataGenerator(cfg.CompressionAlgorithmTag);
            var pgpLiteralDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralDataGenerator();

            using (var fs = inputFile.OpenRead())
            using (var outputStreamEncrypted = pgpEncryptedDataGenerator.Open(outputStream, new byte[cfg.BufferSize]))
            using (var outputStreamEncryptedCompressed = pgpCompressedDataGenerator.Open(outputStreamEncrypted))
            using (var outputStreamEncryptedCompressedLiteral = pgpLiteralDataGenerator.Open(outputStreamEncryptedCompressed,
                Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralData.Binary, inputFile.Name, inputFile.Length, inputFile.LastWriteTime))
            {
                int dataLenght = 0;
                var buffer = new byte[cfg.BufferSize];
                while ((dataLenght = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStreamEncryptedCompressedLiteral.Write(buffer, 0, dataLenght);
                }
            }
        }

        public static bool Decrypt(System.IO.Stream inputStream,System.IO.Stream outputStream,System.IO.Stream praKeyStream,string praKeyPwd)
        {
            var pgpEncryptedDatalist = GetEncryptedDatalist(inputStream);
            var pgpEncryptedDataWrapper = Decrypt(pgpEncryptedDatalist, praKeyStream, praKeyPwd);
            if (pgpEncryptedDataWrapper.Item1.IsIntegrityProtected() && !pgpEncryptedDataWrapper.Item1.Verify())
                return false;
            var obj = pgpEncryptedDataWrapper.Item2.NextPgpObject();
            if (obj is PgpCompressedData)
                Decrypt((PgpCompressedData)obj, outputStream);
            else if (obj is PgpLiteralData)
                Decrypt((PgpLiteralData)obj, outputStream);
            else if (obj is PgpOnePassSignatureList)
                throw new ArgumentException("加密文件包含签名，请使用其它合适的解密方法");
            else
                throw new ArgumentException("不支持的加密文件");
            return true;
        }

        private static void Decrypt(PgpLiteralData objLiteral, Stream outputStream)
        {
            using (var ms=objLiteral.GetInputStream())
            {
                Org.BouncyCastle.Utilities.IO.Streams.PipeAll(ms, outputStream);
            }
        }

        private static void Decrypt(PgpCompressedData objCompressed, Stream outputStream)
        {
            using (var compressedStream=objCompressed.GetDataStream())
            {
                var factory = new PgpObjectFactory(compressedStream);
                var obj = factory.NextPgpObject();
                if (obj is PgpOnePassSignatureList)
                    Decrypt((PgpLiteralData)factory.NextPgpObject(), outputStream);
                else
                    Decrypt((PgpLiteralData)obj, outputStream);
            }
        }

        private static Tuple< PgpPublicKeyEncryptedData, PgpObjectFactory> Decrypt(PgpEncryptedDataList pgpEncryptedDatalist, Stream privateKeyStream, string praKeyPwd)
        {
            var pkstream = Org.BouncyCastle.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(privateKeyStream);
            var bundle = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSecretKeyRingBundle(pkstream);

            foreach (PgpPublicKeyEncryptedData encryptedData in pgpEncryptedDatalist.GetEncryptedDataObjects())
            {
                var privateKey = bundle.GetSecretKey(encryptedData.KeyId)?.ExtractPrivateKey(praKeyPwd.ToCharArray());
                if (privateKey == null)
                    continue;
                using (var tmp=encryptedData.GetDataStream(privateKey))
                {
                    return Tuple.Create(encryptedData, new PgpObjectFactory(tmp));
                }
            }
            throw new ArgumentException("未能正常读取到加密文件内容，请检查文件及密钥");
        }

        private static Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataList GetEncryptedDatalist(Stream inputStream)
        {
            var dstream = Org.BouncyCastle.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(inputStream);
            var pgpObjFacoty = new Org.BouncyCastle.Bcpg.OpenPgp.PgpObjectFactory(dstream);
            var obj = pgpObjFacoty.NextPgpObject();
            var tmp = obj as Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataList;
            if (tmp != null)
                return tmp;
            return (Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataList)pgpObjFacoty.NextPgpObject();
        }
    }
}
