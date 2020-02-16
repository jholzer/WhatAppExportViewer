using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WhatsBack.Model;

namespace WhatsBack.Logic
{
    class BackupContentParser
    {
        public ChatItem[] ParseBackup(string fileContent, string tag = null)
        {
            var items = new List<ChatItem>();

            var regex = new Regex(@"([0-9]*\.[0-9]*\.[0-9]*), ([0-9]*:[0-9]*) - (.*): (.*)");

            ChatItem item = null;
            foreach (var l in fileContent.Split('\n'))
            {
                if (regex.IsMatch(l))
                {
                    if (item != null)
                    {
                        items.Add(item);
                    }

                    var match = regex.Match(l);

                    string name = match.Groups[3].Value;

                    var preText = string.Empty;
                    if (name.Contains(":"))
                    {
                        var idx = name.IndexOf(':');
                        preText = name.Substring(idx + 1) + ":";
                        name = name.Substring(0, idx);
                    }
                    string text = $"{preText} {match.Groups[4].Value}";

                    var timestampText = match.Groups[2].Value;
                    var dateTimeText = match.Groups[1].Value;
                    
                    var date = DateTime.ParseExact(dateTimeText, @"dd\.mm\.yy", CultureInfo.InvariantCulture);
                    var time = TimeSpan.ParseExact(timestampText, @"hh\:mm", CultureInfo.InvariantCulture);
                    
                    var timestamp = date.Add(time);

                    item = new ChatItem(name, text, timestamp, tag);
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
