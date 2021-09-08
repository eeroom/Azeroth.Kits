using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class ArchiveFile : IDisposable {
        MySafeHandleZeroOrMinusOneIsInvalid safeHandle7zlib { set; get; }
        private  IInArchive handlerOf7z { set; get; }
        private  InStreamWrapper archiveFileStream { set; get; }
        private IList<Entry> lstEntry { set; get; }

        private string libraryFilePath { set; get; }

        public ArchiveFile(string filePath, SevenZipFormat format, string libraryFilePath) {
            this.libraryFilePath = libraryFilePath;
            this.safeHandle7zlib = Kernel32Dll.LoadLibrary(libraryFilePath);
            if (this.safeHandle7zlib.IsInvalid) {
                throw new Win32Exception();
            }
            IntPtr functionPtr = Kernel32Dll.GetProcAddress(this.safeHandle7zlib, "GetHandlerProperty");

            // Not valid dll
            if (functionPtr == IntPtr.Zero) {
                this.safeHandle7zlib.Close();
                throw new ArgumentException();
            }
            if (archiveFileStream == null)
                throw new ArgumentException("archiveStream is null");
            IntPtr procAddress = Kernel32Dll.GetProcAddress(this.safeHandle7zlib, "CreateObject");
            CreateObjectDelegate createObject = (CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(CreateObjectDelegate));
            object result;
            Guid interfaceId = typeof(IInArchive).GUID;
            Guid classId = FormatHelper.FormatGuidMapping[format];
            createObject(ref classId, ref interfaceId, out result);
            this.handlerOf7z = result as IInArchive;
            this.archiveFileStream = new InStreamWrapper(System.IO.File.OpenRead(filePath));
        }

        public void Extract(string outputFolder, bool overwrite = false) {
            IList<Stream> fileStreams = new List<Stream>();
            foreach (Entry entry in this.GetEntries()) {
                string outputPath = null;
                string fileName = Path.Combine(outputFolder, entry.FileName);
                if (entry.IsFolder || !File.Exists(fileName) || overwrite) {
                    outputPath = fileName;
                }
                if (outputPath == null) // getOutputPath = null means SKIP
                {
                    fileStreams.Add(null);
                    continue;
                }
                if (entry.IsFolder) {
                    Directory.CreateDirectory(outputPath);
                    fileStreams.Add(null);
                    continue;
                }
                string directoryName = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrWhiteSpace(directoryName)) {
                    Directory.CreateDirectory(directoryName);
                }
                fileStreams.Add(File.Create(outputPath));
            }
            this.handlerOf7z.Extract(null, 0xFFFFFFFF, 0, new ArchiveStreamsCallback(fileStreams));
        }

        public IList<Entry> GetEntries() {
            if (this.lstEntry != null)
                return this.lstEntry;
            ulong checkPos = 32 * 1024;
            int open = this.handlerOf7z.Open(this.archiveFileStream, ref checkPos, null);

            if (open != 0) {
                throw new ArgumentException("Unable to open archive");
            }

            uint itemsCount = this.handlerOf7z.GetNumberOfItems();

            this.lstEntry = new List<Entry>((int)itemsCount);

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

                this.lstEntry.Add(new Entry(this.handlerOf7z, fileIndex) {
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
            this.handlerOf7z.GetProperty(fileIndex, name, ref propVariant);
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

        ~ArchiveFile() {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing) {
            if (this.archiveFileStream != null) {
                this.archiveFileStream.Dispose();
            }

            if (this.handlerOf7z != null) {
                Marshal.ReleaseComObject(this.handlerOf7z);
            }

            if ((this.safeHandle7zlib != null) && !this.safeHandle7zlib.IsClosed) {
                this.safeHandle7zlib.Close();
            }

            this.safeHandle7zlib = null;
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
