using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using WhatsBack.Design;
using WhatsBack.Extensions;
using WhatsBack.Logic;
using WhatsBack.Model;
using Xamarin.Forms;

namespace WhatsBack.ViewModels
{
    public class PartnerViewModel : ViewModelBase, IRoutableViewModel
    {
        public PartnerViewModel(IScreen hostScreen, string partner, IEnumerable<FileContent> files,
            FileContent[] imageFiles)
        {
            if (files == null)
                return;

            HostScreen = hostScreen;
            Partner = partner;

            var fileContents = files as FileContent[] ?? files.ToArray();
            NumberOfFiles = $"{fileContents.Count()} file(s)";

            var parser = new BackupContentParser();

            CmdShowChat = ReactiveCommand.CreateFromTask(_ =>
            {
                var allChatItems = ExtractAllChatItems(fileContents, parser);

                HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, allChatItems, imageFiles))
                    .Subscribe()
                    .DisposeWith(Disposables);

                return Task.FromResult(Unit.Default);
            }).SetupErrorHandling(Disposables);

            CmdMergeFiles = ReactiveCommand.CreateFromTask(async _ =>
                {
                    if (!fileContents.Any())
                        return Task.FromResult(Unit.Default);

                    var allChatItems = ExtractAllChatItems(fileContents, parser);

                    var response = await Application.Current.MainPage.DisplayAlert("Merge files?", "Really merge files", "Yes", "No");
                    if (!response)
                        return Task.FromResult(Unit.Default);

                    var baseDir = Path.GetDirectoryName(fileContents.First().FullPath);
                    foreach (var tuple in allChatItems.GroupBy(CreateDateStamp)
                        .Select(group => new {Datestamp = group.Key, Items = group.ToArray()}))
                    {
                        var filename = $"WhatsApp Chat Merged {Partner} {tuple.Datestamp:yyyyMMdd}.txt";
                        var lines = tuple.Items.Select(item =>
                            {
                                var nameTag = !string.IsNullOrEmpty(item.Name) ? $"{item.Name}:" : string.Empty;
                                return $"{item.TimeStamp:dd.MM.yy, HH:mm} - {nameTag} {item.Text}";
                            });

                        var targetFilePath = Path.Combine(baseDir, filename); 
                        File.WriteAllLines(targetFilePath, lines);
                    }

                    return Task.FromResult(Unit.Default);
                }, Observable.Return(fileContents.Length > 1))
                .SetupErrorHandling(Disposables);
        }

        private static DateTime CreateDateStamp(ChatItem ci)
        {
            return new DateTime(ci.TimeStamp.Year, ci.TimeStamp.Month, ci.TimeStamp.Day);
        }

        private static ChatItem[] ExtractAllChatItems(IEnumerable<FileContent> files, BackupContentParser parser)
        {
            var allChatItems = files.SelectMany(file =>
                {
                    var content = File.ReadAllText(file.FullPath);
                    var chatItems = parser.ParseBackup(content, sourceFile: file.FullPath);
                    return chatItems;
                })
                .OrderBy(ci => ci.TimeStamp)
                .Distinct(ChatItem.Comparer)
                .ToArray();
            return allChatItems;
        }

        public ReactiveCommand<Unit, Task<Unit>> CmdMergeFiles { get; }
        public string NumberOfFiles { get; }
        public ReactiveCommand<Unit, Unit> CmdShowChat { get; private set; }
        public string Partner { get; }
        public string UrlPathSegment { get; } = "Partner";
        public IScreen HostScreen { get; }
    }

    public class DesignPartnerViewModel : PartnerViewModel
    {
        public DesignPartnerViewModel(IScreen hostScreen, string partner, IEnumerable<FileContent> files)
            : base(new DesignHostScreen(), "Partner", DesignData.GetFileContent(), new FileContent[0])
        {
        }
    }
}