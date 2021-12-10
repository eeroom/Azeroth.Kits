using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace qiz4net {
    public class Entry: IArchiveExtractCallback
    {
        private readonly IInArchive archive;
        private readonly uint index;

        internal Entry(IInArchive archive, uint index) {
            this.archive = archive;
            this.index = index;
        }

        /// <summary>
        /// Name of the file with its relative path within the archive
        /// </summary>
        public string FileName { get; internal set; }
        /// <summary>
        /// True if entry is a folder, false if it is a file
        /// </summary>
        public bool IsFolder { get; internal set; }
        /// <summary>
        /// Original entry size
        /// </summary>
        public ulong Size { get; internal set; }
        /// <summary>
        /// Entry size in a archived state
        /// </summary>
        public ulong PackedSize { get; internal set; }

        /// <summary>
        /// Date and time of the file (entry) creation
        /// </summary>
        public DateTime CreationTime { get; internal set; }

        /// <summary>
        /// Date and time of the last change of the file (entry)
        /// </summary>
        public DateTime LastWriteTime { get; internal set; }

        /// <summary>
        /// Date and time of the last access of the file (entry)
        /// </summary>
        public DateTime LastAccessTime { get; internal set; }

        /// <summary>
        /// CRC hash of the entry
        /// </summary>
        public UInt32 CRC { get; internal set; }

        /// <summary>
        /// Attributes of the entry
        /// </summary>
        public UInt32 Attributes { get; internal set; }

        /// <summary>
        /// True if entry is encrypted, otherwise false
        /// </summary>
        public bool IsEncrypted { get; internal set; }

        /// <summary>
        /// Comment of the entry
        /// </summary>
        public string Comment { get; internal set; }

        /// <summary>
        /// Compression method of the entry
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// Host operating system of the entry
        /// </summary>
        public string HostOS { get; internal set; }

        /// <summary>
        /// True if there are parts of this file in previous split archive parts
        /// </summary>
        public bool IsSplitBefore { get; set; }

        /// <summary>
        /// True if there are parts of this file in next split archive parts
        /// </summary>
        public bool IsSplitAfter { get; set; }

        internal QizFileStream OutputStream { set; get; }
        public void Decompress(string outputFilePath, bool preserveTimestamp = true) {
            if (this.IsFolder) {
                Directory.CreateDirectory(outputFilePath);
                return;
            }

            string directoryName = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrWhiteSpace(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
            this.OutputStream = new QizFileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite);
            this.archive.Extract(new[] { this.index }, 1, 0, this);
            //避免解压生成的文件大小为0的问题
            this.OutputStream.Close();
            if (preserveTimestamp) {
                File.SetLastWriteTime(outputFilePath, this.LastWriteTime);
            }
        }


        
        public void Decompress(QizFileStream outputStream) {
            if (this.IsFolder)
                return;
            this.OutputStream = outputStream;
            this.archive.Extract(new[] { this.index }, 1, 0, this);
            //避免解压生成的文件大小为0的问题
            this.OutputStream.BaseStream.Flush();
        }


        void IArchiveExtractCallback.SetTotal(ulong total)
        {
            
        }

        void IArchiveExtractCallback.SetCompleted(ref ulong completeValue)
        {
           
        }

        int IArchiveExtractCallback.GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            outStream = null;
            if (index != this.index || askExtractMode != AskMode.kExtract)
                return 0;
            outStream = this.OutputStream;
            return 0;
        }

        void IArchiveExtractCallback.PrepareOperation(AskMode askExtractMode)
        {
            
        }

        void IArchiveExtractCallback.SetOperationResult(OperationResult resultEOperationResult)
        {
            
        }

        public string OutputFileName { set; get; }
    }
}
