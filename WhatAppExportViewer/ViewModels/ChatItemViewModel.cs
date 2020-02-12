using System;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using WhatAppExportViewer.Extensions;
using WhatAppExportViewer.Model;
using Color = System.Windows.Media.Color;

namespace WhatAppExportViewer.ViewModels
{
    public class ChatItemViewModel : ViewModelBase
    {
        private readonly string baseFolder;
        private string amPerson;
        private Color color;
        private int startColumn;

        public ChatItemViewModel(ChatItem chatItem, string baseFolder)
        {
            this.baseFolder = baseFolder;
            ChatItem = chatItem;
        }

        public string IAmPerson
        {
            get => amPerson;
            set
            {
                if (value == amPerson) return;
                amPerson = value;
                raisePropertyChanged();
            }
        }

        public ChatItem ChatItem { get; }

        public int StartColumn
        {
            get => startColumn;
            private set
            {
                if (value == startColumn) return;
                startColumn = value;
                raisePropertyChanged();
            }
        }

        public Color Color
        {
            get => color;
            private set
            {
                if (value.Equals(color)) return;
                color = value;
                raisePropertyChanged();
            }
        }

        public string Text { get; private set; }

        public override void Initialize()
        {
            Text = ChatItem.Text;
            if (Text.Contains(".jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                var imgStart = Text.IndexOf("IMG", StringComparison.InvariantCultureIgnoreCase);
                var fileNameLength = Text.IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase) + 4 - imgStart;
                var imageFile = Text.Substring(imgStart, fileNameLength).Trim();

                var imagePath = Path.Combine(baseFolder.Trim(), imageFile.Trim());
                if (File.Exists(imagePath))
                {
                    Image = new BitmapImage(new Uri(imagePath));
                }
            }

            this.WhenAnyValue(vm => vm.IAmPerson)
                .Where(p => !string.IsNullOrEmpty(p))
                .Subscribe(p => StartColumn = ChatItem.Name == p ? 1 : 0)
                .AddDisposable(Disposables);

            this.WhenAnyValue(vm => vm.IAmPerson)
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => ChatItem.Name == p
                    ? Colors.WhiteSmoke
                    : Colors.GreenYellow)
                .Subscribe(c => Color = c)
                .AddDisposable(Disposables);
        }

        public BitmapSource Image { get; private set; }
    }
}