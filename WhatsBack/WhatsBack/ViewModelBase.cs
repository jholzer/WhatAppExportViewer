using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using DynamicData.Annotations;
using ReactiveUI;

namespace WhatsBack
{
    public class ViewModelBase : ReactiveObject, IDisposable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();

        public void Dispose()
        {
            Disposables?.Dispose();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void raisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }
    }
}