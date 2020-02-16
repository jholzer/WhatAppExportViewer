using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI;
using WhatsBack.Model;
using Xamarin.Essentials;

namespace WhatsBack
{
    public class DirectoryViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IDirectoryTools directoryTools;

        private readonly BehaviorSubject<IEnumerable<DirectoryContent>> subDirectories =
            new BehaviorSubject<IEnumerable<DirectoryContent>>(new DirectoryContent[0]);

        private readonly ObservableAsPropertyHelper<IEnumerable<DirectoryContent>> subDirectoriesHelper;
        private DirectoryContent selectedDirectory;

        public DirectoryViewModel(IScreen hostScreen, IDirectoryTools directoryTools)
        {
            this.directoryTools = directoryTools;
            HostScreen = hostScreen;

            var sourceDirectory = Preferences.Get("sourceDirectory", string.Empty);

            subDirectories.DisposeWith(Disposables);

            this.WhenAnyValue(vm => vm.SelectedDirectory)
                .Where(x => x != null)
                .Subscribe(directoryContent => SetDirectoryContent(directoryContent.FullPath))
                .DisposeWith(Disposables);

            subDirectoriesHelper = subDirectories.ToProperty(this, vm => vm.SubDirectories);
            subDirectoriesHelper.DisposeWith(Disposables);

            CmdSetSoureDirectory = ReactiveCommand.CreateFromTask(_ =>
                {
                    Preferences.Set("sourceDirectory", SelectedDirectory.FullPath);
                    return Task.FromResult(Unit.Default);
                })
                .DisposeWith(Disposables);

            SetDirectoryContent(sourceDirectory);
        }

        public ReactiveCommand<Unit, Unit> CmdSetSoureDirectory { get; private set; }

        public IEnumerable<DirectoryContent> SubDirectories => subDirectoriesHelper.Value;

        public DirectoryContent SelectedDirectory
        {
            get => selectedDirectory;
            set
            {
                if (selectedDirectory == value) return;
                selectedDirectory = value;
                raisePropertyChanged();
            }
        }

        public string UrlPathSegment { get; } = "Set source directory";
        public IScreen HostScreen { get; }

        private void SetDirectoryContent(string sourceDirectory)
        {
            var content = new List<DirectoryContent>();
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = directoryTools.GetDocumentsFolder();
            }
            else
            {
                content.Add(new DirectoryContent(".. (up)", Path.GetDirectoryName(sourceDirectory)));
            }

            var sourceDirContent = new DirectoryContent(Path.GetFileName(sourceDirectory), sourceDirectory);
            content.AddRange(directoryTools.GetDirectoryContent(baseDirectory: sourceDirContent)
                .OfType<DirectoryContent>());

            subDirectories.OnNext(content);
        }
    }
}