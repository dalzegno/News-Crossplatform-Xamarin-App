using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

using System.Threading;
using System.Threading.Tasks;
using System;

namespace News.Views
{
    public partial class ArticleView : ContentPage
    {
        //Here is where you show the news in Full page
        string _url { get; set; }
        public ArticleView()
        {
            InitializeComponent();
        }
        public ArticleView(string Url)
        {
            InitializeComponent();
            _url = Url;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainThread.BeginInvokeOnMainThread(async () => { await LoadArticle(); });
        }
        private async Task LoadArticle()
        {
            try
            {
                activity.IsVisible = true;
                activity.IsRunning = true;
                webview.IsVisible = false;
                BindingContext = new UrlWebViewSource
                {
                    Url = HttpUtility.UrlDecode(_url)
                };
                await Task.Delay(3000);
                webview.IsVisible = true;
                activity.IsVisible = false;
                activity.IsRunning = false;
            }
            catch(Exception ex)
            {
                await DisplayAlert("Oops! Something went wrong.", $"Error: {ex.Message}", "OK!");
            }
        }
    }
}
