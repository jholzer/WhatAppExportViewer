using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using WhatsBack.ViewModels;
using Xamarin.Forms.Xaml;

namespace WhatsBack.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoryView : ReactiveContentPage<DirectoryViewModel>
    {
        public DirectoryView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.SubDirectories, x => x.SubDirectories.ItemsSource)
                    .DisposeWith(disposable);

                this.Bind(ViewModel, x => x.SelectedDirectory, x => x.SubDirectories.SelectedItem)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, x => x.CmdSetSoureDirectory, x => x.SetSourceDirectory)
                    .DisposeWith(disposable);
            });
        }
    }
}