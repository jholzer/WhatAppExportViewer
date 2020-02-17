using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using WhatsBack.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WhatsBack.ViewModels
{
    public class ChatItemsViewModel : ViewModelBase
    {
        public ChatItemsViewModel(ChatItem item, FileContent[] imageFiles)
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

                var res = imageFiles.FirstOrDefault(f =>
                    f.Name.Trim().Equals(imageFile.Trim(), StringComparison.CurrentCultureIgnoreCase));

                if (res == null)
                {
                    return;
                }

                ImagePath = res.FullPath;
                
                try
                {
                    var fileStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
                    fileStream.DisposeWith(Disposables);

                    ImageSource = ImageSource.FromStream(() => fileStream);
                    ShowImage = true;
                }
                catch (Exception ex)
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