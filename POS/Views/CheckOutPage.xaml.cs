using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.ViewModels;
using Xamarin.Forms;

namespace POS.Views
{
    public partial class CheckOutPage : ContentPage
    {
        CheckOutViewModel ViewModel => (CheckOutViewModel)BindingContext;

        public CheckOutPage()
        {
            InitializeComponent();
            BindingContext = new CheckOutViewModel();
        }
    }
}
