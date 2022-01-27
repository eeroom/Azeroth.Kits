using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncyCastleCrypto
{
    public class GpgEncryptSignCfg
    {
        public GpgEncryptSignCfg()
        {
            this.CompressionAlgorithmTag = Org.BouncyCastle.Bcpg.CompressionAlgorithmTag.Zip;
            this.IntegrityProtected = true;
            this.SymmetricKeyAlgorithmTag = Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag.Aes256;
            this.BufferSize = 8 * 1024;
        }
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

        /// <summary>
        /// 默认值false
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// 默认值false
        /// </summary>
        public bool IsNested { get; set; }
    }
}
