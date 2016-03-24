using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Calculator
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            var masterDetailPage = new MasterDetailPage();
            masterDetailPage.Detail = new CalculatorView();
            masterDetailPage.Master = new ContentPage { Title = "menu" }; //placeholder that will eventually become a menu
            MainPage = masterDetailPage;
           
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
