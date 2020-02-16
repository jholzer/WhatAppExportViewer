﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WhatsBack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannedChatsPage : ReactiveContentPage<ScannedChatsViewModel>
    {
        public ScannedChatsPage()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.PartnerViewModels, x => x.PartnerView.ItemsSource)
                    .DisposeWith(disposable);
            });
        }
    }
}