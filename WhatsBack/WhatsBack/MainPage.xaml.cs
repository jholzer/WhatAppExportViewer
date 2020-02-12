using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace WhatsBack
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ReactiveContentPage<MainPageViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainPageViewModel();

            // Setup the bindings.  
            // Note: We have to use WhenActivated here, since we need to dispose the  
            // bindings on XAML-based platforms, or else the bindings leak memory.  
            this.WhenActivated(disposable =>
            {
                //this.Bind(ViewModel, x => x.UserName, x => x.Username.Text)
                //    .DisposeWith(disposable);

                this.BindCommand(ViewModel, 
                        x => x.CmdLoadFile, 
                        x => x.LoadFile)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, 
                        x => x.CmdScanFolder, 
                        x => x.ScanFolder)
                    .DisposeWith(disposable);
            });
        }
    }
}
