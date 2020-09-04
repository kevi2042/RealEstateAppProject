using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Services.Repository;
using System;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;

namespace RealEstateApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var container = TinyIoCContainer.Current;
            container.Register<IRepository, MockRepository>();




            MainPage = new LoginPage();

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
