using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Functions
{
    public class OrderTest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string BuyerId { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        // Using List<>.AsReadOnly() 
        // This will create a read only wrapper around the private list so is protected against "external updates".
        // It's much cheaper than .ToList() because it will not have to copy all items in a new collection. (Just one heap alloc for the wrapper instance)
        //https://msdn.microsoft.com/en-us/library/e78dcd75(v=vs.110).aspx 
        public List<OrderItem> OrderItems => _orderItems;

        public decimal Total
        {
            get
            {
                var total = 0m;
                foreach (var item in _orderItems)
                {
                    total += item.UnitPrice * item.Units;
                }
                return total;
            }
        }
    }

    public class OrderItem
    {
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
