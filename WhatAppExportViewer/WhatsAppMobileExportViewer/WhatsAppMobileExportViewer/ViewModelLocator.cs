using Ninject;

namespace WhatsAppMobileExportViewer
{
    public class ViewModelLocator
    {
        private readonly IKernel kernel;

        public ViewModelLocator()
        {
            kernel = new StandardKernel();
        }
        public MainPageViewModel MainWindowViewModel => kernel.Get<MainPageViewModel>();
    }
}
