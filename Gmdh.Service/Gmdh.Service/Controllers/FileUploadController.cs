using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Gmdh.Core;

namespace Gmdh.Service.Controllers
{
    public class FileUploadController : ApiController
    {
        public FileUploadController()
        {
            
        }
        [HttpPost]
        [Route("api/file/upload")]
        public HttpResponseMessage UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var fileModel = new FileModel();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    fileModel.Name = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    fileModel.Ext = Path.GetExtension(postedFile.FileName);
                    //fileData = FilesOperations.ReadFile(filePath);
                }
                return HttpResponseUtils.CreateResponseWithJson(HttpStatusCode.Created, fileModel);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Route("api/file")]
        [HttpGet]
        public HttpResponseMessage GetFileData(string name)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
            var data = FilesOperations.ReadFile(filePath);
            return HttpResponseUtils.CreateResponseWithJson(HttpStatusCode.OK, data);
        }
        [HttpGet]
        [Route("api/recentfiles")]
        public IEnumerable<FileModel> GetRecentFiles()
        {
            var path = HttpContext.Current.Server.MapPath("~/App_Data");
            var files = FilesOperations.GetFiles(path);
            return files.Select(f => new FileModel()
            {
                Ext = Path.GetExtension(f),
                Name = Path.GetFileNameWithoutExtension(f)
            });
        }
        [HttpPost]
        [Route("api/file/save")]
        public string SaveFile(CombiModel model)
        {
            var filename = $"{Guid.NewGuid()}.txt";
            var path  = HttpContext.Current.Server.MapPath($"~/App_Data/{filename}");
            var pathToFile = FilesOperations.SaveToTextFile(model, path);
            return filename;
        }
        [Route("api/file/get")]
        public HttpResponseMessage GetFile(string name)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read))
            };
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            message.Content.Headers.ContentDisposition.FileName = "result.txt";
            return message;
        }

       
        public class FileModel
        {
            public string Name { get; set; }
            public string Ext { get; set; }
        }
    }
}

