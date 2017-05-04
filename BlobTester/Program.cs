using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BlobTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // TableWriter();
            // UploadFile();
            //QueryRecords();
            QueueWriteAndRead();
        }
        static void UploadFile()
        {
            var connection = ConfigurationManager.AppSettings.Get("connection");
            var localPath = ConfigurationManager.AppSettings.Get("LocalPath");
            var storageAccount = CloudStorageAccount.Parse(connection);
            var blobAccount = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobAccount.GetContainerReference("wasifoutput");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob/rohial/wasif.txt");
            using (var file = System.IO.File.OpenRead(localPath + "\\rohail.txt"))
            {
                blockBlob.UploadFromStream(file);
            }
            //creating copy blob
            CloudBlobContainer copy = blobAccount.GetContainerReference("destination2");
            copy.CreateIfNotExists(BlobContainerPublicAccessType.Off);
            AsyncCallback cb = new AsyncCallback(x => Console.WriteLine("copy completed with {0}", x.IsCompleted));
            //foreach (var item in container.ListBlobs(null, false))
            //{
            //    if (item.GetType() == typeof(CloudBlockBlob))
            //    {
            //        CloudBlockBlob blob = (CloudBlockBlob)item;
            //        // Console.WriteLine("item url{0} and item name{1}", blob.Uri, blob.Name);
            //        ICloudBlob blobCopy = copy.GetBlockBlobReference(blob.Name);
            //        blobCopy.BeginStartCopyFromBlob(blob.Uri, new AsyncCallback(CompleteRead),blob.Name);
            //    }

            //}
            Console.ReadLine();
        }
        static void CompleteRead(IAsyncResult result)
        {
            var str = (String)result.AsyncState;
            Console.WriteLine(str);

        }
        static void TableWriter()
        {
            try
            {
                var connection = ConfigurationManager.AppSettings.Get("connection");
                var localPath = ConfigurationManager.AppSettings.Get("LocalPath");
                var storageAccount = CloudStorageAccount.Parse(connection);
                var tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("orders");
                table.CreateIfNotExists();
                //Order newOrder = new Order("Archer", "20141216");
                //newOrder.OrderNumber = "101";
                //newOrder.ShippedDate = DateTime.Now;
                //newOrder.RequiredDate = DateTime.Now;
                //newOrder.Status = "shipped";
                //TableOperation insertOperation = TableOperation.InsertOrMerge(newOrder);
                //table.Execute(insertOperation);
                // Batch operation test
                TableBatchOperation batchOperation = new TableBatchOperation();
                Order newOrder1 = new Order("Archer", "20141217");
                newOrder1.OrderNumber = "101";
                newOrder1.ShippedDate = DateTime.Now;
                newOrder1.RequiredDate = DateTime.Now;
                newOrder1.Status = "shipped";
                Order newOrder2 = new Order("Archer", "20141218");
                newOrder2.OrderNumber = "101";
                newOrder2.ShippedDate = DateTime.Now;
                newOrder2.RequiredDate = DateTime.Now;
                newOrder2.Status = "shipped";
                batchOperation.Insert(newOrder1);
                batchOperation.Insert(newOrder2);
                table.ExecuteBatch(batchOperation);
            }
            catch(Exception ex)
            {

            }
        }
        static void QueryRecords()
        {
            var connection = ConfigurationManager.AppSettings.Get("connection");
            var localPath = ConfigurationManager.AppSettings.Get("LocalPath");
            var storageAccount = CloudStorageAccount.Parse(connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var orders = tableClient.GetTableReference("orders");
            TableQuery<Order> tq = new TableQuery<Order>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Archer"));
            foreach(Order ord in orders.ExecuteQuery(tq))
            {
                Console.WriteLine(ord.PartitionKey);
            }
        }
        static void QueueWriteAndRead()
        {
            try
            { 
            var policy = new QueueSaasToken();
            string token = policy.GenerateQueueSaasToken();
            StorageCredentials creds = new StorageCredentials(token);
            Uri uri = new Uri("https://wasifblob.queue.core.windows.net/");

            CloudQueueClient sasClient = new CloudQueueClient(new StorageUri(uri), creds);
            var queue = sasClient.GetQueueReference("result");
            queue.AddMessage(new CloudQueueMessage("new message"));
        }catch(Exception ex)
            {

            }
        }
    }
}
