using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VulnerableApp4APISecurity.Core.DTO.Account;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Token;
using VulnerableApp4APISecurity.Core.DTO.RefreshToken;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Entities.RefreshToken;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.RefreshToken;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

namespace VulnerableApp4APISecurity.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : Controller
{
    private readonly AccountRepository _accountRepository;
    private readonly JWTAuthManager _jwtAuthManager;
    private readonly IMapper _mapper;
    private readonly RefreshTokenRepository _refreshTokenRepository;

    public AccountController(AccountRepository accountRepository, RefreshTokenRepository refreshTokenRepository,
        JWTAuthManager jwtAuthManager,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtAuthManager = jwtAuthManager;
        _mapper = mapper;
    }

    [SwaggerOperation(Summary = "Vulnerability = Multiple Request can be sent. " +
                                "Category = API 04:2019 - Lack of resources and rate limiting, API 02:2019 - Broken authentication")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        if (request.Email is null || request.Password is null)
            return BadRequest(new FailedResponse
                { Message = "Your email or password is empty. Please fill all and try again." });
        
        var account = await _accountRepository.GetAccountByEmailPassword(request.Email, request.Password);
        if (account is null)
            return BadRequest(new FailedResponse
            {
                Message = "There is an error while login process. Please control your email or password"
            });

        var token = _jwtAuthManager.GenerateToken(account);
        
        var getUserRefreshToken =
            await _refreshTokenRepository.GetAsync(userToken => userToken.email == request.Email);

        if (getUserRefreshToken is null)
        {
            var userRefreshToken = new CreateRefreshToken
            {
                email = account.Email,
                refreshToken = token.refreshToken
            };
            
            await _refreshTokenRepository.CreateAsync(_mapper.Map<RefreshTokenEntity>(userRefreshToken));
        }
        else
        {
            getUserRefreshToken.refreshToken = token.refreshToken;
            await _refreshTokenRepository.UpdateAsync(getUserRefreshToken.Id, getUserRefreshToken);
        }

        return Ok(token);

    }

    [SwaggerOperation(Summary = "Vulnerability = Multiple Request in Query String (Sensitive data is sending at URL). " +
                                "Category = API 04:2019 - Lack of resources and rate limiting, API 02:2019 - Broken authentication")]
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> TemporaryLogin([FromQuery] LoginRequest request)
    {
        if (request.Email is null || request.Password is null)
            return BadRequest(new FailedResponse
                { Message = "Your email or password is empty. Please fill all and try again." });
        
        var account = await _accountRepository.GetAccountByEmailPassword(request.Email, request.Password);
        if (account is null)
            return BadRequest(new FailedResponse
            {
                Message = "There is an error while login process. Please control your email or password"
            });

        var token = _jwtAuthManager.GenerateToken(account);
        
        var getUserRefreshToken =
            await _refreshTokenRepository.GetAsync(userToken => userToken.email == request.Email);

        if (getUserRefreshToken is null)
        {
            var userRefreshToken = new CreateRefreshToken
            {
                email = account.Email,
                refreshToken = token.refreshToken
            };
            
            await _refreshTokenRepository.CreateAsync(_mapper.Map<RefreshTokenEntity>(userRefreshToken));
        }
        else
        {
            getUserRefreshToken.refreshToken = token.refreshToken;
            await _refreshTokenRepository.UpdateAsync(getUserRefreshToken.Id, getUserRefreshToken);
        }

        return Ok(token);

    }

    [SwaggerOperation(Summary = "Registers a new user. Vulnerability: Previously Mass Assignment (API06:2019) & Email Detection (API02:2019), now improved. " +
                                "Still potential for Email Detection if not handled carefully. Injection (API08:2019) if DB interaction not parameterized (assumed OK via repo).")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        // Model validation will automatically trigger here due to [ApiController] and DTO attributes.
        // However, explicit checks can be added if needed or for non-DTO validation.
        if (!ModelState.IsValid)
        {
            return BadRequest(new FailedResponse { Message = "Validation failed.", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }

        // Specifically check for nulls if not covered by [Required] or for extra safety
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Surname))
        {
            return BadRequest(new FailedResponse { Message = "Name, Surname, Email, and Password are required." });
        }
       
        var existingAccount = await _accountRepository.GetAccountByEmail(request.Email);
        if (existingAccount is not null)
            return BadRequest(new FailedResponse
                { Message = "The email address which you provided is already in use." });

        var newAccountEntity = new AccountEntity
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Password = request.Password, // Assuming password hashing is done within AccountRepository.CreateAsync or a service layer
            Role = "User", // Explicitly set Role, ignore any potential Role from request
            CreatedAt = DateTime.UtcNow // Set creation date
        };

        // It's good practice for CreateAsync to hash the password if it's not already hashed.
        // For this example, we assume AccountRepository or its underlying logic handles password hashing.
        await _accountRepository.CreateAsync(newAccountEntity);

        // Don't return the created entity or its mapped version directly if it contains sensitive info not suitable for this response.
        // The original code returned a mapped version of 'account' which was the result of GetAccountByEmail (potentially null or existing).
        // It's better to return a generic success message or a carefully crafted DTO for the created account if needed.
        return Ok(new SuccessResponse() { Message = "Account created successfully."});

    }

    [SwaggerOperation(Summary = "This endpoint can be use when token is not working and replace new one sending refresh token.")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> RefreshToken([FromBody] RequestRefresToken request)
    {
        if (request.refreshToken is null)
            return BadRequest(new FailedResponse
                { Message = "Your refreshToken is empty. Please fill all and try again." });
        
        var refreshToken = await _refreshTokenRepository.GetAsync(refreshToken =>
            refreshToken.refreshToken == request.refreshToken);

        if (refreshToken is null)
            return BadRequest(new FailedResponse
            {
                Message = "There is no refreshToken which you entered."
            });

        var account = await _accountRepository.GetAccountByEmail(refreshToken.email);
        var token = _jwtAuthManager.GenerateToken(account);

        refreshToken.refreshToken = token.refreshToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken.Id, refreshToken);

        return Ok(token);

    }

    [SwaggerOperation(Summary = "This endpoint can be use either token is valid or not.")]
    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public ActionResult TokenStatus()
    {
        return Ok(new TokenStatus { status = "Token is valid" });
    }
}