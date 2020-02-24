using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using WhatsBack.Design;
using WhatsBack.Interfaces;
using WhatsBack.Logic;
using WhatsBack.Model;
using Xamarin.Essentials;

namespace WhatsBack.ViewModels
{
    public class ScannedChatsViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IDirectoryTools directoryTools;
        private readonly string sourceDirectory;
        private PartnerViewModel[] partnerViewModels;

        public ScannedChatsViewModel(IScreen hostScreen, IDirectoryTools directoryTools)
        {
            HostScreen = hostScreen;
            this.directoryTools = directoryTools;
            sourceDirectory = Preferences.Get("sourceDirectory", string.Empty);

            FillContent();
        }

        public PartnerViewModel[] PartnerViewModels
        {
            get => partnerViewModels;
            private set
            {
                partnerViewModels = value;
                raisePropertyChanged();
            }
        }

        public string UrlPathSegment { get; } = "Chats";
        public IScreen HostScreen { get; }

        public void Refresh()
        {
            FillContent();
        }
        private void FillContent()
        {
            DisposeViewModels();

            var files = directoryTools.GetDirectoryContent(new DirectoryContent(string.Empty, sourceDirectory))
                .OfType<FileContent>()
                .ToArray();

            var textFiles = files
                .Where(content => content.Extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            var imageFiles = files
                .Where(content => content.Extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            var chatPartners = textFiles.GroupBy(x => ExtractChatPartner(x.Name))
                .Select(g => g.Key)
                .Distinct()
                .ToArray();

            var chatItemSets = chatPartners.SelectMany(partner =>
                {
                    var filesForPartner = textFiles.Where(file => ExtractChatPartner(file.Name) == partner);
                    return CreateChatItemsSets(partner, filesForPartner);
                })
                .OrderByDescending(ci => ci.Date)
                .ThenBy(ci => ci.Partner)
                .ToArray();

            PartnerViewModels = chatItemSets
                .Select(cis => new PartnerViewModel(HostScreen, cis.Partner, cis.ChatItems, imageFiles, this))
                .ToArray();
        }

        public override void Dispose()
        {
            DisposeViewModels();
            base.Dispose();
        }

        private void DisposeViewModels()
        {
            if (PartnerViewModels == null)
                return;

            foreach (var partnerViewModel in PartnerViewModels)
            {
                partnerViewModel.DisposeWith(Disposables);
            }
        }

        private static IEnumerable<ChatItemSet> CreateChatItemsSets(string partner,
            IEnumerable<FileContent> filesForPartner)
        {
            var allChatItems = ExtractAllChatItems(filesForPartner);

            return allChatItems
                .GroupBy(item => new DateTime(item.TimeStamp.Year, item.TimeStamp.Month, item.TimeStamp.Day))
                .Select(group => new ChatItemSet
                {
                    Partner = partner,
                    ChatItems = @group.ToArray(),
                    Date = @group.Key
                });
        }

        private static ChatItem[] ExtractAllChatItems(IEnumerable<FileContent> files)
        {
            var parser = new BackupContentParser();
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

        private string ExtractChatPartner(string fileName)
        {
            var split = Path.GetFileNameWithoutExtension(fileName)?.Split(' ');
            if (split?.Length > 4)
                return $"{split[3]} {split[4]}";
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }

    public class ChatItemSet
    {
        public string Partner { get; set; }
        public ChatItem[] ChatItems { get; set; }
        public DateTime Date { get; set; }
    }

    public class DesignScannedChatsViewModel : ScannedChatsViewModel
    {
        public DesignScannedChatsViewModel(IScreen hostScreen, IDirectoryTools directoryTools)
            : base(new DesignHostScreen(), null)
        {
        }
    }
}