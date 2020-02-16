using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatsBack.Interfaces;
using WhatsBack.Model;
using Environment = Android.OS.Environment;

namespace WhatsBack.Droid
{
    public class AndroidDirectoryTools : IDirectoryTools
    {
        public string GetLocaPath(Uri uri)
        {
            var split = uri.LocalPath.Split(':');
            var pathSplit = split[0].Split('/');

            var docsDir = Android.OS.Environment.DirectoryDocuments;
            var exStorageDir = Android.OS.Environment.ExternalStorageDirectory;
            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var docsDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);

            //ImagePath = @"/storage/1D11-380A/TestData/20190822_201330_Vivid.jpg";

            return Android.OS.Environment.ExternalStorageDirectory + "/" + pathSplit[2] + "/" + split[1];
        }

        public string GetSpecialFoldersPaths()
        {
            return $"ExternalStorageDirectory: {Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}" +
                   $"DirectoryDocuments: {Android.OS.Environment.DirectoryDocuments}";
        }

        public string GetExternalStorageDirectoryContent()
        {
            var dirs = string.Join("\n",
                Directory.GetDirectories(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath)
                    .Select(d => $"(DIR) {d}"));

            var files = string.Join("\n",
                Directory.GetFiles(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath)
                    .Select(d => $"(DIR) {d}"));

            return dirs + "\n" + files;
        }

        public IEnumerable<DirectoryContentBase> GetDirectoryContent(DirectoryContent baseDirectory = null,
            string searchPattern = "*")
        {
            try
            {
                if (string.IsNullOrEmpty(baseDirectory.FullPath))
                {
                    var dir = Environment.ExternalStorageDirectory.AbsolutePath;
                    baseDirectory = new DirectoryContent(Path.GetFileName(dir), dir);
                }

                var directoryContents = Directory.GetDirectories(baseDirectory.FullPath)
                    .Select(dirName => new DirectoryContent(Path.GetFileName(dirName), dirName));

                var fileContents = Directory.GetFiles(baseDirectory.FullPath, searchPattern)
                    .Select(fullPath =>
                    {
                        var fileName = Path.GetFileName(fullPath);
                        var extension = Path.GetExtension(fullPath);
                        return new FileContent(fileName, fullPath, extension);
                    });

                var contents = new List<DirectoryContentBase>();
                contents.AddRange(directoryContents);
                contents.AddRange(fileContents);
                return contents;
            }
            catch
            {
                return new[] {baseDirectory};
            }
        }

        public string GetDocumentsFolder()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }
    }
}