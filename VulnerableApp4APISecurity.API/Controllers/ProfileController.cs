using AutoMapper;
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
        private readonly IMapper _mapper;


        public ProfileController(ProfileRepository profileRepository, JWTAuthManager jwtAuthManager, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _jwtAuthManager = jwtAuthManager;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetProfile([FromQuery] ProfileSearchRequest request)
        {
            if (request.Email is not null)
            {
                var profile = await _profileRepository.GetProfileByEmail(request.Email);

                if (profile is not null)
                {
                    return Ok(_mapper.Map<ProfileResponse>(profile));

                }

                return BadRequest(new FailedResponse() { Message = "There is no Profile." });

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
                var profil = _mapper.Map<ProfileEntity>(request);
                profil.Email = email;

                var prof = await _profileRepository.CreateAsync(profil);

                return Ok(_mapper.Map<ProfileResponse>(prof));
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
                var profil = _mapper.Map<ProfileEntity>(request);
                profil.Email = email;
                profil.Id = profile.Id;
                  
                await _profileRepository.UpdateAsync(profile.Id,profil);

                return Ok(_mapper.Map<ProfileResponse>(request));
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

