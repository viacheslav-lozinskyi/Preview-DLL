using PeNet;
using PeNet.Header.Pe;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace resource.preview
{
    internal class VSPreview : extension.AnyPreview
    {
        [DllImport("dbghelp.dll", SetLastError = true, PreserveSig = true)]
        static extern int UnDecorateSymbolName(
            [In][MarshalAs(UnmanagedType.LPStr)] string decoratedName,
            [Out] StringBuilder undecoratedName,
            [In][MarshalAs(UnmanagedType.U4)] int undecoratedSize,
            [In][MarshalAs(UnmanagedType.U4)] int flags);

        internal class HINT
        {
            public static string DATA_TYPE = "[[[Data Type]]]";
            public static string EMPTY = "";
            public static string MODULE_NAME = "<[[[Module Name]]]>";
        }

        internal class TYPE
        {
            public static string BOOLEAN = "[[[Boolean]]]";
            public static string DATE = "[[[Date]]]";
            public static string DIRECTORY = "[[[Directory]]]";
            public static string DOUBLE = "[[[Double]]]";
            public static string ENUM = "[[[Enum]]]";
            public static string FLAGS = "[[[Flags]]]";
            public static string FUNCTION = "[[[Function]]]";
            public static string GUID = "GUID";
            public static string INTEGER = "[[[Integer]]]";
            public static string POINTER = "[[[Pointer]]]";
            public static string SECTION = "[[[Section]]]";
            public static string STRING = "[[[String]]]";
            public static string STRUCT = "[[[Struct]]]";
            public static string VERSION = "[[[Version]]]";
        }

        internal class Info
        {
            static public void Execute(atom.Trace context, int level, PeFile data, string url)
            {
                Send(context, NAME.EVENT.HEADER, level, "[[[Info]]]", "", "", "", "");
                {
                    Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[File Name]]]", url, TYPE.STRING, HINT.DATA_TYPE);
                    Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[File Size]]]", (UInt64)data.FileSize, TYPE.INTEGER, HINT.DATA_TYPE);
                    Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Flags]]]", __GetFlags(data), TYPE.FLAGS, HINT.DATA_TYPE, "");
                }
            }

            static private string __GetFlags(PeFile data)
            {
                var a_Result = "";
                try
                {
                    a_Result = AddFlag(a_Result, "DLL", data.IsDll);
                    a_Result = AddFlag(a_Result, "EXE", data.IsExe);
                    a_Result = AddFlag(a_Result, "[[[DRIVER]]]", data.IsDriver);
                    a_Result = AddFlag(a_Result, "32_[[[BITS]]]", data.Is32Bit);
                    a_Result = AddFlag(a_Result, "64_[[[BITS]]]", data.Is64Bit);
                    a_Result = AddFlag(a_Result, "[[[INVALID_SIGNATURE]]]", data.HasValidAuthenticodeSignature == false);
                    a_Result = AddFlag(a_Result, "[[[SIGNED]]]", data.IsAuthenticodeSigned);
                    //a_Result = AddFlag(a_Result, "[[[DEBUG]]]", data.ImageDebugDirectory != null);
                    //a_Result = AddFlag(a_Result, "[[[RELEASE]]]", data.ImageDebugDirectory == null);
                }
                catch (Exception)
                {
                    a_Result = AddFlag(a_Result, "[[[FAILED]]]", true);
                }
                return a_Result;
            }
        }

        internal class Versions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                try
                {
                    if ((data.Resources != null) && (data.Resources.VsVersionInfo != null) && (data.Resources.VsVersionInfo.StringFileInfo != null) && (data.Resources.VsVersionInfo.StringFileInfo.StringTable != null))
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Versions]]]", "", GetArraySize(data.Resources.VsVersionInfo.StringFileInfo.StringTable.Length), HINT.EMPTY);
                        foreach (var a_Context in data.Resources.VsVersionInfo.StringFileInfo.StringTable)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, string.IsNullOrEmpty(a_Context.ProductName) ? "<[[[UNKNWON]]]>" : a_Context.ProductName, "", TYPE.VERSION, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Product Name]]]", a_Context.ProductName, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Private Build]]]", a_Context.PrivateBuild, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Original Filename]]]", a_Context.OriginalFilename, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Legal Trademarks]]]", a_Context.LegalTrademarks, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Legal Copyright]]]", a_Context.LegalCopyright, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Internal Name]]]", a_Context.InternalName, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[File Version]]]", a_Context.FileVersion, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[File Description]]]", a_Context.FileDescription, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Company Name]]]", a_Context.CompanyName, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Comments]]]", a_Context.Comments, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Product Version]]]", a_Context.ProductVersion, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Special Build]]]", a_Context.SpecialBuild, TYPE.STRING, HINT.DATA_TYPE);
                            }
                            foreach (var a_Context1 in a_Context.String)
                            {
                                if (GetState() == NAME.STATE.WORK.CANCEL)
                                {
                                    return;
                                }
                                else
                                {
                                    if (a_Context1.SzKey == "ProductName") continue;
                                    if (a_Context1.SzKey == "PrivateBuild") continue;
                                    if (a_Context1.SzKey == "OriginalFilename") continue;
                                    if (a_Context1.SzKey == "LegalTrademarks") continue;
                                    if (a_Context1.SzKey == "LegalCopyright") continue;
                                    if (a_Context1.SzKey == "InternalName") continue;
                                    if (a_Context1.SzKey == "FileVersion") continue;
                                    if (a_Context1.SzKey == "FileDescription") continue;
                                    if (a_Context1.SzKey == "CompanyName") continue;
                                    if (a_Context1.SzKey == "Comments") continue;
                                    if (a_Context1.SzKey == "ProductVersion") continue;
                                    if (a_Context1.SzKey == "SpecialBuild") continue;
                                }
                                {
                                    Send(context, NAME.EVENT.PARAMETER, level + 2, a_Context1.SzKey, a_Context1.Value, TYPE.STRING, HINT.DATA_TYPE);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }
        }

        internal class Headers
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                Send(context, NAME.EVENT.FOLDER, level, "[[[Headers]]]", "");
                try
                {
                    if (GetState() == NAME.STATE.WORK.CANCEL)
                    {
                        return;
                    }
                    else
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[File Header]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if ((data.ImageNtHeaders != null) && (data.ImageNtHeaders.FileHeader != null))
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Machine]]]", data.ImageNtHeaders.FileHeader.Machine.ToString(), TYPE.ENUM, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[TimeStamp]]]", GetHex(data.ImageNtHeaders.FileHeader.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Pointer To Symbol Table]]]", data.ImageNtHeaders.FileHeader.PointerToSymbolTable, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Number Of Symbols]]]", data.ImageNtHeaders.FileHeader.NumberOfSymbols, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Number Of Sections]]]", data.ImageNtHeaders.FileHeader.NumberOfSections, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Characteristics]]]", data.ImageNtHeaders.FileHeader.Characteristics.ToString(), TYPE.FLAGS, HINT.DATA_TYPE);
                        }
                    }
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Optional Header]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if ((data.ImageNtHeaders != null) && (data.ImageNtHeaders.OptionalHeader != null))
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[CheckSum]]]", GetHex(data.ImageNtHeaders.OptionalHeader.CheckSum), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address Of Entry Point]]]", GetHex(data.ImageNtHeaders.OptionalHeader.AddressOfEntryPoint), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Image Base]]]", GetHex(data.ImageNtHeaders.OptionalHeader.ImageBase), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Base Of Data]]]", data.ImageNtHeaders.OptionalHeader.BaseOfData, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Base Of Code]]]", data.ImageNtHeaders.OptionalHeader.BaseOfCode, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Heap Commit]]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeapCommit, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Heap Reserve]]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeapReserve, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Stack Commit]]]", data.ImageNtHeaders.OptionalHeader.SizeOfStackCommit, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Stack Reserve]]]", data.ImageNtHeaders.OptionalHeader.SizeOfStackReserve, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Headers]]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeaders, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Image]]]", data.ImageNtHeaders.OptionalHeader.SizeOfImage, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Uninitialized Data]]]", data.ImageNtHeaders.OptionalHeader.SizeOfUninitializedData, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Initialized Data]]]", data.ImageNtHeaders.OptionalHeader.SizeOfInitializedData, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Code]]]", data.ImageNtHeaders.OptionalHeader.SizeOfCode, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[File Alignment]]]", data.ImageNtHeaders.OptionalHeader.FileAlignment, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Section Alignment]]]", data.ImageNtHeaders.OptionalHeader.SectionAlignment, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Characteristics]]]", data.ImageNtHeaders.OptionalHeader.DllCharacteristics.ToString(), TYPE.FLAGS, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Subsystem]]]", data.ImageNtHeaders.OptionalHeader.Subsystem.ToString(), TYPE.ENUM, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Magic]]]", data.ImageNtHeaders.OptionalHeader.Magic.ToString(), TYPE.FLAGS, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Loader Flags]]]", GetHex(data.ImageNtHeaders.OptionalHeader.LoaderFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Number Of]]] RVA [[[And Sizes]]]", data.ImageNtHeaders.OptionalHeader.NumberOfRvaAndSizes, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Image Version]]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorImageVersion, data.ImageNtHeaders.OptionalHeader.MinorImageVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Linker Version]]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorLinkerVersion, data.ImageNtHeaders.OptionalHeader.MinorLinkerVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Subsystem Version]]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorSubsystemVersion, data.ImageNtHeaders.OptionalHeader.MinorSubsystemVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Operating System Version]]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorOperatingSystemVersion, data.ImageNtHeaders.OptionalHeader.MinorOperatingSystemVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "Win32 [[[Version]]]", data.ImageNtHeaders.OptionalHeader.Win32VersionValue, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "DOS [[[Header]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if (data.ImageDosHeader != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "oeminfo", data.ImageDosHeader.E_oeminfo, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "oemid", data.ImageDosHeader.E_oemid, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "ovno", data.ImageDosHeader.E_ovno, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "lfarlc", data.ImageDosHeader.E_lfarlc, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "cs", data.ImageDosHeader.E_cs, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "ip", data.ImageDosHeader.E_ip, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "csum", data.ImageDosHeader.E_csum, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "sp", data.ImageDosHeader.E_sp, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "ss", data.ImageDosHeader.E_ss, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "maxalloc", data.ImageDosHeader.E_maxalloc, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "minalloc", data.ImageDosHeader.E_minalloc, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "cparhdr", data.ImageDosHeader.E_cparhdr, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "crlc", data.ImageDosHeader.E_crlc, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "cp", data.ImageDosHeader.E_cp, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "cblp", data.ImageDosHeader.E_cblp, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "magic", __GetMagic(data.ImageDosHeader.E_magic), TYPE.FLAGS, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 2, "lfanew", data.ImageDosHeader.E_lfanew, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }

            //static private string __GetMachine(ushort value)
            //{
            //    switch ((IMAGE_FILE_HEADER_MACHINE)value)
            //    {
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_UNKNOWN: return "UNKNOWN";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_I386: return "I386";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R3000: return "R3000";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R4000: return "R4000";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R10000: return "R10000";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_WCEMIPSV2: return "WCEMIPSV2";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ALPHA: return "ALPHA";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3: return "SH3";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3DSP: return "SH3DSP";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3E: return "SH3E";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH4: return "SH4";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH5: return "SH5";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ARM: return "ARM";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_THUMB: return "THUMB";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ARMNT: return "ARMNT";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_AM33: return "AM33";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_POWERPC: return "POWERPC";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_POWERPCFP: return "POWERPCFP";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_IA64: return "IA64";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPS16: return "MIPS16";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ALPHA64: return "ALPHA64";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU: return "MIPSFPU";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU16: return "MIPSFPU16";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_TRICORE: return "TRICORE";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_CEF: return "CEF";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_EBC: return "EBC";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_AMD64: return "AMD64";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_M32R: return "M32R";
            //        case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_CEE: return "CEE";
            //    }
            //    return value.ToString();
            //}

            //static private string __GetCharacteristics(int value)
            //{
            //    var a_Result = "";
            //    {
            //        a_Result = AddFlag(a_Result, "RELOCS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_RELOCS_STRIPPED) != 0);
            //        a_Result = AddFlag(a_Result, "EXECUTABLE_IMAGE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_EXECUTABLE_IMAGE) != 0);
            //        a_Result = AddFlag(a_Result, "LINE_NUMS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LINE_NUMS_STRIPPED) != 0);
            //        a_Result = AddFlag(a_Result, "LOCAL_SYMS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LOCAL_SYMS_STRIPPED) != 0);
            //        a_Result = AddFlag(a_Result, "AGGRESIVE_WS_TRIM", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_AGGRESIVE_WS_TRIM) != 0);
            //        a_Result = AddFlag(a_Result, "LARGE_ADDRESS_AWARE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LARGE_ADDRESS_AWARE) != 0);
            //        a_Result = AddFlag(a_Result, "BYTES_REVERSED_LO", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_BYTES_REVERSED_LO) != 0);
            //        a_Result = AddFlag(a_Result, "32BIT_MACHINE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_32BIT_MACHINE) != 0);
            //        a_Result = AddFlag(a_Result, "DEBUG_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_DEBUG_STRIPPED) != 0);
            //        a_Result = AddFlag(a_Result, "REMOVABLE_RUN_FROM_SWAP", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP) != 0);
            //        a_Result = AddFlag(a_Result, "NET_RUN_FROM_SWAP", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_NET_RUN_FROM_SWAP) != 0);
            //        a_Result = AddFlag(a_Result, "SYSTEM", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_SYSTEM) != 0);
            //        a_Result = AddFlag(a_Result, "DLL", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_DLL) != 0);
            //        a_Result = AddFlag(a_Result, "UP_SYSTEM_ONLY", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_UP_SYSTEM_ONLY) != 0);
            //        a_Result = AddFlag(a_Result, "BYTES_REVERSED_HI", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_BYTES_REVERSED_HI) != 0);
            //    }
            //    return a_Result;
            //}

            static private string __GetMagic(int value)
            {
                var a_Result = "";
                {
                    a_Result = AddFlag(a_Result, "HDR32_MAGIC", (value & (int)IMAGE_OPTIONAL_HEADER_MAGIC.IMAGE_NT_OPTIONAL_HDR32_MAGIC) != 0);
                    a_Result = AddFlag(a_Result, "HDR64_MAGIC", (value & (int)IMAGE_OPTIONAL_HEADER_MAGIC.IMAGE_NT_OPTIONAL_HDR64_MAGIC) != 0);
                    a_Result = AddFlag(a_Result, "HDR_MAGIC", (value & (int)IMAGE_OPTIONAL_HEADER_MAGIC.IMAGE_ROM_OPTIONAL_HDR_MAGIC) != 0);
                }
                return a_Result;
            }

            //static private string __GetSubsystem(int value)
            //{
            //    switch ((IMAGE_OPTIONAL_HEADER_SUBSYSTEM)value)
            //    {
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_UNKNOWN: return "UNKNOWN";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_NATIVE: return "NATIVE";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_GUI: return "WINDOWS_GUI";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_CUI: return "WINDOWS_CUI";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_OS2_CUI: return "OS2_CUI";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_POSIX_CUI: return "POSIX_CUI";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_CE_GUI: return "WINDOWS_CE_GUI";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_APPLICATION: return "EFI_APPLICATION";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER: return "EFI_BOOT_SERVICE_DRIVER";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER: return "EFI_RUNTIME_DRIVER";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_ROM: return "EFI_ROM";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_XBOX: return "XBOX";
            //        case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION: return "WINDOWS_BOOT_APPLICATION";
            //    }
            //    return value.ToString();
            //}

            //static private string __GetDllCharacteristics(int value)
            //{
            //    var a_Result = "";
            //    {
            //        a_Result = AddFlag(a_Result, "DYNAMIC_BASE", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE) != 0);
            //        a_Result = AddFlag(a_Result, "FORCE_INTEGRITY", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY) != 0);
            //        a_Result = AddFlag(a_Result, "NX_COMPAT", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NX_COMPAT) != 0);
            //        a_Result = AddFlag(a_Result, "NO_ISOLATION", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_ISOLATION) != 0);
            //        a_Result = AddFlag(a_Result, "NO_SEH", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_SEH) != 0);
            //        a_Result = AddFlag(a_Result, "NO_BIND", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_BIND) != 0);
            //        a_Result = AddFlag(a_Result, "WDM_DRIVER", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_WDM_DRIVER) != 0);
            //        a_Result = AddFlag(a_Result, "TERMINAL_SERVER_AWARE", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE) != 0);
            //    }
            //    return a_Result;
            //}
        }

        internal class Metadata
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                //Send(context, NAME.EVENT.FOLDER, level, "[[[Metadata]]]", "");
                //{
                // TODO: Implement visualization of .NET metadata
                //}
            }
        }

        internal class ExportFunctions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                try
                {
                    if ((data.ExportedFunctions != null) && (data.ExportedFunctions.Length > 0) /*&& data.HasValidExportDir*/)
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Export Functions]]]", "", GetArraySize(data.ExportedFunctions), HINT.EMPTY);
                        foreach (var a_Context in data.ExportedFunctions)
                        {
                            Send(context, (a_Context.HasName ? NAME.EVENT.FUNCTION : NAME.EVENT.WARNING), level + 1, GetFunctionName(a_Context.Name, a_Context.HasName), "", TYPE.FUNCTION, HINT.DATA_TYPE, "");
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            if (a_Context.HasForward)
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Forward Name]]]", a_Context.ForwardName, TYPE.STRING, HINT.DATA_TYPE);
                            }
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(a_Context.Address), TYPE.POINTER, HINT.DATA_TYPE);
                            }
                            if (a_Context.HasOrdinal)
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Ordinal]]]", a_Context.Ordinal, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }
        }

        internal class ImportFunctions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                try
                {
                    if ((data.ImportedFunctions != null) && (data.ImportedFunctions.Length > 0) /*&& data.HasValidImportDir*/)
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Import Functions]]]", "", GetArraySize(data.ImportedFunctions), HINT.EMPTY);
                        foreach (var a_Context in data.ImportedFunctions)
                        {
                            var a_Name = GetFunctionName(a_Context.Name, true);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            if (string.IsNullOrEmpty(a_Name) == false)
                            {
                                var a_Context1 = GetModuleName(a_Context.DLL) + " [" + a_Context.Hint.ToString() + ":" + a_Context.IATOffset.ToString() + "]";
                                {
                                    Send(context, (string.IsNullOrEmpty(a_Context.Name) ? NAME.EVENT.WARNING : NAME.EVENT.FUNCTION), level + 1, a_Name, "", a_Context1, HINT.MODULE_NAME + " [Hint : IAT Offset]", a_Context.DLL);
                                }
                                if (a_Name != a_Context.Name)
                                {
                                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Name]]]", a_Context.Name, TYPE.STRING, HINT.DATA_TYPE);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }
        }

        internal class ImportModules
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                try
                {
                    var a_Context = "";
                    var a_Count = __GetCount(data);
                    if (a_Count > 0)
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Import Modules]]]", "", GetArraySize(a_Count), HINT.EMPTY);
                        foreach (var a_Context1 in data.ImportedFunctions)
                        {
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            if (string.IsNullOrEmpty(a_Context1.DLL))
                            {
                                continue;
                            }
                            if (a_Context.Contains(a_Context1.DLL.ToUpper() + "\n"))
                            {
                                continue;
                            }
                            {
                                Send(context, NAME.EVENT.FILE, level + 1, a_Context1.DLL, "", "[[[Module]]]", HINT.DATA_TYPE, a_Context1.DLL);
                            }
                            {
                                a_Context += a_Context1.DLL.ToUpper() + "\n";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }

            static private int __GetCount(PeFile data)
            {
                var a_Result = 0;
                if (data.ImportedFunctions != null)
                {
                    var a_Context = "";
                    foreach (var a_Context1 in data.ImportedFunctions)
                    {
                        if (string.IsNullOrEmpty(a_Context1.DLL))
                        {
                            continue;
                        }
                        if (a_Context.Contains(a_Context1.DLL.ToUpper() + "\n"))
                        {
                            continue;
                        }
                        {
                            a_Result++;
                        }
                        {
                            a_Context += a_Context1.DLL.ToUpper() + "\n";
                        }
                    }
                }
                return a_Result;
            }
        }

        internal class Directories
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                var a_IsFound =
                    (data.ImageDebugDirectory != null) ||
                    (data.ImageRelocationDirectory != null) ||
                    (data.ImageResourceDirectory != null) ||
                    (data.ExceptionDirectory != null) ||
                    (data.ImageTlsDirectory != null) ||
                    (data.ImageLoadConfigDirectory != null) ||
                    (data.ImageExportDirectory != null) ||
                    (data.ImageImportDescriptors != null) ||
                    (data.ImageBoundImportDescriptor != null) ||
                    (data.ImageDelayImportDescriptor != null) ||
                    (data.ImageComDescriptor != null);
                if (a_IsFound)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Directories]]]", "");
                    try
                    {
                        __Execute(context, level + 1, data);
                        __Execute(context, level + 1, data.ImageDebugDirectory);
                        __Execute(context, level + 1, data.ImageRelocationDirectory);
                        __Execute(context, level + 1, data.ImageResourceDirectory);
                        __Execute(context, level + 1, data.ExceptionDirectory);
                        __Execute(context, level + 1, data.ImageTlsDirectory);
                        __Execute(context, level + 1, data.ImageLoadConfigDirectory);
                        __Execute(context, level + 1, data.ImageExportDirectory);
                        __Execute(context, level + 1, data.ImageImportDescriptors);
                        __Execute(context, level + 1, data.ImageBoundImportDescriptor);
                        __Execute(context, level + 1, data.ImageDelayImportDescriptor);
                        __Execute(context, level + 1, data.ImageComDescriptor);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, PeFile data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Security]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "SHA256", data.Sha256, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "SHA1", data.Sha1, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "MD5", data.Md5, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "ImpHash", data.ImpHash, TYPE.STRING, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageDebugDirectory[] data)
            {
                if (data != null)
                {
                    try
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Debug]]]", "", GetArraySize(data.Length), HINT.EMPTY);
                        foreach (var a_Context in data)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, GetHex(a_Context.PointerToRawData), "", TYPE.STRUCT, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[TimeStamp]]]", GetHex(a_Context.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Version]]]", GetVersion(a_Context.MajorVersion, a_Context.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Type]]]", a_Context.Type, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Data]]]", a_Context.SizeOfData, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address Of Raw Data]]]", GetHex(a_Context.AddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Pointer To Raw Data]]]", GetHex(a_Context.PointerToRawData), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Characteristics]]]", GetHex(a_Context.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                            }
                            if (a_Context.CvInfoPdb70 != null)
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "PDB [[[File Name]]]", a_Context.CvInfoPdb70.PdbFileName, TYPE.STRING, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Signature]]]", a_Context.CvInfoPdb70.Signature.ToString().ToUpper(), TYPE.GUID, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "CV [[[Signature]]]", GetHex(a_Context.CvInfoPdb70.CvSignature), TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Age]]]", a_Context.CvInfoPdb70.Age, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageBaseRelocation[] data)
            {
                if (data != null)
                {
                    try
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Relocation]]]", "", GetArraySize(data.Length), HINT.EMPTY);
                        foreach (var a_Context in data)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, GetHex(a_Context.VirtualAddress), "", TYPE.STRUCT, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Virtual Address]]]", GetHex(a_Context.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Block]]]", a_Context.SizeOfBlock, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                            // TODO: Uncomment it after performance issue will be solved
                            //if ((a_Context.TypeOffsets != null) && (a_Context.TypeOffsets.Length > 0))
                            //{
                            //    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Type Offsets]]]", "", GetArraySize(a_Context.TypeOffsets.Length), HINT.EMPTY);
                            //    foreach (var a_Context1 in a_Context.TypeOffsets)
                            //    {
                            //        Send(context, NAME.EVENT.PARAMETER, level + 3, GetHex(a_Context1.Offset), a_Context1.Type.ToString(), TYPE.INTEGER, HINT.DATA_TYPE);
                            //    }
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageResourceDirectory data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Resource]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[TimeStamp]]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Version]]]", GetVersion(data.MajorVersion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Number Of Name Entries]]]", data.NumberOfNameEntries, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Number Of Id Entries]]]", data.NumberOfIdEntries, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Characteristics]]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, RuntimeFunction[] data)
            {
                if (data != null)
                {
                    try
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Exceptions]]]", "", GetArraySize(data.Length), HINT.EMPTY);
                        foreach (var a_Context in data)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, GetHex(a_Context.FunctionStart), "", TYPE.FUNCTION, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Function Start]]]", GetHex(a_Context.FunctionStart), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Function End]]]", GetHex(a_Context.FunctionEnd), TYPE.POINTER, HINT.DATA_TYPE);
                            }
                            if (a_Context.ResolvedUnwindInfo != null)
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Version]]]", a_Context.ResolvedUnwindInfo.Version, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Flags]]]", GetHex(a_Context.ResolvedUnwindInfo.Flags), TYPE.FLAGS, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Prologue]]]", a_Context.ResolvedUnwindInfo.SizeOfProlog, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Count Of Codes]]]", a_Context.ResolvedUnwindInfo.CountOfCodes, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Frame Register]]]", a_Context.ResolvedUnwindInfo.FrameRegister, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Frame Offset]]]", a_Context.ResolvedUnwindInfo.FrameOffset, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Exception Handler]]]", GetHex(a_Context.ResolvedUnwindInfo.ExceptionHandler), TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageTlsDirectory data)
            {
                if (data != null)
                {
                    try
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Thread Local Storage]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                        if (GetState() == NAME.STATE.WORK.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Start Address Of Raw Data]]]", GetHex(data.StartAddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[End Address Of Raw Data]]]", GetHex(data.EndAddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Address Of Index]]]", GetHex(data.AddressOfIndex), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Address Of CallBacks]]]", GetHex(data.AddressOfCallBacks), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Size Of Zero Fill]]]", data.SizeOfZeroFill, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Characteristics]]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                        }
                        if ((data.TlsCallbacks != null) && (data.TlsCallbacks.Length > 0))
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Callbacks]]]", "", GetArraySize(data.TlsCallbacks.Length), HINT.EMPTY);
                            foreach (var a_Context in data.TlsCallbacks)
                            {
                                if (GetState() == NAME.STATE.WORK.CANCEL)
                                {
                                    return;
                                }
                                else
                                {
                                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Callback]]]", GetHex(a_Context.Callback), TYPE.POINTER, HINT.DATA_TYPE);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageLoadConfigDirectory data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Load Config]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[TimeStamp]]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Version]]]", GetVersion(data.MajorVesion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Size]]]", data.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Guard]]] CF [[[Function Table]]]", GetHex(data.GuardCFFunctionTable), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Guard]]] CF [[[Check Function Pointer]]]", GetHex(data.GuardCFCheckFunctionPointer), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Guard]]] CF [[[Function Count]]]", data.GuardCFFunctionCount, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "SE [[[Handler Count]]]", data.SEHandlerCount, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "SE [[[Handler Table]]]", data.SEHandlerTable, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Security Cookie]]]", GetHex(data.SecurityCoockie), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Edit List]]]", data.EditList, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "CSD [[[Version]]]", data.CSDVersion, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Process Affinity Mask]]]", data.ProcessAffinityMask, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Maximum Allocation Size]]]", data.MaximumAllocationSize, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Lock Prefix Table]]]", data.LockPrefixTable, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Virtual Memory Threshold]]]", data.VirtualMemoryThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[DeCommit Total Free Threshold]]]", data.DeCommitTotalFreeThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[DeCommit Free Block Threshold]]]", data.DeCommitFreeBlockThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Critical Section Default Timeout]]]", data.CriticalSectionDefaultTimeout, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Global Flags Set]]]", data.GlobalFlagsSet, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Global Flags Clear]]]", data.GlobalFlagsClear, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Process Heap Flags]]]", GetHex(data.ProcessHeapFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Guard Flags]]]", GetHex(data.GuardFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Reserved1]]]", GetHex(data.Reserved1), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Reserved2]]]", GetHex(data.Reserved2), TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageExportDirectory data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Export]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[TimeStamp]]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Version]]]", GetVersion(data.MajorVersion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Name]]]", data.Name, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Base]]]", data.Base, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Number Of Functions]]]", data.NumberOfFunctions, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Number Of Names]]]", data.NumberOfNames, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Address Of Functions]]]", GetHex(data.AddressOfFunctions), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Address Of Names]]]", GetHex(data.AddressOfNames), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Address Of Name Ordinals]]]", GetHex(data.AddressOfNameOrdinals), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Characteristics]]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageImportDescriptor[] data)
            {
                if (data != null)
                {
                    try
                    {
                        Send(context, NAME.EVENT.FOLDER, level, "[[[Import]]]", "", GetArraySize(data.Length), HINT.EMPTY);
                        foreach (var a_Context in data)
                        {
                            Send(context, NAME.EVENT.FOLDER, level + 1, GetHex(a_Context.Name), "", TYPE.STRUCT, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[TimeStamp]]]", GetHex(a_Context.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Name]]]", GetHex(a_Context.Name), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Original First Thunk]]]", GetHex(a_Context.OriginalFirstThunk), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Forwarder Chain]]]", GetHex(a_Context.ForwarderChain), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[First Thunk]]]", GetHex(a_Context.FirstThunk), TYPE.POINTER, HINT.DATA_TYPE);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageBoundImportDescriptor data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Bound Import]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[TimeStamp]]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Offset Module Name]]]", GetHex(data.OffsetModuleName), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Number Of Module Forwarder Refs]]]", data.NumberOfModuleForwarderRefs, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageDelayImportDescriptor data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Delay Import]]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[TimeStamp]]]", GetHex(data.DwTimeStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Attributes]]]", GetHex(data.GrAttrs), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Name]]]", GetHex(data.SzName), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Mod]]]", GetHex(data.Phmod), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[PBound IAT]]]", GetHex(data.PBoundIAT), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[INT]]]", GetHex(data.PInt), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Unload IAT]]] IAT", GetHex(data.PUnloadIAT), TYPE.POINTER, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, ImageCor20Header data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "COM", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    try
                    {
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Runtime Version]]]", GetVersion(data.MajorRuntimeVersion, data.MinorRuntimeVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Flags]]]", data.Flags.ToString(), TYPE.FLAGS, HINT.DATA_TYPE);
                        }
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Entry Point]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Token]]]", data.EntryPointToken, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Relative Virtual Address]]]", data.EntryPointRva, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.MetaData != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Metadata]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.MetaData.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.MetaData.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.Resources != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Resources]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.Resources.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.Resources.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.StrongNameSignature != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Strong Name Signature]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.StrongNameSignature.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.StrongNameSignature.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.CodeManagerTable != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Code Manager Table]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.CodeManagerTable.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.CodeManagerTable.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.VTableFixups != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Virtual Table Fix Up]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.VTableFixups.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.VTableFixups.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.ExportAddressTableJumps != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Export Address Table Jumps]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.ExportAddressTableJumps.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.ExportAddressTableJumps.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                        if (data.ManagedNativeHeader != null)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Managed Native Header]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Address]]]", GetHex(data.ManagedNativeHeader.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", data.ManagedNativeHeader.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }
        }

        internal class Sections
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                try
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Sections]]]", "", GetArraySize(data.ImageSectionHeaders), HINT.EMPTY);
                    if (data.ImageSectionHeaders != null)
                    {
                        foreach (var a_Context in data.ImageSectionHeaders)
                        {
                            Send(context, NAME.EVENT.PARAMETER, level + 1, a_Context.Name, "", TYPE.SECTION, HINT.DATA_TYPE);
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                return;
                            }
                            else
                            {
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Image Base Address]]]", GetHex(a_Context.ImageBaseAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Virtual Address]]]", GetHex(a_Context.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Virtual Size]]]", a_Context.VirtualSize, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size Of Raw Data]]]", a_Context.SizeOfRawData, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Pointer To Raw Data]]]", GetHex(a_Context.PointerToRawData), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Pointer To Relocations]]]", GetHex(a_Context.PointerToRelocations), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Pointer To Line Numbers]]]", GetHex(a_Context.PointerToLinenumbers), TYPE.POINTER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Number Of Relocations]]]", a_Context.NumberOfRelocations, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Number Of Line Numbers]]]", a_Context.NumberOfLinenumbers, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Characteristics]]]", a_Context.Characteristics.ToString(), TYPE.FLAGS, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                }
            }

            //static private string __GetCharacteristics(uint value)
            //{
            //    var a_Result = "";
            //    {
            //        a_Result = AddFlag(a_Result, "TYPE_NO_PAD", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_TYPE_NO_PAD) != 0);
            //        a_Result = AddFlag(a_Result, "CNT_CODE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_CODE) != 0);
            //        a_Result = AddFlag(a_Result, "CNT_INITIALIZED_DATA", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_INITIALIZED_DATA) != 0);
            //        a_Result = AddFlag(a_Result, "CNT_UNINITIALIZED_DATA", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0);
            //        a_Result = AddFlag(a_Result, "LNK_OTHER", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_OTHER) != 0);
            //        a_Result = AddFlag(a_Result, "LNK_INFO", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_INFO) != 0);
            //        a_Result = AddFlag(a_Result, "LNK_REMOVE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_REMOVE) != 0);
            //        a_Result = AddFlag(a_Result, "LNK_COMDAT", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_COMDAT) != 0);
            //        a_Result = AddFlag(a_Result, "NO_DEFER_SPEC_EXC", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_NO_DEFER_SPEC_EXC) != 0);
            //        a_Result = AddFlag(a_Result, "GPREL", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_GPREL) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_PURGEABLE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_PURGEABLE) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_LOCKED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_LOCKED) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_PRELOAD", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_PRELOAD) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_1BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_1BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_2BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_2BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_4BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_4BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_8BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_8BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_16BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_16BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_32BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_32BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_64BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_64BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_128BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_128BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_256BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_256BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_512BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_512BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_1024BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_1024BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_2048BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_2048BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_4096BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_4096BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "ALIGN_8192BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_8192BYTES) != 0);
            //        a_Result = AddFlag(a_Result, "LNK_NRELOC_OVFL", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_NRELOC_OVFL) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_DISCARDABLE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_DISCARDABLE) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_NOT_CACHED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_NOT_CACHED) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_NOT_PAGED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_NOT_PAGED) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_SHARED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_SHARED) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_EXECUTE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_EXECUTE) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_READ", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_READ) != 0);
            //        a_Result = AddFlag(a_Result, "MEM_WRITE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_WRITE) != 0);
            //    }
            //    return a_Result;
            //}
        }

        internal class Certificate
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                if (data.WinCertificate != null)
                {
                    Send(context, NAME.EVENT.FOLDER, level, "[[[Certificate]]]", "");
                    try
                    {
                        __Execute(context, level + 1, data.WinCertificate);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, WinCertificate data)
            {
                if (data != null)
                {
                    Send(context, NAME.EVENT.PARAMETER, level, "Windows", "");
                    try
                    {
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Type]]]", data.WCertificateType.ToString(), TYPE.ENUM, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Size]]]", data.DwLength, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Revision]]]", data.WRevision, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                    catch (Exception ex)
                    {
                        Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
                    }
                }
            }

            //static private void __Execute(atom.Trace context, int level, X509Certificate2 data)
            //{
            //    if (data != null)
            //    {
            //        try
            //        {
            //            Send(context, NAME.EVENT.PARAMETER, level, "X509", "");
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Friendly Name]]]", data.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Subject Name]]]", data.SubjectName.Name, TYPE.STRING, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Issuer Name]]]", (data.IssuerName != null) ? data.IssuerName.Name : "", TYPE.STRING, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Serial Number]]]", data.SerialNumber, TYPE.STRING, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Thumb Print]]]", data.Thumbprint, TYPE.STRING, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Archived]]]", GetBool(data.Archived), TYPE.BOOLEAN, HINT.DATA_TYPE);
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Version]]]", (uint)data.Version, TYPE.INTEGER, HINT.DATA_TYPE);
            //            }
            //            if (data.NotBefore != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Not Before]]]", data.NotBefore.ToString(), TYPE.DATE, HINT.DATA_TYPE);
            //            }
            //            if (data.NotAfter != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Not After]]]", data.NotAfter.ToString(), TYPE.DATE, HINT.DATA_TYPE);
            //            }
            //            if (data.PublicKey != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Public Key]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
            //                if (data.PublicKey.Oid != null)
            //                {
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Friendly Name]]]", data.PublicKey.Oid.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Value]]]", data.PublicKey.Oid.Value, TYPE.STRING, HINT.DATA_TYPE);
            //                }
            //                if (data.PublicKey.Key != null)
            //                {
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Exchange Algorithm]]]", data.PublicKey.Key.KeyExchangeAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", (uint)data.PublicKey.Key.KeySize, TYPE.INTEGER, HINT.DATA_TYPE);
            //                }
            //            }
            //            if (data.PrivateKey != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Private Key]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
            //                {
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Exchange Algorithm]]]", data.PrivateKey.KeyExchangeAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Signature Algorithm]]]", data.PrivateKey.SignatureAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Size]]]", (uint)data.PrivateKey.KeySize, TYPE.INTEGER, HINT.DATA_TYPE);
            //                }
            //            }
            //            if (data.SignatureAlgorithm != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Signature Algorithm]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
            //                {
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Friendly Name]]]", data.SignatureAlgorithm.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
            //                    Send(context, NAME.EVENT.PARAMETER, level + 2, "[[[Value]]]", data.SignatureAlgorithm.Value, TYPE.STRING, HINT.DATA_TYPE);
            //                }
            //            }
            //            if (data.Extensions != null)
            //            {
            //                Send(context, NAME.EVENT.PARAMETER, level + 1, "[[[Extensions]]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
            //                foreach (var a_Context in data.Extensions)
            //                {
            //                    if (GetState() == NAME.STATE.WORK.CANCEL)
            //                    {
            //                        return;
            //                    }
            //                    if (a_Context.Oid != null)
            //                    {
            //                        Send(context, NAME.EVENT.PARAMETER, level + 2, a_Context.Oid.FriendlyName, "", TYPE.STRUCT, HINT.DATA_TYPE);
            //                        {
            //                            Send(context, NAME.EVENT.PARAMETER, level + 3, "[[[Friendly Name]]]", a_Context.Oid.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
            //                            Send(context, NAME.EVENT.PARAMETER, level + 3, "[[[Value]]]", a_Context.Oid.Value, TYPE.STRING, HINT.DATA_TYPE);
            //                            Send(context, NAME.EVENT.PARAMETER, level + 3, "[[[Critical]]]", GetBool(a_Context.Critical), TYPE.BOOLEAN, HINT.DATA_TYPE);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Send(context, NAME.EVENT.ERROR, level + 1, ex.Message, "");
            //        }
            //    }
            //}

            //static private string __GetType(int value)
            //{
            //    switch ((WIN_CERTIFICATE_TYPE)value)
            //    {
            //        case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_PKCS_SIGNED_DATA: return "PKCS_SIGNED_DATA";
            //        case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_EFI_PKCS115: return "EFI_PKCS115";
            //        case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_EFI_GUID: return "EFI_GUID";
            //    }
            //    return value.ToString();
            //}
        }

        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            var a_Context = (PeFile)null;
            if (PeFile.TryParse(file, out a_Context))
            {
                {
                    context.
                        SetTrace(null, NAME.STATE.TRACE.BLINK).
                        SetProgress(CONSTANT.PROGRESS.INFINITE).
                        SendPreview(NAME.EVENT.INFO, url);
                }
                {
                    Info.Execute(context, level, a_Context, url);
                    Versions.Execute(context, level, a_Context);
                    Headers.Execute(context, level, a_Context);
                    Metadata.Execute(context, level, a_Context);
                    ExportFunctions.Execute(context, level, a_Context);
                    ImportFunctions.Execute(context, level, a_Context);
                    ImportModules.Execute(context, level, a_Context);
                    Directories.Execute(context, level, a_Context);
                    Sections.Execute(context, level, a_Context);
                    Certificate.Execute(context, level, a_Context);
                }
                {
                    context.
                        SetFont(null, 0, NAME.STATE.FONT.NONE).
                        SetProgress(100).
                        SendPreview(NAME.EVENT.INFO, url);
                }
            }
            else
            {
                Send(context, NAME.EVENT.ERROR, level, "[[[This is not]]] PE [[[data format]]]", "", "", HINT.EMPTY, "");
            }
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value, string comment, string hint, string url)
        {
            context.
                SetComment(comment, hint).
                SetUrl(url).
                Send(NAME.SOURCE.PREVIEW, type, level, GetFinalText(name), GetFinalText(value));
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value, string comment, string hint, string url)
        {
            context.
                SetComment(comment, hint).
                SetUrl(url).
                Send(NAME.SOURCE.PREVIEW, type, level, GetFinalText(name), value.ToString());
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value, string comment, string commentHint)
        {
            Send(context, type, level, name, value, comment, commentHint, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value, string comment, string commentHint)
        {
            Send(context, type, level, name, value, comment, commentHint, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value)
        {
            Send(context, type, level, name, value, "", HINT.EMPTY, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value)
        {
            Send(context, type, level, name, value, "", HINT.EMPTY, "");
        }

        internal static string AddFlag(string context, string value, bool isEnabled)
        {
            if (isEnabled)
            {
                if (string.IsNullOrEmpty(context))
                {
                    return value;
                }
                return context + "; " + value;
            }
            return context;
        }

        internal static string GetArraySize(Array data)
        {
            if ((data != null) && (data is Array))
            {
                return "[[[Found]]]: " + (data as Array).Length.ToString();
            }
            return "[[[Not found]]]";
        }

        internal static string GetArraySize(int data)
        {
            return (data != 0) ? "[[[Found]]]: " + data.ToString() : "[[[Not found]]]";
        }

        internal static string GetVersion(ushort major, ushort minor)
        {
            return major.ToString() + "." + minor.ToString();
        }

        internal static string GetHex(UInt64 data)
        {
            return "0x" + data.ToString((data > UInt32.MaxValue) ? "X16" : "X8");
        }

        internal static string GetBool(bool data)
        {
            return data ? "[[[true]]]" : "[[[false]]]";
        }

        internal static string GetFunctionName(string data, bool isEnabled)
        {
            if (isEnabled && (string.IsNullOrEmpty(data) == false))
            {
                try
                {
                    var a_Result = new StringBuilder(1024);
                    {
                        UnDecorateSymbolName(data, a_Result, Int32.MaxValue, 0);
                    }
                    return a_Result.ToString();
                }
                catch (Exception)
                {
                }
                return data;
            }
            return "[[[UNDEFINED]]]";
        }

        internal static string GetModuleName(string data)
        {
            if (string.IsNullOrEmpty(data) == false)
            {
                return "<" + data + ">";
            }
            return "";
        }
    };
}
