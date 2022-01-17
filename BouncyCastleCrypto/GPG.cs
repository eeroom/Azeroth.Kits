using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncyCastleCrypto
{
    public class GPG
    {
        /// <summary>
        /// 加密并签名
        /// </summary>
        /// <param name="parameter"></param>
        public static void EncryptAndSign(EncryptInputGPG parameter)
        {
            var pgpSignatureGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureGenerator(parameter.PrivateKeySecreted.PublicKey.Algorithm, Org.BouncyCastle.Bcpg.HashAlgorithmTag.Sha256);
            pgpSignatureGenerator.InitSign(Org.BouncyCastle.Bcpg.OpenPgp.PgpSignature.BinaryDocument, parameter.PrivateKey);
            var userId= parameter.PrivateKeySecreted.PublicKey.GetUserIds().Cast<string>().First();
            var pgpSignatureSubpacketGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSignatureSubpacketGenerator();
            pgpSignatureSubpacketGenerator.SetSignerUserId(false,userId);
            pgpSignatureGenerator.SetHashedSubpackets(pgpSignatureSubpacketGenerator.Generate());

            var sr = new Org.BouncyCastle.Security.SecureRandom();
            var pgpEncryptedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpEncryptedDataGenerator(parameter.SymmetricKeyAlgorithmTag, sr);
            pgpEncryptedDataGenerator.AddMethod(parameter.PublickKey);
            var pgpCompressedDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpCompressedDataGenerator(parameter.CompressionAlgorithmTag);
            var pgpLiteralDataGenerator = new Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralDataGenerator();

            using (var fs = parameter.InputFile.OpenRead())
            using (var outputStreamEncrypted = pgpEncryptedDataGenerator.Open(parameter.OutputStream, new byte[parameter.BufferSize]))
            using (var outputStreamEncryptedCompressed = pgpCompressedDataGenerator.Open(outputStreamEncrypted))
            using (var outputStreamEncryptedCompressedLiteral = pgpLiteralDataGenerator.Open(outputStreamEncryptedCompressed, Org.BouncyCastle.Bcpg.OpenPgp.PgpLiteralData.Binary, parameter.InputFile.Name, parameter.InputFile.Length, parameter.InputFile.LastWriteTime))
            {
                pgpSignatureGenerator.GenerateOnePassVersion(false).Encode(outputStreamEncryptedCompressedLiteral);
                int dataLenght = 0;
                var buffer = new byte[parameter.BufferSize];
                while ((dataLenght=fs.Read(buffer,0,buffer.Length))>0)
                {
                    outputStreamEncryptedCompressedLiteral.Write(buffer, 0, dataLenght);
                    pgpSignatureGenerator.Update(buffer, 0, dataLenght);
                }
                pgpSignatureGenerator.Generate().Encode(outputStreamEncryptedCompressedLiteral);

            }
        }
    }
}
