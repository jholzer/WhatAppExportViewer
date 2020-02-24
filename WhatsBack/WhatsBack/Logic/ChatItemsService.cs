using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatsBack.Model;

namespace WhatsBack.Logic
{
    class ChatItemsService
    {
        public static ChatItem[] ExtractAllChatItems(IEnumerable<FileContent> files)
        {
            var allChatItems = files.SelectMany(file => ExtractAllChatItems(file.FullPath))
                .OrderBy(ci => ci.TimeStamp)
                .Distinct(ChatItem.Comparer)
                .ToArray();
            return allChatItems;
        }

        public static ChatItem[] ExtractAllChatItems(string filePath)
        {
            var parser = new BackupContentParser();
            var content = File.ReadAllText(filePath);
                    
            return parser.ParseBackup(content, sourceFile: filePath)
                .OrderBy(ci => ci.TimeStamp)
                .Distinct(ChatItem.Comparer)
                .ToArray();
        }
    }
}
