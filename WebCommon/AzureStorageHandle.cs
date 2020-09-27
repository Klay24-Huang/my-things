using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    /// <summary>
    /// 存取AzureStoreage
    /// </summary>
    public class AzureStorageHandle
    {
        public string connectionString = (ConfigurationManager.AppSettings["storageStr"] == null) ? "" : ConfigurationManager.AppSettings["storageStr"].ToString();
        public bool UploadImage(string FileBase64Str,string dir,string FileName)
        {
            bool flag = true;
            // Retrieve storage account from connection string.
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =  blobServiceClient.GetBlobContainerClient(dir);
            // Create the blob client.
            BlobClient blobClient = containerClient.GetBlobClient(FileName);

            return flag;
        }
    }
}
