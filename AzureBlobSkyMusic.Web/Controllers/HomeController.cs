using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureBlobSkyMusic.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateMusic()
        {
            return View();
        }

        public ActionResult MyAudio()
        {
            CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
            //CloudStorageAccount account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("skymusic");
            CloudBlockBlob blob = container.GetBlockBlobReference("Test.mp3");
            byte[] array;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                array = memoryStream.ToArray();
            }
            return File(array, "audio/mpeg3");
        }
    }
}