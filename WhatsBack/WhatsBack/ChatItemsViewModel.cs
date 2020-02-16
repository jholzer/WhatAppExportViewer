using System;
using System.IO;
using System.Reactive.Disposables;
using DynamicData.Tests;
using WhatsBack.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WhatsBack
{
    public class ChatItemsViewModel : ViewModelBase
    {
        private FileStream fileStream;

        public ChatItemsViewModel(ChatItem item, string baseFolder)
        {
            ChatItem = item;

            var ownName = Preferences.Get("ownName", string.Empty);
            StartColumn = ChatItem.Name == ownName ? 1 : 0;

            Text = ChatItem.Text;
            if (Text.ToLowerInvariant().Contains(".jpg"))
            {
                var jpgIdx = Text.IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase);
                var untilFile = Text.Substring(0, jpgIdx + 4);
                var fileStartIdx = untilFile.LastIndexOf(' ');
                var imageFile = untilFile.Substring(fileStartIdx);

                ImagePath = Path.Combine(baseFolder.Trim(), imageFile.Trim());

                //ImagePath = @"/storage/1D11-380A/TestData/20190822_201330_Vivid.jpg";

                try
                {
                    fileStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
                    fileStream.DisposeWith(Disposables);

                    ImageSource = ImageSource.FromStream(() => fileStream);
                    ShowImage = true;
                }
                catch
                {
                    // Ignore
                }
            }
        }

        public ImageSource ImageSource { get; }
        public bool ShowImage { get; }
        public string ImagePath { get; }
        public string Text { get; }
        public int StartColumn { get; }
        public ChatItem ChatItem { get; }
    }
}