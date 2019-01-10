using System;
using POS.Interfaces;
using POS.Models;

namespace POS.ViewModels
{
    public class SaleItemViewModel : ViewModelBase, ISaleItem
    {
        private ISaleItem _item;

        public int ProductId
        {
            get { return _item.ProductId; }
        }

        public string Description
        {
            get { return _item.Description; }
            set { _item.Description = SetProperty(_item.Description, value); }
        }

        public int Quantity
        {
            get { return _item.Quantity; }
            set { _item.Quantity = SetProperty(_item.Quantity, value, () => Quantity, () => TotalPrice, () => StateLabel); }
        }

        public decimal UnitPrice
        {
            get { return _item.UnitPrice; }
            set { _item.UnitPrice = SetProperty(_item.UnitPrice, value, () => UnitPrice, () => TotalPrice, () => StateLabel); }
        }

        public decimal Discount
        {
            get { return _item.Discount; }
            set { _item.Discount = SetProperty(_item.Discount, value, () => Discount, () => TotalPrice, () => StateLabel); }
        }

        public decimal TotalPrice
        {
            get { return _item.TotalPrice; }
        }


        public string StateLabel => (Discount == 0M) ? $"{Quantity} x {UnitPrice:C}"
                                                     : $"{Quantity} x {UnitPrice:C} with {Discount:C} discount";



        public SaleItemViewModel(ISaleItem item)
        {
            _item = item;
        }
    }
}
