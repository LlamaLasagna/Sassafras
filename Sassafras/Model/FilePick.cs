using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sassafras
{
    public static class FilePick
    {


        public static string OpenFileSelector(string defaultPath = null, bool checkFileExists = true)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = checkFileExists
            };
            if (!string.IsNullOrEmpty(defaultPath))
            {
                dialog.FileName = defaultPath;
            }
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                return dialog.FileName;
            }
            return null;
        }


        public static string OpenDirectorySelector(string defaultPath = null)
        {
            //Hacky method of doing folder selection in file dialog without using gross WinForms selector
            OpenFileDialog dialog = new OpenFileDialog()
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };
            if (!string.IsNullOrEmpty(defaultPath))
            {
                dialog.InitialDirectory = defaultPath;
            }
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                string directoryPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                return directoryPath;
            }
            return null;
        }


    }
}
