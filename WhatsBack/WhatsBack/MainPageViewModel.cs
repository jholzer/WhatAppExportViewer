using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace WhatsBack
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            CmdLoadFile = ReactiveCommand.CreateFromTask(_ => LoadFile()).DisposeWith(Disposables);
            CmdScanFolder = ReactiveCommand.CreateFromTask(_ => ScanFolder()).DisposeWith(Disposables);
        }

        public ReactiveCommand<Unit, Unit> CmdScanFolder { get; }

        public ICommand CmdLoadFile { get; }

        private Task ScanFolder()
        {
            throw new NotImplementedException();
        }

        private Task LoadFile()
        {
            throw new NotImplementedException();
        }
    }

    public class ViewModelBase : ReactiveObject, IDisposable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}