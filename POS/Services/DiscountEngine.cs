using System;
using System.Collections.Generic;
using System.Linq;
using POS.Interfaces;
using POS.Models;

namespace POS.Services
{
    /// <summary>
    /// Encapsulates the logic for discounting. I would imangine in a real system this would be
    /// some type of chain of responsibility pattern as you will have different discounting algorithms
    /// that will be able to be applied and probably configured by users.
    /// </summary>
    public class DiscountEngine
    {
        public DiscountEngine()
        {
        }

        /// <summary>
        /// Discount the specified items, products and salePrice.
        /// Better to have the discount engine keep a cache of the products that have be purchased
        /// as in most shops there are lots of products, but not all get purchased often
        ///      Item Discount = Item Quantity * Item Unit Price * Total Discount/salePrice
        /// </summary>
        /// <returns>The modifed list of items that has been discounted</returns>
        /// <param name="items">Items to be discounted, these are changed</param>
        /// <param name="products">Products with minium pricing</param>
        /// <param name="salePrice">Sale price.</param>
        public IList<ISaleItem> Discount(IList<ISaleItem> items, IList<Product> products, decimal salePrice)
        {
            // As you asked for the function to return the list of Sale Items I have returned the list passed in
            // if you really want new object I can put a clone method in to recreate all the object, but this
            // will mess a bit with the binding of the objects in the list so I have modifed them in place and
            // just returned the same list of sale items

            // reset the discount
            foreach (var item in items)
                item.Discount = 0M;

            if (salePrice <= 0) // cant give money away
                return items;
                
            var remainingDiscountableItems = new List<ISaleItem>(items);
            var totalPrice = remainingDiscountableItems.Sum(i => i.TotalPrice);
            var totalUnappliedDiscount = totalPrice - salePrice;

            if (totalPrice < salePrice) // dont put prices up
                return items;

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
                    return items;
                var error = newTotal-salePrice;
                var largestUndiscountedItem = remainingDiscountableItems.OrderBy(x => x.TotalPrice).Last();
                largestUndiscountedItem.Discount += error;
            }

            return items;
        }

        /// <summary>
        /// Loop through the items and apply the discount up until the limit is reached
        /// sale items that have reached their maximum discount are removed from the items list
        /// to avoid future processing
        /// </summary>
        /// <returns>the unapplied portion of the discount</returns>
        /// <param name="items">Items.</param>
        /// <param name="products">Products.</param>
        /// <param name="discountPercent">Discount percent.</param>
        static private decimal DiscountRemainingItems(IList<ISaleItem> items, IList<Product> products, decimal discountPercent)
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
        static private decimal MinPrice (ISaleItem item, IList<Product> products)
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
        static private decimal ApplyDiscount(ISaleItem item, decimal minPrice, decimal discount)
        {
            var discountToApply = Math.Min(discount, (item.UnitPrice - minPrice)*item.Quantity);
            item.Discount += discountToApply;
            return discount - discountToApply;
        }
    }
}
