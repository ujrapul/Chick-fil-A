using System;
using System.ComponentModel;
using Chick_fil_A.Models;
using Chick_fil_A.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chick_fil_A.Views
{
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();
            BackgroundImageSource = ImageSource.FromResource("Chick-fil-A.Images.Chic-Fil-a-Sandwich-2-1.jpeg");
        }

        async void NFC_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            // Search downward in the hierarchy
            // https://learn.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/navigation
            await Shell.Current.GoToAsync("/NFC");
        }
    }
}