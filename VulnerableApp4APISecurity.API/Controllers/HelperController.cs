using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

namespace VulnerableApp4APISecurity.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class HelperController : Controller
{
    private readonly AccountRepository _accountRepository;
    private readonly JWTAuthManager _jwtAuthManager;
    private readonly ProfileRepository _profileRepository;
    private readonly string? _fileUploadPath;

    public HelperController(AccountRepository accountRepository, ProfileRepository profileRepository,
        JWTAuthManager jWtAuthManager)
    {
        _accountRepository = accountRepository;
        _profileRepository = profileRepository;
        _jwtAuthManager = jWtAuthManager;

        _fileUploadPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FileUploadPath")
            .Value;
    }


    [SwaggerOperation(Summary = "Shows system logs. Requires Admin role. Vulnerability: Exposes potentially sensitive log data (API 03:2019), " +
                                "and could be abused for DoS if logs are very large (API 04:2019 - Lack of resources and rate limiting).")]
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult> ShowLog()
    {
        try
        {
            var logs = await System.IO.File.ReadAllTextAsync("./Log/Serilog.txt");
            return Ok(new SuccessResponse { Message = logs });
        }
        catch (Exception ex)
        {
            return BadRequest(new FailedResponse { Message = "There is a problem while show log " + ex });
        }
    }

    [SwaggerOperation(Summary = "Returns the current system date and time. " +
                                "Previously vulnerable to Command Injection (API 08:2019), now remediated.")]

    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    public ActionResult SystemDate()
    {
        try
        {
            // Get current date and time using .NET's built-in functionality
            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            return Ok(new SuccessResponse { Message = $"Current system date and time: {currentDateTime}" });
        }
        catch (Exception ex)
        {
            // Log the exception ex
            return StatusCode(500, new FailedResponse { Message = "An error occurred while retrieving the system date." });
        }
    }

    
    [SwaggerOperation(Summary = "Vulnerability = Upload big file. Category = API 04:2019 - Lack of resources and rate limiting")]
    [Authorize(Roles = "Admin,User")]
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var userId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
        var user = await _accountRepository.GetByIdAsync(userId);

        var fileName = file.FileName;
        var folderPath = _fileUploadPath + user.Name + "/";

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var filePath = folderPath + fileName;

        try
        {
            await using var localFile = System.IO.File.OpenWrite(filePath);
            await using var uploadedFile = file.OpenReadStream();
            await uploadedFile.CopyToAsync(localFile);
        }
        catch (Exception ex)
        {
            return BadRequest(new FailedResponse { Message = ex.ToString() });
        }

        return Ok(new SuccessResponse { Message = "File is uploaded" });
    }

    
    [SwaggerOperation(Summary = "Vulnerability = List uploaded file by name so IDOR/BOLA. Category = API 01:2019 - Broken object level authorization")]
    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    public async Task<ActionResult> ListFile([FromQuery] string name)
    {
        var user = await _accountRepository.GetAsync(user => user.Name == name);

        if (user is null) return Ok(new FailedResponse() { Message = "Name is not found" });

        var folderPath = _fileUploadPath + user.Name;

        try
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var files = Directory.GetFiles(folderPath);

            var fileList = files.Select(item =>
                $"File name {Path.GetFileName(item)}, File size : {new FileInfo(item).Length}").ToList();

            return Ok(fileList);
        }
        catch (Exception ex)
        {
            return Ok(new FailedResponse() { Message = ex.ToString() });
        }
       
    }

    [SwaggerOperation(Summary = "Vulnerability = SSRF Attack. Category = API 08:2019 - Injection.")]
    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    public async Task<IActionResult> GetImageFromRemote(string url)
    {
        var userId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
        var user = await _accountRepository.GetByIdAsync(userId);

        var fileName = Guid.NewGuid() + ".png";
        var folderPath = _fileUploadPath + user.Name + "/";

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var filePath = folderPath + fileName;
        try
        {
            using var client = new WebClient();
            client.DownloadFileAsync(new Uri(url), filePath);
        }
        catch (Exception ex)
        {
            return BadRequest(new FailedResponse { Message = ex.ToString() });
        }

        return Ok(new SuccessResponse { Message = "Image is Downloaded" });
    }

    [SwaggerOperation(Summary = "Vulnerability = LFI, Directory Traversal. Category = API 08:2019 - Injection.")]
    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    public async Task<IActionResult> GetImageFromLocal(string filename)
    {
        var userId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
        var user = await _accountRepository.GetByIdAsync(userId);

        var folderPath = _fileUploadPath + user.Name + "/";

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var memory = new MemoryStream();
        await using (var stream = new FileStream(folderPath + filename, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;
        return File(memory, "image/png", "download");
    }
}