using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using WhatsBack.Design;
using WhatsBack.Logic;
using WhatsBack.Model;
using Xamarin.Essentials;

namespace WhatsBack
{
    public class PartnerViewModel : ViewModelBase, IRoutableViewModel
    {
        public PartnerViewModel(IScreen hostScreen, string partner, IEnumerable<FileContent> files)
        {
            HostScreen = hostScreen;
            Partner = partner;

            var sourceDirectory = Preferences.Get("sourceDirectory", string.Empty);

            var parser = new BackupContentParser();

            CmdShowChat = ReactiveCommand.CreateFromTask(_ =>
            {
                var allChatItems = files.SelectMany(file =>
                    {
                        var content = File.ReadAllText(file.FullPath);
                        var chatItems = parser.ParseBackup(content);
                        return chatItems;
                    })
                    .OrderBy(ci => ci.TimeStamp)
                    .Distinct(ChatItem.Comparer)
                    .ToArray();
                
                HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, allChatItems, sourceDirectory))
                    .Subscribe()
                    .DisposeWith(Disposables);

                return Task.FromResult(Unit.Default);
            }).DisposeWith(Disposables);
        }

        public ReactiveCommand<Unit, Unit> CmdShowChat { get; private set; }
        public string Partner { get; }

        public string UrlPathSegment { get; } = "Partner";
        public IScreen HostScreen { get; }
    }

    public class DesignPartnerViewModel : PartnerViewModel
    {
        public DesignPartnerViewModel(IScreen hostScreen, string partner, IEnumerable<FileContent> files) 
            : base(new DesignHostScreen(), "Partner", DesignData.GetFileContent())
        {
        }
    }
}