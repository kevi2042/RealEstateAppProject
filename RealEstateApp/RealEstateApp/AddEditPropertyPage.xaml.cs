using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;

        #region PROPERTIES
        public ObservableCollection<Agent> Agents { get; }

        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                if (_property.AgentId != null)
                {
                    SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
                }
               
            }
        }
    
        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (Property != null)
                {
                    _selectedAgent = value;
                    Property.AgentId = _selectedAgent?.Id;
                }                 
            }
        }

        public string StatusMessage { get; set; }

        public Color StatusColor { get; set; } = Color.White;

        public bool LightToggler { get; set; } = false;
        #endregion

        #region Lifetime
        protected override void OnAppearing()
        {
            // Battery Tester
            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
            Battery.EnergySaverStatusChanged += OnEnergySaverStatusChanged;

            // Tilføjer metoden ConnectionTester til en listener der tester om connection er skiftet
            Connectivity.ConnectivityChanged += ConnectionTester;
        }

        protected override void OnDisappearing()
        {
            // Battery Tester
            Battery.BatteryInfoChanged -= Battery_BatteryInfoChanged;
            Battery.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;

            // Tilføjer metoden ConnectionTester til en listener der tester om connection er skiftet
            Connectivity.ConnectivityChanged -= ConnectionTester;
        }
        #endregion

        public AddEditPropertyPage(Property property = null, CompassAspect aspect = null)
        {
            InitializeComponent();


            CheckConnectivity();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());

            if (property == null)
            {
                Title = "Add Property";
                Property = new Property();
            }
            else
            {
                Title = "Edit Property";
                Property = property;
            }

            BindingContext = this;
        }

        private async void SaveProperty_Clicked(object sender, System.EventArgs e)
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
                Vibration.Vibrate(TimeSpan.FromSeconds(5));

            }
            else
            {
                Repository.SaveProperty(Property);
                await Navigation.PopToRootAsync();
            }   
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            Vibration.Cancel();
            await Navigation.PopToRootAsync();
        }

        #region Geolocation
        private async void GetLocationButton_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();

                Property.Longitude = location.Longitude;
                Property.Latitude = location.Latitude;

                var place = await Geocoding.GetPlacemarksAsync(location);

                var placemark = place?.FirstOrDefault();

                if(placemark != null)
                {
                    var geocodeAddress =
                        $"{placemark.FeatureName} " +
                        $"{placemark.Thoroughfare}, " +
                        $"{placemark.Locality} " +
                        $"{placemark.AdminArea} " +
                        $"{placemark.PostalCode} " +
                        $"{placemark.CountryName} ";

                    Property.Address = geocodeAddress;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private async void NewAddressButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddressEntry.Text))
            {
                await DisplayAlert("Error", "Please fill out the address", "OK");
            }
            else
            {
                try
                {
                    var location = await Geocoding.GetLocationsAsync(Property.Address);

                    Property.Latitude = location.FirstOrDefault().Latitude;
                    Property.Longitude = location.FirstOrDefault().Longitude;
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    // Handle not supported on device exception
                }
                catch (FeatureNotEnabledException fneEx)
                {
                    // Handle not enabled on device exception
                }
                catch (PermissionException pEx)
                {
                    // Handle permission exception
                }
                catch (Exception ex)
                {
                    // Unable to get location
                }
            }
        }
        #endregion

        #region Connectivity
        public async void CheckConnectivity()
        {
            try
            {
                var connection = Connectivity.NetworkAccess;

                if (connection != NetworkAccess.Internet && connection != NetworkAccess.ConstrainedInternet)
                {
                    await DisplayAlert("Alert", "You do not have access to the internet", "OK");

                    GetLocationButton.IsEnabled = false;
                    NewAddressButton.IsEnabled = false;
                }
                else
                {
                    GetLocationButton.IsEnabled = true;
                    NewAddressButton.IsEnabled = true;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private void ConnectionTester(object sender, ConnectivityChangedEventArgs e)
        {
            CheckConnectivity();
        }
        #endregion

        #region Battery&Flashlight
        private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            var level = e.ChargeLevel;
            var state = e.State;
            
            if(level < 0.2)
            {
                if(state == BatteryState.Charging)
                {
                    StatusColor = Color.Yellow;
                    StatusMessage = "Battery level is under 20%";
                }
                else
                {
                    StatusMessage = "Battery level is under 20%";
                    StatusColor = Color.Red;
                }


            }
        }

        private void OnEnergySaverStatusChanged(object sender, EnergySaverStatusChangedEventArgs e)
        {
            if (e.EnergySaverStatus == EnergySaverStatus.On)
            {
                StatusColor = Color.Green;
            }
            else
            {
                StatusColor = Color.Default;
            }
        }

        private async void ToggleFlashLight_Clicked(object sender, EventArgs e)
        {

            try
            {
                if (LightToggler)
                {
                    await Flashlight.TurnOnAsync();
                    LightToggler = true;
                }
                else
                {
                    await Flashlight.TurnOffAsync();
                    LightToggler = false;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to turn on/off flashlight
            }

        }

        #endregion

        private async void CompassButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new CompassPage(Property.Aspect)));
        }
    }
}