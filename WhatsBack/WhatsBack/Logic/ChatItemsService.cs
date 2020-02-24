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
            var parser = new BackupContentParser();
            var allChatItems = files.SelectMany(file =>
                {
                    var content = File.ReadAllText(file.FullPath);
                    var chatItems = parser.ParseBackup(content, sourceFile: file.FullPath);
                    return chatItems;
                })
                .OrderBy(ci => ci.TimeStamp)
                .Distinct(ChatItem.Comparer)
                .ToArray();
            return allChatItems;
        }
    }
}
