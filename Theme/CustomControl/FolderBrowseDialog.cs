using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Theme.Native;

namespace Theme.CustomControl
{
    /// <summary>
    /// System folder browse dialog
    /// </summary>
    /// <example>
    /// FolderBrowseDialog.SelectFolder("Select a folder", string.Empty, ownerHandle);
    /// </example>
    public class FolderBrowseDialog
    {
        public static string SelectFolder(string title, string currentPath, IntPtr handle)
        {
            string result = String.Empty;
            var windowHandle = handle;
            FileOpenDialogNativeMethods.IFileOpenDialog fileOpenDialog = (FileOpenDialogNativeMethods.IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(FileOpenDialogNativeMethods.Guids.FileOpenDialog));
            if (fileOpenDialog != null)
            {
                FileOpenDialogNativeMethods.IShellItem siCurrentPath = null;

                while (!string.IsNullOrEmpty(currentPath) && !Directory.Exists(currentPath) && (currentPath.Length > 3))
                {
                    // check if parent directory exists
                    currentPath = Path.GetDirectoryName(currentPath);
                }

                // we need an existing path - fall back to video directory
                if (string.IsNullOrEmpty(currentPath) || !Directory.Exists(currentPath))
                {
                    currentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    try
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                    catch (System.Exception)
                    {
                        currentPath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos, System.Environment.SpecialFolderOption.Create);
                    }
                }

                try
                {
                    FileOpenDialogNativeMethods.SHCreateItemFromParsingName(currentPath, IntPtr.Zero, FileOpenDialogNativeMethods.Guids.ShellItem, out siCurrentPath);
                    if (siCurrentPath != null)
                    {
                        fileOpenDialog.SetFolder(siCurrentPath);
                        fileOpenDialog.SetOptions(FileOpenDialogNativeMethods.FOS.FOS_PICKFOLDERS | FileOpenDialogNativeMethods.FOS.FOS_PATHMUSTEXIST);
                        fileOpenDialog.SetTitle(title);
                        if (FileOpenDialogNativeMethods.HRESULT.S_OK == (FileOpenDialogNativeMethods.HRESULT)fileOpenDialog.Show(windowHandle))
                        {
                            FileOpenDialogNativeMethods.IShellItem siResult = null;
                            fileOpenDialog.GetResult(out siResult);
                            if (siResult != null)
                            {
                                IntPtr ptr = IntPtr.Zero;
                                siResult.GetDisplayName(FileOpenDialogNativeMethods.SIGDN.FILESYSPATH, out ptr);
                                if (ptr != IntPtr.Zero)
                                {
                                    result = Marshal.PtrToStringUni(ptr);
                                    Marshal.FreeCoTaskMem(ptr);
                                }
                            }
                        }
                        Marshal.ReleaseComObject(siCurrentPath);
                    }
                }
                catch (Exception)
                { }
            }
            return result;
        }
    }
}
