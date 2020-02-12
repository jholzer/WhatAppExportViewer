using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppMobileExportViewer.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        public MainPageViewModel()
        {
            CmdSelectFile = ReactiveCommand.CreateFromTask(_ =>
            {
                return Task.FromResult(Unit.Default);
            });
        }

        public ReactiveCommand<Unit, Unit> CmdSelectFile { get; }
    }
}
