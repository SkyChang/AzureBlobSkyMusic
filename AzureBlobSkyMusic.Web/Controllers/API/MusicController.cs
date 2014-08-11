using AzureBlobSkyMusic.Web.Handler;
using AzureBlobSkyMusic.Web.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureBlobSkyMusic.Web.Controllers.API
{
    public class MusicController : ApiController
    {
        CloudBlobContainer _container;

        public MusicController()
        {
            CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
            //CloudStorageAccount account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));
            CloudBlobClient client = account.CreateCloudBlobClient();
            //取得Container。
            _container = client.GetContainerReference("skymusic");
            _container.CreateIfNotExists();
            _container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        /// <summary>
        /// 上傳檔案 // POST api/Music
        /// </summary>
        /// <param name="files">上傳的檔案</param>
        /// <returns></returns>
        public Task<List<FileDetail>> Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var multipartStreamProvider = new CustomMultipartFormDataStreamProvider(_container);
            return Request.Content.ReadAsMultipartAsync<CustomMultipartFormDataStreamProvider>(multipartStreamProvider).ContinueWith<List<FileDetail>>(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                CustomMultipartFormDataStreamProvider provider = t.Result;
                return provider.Files;
            });
        }
    }
}





