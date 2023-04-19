using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Chick_fil_A.Services;
using Chick_fil_A.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chick_fil_A.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BackgroundImageSource = ImageSource.FromResource("Chick-fil-A.Images.Chic-Fil-a-Sandwich-2-1.jpeg");
            NFCService.Instance.SetAlertDelegate(ShowAlert);
        }

        private async void ShowAlert(object sender, AlertEventArgs e)
        {
            await DisplayAlert(string.IsNullOrWhiteSpace(e.Title) ? "NFC" : e.Title, e.Msg, "Cancel");
        }
    }
}