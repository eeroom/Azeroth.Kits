using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace qiz4net {
    public class FileDecompresser : IArchiveExtractCallback {
        public event Action<ulong> BeginDecompress;
        public event Action<ulong> OnDecompress;
        public ArchiveFormat Format { private set; get; }
        //这个需要在解压缩的时候避免被释放，所以需要作为字段
        MySafeHandleZeroOrMinusOneIsInvalid safeHandle7zlib { set; get; }
        IInArchive archive7z { set; get; }

        public FileDecompresser(string lib7zPath, string zipFilePath) {
            this.safeHandle7zlib = Kernel32Dll.LoadLibrary(lib7zPath);
            if (this.safeHandle7zlib.IsInvalid)
                throw new ArgumentException("不能正常调用指定的7z类库文件");
            IntPtr createObjectPointer = Kernel32Dll.GetProcAddress(this.safeHandle7zlib, "CreateObject");
            CreateObjectDelegate createObject = (CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(createObjectPointer, typeof(CreateObjectDelegate));
            List<ArchiveFormat> lstAF = new List<ArchiveFormat>();
            var fileExt = System.IO.Path.GetExtension(zipFilePath).ToLower();
            if (FormatSetting.ExtensionFormatMapping.ContainsKey(fileExt))
                lstAF.Add(FormatSetting.ExtensionFormatMapping[fileExt]);
            this.archive7z= this.LoadArchiveFile(createObject, System.IO.File.Open(zipFilePath, FileMode.Open, FileAccess.Read), lstAF);
        }

        public FileDecompresser(string lib7zPath, Stream zipStream)
        {
            this.safeHandle7zlib = Kernel32Dll.LoadLibrary(lib7zPath);
            if (this.safeHandle7zlib.IsInvalid)
                throw new ArgumentException("不能正常调用指定的7z类库文件");
            IntPtr createObjectPointer = Kernel32Dll.GetProcAddress(this.safeHandle7zlib, "CreateObject");
            CreateObjectDelegate createObject = (CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(createObjectPointer, typeof(CreateObjectDelegate));
            List<ArchiveFormat> lstAF = new List<ArchiveFormat>();
            this.archive7z = this.LoadArchiveFile(createObject, zipStream, lstAF);
        }

        private IInArchive LoadArchiveFile(CreateObjectDelegate createObject,Stream fs, List<ArchiveFormat> lstAF)
        {
            int signatureMaxLength = FormatSetting.FileSignatures.Values.OrderByDescending(v => v.Length).First().Length;
            byte[] signatureBuffer = new byte[signatureMaxLength];
            fs.Position = 0;
            int bytesRead = fs.Read(signatureBuffer, 0, signatureMaxLength);
            var matchedSignature = FormatSetting.FileSignatures.Where(kv => signatureBuffer.Take(kv.Value.Length).SequenceEqual(kv.Value))
                .FirstOrDefault();
            if (matchedSignature.Key != ArchiveFormat.Undefined)
                lstAF.Add(matchedSignature.Key);
            var lstAFTemp = FormatSetting.FormatGuidMapping.Select(x => x.Key).Except(lstAF).ToList();
            lstAF.AddRange(lstAFTemp);
            ulong checkPos = 32 * 1024;
            this.Format = ArchiveFormat.Undefined;
            Guid interfaceId = typeof(IInArchive).GUID;
            foreach (var af in lstAF)
            {
                Guid classId = FormatSetting.FormatGuidMapping[af];
                object tmp = null;
                createObject.Invoke(ref classId, ref interfaceId, out tmp);
                IInArchive archive7z = tmp as IInArchive;
                fs.Position = 0;
                try
                {
                    int code = archive7z.Open(new QizFileStream(fs), ref checkPos, null);
                    if (code == 0)
                    {
                        this.Format = af;
                        return archive7z;
                    }
                    Marshal.ReleaseComObject(archive7z);
                }
                catch (Exception)
                {
                    if (archive7z != null)
                        Marshal.ReleaseComObject(archive7z);
                }
            }
            return null;
        }

        List<Entry> lstEntry {set; get; }

        bool Overwrite { set; get; }
        public void Decompress(string outputPath,Func<Entry,string> outputFileNameHandler, bool overwrite = true)
        {
            this.Overwrite = overwrite;
            this.GetEntries().ForEach(x => x.OutputFileName = outputFileNameHandler(x));
            this.CreateOutputDirectory(this.lstEntry);
            try
            {
                this.archive7z.Extract(null, 0xFFFFFFFF, 0, this);
            }
            finally
            {
                this.lstEntry.ForEach(x => x.OutputStream?.Close());//避免解压生成的文件大小可能为0的情况
            }
        }

        public void Decompress(string outputPath, bool overwrite = true)
        {
            this.Overwrite = overwrite;
            this.GetEntries().ForEach(x => x.OutputFileName = Path.Combine(outputPath, x.FileName ?? string.Empty));
            this.CreateOutputDirectory(this.lstEntry);
            try
            {
                this.archive7z.Extract(null, 0xFFFFFFFF, 0, this);
            }
            finally
            {
                this.lstEntry.ForEach(x => x.OutputStream?.Close());//避免解压生成的文件大小可能为0的情况
            }
        }

        private void CreateOutputDirectory(List<Entry> lstEntry)
        {
            var lstDir = this.lstEntry.Where(x => x.IsFolder).Select(x => x.OutputFileName).Distinct().ToList();
            var lstDir2 = this.lstEntry.Where(x => !x.IsFolder)
                            .Select(x => System.IO.Path.GetDirectoryName(x.OutputFileName))
                            .Distinct();
            lstDir.AddRange(lstDir2);
            var lstDirNotExist = lstDir.Distinct().Where(x => !System.IO.Directory.Exists(x)).ToList();
            lstDirNotExist.ForEach(x => System.IO.Directory.CreateDirectory(x));
        }

        public List<Entry> GetEntries()
        {
            if (this.lstEntry != null)
                return this.lstEntry;
            uint itemsCount = this.archive7z.GetNumberOfItems();
            this.lstEntry = System.Linq.Enumerable.Range(0, (int)itemsCount)
                .Select(x => (uint)x)
                .Select(fileIndex => new Entry(this.archive7z, fileIndex){
                    FileName = this.GetProperty(fileIndex, ItemPropId.kpidPath, x => x.ToString()),
                    IsFolder = this.GetProperty(fileIndex, ItemPropId.kpidIsFolder, x => (bool)x),
                    IsEncrypted = this.GetProperty(fileIndex, ItemPropId.kpidEncrypted, x => (bool)x),
                    Size = this.GetProperty(fileIndex, ItemPropId.kpidSize, x => (ulong)x),
                    PackedSize = this.GetProperty(fileIndex, ItemPropId.kpidPackedSize, x => (ulong)x),
                    CreationTime = this.GetProperty(fileIndex, ItemPropId.kpidCreationTime, x => (DateTime)x),
                    LastWriteTime = this.GetProperty(fileIndex, ItemPropId.kpidLastWriteTime, x => (DateTime)x),
                    LastAccessTime = this.GetProperty(fileIndex, ItemPropId.kpidLastAccessTime, x => (DateTime)x),
                    CRC = this.GetProperty(fileIndex, ItemPropId.kpidCRC, x => (uint)x),
                    Attributes = this.GetProperty(fileIndex, ItemPropId.kpidAttributes, x => (uint)x),
                    Comment = this.GetProperty(fileIndex, ItemPropId.kpidComment, x => x.ToString()),
                    HostOS = this.GetProperty(fileIndex, ItemPropId.kpidHostOS, x => x.ToString()),
                    Method = this.GetProperty(fileIndex, ItemPropId.kpidMethod, x => x.ToString()),
                    IsSplitBefore = this.GetProperty(fileIndex, ItemPropId.kpidSplitBefore, x => (bool)x),
                    IsSplitAfter = this.GetProperty(fileIndex, ItemPropId.kpidSplitAfter, x => (bool)x)
                }).ToList();
            return this.lstEntry;
        }

        private T GetProperty<T>(uint fileIndex, ItemPropId name,Func<object, T> convert) {
            PropVariant propVariant = new PropVariant();
            this.archive7z.GetProperty((uint)fileIndex, name, ref propVariant);
            var value= propVariant.GetValue(convert);
            propVariant.Clear();
            return value;
        }

        /// <summary>
        /// 解压缩后的文件总大小
        /// </summary>
        ulong total { set; get; }
        void IArchiveExtractCallback.SetTotal(ulong total)
        {
            this.total = total;
            this.BeginDecompress?.Invoke(this.total);
        }

        void IArchiveExtractCallback.SetCompleted(ref ulong completeValue)
        {
            this.OnDecompress?.Invoke(completeValue);
        }

        int IArchiveExtractCallback.GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            outStream = null;
            if (askExtractMode != AskMode.kExtract)
                return 0;
            var entry = this.lstEntry[(int)index];
            if (entry.IsFolder || entry.IsEncrypted)
                return 0;
            if(System.IO.File.Exists(entry.OutputFileName) && !this.Overwrite)
                return 0;
            entry.OutputStream= new QizFileStream(entry.OutputFileName, FileMode.Create, FileAccess.ReadWrite);
            outStream = entry.OutputStream;
            return 0;
        }

        void IArchiveExtractCallback.PrepareOperation(AskMode askExtractMode)
        {
            
        }

        void IArchiveExtractCallback.SetOperationResult(OperationResult resultEOperationResult)
        {
            
        }

        public void Close()
        {
            if (this.archive7z != null)
                Marshal.ReleaseComObject(this.archive7z);
            this.archive7z = null;
            if (this.safeHandle7zlib != null)
                this.safeHandle7zlib.Close();
        }
    }
}
