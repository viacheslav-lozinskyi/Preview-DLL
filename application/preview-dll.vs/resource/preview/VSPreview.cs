
using PeNet;
using PeNet.Structures;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace resource.preview
{
    internal class VSPreview : cartridge.AnyPreview
    {
        [DllImport("dbghelp.dll", SetLastError = true, PreserveSig = true)]
        static extern int UnDecorateSymbolName(
            [In][MarshalAs(UnmanagedType.LPStr)] string decoratedName,
            [Out] StringBuilder undecoratedName,
            [In][MarshalAs(UnmanagedType.U4)] int undecoratedSize,
            [In][MarshalAs(UnmanagedType.U4)] int flags);

        internal class HINT
        {
            public static string DATA_TYPE = "[[Data type]]";
            public static string EMPTY = "";
            public static string MODULE_NAME = "<[[Module name]]>";
        }

        internal class TYPE
        {
            public static string BOOLEAN = "[[Boolean]]";
            public static string DATE = "[[Date]]";
            public static string DIRECTORY = "[[Directory]]";
            public static string DOUBLE = "[[Double]]";
            public static string ENUM = "[[Enum]]";
            public static string FLAGS = "[[Flags]]";
            public static string FUNCTION = "[[Function]]";
            public static string GUID = "GUID";
            public static string INTEGER = "[[Integer]]";
            public static string POINTER = "[[Pointer]]";
            public static string SECTION = "[[Section]]";
            public static string STRING = "[[String]]";
            public static string STRUCT = "[[Struct]]";
            public static string VERSION = "[[Version]]";
        }

        internal class Info
        {
            static public void Execute(atom.Trace context, int level, PeFile data, string url)
            {
                Send(context, NAME.TYPE.FOLDER, level, "[[Info]]", "", "", "", NAME.STATE.HEADER, "");
                {
                    Send(context, NAME.TYPE.VARIABLE, level + 1, "[[File Name]]", url, TYPE.STRING, HINT.DATA_TYPE);
                    Send(context, NAME.TYPE.VARIABLE, level + 1, "[[File Size]]", (UInt64)data.FileSize, TYPE.INTEGER, HINT.DATA_TYPE);
                    Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Flags]]", __GetFlags(data), TYPE.FLAGS, HINT.DATA_TYPE, NAME.STATE.NONE, "");
                }
            }

            static private string __GetFlags(PeFile data)
            {
                var a_Result = "";
                try
                {
                    a_Result = AddFlag(a_Result, "DLL", data.IsDLL);
                    a_Result = AddFlag(a_Result, "EXE", data.IsEXE);
                    a_Result = AddFlag(a_Result, "[[DRIVER]]", data.IsDriver);
                    a_Result = AddFlag(a_Result, "32_[[BITS]]", data.Is32Bit);
                    a_Result = AddFlag(a_Result, "64_[[BITS]]", data.Is64Bit);
                    a_Result = AddFlag(a_Result, "[[INVALID_SIGNATURE]]", data.IsSignatureValid == false);
                    a_Result = AddFlag(a_Result, "[[SIGNED]]", data.IsSigned);
                    //a_Result = AddFlag(a_Result, "[[DEBUG]]", data.ImageDebugDirectory != null);
                    //a_Result = AddFlag(a_Result, "[[RELEASE]]", data.ImageDebugDirectory == null);
                }
                catch (Exception)
                {
                    a_Result = AddFlag(a_Result, "[[FAILED]]", true);
                }
                return a_Result;
            }
        }

        internal class Versions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                if ((data.Resources != null) && (data.Resources.VsVersionInfo != null) && (data.Resources.VsVersionInfo.StringFileInfo != null) && (data.Resources.VsVersionInfo.StringFileInfo.StringTable != null))
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Versions]]", "", GetArraySize(data.Resources.VsVersionInfo.StringFileInfo.StringTable.Length), HINT.EMPTY);
                    foreach (var a_Context in data.Resources.VsVersionInfo.StringFileInfo.StringTable)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, string.IsNullOrEmpty(a_Context.ProductName) ? "<[[UNKNWON]]>" : a_Context.ProductName, "", TYPE.VERSION, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Product Name]]", a_Context.ProductName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Private Build]]", a_Context.PrivateBuild, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Original Filename]]", a_Context.OriginalFilename, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Legal Trademarks]]", a_Context.LegalTrademarks, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Legal Copyright]]", a_Context.LegalCopyright, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Internal Name]]", a_Context.InternalName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[File Version]]", a_Context.FileVersion, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[File Description]]", a_Context.FileDescription, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Company Name]]", a_Context.CompanyName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Comments]]", a_Context.Comments, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Product Version]]", a_Context.ProductVersion, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Special Build]]", a_Context.SpecialBuild, TYPE.STRING, HINT.DATA_TYPE);
                        }
                        foreach (var a_Context1 in a_Context.String)
                        {
                            {
                                if (a_Context1.szKey == "ProductName") continue;
                                if (a_Context1.szKey == "PrivateBuild") continue;
                                if (a_Context1.szKey == "OriginalFilename") continue;
                                if (a_Context1.szKey == "LegalTrademarks") continue;
                                if (a_Context1.szKey == "LegalCopyright") continue;
                                if (a_Context1.szKey == "InternalName") continue;
                                if (a_Context1.szKey == "FileVersion") continue;
                                if (a_Context1.szKey == "FileDescription") continue;
                                if (a_Context1.szKey == "CompanyName") continue;
                                if (a_Context1.szKey == "Comments") continue;
                                if (a_Context1.szKey == "ProductVersion") continue;
                                if (a_Context1.szKey == "SpecialBuild") continue;
                            }
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 2, a_Context1.szKey, a_Context1.Value, TYPE.STRING, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
            }
        }

        internal class Headers
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                Send(context, NAME.TYPE.FOLDER, level, "[[Headers]]", "");
                if (GetState() == STATE.CANCEL)
                {
                    return;
                }
                else
                {
                    Send(context, NAME.TYPE.INFO, level + 1, "[[File Header]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                    if ((data.ImageNtHeaders != null) && (data.ImageNtHeaders.FileHeader != null))
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Machine]]", __GetMachine(data.ImageNtHeaders.FileHeader.Machine), TYPE.ENUM, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[TimeStamp]]", GetHex(data.ImageNtHeaders.FileHeader.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Pointer To Symbol Table]]", data.ImageNtHeaders.FileHeader.PointerToSymbolTable, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Number Of Symbols]]", data.ImageNtHeaders.FileHeader.NumberOfSymbols, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Number Of Sections]]", data.ImageNtHeaders.FileHeader.NumberOfSections, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Characteristics]]", __GetCharacteristics(data.ImageNtHeaders.FileHeader.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                }
                {
                    Send(context, NAME.TYPE.INFO, level + 1, "[[Optional Header]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                    if ((data.ImageNtHeaders != null) && (data.ImageNtHeaders.OptionalHeader != null))
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[CheckSum]]", GetHex(data.ImageNtHeaders.OptionalHeader.CheckSum), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address Of Entry Point]]", GetHex(data.ImageNtHeaders.OptionalHeader.AddressOfEntryPoint), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Image Base]]", GetHex(data.ImageNtHeaders.OptionalHeader.ImageBase), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Base Of Data]]", data.ImageNtHeaders.OptionalHeader.BaseOfData, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Base Of Code]]", data.ImageNtHeaders.OptionalHeader.BaseOfCode, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Heap Commit]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeapCommit, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Heap Reserve]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeapReserve, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Stack Commit]]", data.ImageNtHeaders.OptionalHeader.SizeOfStackCommit, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Stack Reserve]]", data.ImageNtHeaders.OptionalHeader.SizeOfStackReserve, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Headers]]", data.ImageNtHeaders.OptionalHeader.SizeOfHeaders, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Image]]", data.ImageNtHeaders.OptionalHeader.SizeOfImage, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Uninitialized Data]]", data.ImageNtHeaders.OptionalHeader.SizeOfUninitializedData, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Initialized Data]]", data.ImageNtHeaders.OptionalHeader.SizeOfInitializedData, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Code]]", data.ImageNtHeaders.OptionalHeader.SizeOfCode, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[File Alignment]]", data.ImageNtHeaders.OptionalHeader.FileAlignment, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Section Alignment]]", data.ImageNtHeaders.OptionalHeader.SectionAlignment, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Characteristics]]", __GetDllCharacteristics(data.ImageNtHeaders.OptionalHeader.DllCharacteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Subsystem]]", __GetSubsystem(data.ImageNtHeaders.OptionalHeader.Subsystem), TYPE.ENUM, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Magic]]", __GetMagic(data.ImageNtHeaders.OptionalHeader.Magic), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Loader Flags]]", GetHex(data.ImageNtHeaders.OptionalHeader.LoaderFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Number Of]] RVA [[And Sizes]]", data.ImageNtHeaders.OptionalHeader.NumberOfRvaAndSizes, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Image Version]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorImageVersion, data.ImageNtHeaders.OptionalHeader.MinorImageVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Linker Version]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorLinkerVersion, data.ImageNtHeaders.OptionalHeader.MinorLinkerVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Subsystem Version]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorSubsystemVersion, data.ImageNtHeaders.OptionalHeader.MinorSubsystemVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Operating System Version]]", GetVersion(data.ImageNtHeaders.OptionalHeader.MajorOperatingSystemVersion, data.ImageNtHeaders.OptionalHeader.MinorOperatingSystemVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "Win32 [[Version]]", data.ImageNtHeaders.OptionalHeader.Win32VersionValue, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                }
                {
                    Send(context, NAME.TYPE.INFO, level + 1, "DOS [[Header]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                    if (data.ImageDosHeader != null)
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "oeminfo", data.ImageDosHeader.e_oeminfo, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "oemid", data.ImageDosHeader.e_oemid, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "ovno", data.ImageDosHeader.e_ovno, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "lfarlc", data.ImageDosHeader.e_lfarlc, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "cs", data.ImageDosHeader.e_cs, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "ip", data.ImageDosHeader.e_ip, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "csum", data.ImageDosHeader.e_csum, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "sp", data.ImageDosHeader.e_sp, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "ss", data.ImageDosHeader.e_ss, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "maxalloc", data.ImageDosHeader.e_maxalloc, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "minalloc", data.ImageDosHeader.e_minalloc, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "cparhdr", data.ImageDosHeader.e_cparhdr, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "crlc", data.ImageDosHeader.e_crlc, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "cp", data.ImageDosHeader.e_cp, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "cblp", data.ImageDosHeader.e_cblp, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "magic", __GetMagic(data.ImageDosHeader.e_magic), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 2, "lfanew", data.ImageDosHeader.e_lfanew, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                }
            }

            static private string __GetMachine(ushort value)
            {
                switch ((IMAGE_FILE_HEADER_MACHINE)value)
                {
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_UNKNOWN: return "UNKNOWN";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_I386: return "I386";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R3000: return "R3000";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R4000: return "R4000";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_R10000: return "R10000";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_WCEMIPSV2: return "WCEMIPSV2";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ALPHA: return "ALPHA";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3: return "SH3";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3DSP: return "SH3DSP";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH3E: return "SH3E";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH4: return "SH4";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_SH5: return "SH5";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ARM: return "ARM";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_THUMB: return "THUMB";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ARMNT: return "ARMNT";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_AM33: return "AM33";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_POWERPC: return "POWERPC";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_POWERPCFP: return "POWERPCFP";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_IA64: return "IA64";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPS16: return "MIPS16";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_ALPHA64: return "ALPHA64";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU: return "MIPSFPU";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU16: return "MIPSFPU16";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_TRICORE: return "TRICORE";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_CEF: return "CEF";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_EBC: return "EBC";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_AMD64: return "AMD64";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_M32R: return "M32R";
                    case IMAGE_FILE_HEADER_MACHINE.IMAGE_FILE_MACHINE_CEE: return "CEE";
                }
                return value.ToString();
            }

            static private string __GetCharacteristics(int value)
            {
                var a_Result = "";
                {
                    a_Result = AddFlag(a_Result, "RELOCS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_RELOCS_STRIPPED) != 0);
                    a_Result = AddFlag(a_Result, "EXECUTABLE_IMAGE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_EXECUTABLE_IMAGE) != 0);
                    a_Result = AddFlag(a_Result, "LINE_NUMS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LINE_NUMS_STRIPPED) != 0);
                    a_Result = AddFlag(a_Result, "LOCAL_SYMS_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LOCAL_SYMS_STRIPPED) != 0);
                    a_Result = AddFlag(a_Result, "AGGRESIVE_WS_TRIM", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_AGGRESIVE_WS_TRIM) != 0);
                    a_Result = AddFlag(a_Result, "LARGE_ADDRESS_AWARE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_LARGE_ADDRESS_AWARE) != 0);
                    a_Result = AddFlag(a_Result, "BYTES_REVERSED_LO", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_BYTES_REVERSED_LO) != 0);
                    a_Result = AddFlag(a_Result, "32BIT_MACHINE", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_32BIT_MACHINE) != 0);
                    a_Result = AddFlag(a_Result, "DEBUG_STRIPPED", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_DEBUG_STRIPPED) != 0);
                    a_Result = AddFlag(a_Result, "REMOVABLE_RUN_FROM_SWAP", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP) != 0);
                    a_Result = AddFlag(a_Result, "NET_RUN_FROM_SWAP", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_NET_RUN_FROM_SWAP) != 0);
                    a_Result = AddFlag(a_Result, "SYSTEM", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_SYSTEM) != 0);
                    a_Result = AddFlag(a_Result, "DLL", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_DLL) != 0);
                    a_Result = AddFlag(a_Result, "UP_SYSTEM_ONLY", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_UP_SYSTEM_ONLY) != 0);
                    a_Result = AddFlag(a_Result, "BYTES_REVERSED_HI", (value & (int)IMAGE_FILE_HEADER_CHARACTERISTICS.IMAGE_FILE_BYTES_REVERSED_HI) != 0);
                }
                return a_Result;
            }

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

            static private string __GetSubsystem(int value)
            {
                switch ((IMAGE_OPTIONAL_HEADER_SUBSYSTEM)value)
                {
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_UNKNOWN: return "UNKNOWN";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_NATIVE: return "NATIVE";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_GUI: return "WINDOWS_GUI";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_CUI: return "WINDOWS_CUI";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_OS2_CUI: return "OS2_CUI";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_POSIX_CUI: return "POSIX_CUI";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_CE_GUI: return "WINDOWS_CE_GUI";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_APPLICATION: return "EFI_APPLICATION";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER: return "EFI_BOOT_SERVICE_DRIVER";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER: return "EFI_RUNTIME_DRIVER";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_EFI_ROM: return "EFI_ROM";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_XBOX: return "XBOX";
                    case IMAGE_OPTIONAL_HEADER_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION: return "WINDOWS_BOOT_APPLICATION";
                }
                return value.ToString();
            }

            static private string __GetDllCharacteristics(int value)
            {
                var a_Result = "";
                {
                    a_Result = AddFlag(a_Result, "DYNAMIC_BASE", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE) != 0);
                    a_Result = AddFlag(a_Result, "FORCE_INTEGRITY", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY) != 0);
                    a_Result = AddFlag(a_Result, "NX_COMPAT", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NX_COMPAT) != 0);
                    a_Result = AddFlag(a_Result, "NO_ISOLATION", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_ISOLATION) != 0);
                    a_Result = AddFlag(a_Result, "NO_SEH", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_SEH) != 0);
                    a_Result = AddFlag(a_Result, "NO_BIND", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_NO_BIND) != 0);
                    a_Result = AddFlag(a_Result, "WDM_DRIVER", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_WDM_DRIVER) != 0);
                    a_Result = AddFlag(a_Result, "TERMINAL_SERVER_AWARE", (value & (int)IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS.IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE) != 0);
                }
                return a_Result;
            }
        }
        
        internal class Metadata
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                // TODO: Implement visualization of .NET metadata
                //Send(context, NAME.TYPE.FOLDER, level, "[[Metadata]]", "");
            }
        }

        internal class ExportFunctions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                if ((data.ExportedFunctions != null) && (data.ExportedFunctions.Length > 0) && data.HasValidExportDir)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Export Functions]]", "", GetArraySize(data.ExportedFunctions), HINT.EMPTY);
                    foreach (var a_Context in data.ExportedFunctions)
                    {
                        Send(context, (a_Context.HasName ? NAME.TYPE.FUNCTION : NAME.TYPE.WARNING), level + 1, GetFunctionName(a_Context.Name, a_Context.HasName), "", TYPE.FUNCTION, HINT.DATA_TYPE, NAME.STATE.NONE, "");
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        if (a_Context.HasForward)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Forward Name]]", a_Context.ForwardName, TYPE.STRING, HINT.DATA_TYPE);
                        }
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(a_Context.Address), TYPE.POINTER, HINT.DATA_TYPE);
                        }
                        if (a_Context.HasOrdinal)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Ordinal]]", a_Context.Ordinal, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                }
            }
        }

        internal class ImportFunctions
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                if ((data.ImportedFunctions != null) && (data.ImportedFunctions.Length > 0) && data.HasValidImportDir)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Import Functions]]", "", GetArraySize(data.ImportedFunctions), HINT.EMPTY);
                    foreach (var a_Context in data.ImportedFunctions)
                    {
                        var a_Name = GetFunctionName(a_Context.Name, true);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        if (string.IsNullOrEmpty(a_Name) == false)
                        {
                            Send(context, (string.IsNullOrEmpty(a_Context.Name) ? NAME.TYPE.WARNING : NAME.TYPE.FUNCTION), level + 1, a_Name, "", GetModuleName(a_Context.DLL), HINT.MODULE_NAME, NAME.STATE.NONE, a_Context.DLL);
                            if (a_Name != a_Context.Name)
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Name]]", a_Context.Name, TYPE.STRING, HINT.DATA_TYPE);
                            }
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Hint]]", a_Context.Hint, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "IAT [[Offset]]", a_Context.IATOffset, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
            }
        }

        internal class ImportModules
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                var a_Context = "";
                var a_Count = __GetCount(data);
                if (a_Count > 0)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Import Modules]]", "", GetArraySize(a_Count), HINT.EMPTY);
                    foreach (var a_Context1 in data.ImportedFunctions)
                    {
                        if (GetState() == STATE.CANCEL)
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
                            Send(context, NAME.TYPE.FILE, level + 1, a_Context1.DLL, "", "[[Module]]", HINT.DATA_TYPE, NAME.STATE.NONE, a_Context1.DLL);
                        }
                        {
                            a_Context += a_Context1.DLL.ToUpper() + "\n";
                        }
                    }
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
                    Send(context, NAME.TYPE.FOLDER, level, "[[Directories]]", "");
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
                }
            }

            static private void __Execute(atom.Trace context, int level, PeFile data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Security]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "SHA256", data.SHA256, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "SHA1", data.SHA1, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "MD5", data.MD5, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "ImpHash", data.ImpHash, TYPE.STRING, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_DEBUG_DIRECTORY[] data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Debug]]", "", GetArraySize(data.Length), HINT.EMPTY);
                    foreach (var a_Context in data)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, GetHex(a_Context.PointerToRawData), "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[TimeStamp]]", GetHex(a_Context.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Version]]", GetVersion(a_Context.MajorVersion, a_Context.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Type]]", a_Context.Type, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Data]]", a_Context.SizeOfData, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address Of Raw Data]]", GetHex(a_Context.AddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Pointer To Raw Data]]", GetHex(a_Context.PointerToRawData), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Characteristics]]", GetHex(a_Context.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                        }
                        if (a_Context.CvInfoPdb70 != null)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "PDB [[File Name]]", a_Context.CvInfoPdb70.PdbFileName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Signature]]", a_Context.CvInfoPdb70.Signature.ToString().ToUpper(), TYPE.GUID, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "CV [[Signature]]", GetHex(a_Context.CvInfoPdb70.CvSignature), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Age]]", a_Context.CvInfoPdb70.Age, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_BASE_RELOCATION[] data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Relocation]]", "", GetArraySize(data.Length), HINT.EMPTY);
                    foreach (var a_Context in data)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, GetHex(a_Context.VirtualAddress), "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Virtual Address]]", GetHex(a_Context.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Block]]", a_Context.SizeOfBlock, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                        if ((a_Context.TypeOffsets != null) && (a_Context.TypeOffsets.Length > 0))
                        {
                            Send(context, NAME.TYPE.INFO, level + 2, "[[Type Offsets]]", "", GetArraySize(a_Context.TypeOffsets.Length), HINT.EMPTY);
                            foreach (var a_Context1 in a_Context.TypeOffsets)
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 3, GetHex(a_Context1.Offset), a_Context1.Type.ToString(), TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_RESOURCE_DIRECTORY data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Resource]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[TimeStamp]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Version]]", GetVersion(data.MajorVersion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Number Of Name Entries]]", data.NumberOfNameEntries, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Number Of Id Entries]]", data.NumberOfIdEntries, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Characteristics]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, RUNTIME_FUNCTION[] data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Exceptions]]", "", GetArraySize(data.Length), HINT.EMPTY);
                    foreach (var a_Context in data)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, GetHex(a_Context.FunctionStart), "", TYPE.FUNCTION, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Function Start]]", GetHex(a_Context.FunctionStart), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Function End]]", GetHex(a_Context.FunctionEnd), TYPE.POINTER, HINT.DATA_TYPE);
                        }
                        if (a_Context.ResolvedUnwindInfo != null)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Version]]", a_Context.ResolvedUnwindInfo.Version, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Flags]]", GetHex(a_Context.ResolvedUnwindInfo.Flags), TYPE.FLAGS, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Prolog]]", a_Context.ResolvedUnwindInfo.SizeOfProlog, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Count Of Codes]]", a_Context.ResolvedUnwindInfo.CountOfCodes, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Frame Register]]", a_Context.ResolvedUnwindInfo.FrameRegister, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Frame Offset]]", a_Context.ResolvedUnwindInfo.FrameOffset, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Exception Handler]]", GetHex(a_Context.ResolvedUnwindInfo.ExceptionHandler), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Function Entry]]", GetHex(a_Context.ResolvedUnwindInfo.FunctionEntry), TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                        if ((a_Context.ResolvedUnwindInfo != null) && (a_Context.ResolvedUnwindInfo.UnwindCode != null) && (a_Context.ResolvedUnwindInfo.UnwindCode.Length > 0))
                        {
                            foreach (var a_Context1 in a_Context.ResolvedUnwindInfo.UnwindCode)
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 3, "[[Code Offset]]", a_Context1.CodeOffset, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.TYPE.VARIABLE, level + 3, "[[Frame Offset]]", a_Context1.FrameOffset, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.TYPE.VARIABLE, level + 3, "OP [[Info]]", a_Context1.Opinfo, TYPE.INTEGER, HINT.DATA_TYPE);
                                Send(context, NAME.TYPE.VARIABLE, level + 3, "OP [[Unwind]]", a_Context1.UnwindOp, TYPE.INTEGER, HINT.DATA_TYPE);
                            }
                        }
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_TLS_DIRECTORY data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Thread Local Storage]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    if (GetState() == STATE.CANCEL)
                    {
                        return;
                    }
                    else
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Start Address Of Raw Data]]", GetHex(data.StartAddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[End Address Of Raw Data]]", GetHex(data.EndAddressOfRawData), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Address Of Index]]", GetHex(data.AddressOfIndex), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Address Of CallBacks]]", GetHex(data.AddressOfCallBacks), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Size Of Zero Fill]]", data.SizeOfZeroFill, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Characteristics]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                    if ((data.TlsCallbacks != null) && (data.TlsCallbacks.Length > 0))
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Callbacks]]", "", GetArraySize(data.TlsCallbacks.Length), HINT.EMPTY);
                        foreach (var a_Context in data.TlsCallbacks)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Callback]]", GetHex(a_Context.Callback), TYPE.POINTER, HINT.DATA_TYPE);
                        }
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_LOAD_CONFIG_DIRECTORY data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Load Config]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[TimeStamp]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Version]]", GetVersion(data.MajorVesion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Size]]", data.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Guard]] CF [[Function Table]]", GetHex(data.GuardCFFunctionTable), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Guard]] CF [[Check Function Pointer]]", GetHex(data.GuardCFCheckFunctionPointer), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Guard]] CF [[Function Count]]", data.GuardCFFunctionCount, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "SE [[Handler Count]]", data.SEHandlerCount, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "SE [[Handler Table]]", data.SEHandlerTable, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Security Coockie]]", GetHex(data.SecurityCoockie), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Edit List]]", data.EditList, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "CSD [[Version]]", data.CSDVersion, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Process Affinity Mask]]", data.ProcessAffinityMask, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Maximum Allocation Size]]", data.MaximumAllocationSize, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Lock Prefix Table]]", data.LockPrefixTable, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Virtual Memory Threshold]]", data.VirtualMemoryThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[DeCommit Total Free Threshold]]", data.DeCommitTotalFreeThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[DeCommit Free Block Threshold]]", data.DeCommitFreeBlockThreshold, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Critical Section Default Timeout]]", data.CriticalSectionDefaultTimeout, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Global Flags Set]]", data.GlobalFlagsSet, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Global Flags Clear]]", data.GlobalFlagsClear, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Process Heap Flags]]", GetHex(data.ProcessHeapFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Guard Flags]]", GetHex(data.GuardFlags), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Reserved1]]", GetHex(data.Reserved1), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Reserved2]]", GetHex(data.Reserved2), TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_EXPORT_DIRECTORY data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Export]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[TimeStamp]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Version]]", GetVersion(data.MajorVersion, data.MinorVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Name]]", data.Name, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Base]]", data.Base, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Number Of Functions]]", data.NumberOfFunctions, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Number Of Names]]", data.NumberOfNames, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Address Of Functions]]", GetHex(data.AddressOfFunctions), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Address Of Names]]", GetHex(data.AddressOfNames), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Address Of Name Ordinals]]", GetHex(data.AddressOfNameOrdinals), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Characteristics]]", GetHex(data.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_IMPORT_DESCRIPTOR[] data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Import]]", "", GetArraySize(data.Length), HINT.EMPTY);
                    foreach (var a_Context in data)
                    {
                        Send(context, NAME.TYPE.FOLDER, level + 1, GetHex(a_Context.Name), "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[TimeStamp]]", GetHex(a_Context.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Name]]", GetHex(a_Context.Name), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Original First Thunk]]", GetHex(a_Context.OriginalFirstThunk), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Forwarder Chain]]", GetHex(a_Context.ForwarderChain), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[First Thunk]]", GetHex(a_Context.FirstThunk), TYPE.POINTER, HINT.DATA_TYPE);
                        }
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_BOUND_IMPORT_DESCRIPTOR data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Bound Import]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[TimeStamp]]", GetHex(data.TimeDateStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Offset Module Name]]", GetHex(data.OffsetModuleName), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Number Of Module Forwarder Refs]]", data.NumberOfModuleForwarderRefs, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_DELAY_IMPORT_DESCRIPTOR data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Delay Import]]", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[TimeStamp]]", GetHex(data.dwTimeStamp), TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Attributes]]", GetHex(data.grAttrs), TYPE.FLAGS, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Name]]", GetHex(data.szName), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Mod]]", GetHex(data.phmod), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[IAT]]", GetHex(data.pIAT), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[INT]]", GetHex(data.pINT), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Bound IAT]]", GetHex(data.pBoundIAT), TYPE.POINTER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Unload IAT]]", GetHex(data.pUnloadIAT), TYPE.POINTER, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, IMAGE_COR20_HEADER data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.FOLDER, level, "COM", "", TYPE.DIRECTORY, HINT.DATA_TYPE);
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Runtime Version]]", GetVersion(data.MajorRuntimeVersion, data.MinorRuntimeVersion), TYPE.VERSION, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Flags]]", GetHex(data.Flags), TYPE.FLAGS, HINT.DATA_TYPE);
                    }
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Entry Point]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Token]]", data.EntryPointToken, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Relative Virtual Address]]", data.EntryPointRVA, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.MetaData != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Metadata]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.MetaData.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.MetaData.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.Resources != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Resources]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.Resources.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.Resources.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.StrongNameSignature != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Strong Name Signature]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.StrongNameSignature.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.StrongNameSignature.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.CodeManagerTable != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Code Manager Table]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.CodeManagerTable.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.CodeManagerTable.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.VTableFixups != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Virtual Table Fix Up]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.VTableFixups.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.VTableFixups.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.ExportAddressTableJumps != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Export Address Table Jumps]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.ExportAddressTableJumps.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.ExportAddressTableJumps.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.ManagedNativeHeader != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Managed Native Header]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Address]]", GetHex(data.ManagedNativeHeader.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", data.ManagedNativeHeader.Size, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                }
            }
        }

        internal class Sections
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                Send(context, NAME.TYPE.FOLDER, level, "[[Sections]]", "", GetArraySize(data.ImageSectionHeaders), HINT.EMPTY);
                if (data.ImageSectionHeaders != null)
                {
                    foreach (var a_Context in data.ImageSectionHeaders)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, a_Context.NameResolved, "", TYPE.SECTION, HINT.DATA_TYPE);
                        if (GetState() == STATE.CANCEL)
                        {
                            return;
                        }
                        else
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Image Base Address]]", GetHex(a_Context.ImageBaseAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Virtual Address]]", GetHex(a_Context.VirtualAddress), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Virtual Size]]", a_Context.VirtualSize, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size Of Raw Data]]", a_Context.SizeOfRawData, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Pointer To Raw Data]]", GetHex(a_Context.PointerToRawData), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Pointer To Relocations]]", GetHex(a_Context.PointerToRelocations), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Pointer To Line Numbers]]", GetHex(a_Context.PointerToLinenumbers), TYPE.POINTER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Number Of Relocations]]", a_Context.NumberOfRelocations, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Number Of Line Numbers]]", a_Context.NumberOfLinenumbers, TYPE.INTEGER, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Characteristics]]", __GetCharacteristics(a_Context.Characteristics), TYPE.FLAGS, HINT.DATA_TYPE);
                        }
                    }
                }
            }

            static private string __GetCharacteristics(uint value)
            {
                var a_Result = "";
                {
                    a_Result = AddFlag(a_Result, "TYPE_NO_PAD", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_TYPE_NO_PAD) != 0);
                    a_Result = AddFlag(a_Result, "CNT_CODE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_CODE) != 0);
                    a_Result = AddFlag(a_Result, "CNT_INITIALIZED_DATA", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_INITIALIZED_DATA) != 0);
                    a_Result = AddFlag(a_Result, "CNT_UNINITIALIZED_DATA", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0);
                    a_Result = AddFlag(a_Result, "LNK_OTHER", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_OTHER) != 0);
                    a_Result = AddFlag(a_Result, "LNK_INFO", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_INFO) != 0);
                    a_Result = AddFlag(a_Result, "LNK_REMOVE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_REMOVE) != 0);
                    a_Result = AddFlag(a_Result, "LNK_COMDAT", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_COMDAT) != 0);
                    a_Result = AddFlag(a_Result, "NO_DEFER_SPEC_EXC", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_NO_DEFER_SPEC_EXC) != 0);
                    a_Result = AddFlag(a_Result, "GPREL", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_GPREL) != 0);
                    a_Result = AddFlag(a_Result, "MEM_PURGEABLE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_PURGEABLE) != 0);
                    a_Result = AddFlag(a_Result, "MEM_LOCKED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_LOCKED) != 0);
                    a_Result = AddFlag(a_Result, "MEM_PRELOAD", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_PRELOAD) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_1BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_1BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_2BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_2BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_4BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_4BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_8BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_8BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_16BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_16BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_32BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_32BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_64BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_64BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_128BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_128BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_256BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_256BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_512BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_512BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_1024BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_1024BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_2048BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_2048BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_4096BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_4096BYTES) != 0);
                    a_Result = AddFlag(a_Result, "ALIGN_8192BYTES", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_ALIGN_8192BYTES) != 0);
                    a_Result = AddFlag(a_Result, "LNK_NRELOC_OVFL", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_LNK_NRELOC_OVFL) != 0);
                    a_Result = AddFlag(a_Result, "MEM_DISCARDABLE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_DISCARDABLE) != 0);
                    a_Result = AddFlag(a_Result, "MEM_NOT_CACHED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_NOT_CACHED) != 0);
                    a_Result = AddFlag(a_Result, "MEM_NOT_PAGED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_NOT_PAGED) != 0);
                    a_Result = AddFlag(a_Result, "MEM_SHARED", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_SHARED) != 0);
                    a_Result = AddFlag(a_Result, "MEM_EXECUTE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_EXECUTE) != 0);
                    a_Result = AddFlag(a_Result, "MEM_READ", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_READ) != 0);
                    a_Result = AddFlag(a_Result, "MEM_WRITE", (value & (uint)IMAGE_SECTION_HEADER_CHARACTERISTICS.IMAGE_SCN_MEM_WRITE) != 0);
                }
                return a_Result;
            }
        }

        internal class Certificate
        {
            static public void Execute(atom.Trace context, int level, PeFile data)
            {
                if ((data.WinCertificate != null) && (data.PKCS7 != null))
                {
                    Send(context, NAME.TYPE.FOLDER, level, "[[Certificate]]", "");
                    {
                        __Execute(context, level + 1, data.WinCertificate);
                        __Execute(context, level + 1, data.PKCS7);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, WIN_CERTIFICATE data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.INFO, level, "Windows", "");
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Type]]", __GetType(data.wCertificateType), TYPE.ENUM, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Size]]", data.dwLength, TYPE.INTEGER, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Revision]]", data.wRevision, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                }
            }

            static private void __Execute(atom.Trace context, int level, X509Certificate2 data)
            {
                if (data != null)
                {
                    Send(context, NAME.TYPE.INFO, level, "X509", "");
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Friendly Name]]", data.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Subject Name]]", data.SubjectName.Name, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Issuer Name]]", (data.IssuerName != null) ? data.IssuerName.Name : "", TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Serial Number]]", data.SerialNumber, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Thumbprint]]", data.Thumbprint, TYPE.STRING, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Archived]]", GetBool(data.Archived), TYPE.BOOLEAN, HINT.DATA_TYPE);
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Version]]", (uint)data.Version, TYPE.INTEGER, HINT.DATA_TYPE);
                    }
                    if (data.NotBefore != null)
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Not Before]]", data.NotBefore.ToString(), TYPE.DATE, HINT.DATA_TYPE);
                    }
                    if (data.NotAfter != null)
                    {
                        Send(context, NAME.TYPE.VARIABLE, level + 1, "[[Not After]]", data.NotAfter.ToString(), TYPE.DATE, HINT.DATA_TYPE);
                    }
                    if (data.PublicKey != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Public Key]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        if (data.PublicKey.Oid != null)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Friendly Name]]", data.PublicKey.Oid.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Value]]", data.PublicKey.Oid.Value, TYPE.STRING, HINT.DATA_TYPE);
                        }
                        if (data.PublicKey.Key != null)
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Exchange Algorithm]]", data.PublicKey.Key.KeyExchangeAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", (uint)data.PublicKey.Key.KeySize, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.PrivateKey != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Private Key]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Exchange Algorithm]]", data.PrivateKey.KeyExchangeAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Signature Algorithm]]", data.PrivateKey.SignatureAlgorithm, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Size]]", (uint)data.PrivateKey.KeySize, TYPE.INTEGER, HINT.DATA_TYPE);
                        }
                    }
                    if (data.SignatureAlgorithm != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Signature Algorithm]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        {
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Friendly Name]]", data.SignatureAlgorithm.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
                            Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Value]]", data.SignatureAlgorithm.Value, TYPE.STRING, HINT.DATA_TYPE);
                        }
                    }
                    if (data.Extensions != null)
                    {
                        Send(context, NAME.TYPE.INFO, level + 1, "[[Extensions]]", "", TYPE.STRUCT, HINT.DATA_TYPE);
                        foreach (var a_Context in data.Extensions)
                        {
                            if (GetState() == STATE.CANCEL)
                            {
                                return;
                            }
                            if (a_Context.Oid != null)
                            {
                                Send(context, NAME.TYPE.INFO, level + 2, a_Context.Oid.FriendlyName, "", TYPE.STRUCT, HINT.DATA_TYPE);
                                {
                                    Send(context, NAME.TYPE.VARIABLE, level + 3, "[[Friendly Name]]", a_Context.Oid.FriendlyName, TYPE.STRING, HINT.DATA_TYPE);
                                    Send(context, NAME.TYPE.VARIABLE, level + 3, "[[Value]]", a_Context.Oid.Value, TYPE.STRING, HINT.DATA_TYPE);
                                    Send(context, NAME.TYPE.VARIABLE, level + 3, "[[Critical]]", GetBool(a_Context.Critical), TYPE.BOOLEAN, HINT.DATA_TYPE);
                                }
                            }
                        }
                    }
                }
            }

            static private string __GetType(int value)
            {
                switch ((WIN_CERTIFICATE_TYPE)value)
                {
                    case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_PKCS_SIGNED_DATA: return "PKCS_SIGNED_DATA";
                    case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_EFI_PKCS115: return "EFI_PKCS115";
                    case WIN_CERTIFICATE_TYPE.WIN_CERT_TYPE_EFI_GUID: return "EFI_GUID";
                }
                return value.ToString();
            }
        }

        protected override void _Execute(atom.Trace context, string url, int level)
        {
            var a_Context = (PeFile)null;
            if (PeFile.TryParse(url, out a_Context))
            {
                {
                    context.
                        SetState(NAME.STATE.EXPAND).
                        SetCommand(NAME.COMMAND.UPDATE).
                        SetId(CONSTANT.OUTPUT_PREVIEW_PREFIX + url).
                        SetUrl(url, "").
                        SetProgress(0, true, "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, 0, CONSTANT.OUTPUT_PREVIEW_PREFIX + Path.GetFileName(url));
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
                        SetState(NAME.STATE.EXPAND).
                        SetCommand(NAME.COMMAND.UPDATE).
                        SetId(CONSTANT.OUTPUT_PREVIEW_PREFIX + url).
                        SetUrl(url, "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, 0, CONSTANT.OUTPUT_PREVIEW_PREFIX + Path.GetFileName(url));
                }
            }
            else
            {
                Send(context, NAME.TYPE.ERROR, level, "[[This is not]] PE [[data format]]", "", "", HINT.EMPTY, NAME.STATE.NONE, "");
            }
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value, string comment, string hint, string state, string url)
        {
            context.
                SetComment(comment, hint).
                SetState(state).
                SetUrl(url, "").
                Send(NAME.SOURCE.PREVIEW, type, level, GetCleanString(name), GetCleanString(value));
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value, string comment, string hint, string state, string url)
        {
            context.
                SetComment(comment, hint).
                SetState(state).
                SetUrl(url, "").
                Send(NAME.SOURCE.PREVIEW, type, level, GetCleanString(name), value.ToString());
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value, string comment, string commentHint)
        {
            Send(context, type, level, name, value, comment, commentHint, NAME.STATE.NONE, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value, string comment, string commentHint)
        {
            Send(context, type, level, name, value, comment, commentHint, NAME.STATE.NONE, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, string value)
        {
            Send(context, type, level, name, value, "", HINT.EMPTY, NAME.STATE.NONE, "");
        }

        internal static void Send(atom.Trace context, string type, int level, string name, UInt64 value)
        {
            Send(context, type, level, name, value, "", HINT.EMPTY, NAME.STATE.NONE, "");
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

        internal static string GetArraySize(Array value)
        {
            if ((value != null) && (value is Array))
            {
                return "[[Found]]: " + (value as Array).Length.ToString();
            }
            return "[[Not found]]";
        }

        internal static string GetArraySize(int value)
        {
            return (value != 0) ? "[[Found]]: " + value.ToString() : "[[Not found]]";
        }

        internal static string GetVersion(ushort major, ushort minor)
        {
            return major.ToString() + "." + minor.ToString();
        }

        internal static string GetHex(UInt64 value)
        {
            return "0x" + value.ToString((value > UInt32.MaxValue) ? "X16" : "X8");
        }

        internal static string GetBool(bool value)
        {
            return value ? "[[true]]" : "[[false]]";
        }

        internal static string GetFunctionName(string value, bool isEnabled)
        {
            if (isEnabled && (string.IsNullOrEmpty(value) == false))
            {
                try
                {
                    var a_Result = new StringBuilder(1024);
                    {
                        UnDecorateSymbolName(value, a_Result, Int32.MaxValue, 0);
                    }
                    return a_Result.ToString();
                }
                catch (Exception)
                {
                }
                return value;
            }
            return "[[UNDEFINED]]";
        }

        internal static string GetModuleName(string value)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                return "<" + value + ">";
            }
            return "";
        }
    };
}
