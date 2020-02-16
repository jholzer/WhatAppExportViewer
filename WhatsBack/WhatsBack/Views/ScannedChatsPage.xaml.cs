using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using WhatsBack.ViewModels;
using Xamarin.Forms.Xaml;

namespace WhatsBack.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannedChatsPage : ReactiveContentPage<ScannedChatsViewModel>
    {
        public ScannedChatsPage()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.PartnerViewModels, x => x.PartnerView.ItemsSource)
                    .DisposeWith(disposable);
            });
        }
    }
}