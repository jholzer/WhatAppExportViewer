using System;
using System.Drawing;
using System.Reactive.Linq;
using ReactiveUI;
using WhatAppExportViewer.Extensions;
using WhatAppExportViewer.Model;

namespace WhatAppExportViewer.ViewModels
{
    public class ChatItemViewModel : ViewModelBase
    {
        private string amPerson;
        private Brush background;
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

        public Brush Background
        {
            get => background;
            private set
            {
                if (Equals(value, background)) return;
                background = value;
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
                .Subscribe(p =>
                    Background = ChatItem.Name == p
                        ? Brushes.WhiteSmoke
                        : Brushes.GreenYellow)
                .AddDisposable(Disposables);
        }
    }
}