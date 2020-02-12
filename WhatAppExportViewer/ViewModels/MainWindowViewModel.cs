using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;
using ReactiveUI;
using WhatAppExportViewer.Extensions;

namespace WhatAppExportViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IKernel kernel;
        private ChatViewModel chatViewModel;
        private string[] files;
        private string selectedFile;
        private string selectedBackupFolder;

        public MainWindowViewModel(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public string[] Files
        {
            get => files;
            private set
            {
                if (Equals(value, files)) return;
                files = value;
                raisePropertyChanged();
            }
        }

        public string SelectedFile
        {
            get => selectedFile;
            set
            {
                if (value == selectedFile) return;
                selectedFile = value;
                raisePropertyChanged();
            }
        }

        public ReactiveCommand<Unit, Unit> CmdSelectDirectory { get; private set; }

        public ChatViewModel ChatViewModel
        {
            get => chatViewModel;
            private set
            {
                if (Equals(value, chatViewModel)) return;
                chatViewModel = value;
                raisePropertyChanged();
            }
        }

        public string SelectedBackupFolder
        {
            get => selectedBackupFolder;
            set
            {
                if (value == selectedBackupFolder) return;
                selectedBackupFolder = value;
                raisePropertyChanged();
            }
        }

        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedFile)
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(SetBackupFile)
                .AddDisposable(Disposables);

            SelectedBackupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            this.WhenAnyValue(vm => vm.SelectedBackupFolder)
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(SetBackupFolder)
                .AddDisposable(Disposables);

            CmdSelectDirectory = ReactiveCommand.CreateFromTask(_ => { return Task.FromResult(Unit.Default); })
                .AddDisposable(Disposables);
        }

        public void SetBackupFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            Files = Directory.GetFiles(folder, "*.txt");
        }

        private void SetBackupFile(string file)
        {
            ChatViewModel?.Dispose();
            ChatViewModel = kernel.Get<ChatViewModel>(
                new ConstructorArgument(@"backupFile", file),
                new ConstructorArgument(@"baseFolder", SelectedBackupFolder));
        }

        public override void Dispose()
        {
            ChatViewModel?.Dispose();
            base.Dispose();
        }
    }
}