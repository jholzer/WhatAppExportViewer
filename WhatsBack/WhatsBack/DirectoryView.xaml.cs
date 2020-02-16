using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WhatsBack
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