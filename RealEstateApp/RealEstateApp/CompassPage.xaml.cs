using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompassPage : ContentPage
    {
        SensorSpeed speed = SensorSpeed.UI;

        public CompassAspect compass { get; set; }

        public CompassPage(CompassAspect aspect)
        {
            InitializeComponent();
            ToggleCompass();
            compass = aspect;

            BindingContext = this;
            
        }

        protected override void OnAppearing()
        {
            Compass.ReadingChanged += Compass_ReadingChanged;
        }

        protected override void OnDisappearing()
        {
            Compass.ReadingChanged -= Compass_ReadingChanged;
        }

        public void ToggleCompass()
        {
            try
            {
                if (Compass.IsMonitoring)
                    Compass.Stop();
                else
                    Compass.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Some other exception has occurred
            }
        }

        public void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            compass.RotationAngle = 360 - data.HeadingMagneticNorth;
            compass.CurrentHeading = data.HeadingMagneticNorth;


            string[] names = { "North" ,"East", "South", "West", "North" };
            compass.CurrentAspect = names[GetClosestAspect(data.HeadingMagneticNorth)];
        }

        public int GetClosestAspect(double input)
        {
            int[] numbers = { 0, 90, 180, 270, 360 };
            

            if (input > 0 && input < 45)
            {
                return 0;
            }
            else if(input >= 45 && input < 135)
            {
                return 1;
            }
            else if(input >= 135 && input < 225)
            {
                return 2;
            }
            else if(input >= 225 && input < 315)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        private async void SaveCompass_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}