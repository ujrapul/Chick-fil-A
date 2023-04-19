using System;
using Chick_fil_A.Services;
using Plugin.NFC;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Chick_fil_A.Services
{
    public delegate void AlertEventHandler(
        object sender,
        AlertEventArgs args);

    public delegate void PropertyChangedHandler(
        object sender,
        string property);

    public class AlertEventArgs : EventArgs
    {
        private readonly string msg;
        private readonly string title;

        public AlertEventArgs(string msg, string title = "")
        {
            this.msg = msg;
            this.title = title;
        }

        public string Msg
        {
            get { return msg; }
        }

        public string Title
        {
            get { return title; }
        }
    }

    public sealed class NFCService
    {
        private static NFCService instance = null;
        public static NFCService Instance
        {
            get {
                if (instance == null)
                {
                    instance = new NFCService();
                }
                return instance;
            }
        }
        private void DummyAlert(object sender, AlertEventArgs e)
        {
        }

        private void DummyPropertyChanged(object sender, string property)
        {
        }

        public void SetAlertDelegate(AlertEventHandler handler)
        {
            ShowAlert = null;
            ShowAlert += handler;
        }

        public string text { get; set; }

        // Needs to be lowercase????
        public const string MIME_TYPE = "application/com.ujwalranganeni.chick-fil-a";

        NFCNdefTypeFormat _type;
        bool _makeReadOnly = false;
        bool _eventsAlreadySubscribed = false;
        bool _isDeviceiOS = false;

        /// <summary>
        /// Property that tracks whether the Android device is still listening,
        /// so it can indicate that to the user.
        /// </summary>
        public bool DeviceIsListening
        {
            get => _deviceIsListening;
            set
            {
                _deviceIsListening = value;
                OnPropertyChanged(this, nameof(DeviceIsListening));
            }
        }

        private bool _deviceIsListening = false;

        private bool _nfcIsEnabled = false;

        public bool NfcIsEnabled
        {
            get => _nfcIsEnabled;
            set
            {
                _nfcIsEnabled = value;
                OnPropertyChanged(this, nameof(NfcIsEnabled));
                OnPropertyChanged(this, nameof(NfcIsDisabled));
            }
        }

        public bool NfcIsDisabled => !NfcIsEnabled;

        private NFCService()
        {
            ShowAlert += DummyAlert;
            OnPropertyChanged += DummyPropertyChanged;

            // In order to support Mifare Classic 1K tags (read/write), you must set legacy mode to true.
            CrossNFC.Legacy = false;

            if (CrossNFC.IsSupported)
            {
                //if (!CrossNFC.Current.IsAvailable)
                //    ShowAlert(this, new AlertEventArgs("NFC is not available"));

                NfcIsEnabled = CrossNFC.Current.IsEnabled;
                //if (!NfcIsEnabled)
                //    ShowAlert(this, new AlertEventArgs("NFC is disabled"));

                if (Device.RuntimePlatform == Device.iOS)
                    _isDeviceiOS = true;

                //// Custom NFC configuration (ex. UI messages in French)
                //CrossNFC.Current.SetConfiguration(new NfcConfiguration
                //{
                //	DefaultLanguageCode = "fr",
                //	Messages = new UserDefinedMessages
                //	{
                //		NFCSessionInvalidated = "Session invalidée",
                //		NFCSessionInvalidatedButton = "OK",
                //		NFCWritingNotSupported = "L'écriture des TAGs NFC n'est pas supporté sur cet appareil",
                //		NFCDialogAlertMessage = "Approchez votre appareil du tag NFC",
                //		NFCErrorRead = "Erreur de lecture. Veuillez rééssayer",
                //		NFCErrorEmptyTag = "Ce tag est vide",
                //		NFCErrorReadOnlyTag = "Ce tag n'est pas accessible en écriture",
                //		NFCErrorCapacityTag = "La capacité de ce TAG est trop basse",
                //		NFCErrorMissingTag = "Aucun tag trouvé",
                //		NFCErrorMissingTagInfo = "Aucune information à écrire sur le tag",
                //		NFCErrorNotSupportedTag = "Ce tag n'est pas supporté",
                //		NFCErrorNotCompliantTag = "Ce tag n'est pas compatible NDEF",
                //		NFCErrorWrite = "Aucune information à écrire sur le tag",
                //		NFCSuccessRead = "Lecture réussie",
                //		NFCSuccessWrite = "Ecriture réussie",
                //		NFCSuccessClear = "Effaçage réussi"
                //	}
                //});

                SubscribeEvents();

                //_ = StartListeningIfNotiOS();
            }
        }

        public void OnBackButtonPressed()
        {
            UnsubscribeEvents();
        }

        /// <summary>
        /// Subscribe to the NFC events
        /// </summary>
        void SubscribeEvents()
        {
            if (_eventsAlreadySubscribed)
                return;

            _eventsAlreadySubscribed = true;

            CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
            CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
            CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
            CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;

            if (_isDeviceiOS)
                CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;
        }

        /// <summary>
        /// Unsubscribe from the NFC events
        /// </summary>
        void UnsubscribeEvents()
        {
            CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
            CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;
            CrossNFC.Current.OnNfcStatusChanged -= Current_OnNfcStatusChanged;
            CrossNFC.Current.OnTagListeningStatusChanged -= Current_OnTagListeningStatusChanged;

            if (_isDeviceiOS)
                CrossNFC.Current.OniOSReadingSessionCancelled -= Current_OniOSReadingSessionCancelled;
        }

        /// <summary>
        /// Event raised when Listener Status has changed
        /// </summary>
        /// <param name="isListening"></param>
        void Current_OnTagListeningStatusChanged(bool isListening) => DeviceIsListening = isListening;

        /// <summary>
        /// Event raised when NFC Status has changed
        /// </summary>
        /// <param name="isEnabled">NFC status</param>
        void Current_OnNfcStatusChanged(bool isEnabled)
        {
            NfcIsEnabled = isEnabled;
            ShowAlert(this, new AlertEventArgs($"NFC has been {(isEnabled ? "enabled" : "disabled")}"));
        }

        /// <summary>
        /// Event raised when a NDEF message is received
        /// </summary>
        /// <param name="tagInfo">Received <see cref="ITagInfo"/></param>
        void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                ShowAlert(this, new AlertEventArgs("No tag found"));
                return;
            }

            // Customized serial number
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
            var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

            if (!tagInfo.IsSupported)
            {
                ShowAlert(this, new AlertEventArgs("Unsupported tag (app)", title));
            }
            else if (tagInfo.IsEmpty)
            {
                ShowAlert(this, new AlertEventArgs("Empty tag", title));
            }
            else
            {
                var first = tagInfo.Records[0];
                //ShowAlert(this, new AlertEventArgs("Formatting tag operation successful", title));
                ShowAlert(this, new AlertEventArgs(GetMessage(first), title));
            }
        }

        /// <summary>
        /// Event raised when user cancelled NFC session on iOS 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_OniOSReadingSessionCancelled(object sender, EventArgs e) => Debug("iOS NFC Session has been cancelled");

        /// <summary>
        /// Event raised when data has been published on the tag
        /// </summary>
        /// <param name="tagInfo">Published <see cref="ITagInfo"/></param>
        void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            try
            {
                //ChkReadOnly.IsChecked = false;
                CrossNFC.Current.StopPublishing();
                if (tagInfo.IsEmpty)
                    ShowAlert(this, new AlertEventArgs("Formatting tag operation successful"));
                else
                    ShowAlert(this, new AlertEventArgs("Writing tag operation successful"));
            }
            catch (Exception ex)
            {
                ShowAlert(this, new AlertEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// Event raised when a NFC Tag is discovered
        /// </summary>
        /// <param name="tagInfo"><see cref="ITagInfo"/> to be published</param>
        /// <param name="format">Format the tag</param>
        void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            if (!CrossNFC.Current.IsWritingTagSupported)
            {
                ShowAlert(this, new AlertEventArgs("Writing tag is not supported on this device"));
                return;
            }

            try
            {
                NFCNdefRecord record = null;
                switch (_type)
                {
                    case NFCNdefTypeFormat.WellKnown:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.WellKnown,
                            MimeType = MIME_TYPE,
                            Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!"),
                            LanguageCode = "en"
                        };
                        break;
                    case NFCNdefTypeFormat.Uri:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Uri,
                            Payload = NFCUtils.EncodeToByteArray("https://github.com/franckbour/Plugin.NFC")
                        };
                        break;
                    case NFCNdefTypeFormat.Mime:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Mime,
                            MimeType = MIME_TYPE,
                            Payload = NFCUtils.EncodeToByteArray(text + " is activated and written to NFC Tag!")
                        };
                        break;
                    default:
                        break;
                }

                if (!format && record == null)
                    throw new Exception("Record can't be null.");

                tagInfo.Records = new[] { record };

                if (format)
                    CrossNFC.Current.ClearMessage(tagInfo);
                else
                {
                    CrossNFC.Current.PublishMessage(tagInfo, _makeReadOnly);
                }
            }
            catch (Exception ex)
            {
                ShowAlert(this, new AlertEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// Task to publish data to the tag
        /// </summary>
        /// <param name="type"><see cref="NFCNdefTypeFormat"/></param>
        /// <returns>The task to be performed</returns>
        public async Task Publish(NFCNdefTypeFormat? type = null)
        {
            await StartListeningIfNotiOS();
            try
            {
                _type = NFCNdefTypeFormat.Empty;
                //if (ChkReadOnly.IsChecked)
                //{
                //    if (!await DisplayAlert("Warning", "Make a Tag read-only operation is permanent and can't be undone. Are you sure you wish to continue?", "Yes", "No"))
                //    {
                //        ChkReadOnly.IsChecked = false;
                //        return;
                //    }
                //    _makeReadOnly = true;
                //}
                //else
                //    _makeReadOnly = false;

                if (type.HasValue) _type = type.Value;
                CrossNFC.Current.StartPublishing(!type.HasValue);
            }
            catch (Exception ex)
            {
                ShowAlert(this, new AlertEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// Returns the tag information from NDEF record
        /// </summary>
        /// <param name="record"><see cref="NFCNdefRecord"/></param>
        /// <returns>The tag information</returns>
        string GetMessage(NFCNdefRecord record)
        {
            var message = $"Message: {record.Message}";
            message += Environment.NewLine;
            message += $"RawMessage: {Encoding.UTF8.GetString(record.Payload)}";
            message += Environment.NewLine;
            message += $"Type: {record.TypeFormat}";

            if (!string.IsNullOrWhiteSpace(record.MimeType))
            {
                message += Environment.NewLine;
                message += $"MimeType: {record.MimeType}";
            }

            return message;
        }

        /// <summary>
        /// Write a debug message in the debug console
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        void Debug(string message) => System.Diagnostics.Debug.WriteLine(message);

        /// <summary>
        /// Display an alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Alert title</param>
        /// <returns>The task to be performed</returns>
        //public Task ShowAlert(string message, string title = null) { return new Task(); }
        public event AlertEventHandler ShowAlert;

        public event PropertyChangedHandler OnPropertyChanged;

        /// <summary>
        /// Task to start listening for NFC tags if the user's device platform is not iOS
        /// </summary>
        /// <returns>The task to be performed</returns>
        public async Task StartListeningIfNotiOS()
        {
            if (_isDeviceiOS)
                return;
            await BeginListening();
        }

        /// <summary>
        /// Task to safely start listening for NFC Tags
        /// </summary>
        /// <returns>The task to be performed</returns>
        public Task BeginListening()
        {
            try
            {
                CrossNFC.Current.StartListening();
            }
            catch (Exception ex)
            {
                ShowAlert(this, new AlertEventArgs(ex.Message));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Task to safely stop listening for NFC tags
        /// </summary>
        /// <returns>The task to be performed</returns>
        public Task StopListening()
        {
            try
            {
                CrossNFC.Current.StopListening();
            }
            catch (Exception ex)
            {
                ShowAlert(this, new AlertEventArgs(ex.Message));
            }

            return Task.CompletedTask;
        }
    }
}

