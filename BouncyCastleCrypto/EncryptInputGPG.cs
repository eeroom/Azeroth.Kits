﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncyCastleCrypto
{
    public class EncryptInputGPG
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="publickKeyStream">对应gpg4win导出的*.asc公钥文件</param>
        /// <param name="privateKeyStream">对应gpg4win导出的*.gpg私钥文件（被密码加密）</param>
        /// <param name="privateKeyPwd">*.gpg私钥文件的密码</param>
        public EncryptInputGPG(System.IO.Stream publickKeyStream, System.IO.Stream privateKeyStream,string privateKeyPwd)
        {
            using (var pkstream= Org.BouncyCastle.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(publickKeyStream))
            {
                var bundle = new Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRingBundle(pkstream);
                foreach (Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRing keyRing in bundle.GetKeyRings())
                {
                    foreach (Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKey key in keyRing.GetPublicKeys())
                    {
                        if (key?.IsEncryptionKey??false)
                        {
                            this.PublickKey = key;
                        }
                    }
                }
                if (this.PublickKey == null)
                    throw new ArgumentException("公钥数据有问题,没有找到公钥");
            }
            using (var pkstream= Org.BouncyCastle.Bcpg.OpenPgp.PgpUtilities.GetDecoderStream(privateKeyStream))
            {
                var bundle = new Org.BouncyCastle.Bcpg.OpenPgp.PgpSecretKeyRingBundle(pkstream);
                foreach (Org.BouncyCastle.Bcpg.OpenPgp.PgpSecretKeyRing keyRing in bundle.GetKeyRings())
                {
                    foreach (Org.BouncyCastle.Bcpg.OpenPgp.PgpSecretKey key in keyRing.GetSecretKeys())
                    {
                        if (key?.IsSigningKey ?? false)
                        {
                            this.PrivateKeySecreted = key;
                        }
                    }
                }
                if (this.PrivateKeySecreted == null)
                    throw new ArgumentException("私钥数据有问题,没有找到公钥");
            }
            this.PrivateKey = this.PrivateKeySecreted.ExtractPrivateKey(privateKeyPwd.ToCharArray());
            this.CompressionAlgorithmTag = Org.BouncyCastle.Bcpg.CompressionAlgorithmTag.Zip;
            this.IntegrityProtected = true;
            this.SymmetricKeyAlgorithmTag = Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag.Aes256;
            this.BufferSize = 8*1024;
        }

        /// <summary>
        /// 对应gpg4win导出的*.asc公钥文件
        /// </summary>
        public Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKey PublickKey { get;private set; }

        /// <summary>
        /// 对应gpg4win导出的*.gpg私钥文件（被密码加密）
        /// </summary>
        public Org.BouncyCastle.Bcpg.OpenPgp.PgpSecretKey PrivateKeySecreted { get; private set; }

        /// <summary>
        /// *.gpg私钥文件的密码
        /// </summary>
        public Org.BouncyCastle.Bcpg.OpenPgp.PgpPrivateKey PrivateKey { get; private set; }

        public System.IO.FileInfo InputFile { get; set; }

        public System.IO.Stream OutputStream { get; set; }

        /// <summary>
        /// 默认值：AES256
        /// </summary>
        public Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag SymmetricKeyAlgorithmTag { get; set; }

        /// <summary>
        /// 默认值：true
        /// </summary>
        public bool IntegrityProtected { get; set; }

        /// <summary>
        /// 默认值：Zip
        /// </summary>
        public Org.BouncyCastle.Bcpg.CompressionAlgorithmTag CompressionAlgorithmTag { get; set; }

        /// <summary>
        /// 默认值：8*1024
        /// </summary>
        public int BufferSize { get; set; }

    }
}
