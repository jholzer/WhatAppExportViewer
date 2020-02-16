using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using WhatsBack.ViewModels;
using Xamarin.Forms.Xaml;

namespace WhatsBack.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ReactiveContentPage<ChatPageViewModel>, IViewFor<ChatPageViewModel>
    {
        public ChatPage()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.ChatItemViewModels, x => x.ChatItems.ItemsSource)
                    .DisposeWith(disposable);
            });
        }
    }
}