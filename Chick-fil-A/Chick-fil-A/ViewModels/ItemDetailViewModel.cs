using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Chick_fil_A.Services;
using Plugin.NFC;
using Xamarin.Forms;

namespace Chick_fil_A.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        private List<dynamic> properties;
        public string Id { get; set; }

        public ItemDetailViewModel()
        {
            NFCService.Instance.OnPropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, string property)
        {
            OnPropertyChanged(property);
        }

        public string Text
        {
            get => text;
            set {
                SetProperty(ref text, value);
                NFCService.Instance.text = Text;
            }
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public List<dynamic> Properties
        {
            get => properties;
            set => SetProperty(ref properties, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }        

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to Load Item");
            }
        }

        public bool NfcIsEnabled
        {
            get => NFCService.Instance.NfcIsEnabled;
            set
            {
                NFCService.Instance.NfcIsEnabled = value;
            }
        }

        public bool DeviceIsListening
        {
            get => NFCService.Instance.DeviceIsListening;
            set
            {
                NFCService.Instance.DeviceIsListening = value;
            }
        }

        public void OnBackButtonPressed()
        {
            NFCService.Instance.OnBackButtonPressed();
        }

        /// <summary>
        /// Start listening for NFC Tags when "READ TAG" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_StartListening(object sender, System.EventArgs e) => await NFCService.Instance.BeginListening();

        /// <summary>
        /// Stop listening for NFC tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_StopListening(object sender, System.EventArgs e) => await NFCService.Instance.StopListening();

        /// <summary>
        /// Start publish operation to write the tag (TEXT) when <see cref="Current_OnTagDiscovered(ITagInfo, bool)"/> event will be raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_StartWriting(object sender, System.EventArgs e) => await NFCService.Instance.Publish(NFCNdefTypeFormat.WellKnown);

        /// <summary>
        /// Start publish operation to write the tag (URI) when <see cref="Current_OnTagDiscovered(ITagInfo, bool)"/> event will be raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_StartWriting_Uri(object sender, System.EventArgs e) => await NFCService.Instance.Publish(NFCNdefTypeFormat.Uri);

        /// <summary>
        /// Start publish operation to write the tag (CUSTOM) when <see cref="Current_OnTagDiscovered(ITagInfo, bool)"/> event will be raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_StartWriting_Custom(object sender, System.EventArgs e) => await NFCService.Instance.Publish(NFCNdefTypeFormat.Mime);

        /// <summary>
        /// Start publish operation to format the tag when <see cref="Current_OnTagDiscovered(ITagInfo, bool)"/> event will be raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Button_Clicked_FormatTag(object sender, System.EventArgs e) => await NFCService.Instance.Publish();
    }
}
