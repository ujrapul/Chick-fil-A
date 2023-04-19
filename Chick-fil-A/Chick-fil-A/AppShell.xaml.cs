using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chick_fil_A.Services;
using Chick_fil_A.Views;
using Xamarin.Forms;

namespace Chick_fil_A
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("NFC", typeof(ItemsPage));
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        public async Task IntentAsync(IIntent intent)
        {
            if (intent.IsTagDiscovered()
                && intent.Type() == Services.NFCService.MIME_TYPE)
            {
                await Current.GoToAsync("//ScanPage/NFC/ItemDetailPage");
            }
        }
    }
}
