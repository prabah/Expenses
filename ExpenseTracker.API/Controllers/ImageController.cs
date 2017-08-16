﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ExpenseTracker.API.Controllers
{
    public class ImageController : ApiController
    {
        private readonly CloudBlobContainer blobContainer;

        public ImageController()
        {
            var storageConnectionString = "UseDevelopmentStorage=true";

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // We are going to use Blob Storage, so we need a blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Data in blobs are organized in containers.
            // Here, we create a new, empty container.
            blobContainer = blobClient.GetContainerReference("myfirstcontainer");
            blobContainer.CreateIfNotExists();

            // We also set the permissions to "Public", so anyone will be able to access the file.
            // By default, containers are created with private permissions only.
            blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public static WebImage GetImageFromRequest()
        {
            var request = HttpContext.Current.Request;

            //if (request.Files.Length == 0)
            //{
            //    return null;
            //}

            try
            {
                var postedFile = request.Files[0];
                var image = new WebImage(postedFile.InputStream)
                {
                    FileName = postedFile.FileName
                };
                return image;
            }
            catch
            {
                // The user uploaded a file that wasn't an image or an image format that we don't understand
                return null;
            }
        }

        [HttpPost]
        [Route("image/upload")]
        public async Task<IHttpActionResult> Upload()
        {
            var image = GetImageFromRequest();
            var imageBytes = image.GetBytes();

            // The parameter to the GetBlockBlobReference method will be the name
            // of the image (the blob) as it appears on the storage server.
            // You can name it anything you like; in this example, I am just using
            // the actual filename of the uploaded image.
            var blockBlob = blobContainer.GetBlockBlobReference(image.FileName);
            blockBlob.Properties.ContentType = "image/" + image.ImageFormat;

            await blockBlob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);

            return Ok();
        }
    }
}
