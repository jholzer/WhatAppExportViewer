using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using DynamicData.Annotations;
using ReactiveUI;
using Xamarin.Forms;

namespace WhatsBack
{
    public class ViewModelBase : ReactiveObject, IDisposable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();

        public virtual void Dispose()
        {
            Disposables?.Dispose();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void raisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        public ViewModelBase()
        {
            ThrownExceptions
                .Subscribe(async ex => await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"))
                .DisposeWith(Disposables);
        }
    }
}