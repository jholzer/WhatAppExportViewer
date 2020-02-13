using System;
using System.IO;
using DynamicData.Tests;
using WhatsBack.Model;
using Xamarin.Essentials;

namespace WhatsBack
{
    public class ChatItemsViewModel : ViewModelBase
    {
        public ChatItemsViewModel(ChatItem item, string baseFolder)
        {
            ChatItem = item;

            var ownName = Preferences.Get("ownName", string.Empty);
            StartColumn = ChatItem.Name == ownName ? 1 : 0;

            Text = ChatItem.Text;
            //if (Text.ToLowerInvariant().Contains(".jpg"))
            //{
            //    var jpgIdx = Text.IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase);
            //    var untilFile = Text.Substring(0, jpgIdx + 4);
            //    var fileStartIdx = untilFile.LastIndexOf(' ');
            //    var imageFile = untilFile.Substring(fileStartIdx);

            //    ImagePath = Path.Combine(baseFolder.Trim(), imageFile.Trim());
            //    ShowImage = true;
            //}
        }

        //public bool ShowImage { get; }
        //public string ImagePath { get; }
        public string Text { get; }
        public int StartColumn { get; }
        public ChatItem ChatItem { get; }
    }
}