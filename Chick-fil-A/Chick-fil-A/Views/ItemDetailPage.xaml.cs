using System.ComponentModel;
using Xamarin.Forms;
using Chick_fil_A.ViewModels;
using Plugin.NFC;
using System;
using System.Text;
using System.Threading.Tasks;
using Chick_fil_A.Services;

namespace Chick_fil_A.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public const string ALERT_TITLE = "NFC";

        private readonly ItemDetailViewModel _model = new ItemDetailViewModel();

        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = _model;
            NFCService.Instance.SetAlertDelegate(ShowAlert);
        }

        private async void ShowAlert(object sender, AlertEventArgs e)
        {
            await DisplayAlert(string.IsNullOrWhiteSpace(e.Title) ? ALERT_TITLE : e.Title, e.Msg, "Cancel");
        }

        protected override bool OnBackButtonPressed()
        {
            _model.OnBackButtonPressed();
            CrossNFC.Current.StopListening();
            return base.OnBackButtonPressed();
        }

        void Button_Clicked_StartListening(System.Object sender, System.EventArgs e)
        {
            _model.Button_Clicked_StartListening(sender, e);
        }

        void Button_Clicked_StopListening(System.Object sender, System.EventArgs e)
        {
            _model.Button_Clicked_StopListening(sender, e);
        }

        void Button_Clicked_StartWriting(System.Object sender, System.EventArgs e)
        {
            _model.Button_Clicked_StartWriting_Custom(sender, e);
        }
    }
}