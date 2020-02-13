using System;
using System.Linq;
using ReactiveUI;
using Splat;
using WhatsBack.Model;

namespace WhatsBack
{
    public class ChatPageViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly ChatItem[] chatItems;

        public ChatPageViewModel(IScreen hostScreen, ChatItem[] chatItems)
        {
            HostScreen = hostScreen;
            this.chatItems = chatItems;
        }

        public string UrlPathSegment { get; } = "Chat";
        public IScreen HostScreen { get; }
    }
}