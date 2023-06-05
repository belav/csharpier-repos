// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WpfTestApp
{
    partial
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MainWindow : Window
    {
        public MainWindow()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddSingleton<HttpClient>();
            Resources.Add("services", serviceCollection.BuildServiceProvider());

            InitializeComponent();
        }
    }
}
