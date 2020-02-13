using System;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WhatsBack
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootStrapper();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            MainPage = RxApp.SuspensionHost
                .GetAppState<AppBootStrapper>()
                .CreateMainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
