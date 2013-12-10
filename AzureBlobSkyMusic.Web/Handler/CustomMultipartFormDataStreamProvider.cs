using AzureBlobSkyMusic.Web.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AzureBlobSkyMusic.Web.Handler
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        private CloudBlobContainer _container;
        public List<FileDetail> Files { get; set; }

        public CustomMultipartFormDataStreamProvider(CloudBlobContainer container) 
            : base(Path.GetTempPath()) 
        { 
            _container = container;
            Files = new List<FileDetail>(); 
        }

        

        public override Task ExecutePostProcessingAsync()
        {
            // Upload the files to azure blob storage and remove them from local disk 
            foreach (var fileData in this.FileData)
            {
                string fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));

                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = "audio/mpeg3";
                blockBlob.UploadFromFile(fileData.LocalFileName,FileMode.Open);
                File.Delete(fileData.LocalFileName);
                Files.Add(new FileDetail
                {
                    ContentType = blockBlob.Properties.ContentType,
                    Name = blockBlob.Name,
                    Size = blockBlob.Properties.Length,
                    Location = blockBlob.Uri.AbsoluteUri
                });
            }

            return base.ExecutePostProcessingAsync();
        } 

    }
}