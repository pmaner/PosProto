using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using POS.Interfaces;
using POS.Models;
using POS.Services;
using Xamarin.Forms;

namespace POS.ViewModels
{
    public class CheckOutViewModel : ViewModelBase
    {
        public IDataStore<ISaleItem> TransactionDataStore => DependencyService.Get<IDataStore<ISaleItem>>();
        public IDataStore<Product> ProductDataStore => DependencyService.Get<IDataStore<Product>>();
        public DiscountEngine DiscountEngine => DependencyService.Get<DiscountEngine>();

        public ICommand State1Command { get; }
        public ICommand State2Command { get; }
        public ICommand State3Command { get; }

        public ICommand SetDiscountCommand { get; }
        public ICommand EditDiscountCommand { get; }

        public BindingList<ISaleItem> Cart { get; }
        public decimal CartTotal => Cart.Sum(item => item.TotalPrice);


        decimal _cartTotalOverride;
        public decimal CartTotalOveride
        {
            get { return _cartTotalOverride; }
            set
            {
                if (SetProperty(ref _cartTotalOverride, value))
                {

                }
            }
        }

        bool _isEditingTotal = false;
        public bool IsEditingTotal
        {
            get { return _isEditingTotal; }
            set { SetProperty(ref _isEditingTotal, value); }
        }

        public CheckOutViewModel()
        {
            Cart = new BindingList<ISaleItem>();

            // should be handled better if it is really a long service call
            foreach (var item in TransactionDataStore.GetItemsAsync().Result)
                Cart.Add(new SaleItemViewModel(item));

            State1Command = new Command(() =>
            {
                ((MockProductDataStore)ProductDataStore).Init(MockProductDataStore.CreateNoMinimumsProducts());
                UpdateDiscounts();
            });

            State2Command = new Command(() =>
            {
                ((MockProductDataStore)ProductDataStore).Init(MockProductDataStore.CreateSimpleMinimumsProducts());
                UpdateDiscounts();
            });

            State3Command = new Command(() =>
            {
                ((MockProductDataStore)ProductDataStore).Init(MockProductDataStore.CreateAllMinimumsProducts());
                UpdateDiscounts();
            });

            SetDiscountCommand = new Command(() =>
            {
                IsEditingTotal = false;
                UpdateDiscounts();
            });
            EditDiscountCommand = new Command(() =>
            {
                IsEditingTotal = true;
                CartTotalOveride = CartTotal;
            });
        }

        private void UpdateDiscounts()
        {
            DiscountEngine.Discount((IList<ISaleItem>)Cart,
                        ProductDataStore.GetItemsAsync().Result.ToList(),
                        CartTotalOveride);
            NotifyPropertyChanged(() => CartTotal);
        }
    }
}
