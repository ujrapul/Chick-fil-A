using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.NFC;
using Android.Content;
using Android.Nfc;
using AndroidX.AppCompat.App;
using Chick_fil_A.Views;
using Chick_fil_A.ViewModels;
using Chick_fil_A.Services;
using Xamarin.Forms;

namespace Chick_fil_A.Droid
{
    public class AndroidIntent : IIntent
    {
        private Intent mIntent;
        public AndroidIntent(Intent intent)
        {
            mIntent = intent;
        }

        public bool IsTagDiscovered()
        {
            return mIntent.Action == NfcAdapter.ActionNdefDiscovered;
        }

        public string Type()
        {
            return mIntent.Type != null ? mIntent.Type : "";
        }
    }

    [Activity(Label = "Chick-fil-A",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges =
            ConfigChanges.ScreenSize
            | ConfigChanges.Orientation
            | ConfigChanges.UiMode
            | ConfigChanges.ScreenLayout
            | ConfigChanges.SmallestScreenSize, LaunchMode = LaunchMode.SingleTask )
    ]
    [IntentFilter(
        new[] { NfcAdapter.ActionNdefDiscovered },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeType = Chick_fil_A.Services.NFCService.MIME_TYPE)
    ]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private App app;
        private NfcAdapter mAdapter;
        private PendingIntent pendingIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            base.OnCreate(savedInstanceState);
            CrossNFC.Init(this);

            Forms.SetFlags("FastRenderers_Experimental");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            app = new App();
            LoadApplication(app);
            Window.SetStatusBarColor(Android.Graphics.Color.Argb(0, 0, 0, 0)); //here

            mAdapter = NfcAdapter.GetDefaultAdapter(this);
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Mutable);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnPause()
        {
            base.OnPause();
            //mAdapter.DisableForegroundDispatch(this);
        }
        protected override void OnResume()
        {
            base.OnResume();

            //var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            //var filters = new[] { tagDetected };
            //mAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);

            // Plugin NFC: Restart NFC listening on resume (needed for Android 10+) 
            CrossNFC.OnResume();
        }
        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            // Plugin NFC: Tag Discovery Interception
            CrossNFC.OnNewIntent(intent);

            await app.Intent(new AndroidIntent(intent));
        }
    }
}