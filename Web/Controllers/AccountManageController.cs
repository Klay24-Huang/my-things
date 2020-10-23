using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class AccountManageController : Controller
    {
       /// <summary>
       /// 加盟業者維護
       /// </summary>
       /// <returns></returns>
        public ActionResult FranchiseesMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FranchiseesMaintain(string Operator,string OperatorName,string StartDate,string EndDate,string hidPic, HttpPostedFileBase fileImport)
        {
            if (fileImport != null)
            {
                bool flag = UploadFileToAzureStorage(fileImport,"operatoricon");
            }
            if (!string.IsNullOrEmpty(hidPic))
            {
                bool flag = UploadFileToAzureStorage(hidPic, "iRent.png", "operatoricon");
            }
            return View();
        }
        /// <summary>
        /// 功能群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncGroupMaintain()
        {
            return View();
        }
        /// <summary>
        /// 功能維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncMaintain()
        {
            return View();
        }
        /// <summary>
        /// 使用者群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserGroupMaintain()
        {
            return View();
        }
        /// <summary>
        /// 使用者維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserMaintain()
        {
            return View();
        }
        private bool UploadFileToAzureStorage(HttpPostedFileBase file,string ContainerName)
        {
            bool flag = true;
           string file_extension = Path.GetExtension(file.FileName);
           string filename_withExtension = Path.GetFileName(file.FileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        CloudConfigurationManager.GetSetting("StorageConnectionString"));

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
        private bool UploadFileToAzureStorage(string fileStr,string fileName, string ContainerName)
        {
            bool flag = true;
            FileStream fs;
            byte[] imageBytes = Convert.FromBase64String(fileStr.Replace("⊙",""));
            fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            fs.Write(imageBytes, 0, imageBytes.Length);
            
            string file_extension = Path.GetExtension(fileName);
            string filename_withExtension = Path.GetFileName(fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        CloudConfigurationManager.GetSetting("StorageConnectionString"));

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
            return flag;
        }

    }
}