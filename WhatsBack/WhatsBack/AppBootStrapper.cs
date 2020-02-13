using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace WhatsBack
{
    public class AppBootStrapper : ViewModelBase, IScreen
    {
        public AppBootStrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState router = null)
        {
            Router = router ?? new RoutingState();

            RegisterParts(dependencyResolver ?? Locator.CurrentMutable);

            Router.Navigate.Execute(new StartPageViewModel(this));
            //Router.Navigate.Execute(new ChatPageViewModel(null, null));
        }

        public RoutingState Router { get; }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new StartPage(), typeof(IViewFor<StartPageViewModel>));
            dependencyResolver.Register(() => new ChatPage(), typeof(IViewFor<ChatPageViewModel>));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}