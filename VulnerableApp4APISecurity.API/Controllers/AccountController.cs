using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp4APISecurity.Core.DTO.Account;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VulnerableApp4APISecurity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly JWTAuthManager _jwtAuthManager;

        public AccountController(AccountRepository accountRepository, JWTAuthManager jwtAuthManager)
        {
            _accountRepository = accountRepository;
            _jwtAuthManager = jwtAuthManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {

            if (request.Email is not null && request.Password is not null)
            {
                var account = await _accountRepository.GetAccountByEmailPassword(request.Email, request.Password);
                if (account is null)
                {
                    return BadRequest(new FailedResponse() { Message = "There is and error while login process. Please control your email or password" });
                }

                var token = _jwtAuthManager.GenerateTokens(account);

                return Ok(token);
            }

            return BadRequest(new FailedResponse() { Message = "Your email or password is empty. Please fill all and try again." });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> TemporaryLogin([FromQuery] LoginRequest request)
        {
            if (request.Email is not null && request.Password is not null)
            {
                var account = await _accountRepository.GetAccountByEmailPassword(request.Email, request.Password);
                if (account is null)
                {
                    return BadRequest(new FailedResponse() { Message = "There is and error while login process. Please control your email or password" });
                }

                var token = _jwtAuthManager.GenerateTokens(account);

                return Ok(token);
            }

            return BadRequest(new FailedResponse() { Message = "Your email or password is empty. Please fill all and try again." });

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] AccountEntity request)
        {

            if (request.Email is not null)
            {
                var account = await _accountRepository.GetAccountByEmail(request.Email);
                if (account != null)
                {
                    return BadRequest(new FailedResponse() { Message = "The email address which you provided is using another user." });
                }

                if (request.Role is null) { request.Role = "User"; }

                await _accountRepository.CreateAsync(request);

                AccountResponse accres = new AccountResponse
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                };

                return Ok(accres);
            }

            return BadRequest(new FailedResponse() { Message = "Your email is empty. Please fill all and try again." });
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> GetAccount([FromQuery] string email)
        {
            //email adresindeki kullanıcıları getir.
            var account = await _accountRepository.GetAccountByEmail(email);
            if (account is null)
            {
                return BadRequest(new FailedResponse { Message = "There is no profile using email address whhich provided" });
            }
            return Ok(new AccountResponse { Name = account.Name, Surname = account.Surname, Email = account.Email });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut]
        public async Task<ActionResult> UpdateNameSurnameAccount([FromBody] UpdateNameSurnameRequest request)
        {
            var email = _jwtAuthManager.TakeEmailFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var account = await _accountRepository.GetAccountByEmail(email);
            if (account.Id is null)
            {
                return BadRequest(new FailedResponse { Message = "There is an error while getting Acccount information" });
            }

            var updated = await _accountRepository.UpdateAsync(account.Id, new AccountEntity
            {
                Id = account.Id,
                Name = request.Name,
                Surname = request.Surname,
                Email = account.Email,
                Password = account.Password,
                CreatedAt = account.CreatedAt,
                Role = account.Role
            });
            return Ok(new SuccessResponse { Message = "Account's Name and Surname are updated" });
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> GetAccounts()
        {
            var account = await _accountRepository.GetAllAsync();
            if (account is null)
            {
                return BadRequest(new FailedResponse { Message = "There is no profile" });
            }
            List<AccountResponse> accountResponses = new List<AccountResponse>();
            foreach (var item in account)
            {
                accountResponses.Add(new AccountResponse { Name = item.Name, Surname = item.Surname, Email = item.Email });
            }
            return Ok(accountResponses);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAccount([FromQuery] string email)
        {
            var account = await _accountRepository.GetAccountByEmail(email);
            if (account is null)
            {
                return BadRequest(new FailedResponse { Message = "There is no account with start email address which you entered" });
            }
            var result = _accountRepository.DeleteAccountByEmail(email);
            return Ok(new SuccessResponse { Message = "User which you provided is deleted" });
        }
    }
}

