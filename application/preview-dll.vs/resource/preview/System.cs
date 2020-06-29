
namespace resource.preview
{
    enum IMAGE_FILE_HEADER_MACHINE
    {
        IMAGE_FILE_MACHINE_UNKNOWN                     = 0,
        IMAGE_FILE_MACHINE_I386                        = 0x014c,  // Intel 386.
        IMAGE_FILE_MACHINE_R3000                       = 0x0162,  // MIPS little-endian, 0x160 big-endian
        IMAGE_FILE_MACHINE_R4000                       = 0x0166,  // MIPS little-endian
        IMAGE_FILE_MACHINE_R10000                      = 0x0168,  // MIPS little-endian
        IMAGE_FILE_MACHINE_WCEMIPSV2                   = 0x0169,  // MIPS little-endian WCE v2
        IMAGE_FILE_MACHINE_ALPHA                       = 0x0184,  // Alpha_AXP
        IMAGE_FILE_MACHINE_SH3                         = 0x01a2,  // SH3 little-endian
        IMAGE_FILE_MACHINE_SH3DSP                      = 0x01a3,
        IMAGE_FILE_MACHINE_SH3E                        = 0x01a4,  // SH3E little-endian
        IMAGE_FILE_MACHINE_SH4                         = 0x01a6,  // SH4 little-endian
        IMAGE_FILE_MACHINE_SH5                         = 0x01a8,  // SH5
        IMAGE_FILE_MACHINE_ARM                         = 0x01c0,  // ARM Little-Endian
        IMAGE_FILE_MACHINE_THUMB                       = 0x01c2,  // ARM Thumb/Thumb-2 Little-Endian
        IMAGE_FILE_MACHINE_ARMNT                       = 0x01c4,  // ARM Thumb-2 Little-Endian
        IMAGE_FILE_MACHINE_AM33                        = 0x01d3,
        IMAGE_FILE_MACHINE_POWERPC                     = 0x01F0,  // IBM PowerPC Little-Endian
        IMAGE_FILE_MACHINE_POWERPCFP                   = 0x01f1,
        IMAGE_FILE_MACHINE_IA64                        = 0x0200,  // Intel 64
        IMAGE_FILE_MACHINE_MIPS16                      = 0x0266,  // MIPS
        IMAGE_FILE_MACHINE_ALPHA64                     = 0x0284,  // ALPHA64
        IMAGE_FILE_MACHINE_MIPSFPU                     = 0x0366,  // MIPS
        IMAGE_FILE_MACHINE_MIPSFPU16                   = 0x0466,  // MIPS
        IMAGE_FILE_MACHINE_TRICORE                     = 0x0520,  // Infineon
        IMAGE_FILE_MACHINE_CEF                         = 0x0CEF,
        IMAGE_FILE_MACHINE_EBC                         = 0x0EBC,  // EFI Byte Code
        IMAGE_FILE_MACHINE_AMD64                       = 0x8664,  // AMD64 (K8)
        IMAGE_FILE_MACHINE_M32R                        = 0x9041,  // M32R little-endian
        IMAGE_FILE_MACHINE_CEE                         = 0xC0EE
    }

    enum IMAGE_FILE_HEADER_CHARACTERISTICS
    {
        IMAGE_FILE_RELOCS_STRIPPED                     = 0x0001, // Relocation information was stripped from the file. The file must be loaded at its preferred base address. If the base address is not available, the loader reports an error.
        IMAGE_FILE_EXECUTABLE_IMAGE                    = 0x0002, // The file is executable (there are no unresolved external references).
        IMAGE_FILE_LINE_NUMS_STRIPPED                  = 0x0004, // COFF line numbers were stripped from the file.
        IMAGE_FILE_LOCAL_SYMS_STRIPPED                 = 0x0008, // COFF symbol table entries were stripped from file.
        IMAGE_FILE_AGGRESIVE_WS_TRIM                   = 0x0010, // Aggressively trim the working set. This value is obsolete.
        IMAGE_FILE_LARGE_ADDRESS_AWARE                 = 0x0020, // The application can handle addresses larger than 2 GB.
        IMAGE_FILE_BYTES_REVERSED_LO                   = 0x0080, // The bytes of the word are reversed. This flag is obsolete.
        IMAGE_FILE_32BIT_MACHINE                       = 0x0100, // The computer supports 32-bit words.
        IMAGE_FILE_DEBUG_STRIPPED                      = 0x0200, // Debugging information was removed and stored separately in another file.
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP             = 0x0400, // If the image is on removable media, copy it to and run it from the swap file.
        IMAGE_FILE_NET_RUN_FROM_SWAP                   = 0x0800, // If the image is on the network, copy it to and run it from the swap file.
        IMAGE_FILE_SYSTEM                              = 0x1000, // The image is a system file.
        IMAGE_FILE_DLL                                 = 0x2000, // The image is a DLL file. While it is an executable file, it cannot be run directly.
        IMAGE_FILE_UP_SYSTEM_ONLY                      = 0x4000, // The file should be run only on a uniprocessor computer.
        IMAGE_FILE_BYTES_REVERSED_HI                   = 0x8000 // The bytes of the word are reversed. This flag is obsolete.
    }

    enum IMAGE_OPTIONAL_HEADER_MAGIC
    {
        IMAGE_NT_OPTIONAL_HDR32_MAGIC                  = 0x10b, // The file is an executable image.
        IMAGE_NT_OPTIONAL_HDR64_MAGIC                  = 0x20b, // The file is an executable image.
        IMAGE_ROM_OPTIONAL_HDR_MAGIC                   = 0x107, // The file is a ROM image.
    }

    enum IMAGE_OPTIONAL_HEADER_SUBSYSTEM
    {
        IMAGE_SUBSYSTEM_UNKNOWN                        = 0, // Unknown subsystem.
        IMAGE_SUBSYSTEM_NATIVE                         = 1, // No subsystem required (device drivers and native system processes).
        IMAGE_SUBSYSTEM_WINDOWS_GUI                    = 2, // Windows graphical user interface (GUI) subsystem.
        IMAGE_SUBSYSTEM_WINDOWS_CUI                    = 3, // Windows character-mode user interface (CUI) subsystem.
        IMAGE_SUBSYSTEM_OS2_CUI                        = 5, // OS/2 CUI subsystem.
        IMAGE_SUBSYSTEM_POSIX_CUI                      = 7, // POSIX CUI subsystem.
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI                 = 9, // Windows CE system.
        IMAGE_SUBSYSTEM_EFI_APPLICATION                = 10, // Extensible Firmware Interface (EFI) application.
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER        = 11, // EFI driver with boot services.
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER             = 12, // EFI driver with run-time services.
        IMAGE_SUBSYSTEM_EFI_ROM                        = 13, // EFI ROM image.
        IMAGE_SUBSYSTEM_XBOX                           = 14, // Xbox system.
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION       = 16 // Boot application.
    }

    enum IMAGE_OPTIONAL_HEADER_DLLCHARACTERISTICS
    {
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE          = 0x0040, // The DLL can be relocated at load time.
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY       = 0x0080, // Code integrity checks are forced. If you set this flag and a section contains only uninitialized data, set the PointerToRawData member of IMAGE_SECTION_HEADER for that section to zero; otherwise, the image will fail to load because the digital signature cannot be verified.
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT             = 0x0100, // The image is compatible with data execution prevention (DEP).
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION          = 0x0200, // The image is isolation aware, but should not be isolated.
        IMAGE_DLLCHARACTERISTICS_NO_SEH                = 0x0400, // The image does not use structured exception handling (SEH). No handlers can be called in this image.
        IMAGE_DLLCHARACTERISTICS_NO_BIND               = 0x0800, // Do not bind the image.
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER            = 0x2000, // A WDM driver.
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000 // The image is terminal server aware
    }

    enum IMAGE_SECTION_HEADER_CHARACTERISTICS : uint
    {
        IMAGE_SCN_TYPE_NO_PAD                          = 0x00000008, // The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES.
        IMAGE_SCN_CNT_CODE                             = 0x00000020, // The section contains executable code.
        IMAGE_SCN_CNT_INITIALIZED_DATA                 = 0x00000040, // The section contains initialized data.
        IMAGE_SCN_CNT_UNINITIALIZED_DATA               = 0x00000080, // The section contains uninitialized data.
        IMAGE_SCN_LNK_OTHER                            = 0x00000100, // Reserved.
        IMAGE_SCN_LNK_INFO                             = 0x00000200, // The section contains comments or other information. This is valid only for object files.
        IMAGE_SCN_LNK_REMOVE                           = 0x00000800, // The section will not become part of the image. This is valid only for object files.
        IMAGE_SCN_LNK_COMDAT                           = 0x00001000, // The section contains COMDAT data. This is valid only for object files.
        IMAGE_SCN_NO_DEFER_SPEC_EXC                    = 0x00004000, // Reset speculative exceptions handling bits in the TLB entries for this section.
        IMAGE_SCN_GPREL                                = 0x00008000, // The section contains data referenced through the global pointer.
        IMAGE_SCN_MEM_PURGEABLE                        = 0x00020000, // Reserved.
        IMAGE_SCN_MEM_LOCKED                           = 0x00040000, // Reserved.
        IMAGE_SCN_MEM_PRELOAD                          = 0x00080000, // Reserved.
        IMAGE_SCN_ALIGN_1BYTES                         = 0x00100000, // Align data on a 1-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_2BYTES                         = 0x00200000, // Align data on a 2-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_4BYTES                         = 0x00300000, // Align data on a 4-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_8BYTES                         = 0x00400000, // Align data on a 8-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_16BYTES                        = 0x00500000, // Align data on a 16-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_32BYTES                        = 0x00600000, // Align data on a 32-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_64BYTES                        = 0x00700000, // Align data on a 64-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_128BYTES                       = 0x00800000, // Align data on a 128-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_256BYTES                       = 0x00900000, // Align data on a 256-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_512BYTES                       = 0x00A00000, // Align data on a 512-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_1024BYTES                      = 0x00B00000, // Align data on a 1024-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_2048BYTES                      = 0x00C00000, // Align data on a 2048-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_4096BYTES                      = 0x00D00000, // Align data on a 4096-byte boundary. This is valid only for object files.
        IMAGE_SCN_ALIGN_8192BYTES                      = 0x00E00000, // Align data on a 8192-byte boundary. This is valid only for object files.
        IMAGE_SCN_LNK_NRELOC_OVFL                      = 0x01000000, // The section contains extended relocations. The count of relocations for the section exceeds the 16 bits that is reserved for it in the section header. If the NumberOfRelocations field in the section header is 0xffff, the actual relocation count is stored in the VirtualAddress field of the first relocation. It is an error if IMAGE_SCN_LNK_NRELOC_OVFL is set and there are fewer than 0xffff relocations in the section.
        IMAGE_SCN_MEM_DISCARDABLE                      = 0x02000000, // The section can be discarded as needed.
        IMAGE_SCN_MEM_NOT_CACHED                       = 0x04000000, // The section cannot be cached.
        IMAGE_SCN_MEM_NOT_PAGED                        = 0x08000000, // The section cannot be paged.
        IMAGE_SCN_MEM_SHARED                           = 0x10000000, // The section can be shared in memory.
        IMAGE_SCN_MEM_EXECUTE                          = 0x20000000, // The section can be executed as code.
        IMAGE_SCN_MEM_READ                             = 0x40000000, // The section can be read.
        IMAGE_SCN_MEM_WRITE                            = 0x80000000 // The section can be written to.
    }

    enum WIN_CERTIFICATE_TYPE
    {
        WIN_CERT_TYPE_PKCS_SIGNED_DATA                 = 0x0002,
        WIN_CERT_TYPE_EFI_PKCS115                      = 0x0EF0,
        WIN_CERT_TYPE_EFI_GUID                         = 0x0EF1
    }
}
