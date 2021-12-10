using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace qiz4net {
   public static class FormatSetting {
       public static Dictionary<string, ArchiveFormat> ExtensionFormatMapping { set; get; }

        public static Dictionary<ArchiveFormat, Guid> FormatGuidMapping { set; get; }

        public static Dictionary<ArchiveFormat, byte[]> FileSignatures { set; get; }
        static FormatSetting() {
            ExtensionFormatMapping = new Dictionary<string, ArchiveFormat>();
            ExtensionFormatMapping.Add(".7z", ArchiveFormat.SevenZip);
            ExtensionFormatMapping.Add(".gz", ArchiveFormat.GZip);
            ExtensionFormatMapping.Add(".tar", ArchiveFormat.Tar);
            ExtensionFormatMapping.Add(".rar", ArchiveFormat.Rar);
            ExtensionFormatMapping.Add(".zip", ArchiveFormat.Zip);
            ExtensionFormatMapping.Add(".lzma", ArchiveFormat.Lzma);
            ExtensionFormatMapping.Add(".lzh", ArchiveFormat.Lzh);
            ExtensionFormatMapping.Add(".arj", ArchiveFormat.Arj);
            ExtensionFormatMapping.Add(".bz2", ArchiveFormat.BZip2);
            ExtensionFormatMapping.Add(".cab", ArchiveFormat.Cab);
            ExtensionFormatMapping.Add(".chm", ArchiveFormat.Chm);
            ExtensionFormatMapping.Add(".deb", ArchiveFormat.Deb);
            ExtensionFormatMapping.Add(".iso", ArchiveFormat.Iso);
            ExtensionFormatMapping.Add(".rpm", ArchiveFormat.Rpm);
            ExtensionFormatMapping.Add(".wim", ArchiveFormat.Wim);
            ExtensionFormatMapping.Add(".udf", ArchiveFormat.Udf);
            ExtensionFormatMapping.Add(".mub", ArchiveFormat.Mub);
            ExtensionFormatMapping.Add(".xar", ArchiveFormat.Xar);
            ExtensionFormatMapping.Add(".hfs", ArchiveFormat.Hfs);
            ExtensionFormatMapping.Add(".dmg", ArchiveFormat.Dmg);
            ExtensionFormatMapping.Add(".z", ArchiveFormat.Lzw);
            ExtensionFormatMapping.Add(".xz", ArchiveFormat.XZ);
            ExtensionFormatMapping.Add(".flv", ArchiveFormat.Flv);
            ExtensionFormatMapping.Add(".swf", ArchiveFormat.Swf);
            ExtensionFormatMapping.Add(".exe", ArchiveFormat.PE);
            ExtensionFormatMapping.Add(".dll", ArchiveFormat.PE);
            ExtensionFormatMapping.Add(".vhd", ArchiveFormat.Vhd);

            FormatGuidMapping = new Dictionary<ArchiveFormat, Guid>();
            FormatGuidMapping.Add(ArchiveFormat.Zip, new Guid("23170f69-40c1-278a-1000-000110010000"));
            FormatGuidMapping.Add(ArchiveFormat.Rar, new Guid("23170f69-40c1-278a-1000-000110030000"));
            FormatGuidMapping.Add(ArchiveFormat.Rar5, new Guid("23170f69-40c1-278a-1000-000110CC0000"));
            FormatGuidMapping.Add(ArchiveFormat.SevenZip, new Guid("23170f69-40c1-278a-1000-000110070000"));
            FormatGuidMapping.Add(ArchiveFormat.Tar, new Guid("23170f69-40c1-278a-1000-000110ee0000"));
            FormatGuidMapping.Add(ArchiveFormat.GZip, new Guid("23170f69-40c1-278a-1000-000110ef0000"));
            FormatGuidMapping.Add(ArchiveFormat.BZip2, new Guid("23170f69-40c1-278a-1000-000110020000"));
            FormatGuidMapping.Add(ArchiveFormat.Iso, new Guid("23170f69-40c1-278a-1000-000110e70000"));
            FormatGuidMapping.Add(ArchiveFormat.Lzh, new Guid("23170f69-40c1-278a-1000-000110060000"));
            FormatGuidMapping.Add(ArchiveFormat.Lzma, new Guid("23170f69-40c1-278a-1000-0001100a0000"));
            FormatGuidMapping.Add(ArchiveFormat.Wim, new Guid("23170f69-40c1-278a-1000-000110e60000"));
            FormatGuidMapping.Add(ArchiveFormat.Dmg, new Guid("23170f69-40c1-278a-1000-000110E40000"));
            FormatGuidMapping.Add(ArchiveFormat.Vhd, new Guid("23170f69-40c1-278a-1000-000110DC0000"));
            FormatGuidMapping.Add(ArchiveFormat.XZ, new Guid("23170f69-40c1-278a-1000-0001100C0000"));
            FormatGuidMapping.Add(ArchiveFormat.Cab, new Guid("23170f69-40c1-278a-1000-000110080000"));

            FormatGuidMapping.Add(ArchiveFormat.Arj, new Guid("23170f69-40c1-278a-1000-000110040000"));
            FormatGuidMapping.Add(ArchiveFormat.Chm, new Guid("23170f69-40c1-278a-1000-000110e90000"));
            FormatGuidMapping.Add(ArchiveFormat.Compound, new Guid("23170f69-40c1-278a-1000-000110e50000"));
            FormatGuidMapping.Add(ArchiveFormat.Cpio, new Guid("23170f69-40c1-278a-1000-000110ed0000"));
            FormatGuidMapping.Add(ArchiveFormat.Deb, new Guid("23170f69-40c1-278a-1000-000110ec0000"));
            FormatGuidMapping.Add(ArchiveFormat.Nsis, new Guid("23170f69-40c1-278a-1000-000110090000"));
            FormatGuidMapping.Add(ArchiveFormat.Rpm, new Guid("23170f69-40c1-278a-1000-000110eb0000"));
            FormatGuidMapping.Add(ArchiveFormat.Split, new Guid("23170f69-40c1-278a-1000-000110ea0000"));
            FormatGuidMapping.Add(ArchiveFormat.Lzw, new Guid("23170f69-40c1-278a-1000-000110050000"));
            FormatGuidMapping.Add(ArchiveFormat.Udf, new Guid("23170f69-40c1-278a-1000-000110E00000"));
            FormatGuidMapping.Add(ArchiveFormat.Xar, new Guid("23170f69-40c1-278a-1000-000110E10000"));
            FormatGuidMapping.Add(ArchiveFormat.Mub, new Guid("23170f69-40c1-278a-1000-000110E20000"));
            FormatGuidMapping.Add(ArchiveFormat.Hfs, new Guid("23170f69-40c1-278a-1000-000110E30000"));
            FormatGuidMapping.Add(ArchiveFormat.Mslz, new Guid("23170f69-40c1-278a-1000-000110D50000"));
            FormatGuidMapping.Add(ArchiveFormat.PE, new Guid("23170f69-40c1-278a-1000-000110DD0000"));
            FormatGuidMapping.Add(ArchiveFormat.Elf, new Guid("23170f69-40c1-278a-1000-000110DE0000"));
            FormatGuidMapping.Add(ArchiveFormat.Swf, new Guid("23170f69-40c1-278a-1000-000110D70000"));
            FormatGuidMapping.Add(ArchiveFormat.Flv, new Guid("23170f69-40c1-278a-1000-000110D60000"));
            FormatGuidMapping.Add(ArchiveFormat.SquashFS, new Guid("23170f69-40c1-278a-1000-000110D20000"));
            FormatGuidMapping.Add(ArchiveFormat.Lzma86, new Guid("23170f69-40c1-278a-1000-0001100B0000"));
            FormatGuidMapping.Add(ArchiveFormat.Ppmd, new Guid("23170f69-40c1-278a-1000-0001100D0000"));
            FormatGuidMapping.Add(ArchiveFormat.TE, new Guid("23170f69-40c1-278a-1000-000110CF0000"));
            FormatGuidMapping.Add(ArchiveFormat.UEFIc, new Guid("23170f69-40c1-278a-1000-000110D00000"));
            FormatGuidMapping.Add(ArchiveFormat.UEFIs, new Guid("23170f69-40c1-278a-1000-000110D10000"));
            FormatGuidMapping.Add(ArchiveFormat.CramFS, new Guid("23170f69-40c1-278a-1000-000110D30000"));
            FormatGuidMapping.Add(ArchiveFormat.APM, new Guid("23170f69-40c1-278a-1000-000110D40000"));
            FormatGuidMapping.Add(ArchiveFormat.Swfc, new Guid("23170f69-40c1-278a-1000-000110D80000"));
            FormatGuidMapping.Add(ArchiveFormat.Ntfs, new Guid("23170f69-40c1-278a-1000-000110D90000"));
            FormatGuidMapping.Add(ArchiveFormat.Fat, new Guid("23170f69-40c1-278a-1000-000110DA0000"));
            FormatGuidMapping.Add(ArchiveFormat.Mbr, new Guid("23170f69-40c1-278a-1000-000110DB0000"));
            FormatGuidMapping.Add(ArchiveFormat.MachO, new Guid("23170f69-40c1-278a-1000-000110DF0000"));

            FileSignatures = new Dictionary<ArchiveFormat, byte[]>();
            FileSignatures.Add(ArchiveFormat.Rar5, new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 });
            FileSignatures.Add(ArchiveFormat.Rar, new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 });
            FileSignatures.Add(ArchiveFormat.Vhd, new byte[] { 0x63, 0x6F, 0x6E, 0x65, 0x63, 0x74, 0x69, 0x78 });
            FileSignatures.Add(ArchiveFormat.Deb, new byte[] { 0x21, 0x3C, 0x61, 0x72, 0x63, 0x68, 0x3E });
            FileSignatures.Add(ArchiveFormat.Dmg, new byte[] { 0x78, 0x01, 0x73, 0x0D, 0x62, 0x62, 0x60 });
            FileSignatures.Add(ArchiveFormat.SevenZip, new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C });
            FileSignatures.Add(ArchiveFormat.Tar, new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72 });
            FileSignatures.Add(ArchiveFormat.Iso, new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 });
            FileSignatures.Add(ArchiveFormat.Cab, new byte[] { 0x4D, 0x53, 0x43, 0x46 });
            FileSignatures.Add(ArchiveFormat.Rpm, new byte[] { 0xed, 0xab, 0xee, 0xdb });
            FileSignatures.Add(ArchiveFormat.Xar, new byte[] { 0x78, 0x61, 0x72, 0x21 });
            FileSignatures.Add(ArchiveFormat.Chm, new byte[] { 0x49, 0x54, 0x53, 0x46 });
            FileSignatures.Add(ArchiveFormat.BZip2, new byte[] { 0x42, 0x5A, 0x68 });
            FileSignatures.Add(ArchiveFormat.Flv, new byte[] { 0x46, 0x4C, 0x56 });
            FileSignatures.Add(ArchiveFormat.Swf, new byte[] { 0x46, 0x57, 0x53 });
            FileSignatures.Add(ArchiveFormat.GZip, new byte[] { 0x1f, 0x0b });
            FileSignatures.Add(ArchiveFormat.Zip, new byte[] { 0x50, 0x4b });
            FileSignatures.Add(ArchiveFormat.Arj, new byte[] { 0x60, 0xEA });
            FileSignatures.Add(ArchiveFormat.Lzh, new byte[] { 0x2D, 0x6C, 0x68 });
        }

    }
}
