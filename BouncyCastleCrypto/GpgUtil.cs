using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncyCastleCrypto
{
    public class GpgUtil
    {
        /// <summary>
        /// 加密并签名
        /// </summary>
        /// <param name="kp"></param>
        /// <param name="cfg"></param>
        /// <param name="InputFile"></param>
        /// <param name="OutputStream"></param>
        public static void EncryptAndSign(System.IO.FileInfo InputFile,System.IO.Stream OutputStream, GpgKeyPair kp, GpgEncryptSignCfg cfg)
        {
            var pgpSignatureGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureGenerator(kp.PrivateKeySecreted.PublicKey.Algorithm,
                Org.BouncyCastle.Bcpg.HashAlgorithmTag.Sha256);
            pgpSignatureGenerator.InitSign(Org.BouncyCastle.Bcpg.OpenPgp.PgpSignature.BinaryDocument, kp.PrivateKey);
            var userId= kp.PrivateKeySecreted.PublicKey.GetUserIds().Cast<string>().First();
            var pgpSignatureSubpacketGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator();
            pgpSignatureSubpacketGenerator.SetSignerUserId(false,userId);
            pgpSignatureGenerator.SetHashedSubpackets(pgpSignatureSubpacketGenerator.Generate());

            var sr = new Org.BouncyCastle.Security.SecureRandom();
            var pgpEncryptedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataGenerator(cfg.SymmetricKeyAlgorithmTag,cfg.IntegrityProtected, sr);
            pgpEncryptedDataGenerator.AddMethod(kp.PublickKey);
            var pgpCompressedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpCompressedDataGenerator(cfg.CompressionAlgorithmTag);
            var pgpLiteralDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralDataGenerator();

            using (var fs = InputFile.OpenRead())
            using (var outputStreamEncrypted = pgpEncryptedDataGenerator.Open(OutputStream, new byte[cfg.BufferSize]))
            using (var outputStreamEncryptedCompressed = pgpCompressedDataGenerator.Open(outputStreamEncrypted))
            using (var outputStreamEncryptedCompressedLiteral = pgpLiteralDataGenerator.Open(outputStreamEncryptedCompressed,
                Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralData.Binary, InputFile.Name, InputFile.Length,InputFile.LastWriteTime))
            {
                pgpSignatureGenerator.GenerateOnePassVersion(false).Encode(outputStreamEncryptedCompressedLiteral);
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

        public static void Encrypt(System.IO.FileInfo inputFile, System.IO.Stream outputStream, Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKey pubKey, GpgEncryptSignCfg cfg)
        {
            var sr = new Org.BouncyCastle.Security.SecureRandom();
            var pgpEncryptedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataGenerator(cfg.SymmetricKeyAlgorithmTag, cfg.IntegrityProtected, sr);
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
    }
}
