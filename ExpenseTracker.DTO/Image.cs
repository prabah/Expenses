using System;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;

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

        public int ImageType { get; set; }

        public int ClientId { get; set; }

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
                    ClientId = 1,
                    ContainerName = blob.Container.Name
                };
            }
            return null;
        }
    }
}