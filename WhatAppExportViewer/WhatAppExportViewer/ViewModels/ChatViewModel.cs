using System;
using System.Linq;
using Ninject;
using Ninject.Parameters;
using ReactiveUI;
using WhatAppExportViewer.Extensions;
using WhatAppExportViewer.Interfaces;

namespace WhatAppExportViewer.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IKernel kernel;
        private readonly string baseFolder;
        private readonly IBackupFileParser parser;
        private string amPerson;

        public ChatViewModel(string backupFile, 
            string baseFolder,
            IBackupFileParser parser, 
            IKernel kernel)
        {
            this.baseFolder = baseFolder;
            this.parser = parser;
            this.kernel = kernel;
            BackupFile = backupFile;
        }

        public string BackupFile { get; }

        public ChatItemViewModel[] ChatItemViewModels { get; private set; }

        public string[] Persons { get; private set; }

        public string IAmPerson
        {
            get => amPerson;
            set
            {
                if (value == amPerson) return;
                amPerson = value;
                raisePropertyChanged();
            }
        }

        public override void Initialize()
        {
            var chatItems = parser.ParseBackup(BackupFile);

            Persons = chatItems.Select(c => c.Name).Distinct().ToArray();
            IAmPerson = Persons.FirstOrDefault();

            ChatItemViewModels = chatItems
                .Select(c => kernel.Get<ChatItemViewModel>(
                    new ConstructorArgument(@"chatItem", c),
                    new ConstructorArgument(@"baseFolder", baseFolder))).ToArray();
            ChatItemViewModels.AddDisposables(Disposables);

            this.WhenAnyValue(vm => vm.IAmPerson)
                .Subscribe(iAmPerson =>
                {
                    foreach (var vm in ChatItemViewModels)
                    {
                        vm.IAmPerson = iAmPerson;
                    }
                })
                .AddDisposable(Disposables);
        }
    }
}