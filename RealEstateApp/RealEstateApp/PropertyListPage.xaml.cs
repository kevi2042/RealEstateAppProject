using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyListPage : ContentPage
    {
        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; set; } = new ObservableCollection<PropertyListItem>();
        public Xamarin.Essentials.Location location { get; set; } = new Location();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();

            LoadProperties();
            BindingContext = this; 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadProperties();
        }

        void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            LoadProperties();
            list.IsRefreshing = false;
        }

        async void LoadProperties()
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();

            try
            {
                location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync();
                }

                foreach (Property item in items)
                {
                    PropertyListItem listitem = new PropertyListItem(item);
                    Location currLocation = new Location((double)item.Latitude, (double)item.Longitude);
                    listitem.Distance = Xamarin.Essentials.Location.CalculateDistance(currLocation, location, DistanceUnits.Kilometers);
                    PropertiesCollection.Add(listitem);
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

        public async void SortAsync()
        {
            PropertiesCollection = new ObservableCollection<PropertyListItem>(PropertiesCollection.OrderBy(x => x.Distance));
        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }

        private void SortButton_Clicked(object sender, EventArgs e)
        {
            LoadProperties();
            SortAsync();
        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                SecureStorage.RemoveAll();

                App.Current.MainPage = new LoginPage();
            }
            catch (Exception ex)
            {

            }
        }
    }
}