using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeightCalculatorPage : ContentPage
    {
        public double CurrentPressure { get; set; }
        public double CurrentAltitude { get; set; }

        SensorSpeed speed = SensorSpeed.UI;
        
        public ObservableCollection<BarometerMeasurement> measurements { get; set; } = new ObservableCollection<BarometerMeasurement>();
        
        public HeightCalculatorPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            Barometer.ReadingChanged += Barometer_ReadingChanged;
            ToggleBarometer();
        }

        protected override void OnDisappearing()
        {
            Barometer.ReadingChanged -= Barometer_ReadingChanged;
            ToggleBarometer();
        }

        public void ToggleBarometer()
        {
            try
            {
                if (Barometer.IsMonitoring)
                    Barometer.Stop();
                else
                    Barometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            var data = e.Reading;
            double seaLevelPressure = 1013;
            CurrentPressure = data.PressureInHectopascals;

            CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / seaLevelPressure, 0.190284));
        }

        private void SaveMeasurements_Clicked(object sender, EventArgs e)
        {
            BarometerMeasurement barometer = new BarometerMeasurement();
            barometer.Altitude = CurrentAltitude;
            barometer.Pressure = CurrentPressure;
            barometer.Label = EntryLabelName.Text;

            if(measurements.Count > 0)
            {
                barometer.HeightChange = measurements.LastOrDefault().Altitude + barometer.Altitude;
            }
            else
            {
                barometer.HeightChange = barometer.Altitude;
            }

            measurements.Add(barometer);
        }
    }
}