using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chick_fil_A.Models;

namespace Chick_fil_A.Services
{
    public class NFCSelectDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public NFCSelectDataStore()
        {
            items = new List<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "Reward Redemption", Description="This is an item description.", Properties = new List<dynamic>() { 100 } },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Reward Receiver", Description="This is an item description.", Properties = new List<dynamic>() { 200 } },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Special Marketing Events", Description="This is an item description.", Properties = new List<dynamic>() { } },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Incentives for Event Participation", Description="This is an item description.", Properties = new List<dynamic>() { } },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Associated Topics", Description="This is an item description.", Properties = new List<dynamic>() { } },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}