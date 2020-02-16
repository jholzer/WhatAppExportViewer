using System;
using System.Collections.Generic;
using WhatsBack.Model;

namespace WhatsBack
{
    public interface IDirectoryTools
    {
        string GetLocaPath(Uri uri);
        string GetSpecialFoldersPaths();
        string GetExternalStorageDirectoryContent();

        IEnumerable<DirectoryContentBase> GetDirectoryContent(DirectoryContent baseDirectory = null, string searchPattern = "*");
        string GetDocumentsFolder();
    }
}