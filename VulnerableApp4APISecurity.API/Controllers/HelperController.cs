using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VulnerableApp4APISecurity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly ProfileRepository _profileRepository;
        private readonly JWTAuthManager _jwtAuthManager;
        private string? FileUploadPath = "";

        public HelperController(AccountRepository accountRepository, ProfileRepository profileRepository, JWTAuthManager jWTAuthManager)
        {
            _accountRepository = accountRepository;
            _profileRepository = profileRepository;
            _jwtAuthManager = jWTAuthManager;

            FileUploadPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FileUploadPath").Value;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ShowLog()
        {
            try
            {
                var logs = await System.IO.File.ReadAllTextAsync("./Log/Serilog.txt");
                return Ok(new SuccessResponse{ Message = logs });
            }
            catch (Exception ex)
            {
                return BadRequest(new FailedResponse { Message = "There is a problem while show log " + ex.ToString() });
            }
            
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> ShowPorfileAsHtmlFormat()
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);

            if (UserId != null)
            {
                var user = await _accountRepository.GetByIdAsync(UserId);

                if (user.Email is not null)
                {
                    var profile = await _profileRepository.GetProfileByEmail(user.Email);

                    return new ContentResult
                    {
                        ContentType = "text/html",
                        Content = string.Format("<div>Username {0}</div>" +
                        "<div>Surname : {1}</div>" +
                        "<div>Email : {2}</div>" +
                        "<div>Password : {3}</div>" +
                        "<div>Address : {4}</div>" +
                        "<div>Hobby : {5}</div>" +
                        "<div>Birtday : {6}</div>", user.Name, user.Surname, user.Email, user.Password, profile.Address, profile.Hobby, profile.Birthday)
                    };
                }
                else
                {
                    return BadRequest(new FailedResponse { Message = "There is an while getting UserId data. Please try later." });

                }

            }
            else
            {
                return BadRequest(new FailedResponse { Message = "There is an error while write user and profile information. Please control profile section and create if you did not" });
            }

        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult SystemDate([FromQuery] string? arguments)
        {
            var payload = "date" + arguments;
            var shellName = "/bin/bash";
            var argsPrepend = "-c ";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shellName = @"C:\Windows\System32\cmd.exe";
                argsPrepend = "/c ";
            }

            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = shellName,
                        Arguments = argsPrepend + "\"" + payload + "\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return Ok(new SuccessResponse { Message = output });
            }
            catch (Exception ex)
            {
                return BadRequest(new FailedResponse { Message = ex.ToString() });
            }

        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var user = await _accountRepository.GetByIdAsync(UserId);

            string FileName = file.FileName;
            string FolderPath = FileUploadPath + user.Name + "/";

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string FilePath = FolderPath + FileName;

            try
            {
                using (var localFile = System.IO.File.OpenWrite(FilePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new FailedResponse { Message = ex.ToString() });
            }

            return Ok(new SuccessResponse { Message = "File is uploaded" });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> ListUploadedFile()
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var user = await _accountRepository.GetByIdAsync(UserId);


            List<string> fileList = new List<string>();

            string FolderPath = FileUploadPath + user.Name;

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string[] files = Directory.GetFiles(FolderPath);

            foreach (var item in files)
            {
                fileList.Add(string.Format("File name {0}, File size : {1}", Path.GetFileName(item), new FileInfo(item).Length));
            }

            return Ok(fileList);

        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> DownloadImageFromRemote(string url)
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var user = await _accountRepository.GetByIdAsync(UserId);

            string FileName = Guid.NewGuid() + ".png";
            string FolderPath = FileUploadPath + user.Name + "/";

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string FilePath = FolderPath + FileName;
            try
            {

                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(url), FilePath);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new FailedResponse { Message = ex.ToString() });
            }

            return Ok(new SuccessResponse { Message = "Image is Downloaded" });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> DownloadImageFromLocal(string filename)
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var user = await _accountRepository.GetByIdAsync(UserId);

            string FolderPath = FileUploadPath + user.Name + "/";

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(FolderPath + filename, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/png", "download");


        }
    }
}

