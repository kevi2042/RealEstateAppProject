using Newtonsoft.Json;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Linq;
using System.Threading;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {
        public PropertyDetailPage(PropertyListItem propertyListItem)
        {
            InitializeComponent();

            // Sætter preference indstillinger hvis der er nogle

            if (Preferences.ContainsKey("TextToSpeech_Pitch"))
            {
                double pref = Preferences.Get("TextToSpeech_Pitch", 10.0);
                PitchSlider.Value = pref;
                options.Pitch = (float)(pref / 10);
            }
            if (Preferences.ContainsKey("TextToSpeech_Volume"))
            {
                double pref = Preferences.Get("TextToSpeech_Volume", 5.0);
                VolumeSlider.Value = pref;
                options.Volume = (float)(pref / 10);
            }



            Property = propertyListItem.Property;

            IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            BindingContext = this;
        }

        CancellationTokenSource cts;

        public Agent Agent { get; set; }

        public Property Property { get; set; }
        private bool IsSpeech = false;
        SpeechOptions options { get; set; } = new SpeechOptions();

        private async void EditProperty_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        #region Image Carousel
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImageListPage(Property.ImageUrls));
        }
        #endregion

        #region Text-To-Speech
        private async void SpeechButton_Clicked(object sender, System.EventArgs e)
        {
            if (!IsSpeech)
            {
                SpeechButton.Text = "\uf04d";
                cts = new CancellationTokenSource();
                IsSpeech = true;
                await TextToSpeech.SpeakAsync(Property.Description, options, cts.Token);
                if (IsSpeech) 
                {
                    IsSpeech = false;
                    SpeechButton.Text = "\uf04b";
                };
            }
            else
            {
                if(cts.IsCancellationRequested == false)
                {
                    cts.Cancel();
                }
                IsSpeech = false;
                SpeechButton.Text = "\uf04b";
            }
        }

        private void PitchSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Preferences.Set("TextToSpeech_Pitch", PitchSlider.Value);
            options.Pitch = (float)(PitchSlider.Value / 10);
        }

        private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Preferences.Set("TextToSpeech_Volume", VolumeSlider.Value);
            options.Volume = (float)(VolumeSlider.Value / 10);
        }

        #endregion

        #region Email, SMS, Call
        private async void TapEmail_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Email.ComposeAsync(new EmailMessage("", "", Property.Vendor.Email));
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }

        private async void TapPhone_Tapped(object sender, EventArgs e)
        {
            try
            {
                var action = await DisplayActionSheet(Property.Vendor.Phone, "Cancel", null, "Call", "SMS");

                switch (action)
                {
                    case "Call":
                        PhoneDialer.Open(Property.Vendor.Phone);
                        break;

                    case "SMS":
                        await Sms.ComposeAsync(new SmsMessage(string.Format("Hej, {0}, angående {1}", Property.Vendor.FirstName, Property.Address), new[] { Property.Vendor.FullName }));
                        break;
                }
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }

        }
        #endregion

        #region Maps
        private async void OpenDestinationButton_Clicked(object sender, EventArgs e)
        {
            var location = new Location((double)Property.Latitude, (double)Property.Longitude);
            var options = new MapLaunchOptions { Name = Property.Address };

            try
            {
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                // No map application available to open
            }
        }

        private async void OpenNavigationButton_Clicked(object sender, EventArgs e)
        {
            var location = new Location((double)Property.Latitude, (double)Property.Longitude);
            var options = new MapLaunchOptions { Name = Property.Address, NavigationMode = NavigationMode.Driving };

            try
            {
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region BrowserLauncher (Open Browser and pdf)
        private async void OpenBrowserButton_Clicked(object sender, EventArgs e)
        {
            string url = Property.NeighbourhoodUrl;

            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }

        private async void OpenBrowserOutsideButton_Clicked(object sender, EventArgs e)
        {
            string url = Property.NeighbourhoodUrl;

            await Browser.OpenAsync(url, BrowserLaunchMode.External);
        }

        private async void OpenPDFButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string file = Property.ContractFilePath;
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(file)
                });
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }
        #endregion

        #region Share & Clipboard
        private async void ShareTextButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = string.Format("Address: {0}, Price: {1}, Beds: {2}", Property.Address, Property.Price, Property.Beds),
                Uri = Property.NeighbourhoodUrl,
                Subject = "A Property you may be interested in",
                Title = "Share Property"
            });

        }

        private async void ShareFileButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareFileRequest()
            {
                Title = "Share Property Contract",
                File = new ShareFile(Property.ContractFilePath)
            });
        }

        private async void CopyToClipboardButton_Clicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(JsonConvert.SerializeObject(Property));
        }
        #endregion
    }
}