using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        IRepository repos = TinyIoCContainer.Current.Resolve<IRepository>();

        public LoginPage()
        {
            InitializeComponent();

            if (CheckLogin().Result)
            {
                App.Current.MainPage = new MainPage();
            }
        }

        private async Task<bool> CheckLogin()
        {
            IRepository repos = TinyIoCContainer.Current.Resolve<IRepository>();

            try
            {
                LoginResult res = await repos.GetLoginStorage();
                if (res.Succeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoginResult res = await repos.LoginAsync(EntryUsername.Text, EntryPassword.Text);

                if (res.Succeded)
                {
                    App.Current.MainPage = new MainPage();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to login", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}