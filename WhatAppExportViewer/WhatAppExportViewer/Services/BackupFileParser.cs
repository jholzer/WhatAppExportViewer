using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WhatAppExportViewer.Interfaces;
using WhatAppExportViewer.Model;

namespace WhatAppExportViewer.Services
{
    public class BackupFileParser : IBackupFileParser
    {
        public ChatItem[] ParseBackup(string file)
        {
            var lines = File.ReadAllLines(file);

            var items = new List<ChatItem>();

            var regex = new Regex(@"([0-9]*\.[0-9]*\.[0-9]*), ([0-9]*:[0-9]*) - (.*): (.*)");

            ChatItem item = null;
            foreach (var l in lines)
            {
                if (regex.IsMatch(l))
                {
                    if (item != null)
                    {
                        items.Add(item);
                    }

                    var match = regex.Match(l);

                    string name = match.Groups[3].Value;
                    string text = match.Groups[4].Value;
                    DateTime timestamp = DateTime.Parse(match.Groups[1].Value).Add(TimeSpan.Parse(match.Groups[2].Value));
                    item = new ChatItem(name, text, timestamp);
                }
                else
                {
                    item?.AppendText(l);
                }
            }

            if (item != null)
            {
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}