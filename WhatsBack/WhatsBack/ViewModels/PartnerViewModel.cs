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
using WhatsBack.Extensions;
using WhatsBack.Model;
using Xamarin.Forms;

namespace WhatsBack.ViewModels
{
    public class PartnerViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly ScannedChatsViewModel parentViewModel;

        public PartnerViewModel(IScreen hostScreen, string partner, ChatItem[] chatItems,
            FileContent[] imageFiles, ScannedChatsViewModel parentViewModel)
        {
            this.parentViewModel = parentViewModel;
            if (chatItems == null)
                return;

            HostScreen = hostScreen;
            Partner = partner;

            var fileCount = GetFileCount(chatItems);
            MoreThanOneFile = fileCount > 1;
            JustOneFile = fileCount == 1;
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

            CmdShowChat = ReactiveCommand.CreateFromTask(_ => ShowChat(hostScreen, chatItems, imageFiles))
                .SetupErrorHandling(Disposables);

            CmdRenameFile = ReactiveCommand.CreateFromTask(_ => RenameSingleFile(chatItems))
                .SetupErrorHandling(Disposables);

            CmdMergeFiles = ReactiveCommand.CreateFromTask(_ => MergeFiles(chatItems),
                    Observable.Return(fileCount > 1))
                .SetupErrorHandling(Disposables);
        }

        public ReactiveCommand<Unit, Unit> CmdRenameFile { get; }
        public ReactiveCommand<Unit, Unit> CmdMergeFiles { get; }
        public string NumberOfFiles { get; }
        public bool MoreThanOneFile { get; }
        public bool JustOneFile { get; }
        public string DateRange { get; }
        public ReactiveCommand<Unit, Unit> CmdShowChat { get; }
        public string Partner { get; }
        public string UrlPathSegment { get; } = "Partner";
        public IScreen HostScreen { get; }

        private Task ShowChat(IScreen hostScreen, ChatItem[] chatItems, FileContent[] imageFiles)
        {
            HostScreen.Router.Navigate.Execute(new ChatPageViewModel(hostScreen, chatItems, imageFiles))
                .Subscribe()
                .DisposeWith(Disposables);

            return Task.FromResult(Unit.Default);
        }

        private async Task MergeFiles(ChatItem[] chatItems)
        {
            if (!chatItems.Any())
                return;

            var response =
                await Application.Current.MainPage.DisplayAlert("Merge files?", "Really merge files", "Yes",
                    "No");
            if (!response)
                return;

            var baseDir = Path.GetDirectoryName(chatItems.First().SourceFile);
            foreach (var tuple in chatItems.GroupBy(CreateDateStamp)
                .Select(group => new {Datestamp = group.Key, Items = group.ToArray()}))
            {
                if (!tuple.Items.Any())
                    continue;

                var firstItemTimeStamp = tuple.Items.First().TimeStamp;
                var filename = $"WhatsApp Chat Merged {Partner} {firstItemTimeStamp:yyyyMMdd_hhmm}.txt";
                var lines = tuple.Items.Select(item =>
                {
                    var nameTag = !string.IsNullOrEmpty(item.Name) ? $"{item.Name}:" : string.Empty;
                    return $"{item.TimeStamp:dd.MM.yy, HH:mm} - {nameTag} {item.Text}";
                });

                if (baseDir != null)
                {
                    var targetFilePath = Path.Combine(baseDir, filename);
                    File.WriteAllLines(targetFilePath, lines);
                }
            }

            BackupFiles(chatItems, baseDir);
            if (await Application.Current.MainPage.DisplayAlert("Delete merged files?",
                "Really delete files? Backup was taken...", "Yes",
                "No"))
            {
                foreach (var file in GetFiles(chatItems))
                {
                    File.Delete(file);
                }
            }

            parentViewModel.Refresh();
        }

        private async Task RenameSingleFile(ChatItem[] chatItems)
        {
            var originalFile = chatItems.Select(ci => ci.SourceFile).Distinct().Single();
            var origPath = Path.GetDirectoryName(originalFile);
            var origFileName = Path.GetFileNameWithoutExtension(originalFile);
            var extension = Path.GetExtension(originalFile);
            var timestamp = chatItems.First().TimeStamp;
            var newFileName = $"{origFileName} {timestamp:yyyyMMdd}";
            if (origPath != null)
            {
                var newFile = Path.Combine(origPath, Path.ChangeExtension(newFileName, extension));

                if (!await Application.Current.MainPage.DisplayAlert("Rename file?",
                    $"'{origFileName}' to '{newFileName}'", "Yes", "No"))
                    return;

                File.Move(originalFile, newFile);
            }

            parentViewModel.Refresh();
        }

        private static void BackupFiles(ChatItem[] chatItems, string baseDir)
        {
            var filesToBackup = GetFiles(chatItems);
            var archiveFile = Path.Combine(baseDir, "MergeBackup.zip");

            var zipArchiveMode = File.Exists(archiveFile) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
            using (var zip = ZipFile.Open(archiveFile, zipArchiveMode))
            {
                var backupTime = DateTime.Now;
                foreach (var file in filesToBackup)
                {
                    var targetFile = $"{backupTime:yyyyMMddhhmm}_{Path.GetFileName(file)}";
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
    }
}