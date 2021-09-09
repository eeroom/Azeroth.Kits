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
        
        //这个需要在解压缩的时候避免被释放，所以需要作为字段
        MySafeHandleZeroOrMinusOneIsInvalid safeHandle7zlib { set; get; }
        private  IInArchive archive7z { set; get; }

        public Wrapper7z(string lib7zPath, ArchiveFormat format) {
            this.safeHandle7zlib = Kernel32Dll.LoadLibrary(lib7zPath);
            if (this.safeHandle7zlib.IsInvalid)
                throw new ArgumentException("不能正常调用指定的7z类库文件");
            IntPtr createObjectPointer = Kernel32Dll.GetProcAddress(this.safeHandle7zlib, "CreateObject");
            CreateObjectDelegate createObject = (CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(createObjectPointer, typeof(CreateObjectDelegate));
            object result;
            Guid interfaceId = typeof(IInArchive).GUID;
            Guid classId = FormatHelper.FormatGuidMapping[format];
            createObject(ref classId, ref interfaceId, out result);
            this.archive7z = result as IInArchive;
        }

        public void Extract(string zipFilePath,string outputFolder, bool overwrite = false) {
            this.Extract(this.GetEntries(zipFilePath), outputFolder, overwrite);
        }

        List<string> lstOutputFileName { set; get; }
        public void Extract(IList<Entry> lstEntry,string outputFolder, bool overwrite = false)
        {
            this.lstOutputFileName = new List<string>();
            foreach (Entry entry in lstEntry)
            {
                string outputPath = null;
                string fileName = Path.Combine(outputFolder, entry.FileName);
                if (entry.IsFolder || !File.Exists(fileName) || overwrite)
                {
                    outputPath = fileName;
                }
                if (outputPath == null) // getOutputPath = null means SKIP
                {
                    lstOutputFileName.Add(string.Empty);
                    continue;
                }
                if (entry.IsFolder)
                {
                    Directory.CreateDirectory(outputPath);
                    lstOutputFileName.Add(string.Empty);
                    continue;
                }
                string directoryName = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                lstOutputFileName.Add(outputPath);
            }
            this.archive7z.Extract(null, 0xFFFFFFFF, 0, this);
        }

        public List<Entry> GetEntries(string zipFilePath) {
            ulong checkPos = 32 * 1024;
            int open = this.archive7z.Open(new WrapperStream7z(zipFilePath, FileMode.Open, FileAccess.Read), ref checkPos, null);
            if (open != 0)
                throw new ArgumentException("按照指定格式打开压缩包发生异常");
            uint itemsCount = this.archive7z.GetNumberOfItems();
            var lstEntry = new List<Entry>((int)itemsCount);
            for (uint fileIndex = 0; fileIndex < itemsCount; fileIndex++) {
                string fileName = this.GetProperty<string>(fileIndex, ItemPropId.kpidPath);
                bool isFolder = this.GetProperty<bool>(fileIndex, ItemPropId.kpidIsFolder);
                bool isEncrypted = this.GetProperty<bool>(fileIndex, ItemPropId.kpidEncrypted);
                ulong size = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidSize);
                ulong packedSize = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidPackedSize);
                DateTime creationTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidCreationTime);
                DateTime lastWriteTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidLastWriteTime);
                DateTime lastAccessTime = this.GetPropertySafe<DateTime>(fileIndex, ItemPropId.kpidLastAccessTime);
                uint crc = this.GetPropertySafe<uint>(fileIndex, ItemPropId.kpidCRC);
                uint attributes = this.GetPropertySafe<uint>(fileIndex, ItemPropId.kpidAttributes);
                string comment = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidComment);
                string hostOS = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidHostOS);
                string method = this.GetPropertySafe<string>(fileIndex, ItemPropId.kpidMethod);

                bool isSplitBefore = this.GetPropertySafe<bool>(fileIndex, ItemPropId.kpidSplitBefore);
                bool isSplitAfter = this.GetPropertySafe<bool>(fileIndex, ItemPropId.kpidSplitAfter);

                lstEntry.Add(new Entry(this.archive7z, fileIndex) {
                    FileName = fileName,
                    IsFolder = isFolder,
                    IsEncrypted = isEncrypted,
                    Size = size,
                    PackedSize = packedSize,
                    CreationTime = creationTime,
                    LastWriteTime = lastWriteTime,
                    LastAccessTime = lastAccessTime,
                    CRC = crc,
                    Attributes = attributes,
                    Comment = comment,
                    HostOS = hostOS,
                    Method = method,
                    IsSplitBefore = isSplitBefore,
                    IsSplitAfter = isSplitAfter
                });
            }
            return lstEntry;
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
            if (this.lstOutputFileName == null)
                return 0;
            var filepath = this.lstOutputFileName[(int)index];
            if (string.IsNullOrEmpty(filepath))
                return 0;
            outStream = new WrapperStream7z(filepath, FileMode.Create, FileAccess.ReadWrite);
            return 0;
        }

        void IArchiveExtractCallback.PrepareOperation(AskMode askExtractMode)
        {
            
        }

        void IArchiveExtractCallback.SetOperationResult(OperationResult resultEOperationResult)
        {
            
        }
    }
}
