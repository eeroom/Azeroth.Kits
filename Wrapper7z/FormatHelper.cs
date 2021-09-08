using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wrapper7z {
   public static class FormatHelper {
       public static Dictionary<string, SevenZipFormat> ExtensionFormatMapping { set; get; }

        public static Dictionary<SevenZipFormat, Guid> FormatGuidMapping { set; get; }

        public static Dictionary<SevenZipFormat, byte[]> FileSignatures { set; get; }
        static FormatHelper() {
            ExtensionFormatMapping = new Dictionary<string, SevenZipFormat>();
            ExtensionFormatMapping.Add("7z", SevenZipFormat.SevenZip);
            ExtensionFormatMapping.Add("gz", SevenZipFormat.GZip);
            ExtensionFormatMapping.Add("tar", SevenZipFormat.Tar);
            ExtensionFormatMapping.Add("rar", SevenZipFormat.Rar);
            ExtensionFormatMapping.Add("zip", SevenZipFormat.Zip);
            ExtensionFormatMapping.Add("lzma", SevenZipFormat.Lzma);
            ExtensionFormatMapping.Add("lzh", SevenZipFormat.Lzh);
            ExtensionFormatMapping.Add("arj", SevenZipFormat.Arj);
            ExtensionFormatMapping.Add("bz2", SevenZipFormat.BZip2);
            ExtensionFormatMapping.Add("cab", SevenZipFormat.Cab);
            ExtensionFormatMapping.Add("chm", SevenZipFormat.Chm);
            ExtensionFormatMapping.Add("deb", SevenZipFormat.Deb);
            ExtensionFormatMapping.Add("iso", SevenZipFormat.Iso);
            ExtensionFormatMapping.Add("rpm", SevenZipFormat.Rpm);
            ExtensionFormatMapping.Add("wim", SevenZipFormat.Wim);
            ExtensionFormatMapping.Add("udf", SevenZipFormat.Udf);
            ExtensionFormatMapping.Add("mub", SevenZipFormat.Mub);
            ExtensionFormatMapping.Add("xar", SevenZipFormat.Xar);
            ExtensionFormatMapping.Add("hfs", SevenZipFormat.Hfs);
            ExtensionFormatMapping.Add("dmg", SevenZipFormat.Dmg);
            ExtensionFormatMapping.Add("z", SevenZipFormat.Lzw);
            ExtensionFormatMapping.Add("xz", SevenZipFormat.XZ);
            ExtensionFormatMapping.Add("flv", SevenZipFormat.Flv);
            ExtensionFormatMapping.Add("swf", SevenZipFormat.Swf);
            ExtensionFormatMapping.Add("exe", SevenZipFormat.PE);
            ExtensionFormatMapping.Add("dll", SevenZipFormat.PE);
            ExtensionFormatMapping.Add("vhd", SevenZipFormat.Vhd);

            FormatGuidMapping = new Dictionary<SevenZipFormat, Guid>();
            FormatGuidMapping.Add(SevenZipFormat.SevenZip, new Guid("23170f69-40c1-278a-1000-000110070000"));
            FormatGuidMapping.Add(SevenZipFormat.Arj, new Guid("23170f69-40c1-278a-1000-000110040000"));
            FormatGuidMapping.Add(SevenZipFormat.BZip2, new Guid("23170f69-40c1-278a-1000-000110020000"));
            FormatGuidMapping.Add(SevenZipFormat.Cab, new Guid("23170f69-40c1-278a-1000-000110080000"));
            FormatGuidMapping.Add(SevenZipFormat.Chm, new Guid("23170f69-40c1-278a-1000-000110e90000"));
            FormatGuidMapping.Add(SevenZipFormat.Compound, new Guid("23170f69-40c1-278a-1000-000110e50000"));
            FormatGuidMapping.Add(SevenZipFormat.Cpio, new Guid("23170f69-40c1-278a-1000-000110ed0000"));
            FormatGuidMapping.Add(SevenZipFormat.Deb, new Guid("23170f69-40c1-278a-1000-000110ec0000"));
            FormatGuidMapping.Add(SevenZipFormat.GZip, new Guid("23170f69-40c1-278a-1000-000110ef0000"));
            FormatGuidMapping.Add(SevenZipFormat.Iso, new Guid("23170f69-40c1-278a-1000-000110e70000"));
            FormatGuidMapping.Add(SevenZipFormat.Lzh, new Guid("23170f69-40c1-278a-1000-000110060000"));
            FormatGuidMapping.Add(SevenZipFormat.Lzma, new Guid("23170f69-40c1-278a-1000-0001100a0000"));
            FormatGuidMapping.Add(SevenZipFormat.Nsis, new Guid("23170f69-40c1-278a-1000-000110090000"));
            FormatGuidMapping.Add(SevenZipFormat.Rar, new Guid("23170f69-40c1-278a-1000-000110030000"));
            FormatGuidMapping.Add(SevenZipFormat.Rar5, new Guid("23170f69-40c1-278a-1000-000110CC0000"));
            FormatGuidMapping.Add(SevenZipFormat.Rpm, new Guid("23170f69-40c1-278a-1000-000110eb0000"));
            FormatGuidMapping.Add(SevenZipFormat.Split, new Guid("23170f69-40c1-278a-1000-000110ea0000"));
            FormatGuidMapping.Add(SevenZipFormat.Tar, new Guid("23170f69-40c1-278a-1000-000110ee0000"));
            FormatGuidMapping.Add(SevenZipFormat.Wim, new Guid("23170f69-40c1-278a-1000-000110e60000"));
            FormatGuidMapping.Add(SevenZipFormat.Lzw, new Guid("23170f69-40c1-278a-1000-000110050000"));
            FormatGuidMapping.Add(SevenZipFormat.Zip, new Guid("23170f69-40c1-278a-1000-000110010000"));
            FormatGuidMapping.Add(SevenZipFormat.Udf, new Guid("23170f69-40c1-278a-1000-000110E00000"));
            FormatGuidMapping.Add(SevenZipFormat.Xar, new Guid("23170f69-40c1-278a-1000-000110E10000"));
            FormatGuidMapping.Add(SevenZipFormat.Mub, new Guid("23170f69-40c1-278a-1000-000110E20000"));
            FormatGuidMapping.Add(SevenZipFormat.Hfs, new Guid("23170f69-40c1-278a-1000-000110E30000"));
            FormatGuidMapping.Add(SevenZipFormat.Dmg, new Guid("23170f69-40c1-278a-1000-000110E40000"));
            FormatGuidMapping.Add(SevenZipFormat.XZ, new Guid("23170f69-40c1-278a-1000-0001100C0000"));
            FormatGuidMapping.Add(SevenZipFormat.Mslz, new Guid("23170f69-40c1-278a-1000-000110D50000"));
            FormatGuidMapping.Add(SevenZipFormat.PE, new Guid("23170f69-40c1-278a-1000-000110DD0000"));
            FormatGuidMapping.Add(SevenZipFormat.Elf, new Guid("23170f69-40c1-278a-1000-000110DE0000"));
            FormatGuidMapping.Add(SevenZipFormat.Swf, new Guid("23170f69-40c1-278a-1000-000110D70000"));
            FormatGuidMapping.Add(SevenZipFormat.Vhd, new Guid("23170f69-40c1-278a-1000-000110DC0000"));
            FormatGuidMapping.Add(SevenZipFormat.Flv, new Guid("23170f69-40c1-278a-1000-000110D60000"));
            FormatGuidMapping.Add(SevenZipFormat.SquashFS, new Guid("23170f69-40c1-278a-1000-000110D20000"));
            FormatGuidMapping.Add(SevenZipFormat.Lzma86, new Guid("23170f69-40c1-278a-1000-0001100B0000"));
            FormatGuidMapping.Add(SevenZipFormat.Ppmd, new Guid("23170f69-40c1-278a-1000-0001100D0000"));
            FormatGuidMapping.Add(SevenZipFormat.TE, new Guid("23170f69-40c1-278a-1000-000110CF0000"));
            FormatGuidMapping.Add(SevenZipFormat.UEFIc, new Guid("23170f69-40c1-278a-1000-000110D00000"));
            FormatGuidMapping.Add(SevenZipFormat.UEFIs, new Guid("23170f69-40c1-278a-1000-000110D10000"));
            FormatGuidMapping.Add(SevenZipFormat.CramFS, new Guid("23170f69-40c1-278a-1000-000110D30000"));
            FormatGuidMapping.Add(SevenZipFormat.APM, new Guid("23170f69-40c1-278a-1000-000110D40000"));
            FormatGuidMapping.Add(SevenZipFormat.Swfc, new Guid("23170f69-40c1-278a-1000-000110D80000"));
            FormatGuidMapping.Add(SevenZipFormat.Ntfs, new Guid("23170f69-40c1-278a-1000-000110D90000"));
            FormatGuidMapping.Add(SevenZipFormat.Fat, new Guid("23170f69-40c1-278a-1000-000110DA0000"));
            FormatGuidMapping.Add(SevenZipFormat.Mbr, new Guid("23170f69-40c1-278a-1000-000110DB0000"));
            FormatGuidMapping.Add(SevenZipFormat.MachO, new Guid("23170f69-40c1-278a-1000-000110DF0000"));

            FileSignatures = new Dictionary<SevenZipFormat, byte[]>();
            FileSignatures.Add(SevenZipFormat.Rar5, new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 });
            FileSignatures.Add(SevenZipFormat.Rar, new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 });
            FileSignatures.Add(SevenZipFormat.Vhd, new byte[] { 0x63, 0x6F, 0x6E, 0x65, 0x63, 0x74, 0x69, 0x78 });
            FileSignatures.Add(SevenZipFormat.Deb, new byte[] { 0x21, 0x3C, 0x61, 0x72, 0x63, 0x68, 0x3E });
            FileSignatures.Add(SevenZipFormat.Dmg, new byte[] { 0x78, 0x01, 0x73, 0x0D, 0x62, 0x62, 0x60 });
            FileSignatures.Add(SevenZipFormat.SevenZip, new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C });
            FileSignatures.Add(SevenZipFormat.Tar, new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72 });
            FileSignatures.Add(SevenZipFormat.Iso, new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 });
            FileSignatures.Add(SevenZipFormat.Cab, new byte[] { 0x4D, 0x53, 0x43, 0x46 });
            FileSignatures.Add(SevenZipFormat.Rpm, new byte[] { 0xed, 0xab, 0xee, 0xdb });
            FileSignatures.Add(SevenZipFormat.Xar, new byte[] { 0x78, 0x61, 0x72, 0x21 });
            FileSignatures.Add(SevenZipFormat.Chm, new byte[] { 0x49, 0x54, 0x53, 0x46 });
            FileSignatures.Add(SevenZipFormat.BZip2, new byte[] { 0x42, 0x5A, 0x68 });
            FileSignatures.Add(SevenZipFormat.Flv, new byte[] { 0x46, 0x4C, 0x56 });
            FileSignatures.Add(SevenZipFormat.Swf, new byte[] { 0x46, 0x57, 0x53 });
            FileSignatures.Add(SevenZipFormat.GZip, new byte[] { 0x1f, 0x0b });
            FileSignatures.Add(SevenZipFormat.Zip, new byte[] { 0x50, 0x4b });
            FileSignatures.Add(SevenZipFormat.Arj, new byte[] { 0x60, 0xEA });
            FileSignatures.Add(SevenZipFormat.Lzh, new byte[] { 0x2D, 0x6C, 0x68 });
        }

        public static SevenZipFormat GetFormate(string filePath) {
            string fileExtension = System.IO.Path.GetExtension(filePath)?.TrimStart('.')?.ToLower();
            if (!string.IsNullOrEmpty(fileExtension) && fileExtension != "rar" && FormatHelper.ExtensionFormatMapping.ContainsKey(fileExtension))
                return FormatHelper.ExtensionFormatMapping[fileExtension];
            using (FileStream fileStream = System.IO.File.OpenRead(filePath)) {
                int longestSignature = FormatHelper.FileSignatures.Values.OrderByDescending(v => v.Length).First().Length;
                byte[] archiveFileSignature = new byte[longestSignature];
                int bytesRead = fileStream.Read(archiveFileSignature, 0, longestSignature);
                if (bytesRead != longestSignature)
                    return SevenZipFormat.Undefined;
                foreach (KeyValuePair<SevenZipFormat, byte[]> kv in FormatHelper.FileSignatures) {
                    if (archiveFileSignature.Take(kv.Value.Length).SequenceEqual(kv.Value)) {
                        return kv.Key;
                    }
                }
            }
            return SevenZipFormat.Undefined;
        }

    }
}
