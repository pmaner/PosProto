using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Interfaces;
using POS.Models;

namespace POS.Services
{
    public class MockProductDataStore : IDataStore<Product>
    {
        List<Product> _items;

        public MockProductDataStore()
        {
            _items = new List<Product>();
            Init(CreateNoMinimumsProducts());
        }

        public void Init(IList<Product> products)
        {
            _items.Clear();
            foreach (var item in products)
            {
                _items.Add(item);
            }
        }

        public static IList<Product> CreateNoMinimumsProducts()
        {
            var mockItems = new List<Product>
            {
                new Product() { ProductId = 1,  MinimumPrice = 0 }, // Widget
                new Product() { ProductId = 2,  MinimumPrice = 0 }, // Gadget
                new Product() { ProductId = 3,  MinimumPrice = 0 }, // Gizmo
            };
            return mockItems;
        }

        public static IList<Product> CreateSimpleMinimumsProducts()
        {
            var mockItems = new List<Product>
            {
                new Product() { ProductId = 1,  MinimumPrice = 0 }, // Widget
                new Product() { ProductId = 2,  MinimumPrice = 2.50M }, // Gadget
                new Product() { ProductId = 3,  MinimumPrice = 0 }, // Gizmo
            };
            return mockItems;
        }

        public async Task<bool> AddItemAsync(Product item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Product item)
        {
            var oldItem = _items.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
            _items.Remove(oldItem);
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = _items.Where( arg => arg.ProductId == id).FirstOrDefault();
            _items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Product> GetItemAsync(int id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.ProductId == id));
        }

        public async Task<IEnumerable<Product>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }
    }
}
