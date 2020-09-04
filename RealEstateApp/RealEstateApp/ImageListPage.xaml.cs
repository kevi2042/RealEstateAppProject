using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageListPage : ContentPage
    {
        SensorSpeed speed = SensorSpeed.UI;
        public ObservableCollection<string> images { get; set; }

        public ImageListPage(List<string> urls)
        {
            InitializeComponent();

            images = new ObservableCollection<string>(urls);

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            ToggleAccelerometer();
        }

        protected override void OnDisappearing()
        {
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            ToggleAccelerometer();
        }


        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            var x = data.Acceleration.X;
            var y = data.Acceleration.Y;
            var z = data.Acceleration.Z;
            var position = CarouselViewer.Position;

            if (data.Acceleration.X > 0.1)
            {

                if (CarouselViewer.Position == images.Count-1)
                {
                    CarouselViewer.Position = 0;
                }
                else
                {
                    CarouselViewer.Position = CarouselViewer.Position + 1;
                }

            }
            //else if (data.Acceleration.X > 0.3)
            //{
            //    if (CarouselViewer.Position == 0)
            //    {
            //        CarouselViewer.Position = images.Count - 1;
            //    }
            //    else
            //    {
            //        CarouselViewer.Position = CarouselViewer.Position - 1;
            //    }
            //}
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
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
    }
}