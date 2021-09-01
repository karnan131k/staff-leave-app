using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using staff_leave_app.Authentication;
using staff_leave_app.Model;

namespace staff_leave_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly string AppDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private static List<FileRecord> fileDB = new List<FileRecord>();
        private readonly ApplicationDbContext dBcontext;
        public FileManagerController(ApplicationDbContext context)
        {
            this.dBcontext = context;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<HttpResponseMessage> PostAsync([FromForm] FileModel model)
        {
            try
            {
                FileRecord file = await SaveFileAsync(model.MyFile);

                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    file.AltText = model.AltText;
                    file.Description = model.Description;
                    //Save to Inmemory object
                    //fileDB.Add(file);
                    //Save to SQL Server DB
                    var profileImages = await dBcontext.FileData.Where(e => e.UserId == model.UserId).FirstOrDefaultAsync();
                    if (profileImages is null)
                    {
                        SaveToDB(file, model.UserId);
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else
                    {
                        dBcontext.Remove(profileImages);
                        SaveToDB(file, model.UserId);
                        //await dBcontext.SaveChangesAsync();
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                };
            }
        }
        private async Task<FileRecord> SaveFileAsync(IFormFile myFile)
        {
            FileRecord file = new FileRecord();
            if (myFile != null)
            {
                if (!Directory.Exists(AppDirectory))
                    Directory.CreateDirectory(AppDirectory);

                var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(myFile.FileName);
                var path = Path.Combine(AppDirectory, fileName);

                file.Id = fileDB.Count() + 1;
                file.FilePath = path;
                file.FileName = fileName;
                file.FileFormat = Path.GetExtension(myFile.FileName);
                file.ContentType = myFile.ContentType;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await myFile.CopyToAsync(stream);
                }

                return file;
            }
            return file;
        }

        private void SaveToDB(FileRecord record,string StaffId)
        {
            if (record == null)
                throw new ArgumentNullException($"{nameof(record)}");

            FileData fileData = new FileData();
            fileData.UserId = StaffId;
            fileData.FilePath = record.FilePath;
            fileData.FileName = record.FileName;
            fileData.FileExtension = record.FileFormat;
            fileData.MimeType = record.ContentType;

            dBcontext.FileData.Add(fileData);
            dBcontext.SaveChanges();
        }
        [HttpGet]
        [Route("GetUserAvatar/{UserId}")]
        public async Task<IActionResult> GetUserAvatar(string UserId)
        {
            //getting data from inmemory obj
            //return fileDB;
            //getting data from SQL DB
            if (UserId is null)
            {
                return BadRequest(new { message = "staff id is required" });
            }
            var profileImages = await dBcontext.FileData.Where(e => e.UserId == UserId).ToListAsync();
            return Ok(new {data = profileImages });

        }

    }
}
