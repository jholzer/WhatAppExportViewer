using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using ReactiveUI;
using WhatsBack.Extensions;
using WhatsBack.Logic;
using Xamarin.Forms;

namespace WhatsBack.ViewModels
{
    public class StartPageViewModel : ViewModelBase, IRoutableViewModel
    {
        public StartPageViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            CmdScanFolder = ReactiveCommand.CreateFromTask( _ =>
            {
                var directoryTools = (Application.Current as App)?.DirectoryTools;
                HostScreen.Router.Navigate.Execute(new ScannedChatsViewModel(hostScreen, directoryTools))
                    .Subscribe()
                    .DisposeWith(Disposables);

                return Task.FromResult(Unit.Default);
            }).SetupErrorHandling(Disposables);

            CmdSetSourceFolder = ReactiveCommand.CreateFromTask(_ =>
            {
                var directoryTools = (Application.Current as App)?.DirectoryTools;

                HostScreen.Router.Navigate.Execute(new DirectoryViewModel(hostScreen, directoryTools))
                    .Subscribe()
                    .DisposeWith(Disposables);

                return Task.FromResult(Unit.Default);
            }).SetupErrorHandling(Disposables);

            CmdLoadFile = ReactiveCommand.CreateFromTask(async _ =>
            {
                try
                {
                    var fileData = await CrossFilePicker.Current.PickFile();
                    if (fileData == null)
                        return; // user canceled file picking

                    var contents = Encoding.UTF8.GetString(fileData.DataArray);
                    var parser = new BackupContentParser();
                    var chatItems = parser.ParseBackup(contents);

                    var uri = new Uri(fileData.FilePath);

                    var resolver = (Application.Current as App)?.DirectoryTools;
                    var localPath = resolver?.GetLocaPath(uri) ?? string.Empty;

                    var baseFolder = Path.GetDirectoryName(localPath);

                    HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, chatItems, baseFolder))
                        .Subscribe()
                        .DisposeWith(Disposables);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error getting file", ex.Message, "Dismiss");
                }
            }).SetupErrorHandling(Disposables);
        }

        public ReactiveCommand<Unit, Unit> CmdSetSourceFolder { get; private set; }
        public ReactiveCommand<Unit, Unit> CmdScanFolder { get; }
        public ICommand CmdLoadFile { get; }
        public string UrlPathSegment { get; } = "Start";
        public IScreen HostScreen { get; }
    }
}