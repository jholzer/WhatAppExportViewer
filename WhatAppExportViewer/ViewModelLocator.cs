using System.Text;
using Ninject;
using WhatAppExportViewer.Interfaces;
using WhatAppExportViewer.Services;
using WhatAppExportViewer.ViewModels;

namespace WhatAppExportViewer
{
    public class ViewModelLocator
    {
        private readonly IKernel kernel;

        public ViewModelLocator()
        {
            kernel = new StandardKernel();

            kernel.Bind<IBackupFileParser>().To<BackupFileParser>();
        }
        public MainWindowViewModel MainWindowViewModel => kernel.Get<MainWindowViewModel>();
    }
}
