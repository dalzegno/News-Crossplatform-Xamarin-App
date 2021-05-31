using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using News.Models;
using News.Services;
using Xamarin.Essentials;

namespace News.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public NewsPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainThread.BeginInvokeOnMainThread(async () => { await LoadNews(); });
        }
        
        private async Task LoadNews()
        {
            try
            {
                NewsService ns = new NewsService();
                NewsCategory nc = new NewsCategory();
                foreach (NewsCategory item in Enum.GetValues(typeof(NewsCategory)))
                {
                    if (Title.ToLower() == item.ToString())
                        nc = item;
                }
                var n = await ns.GetNewsAsync(nc);
                headLabel.Text = $"Todays {Title} Headlines";
                headLabel.HorizontalOptions = LayoutOptions.Center;
                lw.ItemsSource = n.Articles;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oops! Something went wrong.", $"Could not load news, try again later!\nError Message: {ex.Message}", "OK!");
                
            }  
        }
        private async void lw_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                NewsItem item = (NewsItem)e.Item;
                var page = (Page)Activator.CreateInstance(typeof(ArticleView));
                page = new ArticleView(item.Url);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oops! Something went wrong.", $"Could not load news, try again later!\nError Message: {ex.Message}", "OK!");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            aIndicator.IsVisible = true;
            aIndicator.IsRunning = true;
            lw.IsVisible = false;
            await LoadNews();
            await Task.Delay(2000);
            lw.IsVisible = true;
            aIndicator.IsVisible = false;
            aIndicator.IsRunning = false;
            
        }
    }
}