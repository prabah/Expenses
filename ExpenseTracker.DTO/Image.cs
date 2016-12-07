using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System.ComponentModel;
using ExpenseTracker.Constants;

namespace ExpenseTracker.DTO
{
    public class Image
    {
        public string FileName { get; set; }

        public string URL { get; set; }

        public long Size { get; set; }

        public long BlockCount { get; set; }

        public CloudBlockBlob BlockBlob { get; set; }

        public DateTime StartTime { get; set; }

        public string UploadStatusMessage { get; set; }

        public bool IsUploadCompleted { get; set; }

        public string FileKey { get; set; }

        public int FileIndex { get; set; }

        public ImageFileType ImageType { get; set; }

        public string ClientId { get; set; }

        public string ContainerName { get; set; }

        public static Image CreateFromIListBlobItem(IListBlobItem item, string cdnEndpoint)
        {
            if (item is CloudBlockBlob)
            {
                var blob = (CloudBlockBlob)item;
                blob.FetchAttributes();

                var urlPathAndQuery = blob.Uri.PathAndQuery;

                return new Image
                {
                    FileName = blob.Name,
                    URL = cdnEndpoint + urlPathAndQuery,
                    Size = blob.Properties.Length,
                    ClientId = "client_id",
                    ContainerName = blob.Container.Name
                };
            }
            return null;
        }

        public enum ImageFileType
        {
            Logo = 1,
            Marketing = 2,
        }
    }
}