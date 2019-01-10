using System;
using System.Collections.Generic;
using System.Linq;
using POS.Interfaces;
using POS.Models;

namespace POS.Services
{
    public class DiscountEngine
    {
        public DiscountEngine()
        {
        }

        /// <summary>
        /// Better to have the discount engine keep a cache of the products that have be purchased
        /// as in most shops there are lots of products, but not all get purchased often
        ///      Item Discount = Item Quantity * Item Unit Price * Total Discount/<paramref name="totalPrice"/>
        /// </summary>
        /// <param name="items">Items.</param>
        /// <param name="products">Products.</param>
        /// <param name="totalDiscount">Total discount.</param>
        public void Discount(IList<ISaleItem> items, IList<Product> products, decimal totalPrice, decimal salePrice)
        {
            if (salePrice > totalPrice) // cant make it more expensive
                return;

            // reset the discount
            foreach (var item in items)
                item.Discount = 0M;
                
            var remainingDiscountableItems = new List<ISaleItem>(items);
            var totalUnappliedDiscount = totalPrice - salePrice;

            // Discount items based on percentage of the sale they unit price
            // of each item represents
            do
            {
                totalPrice = remainingDiscountableItems.Sum(i => i.TotalPrice);
                var discountPercent = totalUnappliedDiscount / totalPrice;
                totalUnappliedDiscount = DiscountRemainingItems(remainingDiscountableItems, products, discountPercent);
            } while ((remainingDiscountableItems.Count > 0) && (totalUnappliedDiscount > 0));


            // Everything is fully discounted do a sanity check for rounding errors
            var newTotal = items.Sum(i => i.TotalPrice);
            if (salePrice < newTotal) // 
            {
                if (remainingDiscountableItems.Count == 0)
                    return;
                var error = newTotal-salePrice;
                var largestUndiscountedItem = remainingDiscountableItems.OrderBy(x => x.TotalPrice).Last();
                largestUndiscountedItem.Discount += error;
            }
        }


        private decimal DiscountRemainingItems(IList<ISaleItem> items, IList<Product> products, decimal discountPercent)
        {
            var totalUnappliedDiscount = 0M;

            // Discount items based on percentage of the sale they unit price
            // of each item represents
            for (int index = items.Count - 1; index >= 0; index--)
            {
                var item = items[index];
                var minPrice = MinPrice(item, products);
                var unitPrice = item.TotalPrice / item.Quantity; // unit price has changed after first discount pass
                var unappliedDiscount = ApplyDiscount(item, minPrice, Math.Round(unitPrice * discountPercent * item.Quantity, 2));
                if (unappliedDiscount > 0) // reached full discount, so cant discount any more
                {
                    items.Remove(item);
                    totalUnappliedDiscount += unappliedDiscount;
                }
            }
            return totalUnappliedDiscount;
        }

        /// <summary>
        /// Logic for getting products minimum price, should cache the most purchased products
        /// or fetch from Datastore instead of passing in products list
        /// </summary>
        /// <returns>The price.</returns>
        /// <param name="item">Item.</param>
        /// <param name="products">Products.</param>
        private decimal MinPrice (ISaleItem item, IList<Product> products)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            //if (product == null)
            //    throw new ArgumentOutOfRangeException("There is not product by matching the sale item");
            if (product == null)
                return 0M;
            else
                return product.MinimumPrice;
        }

        /// <summary>
        /// Applies the discount to single item
        /// </summary>
        /// <returns>the unapplied portion of the discount</returns>
        /// <param name="item">Item.</param>
        /// <param name="discount">Discount for whole sale item</param>
        private decimal ApplyDiscount(ISaleItem item, decimal minPrice, decimal discount)
        {
            var discountToApply = Math.Min(discount, (item.UnitPrice - minPrice)*item.Quantity);
            item.Discount += discountToApply;
            return discount - discountToApply;
        }
    }
}
