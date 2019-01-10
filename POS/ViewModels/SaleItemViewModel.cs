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
            set { _item.Quantity = SetProperty(_item.Quantity, value, () => Quantity, () => TotalPrice); }
        }

        public decimal UnitPrice
        {
            get { return _item.UnitPrice; }
            set { _item.UnitPrice = SetProperty(_item.UnitPrice, value, () => UnitPrice, () => TotalPrice); }
        }

        public decimal Discount
        {
            get { return _item.Discount; }
            set { _item.Discount = SetProperty(_item.Discount, value, () => Discount, () => TotalPrice); }
        }

        public decimal TotalPrice
        {
            get { return _item.TotalPrice; }
        }

        public SaleItemViewModel(ISaleItem item)
        {
            _item = item;
        }
    }
}
