using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using WhatsBack.Design;
using WhatsBack.Interfaces;
using WhatsBack.Model;
using Xamarin.Essentials;

namespace WhatsBack.ViewModels
{
    public class ScannedChatsViewModel : ViewModelBase, IRoutableViewModel
    {
        public ScannedChatsViewModel(IScreen hostScreen, IDirectoryTools directoryTools)
        {
            HostScreen = hostScreen;

            var sourceDirectory = Preferences.Get("sourceDirectory", string.Empty);

            var foundFiles = directoryTools.GetDirectoryContent(new DirectoryContent(string.Empty, sourceDirectory))
                .OfType<FileContent>()
                .Where(content => content.Extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            var chatPartners = foundFiles.GroupBy(x => ExtractChatPartner(x.Name))
                .Select(g => g.Key)
                .Distinct()
                .ToArray();

            var filesForPartners = chatPartners.Select(partner =>
            {
                return new
                {
                    Partner = partner, Files = foundFiles.Where(file => ExtractChatPartner(file.Name) == partner)
                };
            });

            PartnerViewModels = filesForPartners.Select(tuple => new PartnerViewModel(HostScreen, tuple.Partner, tuple.Files))
                .ToArray();
            foreach (var partnerViewModel in PartnerViewModels)
            {
                partnerViewModel.DisposeWith(Disposables);
            }
        }

        public PartnerViewModel[] PartnerViewModels { get; private set; }

        private string ExtractChatPartner(string fileName)
        {
            var split = Path.GetFileNameWithoutExtension(fileName)?.Split(' ');
            if (split?.Length > 4)
                return $"{split[3]} {split[4]}";
            return fileName;
        }

        public string UrlPathSegment { get; } = "Chats";
        public IScreen HostScreen { get; }
    }

    public class DesignScannedChatsViewModel : ScannedChatsViewModel
    {
        public DesignScannedChatsViewModel(IScreen hostScreen, IDirectoryTools directoryTools) 
            : base(new DesignHostScreen(), null)
        {
        }
    }
}