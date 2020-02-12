using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using Ninject;
using ReactiveUI;
using WhatAppExportViewer.Annotations;

namespace WhatAppExportViewer.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, INotifyPropertyChanged, IInitializable, IDisposable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        public abstract void Initialize();

        [NotifyPropertyChangedInvocator]
        protected virtual void raisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        public virtual void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}