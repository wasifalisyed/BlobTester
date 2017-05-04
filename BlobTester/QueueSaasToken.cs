using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace BlobTester
{
   public class QueueSaasToken
    {
        public string GenerateQueueSaasToken()
        {
            var connection = ConfigurationManager.AppSettings.Get("connection");
            var localPath = ConfigurationManager.AppSettings.Get("LocalPath");
            var storageAccount = CloudStorageAccount.Parse(connection);
            var bLobClient = storageAccount.CreateCloudBlobClient();
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = DateTime.Now.AddHours(1);
            policy.SharedAccessStartTime= DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0));
            policy.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;
            var blobContainer = bLobClient.GetContainerReference("wasifoutput");
            var saasToken = blobContainer.GetSharedAccessSignature(policy);


            return saasToken;
        }
    }
}
