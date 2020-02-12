using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace WhatAppExportViewer.Extensions
{
    public static class DisposableExtensions
    {
        public static T AddDisposable<T>(this T disposable, CompositeDisposable disposables) where T : IDisposable
        {
            disposables.Add(disposable);

            return disposable;
        }

        public static void AddDisposables(this IEnumerable<IDisposable> source, CompositeDisposable disposables)
        {
            foreach (var disposable in source)
            {
                disposables.Add(disposable);
            }
        }
    }
}
