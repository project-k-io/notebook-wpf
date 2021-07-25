using Microsoft.Win32;
using System.IO;

namespace ProjectK.View.Helpers.Extensions
{
    public static class FileDialogExtensions
    {
        public static (string fileName, bool ok) SetFileDialog(this FileDialog dialog, string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
                dialog.InitialDirectory = directoryName;

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName)) dialog.FileName = fileName;

            dialog.DefaultExt = ".json";
            dialog.Filter = "Json documents (.json)|*.json" +
                            "|XML documents(.xml) | *.xml";

            var result = dialog.ShowDialog();
            if (!result ?? true)
                return ("", false);

            return (dialog.FileName, true);
        }
    }
}