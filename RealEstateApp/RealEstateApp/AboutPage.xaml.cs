using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void ClearSpeechPreferencesButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Preferences.Remove("TextToSpeech_Volume");
                Preferences.Remove("TextToSpeech_Pitch");
                await DisplayAlert("Successful", "The saved settings of Volume and Pitch have been cleared successfully", "Ok");
            }
            catch (Exception ex)
            {
            }

        }

        private void ClearLoginStorageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {

            }
        }
    }
}