using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using POS.Views;
using POS.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace POS
{


    public partial class App : Application
    {
        public static bool UseMockDataStore = true;

        public App()
        {
            InitializeComponent();

            // /////////////////////////
            // Some services we will use
            // As it just a mockup with dont really need them but real apps do
            // so I throught I would put in some mock ones
            if (UseMockDataStore)
            {
                DependencyService.Register<MockProductDataStore>();
                DependencyService.Register<MockTransactionDataStore>();
                DependencyService.Register<DiscountEngine>(); 
            }

            MainPage = new CheckOutPage();
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
