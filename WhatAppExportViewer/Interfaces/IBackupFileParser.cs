using System;
using System.Collections.Generic;
using System.Text;
using WhatAppExportViewer.Model;

namespace WhatAppExportViewer.Interfaces
{
    public interface IBackupFileParser
    {
        ChatItem[] ParseBackup(string file);
    }
}
