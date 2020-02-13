using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using ReactiveUI;
using WhatsBack.Logic;
using Xamarin.Forms;

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

        private async Task LoadFile()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                DisplayFileContent(contents);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error getting file", ex.Message, "Dismiss");
            }
        }

        private void DisplayFileContent(string contents)
        {
            var parser = new BackupContentParser();
            var chatItems = parser.ParseBackup(contents);


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