using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public PartnerViewModel(IScreen hostScreen, string partner, ChatItem[] chatItems,
            FileContent[] imageFiles)
        {
            if (chatItems == null)
                return;

            HostScreen = hostScreen;
            Partner = partner;

            var fileCount = GetFileCount(chatItems);
            NumberOfFiles = $"{fileCount} file(s)";

            var timeStamps = chatItems
                .Select(i => i.TimeStamp)
                .Distinct()
                .ToArray();

            var rangeStart = timeStamps.Min();
            var rangeEnd = timeStamps.Max();
            DateRange = rangeStart != rangeEnd
                ? $"{rangeStart:dd.MM.yy HH.mm} - {rangeEnd:dd.MM.yy HH.mm}"
                : $"{rangeStart:dd.MM.yy HH.mm}";

            CmdShowChat = ReactiveCommand.CreateFromTask(_ =>
            {
                HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, chatItems, imageFiles))
                    .Subscribe()
                    .DisposeWith(Disposables);

                return Task.FromResult(Unit.Default);
            }).SetupErrorHandling(Disposables);

            CmdMergeFiles = ReactiveCommand.CreateFromTask(async _ =>
                {
                    if (!chatItems.Any())
                        return Task.FromResult(Unit.Default);

                    var response = await Application.Current.MainPage.DisplayAlert("Merge files?", "Really merge files", "Yes", "No");
                    if (!response)
                        return Task.FromResult(Unit.Default);

                    var baseDir = Path.GetDirectoryName(chatItems.First().SourceFile);
                    foreach (var tuple in chatItems.GroupBy(CreateDateStamp)
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

                    BackupFiles(chatItems, baseDir);
                    if (await Application.Current.MainPage.DisplayAlert("Delete merged files?", "Really delete files? Backup was taken...", "Yes",
                        "No"))
                    {
                        foreach (var file in GetFiles(chatItems))
                        {
                            ChatItemsService.ExtractAllChatItems(fileContent);
                            File.Delete(file);
                        }
                    }

                    return Task.FromResult(Unit.Default);
                }, Observable.Return(fileCount > 1))
                .SetupErrorHandling(Disposables);
        }

        private static void BackupFiles(ChatItem[] chatItems, string baseDir)
        {
            var filesToBackup = GetFiles(chatItems);
            var archiveFile = Path.Combine(baseDir, "MergeBackup.zip");

            var zipArchiveMode = File.Exists(archiveFile) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
            using (ZipArchive zip = ZipFile.Open(archiveFile, zipArchiveMode))
            {
                var backupTime = DateTime.Now;
                foreach (var file in filesToBackup)
                {
                    var targetFile = $"{backupTime:yyyyMMddhhmm}_{Path.GetFileNameWithoutExtension(file)}";
                    zip.CreateEntryFromFile(file, targetFile, CompressionLevel.Optimal);
                }
            }
        }

        private int GetFileCount(ChatItem[] chatItems)
        {
            return GetFiles(chatItems).Count();
        }

        private static IEnumerable<string> GetFiles(ChatItem[] chatItems)
        {
            return chatItems.Select(c => c.SourceFile).Distinct();
        }

        private static DateTime CreateDateStamp(ChatItem ci)
        {
            return new DateTime(ci.TimeStamp.Year, ci.TimeStamp.Month, ci.TimeStamp.Day);
        }

        public ReactiveCommand<Unit, Task<Unit>> CmdMergeFiles { get; }
        public string NumberOfFiles { get; }
        public string DateRange { get; }
        public ReactiveCommand<Unit, Unit> CmdShowChat { get; private set; }
        public string Partner { get; }
        public string UrlPathSegment { get; } = "Partner";
        public IScreen HostScreen { get; }
    }

    public class DesignPartnerViewModel : PartnerViewModel
    {
        public DesignPartnerViewModel(IScreen hostScreen, string partner, IEnumerable<FileContent> files)
            : base(new DesignHostScreen(), "Partner", new ChatItem[0], new FileContent[0])
        {
        }
    }
}