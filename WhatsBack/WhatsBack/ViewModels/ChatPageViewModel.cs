using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using WhatsBack.Model;

namespace WhatsBack.ViewModels
{
    public class ChatPageViewModel : ViewModelBase, IRoutableViewModel
    {
        public ChatPageViewModel(IScreen hostScreen, ChatItem[] chatItems, string baseFolder)
        {
            HostScreen = hostScreen;

            ChatItemViewModels = chatItems.Select(item => new ChatItemsViewModel(item, baseFolder)).ToArray();

            foreach (var vm in ChatItemViewModels)
            {
                vm.DisposeWith(Disposables);
            }
        }

        public string UrlPathSegment { get; } = "Chat";
        public IScreen HostScreen { get; }
        public ChatItemsViewModel[] ChatItemViewModels { get; }
    }
}