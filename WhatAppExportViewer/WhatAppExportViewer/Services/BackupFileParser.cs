using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WhatAppExportViewer.Interfaces;
using WhatAppExportViewer.Model;

namespace WhatAppExportViewer.Services
{
    public class BackupFileParser : IBackupFileParser
    {
        public ChatItem[] ParseBackup(string file)
        {
            var lines = File.ReadAllLines(file);

            return lines.Select(l =>
                {
                    try
                    {
                        var dateStopIdx = l.IndexOf(',');
                        var dateStr = l.Substring(0, dateStopIdx);
                        var date = DateTime.Parse(dateStr, CultureInfo.CurrentCulture);

                        var timeStopInx = l.IndexOf('-');
                        var timeStr = l.Substring(dateStopIdx + 2, timeStopInx - dateStopIdx - 2);
                        var time = TimeSpan.Parse(timeStr, CultureInfo.CurrentCulture);

                        var textStartIdxtStartIdx = l.IndexOf(':', timeStopInx);

                        var timeStamp = date.Add(time);

                        string name = l.Substring(timeStopInx + 1, textStartIdxtStartIdx - timeStopInx - 1).Trim();

                        string text = l.Substring(textStartIdxtStartIdx + 1).Trim();
                        return new ChatItem(name, text, timeStamp);
                    }
                    catch
                    {
                        // Ignore
                    }

                    return null;
                })
                .Where(x => x != null)
                .ToArray();
        }
    }
}