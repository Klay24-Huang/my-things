using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebCommon
{
    /// <summary>
    /// 存取AzureStoreage
    /// </summary>
    public class AzureStorageHandle
    {
        public string connectionString = (ConfigurationManager.AppSettings["StorageConnectionStrings"] == null) ? "" : ConfigurationManager.AppSettings["StorageConnectionStrings"].ToString();
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
        /// <summary>
        /// HttpPost File到AzureStorage
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ContainerName"></param>
        /// <returns></returns>
        public bool UploadFileToAzureStorage(HttpPostedFileBase file, string ContainerName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            bool flag = true;
            string file_extension = Path.GetExtension(file.FileName);
            string filename_withExtension = Path.GetFileName(file.FileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //specified container name
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
            container.SetPermissions(
            new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            //  CloudBlockBlob blockBlob = container.GetBlockBlobReference("myBlob");
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = file.InputStream)
            {
                cloudBlockBlob.UploadFromStream(fileStream);
            }
            
            return flag;
        }
        /// <summary>
        /// HttpPost File到AzureStorage(可變更名稱)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ContainerName"></param>
        /// <returns></returns>
        public bool UploadFileToAzureStorage(HttpPostedFileBase file, string fileName, string ContainerName, string path)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            bool flag = true;
            string file_extension = Path.GetExtension(fileName);
            string filename_withExtension = Path.GetFileName(fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //specified container name
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't already exist.
            //container.CreateIfNotExists();
            container.SetPermissions(
            new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            //  CloudBlockBlob blockBlob = container.GetBlockBlobReference("myBlob");
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;
            

            string filePath = Path.GetFullPath(fileName);

            // Create or overwrite the "myblob" blob with contents from a local file.

            cloudBlockBlob.UploadFromFile(path);


            return flag;
        }
        /// <summary>
        /// Base64轉png後上傳到AzureStorage
        /// </summary>
        /// <param name="fileStr"></param>
        /// <param name="fileName"></param>
        /// <param name="ContainerName"></param>
        /// <returns></returns>
        public bool UploadFileToAzureStorage(string fileStr, string fileName, string ContainerName)
        {
            bool flag = true;

            //FileStream fs;
            //byte[] imageBytes = Convert.FromBase64String(fileStr.Replace("⊙", ""));
            //fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            //fs.Write(imageBytes, 0, imageBytes.Length);

            byte[] imageBytes = Convert.FromBase64String(fileStr.Replace("⊙", ""));
            Stream fs = new MemoryStream(imageBytes);
            string file_extension = Path.GetExtension(fileName);
            string filename_withExtension = Path.GetFileName(fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //specified container name
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(
            new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            //  CloudBlockBlob blockBlob = container.GetBlockBlobReference("myBlob");
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = fs)
            {
                cloudBlockBlob.UploadFromStream(fileStream);
            }
            fs.Close();
            return flag;
        }
        /// <summary>
        /// AzureStorage rename
        /// </summary>
        /// <param name="NewFileName"></param>
        /// <param name="FileName"></param>
        /// <param name="ContainerName"></param>
        /// <returns></returns>
        public bool RenameFromAzureStorage(string NewFileName, string FileName, string ContainerName)
        {
            bool flag = true;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
             CloudConfigurationManager.GetSetting("StorageConnectionStrings"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //specified container name
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(
            new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            CloudBlockBlob blobCopy = container.GetBlockBlobReference(NewFileName);
            if (!blobCopy.Exists())
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(FileName);

                if (blob.Exists())
                {
                    blobCopy.StartCopy(blob);
                    blob.DeleteIfExists();
                }
            }

            return flag;
        }

        public CloudBlockBlob DownloadFile(string ContainerName, string fileName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            bool flag = true;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //specified container name
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            container.CreateIfNotExists();

            var blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob;
        }
    }
}
