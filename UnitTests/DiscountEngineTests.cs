using System;
using POS.Services;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace UnitTests
{
    public class DiscountEngineTests
    {
        const decimal KnownTestTotal = 14.70M;

        [Fact]
        public void TestTransactionShouldBeKnownQuantity()
        {
            var transactions = MockTransactionDataStore.CreateTestSale();

            var total = transactions.Sum(i => i.TotalPrice);

            Assert.Equal(KnownTestTotal, total);
        }

        [Fact]
        public void NoDiscountShouldBeSameAsZeroDiscount()
        {
            var discountEngine = new DiscountEngine();
            var transactions = MockTransactionDataStore.CreateTestSale();
            var products = MockProductDataStore.CreateNoMinimumsProducts();

            var totalBeforeDiscount = transactions.Sum(i => i.TotalPrice);

            discountEngine.Discount(transactions, products, totalBeforeDiscount, totalBeforeDiscount);

            var totalAfterDiscount = transactions.Sum(i => i.TotalPrice);

            Assert.Equal(totalBeforeDiscount, totalAfterDiscount);
        }

        [Fact]
        public void DiscountWithNoRestraintsShouldBeEvenlyDistributed()
        {
            var discountEngine = new DiscountEngine();
            var transactions = MockTransactionDataStore.CreateTestSale();
            var products = MockProductDataStore.CreateNoMinimumsProducts();
            var paid = 10M;

            discountEngine.Discount(transactions, products, KnownTestTotal, paid);

            var totalAfterDiscount = transactions.Sum(i => i.TotalPrice);

            Assert.Equal(paid, Math.Round(totalAfterDiscount, 2));

            Assert.Equal(0.56M, Math.Round(transactions[0].Discount, 2));
            Assert.Equal(1.89M, Math.Round(transactions[1].Discount, 2));
            Assert.Equal(2.25M, Math.Round(transactions[2].Discount, 2));
        }

        [Fact]
        public void DiscountWithRestraintsShouldDistubiteAcrossOtherItems()
        {
            var discountEngine = new DiscountEngine();
            var transactions = MockTransactionDataStore.CreateTestSale();
            var products = MockProductDataStore.CreateSimpleMinimumsProducts();
            var paid = 10M;

            discountEngine.Discount(transactions, products, KnownTestTotal, paid);

            var totalAfterDiscount = transactions.Sum(i => i.TotalPrice);

            Assert.Equal(paid, Math.Round(totalAfterDiscount, 2));

            Assert.Equal(0.76M, Math.Round(transactions[0].Discount, 2));
            Assert.Equal(0.90M, Math.Round(transactions[1].Discount, 2));
            Assert.Equal(3.04M, Math.Round(transactions[2].Discount, 2));
        }

        [Fact]
        public void DiscountWithLeftOverGoesToHighestPriceLineItem()
        {
            var discountEngine = new DiscountEngine();
            var transactions = MockTransactionDataStore.CreateTestSale();
            var products = MockProductDataStore.CreateNoMinimumsProducts();
            var paid = 10.23M;

            discountEngine.Discount(transactions, products, KnownTestTotal, paid);

            var totalAfterDiscount = transactions.Sum(i => i.TotalPrice);

            Assert.Equal(paid, Math.Round(totalAfterDiscount, 2));

            Assert.Equal(0.53M, Math.Round(transactions[0].Discount, 2));
            Assert.Equal(1.79M, Math.Round(transactions[1].Discount, 2));
            Assert.Equal(2.15M, Math.Round(transactions[2].Discount, 2));
        }
    }
}
