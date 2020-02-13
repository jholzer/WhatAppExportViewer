using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using ReactiveUI;
using WhatsBack.Logic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;

namespace WhatsBack
{
    public class StartPageViewModel : ViewModelBase, IRoutableViewModel
    {
        public StartPageViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            CmdScanFolder = ReactiveCommand.CreateFromTask(_ => Task.FromResult(Unit.Default)).DisposeWith(Disposables);
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
                    var baseFolder = Path.GetDirectoryName(uri.LocalPath);
                    HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, chatItems, baseFolder)).Subscribe();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error getting file", ex.Message, "Dismiss");
                }
            }).DisposeWith(Disposables);
        }

        public ReactiveCommand<Unit, Unit> CmdScanFolder { get; }
        public ICommand CmdLoadFile { get; }
        public string UrlPathSegment { get; } = "Start";
        public IScreen HostScreen { get; }
    }
}