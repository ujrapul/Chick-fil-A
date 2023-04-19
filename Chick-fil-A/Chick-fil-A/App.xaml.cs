using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chick_fil_A.Services;
using Chick_fil_A.Views;
using System.Threading.Tasks;

namespace Chick_fil_A
{
    public partial class App : Application
    {
        public AppShell appShell;
        public App ()
        {
            InitializeComponent();

            DependencyService.Register<NFCSelectDataStore>();
            MainPage = appShell = new AppShell();
        }

        public async Task Intent(IIntent intent)
        {
            await appShell.IntentAsync(intent);
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}
