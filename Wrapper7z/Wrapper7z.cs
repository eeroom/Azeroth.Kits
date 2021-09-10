using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class Wrapper7z : IArchiveExtractCallback {
        public event Action<ulong> BeginExtract;
        public event Action<ulong> OnExtract;
        public ArchiveFormat Format { private set; get; }
        //这个需要在解压缩的时候避免被释放，所以需要作为字段
        MySafeHandleZeroOrMinusOneIsInvalid safeHandle7zlib { set; get; }
        IInArchive archive7z { set; get; }

        public Wrapper7z(string lib7zPath, string zipFilePath) {
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

        public Wrapper7z(string lib7zPath, Stream zipStream)
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
                    int code = archive7z.Open(new WrapperStream7z(fs), ref checkPos, null);
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
            this.lstEntry.ForEach(x => x.OutputFileName = outputFileNameHandler(x));
            this.CreateOutputDirectory(this.lstEntry);
            this.archive7z.Extract(null, 0xFFFFFFFF, 0, this);
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

        public void Decompress(string outputPath, bool overwrite=true)
        {
            this.Overwrite = overwrite;
            this.lstEntry.ForEach(x => x.OutputFileName = Path.Combine(outputPath, x.FileName ?? string.Empty));
            this.CreateOutputDirectory(this.lstEntry);
            this.archive7z.Extract(null, 0xFFFFFFFF, 0, this);
        }

        public List<Entry> GetEntries()
        {
            if (this.lstEntry != null)
                return this.lstEntry;
            uint itemsCount = this.archive7z.GetNumberOfItems();
            this.lstEntry = new List<Entry>((int)itemsCount);
            for (uint fileIndex = 0; fileIndex < itemsCount; fileIndex++)
            {
                var entry = new Entry(this.archive7z, fileIndex)
                {
                    FileName = this.GetProperty<string>(fileIndex, ItemPropId.kpidPath),
                    IsFolder = this.GetProperty<bool>(fileIndex, ItemPropId.kpidIsFolder),
                    IsEncrypted = this.GetProperty<bool>(fileIndex, ItemPropId.kpidEncrypted),
                    Size = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidSize),
                    PackedSize = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidPackedSize),
                    CreationTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidCreationTime),
                    LastWriteTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidLastWriteTime),
                    LastAccessTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidLastAccessTime),
                    CRC = this.GetPropertySafe<uint>(fileIndex, ItemPropId.kpidCRC),
                    Attributes = this.GetPropertySafe<uint>(fileIndex, ItemPropId.kpidAttributes),
                    Comment = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidComment),
                    HostOS = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidHostOS),
                    Method = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidMethod),
                    IsSplitBefore = this.GetPropertySafe<bool>(fileIndex, ItemPropId.kpidSplitBefore),
                    IsSplitAfter = this.GetPropertySafe<bool>(fileIndex, ItemPropId.kpidSplitAfter)
                };
                this.lstEntry.Add(entry);
            }
            return this.lstEntry;
        }

        private T GetPropertySafe<T>(uint fileIndex, ItemPropId name) {
            try {
                return this.GetProperty<T>(fileIndex, name);
            } catch (InvalidCastException) {
                return default(T);
            }
        }

        private T GetProperty<T>(uint fileIndex, ItemPropId name) {
            PropVariant propVariant = new PropVariant();
            this.archive7z.GetProperty(fileIndex, name, ref propVariant);
            object value = propVariant.GetObject();

            if (propVariant.VarType == VarEnum.VT_EMPTY) {
                propVariant.Clear();
                return default(T);
            }

            propVariant.Clear();

            if (value == null) {
                return default(T);
            }

            Type type = typeof(T);
            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            Type underlyingType = isNullable ? Nullable.GetUnderlyingType(type) : type;

            T result = (T)Convert.ChangeType(value.ToString(), underlyingType);

            return result;
        }

        /// <summary>
        /// 解压缩后的文件总大小
        /// </summary>
        ulong total;
        void IArchiveExtractCallback.SetTotal(ulong total)
        {
            this.total = total;
            this.BeginExtract?.Invoke(this.total);
        }

        void IArchiveExtractCallback.SetCompleted(ref ulong completeValue)
        {
            this.OnExtract?.Invoke(completeValue);
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
            outStream = new WrapperStream7z(entry.OutputFileName, FileMode.Create, FileAccess.ReadWrite);
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
