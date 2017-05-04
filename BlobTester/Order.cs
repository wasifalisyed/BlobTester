using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace BlobTester
{
   public class Order:TableEntity
    {
        public Order(string customerName, String orderDate)
        {
            this.PartitionKey = customerName;
            this.RowKey = orderDate;
        }
        public Order() { }
        public string OrderNumber { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string Status { get; set; }
    }
}

