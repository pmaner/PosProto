using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Interfaces;
using POS.Models;

namespace POS.Services
{
    /// <summary>
    /// Just habbit making DataStores, would communicate with the backend
    /// </summary>
    public class MockTransactionDataStore : IDataStore<ISaleItem>
    {
        List<ISaleItem> _items;

        public MockTransactionDataStore()
        {
            _items = new List<ISaleItem>(CreateTestSale());
        }

        public static IList<ISaleItem> CreateTestSale()
        {
            var mockItems = new List<ISaleItem>
            {
                new SaleItem() { ProductId = 1,  Description = "Widget", Quantity = 1, UnitPrice = 1.75M }, 
                new SaleItem() { ProductId = 2,  Description = "Gadget", Quantity = 2, UnitPrice = 2.95M }, 
                new SaleItem() { ProductId = 3,  Description = "Gizmo", Quantity = 3, UnitPrice = 2.35M }, 
            };
            return mockItems;
        }


        public async Task<bool> AddItemAsync(ISaleItem item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(ISaleItem item)
        {
            var oldItem = _items.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
            _items.Remove(oldItem);
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = _items.Where(arg => arg.ProductId == id).FirstOrDefault();
            _items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<ISaleItem> GetItemAsync(int id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.ProductId == id));
        }

        public async Task<IEnumerable<ISaleItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }
    }
}
