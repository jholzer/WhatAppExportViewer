using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace WhatsBack.Extensions
{
    static class CommandExtensions
    {
        public static ReactiveCommand<T1, T2> SetupErrorHandling<T1, T2>(this ReactiveCommand<T1, T2> command, CompositeDisposable disposable)
        {
            command.ThrownExceptions
                .Subscribe(async ex => await Application.Current.MainPage.DisplayAlert("Error running command", ex.Message, "OK"))
                .DisposeWith(disposable);
            return command;
        }
    }
}
