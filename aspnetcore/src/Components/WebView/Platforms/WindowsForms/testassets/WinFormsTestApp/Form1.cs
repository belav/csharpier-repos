using System.Net.Http;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace WinFormsTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddSingleton<HttpClient>();
            InitializeComponent();

            blazorWebView1.HostPage = @"wwwroot\webviewhost.html";
            blazorWebView1.Services = serviceCollection.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<BasicTestApp.Index>("root");
        }
    }
}
