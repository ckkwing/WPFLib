using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Theme.Native
{
    public static class FileOpenDialogNativeMethods
    {
        #region IFileOpenDialog

        public static class Guids
        {
            public static readonly Guid ShellItem = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
            public static readonly Guid FileOpenDialog = new Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");
        }

        public enum HRESULT : long
        {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        public enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 1,
            SIATTRIBFLAGS_APPCOMPAT = 3,
            SIATTRIBFLAGS_OR = 2
        }

        public enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

        public enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        public enum SIGDN : uint
        {
            NORMALDISPLAY = 0,
            PARENTRELATIVEPARSING = 0x80018001,
            PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000
        }

        [Flags]
        public enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        public enum FDAP
        {
            FDAP_BOTTOM = 0,
            FDAP_TOP = 1
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszSpec;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        public interface IShellItem
        {
            void BindToHandler(IntPtr pbc,
                [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IntPtr ppv);

            void GetParent(out IShellItem ppsi);

            void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

            void Compare(IShellItem psi, uint hint, out int piOrder);
        };

        [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItemArray
        {
            // Not supported: IBindCtx
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid,
                    [In] ref Guid riid, out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCount(out uint pdwNumItems);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
        }

        [ComImport, Guid("b4db1657-70d7-485e-8e3e-6fcb5a5c1802"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IModalWindow
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            int Show([In] IntPtr parent);
        }

        [ComImport, Guid("973510DB-7D7F-452B-8975-74A85828D354"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialogEvents
        {
            // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
            // the dialog should not proceed (e.g. with closing, changing folder); to 
            // support this, we need to use the PreserveSig attribute to enable us to return
            // the proper HRESULT
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            HRESULT OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            HRESULT OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                         [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                      [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
                      out FDE_SHAREVIOLATION_RESPONSE pResponse);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                     [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
                     out FDE_OVERWRITE_RESPONSE pResponse);
        }

        [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialog : IModalWindow
        {
            // Defined on IModalWindow - repeated here due to requirements of COM interop layer
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            int Show([In] IntPtr parent);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // IFileDialog-Specific interface members
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileTypes([In] uint cFileTypes,
                      [In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileTypeIndex([In] uint iFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFileTypeIndex(out uint piFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Unadvise([In] uint dwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetOptions([In] FOS fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetOptions(out FOS pfos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FDAP fdap);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Close([MarshalAs(UnmanagedType.Error)] int hr);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetClientGuid([In] ref Guid guid);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void ClearClientData();

            // Not supported:  IShellItemFilter is not defined, converting to IntPtr
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
        }

        [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileOpenDialog : IFileDialog
        {
            // Defined on IModalWindow - repeated here due to requirements of COM interop layer
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            int Show([In] IntPtr parent);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Defined on IFileDialog - repeated here due to requirements of COM interop layer
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileTypes([In] uint cFileTypes, [In] COMDLG_FILTERSPEC[] rgFilterSpec);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileTypeIndex([In] uint iFileType);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFileTypeIndex(out uint piFileType);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Unadvise([In] uint dwCookie);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetOptions([In] FOS fos);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetOptions(out FOS pfos);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FDAP fdap);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Close([MarshalAs(UnmanagedType.Error)] int hr);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetClientGuid([In] ref Guid guid);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void ClearClientData();
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Not supported:  IShellItemFilter is not defined, converting to IntPtr
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Defined by IFileOpenDialog
            // ---------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateItemFromParsingName([In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);

        #endregion
    }
}
