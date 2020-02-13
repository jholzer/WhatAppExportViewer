using System;
using System.Linq;
using System.Reactive.Disposables;
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
    }
}