using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Core.DTO.Profile;
using VulnerableApp4APISecurity.Core.Entities.Profile;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VulnerableApp4APISecurity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProfileController : Controller
    {

        private readonly ProfileRepository _profileRepository;
        private readonly JWTAuthManager _jwtAuthManager;

        public ProfileController(ProfileRepository profileRepository, JWTAuthManager jwtAuthManager)
        {
            _profileRepository = profileRepository;
            _jwtAuthManager = jwtAuthManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetProfile([FromQuery] ProfileSearchRequest request)
        {
            if (request.Email is not null) {
                var profile = await _profileRepository.GetProfileByEmail(request.Email);
                return Ok(profile);
            }
            return BadRequest(new FailedResponse() { Message = "Please enter Email and try again." });
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> CreateProfile([FromBody] ProfileRequest request)
        {
            var email = _jwtAuthManager.TakeEmailFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);

            var profile = await _profileRepository.GetProfileByEmail(email);
            if (profile is null)
            {
                var prof = await _profileRepository.CreateAsync(new ProfileEntity
                {
                    Email = email,
                    Address = request.Address,
                    Birthday = request.Birthday,
                    Hobby = request.Hobby
                });

                return Ok(new ProfileResponse { Address = prof.Address, Birthday = prof.Birthday, Hobby = prof.Hobby });
            }

            return BadRequest(new FailedResponse() { Message = "Profile is already created." });
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> UpdateProfile([FromBody] ProfileRequest request)
        {
            var email = _jwtAuthManager.TakeEmailFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var profile = await _profileRepository.GetProfileByEmail(email);
            if (profile.Id is not null)
            {
                await _profileRepository.UpdateAsync(profile.Id, new ProfileEntity
                {
                    Id = profile.Id,
                    Address = request.Address,
                    Birthday = request.Birthday,
                    Hobby = request.Hobby,
                    Email = email,
                });
                return Ok(request);
            }

            return BadRequest(new FailedResponse() { Message = "Profile is not created yet. This process can be done after profile is created." });

        }

        [HttpDelete]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteProfile([FromQuery] ProfileSearchRequest request)
        {

            if (request.Email is not null)
            {
                var profile = await _profileRepository.GetProfileByEmail(request.Email);

                if (profile is not null && profile.Id is not null)
                {
                    await _profileRepository.DeleteAsync(profile.Id);

                    return Ok(new SuccessResponse { Message = "Profile is deleted" });
                }

                return BadRequest(new FailedResponse() { Message = "Profile is not created yet. This process can be done after profile is created." });
            }

            return BadRequest(new FailedResponse() { Message = "Please enter Email information and try again your request." });

        }
    }
}

