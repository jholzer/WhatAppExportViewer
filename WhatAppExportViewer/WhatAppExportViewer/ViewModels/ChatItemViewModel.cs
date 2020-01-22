using System;
using System.Reactive.Linq;
using System.Windows.Media;
using ReactiveUI;
using WhatAppExportViewer.Extensions;
using WhatAppExportViewer.Model;

namespace WhatAppExportViewer.ViewModels
{
    public class ChatItemViewModel : ViewModelBase
    {
        private string amPerson;
        private Color color;
        private int startColumn;

        public ChatItemViewModel(ChatItem chatItem)
        {
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

        public override void Initialize()
        {
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
    }
}